// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Sundial is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Sundial/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sundial
{
    /// <summary>
    /// 调度作业默认实现
    /// </summary>
    internal sealed class SchedulerJob : ISchedulerJob
    {
        /// <summary>
        /// 作业存储器
        /// </summary>
        private readonly IJobStorer _jobStorer;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="jobStorer">作业存储器</param>
        public SchedulerJob(IJobStorer jobStorer)
        {
            _jobStorer = jobStorer;
        }

        /// <summary>
        /// 开始作业
        /// </summary>
        /// <param name="identity">作业唯一标识</param>
        /// <param name="cancellationToken"> 取消任务 Token</param>
        /// <returns><see cref="Task"/> 实例</returns>
        public async Task StartAsync(string identity, CancellationToken cancellationToken = default)
        {
            await UpdateStatusAsync(identity, JobStatus.Normal, cancellationToken);
        }

        /// <summary>
        /// 暂停作业
        /// </summary>
        /// <param name="identity">作业唯一标识</param>
        /// <param name="cancellationToken"> 取消任务 Token</param>
        /// <returns><see cref="Task"/> 实例</returns>
        public async Task PauseAsync(string identity, CancellationToken cancellationToken = default)
        {
            await UpdateStatusAsync(identity, JobStatus.Pause, cancellationToken);
        }

        /// <summary>
        /// 更新作业状态
        /// </summary>
        /// <param name="identity">作业唯一标识</param>
        /// <param name="status"><see cref="JobStatus"/></param>
        /// <param name="cancellationToken"> 取消任务 Token</param>
        /// <returns><see cref="Task"/> 实例</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private async Task UpdateStatusAsync(string identity, JobStatus status, CancellationToken cancellationToken = default)
        {
            var jobDetail = await _jobStorer.GetAsync(identity, cancellationToken);
            if (jobDetail == null)
            {
                throw new InvalidOperationException($"The <{identity}> job detail not found.");
            }

            jobDetail.Status = status;
            await _jobStorer.UpdateAsync(jobDetail, cancellationToken);
        }
    }
}