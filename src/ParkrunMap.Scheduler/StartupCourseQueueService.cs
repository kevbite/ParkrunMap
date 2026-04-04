using Microsoft.Extensions.DependencyInjection;
using ParkrunMap.Contracts;
using ParkrunMap.Data.Mongo.Queries;

namespace ParkrunMap.Scheduler;

/// <summary>
/// Runs once at startup (when Jobs:RunAtStartup is true) to queue course downloads
/// for any parkruns that were inserted by the events.json job.
/// Polls MongoDB until unprocessed courses appear, then publishes a ScrapeCourseBatch.
/// </summary>
public class StartupCourseQueueService(
    IServiceScopeFactory scopeFactory,
    ILogger<StartupCourseQueueService> logger) : BackgroundService
{
    private const int BatchSize = 100;
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(3);
    private static readonly TimeSpan MaxWait = TimeSpan.FromMinutes(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("StartupCourseQueueService: starting, will queue course downloads as parkruns are inserted");

        var deadline = DateTimeOffset.UtcNow.Add(MaxWait);
        // Track what has already been queued so we never send the same target twice.
        var alreadyQueued = new HashSet<(string Domain, string Path)>();

        while (DateTimeOffset.UtcNow < deadline && !stoppingToken.IsCancellationRequested)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var query = scope.ServiceProvider.GetRequiredService<GetUnprocessedCourseTargets>();

            var allTargets = await query.ExecuteAsync(BatchSize, stoppingToken);
            var newTargets = allTargets.Where(t => !alreadyQueued.Contains(t)).ToList();

            if (newTargets.Count > 0)
            {
                logger.LogInformation("StartupCourseQueueService: queuing {Count} new course downloads ({Total} total so far)",
                    newTargets.Count, alreadyQueued.Count + newTargets.Count);

                var publishEndpoint = scope.ServiceProvider.GetRequiredService<MassTransit.IPublishEndpoint>();
                var batch = new ScrapeCourseBatch(
                    newTargets.Select(t => new CourseScrapeTarget(t.Domain, t.Path)).ToList());

                await publishEndpoint.Publish(batch, stoppingToken);
                newTargets.ForEach(t => alreadyQueued.Add(t));
            }

            await Task.Delay(PollInterval, stoppingToken);
        }

        logger.LogInformation("StartupCourseQueueService: finished, queued {Total} course downloads in total", alreadyQueued.Count);
    }
}
