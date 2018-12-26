using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.FunctionsApp.DownloadGeoXml
{

    public class DownloadGeoXmlTimerFunction
    {
        private readonly ILogger _logger;
        private readonly GeoXmlDownloader _geoXmlDownloader;

        public DownloadGeoXmlTimerFunction(ILogger logger, GeoXmlDownloader geoXmlDownloader)
        {
            _logger = logger;
            _geoXmlDownloader = geoXmlDownloader;
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

            if (await blob.ExistsAsync().ConfigureAwait(false))
            {
                await blob.FetchAttributesAsync().ConfigureAwait(false);

                var md5 = CalculateMd5(bytes);
                if (blob.Properties.ContentMD5 == md5)
                {
                    _logger.LogInformation("Geo Xml MD5 '{MD5}' matches previously downloaded file", md5);
                    return;
                }
            }

            _logger.LogInformation("Uploading changed Geo Xml");
            await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length)
                .ConfigureAwait(false);
        }

        private static string CalculateMd5(byte[] bytes)
        {
            using (var md5 = MD5.Create())
            {
                var md5Bytes = md5.ComputeHash(bytes);

                return Convert.ToBase64String(md5Bytes);
            }
        }
    }
}