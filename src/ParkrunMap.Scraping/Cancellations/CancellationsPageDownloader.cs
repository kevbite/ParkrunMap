using System.Net.Http;
using System.Threading.Tasks;

namespace ParkrunMap.Scraping.Cancellations
{
    public class CancellationsPageDownloader
    {
        private readonly HttpClient _httpClient;

        public CancellationsPageDownloader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<byte[]> DownloadAsync()
        {
            var response = await _httpClient
                .GetAsync("https://www.parkrun.org.uk/cancellations/")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync()
                .ConfigureAwait(false);
        }
    }
}
