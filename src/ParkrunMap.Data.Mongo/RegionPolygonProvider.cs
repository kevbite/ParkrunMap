namespace ParkrunMap.Data.Mongo
{
    public class RegionPolygonProvider : IRegionPolygonProvider
    {
        public double[,] GetPolygon(QueryParkrunByRegion.Region region)
        {
            var ukPolygon = new double[,]
            {
                {
                    -10.8544921875,
                    49.82380908513249
                },
                {
                    -10.8544921875,
                    59.478568831926395
                },
                {
                    2.021484375,
                    59.478568831926395
                },
                {
                    2.021484375,
                    49.82380908513249
                },
                {
                    -10.8544921875,
                    49.82380908513249
                }
            };

            if (region == QueryParkrunByRegion.Region.UK)
            {
                return ukPolygon;
            }

            return new double[0,0];
        }
    }
}