using MassTransit;
using ParkrunMap.Data.Mongo;
using ParkrunMap.Scraping;
using ParkrunMap.Worker.Consumers;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

// MongoDB
builder.Services.AddMongoClient(builder.Configuration, "parkrun-map");
builder.Services.AddParkrunMongo();

// Scraping (Playwright browser automation — justify scaling N replicas without schedule duplication)
builder.Services.AddParkrunScraping();

// MassTransit with RabbitMQ — consumers only; scheduling lives in ParkrunMap.Scheduler
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UpsertParkrunConsumer>();
    x.AddConsumer<ScrapeCourseBatchConsumer>();
    x.AddConsumer<UpdateCourseDetailsConsumer>();
    x.AddConsumer<UpdateParkrunStatisticsConsumer>();
    x.AddConsumer<AddParkrunCancellationConsumer>();
    x.AddConsumer<UpdateParkrunFeaturesConsumer>();
    x.AddConsumer<ScrapeSpecialEventsBatchConsumer>();
    x.AddConsumer<UpdateParkrunSpecialEventsConsumer>();
    x.AddConsumer<AggregateQuestionnaireResponsesConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("rabbitmq"));
        cfg.ConfigureEndpoints(ctx);
        cfg.UseMessageRetry(r => r.Exponential(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(3)));
    });
});

var host = builder.Build();
host.Run();
