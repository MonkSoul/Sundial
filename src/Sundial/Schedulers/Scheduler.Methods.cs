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
/// 作业计划
/// </summary>
internal sealed partial class Scheduler
{
    /// <summary>
    /// 返回可公开访问的作业计划模型
    /// </summary>
    /// <remarks>常用于接口返回或序列化操作</remarks>
    /// <returns><see cref="SchedulerModel"/></returns>
    public SchedulerModel GetModel()
    {
        return new SchedulerModel
        {
            JobDetail = JobDetail,
            Triggers = Triggers.Values.ToArray()
        };
    }

    /// <summary>
    /// 获取作业计划构建器
    /// </summary>
    /// <returns><see cref="SchedulerBuilder"/></returns>
    public SchedulerBuilder GetBuilder()
    {
        return SchedulerBuilder.From(this);
    }

    /// <summary>
    /// 获取作业计划信息构建器
    /// </summary>
    /// <returns><see cref="JobBuilder"/></returns>
    public JobBuilder GetJobBuilder()
    {
        return GetBuilder().GetJobBuilder();
    }

    /// <summary>
    /// 获取作业计划触发器构建器集合
    /// </summary>
    /// <returns><see cref="List{TriggerBuilder}"/></returns>
    public List<TriggerBuilder> GetTriggerBuilders()
    {
        return GetBuilder().GetTriggerBuilders();
    }

    /// <summary>
    /// 获取作业计划触发器构建器
    /// </summary>
    /// <param name="triggerId">作业触发器 Id</param>
    /// <returns><see cref="TriggerBuilder"/></returns>
    public TriggerBuilder GetTriggerBuilder(string triggerId)
    {
        return GetBuilder().GetTriggerBuilder(triggerId);
    }

    /// <summary>
    /// 启动作业计划
    /// </summary>
    public void Start()
    {
        var changed = 0;

        // 逐条启用所有作业触发器
        foreach (var triggerId in Triggers.Keys)
        {
            if (StartTrigger(triggerId, false)) changed++;
        }

        // 取消作业调度器休眠状态（强制唤醒）
        if (changed > 0) Factory.CancelSleep();
    }

    /// <summary>
    /// 暂停作业计划
    /// </summary>
    public void Pause()
    {
        var changed = 0;

        // 逐条暂停所有作业触发器
        foreach (var triggerId in Triggers.Keys)
        {
            if (PauseTrigger(triggerId, false)) changed++;
        }

        // 取消作业调度器休眠状态（强制唤醒）
        if (changed > 0) Factory.CancelSleep();
    }

    /// <summary>
    /// 启动作业计划单个触发器
    /// </summary>
    /// <param name="triggerId">作业触发器 Id</param>
    /// <param name="immediately">使作业调度器立即载入</param>
    /// <returns><see cref="bool"/></returns>
    public bool StartTrigger(string triggerId, bool immediately = true)
    {
        // 获取作业触发器构建器
        var schedulerBuilder = GetBuilder();
        var triggerBuilder = schedulerBuilder.GetTriggerBuilder(triggerId);

        // 空检查
        if (triggerBuilder == null)
        {
            // 输出日志
            Logger.LogWarning("The <{triggerId}> trigger for scheduler of <{jobId}> is not found.", triggerId, JobId);

            return false;
        }

        triggerBuilder.StartNow = true;
        triggerBuilder.SetStatus(TriggerStatus.Ready);

        // 更新作业触发器构建器
        schedulerBuilder.UpdateTriggerBuilder(triggerBuilder);

        // 更新作业
        var scheduleResult = Factory.TryUpdateJob(schedulerBuilder, out _, immediately);
        if (scheduleResult != ScheduleResult.Succeed)
        {
            return false;
        }

        // 输出日志
        Logger.LogInformation("The <{triggerId}> trigger for scheduler of <{JobId}> successfully started to the schedule.", triggerId, JobId);

        return true;
    }

    /// <summary>
    /// 暂停作业计划单个触发器
    /// </summary>
    /// <param name="triggerId">作业触发器 Id</param>
    /// <param name="immediately">使作业调度器立即载入</param>
    /// <returns><see cref="bool"/></returns>
    public bool PauseTrigger(string triggerId, bool immediately = true)
    {
        // 获取作业触发器构建器
        var schedulerBuilder = GetBuilder();
        var triggerBuilder = schedulerBuilder.GetTriggerBuilder(triggerId);

        // 空检查
        if (triggerBuilder == null)
        {
            // 输出日志
            Logger.LogWarning("The <{triggerId}> trigger for scheduler of <{jobId}> is not found.", triggerId, JobId);

            return false;
        }

        triggerBuilder.SetStatus(TriggerStatus.Pause);

        // 更新作业触发器构建器
        schedulerBuilder.UpdateTriggerBuilder(triggerBuilder);

        // 更新作业
        var scheduleResult = Factory.TryUpdateJob(schedulerBuilder, out _, immediately);
        if (scheduleResult != ScheduleResult.Succeed)
        {
            return false;
        }

        // 输出日志
        Logger.LogInformation("The <{triggerId}> trigger for scheduler of <{JobId}> successfully paused to the schedule.", triggerId, JobId);

        return true;
    }

    /// <summary>
    /// 更新作业计划信息
    /// </summary>
    /// <param name="jobBuilder">作业信息构建器</param>
    /// <param name="jobDetail">作业信息</param>
    /// <returns><see cref="ScheduleResult"/></returns>
    public ScheduleResult TryUpdateDetail(JobBuilder jobBuilder, out JobDetail jobDetail)
    {
        // 空检查
        if (jobBuilder == null) throw new ArgumentNullException(nameof(jobBuilder));

        // 获取作业信息构建器
        var schedulerBuilder = GetBuilder();
        schedulerBuilder.UpdateJobBuilder(jobBuilder);

        // 更新作业
        var scheduleResult = Factory.TryUpdateJob(schedulerBuilder, out var scheduler);
        if (scheduleResult != ScheduleResult.Succeed)
        {
            jobDetail = null;
            return scheduleResult;
        }

        jobDetail = ((Scheduler)scheduler).JobDetail;
        return scheduleResult;
    }

    /// <summary>
    /// 更新作业计划信息
    /// </summary>
    /// <param name="jobBuilder">作业信息构建器</param>
    public void UpdateDetail(JobBuilder jobBuilder)
    {
        _ = TryUpdateDetail(jobBuilder, out _);
    }

    /// <summary>
    /// 查找作业计划触发器
    /// </summary>
    /// <param name="triggerId">作业触发器 Id</param>
    /// <param name="trigger">作业触发器</param>
    /// <returns><see cref="ScheduleResult"/></returns>
    public ScheduleResult TryGetTrigger(string triggerId, out Trigger trigger)
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(triggerId)) throw new ArgumentNullException(nameof(triggerId));

        var succeed = Triggers.TryGetValue(triggerId, out var internalTrigger);
        trigger = internalTrigger;

        return succeed
            ? ScheduleResult.Succeed
            : ScheduleResult.NotFound;
    }

    /// <summary>
    /// 查找作业计划触发器
    /// </summary>
    /// <param name="triggerId">作业触发器 Id</param>
    /// <returns><see cref="Trigger"/></returns>
    public Trigger GetTrigger(string triggerId)
    {
        _ = TryGetTrigger(triggerId, out var trigger);
        return trigger;
    }

    /// <summary>
    /// 添加作业计划触发器
    /// </summary>
    /// <param name="triggerBuilder">作业触发器构建器</param>
    /// <param name="trigger">作业触发器</param>
    /// <returns><see cref="ScheduleResult"/></returns>
    public ScheduleResult TryAddTrigger(TriggerBuilder triggerBuilder, out Trigger trigger)
    {
        // 空检查
        if (triggerBuilder == null) throw new ArgumentNullException(nameof(triggerBuilder));

        // 添加作业触发器构建器
        var schedulerBuilder = GetBuilder();
        schedulerBuilder.AddTriggerBuilder(triggerBuilder);

        // 更新作业
        var scheduleResult = Factory.TryUpdateJob(schedulerBuilder, out var scheduler);
        if (scheduleResult != ScheduleResult.Succeed)
        {
            trigger = null;
            return scheduleResult;
        }

        // 返回新的作业触发器
        trigger = scheduler.GetTrigger(triggerBuilder.TriggerId);
        return scheduleResult;
    }

    /// <summary>
    /// 添加作业计划触发器
    /// </summary>
    /// <param name="triggerBuilder">作业触发器构建器</param>
    public void AddTrigger(TriggerBuilder triggerBuilder)
    {
        _ = TryAddTrigger(triggerBuilder, out _);
    }

    /// <summary>
    /// 更新作业计划触发器
    /// </summary>
    /// <param name="triggerBuilder">作业触发器构建器</param>
    /// <param name="trigger">作业触发器</param>
    /// <returns><see cref="ScheduleResult"/></returns>
    public ScheduleResult TryUpdateTrigger(TriggerBuilder triggerBuilder, out Trigger trigger)
    {
        // 空检查
        if (triggerBuilder == null) throw new ArgumentNullException(nameof(triggerBuilder));

        // 更新作业触发器构建器
        var schedulerBuilder = GetBuilder();
        schedulerBuilder.UpdateTriggerBuilder(triggerBuilder);

        // 更新作业
        var scheduleResult = Factory.TryUpdateJob(schedulerBuilder, out var scheduler);
        if (scheduleResult != ScheduleResult.Succeed)
        {
            trigger = null;
            return scheduleResult;
        }

        // 返回更新后的作业触发器
        trigger = scheduler.GetTrigger(triggerBuilder.TriggerId);
        return scheduleResult;
    }

    /// <summary>
    /// 更新作业计划触发器
    /// </summary>
    /// <param name="triggerBuilder">作业触发器构建器</param>
    public void UpdateTrigger(TriggerBuilder triggerBuilder)
    {
        _ = TryUpdateTrigger(triggerBuilder, out _);
    }

    /// <summary>
    /// 删除作业计划触发器
    /// </summary>
    /// <param name="triggerId">作业触发器 Id</param>
    /// <param name="trigger">作业触发器</param>
    /// <returns><see cref="ScheduleResult"/></returns>
    public ScheduleResult TryRemoveTrigger(string triggerId, out Trigger trigger)
    {
        // 删除作业触发器构建器
        var schedulerBuilder = GetBuilder();
        schedulerBuilder.RemoveTriggerBuilder(triggerId, out var triggerBuilder);

        // 空检查
        if (triggerBuilder == null)
        {
            // 输出日志
            Logger.LogWarning("The <{triggerId}> trigger for scheduler of <{jobId}> is not found.", triggerId, JobId);

            trigger = default;
            return ScheduleResult.NotFound;
        }

        // 更新作业
        var scheduleResult = Factory.TryUpdateJob(schedulerBuilder, out _);
        if (scheduleResult != ScheduleResult.Succeed)
        {
            trigger = null;
            return scheduleResult;
        }

        // 返回删除后的作业触发器
        trigger = triggerBuilder.Build(JobId);
        return scheduleResult;
    }

    /// <summary>
    /// 删除作业计划触发器
    /// </summary>
    /// <param name="triggerId">作业触发器 Id</param>
    public void RemoveTrigger(string triggerId)
    {
        _ = TryRemoveTrigger(triggerId, out _);
    }

    /// <summary>
    /// 检查作业计划触发器是否存在
    /// </summary>
    /// <param name="triggerId">作业触发器 Id</param>
    /// <returns><see cref="bool"/></returns>
    public bool ContainsTrigger(string triggerId)
    {
        return TryGetTrigger(triggerId, out _) == ScheduleResult.Succeed;
    }

    /// <summary>
    /// 将当前作业计划从调度器中删除
    /// </summary>
    /// <remarks><see cref="ScheduleResult"/></remarks>
    public ScheduleResult TryRemove()
    {
        return Factory.TryRemoveJob(this);
    }

    /// <summary>
    /// 将当前作业计划从调度器中删除
    /// </summary>
    public void Remove()
    {
        _ = TryRemove();
    }

    /// <summary>
    /// 强制触发作业持久化记录
    /// </summary>
    public void Persist()
    {
        // 将作业信息运行数据写入持久化
        Factory.Shorthand(JobDetail);

        // 逐条将作业触发器运行数据写入持久化
        foreach (var (_, trigger) in Triggers)
        {
            Factory.Shorthand(JobDetail, trigger);
        }
    }

    /// <summary>
    /// 转换成 JSON 字符串
    /// </summary>
    /// <param name="naming">命名法</param>
    /// <returns><see cref="string"/></returns>
    public string ConvertToJSON(NamingConventions naming = NamingConventions.CamelCase)
    {
        return GetBuilder().ConvertToJSON(naming);
    }
}