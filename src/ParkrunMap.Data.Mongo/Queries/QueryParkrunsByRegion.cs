using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo.Queries;

public class QueryParkrunsByRegion(QueryParkrunsByPolygon queryByPolygon)
{
    public Task<List<Parkrun>> ExecuteAsync(ParkrunRegion region, CancellationToken ct = default)
    {
        var polygon = RegionPolygonProvider.GetPolygon(region);
        return queryByPolygon.ExecuteAsync(polygon, ct);
    }
}
