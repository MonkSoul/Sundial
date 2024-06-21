// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

namespace Sundial;

/// <summary>
/// 作业执行记录事件参数
/// </summary>
public sealed class JobExecutionRecordEventArgs : EventArgs
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="context">作业执行记录持久上下文</param>
    public JobExecutionRecordEventArgs(PersistenceExecutionRecordContext context)
    {
        Context = context;
    }

    /// <summary>
    /// 作业执行记录持久上下文
    /// </summary>
    public PersistenceExecutionRecordContext Context { get; }
}