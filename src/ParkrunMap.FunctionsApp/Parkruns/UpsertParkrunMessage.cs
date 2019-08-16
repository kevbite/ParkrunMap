using System;

namespace ParkrunMap.FunctionsApp.Parkruns
{
    public class UpsertParkrunMessage
    {
        public string Name { get; set; }

        public string WebsiteDomain { get; set; }

        public string WebsitePath { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}