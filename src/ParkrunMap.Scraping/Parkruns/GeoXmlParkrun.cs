using System;

namespace ParkrunMap.Scraping.Parkruns
{
    public class GeoXmlParkrun
    {
        public GeoXmlParkrun(int geoXmlId, string name, string websiteDomain, string websitePath, string region, string country, double latitude, double longitude)
        {
            GeoXmlId = geoXmlId;
            Name = name;
            WebsiteDomain = websiteDomain;
            WebsitePath = websitePath;
            Region = region;
            Country = country;
            Latitude = latitude;
            Longitude = longitude;
        }

        public int GeoXmlId { get; }

        public string Name { get; }

        public string WebsiteDomain { get; }

        public string WebsitePath { get; }

        public string Region { get; }

        public string Country { get; }

        public double Latitude { get; }

        public double Longitude { get; }

    }

    public class EventsJsonParkrun
    {
        public EventsJsonParkrun(string name, string websiteDomain, string websitePath, double latitude, double longitude)
        {
            Name = name;
            WebsiteDomain = websiteDomain;
            WebsitePath = websitePath;
            Latitude = latitude;
            Longitude = longitude;
        }

        public string Name { get; }

        public string WebsiteDomain { get; }

        public string WebsitePath { get; }

        public double Latitude { get; }

        public double Longitude { get; }
    }
}