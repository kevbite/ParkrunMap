using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ParkrunMap.Scraping.Course;

namespace ParkrunMap.FunctionsApp.Course
{
    public class DownloadCourseFunction
    {
        private readonly CoursePageDownloader _downloader;
        private readonly CourseParser _parser;
        private readonly ILogger _logger;

        public DownloadCourseFunction(CoursePageDownloader downloader, CourseParser parser, ILogger logger)
        {
            _downloader = downloader;
            _parser = parser;
            _logger = logger;
        }

        [FunctionName("DownloadCourseFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.DownloadCoursePage, Connection = "AzureWebJobsStorage")]DownloadCourseMessage message,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            await Container.Instance.Resolve<DownloadCourseFunction>(logger).Run(message, cancellationToken);
        }

        private async Task Run(DownloadCourseMessage message, CancellationToken cancellationToken)
        {
            using (var stream = await _downloader.DownloadAsync(message.WebsiteDomain, message.WebsitePath)
                .ConfigureAwait(false))
            {
                var courseDetails = await _parser.Parse(stream, message.WebsiteDomain)
                    .ConfigureAwait(false);

                _logger.LogInformation("Course Details {Description}", courseDetails.Description);
            }
        }
    }
}
