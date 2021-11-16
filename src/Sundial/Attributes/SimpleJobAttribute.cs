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
    /// 周期作业特性
    /// </summary>
    /// <remarks>该特性是调度作业模块内置的特性，主要用来解析并创建 <see cref="SimpleTrigger"/> 触发器</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class SimpleJobAttribute : JobAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="identity">作业唯一标识</param>
        /// <param name="interval">间隔时间（毫秒）</param>
        public SimpleJobAttribute(string identity, int interval)
            : base(identity)
        {
            // 有效值检查
            if (interval <= 0)
            {
                throw new InvalidOperationException("The <interval> must be greater than 0.");
            }

            Interval = interval;
        }

        /// <summary>
        /// 间隔时间（毫秒）
        /// </summary>
        public int Interval { get; }
    }
}