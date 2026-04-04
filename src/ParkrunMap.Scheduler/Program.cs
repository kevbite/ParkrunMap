using MassTransit;
using NCronJob;
using ParkrunMap.Data.Mongo;
using ParkrunMap.Scraping;
using ParkrunMap.Scheduler.Jobs;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// MongoDB (needed by QueueCourseDownloadsJob to find unprocessed targets)
builder.Services.AddMongoClient(builder.Configuration, "parkrun-map");
builder.Services.AddParkrunMongo();

// Scraping (needed by DownloadEventsJsonJob, DownloadCancellationsJob, DownloadQuestionnaireJob)
builder.Services.AddParkrunScraping();

// MassTransit  publish-only; no consumers registered here
// Scale this service to exactly 1 replica so each job fires once per interval
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((_, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("rabbitmq"));
    });
});

// Recurring jobs  run in a single instance to avoid duplicate message publishing.
// Deploy ParkrunMap.Scheduler with replicas: 1.
// Consumers in ParkrunMap.Worker can be scaled independently.
var runAtStartup = builder.Configuration.GetValue<bool>("Jobs:RunAtStartup");

if (runAtStartup)
    builder.Services.AddHostedService<ParkrunMap.Scheduler.StartupCourseQueueService>();

builder.Services.AddNCronJob(options =>
{
    // Monday 2 AM  download events.json master list
    options.AddJob<DownloadEventsJsonJob>(j =>
    {
        j.WithCronExpression("0 2 * * MON").WithName("download-events-json");
        if (runAtStartup)
            j.RunAtStartup();
    });

    // Every 2 hours  download cancellations page
    options.AddJob<DownloadCancellationsJob>(j => j.WithCronExpression("0 */2 * * *").WithName("download-cancellations"));

    // Every hour, 8 AM to 11 PM  queue course downloads
    options.AddJob<QueueCourseDownloadsJob>(j => j.WithCronExpression("0 8-23 * * *").WithName("queue-course-downloads"));

    // Monday 2 AM  download questionnaire responses
    options.AddJob<DownloadQuestionnaireJob>(j => j.WithCronExpression("0 2 * * MON").WithName("download-questionnaire"));

    // 7:15 AM on 1st7th December  queue special events scraping
    options.AddJob<QueueSpecialEventsDownloadJob>(j => j.WithCronExpression("15 7 1-7 12 *").WithName("queue-special-events-download"));
});

var host = builder.Build();
await host.UseNCronJobAsync();
await host.RunAsync();
