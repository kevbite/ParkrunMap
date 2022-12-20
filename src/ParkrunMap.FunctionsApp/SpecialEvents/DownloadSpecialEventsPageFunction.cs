using System;
using System.Threading.Tasks;
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
 
        [FunctionName("DownloadSpecialEventsPageFunction")]
        public static async Task Run([TimerTrigger("0 0 */3 * * *")]TimerInfo myTimer,
            [Blob(DownloadFilePaths.UKSpecialEventsHtml, Connection = "AzureWebJobsStorage")]
            CloudBlockBlob specialEventsHtml,
            ILogger logger)
        {
            var func = Container.Instance.Resolve<DownloadSpecialEventsPageFunction>(logger);

            await func.Run(specialEventsHtml)
                .ConfigureAwait(false);
        }

        private async Task Run(CloudBlockBlob blob)
        {
            var bytes = await _specialEventsPageDownloader.DownloadAsync()
                .ConfigureAwait(false);

            await _cloudBlockBlobUpdater.UpdateAsync(blob, bytes);
        }
    }
}
