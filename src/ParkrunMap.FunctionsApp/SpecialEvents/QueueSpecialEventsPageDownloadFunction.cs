using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ParkrunMap.FunctionsApp.SpecialEvents
{
    public class QueueSpecialEventsPageDownloadFunction
    {
        [FunctionName(nameof(QueueSpecialEventsPageDownloadFunction))]
        public static async Task Run([TimerTrigger("15 7 * 12 1-7 *")]TimerInfo myTimer,
            [Queue(QueueNames.DownloadSpecialEventsPage, Connection = "AzureWebJobsStorage")]
            IAsyncCollector<DownloadSpecialEventsPageMessage> messageCollector,
            ILogger logger)
        {
            var func = Container.Instance.Resolve<QueueSpecialEventsPageDownloadFunction>(logger);

            await func.Run(messageCollector)
                .ConfigureAwait(false);
        }

        private readonly string[] _websiteDomains = {
            "www.parkrun.com.au",
            "www.parkrun.ie",
            "www.parkrun.it",
            "www.parkrun.co.nz",
            "www.parkrun.org.uk"
        };

        private async Task Run(IAsyncCollector<DownloadSpecialEventsPageMessage> messages)
        {
            foreach (var websiteDomain in _websiteDomains)
            {
                var message = new DownloadSpecialEventsPageMessage
                {
                    WebsiteDomain = websiteDomain
                };

                await messages.AddAsync(message);
            }
        }
    }
}
