using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ParkrunMap.Data.Mongo;

namespace ParkrunMap.FunctionsApp.Course
{
    public class QueueUpCourseDownloadsFunction
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public QueueUpCourseDownloadsFunction(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [FunctionName("QueueUpCourseDownloadsFunction")]
        public static async Task Run([TimerTrigger("0 0 3 * * *")] TimerInfo myTimer,
            [Queue(QueueNames.DownloadCoursePage, Connection = "AzureWebJobsStorage")]
            IAsyncCollector<DownloadCourseMessage> collector, ILogger logger, CancellationToken cancellationToken)
        {
            await Container.Instance.Resolve<QueueUpCourseDownloadsFunction>(logger).Run(collector, cancellationToken);
        }

        private async Task Run(IAsyncCollector<DownloadCourseMessage> collector, CancellationToken cancellationToken)
        {
            var response = await  _mediator.Send(new QueryAllParkrunForWebsite.Request(), cancellationToken);

            foreach (var parkrun in response.Parkruns)
            {
                var message = _mapper.Map<DownloadCourseMessage>(parkrun);
                await collector.AddAsync(message, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
