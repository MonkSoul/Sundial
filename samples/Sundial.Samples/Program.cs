using Sundial;
using Sundial.Samples;
using TimeCrontab;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSchedule(options =>
{
    options.AddJob<MyJob>(Triggers.Minutely()
     , Triggers.Period(5000)
     , Triggers.Cron("3,7,8 * * * * ?", CronStringFormat.WithSeconds));
});

var app = builder.Build();

app.UseStaticFiles();
app.UseScheduleUI();

app.Run();
