﻿// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Sundial;

/// <summary>
/// 毫秒周期（间隔）作业触发器
/// </summary>
public class PeriodTrigger : Trigger
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="interval">间隔（毫秒）</param>
    public PeriodTrigger(long interval)
    {
        // 最低运行毫秒数为 100ms
        if (interval < 100) throw new InvalidOperationException($"The interval cannot be less than 100ms, but the value is <{interval}ms>.");

        Interval = interval;
    }

    /// <summary>
    /// 间隔（毫秒）
    /// </summary>
    protected long Interval { get; }

    /// <summary>
    /// 计算下一个触发时间
    /// </summary>
    /// <param name="startAt">起始时间</param>
    /// <returns><see cref="DateTime"/></returns>
    public override DateTime GetNextOccurrence(DateTime startAt)
    {
        // 获取当前时间
        var nowTime = Penetrates.GetNowTime(ScheduleOptionsBuilder.UseUtcTimestampProperty);

        // 处理作业触发器积压状态
        if (startAt >= nowTime)
        {
            return startAt.AddMilliseconds(Interval);
        }

        // 获取间隔触发器周期计算基准时间
        var baseTime = StartTime == null ? nowTime : StartTime.Value;

        // 获取从起始时间开始到现在经过了多少个完整周期
        var elapsedMilliseconds = (nowTime - baseTime).Ticks / TimeSpan.TicksPerMillisecond;
        var fullPeriods = elapsedMilliseconds / Interval;

        // 获取下一次执行时间
        var nextRunTime = baseTime.AddMilliseconds(fullPeriods * Interval);

        // 确保下一次执行时间是在当前时间之后
        if (nowTime >= nextRunTime)
        {
            nextRunTime = nextRunTime.AddMilliseconds(Interval);
        }

        return nextRunTime;
    }

    /// <summary>
    /// 作业触发器转字符串输出
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        return $"<{JobId} {TriggerId}> {GetUnit()}{(string.IsNullOrWhiteSpace(Description) ? string.Empty : $" {Description.GetMaxLengthString()}")} {NumberOfRuns}ts";
    }

    /// <summary>
    /// 计算间隔单位
    /// </summary>
    /// <returns></returns>
    private string GetUnit()
    {
        // 毫秒
        if (Interval < 1000) return $"{Interval}ms";
        // 秒
        if (Interval >= 1000 && Interval < 1000 * 60 && Interval % 1000 == 0) return $"{Interval / 1000}s";
        // 分钟
        if (Interval >= 1000 * 60 && Interval < 1000 * 60 * 60 && Interval % (1000 * 60) == 0) return $"{Interval / (1000 * 60)}min";
        // 小时
        if (Interval >= 1000 * 60 * 60 && Interval < 1000 * 60 * 60 * 24 && Interval % (1000 * 60 * 60) == 0) return $"{Interval / (1000 * 60 * 60)}h";

        return $"{Interval}ms";
    }
}