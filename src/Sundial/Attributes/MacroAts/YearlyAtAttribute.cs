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

using TimeCrontab;

namespace Sundial;

/// <summary>
/// 每年特定月1号（午夜）开始作业触发器特性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class YearlyAtAttribute : CronAttribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public YearlyAtAttribute(params int[] fields)
        : base($"0 0 1 {string.Join(',', fields)} *", CronStringFormat.Default)
    {
        // 空检查
        if (fields == null || fields.Length == 0) throw new ArgumentNullException(nameof(fields));
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public YearlyAtAttribute(params string[] fields)
        : base($"0 0 1 {string.Join(',', fields)} *", CronStringFormat.Default)
    {
        // 空检查
        if (fields == null || fields.Length == 0) throw new ArgumentNullException(nameof(fields));
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public YearlyAtAttribute(params object[] fields)
        : base($"0 0 1 {string.Join(',', fields)} *", CronStringFormat.Default)
    {
        // 空检查
        if (fields == null || fields.Length == 0) throw new ArgumentNullException(nameof(fields));
        // 检查 fields 只能是 int 和字符串类型
        if (fields.Any(f => f.GetType() != typeof(int) && f.GetType() != typeof(string))) throw new InvalidOperationException("Invalid Cron expression.");
    }
}