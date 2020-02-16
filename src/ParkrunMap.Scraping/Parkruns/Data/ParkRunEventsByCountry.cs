using System.Collections.Generic;
using Newtonsoft.Json;

namespace ParkrunMap.Scraping.Parkruns.Data
{
    public class ParkRunEventsByCountry
    {
        [JsonProperty("countries")]
        public Dictionary<string, Country> Countries { get; set; }

        [JsonProperty("events")]
        public Events Events { get; set; }
    }
}