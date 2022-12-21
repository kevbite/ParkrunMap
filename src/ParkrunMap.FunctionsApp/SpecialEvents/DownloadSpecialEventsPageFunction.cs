using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using ParkrunMap.Scraping.SpecialEvents;

namespace ParkrunMap.FunctionsApp.SpecialEvents
{
    public class DownloadSpecialEventsPageFunction
    {
        private readonly SpecialEventsPageDownloader _specialEventsPageDownloader;
        private readonly CloudBlockBlobUpdater _cloudBlockBlobUpdater;

        public DownloadSpecialEventsPageFunction(SpecialEventsPageDownloader specialEventsPageDownloader, Func<ILogger, CloudBlockBlobUpdater> cloudBlockBlobUpdater, ILogger logger)
        {
            _specialEventsPageDownloader = specialEventsPageDownloader;
            _cloudBlockBlobUpdater = cloudBlockBlobUpdater(logger);
        }
 
        [FunctionName(nameof(DownloadSpecialEventsPageFunction))]
        public static async Task Run([TimerTrigger("15 7 * 12 1-7 *")]TimerInfo myTimer,
            [Blob(DownloadFilePaths.UKSpecialEventsHtml, FileAccess.ReadWrite, Connection = "AzureWebJobsStorage")]
            BlockBlobClient specialEventsHtml,
            ILogger logger)
        {
            var func = Container.Instance.Resolve<DownloadSpecialEventsPageFunction>(logger);

            await func.Run(specialEventsHtml)
                .ConfigureAwait(false);
        }

        private async Task Run(BlockBlobClient blob)
        {
            var bytes = await _specialEventsPageDownloader.DownloadAsync()
                .ConfigureAwait(false);

            await _cloudBlockBlobUpdater.UpdateAsync(blob, bytes);
        }
    }
}
