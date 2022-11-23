using Sundial;
using Sundial.Samples;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSchedule(options =>
        {
            options.AddJob<MyJob>(Triggers.PeriodSeconds(5)
                , Triggers.Minutely());
        });
    })
    .Build();

await host.RunAsync();
