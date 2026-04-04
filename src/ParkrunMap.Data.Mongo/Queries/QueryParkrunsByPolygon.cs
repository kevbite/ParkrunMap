using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo.Queries;

public class QueryParkrunsByPolygon(IMongoCollection<Parkrun> collection)
{
    public async Task<List<Parkrun>> ExecuteAsync(double[][] polygon, CancellationToken ct = default)
    {
        var geoPolygon = GeoJson.Polygon(polygon.Select(c => GeoJson.Geographic(c[0], c[1])).ToArray());

        var filter = Builders<Parkrun>.Filter.GeoWithin(p => p.Location, geoPolygon);

        return await collection.Find(filter).ToListAsync(ct);
    }
}
