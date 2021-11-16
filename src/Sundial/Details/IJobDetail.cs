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
    /// 作业详细信息
    /// </summary>
    public interface IJobDetail
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        string Identity { get; }

        /// <summary>
        /// 作业描述
        /// </summary>
        string Description { get; internal set; }

        /// <summary>
        /// 作业状态
        /// </summary>
        JobStatus Status { get; internal set; }

        /// <summary>
        /// 作业执行方式
        /// </summary>
        JobMode Mode { get; internal set; }

        /// <summary>
        /// 最近运行时间
        /// </summary>
        DateTime? LastRunTime { get; internal set; }

        /// <summary>
        /// 下一次运行时间
        /// </summary>
        DateTime? NextRunTime { get; internal set; }

        /// <summary>
        /// 运行次数
        /// </summary>
        long NumberOfRuns { get; internal set; }
    }
}