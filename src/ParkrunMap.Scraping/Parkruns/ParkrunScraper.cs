using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ParkrunMap.Scraping.Parkruns
{
    public class ParkrunScraper
    {
        private readonly HttpClient _httpClient;
        private readonly GeoXmlParser _geoXmlParser;

        public ParkrunScraper(HttpClient httpClient, GeoXmlParser geoXmlParser)
        {
            _httpClient = httpClient;
            _geoXmlParser = geoXmlParser;
        }

        public async Task<IReadOnlyCollection<Parkrun>> GetParkruns()
        {
            var response = await _httpClient.GetAsync("https://www.parkrun.org.uk/wp-content/themes/parkrun/xml/geo.xml")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                return _geoXmlParser.Parse(stream);
            }
        }
    }


}
