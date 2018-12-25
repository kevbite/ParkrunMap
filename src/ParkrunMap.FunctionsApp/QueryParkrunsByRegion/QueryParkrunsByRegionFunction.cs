using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ParkrunMap.Data.Mongo;

namespace ParkrunMap.FunctionsApp.QueryParkrunsByRegion
{
    public class QueryParkrunsByRegionFunction
    {
        private readonly IMediator _mediator;

        public QueryParkrunsByRegionFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("QueryParkrunsByRegionFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "parkruns/{region}")] HttpRequest req, string region,
            ILogger logger, CancellationToken cancellationToken)
        {
            return await Container.Instance.Resolve<QueryParkrunsByRegionFunction>(logger).Run(region, cancellationToken);
        }

        private async Task<IActionResult> Run(string region, CancellationToken cancellationToken)
        {
            if (Enum.TryParse(region, true, out QueryParkrunByRegion.Region result))
            {
                var request = new QueryParkrunByRegion.Request()
                {
                    Region = result
                };

                var response = await _mediator.Send(request, cancellationToken)
                    .ConfigureAwait(false);

                return new OkObjectResult(response.Parkruns.Select(x => new
                    { x.Name, x.Uri, lat = x.Location.Coordinates.Latitude, lon = x.Location.Coordinates.Longitude }));
            }
          
            return new BadRequestObjectResult($"Unknown region '{region}'");
        }
    }
}
