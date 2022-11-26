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
/// 作业信息持久化上下文
/// </summary>
public class PersistenceContext
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="jobDetail">作业信息</param>
    /// <param name="behavior">作业持久化行为</param>
    internal PersistenceContext(JobDetail jobDetail
        , PersistenceBehavior behavior)
    {
        JobId = jobDetail.JobId;
        JobDetail = jobDetail;
        Behavior = behavior;
    }

    /// <summary>
    /// 作业 Id
    /// </summary>
    public string JobId { get; }

    /// <summary>
    /// 作业信息
    /// </summary>
    public JobDetail JobDetail { get; }

    /// <summary>
    /// 作业持久化行为
    /// </summary>
    public PersistenceBehavior Behavior { get; }

    /// <summary>
    /// 转换成 Sql 语句
    /// </summary>
    /// <param name="tableName">数据库表名</param>
    /// <param name="naming">命名法</param>
    /// <returns><see cref="string"/></returns>
    public string ConvertToSQL(string tableName, NamingConventions naming = NamingConventions.CamelCase)
    {
        return JobDetail.ConvertToSQL(tableName, Behavior, naming);
    }

    /// <summary>
    /// 转换成 JSON 语句
    /// </summary>
    /// <param name="naming">命名法</param>
    /// <returns><see cref="string"/></returns>
    public string ConvertToJSON(NamingConventions naming = NamingConventions.CamelCase)
    {
        return JobDetail.ConvertToJSON(naming);
    }

    /// <summary>
    /// 转换成 Monitor 字符串
    /// </summary>
    /// <param name="naming">命名法</param>
    /// <returns><see cref="string"/></returns>
    public string ConvertToMonitor(NamingConventions naming = NamingConventions.CamelCase)
    {
        return JobDetail.ConvertToMonitor(naming);
    }

    /// <summary>
    /// 根据不同的命名法返回属性名
    /// </summary>
    /// <param name="propertyName">属性名</param>
    /// <param name="naming">命名法</param>
    /// <returns><see cref="string"/></returns>
    public string GetNaming(string propertyName, NamingConventions naming = NamingConventions.CamelCase)
    {
        // 空检查
        if (!string.IsNullOrWhiteSpace(propertyName)) return propertyName;

        return Penetrates.GetNaming(propertyName, naming);
    }

    /// <summary>
    /// 作业信息持久化上下文转字符串输出
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public override string ToString()
    {
        return $"{JobDetail} <{Behavior}>";
    }
}