using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace ParkrunMap.Scraping.Parkruns
{
    public class EventsJsonParser
    {
        public IReadOnlyCollection<EventsJsonParkrun> Parse(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            using (var reader = new JsonTextReader(sr))
            {
                var serializer = new JsonSerializer();

                var root = serializer.Deserialize<Root>(reader);

                return root.Events.Features.Where(x => !x.Properties.Eventname.EndsWith("-juniors")).Join(root.Countries, feature => feature.Properties.Countrycode, country => country.Key,
                    (feature, country) =>
                    {
                        var latitude = feature.Geometry.Coordinates[1];
                        var longitude = feature.Geometry.Coordinates[0];
                        return new EventsJsonParkrun(feature.Properties.EventShortName, country.Value.Url,
                            $"/{feature.Properties.Eventname}", latitude,
                            longitude);
                    }).ToArray();
            }
        }


        private class Root
        {
            [JsonProperty("countries")]
            public Dictionary<int, Country> Countries { get; set; }
            [JsonProperty("events")]
            public Events Events { get; set; }
        }


        private class Country
        {
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("bounds")]
            public float[] Bounds { get; set; }
        }

        private class Events
        {
            [JsonProperty("features")]
            public Feature[] Features { get; set; }
        }

        private class Feature
        {
            [JsonProperty("geometry")]
            public Geometry Geometry { get; set; }
            [JsonProperty("properties")]
            public Properties Properties { get; set; }
        }

        private class Geometry
        {
            [JsonProperty("type")]
            public string Type { get; set; }
            [JsonProperty("coordinates")]
            public double[] Coordinates { get; set; }
        }

        private class Properties
        {
            [JsonProperty("eventname")]
            public string Eventname { get; set; }
            [JsonProperty("EventShortName")]
            public string EventShortName { get; set; }
            [JsonProperty("countrycode")]
            public int Countrycode { get; set; }
        }
    }


}
