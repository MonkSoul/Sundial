# Sundial ([重构中](https://gitee.com/dotnetchina/Sundial/issues/I4IL3U))

[![license](https://img.shields.io/badge/license-MulanPSL--2.0-orange?cacheSeconds=10800)](https://gitee.com/dotnetchina/Sundial/blob/master/LICENSE) [![nuget](https://img.shields.io/nuget/v/Sundial.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Sundial) [![dotNET China](https://img.shields.io/badge/organization-dotNET%20China-yellow?cacheSeconds=10800)](https://gitee.com/dotnetchina)

.NET 功能齐全的开源作业调度系统，可从最小的应用程序到大型企业系统使用。

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
- 很小，仅 `17KB`
- 无第三方依赖
- 可在 `Windows/Linux/MacOS` 守护进程部署
- 支持分布式、集群（多实例）部署
- 支持负载均衡（基于 TCP/IP）
- 高质量代码和良好单元测试

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

**周期作业**：固定时间执行作业，如 `1秒`。

```cs
[SimpleJob("simple_job", 1000)]
public class SimpleJob : IJob
{
    private readonly ILogger<SimpleJob> _logger;
    public SimpleJob(ILogger<SimpleJob> logger)
    {
        _logger = logger;
    }

    public Task ExecuteAsync(JobExecutingContext context, CancellationToken cancellationToken)
    {
        // Do your job!!!
        _logger.LogInformation("<{JobName}> {DateTime}", context.JobDetail.Identity, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        return Task.CompletedTask;
    }
}
```

**Cron 作业**：支持完整 `Cron 表达式` 精确控制。

```cs
[CronJob("cron_job", "* * * * *")]
public class CronJob : IJob
{
    private readonly ILogger<CronJob> _logger;
    public CronJob(ILogger<CronJob> logger)
    {
        _logger = logger;
    }

    public Task ExecuteAsync(JobExecutingContext context, CancellationToken cancellationToken)
    {
        // // Do your job!!!
        _logger.LogInformation("<{JobName}> {DateTime}", context.JobDetail.Identity, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        return Task.CompletedTask;
    }
}
```

2. 在 `Startup.cs` 注册 `AddSundial` 服务和作业：

```cs
services.AddSundial(builder =>
{
    builder.AddJob<SimpleJob>();
    builder.AddJob<CronJob>();
});
```

3. 运行项目：

```bash
info: Sundial.JobScheduler[0]
      Scheduler of <simple_job> Service is running.
info: Sundial.JobScheduler[0]
      Scheduler of <cron_job> Service is running.
info: Sundial.Samples.SimpleJob[0]
      <simple_job> 2021-11-16 11:15:45
info: Sundial.Samples.CronJob[0]
      <cron_job> 2021-11-16 11:15:45
      <simple_job> 2021-11-16 11:15:46
info: Sundial.Samples.SimpleJob[0]
```

4. 结合 API 调度作业

```cs
public class ApiController: ControllerBase
{
    private readonly IScheduler _scheduler;
    public ApiController(IScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    // 暂停
    public async Task PauseAsync(string jobName)
    {
        await _scheduler.PauseAsync(jobName);
    }

    // 开始
    public async Task StartAsync(string jobName)
    {
        await _scheduler.StartAsync(jobName);
    }

    // ......
}
```

[更多文档](./docs)

## 文档

您可以在[主页](./docs)找到 Sundial 文档。

## 贡献

该存储库的主要目的是继续发展 Sundial 核心，使其更快、更易于使用。Sundial 的开发在 [Gitee](https://gitee.com/dotnetchina/Sundial) 上公开进行，我们感谢社区贡献错误修复和改进。

## 许可证

Sundial 采用 [MulanPSL-2.0](./LICENSE) 开源许可证。

```
Copyright (c) 2020-2021 百小僧, Baiqian Co.,Ltd.
Sundial is licensed under Mulan PSL v2.
You can use this software according to the terms andconditions of the Mulan PSL v2.
You may obtain a copy of Mulan PSL v2 at:
            https://gitee.com/dotnetchina/Sundial/blob/master/LICENSE
THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUTWARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED,INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT,MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
See the Mulan PSL v2 for more details.
```
