using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using ParkrunMap.Domain;

namespace ParkrunMap.FunctionsApp.QueryParkrunsByBox
{
    public class ParkrunResponseObject
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "uri")]
        public Uri Uri { get; set; }

        [JsonProperty(PropertyName = "lat")]
        public double Lat { get; set; }

        [JsonProperty(PropertyName = "lon")]
        public double Lon { get; set; }

        [JsonProperty(PropertyName = "cancellations")]
        public IReadOnlyList<Cancellation> Cancellations { get; set; }

        [JsonProperty(PropertyName = "course")]
        public ParkrunCourseResponseObject Course { get; set; }

        [JsonProperty(PropertyName = "features")]
        public ParkrunFeaturesResponseObject Features { get; set; }
        
        [JsonProperty(PropertyName = "specialEvents")]
        public ParkrunSpecialEventsResponseObject SpecialEvents { get; set; }
    }
}