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

using System.Reflection;

namespace Sundial;

/// <summary>
/// 作业信息构建器
/// </summary>
public sealed class JobBuilder : JobDetail
{
    /// <summary>
    /// 构造函数
    /// </summary>
    private JobBuilder()
    {
    }

    /// <summary>
    /// 创建作业信息构建器
    /// </summary>
    /// <typeparam name="TJob"><see cref="IJob"/> 实现类型</typeparam>
    /// <returns><see cref="JobBuilder"/></returns>
    public static JobBuilder Create<TJob>()
        where TJob : class, IJob
    {
        return Create(typeof(TJob));
    }

    /// <summary>
    /// 创建作业信息构建器
    /// </summary>
    /// <param name="assemblyName">作业类型所在程序集 Name</param>
    /// <param name="jobTypeFullName">作业类型 FullName</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public static JobBuilder Create(string assemblyName, string jobTypeFullName)
    {
        return new JobBuilder()
            .SetJobType(assemblyName, jobTypeFullName);
    }

    /// <summary>
    /// 创建作业信息构建器
    /// </summary>
    /// <param name="jobType">作业类型</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public static JobBuilder Create(Type jobType)
    {
        return new JobBuilder()
            .SetJobType(jobType);
    }

    /// <summary>
    /// 将 <see cref="JobDetail"/> 转换成 <see cref="JobBuilder"/>
    /// </summary>
    /// <param name="jobDetail">作业信息</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public static JobBuilder From(JobDetail jobDetail)
    {
        var jobBuilder = jobDetail.MapTo<JobBuilder>();

        // 初始化运行时作业类型和额外数据
        jobBuilder.SetJobType(jobBuilder.AssemblyName, jobBuilder.JobType)
            .SetProperties(jobBuilder.Properties);

        return jobBuilder;
    }

    /// <summary>
    /// 将 JSON 字符串转换成 <see cref="JobBuilder"/>
    /// </summary>
    /// <param name="json">JSON 字符串</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public static JobBuilder From(string json)
    {
        return From(Penetrates.Deserialize<JobDetail>(json));
    }

    /// <summary>
    /// 克隆作业信息构建器
    /// </summary>
    /// <param name="fromJobBuilder">被克隆的作业信息构建器</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public static JobBuilder Clone(JobBuilder fromJobBuilder)
    {
        return Create(fromJobBuilder.AssemblyName, fromJobBuilder.JobType)
                     .SetGroupName(fromJobBuilder.GroupName)
                     .SetDescription(fromJobBuilder.Description)
                     .SetConcurrent(fromJobBuilder.Concurrent)
                     .SetIncludeAnnotations(fromJobBuilder.IncludeAnnotations)
                     .SetProperties(fromJobBuilder.Properties);
    }

    /// <summary>
    /// 从目标值填充到作业信息构建器
    /// </summary>
    /// <param name="value">目标值</param>
    /// <param name="ignoreNullValue">忽略空值</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public JobBuilder LoadFrom(object value, bool ignoreNullValue = false)
    {
        if (value == null) return this;

        // 排除枚举类型，接口类型，数组类型，值类型
        var valueType = value.GetType();
        if (valueType.IsInterface
            || valueType.IsValueType
            || valueType.IsEnum
            || valueType.IsArray) throw new InvalidOperationException(nameof(value));

        var jobBuilder = value.MapTo<JobBuilder>(this, ignoreNullValue);

        // 初始化运行时作业类型和额外数据
        jobBuilder.SetJobType(jobBuilder.AssemblyName, jobBuilder.JobType)
            .SetProperties(jobBuilder.Properties);

        return jobBuilder;
    }

    /// <summary>
    /// 设置作业 Id
    /// </summary>
    /// <param name="jobId">作业 Id</param>
    /// <returns><see cref="JobBuilder"/></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public JobBuilder SetJobId(string jobId)
    {
        JobId = jobId;

        return this;
    }

    /// <summary>
    /// 设置作业组名称
    /// </summary>
    /// <param name="groupName">作业组名称</param>
    /// <returns><see cref="JobBuilder"/></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public JobBuilder SetGroupName(string groupName)
    {
        GroupName = groupName;

        return this;
    }

    /// <summary>
    /// 设置作业类型
    /// </summary>
    /// <param name="assemblyName">作业类型所在程序集 Name</param>
    /// <param name="jobTypeFullName">作业类型 FullName</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public JobBuilder SetJobType(string assemblyName, string jobTypeFullName)
    {
        AssemblyName = assemblyName;
        JobType = jobTypeFullName;

        // 只有 assemblyName 和 jobTypeFullName 同时存在才创建类型
        if (!string.IsNullOrWhiteSpace(assemblyName)
            && !string.IsNullOrWhiteSpace(jobTypeFullName))
        {
            // 加载 GAC 全局应用程序缓存中的程序集及类型
            var jobType = Assembly.Load(assemblyName)
                .GetType(jobTypeFullName);

            return SetJobType(jobType);
        }

        return this;
    }

    /// <summary>
    /// 设置作业类型
    /// </summary>
    /// <typeparam name="TJob"><see cref="IJob"/> 实现类类型</typeparam>
    /// <returns><see cref="JobBuilder"/></returns>
    public JobBuilder SetJobType<TJob>()
        where TJob : IJob
    {
        return SetJobType(typeof(TJob));
    }

    /// <summary>
    /// 设置作业类型
    /// </summary>
    /// <param name="jobType">作业类型</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public JobBuilder SetJobType(Type jobType)
    {
        // 不做 null 检查
        if (jobType == null) return this;

        // 检查 jobType 类型是否实现 IJob 接口
        if (!jobType.IsJobType()) throw new InvalidOperationException("The <jobType> does not implement IJob interface.");

        AssemblyName = jobType.Assembly.GetName().Name;
        JobType = jobType.FullName;
        RuntimeJobType = jobType;

        return this;
    }

    /// <summary>
    /// 设置描述信息
    /// </summary>
    /// <param name="description">描述信息</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public JobBuilder SetDescription(string description)
    {
        Description = description;

        return this;
    }

    /// <summary>
    /// 设置是否采用并发执行
    /// </summary>
    /// <param name="concurrent">是否并发执行</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public JobBuilder SetConcurrent(bool concurrent)
    {
        Concurrent = concurrent;

        return this;
    }

    /// <summary>
    /// 设置是否扫描 IJob 实现类 [Trigger] 特性触发器
    /// </summary>
    /// <param name="includeAnnotations">是否扫描 IJob 实现类 [Trigger] 特性触发器</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public JobBuilder SetIncludeAnnotations(bool includeAnnotations)
    {
        IncludeAnnotations = includeAnnotations;

        return this;
    }

    /// <summary>
    /// 设置作业信息额外数据
    /// </summary>
    /// <param name="properties">作业信息额外数据</param>
    /// <remarks>必须是 Dictionary{string, object} 类型序列化的结果</remarks>
    /// <returns><see cref="JobBuilder"/></returns>
    public JobBuilder SetProperties(string properties)
    {
        if (string.IsNullOrWhiteSpace(properties)) properties = "{}";

        Properties = properties;
        var jsonDictionary = Penetrates.Deserialize<Dictionary<string, object>>(properties);

        // 解决反序列化 object 类型被转换成了 JsonElement 类型
        var newDictionary = new Dictionary<string, object>(jsonDictionary.Count);
        foreach (var key in jsonDictionary.Keys)
        {
            newDictionary[key] = Penetrates.GetJsonElementValue(jsonDictionary[key]);
        }

        RuntimeProperties = newDictionary;

        return this;
    }

    /// <summary>
    /// 设置作业信息额外数据
    /// </summary>
    /// <param name="properties">作业信息额外数据</param>
    /// <remarks>必须是 Dictionary{string, object} 类型序列化的结果</remarks>
    /// <returns><see cref="JobBuilder"/></returns>
    public JobBuilder SetProperties(Dictionary<string, object> properties)
    {
        properties ??= new();

        Properties = Penetrates.Serialize(properties);
        RuntimeProperties = properties;

        return this;
    }

    /// <summary>
    /// 添加作业信息额外数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public new JobBuilder AddProperty(string key, object value)
    {
        return base.AddProperty(key, value) as JobBuilder;
    }

    /// <summary>
    /// 添加或更新作业信息额外数据
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public new JobBuilder AddOrUpdateProperty(string key, object value)
    {
        return base.AddOrUpdateProperty(key, value) as JobBuilder;
    }

    /// <summary>
    /// 删除作业信息额外数据
    /// </summary>
    /// <param name="key">键</param>
    /// <returns><see cref="JobBuilder"/></returns>
    public new JobBuilder RemoveProperty(string key)
    {
        return base.RemoveProperty(key) as JobBuilder;
    }

    /// <summary>
    /// 清空作业信息额外数据
    /// </summary>
    /// <returns><see cref="JobBuilder"/></returns>
    public new JobBuilder ClearProperties()
    {
        return base.ClearProperties() as JobBuilder;
    }

    /// <summary>
    /// 构建 <see cref="JobDetail"/> 对象
    /// </summary>
    /// <returns><see cref="JobDetail"/></returns>
    internal JobDetail Build()
    {
        // 空检查
        if (string.IsNullOrWhiteSpace(JobId)) throw new ArgumentNullException(nameof(JobId));

        // 检查类型
        if (!string.IsNullOrWhiteSpace(AssemblyName)
            && !string.IsNullOrWhiteSpace(JobType)
            && RuntimeJobType == null) SetJobType(AssemblyName, JobType);

        return this.MapTo<JobDetail>();
    }
}