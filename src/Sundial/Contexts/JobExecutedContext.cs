// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Sundial is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Sundial/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Collections.Generic;

namespace Sundial
{
    /// <summary>
    /// 作业执行后上下文
    /// </summary>
    public sealed class JobExecutedContext : JobExecutionContext
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="jobDetail">作业详细信息</param>
        /// <param name="properties">共享上下文数据</param>
        internal JobExecutedContext(IJobDetail jobDetail, IDictionary<object, object> properties)
            : base(jobDetail, properties)
        {
        }

        /// <summary>
        /// 执行后时间
        /// </summary>
        public DateTime ExecutedTime { get; internal set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public InvalidOperationException Exception { get; internal set; }
    }
}