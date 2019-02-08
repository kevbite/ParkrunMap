using System;

namespace ParkrunMap.FunctionsApp.QueryParkrunsByBox
{
    public class PolygonCreator : IPolygonCreator
    {
        public double[,] FromBox(double lat1, double lon1, double lat2, double lon2)
        {
            var bottomLeft = new { Lat = Math.Min(lat1, lat2), Lon = Math.Min(lon1, lon2) };
            var upperRight = new { Lat = Math.Max(lat1, lat2), Lon = Math.Max(lon1, lon2) };

            return new [,]
            {
                { bottomLeft.Lon, bottomLeft.Lat},
                { upperRight.Lon, bottomLeft.Lat},
                { upperRight.Lon, upperRight.Lat},
                { bottomLeft.Lon, upperRight.Lat},
                { bottomLeft.Lon, bottomLeft.Lat},
            };
        }
    }
}