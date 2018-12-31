using System;

namespace ParkrunMap.FunctionsApp.UpsertParkrun
{
    public class UpsertParkrunMessage
    {
        public int GeoXmlId { get; set; }

        public string Name { get; set; }

        public Uri Uri { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}