using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ParkrunMap.FunctionsApp.SpecialEvents
{
    public class UpdateParkrunSpecialEventsQueueFunction
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public UpdateParkrunSpecialEventsQueueFunction(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [FunctionName(nameof(UpdateParkrunSpecialEventsQueueFunction))]
        public static async Task Run([QueueTrigger(QueueNames.UpdateParkrunSpecialEvents, Connection = "AzureWebJobsStorage")]
            UpdateParkrunSpecialEventsMessage message,
            ILogger logger, CancellationToken cancellationToken)
        {
            await Container.Instance.Resolve<UpdateParkrunSpecialEventsQueueFunction>(logger).Run(message, cancellationToken);
        }

        private async Task Run(UpdateParkrunSpecialEventsMessage message, CancellationToken cancellationToken)
        {
            var request = _mapper.Map<Data.Mongo.UpdateParkrunSpecialEvents.Request>(message);

            await _mediator.Send(request, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
