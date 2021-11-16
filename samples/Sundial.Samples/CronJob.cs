using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sundial.Samples
{
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
}