using MassTransit;
using ParkrunMap.Contracts;
using ParkrunMap.Scraping;

namespace ParkrunMap.Worker.Consumers;

public class ScrapeCourseBatchConsumer(
    CoursePageScraper scraper,
    IPublishEndpoint publishEndpoint,
    ILogger<ScrapeCourseBatchConsumer> logger)
    : IConsumer<ScrapeCourseBatch>
{
    public async Task Consume(ConsumeContext<ScrapeCourseBatch> context)
    {
        var targets = context.Message.Targets;
        logger.LogInformation("Scraping course batch of {Count} parkruns", targets.Count);

        var count = 0;
        await foreach (var result in scraper.ScrapeAsync(
            targets.Select(t => (t.Domain, t.Path)).ToList(),
            context.CancellationToken))
        {
            await publishEndpoint.Publish(new UpdateCourseDetails(
                result.Domain,
                result.Path,
                result.Description,
                result.GoogleMapIds,
                result.Terrain), context.CancellationToken);

            await publishEndpoint.Publish(new UpdateParkrunStatistics(
                result.Domain,
                result.Path,
                result.Statistics.TotalEvents,
                result.Statistics.TotalRunners,
                result.Statistics.TotalRuns,
                result.Statistics.AverageRunnersPerWeek,
                result.Statistics.LargestEvent,
                result.Statistics.SmallestEvent,
                result.Statistics.NumberOfVolunteers,
                result.Statistics.TotalVolunteers,
                result.Statistics.TotalKidsRuns), context.CancellationToken);

            count++;
        }

        logger.LogInformation("Published {Count} course/statistics updates from batch of {BatchSize}",
            count, targets.Count);
    }
}