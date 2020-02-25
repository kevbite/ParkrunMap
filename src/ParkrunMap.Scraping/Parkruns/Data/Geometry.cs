using System.Collections.Generic;
using Newtonsoft.Json;

namespace ParkrunMap.Scraping.Parkruns.Data
{
    public class Geometry
    {
        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; set; }
    }
}