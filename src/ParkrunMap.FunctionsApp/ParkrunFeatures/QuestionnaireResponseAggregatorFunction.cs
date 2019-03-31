using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ParkrunMap.FunctionsApp.ParkrunFeatures
{
    public class QuestionnaireResponseAggregatorFunction
    {

        [FunctionName("QuestionnaireResponseAggregatorFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.AggregateQuestionnaireResponses, Connection = "AzureWebJobsStorage")]
            ParkrunQuestionnaireResponsesMessage message,
            ILogger logger)
        {
            var func = Container.Instance.Resolve<QuestionnaireResponseAggregatorFunction>(logger);

            await func.Run(message)
                .ConfigureAwait(false);
        }

        private async Task Run(ParkrunQuestionnaireResponsesMessage message)
        {

        }
    }
}