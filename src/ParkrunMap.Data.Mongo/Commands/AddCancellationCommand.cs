using MongoDB.Driver;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo.Commands;

public class AddCancellationCommand(IMongoCollection<Parkrun> collection)
{
    public async Task ExecuteAsync(string domain, string path, Cancellation cancellation, CancellationToken ct = default)
    {
        var filter = WebsiteFilter(domain, path);

        var removeDuplicate = Builders<Parkrun>.Update
            .PullFilter(p => p.Cancellations, c => c.Date == cancellation.Date);
        await collection.UpdateOneAsync(filter, removeDuplicate, cancellationToken: ct);

        var addNew = Builders<Parkrun>.Update
            .Push(p => p.Cancellations, cancellation);
        await collection.UpdateOneAsync(filter, addNew, cancellationToken: ct);
    }

    private static FilterDefinition<Parkrun> WebsiteFilter(string domain, string path) =>
        Builders<Parkrun>.Filter.And(
            Builders<Parkrun>.Filter.Eq(p => p.Website.Domain, domain),
            Builders<Parkrun>.Filter.Eq(p => p.Website.Path, path));
}
