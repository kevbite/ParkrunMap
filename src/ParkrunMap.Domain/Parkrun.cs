﻿using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;

namespace ParkrunMap.Domain
{
    public class Parkrun
    {
        public Parkrun()
        {
            Cancellations = new Cancellation[0];
            Features = new Features();
            Statistics = new Statistics();
        }

        public ObjectId Id { get; set; }

        public int GeoXmlId { get; set; }

        public string Name { get; set; }

        public ParkrunWebsite Website { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; }

        public IReadOnlyList<Cancellation> Cancellations { get; set; }

        public Course Course { get; set; }

        public Features Features { get; set; }
        
        public SpecialEvents SpecialEvents { get; set; }
        
        public Statistics Statistics { get; set; }
    }
}