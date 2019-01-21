namespace ParkrunMap.Data.Mongo
{
    public class RegionPolygonProvider : IRegionPolygonProvider
    {
        public double[,] GetPolygon(QueryParkrunByRegion.Region region)
        {
            if (region == QueryParkrunByRegion.Region.UK)
            {
                return RegionPolygons.Uk;
            }

            return new double[0, 0];
        }
    }
}