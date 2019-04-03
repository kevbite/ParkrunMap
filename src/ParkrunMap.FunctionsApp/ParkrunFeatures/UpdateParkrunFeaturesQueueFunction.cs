using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ParkrunMap.Data.Mongo;

namespace ParkrunMap.FunctionsApp.ParkrunFeatures
{
    public class UpdateParkrunFeaturesQueueFunction
    {
        private readonly IMediator _mediator;

        public UpdateParkrunFeaturesQueueFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName(nameof(UpdateParkrunFeaturesQueueFunction))]
        public static async Task Run(
            [QueueTrigger(QueueNames.UpdateParkrunFeatures, Connection = "AzureWebJobsStorage")]UpdateParkrunFeatures.Request request,
            ILogger logger)
        {
            var func = Container.Instance.Resolve<UpdateParkrunFeaturesQueueFunction>(logger);

            await func.Run(request)
                .ConfigureAwait(false);
        }

        private async Task Run(UpdateParkrunFeatures.Request request)
        {
            await _mediator.Send(request)
                .ConfigureAwait(false);
        }
    }
}
