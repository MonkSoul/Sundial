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
    /// 作业状态
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        /// 未开始或取消
        /// </summary>
        None = 0,

        /// <summary>
        /// 正常
        /// </summary>
        /// <remarks>初始值</remarks>
        Normal = 1,

        /// <summary>
        /// 暂停
        /// </summary>
        Pause = 2,

        /// <summary>
        /// 完成
        /// </summary>
        Complete = 3,

        /// <summary>
        /// 出错
        /// </summary>
        Error = 4,

        /// <summary>
        /// 阻塞
        /// </summary>
        Blocked = 5
    }
}