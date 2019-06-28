using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ParkrunMap.Data.Mongo;

namespace ParkrunMap.FunctionsApp.ParkrunStatistics
{
    public class UpdateParkrunStatisticsFunction
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public UpdateParkrunStatisticsFunction(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [FunctionName(nameof(UpdateParkrunStatisticsFunction))]
        public static async Task Run([QueueTrigger(QueueNames.UpdateParkrunStatistics, Connection = "AzureWebJobsStorage")]UpdateParkrunStatistics.Request message, ILogger logger, CancellationToken cancellationToken)
        {
            await Container.Instance.Resolve<UpdateParkrunStatisticsFunction>(logger).Run(message, cancellationToken);
        }

        private async Task Run(UpdateParkrunStatistics.Request message, CancellationToken cancellationToken)
        {
            var request = _mapper.Map<UpdateParkrunCourseDetails.Request>(message);

            await _mediator.Send(request, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
