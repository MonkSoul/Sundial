// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Sundial is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Sundial/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System.Threading;
using System.Threading.Tasks;

namespace Sundial
{
    /// <summary>
    /// 作业监视器
    /// </summary>
    public interface IJobMonitor
    {
        /// <summary>
        /// 作业执行前监视
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="cancellationToken">取消任务 Token</param>
        /// <returns><see cref="Task"/> 实例</returns>
        Task OnExecutingAsync(JobExecutingContext context, CancellationToken cancellationToken);

        /// <summary>
        /// 作业执行后监视
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="cancellationToken">取消任务 Token</param>
        /// <returns><see cref="Task"/> 实例</returns>
        Task OnExecutedAsync(JobExecutedContext context, CancellationToken cancellationToken);
    }
}