// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Sundial is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Sundial/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sundial
{
    /// <summary>
    /// 作业调度器
    /// </summary>
    /// <remarks>每一个 <see cref="IJob"/> 都有一个对应的 Scheduler 调度器</remarks>
    internal sealed class JobScheduler : BackgroundService
    {
        /// <summary>
        /// 避免由 CLR 的终结器捕获该异常从而终止应用程序，让所有未觉察异常被觉察
        /// </summary>
        internal event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

        /// <summary>
        /// 日志对象
        /// </summary>
        private readonly ILogger<JobScheduler> _logger;

        /// <summary>
        /// 作业标识器
        /// </summary>
        private string Identity { get; }

        /// <summary>
        /// 作业执行程序
        /// </summary>
        private IJob Job { get; }

        /// <summary>
        /// 作业触发器
        /// </summary>
        private IJobTrigger Trigger { get; }

        /// <summary>
        /// 作业存储器
        /// </summary>
        private IJobStorer Storer { get; }

        /// <summary>
        /// 作业监视器
        /// </summary>
        private IJobMonitor Monitor { get; }

        /// <summary>
        /// 作业执行器
        /// </summary>
        private IJobExecutor Executor { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志对象</param>
        /// <param name="serviceProvider">服务提供器</param>
        /// <param name="storer">作业存储器</param>
        /// <param name="job">作业执行程序</param>
        /// <param name="identity">作业唯一标识</param>
        /// <param name="trigger">作业触发器</param>
        public JobScheduler(ILogger<JobScheduler> logger
            , IServiceProvider serviceProvider
            , IJobStorer storer
            , IJob job
            , string identity
            , IJobTrigger trigger)
        {
            _logger = logger;
            Identity = identity;
            Storer = storer;
            Job = job;
            Trigger = trigger;
            Storer = serviceProvider.GetRequiredService<IJobStorer>();
            Monitor = serviceProvider.GetService<IJobMonitor>();
            Executor = serviceProvider.GetService<IJobExecutor>();
        }

        /// <summary>
        /// 执行后台任务
        /// </summary>
        /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
        /// <returns><see cref="Task"/> 实例</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Scheduler of <{Identity}> Service is running.", Identity);

            // 调度器服务停止监听
            stoppingToken.Register(() =>
                _logger.LogDebug("Scheduler of <{Identity}> Service is stopping.", Identity));

            // 监听调度器服务是否取消
            while (!stoppingToken.IsCancellationRequested)
            {
                // 执行具体作业
                await BackgroundProcessing(stoppingToken);

                // 在指定速率内检查
                await Task.Delay(Trigger.Rates, stoppingToken);
            }

            // 如果作业调度器停止，则更新作业状态为 None
            var jobDetail = await Storer.GetAsync(Identity, default);
            if (jobDetail != null && jobDetail.Status != JobStatus.None)
            {
                jobDetail.Status = JobStatus.None;
                jobDetail.NextRunTime = null;
                await Storer.UpdateAsync(jobDetail, default);
            }

            _logger.LogCritical("Scheduler of <{Identity}> Service is stopped.", Identity);
        }

        /// <summary>
        /// 后台调用作业处理程序
        /// </summary>
        /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
        /// <returns><see cref="Task"/> 实例</returns>
        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            var referenceTime = DateTime.UtcNow;

            // 判断作业是否符合触发时机
            if (!Trigger.ShouldRun(Identity, referenceTime))
            {
                return;
            }

            // 查询当前作业信息
            var jobDetail = await Storer.GetAsync(Identity, stoppingToken);

            // 如果无法查询作业信息或作业状态标识为 None 或 Pause 则跳过执行
            if (jobDetail == default || jobDetail.Status == JobStatus.None || jobDetail!.Status == JobStatus.Pause)
            {
                return;
            }

            // 如果作业采用串行执行，则只有非 Blocked 时才可往下执行
            if (jobDetail.Mode == JobMode.Serial && jobDetail.Status == JobStatus.Blocked)
            {
                return;
            }

            // 创建一个任务工厂并保证作业处理程序使用当前的计划程序
            var taskFactory = new TaskFactory(TaskScheduler.Current);

            // 计算下一个触发时机
            Trigger.Increment();

            // 创建新的线程执行
            await taskFactory.StartNew(async () =>
            {
                // 创建共享上下文数据对象
                var properties = new Dictionary<object, object>();

                // 创建执行前上下文
                var jobExecutingContext = new JobExecutingContext(jobDetail, properties)
                {
                    ExecutingTime = DateTime.UtcNow
                };

                // 执行异常对象
                InvalidOperationException executionException = default;

                try
                {
                    // 调用执行前监视器
                    if (Monitor != default)
                    {
                        await Monitor.OnExecutingAsync(jobExecutingContext, stoppingToken);
                    }

                    // 更新作业信息
                    jobDetail.Status = JobStatus.Blocked;
                    jobDetail.NumberOfRuns = Trigger.NumberOfRuns;
                    jobDetail.LastRunTime = Trigger.LastRunTime;
                    jobDetail.NextRunTime = Trigger.NextRunTime;
                    await Storer.UpdateAsync(jobDetail, stoppingToken);

                    // 判断是否自定义了执行器
                    if (Executor == default)
                    {
                        await Job.ExecuteAsync(jobExecutingContext, stoppingToken);
                    }
                    else
                    {
                        await Executor.ExecuteAsync(jobExecutingContext, Job, stoppingToken);
                    }

                    jobDetail.Status = JobStatus.Complete;
                }
                catch (Exception ex)
                {
                    jobDetail.Status = JobStatus.Error;

                    // 输出异常日志
                    _logger.LogError(ex, "Error occurred executing of <{Identity}>.", Identity);

                    // 标记异常
                    executionException = new InvalidOperationException(string.Format("Error occurred executing <{0}>.", Identity), ex);

                    // 捕获 Task 任务异常信息并统计所有异常
                    if (UnobservedTaskException != default)
                    {
                        var args = new UnobservedTaskExceptionEventArgs(
                            ex as AggregateException ?? new AggregateException(ex));

                        UnobservedTaskException.Invoke(this, args);
                    }
                }
                finally
                {
                    await Storer.UpdateAsync(jobDetail, stoppingToken);

                    // 调用执行后监视器
                    if (Monitor != default)
                    {
                        // 创建执行后上下文
                        var jobExecutedContext = new JobExecutedContext(jobDetail, properties)
                        {
                            ExecutedTime = DateTime.UtcNow,
                            Exception = executionException
                        };

                        await Monitor.OnExecutedAsync(jobExecutedContext, stoppingToken);
                    }
                }
            }, stoppingToken);
        }
    }
}