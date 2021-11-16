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
    /// 作业触发器基类
    /// </summary>
    public abstract class JobTriggerBase
    {
        /// <summary>
        /// 最近运行时间
        /// </summary>
        public DateTime LastRunTime { get; set; }

        /// <summary>
        /// 下一次运行时间
        /// </summary>
        public DateTime NextRunTime { get; set; }

        /// <summary>
        /// 运行次数
        /// </summary>
        public long NumberOfRuns { get; set; }
    }
}