using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Logging;

namespace ParkrunMap.FunctionsApp
{
    public class CloudBlockBlobUpdater
    {
        private readonly ILogger _logger;

        public CloudBlockBlobUpdater(ILogger logger)
        {
            _logger = logger;
        }

        public async Task UpdateAsync(BlockBlobClient blob, byte[] bytes)
        {
            _logger.LogInformation("Uploading changed for {BlobUri}", blob.Uri);

            await using var writeStream = await blob.OpenWriteAsync(true)
                .ConfigureAwait(false);
            await writeStream.WriteAsync(bytes);
            await writeStream.FlushAsync();
        }

        private static byte[] CalculateMd5(byte[] bytes)
        {
            using var md5 = MD5.Create();
            return md5.ComputeHash(bytes);
        }
    }
}