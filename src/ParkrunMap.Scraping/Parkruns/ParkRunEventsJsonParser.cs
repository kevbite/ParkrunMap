using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using ParkrunMap.Scraping.Parkruns.Data;

namespace ParkrunMap.Scraping.Parkruns
{
    public static class ParkRunEventsJsonParser
    {
        public static IEnumerable<Parkrun> Parse(Stream stream)
        {
            using var reader = new StreamReader(stream);
            
            if (!(JsonSerializer.Create().Deserialize(reader, typeof(ParkRunEventsByCountry)) is ParkRunEventsByCountry parkRuns))
                yield break;

            foreach (var parkRun in parkRuns.Events.Features)
            {
                yield return new Parkrun(
                    geoXmlId: parkRun.Id,
                    name: parkRun.Properties.EventShortName,
                    websiteDomain: parkRuns.Countries[parkRun.Properties.CountryCode.ToString()].Url,
                    websitePath: $"/{parkRun.Properties.EventName}",
                    region: null,
                    country: null,
                    latitude: parkRun.Geometry.Coordinates[1],
                    longitude: parkRun.Geometry.Coordinates[0]
                );
            }
        }
    }
}