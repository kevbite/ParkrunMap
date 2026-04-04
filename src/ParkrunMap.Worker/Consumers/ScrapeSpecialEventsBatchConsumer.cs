using MassTransit;
using ParkrunMap.Contracts;
using ParkrunMap.Scraping;

namespace ParkrunMap.Worker.Consumers;

public class ScrapeSpecialEventsBatchConsumer(
    SpecialEventsScraper scraper,
    IPublishEndpoint publishEndpoint,
    ILogger<ScrapeSpecialEventsBatchConsumer> logger)
    : IConsumer<ScrapeSpecialEventsBatch>
{
    public async Task Consume(ConsumeContext<ScrapeSpecialEventsBatch> context)
    {
        var targets = context.Message.Targets;
        logger.LogInformation("Scraping special events for {Count} countries", targets.Count);

        var results = await scraper.ScrapeAsync(
            targets.Select(t => (t.CountryBaseUrl, t.CountryCode)).ToList(),
            context.CancellationToken);

        foreach (var result in results)
        {
            await publishEndpoint.Publish(new UpdateParkrunSpecialEvents(
                result.Domain,
                result.Path,
                result.Year,
                result.ChristmasDay,
                result.NewYearsDay), context.CancellationToken);
        }

        logger.LogInformation("Published {Count} special events updates", results.Count);
    }
}
