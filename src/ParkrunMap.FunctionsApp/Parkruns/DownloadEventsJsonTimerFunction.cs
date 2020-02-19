using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.FunctionsApp.Parkruns
{
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
        public static async Task Run([TimerTrigger("0 0 2 * * Monday")] TimerInfo myTimer,
            [Blob(DownloadFilePaths.EventsJson, Connection = "AzureWebJobsStorage")]
            CloudBlockBlob geoXml,
            ILogger logger)
        {
            var func = Container.Instance.Resolve<DownloadEventsJsonTimerFunction>(logger);

            await func.Run(geoXml).ConfigureAwait(false);
        }

        private async Task Run(CloudBlockBlob blob)
        {
            var bytes = await _eventsJsonDownloader.Download().ConfigureAwait(false);

            await _cloudBlockBlobUpdater.UpdateAsync(blob, bytes);
        }
    }
}