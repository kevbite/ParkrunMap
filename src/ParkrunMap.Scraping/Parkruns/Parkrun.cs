using System;

namespace ParkrunMap.Scraping.Parkruns
{
    public class Parkrun
    {
        public Parkrun(int geoXmlId, string name, Uri uri, string region, string country, double latitude, double longitude)
        {
            GeoXmlId = geoXmlId;
            Name = name;
            Uri = uri;
            Region = region;
            Country = country;
            Latitude = latitude;
            Longitude = longitude;
        }

        public int GeoXmlId { get; }

        public string Name { get; }

        public Uri Uri { get; }

        public string Region { get; }

        public string Country { get; }

        public double Latitude { get; }

        public double Longitude { get; }

    }
}