using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ParkrunMap.Data.Mongo;

namespace ParkrunMap.FunctionsApp.Course
{
    public class UpdateCourseDetailsFunction
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public UpdateCourseDetailsFunction(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [FunctionName("UpdateCourseDetailsFunction")]
        public static async Task Run([QueueTrigger("update-course-details", Connection = "AzureWebJobsStorage")]UpdateCourseDetailsMessage message, ILogger logger, CancellationToken cancellationToken)
        {
            await Container.Instance.Resolve<UpdateCourseDetailsFunction>(logger).Run(message, cancellationToken);
        }

        private async Task Run(UpdateCourseDetailsMessage message, CancellationToken cancellationToken)
        {
            var request = _mapper.Map<UpdateParkrunCourseDetails.Request>(message);

            await _mediator.Send(request, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
