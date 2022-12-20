using System.Net.Http;
using System.Threading.Tasks;

namespace ParkrunMap.Scraping.SpecialEvents
{
    public class SpecialEventsPageDownloader
    {
        private readonly HttpClient _httpClient;

        public SpecialEventsPageDownloader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<byte[]> DownloadAsync()
        {
            var response = await _httpClient
                .GetAsync("https://www.parkrun.org.uk/special-events/")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync()
                .ConfigureAwait(false);
        }
    }
}
