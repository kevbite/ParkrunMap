using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ParkrunMap.Domain;

namespace ParkrunMap.FunctionsApp.QueryParkrunsByBox
{
    public class ParkrunCourseResponseObject
    {
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "googleMapIds")]
        public IReadOnlyCollection<string> GoogleMapIds { get; set; }

        [JsonProperty(PropertyName = "terrain", ItemConverterType = typeof(StringEnumConverter))]
        public IReadOnlyCollection<TerrainType> Terrain { get; set; }
    }
}