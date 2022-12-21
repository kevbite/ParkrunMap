using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.FunctionsApp.Parkruns
{
    //https://images.parkrun.com/events.json
    public class DownloadEventsJsonTimerFunction
    {
        private readonly EventsJsonDownloader _eventsJsonDownloader;
        private readonly CloudBlockBlobUpdater _cloudBlockBlobUpdater;

        public DownloadEventsJsonTimerFunction(EventsJsonDownloader eventsJsonDownloader, Func<ILogger, CloudBlockBlobUpdater> cloudBlockBlobUpdater, ILogger logger)
        {
            _eventsJsonDownloader = eventsJsonDownloader;
            _cloudBlockBlobUpdater = cloudBlockBlobUpdater(logger);
        }

        [FunctionName(nameof(DownloadEventsJsonTimerFunction))]
        public static async Task Run([TimerTrigger("0 0 2 * * Monday")]
            TimerInfo myTimer,
            [Blob(DownloadFilePaths.EventsJson, Connection = "AzureWebJobsStorage")]
            BlockBlobClient geoXml,
            ILogger logger)
        {
            var func = Container.Instance.Resolve<DownloadEventsJsonTimerFunction>(logger);

            await func.Run(geoXml)
                .ConfigureAwait(false);
        }

        private async Task Run(BlockBlobClient blob)
        {
            var bytes = await _eventsJsonDownloader.Download().ConfigureAwait(false);

            await _cloudBlockBlobUpdater.UpdateAsync(blob, bytes);
        }
    }
}