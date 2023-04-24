using Microsoft.Extensions.DependencyInjection;
using Sundial;

_ = new ServiceCollection()
    .AddSchedule(options =>
    {
        options.AddJob<MyJob>(Triggers.Minutely()
         , Triggers.Period(5000)
         , Triggers.Hourly());
    })
    .GetScheduleHostedService()
    .StartAsync(new CancellationTokenSource().Token);

Console.ReadKey();

public class MyJob : IJob
{
    public Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        System.Console.WriteLine($"{context}");
        return Task.CompletedTask;
    }
}