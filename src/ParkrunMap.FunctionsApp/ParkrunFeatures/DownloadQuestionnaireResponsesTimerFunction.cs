using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ParkrunMap.FunctionsApp.ParkrunFeatures
{
    public class DownloadQuestionnaireResponsesTimerFunction
    {
        private readonly QuestionnaireResponseDownloader _downloader;

        public DownloadQuestionnaireResponsesTimerFunction(QuestionnaireResponseDownloader downloader)
        {
            _downloader = downloader;
        }

        [FunctionName(nameof(DownloadQuestionnaireResponsesTimerFunction))]
        public static async Task Run([TimerTrigger("0 0 2 * * Monday")]TimerInfo myTimer,
            [Queue(QueueNames.AggregateQuestionnaireResponses, Connection = "AzureWebJobsStorage")]
            IAsyncCollector<ParkrunQuestionnaireResponsesMessage> messageCollector, ILogger logger)
        {
            var func = Container.Instance.Resolve<DownloadQuestionnaireResponsesTimerFunction>(logger);

            await func.Run(messageCollector)
                .ConfigureAwait(false);
        }

        private async Task Run(IAsyncCollector<ParkrunQuestionnaireResponsesMessage> messageCollector)
        {
            var responses = await _downloader.DownloadQuestionnaireResponsesAsync()
                .ConfigureAwait(false);

            foreach (var group in responses.GroupBy(x => new {x.WebsiteDomain, x.WebsitePath}))
            {
                var message = new ParkrunQuestionnaireResponsesMessage
                {
                    WebsiteDomain = group.Key.WebsiteDomain,
                    WebsitePath = group.Key.WebsitePath,
                    Responses = group.ToArray()
                };

                await messageCollector.AddAsync(message)
                    .ConfigureAwait(false);
            }
        }
    }
}