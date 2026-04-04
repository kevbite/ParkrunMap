using MongoDB.Driver;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo.Commands;

public class UpdateSpecialEventsCommand(IMongoCollection<Parkrun> collection)
{
    public async Task ExecuteAsync(string domain, string path, int year, bool christmasDay, bool newYearsDay, CancellationToken ct = default)
    {
        var filter = WebsiteFilter(domain, path);

        var christmasUpdate = christmasDay
            ? Builders<Parkrun>.Update.AddToSet(p => p.SpecialEvents.ChristmasDay, year)
            : Builders<Parkrun>.Update.Pull(p => p.SpecialEvents.ChristmasDay, year);

        var newYearsUpdate = newYearsDay
            ? Builders<Parkrun>.Update.AddToSet(p => p.SpecialEvents.NewYearsDay, year)
            : Builders<Parkrun>.Update.Pull(p => p.SpecialEvents.NewYearsDay, year);

        await collection.UpdateOneAsync(
            filter,
            Builders<Parkrun>.Update.Combine(christmasUpdate, newYearsUpdate),
            cancellationToken: ct);
    }

    private static FilterDefinition<Parkrun> WebsiteFilter(string domain, string path) =>
        Builders<Parkrun>.Filter.And(
            Builders<Parkrun>.Filter.Eq(p => p.Website.Domain, domain),
            Builders<Parkrun>.Filter.Eq(p => p.Website.Path, path));
}
