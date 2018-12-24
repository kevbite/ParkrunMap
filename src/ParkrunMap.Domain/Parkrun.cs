using System;
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;

namespace ParkrunMap.Domain
{
    public class Parkrun
    {
        public ObjectId Id { get; set; }

        public int GeoXmlId { get; set; }

        public string Name { get; set; }

        public Uri Uri { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; }
    }
}