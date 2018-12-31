using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.FunctionsApp.DownloadGeoXml
{
    public class DownloadGeoXmlTimerFunction
    {
        private readonly GeoXmlDownloader _geoXmlDownloader;
        private readonly CloudBlockBlobUpdater _cloudBlockBlobUpdater;

        public DownloadGeoXmlTimerFunction(GeoXmlDownloader geoXmlDownloader, Func<ILogger, CloudBlockBlobUpdater> cloudBlockBlobUpdater, ILogger logger)
        {
            _geoXmlDownloader = geoXmlDownloader;
            _cloudBlockBlobUpdater = cloudBlockBlobUpdater(logger);
        }

        [FunctionName("DownloadGeoXmlTimerFunction")]
        public static async Task Run([TimerTrigger("0 0 2 * * *", RunOnStartup = true)]
            TimerInfo myTimer,
            [Blob("downloads/geo.xml", Connection = "AzureWebJobsStorage")]
            CloudBlockBlob geoXml,
            ILogger logger)
        {
            var func = Container.Instance.Resolve<DownloadGeoXmlTimerFunction>(logger);

            await func.Run(geoXml)
                .ConfigureAwait(false);
        }

        private async Task Run(CloudBlockBlob blob)
        {
            var bytes = await _geoXmlDownloader.Download().ConfigureAwait(false);

            await _cloudBlockBlobUpdater.UpdateAsync(blob, bytes);
        }
    }
}