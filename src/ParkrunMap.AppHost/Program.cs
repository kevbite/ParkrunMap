var builder = DistributedApplication.CreateBuilder(args);

if (builder.ExecutionContext.IsPublishMode)
{
    // Production: Azure Application Insights for telemetry + external managed services.
    // Connection strings are supplied via azd environment secrets before deploying:
    //   azd env set CONNECTIONSTRINGS__PARKRUN-MAP "mongodb://..."
    //   azd env set CONNECTIONSTRINGS__RABBITMQ "amqps://..."
    var insights = builder.AddAzureApplicationInsights("appInsights");
    var mongoDb = builder.AddConnectionString("parkrun-map");
    var rabbitMq = builder.AddConnectionString("rabbitmq");

    builder.AddProject<Projects.ParkrunMap_Api>("parkrun-api")
        .WithReference(mongoDb)
        .WithReference(rabbitMq)
        .WithReference(insights)
        .WithExternalHttpEndpoints();

    builder.AddProject<Projects.ParkrunMap_Worker>("parkrun-worker")
        .WithReference(mongoDb)
        .WithReference(rabbitMq)
        .WithReference(insights);

    // Scheduler must run as exactly 1 replica to prevent duplicate NCronJob execution.
    builder.AddProject<Projects.ParkrunMap_Scheduler>("parkrun-scheduler")
        .WithReference(mongoDb)
        .WithReference(rabbitMq)
        .WithReference(insights)
        .WithReplicas(1);
}
else
{
    // Local development: Docker containers managed by Aspire.
    var mongo = builder.AddMongoDB("mongodb")
        .WithMongoExpress();

    var mongoDb = mongo.AddDatabase("parkrun-map");

    var rabbitMq = builder.AddRabbitMQ("rabbitmq")
        .WithManagementPlugin();

    builder.AddProject<Projects.ParkrunMap_Api>("parkrun-api")
        .WithReference(mongoDb)
        .WithReference(rabbitMq)
        .WithExternalHttpEndpoints()
        .WaitFor(mongoDb)
        .WaitFor(rabbitMq);

    var worker = builder.AddProject<Projects.ParkrunMap_Worker>("parkrun-worker")
        .WithReference(mongoDb)
        .WithReference(rabbitMq)
        .WaitFor(mongoDb)
        .WaitFor(rabbitMq);

    // Scheduler must run as exactly 1 replica to prevent duplicate NCronJob execution.
    builder.AddProject<Projects.ParkrunMap_Scheduler>("parkrun-scheduler")
        .WithReference(mongoDb)
        .WithReference(rabbitMq)
        .WithReplicas(1)
        .WaitFor(mongoDb)
        .WaitFor(rabbitMq)
        .WaitFor(worker);
}

builder.Build().Run();

// Expose Program class for integration testing
public partial class Program { }
