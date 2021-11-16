using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sundial;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace SundialUnitTests
{
    public class SundialUnitTest
    {
        protected readonly ITestOutputHelper Output;
        public SundialUnitTest(ITestOutputHelper tempOutput)
        {
            Output = tempOutput;
        }

        [Fact]
        public void TestDefaultServices()
        {
            var builder = Host.CreateDefaultBuilder();
            builder.ConfigureServices(services =>
            {
                services.AddSundial();

                services.Any(s => s.ServiceType == typeof(IJobStorer) && s.Lifetime == ServiceLifetime.Singleton).Should().BeTrue();
                services.Any(s => s.ServiceType == typeof(IScheduler) && s.Lifetime == ServiceLifetime.Singleton).Should().BeTrue();
            });

            var app = builder.Build();
            var services = app.Services;

            var jobStorerService = services.GetService<IJobStorer>();
            jobStorerService.Should().NotBeNull();

            var schedulerService = services.GetService<IScheduler>();
            schedulerService.Should().NotBeNull();
        }

        [Fact]
        public void TestSundialOptionsBuilder()
        {
            var sundialOptionsBuilder = new SundialOptionsBuilder();

            var builderType = typeof(SundialOptionsBuilder);
            var bindingAttr = BindingFlags.NonPublic | BindingFlags.Instance;
            builderType.Invoking(t => t.GetField("_jobs", bindingAttr).GetValue(sundialOptionsBuilder).Should().NotBeNull()).Should().NotThrow();
            builderType.Invoking(t => t.GetField("_jobStorerImplementationFactory", bindingAttr).GetValue(sundialOptionsBuilder).Should().BeNull()).Should().NotThrow();
            builderType.Invoking(t => t.GetField("_jobMonitor", bindingAttr).GetValue(sundialOptionsBuilder).Should().BeNull()).Should().NotThrow();
            builderType.Invoking(t => t.GetField("_jobExecutor", bindingAttr).GetValue(sundialOptionsBuilder).Should().BeNull()).Should().NotThrow();
        }
    }
}
