using MassTransit;
using NCronJob;
using ParkrunMap.Contracts;
using ParkrunMap.Data.Mongo.Queries;

namespace ParkrunMap.Scheduler.Jobs;

public class QueueCourseDownloadsJob(
    GetUnprocessedCourseTargets query,
    IPublishEndpoint publishEndpoint,
    ILogger<QueueCourseDownloadsJob> logger) : IJob
{
    private const int BatchSize = 20;

    public async Task RunAsync(IJobExecutionContext context, CancellationToken token)
    {
        logger.LogInformation("Starting QueueCourseDownloads job (batch size: {BatchSize})", BatchSize);

        var targets = await query.ExecuteAsync(BatchSize, token);

        if (targets.Count == 0)
        {
            logger.LogInformation("No unprocessed course pages found");
            return;
        }

        var batch = new ScrapeCourseBatch(
            targets.Select(t => new CourseScrapeTarget(t.Domain, t.Path)).ToList());

        await publishEndpoint.Publish(batch, token);

        logger.LogInformation("QueueCourseDownloads job queued batch of {Count} course pages", targets.Count);
    }
}
