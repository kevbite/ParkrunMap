using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using ParkrunMap.Scraping.Course;
using Xunit;

namespace ParkrunMap.Scraping.Tests.Course
{
    public class CoursePageDownloaderTests
    {
        [Theory]
        [InlineData("/black-park")]
        [InlineData("/blackpark")]
        [InlineData("/heslington")]
        public async Task ShouldDownloadPage(string path)
        {
            using var client = new HttpClient(new RedirectHandler(new HttpClientHandler()));
            var downloader = new CoursePageDownloader(client);

            var bytes = await downloader.DownloadAsync("www.parkrun.org.uk", path, CancellationToken.None)
                .ConfigureAwait(false);

            var result = Encoding.UTF8.GetString(bytes);  

            result.Should().Contain("The Course");
        }
    }
}
