using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using ParkrunMap.Scraping.Parkruns;
using Xunit;

namespace ParkrunMap.Scraping.Tests.Cancellations
{
    public class CancellationsPageDownloaderTests
    {
        [Fact]
        public async Task ShouldDownloadPage()
        {
            using (var client = new HttpClient())
            {
                var downloader = new CancellationsPageDownloader(client);

                var bytes = await downloader.DownloadAsync()
                    .ConfigureAwait(false);

                var result = System.Text.Encoding.UTF8.GetString(bytes);

                result.Should().Contain("Forthcoming cancellations in the UK");
            }
        }
    }
}
