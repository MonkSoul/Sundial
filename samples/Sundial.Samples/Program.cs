using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Sundial.Samples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSundial(builder =>
                    {
                        builder.AddJob<SimpleJob>();
                        builder.AddJob<CronJob>();
                        builder.AddJob<YourJob>(new YourTrigger(TimeSpan.FromSeconds(1))
                        {
                            NextRunTime = DateTime.UtcNow
                        });
                    });
                });
    }
}
