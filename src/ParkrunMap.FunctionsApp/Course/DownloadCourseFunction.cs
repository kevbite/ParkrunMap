using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using ParkrunMap.Scraping.Course;

namespace ParkrunMap.FunctionsApp.Course
{
    public class DownloadCourseFunction
    {
        private readonly CoursePageDownloader _downloader;
        private readonly CourseParser _parser;
        private readonly ILogger _logger;
        private readonly CloudBlockBlobUpdater _cloudBlockBlobUpdater;

        public DownloadCourseFunction(CoursePageDownloader downloader, CourseParser parser, ILogger logger, Func<ILogger, CloudBlockBlobUpdater> cloudBlockBlobUpdater)
        {
            _downloader = downloader;
            _parser = parser;
            _logger = logger;
            _cloudBlockBlobUpdater = cloudBlockBlobUpdater(logger);
        }

        [FunctionName("DownloadCourseFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.DownloadCoursePage, Connection = "AzureWebJobsStorage")]DownloadCourseMessage message,
            [Blob("downloads/course/{WebsiteDomain}{WebsitePath}.html", Connection = "AzureWebJobsStorage")] BlobClient htmlBlockBlob,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            await Container.Instance.Resolve<DownloadCourseFunction>(logger).Run(message, htmlBlockBlob, cancellationToken);
        }

        private async Task Run(DownloadCourseMessage message, BlobClient htmlBlockBlob, CancellationToken cancellationToken)
        {
            using (var stream = await _downloader.DownloadAsync(message.WebsiteDomain, message.WebsitePath, cancellationToken)
                .ConfigureAwait(false))
            {
                using (var ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms)
                        .ConfigureAwait(false);

                    await _cloudBlockBlobUpdater.UpdateAsync(htmlBlockBlob, ms.ToArray())
                        .ConfigureAwait(false);
                }
            }
        }
    }
}
