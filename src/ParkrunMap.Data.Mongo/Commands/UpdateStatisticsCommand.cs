using MongoDB.Driver;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo.Commands;

public class UpdateStatisticsCommand(IMongoCollection<Parkrun> collection)
{
    public async Task ExecuteAsync(string domain, string path, Statistics stats, CancellationToken ct = default)
    {
        var filter = WebsiteFilter(domain, path);
        var update = Builders<Parkrun>.Update
            .Set(p => p.Statistics.TotalEvents, stats.TotalEvents)
            .Set(p => p.Statistics.TotalRunners, stats.TotalRunners)
            .Set(p => p.Statistics.TotalRuns, stats.TotalRuns)
            .Set(p => p.Statistics.AverageRunnersPerWeek, stats.AverageRunnersPerWeek)
            .Set(p => p.Statistics.LargestEvent, stats.LargestEvent)
            .Set(p => p.Statistics.SmallestEvent, stats.SmallestEvent)
            .Set(p => p.Statistics.NumberOfVolunteers, stats.NumberOfVolunteers)
            .Set(p => p.Statistics.TotalVolunteers, stats.TotalVolunteers)
            .Set(p => p.Statistics.TotalKidsRuns, stats.TotalKidsRuns);

        await collection.UpdateOneAsync(filter, update, cancellationToken: ct);
    }

    private static FilterDefinition<Parkrun> WebsiteFilter(string domain, string path) =>
        Builders<Parkrun>.Filter.And(
            Builders<Parkrun>.Filter.Eq(p => p.Website.Domain, domain),
            Builders<Parkrun>.Filter.Eq(p => p.Website.Path, path));
}
