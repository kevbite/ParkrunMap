using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ParkrunMap.Scraping.SpecialEvents;

namespace ParkrunMap.FunctionsApp.SpecialEvents;

public class DownloadSpecialEventsPageFunction
{
    private readonly SpecialEventsPageDownloader _specialEventsPageDownloader;
    private readonly CloudBlockBlobUpdater _cloudBlockBlobUpdater;

    public DownloadSpecialEventsPageFunction(SpecialEventsPageDownloader specialEventsPageDownloader, CloudBlockBlobUpdater cloudBlockBlobUpdater)
    {
        _specialEventsPageDownloader = specialEventsPageDownloader;
        _cloudBlockBlobUpdater = cloudBlockBlobUpdater;
    }

    [FunctionName(nameof(DownloadSpecialEventsPageFunction))]
    public static async Task Run([QueueTrigger(QueueNames.DownloadSpecialEventsPage, Connection = "AzureWebJobsStorage")]
        DownloadSpecialEventsPageMessage message,
        [Blob(DownloadFilePaths.SpecialEventsHtml, FileAccess.ReadWrite, Connection = "AzureWebJobsStorage")]
        BlockBlobClient specialEventsHtml,
        ILogger logger)
    {
        await Container.Instance.Resolve<DownloadSpecialEventsPageFunction>(logger).Run(message, specialEventsHtml);
    }

    private async Task Run(DownloadSpecialEventsPageMessage message,
        BlockBlobClient blob)
    {
        var bytes = await _specialEventsPageDownloader.DownloadAsync(message.WebsiteDomain)
            .ConfigureAwait(false);
        
        await _cloudBlockBlobUpdater.UpdateAsync(blob, bytes);
    }
}