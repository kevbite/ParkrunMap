using Newtonsoft.Json;

namespace ParkrunMap.Scraping.Parkruns.Data
{
    public class Properties
    {
        [JsonProperty("eventname")]
        public string EventName { get; set; }

        [JsonProperty("EventLongName")]
        public string EventLongName { get; set; }

        [JsonProperty("EventShortName")]
        public string EventShortName { get; set; }

        [JsonProperty("LocalisedEventLongName")]
        public object LocalisedEventLongName { get; set; }

        [JsonProperty("countrycode")]
        public long CountryCode { get; set; }

        [JsonProperty("seriesid")]
        public long SeriesId { get; set; }

        [JsonProperty("EventLocation")]
        public string EventLocation { get; set; }
    }
}