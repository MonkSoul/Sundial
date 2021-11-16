// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Sundial is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Sundial/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TimeCrontab;

namespace Sundial
{
    /// <summary>
    /// 调度作业配置选项构建器
    /// </summary>
    public sealed class SundialOptionsBuilder
    {
        /// <summary>
        /// 作业类型集合
        /// </summary>
        private readonly Dictionary<Type, (string, IJobTrigger)> _jobs = new();

        /// <summary>
        /// 作业存储器实现工厂
        /// </summary>
        private Func<IServiceProvider, IJobStorer> _jobStorerImplementationFactory;

        /// <summary>
        /// 作业监视器
        /// </summary>
        private Type _jobMonitor;

        /// <summary>
        /// 作业执行器
        /// </summary>
        private Type _jobExecutor;

        /// <summary>
        /// 未察觉任务异常处理程序
        /// </summary>
        public EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskExceptionHandler { get; set; }

        /// <summary>
        /// 注册作业
        /// </summary>
        /// <typeparam name="TJob"><see cref="IJob"/> 实例</typeparam>
        /// <param name="jobTrigger">作业触发器</param>
        /// <returns><see cref="SundialOptionsBuilder"/> 实例</returns>
        public SundialOptionsBuilder AddJob<TJob>(IJobTrigger jobTrigger = default)
            where TJob : class, IJob
        {
            return AddJob(typeof(TJob), jobTrigger);
        }

        /// <summary>
        /// 注册作业
        /// </summary>
        /// <param name="jobType">作业类型，必须实现 <see cref="IJob"/> 接口</param>
        /// <param name="jobTrigger">作业触发器</param>
        /// <returns><see cref="SundialOptionsBuilder"/> 实例</returns>
        public SundialOptionsBuilder AddJob(Type jobType, IJobTrigger jobTrigger = default)
        {
            // jobType 须实现 IJob 接口
            if (!typeof(IJob).IsAssignableFrom(jobType))
            {
                throw new InvalidOperationException("The <jobType> does not implement <IJob> interface.");
            }

            // 判断是否贴有 [Job] 或其派生类特性
            if (!jobType.IsDefined(typeof(JobAttribute), false))
            {
                throw new InvalidOperationException("The [Job] attribute is not added to the current job.");
            }

            // 获取 [Job] 特性具体类型
            var jobAttribute = jobType.GetCustomAttribute<JobAttribute>(false)!;

            // 创建作业标识器
            var identity = jobAttribute.Identity;

            // 创建作业触发器
            IJobTrigger trigger;
            if (jobTrigger != default)
            {
                trigger = jobTrigger;
            }
            else
            {
                // 解析 Cron 触发器
                if (jobAttribute is CronJobAttribute cronJobAttribute)
                {
                    // 解析速率，支持秒的 Cron 表达式速率为 1秒，否则速率为 1分钟
                    var rates = cronJobAttribute.Format == CronStringFormat.WithSeconds || cronJobAttribute.Format == CronStringFormat.WithSecondsAndYears
                        ? TimeSpan.FromSeconds(1)
                        : TimeSpan.FromMinutes(1);

                    trigger = new CronTrigger(rates, Crontab.Parse(cronJobAttribute.Schedule, cronJobAttribute.Format))
                    {
                        NextRunTime = DateTime.UtcNow
                    };
                }
                // 解析周期触发器
                else if (jobAttribute is SimpleJobAttribute simpleJobAttribute)
                {
                    // 解析速率
                    var rates = TimeSpan.FromMilliseconds(simpleJobAttribute.Interval);

                    trigger = new SimpleTrigger(rates)
                    {
                        NextRunTime = DateTime.UtcNow
                    };
                }
                else
                {
                    throw new InvalidOperationException("Job trigger not found.");
                }
            }

            // 禁止重复注册作业
            if (!_jobs.TryAdd(jobType, (identity, trigger)))
            {
                throw new InvalidOperationException("The job has been registered. Repeated registration is prohibited.");
            }

            return this;
        }

        /// <summary>
        /// 替换作业存储器
        /// </summary>
        /// <param name="implementationFactory">自定义作业存储器工厂</param>
        /// <returns><see cref="SundialOptionsBuilder"/> 实例</returns>
        public SundialOptionsBuilder ReplaceStorer(Func<IServiceProvider, IJobStorer> implementationFactory)
        {
            _jobStorerImplementationFactory = implementationFactory;
            return this;
        }

        /// <summary>
        /// 注册作业监视器
        /// </summary>
        /// <typeparam name="TJobMonitor">实现自 <see cref="IJobMonitor"/></typeparam>
        /// <returns><see cref="SundialOptionsBuilder"/> 实例</returns>
        public SundialOptionsBuilder AddMonitor<TJobMonitor>()
            where TJobMonitor : class, IJobMonitor
        {
            _jobMonitor = typeof(TJobMonitor);
            return this;
        }

        /// <summary>
        /// 注册作业执行器
        /// </summary>
        /// <typeparam name="TJobExecutor">实现自 <see cref="IJobExecutor"/></typeparam>
        /// <returns><see cref="SundialOptionsBuilder"/> 实例</returns>
        public SundialOptionsBuilder AddExecutor<TJobExecutor>()
            where TJobExecutor : class, IJobExecutor
        {
            _jobExecutor = typeof(TJobExecutor);
            return this;
        }

        /// <summary>
        /// 构建调度作业配置选项
        /// </summary>
        /// <param name="services">服务集合对象</param>
        internal void Build(IServiceCollection services)
        {
            // 注册作业
            foreach (var (jobType, (identity, trigger)) in _jobs)
            {
                AddJob(services, jobType, identity, trigger);
            }

            // 替换作业存储器
            if (_jobStorerImplementationFactory != default)
            {
                services.Replace(ServiceDescriptor.Singleton(_jobStorerImplementationFactory));
            }

            // 注册作业监视器
            if (_jobMonitor != default)
            {
                services.AddSingleton(typeof(IJobMonitor), _jobMonitor);
            }

            // 注册作业执行器
            if (_jobExecutor != default)
            {
                services.AddSingleton(typeof(IJobExecutor), _jobExecutor);
            }
        }

        /// <summary>
        /// 注册作业
        /// </summary>
        /// <param name="services">服务集合对象</param>
        /// <param name="jobType">作业类型</param>
        /// <param name="identity">作业唯一标识</param>
        /// <param name="trigger">作业触发器</param>
        /// <exception cref="InvalidOperationException"></exception>
        private void AddJob(IServiceCollection services, Type jobType, string identity, IJobTrigger trigger)
        {
            // 将作业注册为单例
            services.AddSingleton(jobType);

            // 注册作业调度器
            services.Add(ServiceDescriptor.Singleton<IHostedService>(serviceProvider =>
            {
                // 获取作业存储器
                var jobStorer = serviceProvider.GetRequiredService<IJobStorer>();
                jobStorer.Register(identity);

                // 创建作业调度器
                var jobScheduler = new JobScheduler(
                    serviceProvider.GetRequiredService<ILogger<JobScheduler>>()
                    , serviceProvider
                    , jobStorer
                    , (serviceProvider.GetRequiredService(jobType) as IJob)!
                    , identity
                    , trigger);

                // 订阅未察觉任务异常事件
                var unobservedTaskExceptionHandler = UnobservedTaskExceptionHandler;
                if (unobservedTaskExceptionHandler != default)
                {
                    jobScheduler.UnobservedTaskException += unobservedTaskExceptionHandler;
                }

                return jobScheduler;
            }));
        }
    }
}