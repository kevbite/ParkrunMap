using System.Collections.Generic;
using Newtonsoft.Json;

namespace ParkrunMap.Scraping.Parkruns.Data
{
    public class Events
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("features")]
        public List<Feature> Features { get; set; }
    }
}