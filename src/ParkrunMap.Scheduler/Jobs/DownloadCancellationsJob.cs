using MassTransit;
using NCronJob;
using ParkrunMap.Contracts;
using ParkrunMap.Scraping;

namespace ParkrunMap.Scheduler.Jobs;

public class DownloadCancellationsJob(CancellationsScraper scraper, IPublishEndpoint publishEndpoint, ILogger<DownloadCancellationsJob> logger) : IJob
{
    public async Task RunAsync(IJobExecutionContext context, CancellationToken token)
    {
        logger.LogInformation("Starting DownloadCancellations job");

        var cancellations = await scraper.ScrapeAsync(token);

        foreach (var c in cancellations)
        {
            if (c.WebsiteUri is null) continue;
            if (!Uri.TryCreate(c.WebsiteUri, UriKind.Absolute, out var uri)) continue;

            var domain = uri.Host;
            var path = uri.AbsolutePath.TrimEnd('/') + "/";

            await publishEndpoint.Publish(new AddParkrunCancellation(domain, path, c.Date, c.Reason), token);
        }

        logger.LogInformation("DownloadCancellations job published {Count} cancellation messages", cancellations.Count);
    }
}
