using MassTransit;
using Microsoft.Extensions.Configuration;
using NCronJob;
using ParkrunMap.Contracts;
using ParkrunMap.Domain;
using ParkrunMap.Scraping;

namespace ParkrunMap.Scheduler.Jobs;

public class DownloadQuestionnaireJob(
    QuestionnaireScraper scraper,
    IPublishEndpoint publishEndpoint,
    IConfiguration configuration,
    ILogger<DownloadQuestionnaireJob> logger) : IJob
{
    public async Task RunAsync(IJobExecutionContext context, CancellationToken token)
    {
        var spreadsheetId = configuration["Questionnaire:SpreadsheetId"]
            ?? throw new InvalidOperationException("Questionnaire:SpreadsheetId not configured");
        var apiKey = configuration["Questionnaire:GoogleApiKey"]
            ?? throw new InvalidOperationException("Questionnaire:GoogleApiKey not configured");

        logger.LogInformation("Starting DownloadQuestionnaire job");

        var entries = await scraper.ScrapeAsync(spreadsheetId, apiKey, token);

        if (entries.Count == 0)
        {
            logger.LogWarning("No questionnaire responses returned");
            return;
        }

        var message = new AggregateQuestionnaireResponses(
            entries.Select(e => new QuestionnaireResponse(
                e.Domain, e.Path,
                e.WheelchairFriendly, e.BuggyFriendly, e.VisuallyImpairedFriendly,
                e.Toilets, e.DogsAllowed, e.Cafe, e.PostRunCoffee,
                e.DrinkingFountain, e.ChangingRooms, e.Lockers, e.Showers,
                e.BagDrop, e.BabyChangingFacilities, e.CarParking, e.CycleParking,
                e.CarParkingOptions.ToList(),
                e.CycleParkingOptions.ToList(),
                e.RecommendedBuggy.ToList())).ToList());

        await publishEndpoint.Publish(message, token);
        logger.LogInformation("DownloadQuestionnaire job published {Count} responses for aggregation", entries.Count);
    }
}
