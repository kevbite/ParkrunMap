using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ParkrunMap.FunctionsApp
{
    public class CloudBlockBlobUpdater
    {
        private readonly ILogger _logger;

        public CloudBlockBlobUpdater(ILogger logger)
        {
            _logger = logger;
        }

        public async Task UpdateAsync(ICloudBlob blob, byte[] bytes)
        {
            if (await blob.ExistsAsync().ConfigureAwait(false))
            {
                await blob.FetchAttributesAsync().ConfigureAwait(false);

                var md5 = CalculateMd5(bytes);
                if (blob.Properties.ContentMD5 == md5)
                {
                   _logger.LogInformation("Current MD5 '{MD5}' matches previously downloaded file of {BlobUri}", md5, blob.Uri);
                    return;
                }
            }

            _logger.LogInformation("Uploading changed for {BlobUri}", blob.Uri);

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