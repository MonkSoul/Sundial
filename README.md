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
- 无第三方依赖，[TimeCrontab](https://gitee.com/dotnetchina/TimeCrontab) 也是本人作品
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

    public Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        _logger.LogInformation($"{context}");
        return Task.CompletedTask;
    }
}
```

2. 在 `Startup.cs` 注册 `AddSchedule` 服务和作业：

```cs
services.AddSchedule(options =>
{
    options.AddJob<MyJob>(Triggers.PeriodSeconds(5));
});
```

3. 运行项目：

```bash
info: 2022-12-05 19:32:56.3835407 +08:00 星期一 L System.Logging.ScheduleService[0] #1
      Schedule hosted service is running.
info: 2022-12-05 19:32:56.3913451 +08:00 星期一 L System.Logging.ScheduleService[0] #1
      Schedule hosted service is preloading...
info: 2022-12-05 19:32:56.4322887 +08:00 星期一 L System.Logging.ScheduleService[0] #1
      The <job1_trigger1> trigger for scheduler of <job1> successfully appended to the schedule.
info: 2022-12-05 19:32:56.4347959 +08:00 星期一 L System.Logging.ScheduleService[0] #1
      The scheduler of <job1> successfully appended to the schedule.
warn: 2022-12-05 19:32:56.4504555 +08:00 星期一 L System.Logging.ScheduleService[0] #1
      Schedule hosted service preload completed, and a total of <1> schedulers are appended.
info: 2022-12-05 19:33:01.5100177 +08:00 星期一 L MyJob[0] #13
      <job1> [C] <job1 job1_trigger1> 5s 1ts 2022-12-05 19:33:01.395 -> 2022-12-05 19:33:06.428
info: 2022-12-05 19:33:06.4676792 +08:00 星期一 L MyJob[0] #13
      <job1> [C] <job1 job1_trigger1> 5s 2ts 2022-12-05 19:33:06.428 -> 2022-12-05 19:33:11.435
info: 2022-12-05 19:33:11.4460946 +08:00 星期一 L MyJob[0] #16
      <job1> [C] <job1 job1_trigger1> 5s 3ts 2022-12-05 19:33:11.435 -> 2022-12-05 19:33:16.412
```

`JobExecutionContext` 重写了 `ToString()` 方法并提供以下几种格式：

```bash
# 持续运行格式
<作业Id> 作业描述 [并行C/串行S] <作业Id 触发器Id> 触发器字符串 触发器描述 触发次数ts 触发时间 -> 下一次触发时间

# 触发停止格式
<作业Id> 作业描述 [并行C/串行S] <作业Id 触发器Id> 触发器字符串 触发器描述 触发次数ts 触发时间 [触发器终止状态]
```

[更多文档](https://furion.baiqian.ltd/docs/job/)

## 文档

您可以在[主页](https://furion.baiqian.ltd/docs/job/)找到 Sundial 文档。

## 贡献

该存储库的主要目的是继续发展 Sundial 核心，使其更快、更易于使用。Sundial 的开发在 [Gitee](https://gitee.com/dotnetchina/Sundial) 上公开进行，我们感谢社区贡献错误修复和改进。

## 许可证

Sundial 采用 [MIT](./LICENSE) 开源许可证。
