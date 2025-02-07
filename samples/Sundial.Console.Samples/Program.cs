using Microsoft.Extensions.DependencyInjection;
using Sundial;
using Sundial.Console.Samples;

await new ServiceCollection()
    .AddSchedule(options =>
    {
        options.AddJob<MyJob>(Triggers.Minutely()
         , Triggers.Period(5000)
         , Triggers.Hourly());
    })
    .GetScheduleHostedService()
    .StartAsync(new CancellationTokenSource().Token);

Console.ReadLine();