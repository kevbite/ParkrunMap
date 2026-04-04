using MassTransit;
using NCronJob;
using ParkrunMap.Contracts;

namespace ParkrunMap.Scheduler.Jobs;

public class QueueSpecialEventsDownloadJob(IPublishEndpoint publishEndpoint, ILogger<QueueSpecialEventsDownloadJob> logger) : IJob
{
    private static readonly IReadOnlyList<SpecialEventsScrapeTarget> CountryTargets =
    [
        new("https://www.parkrun.com.au", "AU"),
        new("https://www.parkrun.ie", "IE"),
        new("https://www.parkrun.it", "IT"),
        new("https://www.parkrun.co.nz", "NZ"),
        new("https://www.parkrun.org.uk", "UK"),
    ];

    public async Task RunAsync(IJobExecutionContext context, CancellationToken token)
    {
        logger.LogInformation("Starting QueueSpecialEventsDownload job for {Count} countries", CountryTargets.Count);

        var batch = new ScrapeSpecialEventsBatch(CountryTargets);
        await publishEndpoint.Publish(batch, token);

        logger.LogInformation("QueueSpecialEventsDownload job published batch for {Countries}",
            string.Join(", ", CountryTargets.Select(t => t.CountryCode)));
    }
}
