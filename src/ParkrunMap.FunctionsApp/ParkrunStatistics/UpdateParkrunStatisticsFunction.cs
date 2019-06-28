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

        public UpdateParkrunStatisticsFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName(nameof(UpdateParkrunStatisticsFunction))]
        public static async Task Run([QueueTrigger(QueueNames.UpdateParkrunStatistics, Connection = "AzureWebJobsStorage")]UpdateParkrunStatistics.Request message, ILogger logger, CancellationToken cancellationToken)
        {
            await Container.Instance.Resolve<UpdateParkrunStatisticsFunction>(logger).Run(message, cancellationToken);
        }

        private async Task Run(UpdateParkrunStatistics.Request message, CancellationToken cancellationToken)
        {
            await _mediator.Send(message, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
