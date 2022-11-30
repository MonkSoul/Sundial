﻿// MIT License
//
// Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd and Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

namespace Sundial;

/// <summary>
/// 作业计划工厂服务（内部服务）
/// </summary>
public partial interface ISchedulerFactory
{
    /// <summary>
    /// 作业调度器初始化
    /// </summary>
    void Preload();

    /// <summary>
    /// 查找即将触发的作业
    /// </summary>
    /// <param name="startAt">起始时间</param>
    /// <param name="group">作业组名称</param>
    /// <returns><see cref="IEnumerable{IScheduler}"/></returns>
    IEnumerable<IScheduler> GetCurrentRunJobs(DateTime startAt, string group = default);

    /// <summary>
    /// 查找即将触发的作业并转换成 <see cref="SchedulerModel"/>
    /// </summary>
    /// <param name="startAt">起始时间</param>
    /// <param name="group">作业组名称</param>
    /// <returns><see cref="IEnumerable{SchedulerModel}"/></returns>
    IEnumerable<SchedulerModel> GetCurrentRunJobsOfModels(DateTime startAt, string group = default);

    /// <summary>
    /// 使作业调度器进入休眠状态
    /// </summary>
    /// <param name="startAt">起始时间</param>
    Task SleepAsync(DateTime startAt);

    /// <summary>
    /// 取消作业调度器休眠状态（强制唤醒）
    /// </summary>
    void CancelSleep();

    /// <summary>
    /// 将作业信息运行数据写入持久化
    /// </summary>
    /// <param name="jobDetail">作业信息</param>
    /// <param name="behavior">作业持久化行为</param>
    void Shorthand(JobDetail jobDetail, PersistenceBehavior behavior = PersistenceBehavior.Updated);

    /// <summary>
    /// 将作业触发器运行数据写入持久化
    /// </summary>
    /// <param name="jobDetail">作业信息</param>
    /// <param name="trigger">作业触发器</param>
    /// <param name="behavior">作业持久化行为</param>
    void Shorthand(JobDetail jobDetail, Trigger trigger, PersistenceBehavior behavior = PersistenceBehavior.Updated);
}