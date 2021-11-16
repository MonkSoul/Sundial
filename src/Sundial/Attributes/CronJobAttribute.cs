// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Sundial is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Sundial/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TimeCrontab;

namespace Sundial
{
    /// <summary>
    /// Cron 作业特性
    /// </summary>
    /// <remarks>该特性是调度作业模块内置的特性，主要用来解析并创建 <see cref="CronTrigger"/> 触发器</remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class CronJobAttribute : JobAttribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="identity">作业唯一标识</param>
        /// <param name="schedule">调度计划（Cron 表达式）</param>
        public CronJobAttribute(string identity, string schedule)
            : base(identity)
        {
            // 空检查
            if (string.IsNullOrWhiteSpace(schedule))
            {
                throw new InvalidOperationException("The <schedule> can be not null or empty.");
            }

            Schedule = schedule;
        }

        /// <summary>
        /// 调度计划（Cron 表达式）
        /// </summary>
        public string Schedule { get; }

        /// <summary>
        /// Cron 表达式格式化类型
        /// </summary>
        public CronStringFormat Format { get; set; } = CronStringFormat.Default;
    }
}