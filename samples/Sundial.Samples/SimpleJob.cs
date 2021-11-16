using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sundial.Samples
{
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
            // // Do your job!!!
            _logger.LogInformation("<{JobName}> {DateTime}", context.JobDetail.Identity, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return Task.CompletedTask;
        }
    }
}