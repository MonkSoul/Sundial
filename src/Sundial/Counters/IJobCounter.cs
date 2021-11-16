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
    /// 作业计数器
    /// </summary>
    /// <remarks>主要用来避免频繁从 <see cref="IJobDetail"/> 获取，同时在内存中存有一份</remarks>
    public interface IJobCounter
    {
        /// <summary>
        /// 最近运行时间
        /// </summary>
        DateTime LastRunTime { get; internal set; }

        /// <summary>
        /// 下一次运行时间
        /// </summary>
        DateTime NextRunTime { get; internal set; }

        /// <summary>
        /// 运行次数
        /// </summary>
        long NumberOfRuns { get; internal set; }
    }
}