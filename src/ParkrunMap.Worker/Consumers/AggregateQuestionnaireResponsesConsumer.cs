using MassTransit;
using ParkrunMap.Contracts;
using ParkrunMap.Domain;

namespace ParkrunMap.Worker.Consumers;

public class AggregateQuestionnaireResponsesConsumer(
    IPublishEndpoint publishEndpoint,
    ILogger<AggregateQuestionnaireResponsesConsumer> logger)
    : IConsumer<AggregateQuestionnaireResponses>
{
    public async Task Consume(ConsumeContext<AggregateQuestionnaireResponses> context)
    {
        var responses = context.Message.Responses;
        logger.LogInformation("Aggregating {Count} questionnaire responses", responses.Count);

        var grouped = responses.GroupBy(r => (r.Domain, r.Path));

        foreach (var group in grouped)
        {
            var all = group.ToList();
            var features = new UpdateParkrunFeatures(
                group.Key.Domain,
                group.Key.Path,
                Majority(all, r => r.WheelchairFriendly),
                Majority(all, r => r.BuggyFriendly),
                Majority(all, r => r.VisuallyImpairedFriendly),
                Majority(all, r => r.Toilets),
                Majority(all, r => r.DogsAllowed),
                Majority(all, r => r.Cafe),
                Majority(all, r => r.PostRunCoffee),
                Majority(all, r => r.DrinkingFountain),
                Majority(all, r => r.ChangingRooms),
                Majority(all, r => r.Lockers),
                Majority(all, r => r.Showers),
                Majority(all, r => r.BagDrop),
                Majority(all, r => r.BabyChangingFacilities),
                Majority(all, r => r.CarParking),
                Majority(all, r => r.CycleParking),
                MajorityList(all, r => r.CarParkingOptions),
                MajorityList(all, r => r.CycleParkingOptions),
                MajorityList(all, r => r.RecommendedBuggy));

            await publishEndpoint.Publish(features, context.CancellationToken);
        }
    }

    private static bool? Majority(List<QuestionnaireResponse> responses, Func<QuestionnaireResponse, bool?> selector)
    {
        var answered = responses.Select(selector).Where(v => v.HasValue).Select(v => v!.Value).ToList();
        if (answered.Count == 0) return null;
        var trueCount = answered.Count(v => v);
        return trueCount > answered.Count / 2;
    }

    private static IReadOnlyList<T> MajorityList<T>(List<QuestionnaireResponse> responses, Func<QuestionnaireResponse, IReadOnlyList<T>> selector)
    {
        var allLists = responses.Select(selector).Where(l => l.Count > 0).ToList();
        if (allLists.Count == 0) return [];
        return allLists
            .GroupBy(l => string.Join(",", l))
            .OrderByDescending(g => g.Count())
            .First()
            .First();
    }
}
