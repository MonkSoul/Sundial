// 版权归百小僧及百签科技（广东）有限公司所有。
//
// 此源代码遵循位于源代码树根目录中的 LICENSE 文件的许可证。

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using System.Reflection.Metadata;

namespace Sundial;

/// <summary>
/// 作业调度器静态类
/// </summary>
public static class Schedular
{
    /// <summary>
    /// 序列化对象
    /// </summary>
    /// <remarks>主要用于作业触发器参数，作业信息额外数据序列化</remarks>
    /// <param name="obj">对象</param>
    /// <returns><see cref="string"/></returns>
    public static string Serialize(object obj)
    {
        return Penetrates.Serialize(obj);
    }

    /// <summary>
    /// 反序列化对象
    /// </summary>
    /// <remarks>主要用于作业触发器参数，作业信息额外数据序列化</remarks>
    /// <param name="json">JSON 字符串</param>
    /// <returns>T</returns>
    public static T Deserialize<T>(string json)
    {
        return Penetrates.Deserialize<T>(json);
    }

    /// <summary>
    /// 编译 C# 类定义代码
    /// </summary>
    /// <param name="csharpCode">字符串代码</param>
    /// <param name="assemblyName">自定义程序集名称</param>
    /// <param name="additionalAssemblies">附加的程序集</param>
    /// <returns><see cref="Assembly"/></returns>
    public static Assembly CompileCSharpClassCode(string csharpCode, string assemblyName = default, params Assembly[] additionalAssemblies)
    {
        // 空检查
        if (csharpCode == null) throw new ArgumentNullException(nameof(csharpCode));

        // 合并程序集
        var domainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        var references = assemblyName != null && additionalAssemblies.Length > 0
            ? domainAssemblies.Concat(additionalAssemblies)
            : domainAssemblies;

        // 生成语法树
        var syntaxTree = CSharpSyntaxTree.ParseText(csharpCode);

        // 创建 C# 编译器
        var compilation = CSharpCompilation.Create(
          string.IsNullOrWhiteSpace(assemblyName) ? Path.GetRandomFileName() : assemblyName.Trim(),
          new[]
          {
                    syntaxTree
          },
          references.Where(ass =>
          {
              unsafe
              {
                  return ass.TryGetRawMetadata(out var blob, out var length);
              }
          }).Select(ass =>
          {
              // MetadataReference.CreateFromFile(ass.Location)
              unsafe
              {
                  ass.TryGetRawMetadata(out var blob, out var length);
                  var moduleMetadata = ModuleMetadata.CreateFromMetadata((IntPtr)blob, length);
                  var assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
                  var metadataReference = assemblyMetadata.GetReference();
                  return metadataReference;
              }
          }),
          new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        // 编译代码
        using var memoryStream = new MemoryStream();
        var emitResult = compilation.Emit(memoryStream);

        // 编译失败抛出异常
        if (!emitResult.Success)
        {
            throw new InvalidOperationException($"Unable to compile class code: {string.Join("\n", emitResult.Diagnostics.ToList().Where(w => w.IsWarningAsError || w.Severity == DiagnosticSeverity.Error))}");
        }

        memoryStream.Position = 0;

        // 返回编译程序集
        return Assembly.Load(memoryStream.ToArray());
    }
}