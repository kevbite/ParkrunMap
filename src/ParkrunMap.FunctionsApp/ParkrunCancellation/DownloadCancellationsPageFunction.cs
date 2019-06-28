using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using ParkrunMap.Scraping.Cancellations;
using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.FunctionsApp.ParkrunCancellation
{
    public class DownloadCancellationsPageFunction
    {
        private readonly CancellationsPageDownloader _cancellationsPageDownloader;
        private readonly CloudBlockBlobUpdater _cloudBlockBlobUpdater;

        public DownloadCancellationsPageFunction(CancellationsPageDownloader cancellationsPageDownloader, Func<ILogger, CloudBlockBlobUpdater> cloudBlockBlobUpdater, ILogger logger)
        {
            _cancellationsPageDownloader = cancellationsPageDownloader;
            _cloudBlockBlobUpdater = cloudBlockBlobUpdater(logger);
        }
 
        [FunctionName("DownloadCancellationsPageFunction")]
        public static async Task Run([TimerTrigger("0 0 */2 * * *")]TimerInfo myTimer,
            [Blob(DownloadFilePaths.CancellationsHtml, Connection = "AzureWebJobsStorage")]
            CloudBlockBlob geoXml,
            ILogger logger)
        {
            var func = Container.Instance.Resolve<DownloadCancellationsPageFunction>(logger);

            await func.Run(geoXml)
                .ConfigureAwait(false);
        }

        private async Task Run(CloudBlockBlob blob)
        {
            var bytes = await _cancellationsPageDownloader.DownloadAsync()
                .ConfigureAwait(false);

            await _cloudBlockBlobUpdater.UpdateAsync(blob, bytes);
        }
    }
}
