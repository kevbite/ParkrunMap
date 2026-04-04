using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo.Commands;

public class UpsertParkrunCommand(IMongoCollection<Parkrun> collection)
{
    public async Task ExecuteAsync(Parkrun parkrun, CancellationToken ct = default)
    {
        var filter = Builders<Parkrun>.Filter.And(
            Builders<Parkrun>.Filter.Eq(p => p.Website.Domain, parkrun.Website.Domain),
            Builders<Parkrun>.Filter.Eq(p => p.Website.Path, parkrun.Website.Path));

        var update = Builders<Parkrun>.Update
            .Set(p => p.GeoXmlId, parkrun.GeoXmlId)
            .Set(p => p.Name, parkrun.Name)
            .Set(p => p.Website, new Website { Domain = parkrun.Website.Domain, Path = parkrun.Website.Path })
            .Set(p => p.Region, parkrun.Region)
            .Set(p => p.Country, parkrun.Country)
            .Set(p => p.Location, GeoJson.Point(GeoJson.Geographic(parkrun.Location.Coordinates.Longitude, parkrun.Location.Coordinates.Latitude)));

        await collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true }, ct);
    }
}
