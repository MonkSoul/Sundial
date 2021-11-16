// Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
// Sundial is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//             https://gitee.com/dotnetchina/Sundial/blob/master/LICENSE
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

namespace Sundial
{
    /// <summary>
    /// 作业执行方式
    /// </summary>
    public enum JobMode
    {
        /// <summary>
        /// 并行执行
        /// </summary>
        /// <remarks>无需等待上一次任务完成，默认值</remarks>
        Parallel = 0,

        /// <summary>
        /// 串行执行
        /// </summary>
        /// <remarks>需等待上一次任务完成</remarks>
        Serial = 1
    }
}