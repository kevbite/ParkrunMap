﻿using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;

namespace ParkrunMap.Domain
{
    public class Parkrun
    {
        public Parkrun()
        {
            Cancellations = new Cancellation[0];
        }

        public ObjectId Id { get; set; }

        public int GeoXmlId { get; set; }

        public string Name { get; set; }

        public Uri Uri { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; }

        public IReadOnlyList<Cancellation> Cancellations { get; set; }    
    }

    public class Cancellation
    {
        public DateTime Date { get; set; }
        public string Reason { get; set; }
    }
}