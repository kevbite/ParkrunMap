using MongoDB.Driver;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo.Commands;

public class UpdateFeaturesCommand(IMongoCollection<Parkrun> collection)
{
    public async Task ExecuteAsync(string domain, string path, Features features, CancellationToken ct = default)
    {
        var filter = WebsiteFilter(domain, path);
        var update = Builders<Parkrun>.Update
            .Set(p => p.Features.WheelchairFriendly, features.WheelchairFriendly)
            .Set(p => p.Features.BuggyFriendly, features.BuggyFriendly)
            .Set(p => p.Features.VisuallyImpairedFriendly, features.VisuallyImpairedFriendly)
            .Set(p => p.Features.Toilets, features.Toilets)
            .Set(p => p.Features.DogsAllowed, features.DogsAllowed)
            .Set(p => p.Features.Cafe, features.Cafe)
            .Set(p => p.Features.PostRunCoffee, features.PostRunCoffee)
            .Set(p => p.Features.DrinkingFountain, features.DrinkingFountain)
            .Set(p => p.Features.ChangingRooms, features.ChangingRooms)
            .Set(p => p.Features.Lockers, features.Lockers)
            .Set(p => p.Features.Showers, features.Showers)
            .Set(p => p.Features.BagDrop, features.BagDrop)
            .Set(p => p.Features.BabyChangingFacilities, features.BabyChangingFacilities)
            .Set(p => p.Features.CarParking, features.CarParking)
            .Set(p => p.Features.CycleParking, features.CycleParking)
            .Set(p => p.Features.CarParkingOptions, features.CarParkingOptions)
            .Set(p => p.Features.CycleParkingOptions, features.CycleParkingOptions)
            .Set(p => p.Features.RecommendedBuggy, features.RecommendedBuggy);

        await collection.UpdateOneAsync(filter, update, cancellationToken: ct);
    }

    private static FilterDefinition<Parkrun> WebsiteFilter(string domain, string path) =>
        Builders<Parkrun>.Filter.And(
            Builders<Parkrun>.Filter.Eq(p => p.Website.Domain, domain),
            Builders<Parkrun>.Filter.Eq(p => p.Website.Path, path));
}
