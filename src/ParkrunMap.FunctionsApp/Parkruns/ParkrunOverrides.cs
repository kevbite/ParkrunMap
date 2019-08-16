using System.Collections.Generic;
using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.FunctionsApp.Parkruns
{
    public class ParkrunOverrides
    {
        private static readonly IReadOnlyDictionary<(string websiteDomain, string websitePath), (double latitude, double longitude)> LocationOverrides
            = new Dictionary<(string, string), (double, double)>
            {
                {("www.parkrun.ie", "/tymon"), (53.304650, -6.341203)}
            };

        public EventsJsonParkrun Apply(EventsJsonParkrun parkrun)
        {
            var builder = new ParkrunBuilder(parkrun);
            if (LocationOverrides.TryGetValue((parkrun.WebsiteDomain, parkrun.WebsitePath), out var location))
            {
                builder.SetLocation(location);
            }

            return builder.Build();
        }
    }
}