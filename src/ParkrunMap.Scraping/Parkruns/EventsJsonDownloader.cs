using System.Net.Http;
using System.Threading.Tasks;

namespace ParkrunMap.Scraping.Parkruns
{
    public class EventsJsonDownloader
    {
        private readonly HttpClient _httpClient;

        public EventsJsonDownloader(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<byte[]> Download()
        {
            var response = await _httpClient
                .GetAsync("https://images.parkrun.com/events.json")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }
    }
}