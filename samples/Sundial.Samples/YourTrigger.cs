using System;

namespace Sundial.Samples
{
    public class YourTrigger : JobTriggerBase, IJobTrigger
    {
        public YourTrigger(TimeSpan rates)
        {
            Rates = rates;
        }

        /// <summary>
        /// 调度器检查频率
        /// </summary>
        public TimeSpan Rates { get; }

        /// <summary>
        /// 计算下一次执行时间
        /// </summary>
        public void Increment()
        {
            NumberOfRuns++;
            LastRunTime = NextRunTime;
            NextRunTime = NextRunTime.AddSeconds(2);
        }

        /// <summary>
        /// 触发时机
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        public bool ShouldRun(string identity, DateTime currentTime)
        {
            return NextRunTime < currentTime && LastRunTime != NextRunTime && currentTime.Second % 2 == 0;
        }
    }
}
