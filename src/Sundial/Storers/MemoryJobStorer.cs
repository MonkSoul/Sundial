// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Sundial is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Sundial/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Sundial
{
    /// <summary>
    /// 基于内存作业存储器（默认实现）
    /// </summary>
    internal sealed class MemoryJobStorer : IJobStorer
    {
        /// <summary>
        /// 作业存储集合
        /// </summary>
        private readonly ConcurrentDictionary<string, IJobDetail> _jobData = new();

        /// <summary>
        /// 注册作业
        /// </summary>
        /// <param name="identity"></param>
        public void Register(string identity)
        {
            _jobData.TryAdd(identity, new JobDetail(identity)
            {
                Status = JobStatus.Normal
            });
        }

        /// <summary>
        /// 根据作业标识获取作业详细信息
        /// </summary>
        /// <param name="identity">唯一标识</param>
        /// <param name="cancellationToken">取消任务 Token</param>
        /// <returns><see cref="IJobDetail"/> 实例</returns>
        public Task<IJobDetail> GetAsync(string identity, CancellationToken cancellationToken)
        {
            var isExist = _jobData.TryGetValue(identity, out var jobDetail);
            return Task.FromResult(isExist ? jobDetail! : default!);
        }

        /// <summary>
        /// 更新作业详细信息
        /// </summary>
        /// <param name="detail">作业详细信息</param>
        /// <param name="cancellationToken">取消任务 Token</param>
        /// <returns><see cref="Task"/> 实例</returns>
        public Task UpdateAsync(IJobDetail detail, CancellationToken cancellationToken)
        {
            _jobData.TryUpdate(detail.Identity, detail, detail);

            return Task.CompletedTask;
        }
    }
}