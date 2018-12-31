using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ParkrunMap.FunctionsApp.Parkruns
{
    public class UpsertParkrunQueueFunction
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public UpsertParkrunQueueFunction(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [FunctionName(nameof(UpsertParkrunQueueFunction))]
        public static async Task Run([QueueTrigger(QueueNames.UpsertParkrun, Connection = "AzureWebJobsStorage")]UpsertParkrunMessage message,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            await Container.Instance.Resolve<UpsertParkrunQueueFunction>(logger).Run(message, cancellationToken);
        }

        private async Task Run(UpsertParkrunMessage message, CancellationToken cancellationToken)
        {
            var request = _mapper.Map<Data.Mongo.UpsertParkrun.Request>(message);

            await _mediator.Send(request, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
