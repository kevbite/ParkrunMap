using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ParkrunMap.FunctionsApp.ParkrunCancellation
{
    public class AddParkrunCancellationQueueFunction
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public AddParkrunCancellationQueueFunction(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [FunctionName(nameof(AddParkrunCancellationQueueFunction))]
        public static async Task Run([QueueTrigger(QueueNames.AddParkrunCancellation, Connection = "AzureWebJobsStorage")]
            AddParkrunCancellationMessage message,
            ILogger logger, CancellationToken cancellationToken)
        {
            await Container.Instance.Resolve<AddParkrunCancellationQueueFunction>(logger).Run(message, cancellationToken);
        }

        private async Task Run(AddParkrunCancellationMessage message, CancellationToken cancellationToken)
        {
            var request = _mapper.Map<Data.Mongo.AddParkrunCancellation.Request>(message);

            await _mediator.Send(request, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
