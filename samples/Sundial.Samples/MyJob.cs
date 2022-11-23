namespace Sundial.Samples;

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