// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using System.Text;
using System.Text.RegularExpressions;

namespace Sundial;

/// <summary>
/// 模板静态类
/// </summary>
internal static class TP
{
    /// <summary>
    /// 模板正则表达式对象
    /// </summary>
    private static readonly Lazy<Regex> _lazyRegex = new(() => new(@"^##(?<prop>.*)?##[:：]?\s*(?<content>[\s\S]*)"));

    /// <summary>
    /// 生成规范日志模板
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="description">描述</param>
    /// <param name="items">列表项，如果以 ##xxx## 开头，自动生成 xxx: 属性</param>
    /// <returns><see cref="string"/></returns>
    internal static string Wrapper(string title, string description, params string[] items)
    {
        // 处理不同编码问题
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"┏━━━━━━━━━━━  {title} ━━━━━━━━━━━").AppendLine();

        // 添加描述
        if (!string.IsNullOrWhiteSpace(description))
        {
            stringBuilder.Append($"┣ {description}").AppendLine().Append("┣ ").AppendLine();
        }

        if (items != null && items.Length > 0)
        {
            var propMaxLength = items.Where(u => _lazyRegex.Value.IsMatch(u))
                .DefaultIfEmpty(string.Empty)
                .Max(u => _lazyRegex.Value.Match(u).Groups["prop"].Value.Length);
            propMaxLength += (propMaxLength >= 5 ? 10 : 5);

            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];
                if (_lazyRegex.Value.IsMatch(item))
                {
                    var match = _lazyRegex.Value.Match(item);
                    var prop = match.Groups["prop"].Value;
                    var content = match.Groups["content"].Value;

                    var propTitle = $"{prop}：";
                    stringBuilder.Append($"┣ {PadRight(propTitle, propMaxLength)}{content}").AppendLine();
                }
                else
                {
                    stringBuilder.Append($"┣ {item}").AppendLine();
                }
            }
        }

        stringBuilder.Append($"┗━━━━━━━━━━━  {title} ━━━━━━━━━━━");
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 等宽文字对齐
    /// </summary>
    /// <param name="str"></param>
    /// <param name="totalByteCount"></param>
    /// <returns></returns>
    private static string PadRight(string str, int totalByteCount)
    {
        var coding = Encoding.GetEncoding("gbk");
        var dcount = 0;

        foreach (var ch in str.ToCharArray())
        {
            if (coding.GetByteCount(ch.ToString()) == 2)
                dcount++;
        }

        var w = str.PadRight(totalByteCount - dcount);
        return w;
    }
}