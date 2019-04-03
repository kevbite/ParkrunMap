using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ParkrunMap.Data.Mongo;

namespace ParkrunMap.FunctionsApp.ParkrunFeatures
{
    public class QuestionnaireResponseAggregatorQueueFunction
    {
        private readonly ParkrunQuestionnaireResponseAggregator _aggregator;
        private readonly IMapper _mapper;

        public QuestionnaireResponseAggregatorQueueFunction(ParkrunQuestionnaireResponseAggregator aggregator,
            IMapper mapper)
        {
            _aggregator = aggregator;
            _mapper = mapper;
        }

        [FunctionName(nameof(QuestionnaireResponseAggregatorQueueFunction))]
        public static async Task Run(
            [QueueTrigger(QueueNames.AggregateQuestionnaireResponses, Connection = "AzureWebJobsStorage")]
            ParkrunQuestionnaireResponsesMessage message,
            [Queue(QueueNames.UpdateParkrunFeatures, Connection = "AzureWebJobsStorage")]IAsyncCollector<UpdateParkrunFeatures.Request> requests,
            ILogger logger, CancellationToken cancellationToken)
        {
            var func = Container.Instance.Resolve<QuestionnaireResponseAggregatorQueueFunction>(logger);

            await func.Run(message, requests, cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task Run(ParkrunQuestionnaireResponsesMessage message, IAsyncCollector<UpdateParkrunFeatures.Request> requests, CancellationToken cancellationToken)
        {
            var aggregation = _aggregator.Aggregate(message);

            var request = _mapper.Map<UpdateParkrunFeatures.Request>(aggregation);

            await requests.AddAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}