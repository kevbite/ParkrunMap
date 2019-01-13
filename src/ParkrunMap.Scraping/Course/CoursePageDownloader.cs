using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ParkrunMap.Scraping.Course
{
    public class CoursePageDownloader
    {
        private readonly HttpClient _httpClient;

        public CoursePageDownloader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Stream> DownloadAsync(string domain, string path)
        {
            var response = await _httpClient
                .GetAsync($"https://{domain}{path}/course/")
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStreamAsync()
                .ConfigureAwait(false);
        }
    }
}
