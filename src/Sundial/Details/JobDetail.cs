// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Sundial is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Sundial/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;

namespace Sundial
{
    /// <summary>
    /// 作业详细信息默认实现
    /// </summary>
    public sealed class JobDetail : IJobDetail
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="identity">作业唯一标识</param>
        internal JobDetail(string identity)
        {
            Identity = identity;
        }

        /// <summary>
        /// 唯一标识
        /// </summary>
        public string Identity { get; }

        /// <summary>
        /// 作业描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 作业状态
        /// </summary>
        public JobStatus Status { get; set; } = JobStatus.Normal;

        /// <summary>
        /// 作业执行方式
        /// </summary>
        public JobMode Mode { get; set; } = JobMode.Parallel;

        /// <summary>
        /// 最近运行时间
        /// </summary>
        public DateTime? LastRunTime { get; set; }

        /// <summary>
        /// 下一次运行时间
        /// </summary>
        public DateTime? NextRunTime { get; set; }

        /// <summary>
        /// 运行次数
        /// </summary>
        public long NumberOfRuns { get; set; } = 0;
    }
}