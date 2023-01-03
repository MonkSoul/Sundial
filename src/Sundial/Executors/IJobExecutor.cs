﻿// MIT License
//
// Copyright (c) 2020-2023 百小僧, Baiqian Co.,Ltd and Contributors
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
/// 作业处理程序执行器
/// </summary>
public interface IJobExecutor
{
    /// <summary>
    /// 执行作业处理程序
    /// </summary>
    /// <remarks>在这里可以实现超时控制，失败重试控制等等</remarks>
    /// <param name="context">作业处理程序执行前上下文</param>
    /// <param name="jobHandler">作业处理程序</param>
    /// <param name="stoppingToken">取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    Task ExecuteAsync(JobExecutingContext context, IJob jobHandler, CancellationToken stoppingToken);
}