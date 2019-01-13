using System.IO;
using System.Net.Http;
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
            using (var client = new HttpClient(new RedirectHandler(new HttpClientHandler())))
            {
                var downloader = new CoursePageDownloader(client);

                var stream = await downloader.DownloadAsync("www.parkrun.org.uk", path)
                    .ConfigureAwait(false);

                var streamReader = new StreamReader(stream);

                var result = streamReader.ReadToEnd();

                result.Should().Contain("The Course");
            }
        }
    }
}
