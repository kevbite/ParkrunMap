using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ParkrunMap.Scraping.Parkruns.Data;

namespace ParkrunMap.Scraping.Parkruns
{
    public static class ParkRunEventsJsonParser
    {
        public static IReadOnlyCollection<Parkrun> Parse(Stream stream)
        {
            using var reader = new StreamReader(stream);

            return JsonSerializer.Create().Deserialize(reader, typeof(ParkRunEventsByCountry)) is ParkRunEventsByCountry parkRuns
                ? parkRuns.Events.Features
                    .Select(p => new Parkrun(
                        geoXmlId: p.Id,
                        name: p.Properties.EventShortName,
                        websiteDomain: parkRuns.Countries[p.Properties.CountryCode.ToString()].Url,
                        websitePath: $"/{p.Properties.EventName}",
                        region: null,
                        country: null,
                        latitude: p.Geometry.Coordinates[1],
                        longitude: p.Geometry.Coordinates[0]))
                    .ToList()
                : new List<Parkrun>();
        }
    }
}