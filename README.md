# Sundial

[![license](https://img.shields.io/badge/license-MIT-orange?cacheSeconds=10800)](https://gitee.com/dotnetchina/Sundial/blob/master/LICENSE) [![nuget](https://img.shields.io/nuget/v/Sundial.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Sundial) [![dotNET China](https://img.shields.io/badge/organization-dotNET%20China-yellow?cacheSeconds=10800)](https://gitee.com/dotnetchina)

.NET 功能齐全的开源分布式作业调度系统，可从最小的应用程序到大型企业系统使用。

![Sundial.drawio](https://gitee.com/dotnetchina/Sundial/raw/master/drawio/Sundial.drawio.png "Sundial.drawio.png")

## 特性

- 简化调度作业
  - 支持作业监视器
  - 支持作业执行器
  - 支持自定义作业存储组件（持久化）
  - 支持自定义策略执行
  - 内置周期、Cron 作业
  - 支持自定义作业触发器
  - 支持依赖注入控制（含 HTTP 控制支持）
- 高内聚，低耦合，使代码更简单
- 很小，仅 `41KB`
- 无第三方依赖
- 可在 `Windows/Linux/MacOS` 守护进程部署
- 支持分布式、集群（多实例）部署
- 支持负载均衡（基于 TCP/IP）
- 高质量代码和良好单元测试
- 跨平台，支持 .NET5+

## 安装

- [Package Manager](https://www.nuget.org/packages/Sundial)

```powershell
Install-Package Sundial
```

- [.NET CLI](https://www.nuget.org/packages/Sundial)

```powershell
dotnet add package Sundial
```

## 快速入门

我们在[主页](./samples)上有不少例子，这是让您入门的第一个：

1. 定义作业，并实现 `IJob` 接口：

```cs
public class MyJob : IJob
{
    private readonly ILogger<MyJob> _logger;

    public MyJob(ILogger<MyJob> logger)
    {
        _logger = logger;
    }

    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{context.JobDetail}  {context.Trigger} {context.OccurrenceTime}");
        await Task.CompletedTask;
    }
}
```

2. 在 `Startup.cs` 注册 `AddSchedule` 服务和作业：

```cs
services.AddSchedule(options =>
{
    options.AddJob<MyJob>(Triggers.PeriodSeconds(5)
                , Triggers.Minutely());
});
```

> ASP.NET 6/7 版本，无 `Startup.cs` 文件，可直接在 `Program.cs` 文件中注册：

```cs
builder.Services.AddSchedule(options =>
{
    options.AddJob<MyJob>(Triggers.PeriodSeconds(5)
                , Triggers.Minutely());
});
```

3. 运行项目：

```bash
info: System.Logging.ScheduleService[0]
      Schedule Hosted Service is running.
warn: System.Logging.ScheduleService[0]
      Schedule Hosted Service cancels hibernation and GC.Collect().
info: System.Logging.ScheduleService[0]
      The Scheduler of <job1> successfully updated to the schedule.
info: Sundial.Samples.MyJob[0]
      <job1>   <job1 job1_trigger2>  5000ms 2022/11/23 16:22:51
info: Sundial.Samples.MyJob[0]
      <job1>   <job1 job1_trigger2>  5000ms 2022/11/23 16:22:56
info: Sundial.Samples.MyJob[0]
      <job1>   <job1 job1_trigger3>  * * * * * 2022/11/23 16:23:00
info: Sundial.Samples.MyJob[0]
      <job1>   <job1 job1_trigger2>  5000ms 2022/11/23 16:23:01
info: Sundial.Samples.MyJob[0]
      <job1>   <job1 job1_trigger2>  5000ms 2022/11/23 16:23:07
```

[更多文档](https://furion.baiqian.ltd/docs/job/)

## 文档

您可以在[主页](https://furion.baiqian.ltd/docs/job/)找到 Sundial 文档。

## 贡献

该存储库的主要目的是继续发展 Sundial 核心，使其更快、更易于使用。Sundial 的开发在 [Gitee](https://gitee.com/dotnetchina/Sundial) 上公开进行，我们感谢社区贡献错误修复和改进。

## 许可证

Sundial 采用 [MIT](./LICENSE) 开源许可证。

```
MIT License

Copyright (c) 2020-2022 百小僧, Baiqian Co.,Ltd.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
