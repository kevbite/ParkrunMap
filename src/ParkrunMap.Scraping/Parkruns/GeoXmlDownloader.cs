using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ParkrunMap.Scraping.Parkruns
{
    public class GeoXmlDownloader
    {
        private readonly HttpClient _httpClient;

        public GeoXmlDownloader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<byte[]> Download()
        {
            var response = await _httpClient
                .GetAsync("https://www.parkrun.org.uk/wp-content/themes/parkrun/xml/geo.xml")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync()
                .ConfigureAwait(false);
        }
    }
}
