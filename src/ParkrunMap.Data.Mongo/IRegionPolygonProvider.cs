namespace ParkrunMap.Data.Mongo
{
    public interface IRegionPolygonProvider
    {
        double[,] GetPolygon(QueryParkrunByRegion.Region region);
    }
}