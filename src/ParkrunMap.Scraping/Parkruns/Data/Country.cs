using System.Collections.Generic;
using Newtonsoft.Json;

namespace ParkrunMap.Scraping.Parkruns.Data
{
    public class Country
    {
        [JsonProperty("url")] 
        public string Url { get; set; }

        [JsonProperty("bounds")]
        public List<double> Bounds { get; set; }
    }
}