using MassTransit;
using NCronJob;
using ParkrunMap.Contracts;
using ParkrunMap.Scraping;

namespace ParkrunMap.Scheduler.Jobs;

public class DownloadEventsJsonJob(EventsJsonScraper scraper, IPublishEndpoint publishEndpoint, ILogger<DownloadEventsJsonJob> logger) : IJob
{
    public async Task RunAsync(IJobExecutionContext context, CancellationToken token)
    {
        logger.LogInformation("Starting DownloadEventsJson job");

        var parkruns = await scraper.ScrapeAsync(token);

        foreach (var p in parkruns)
        {
            await publishEndpoint.Publish(new UpsertParkrun(
                p.Id, p.Name, p.Domain, p.Path,
                string.Empty,  // Region is not present in events.json
                p.Country,     // Country code from the countries section (e.g. "UK")
                p.Latitude, p.Longitude), token);
        }

        logger.LogInformation("DownloadEventsJson job published {Count} upsert messages", parkruns.Count);
    }
}
