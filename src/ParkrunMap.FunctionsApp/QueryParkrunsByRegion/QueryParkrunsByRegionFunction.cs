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
using ParkrunMap.Domain;

namespace ParkrunMap.FunctionsApp.QueryParkrunsByRegion
{
    public class QueryParkrunsByRegionFunction
    {
        private readonly IMediator _mediator;

        public QueryParkrunsByRegionFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [FunctionName("QueryParkrunsByUKRegionFunction")]
        public static async Task<IActionResult> Run1(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "parkruns/uk")]
            HttpRequest req,
            ILogger logger, CancellationToken cancellationToken)
        {
            return await Container.Instance.Resolve<QueryParkrunsByRegionFunction>(logger).Run("UK", cancellationToken);
        }

        [FunctionName("QueryParkrunsByRegionFunction")]
        public static async Task<IActionResult> Run2(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "parkruns/region/{region}")]
            HttpRequest req, string region,
            ILogger logger, CancellationToken cancellationToken)
        {
            return await Container.Instance.Resolve<QueryParkrunsByRegionFunction>(logger)
                .Run(region, cancellationToken);
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
                {
                    id = x.Id,
                    x.Name,
                    Uri = new Uri($"https://{x.Website.Domain}{x.Website.Path}"),
                    lat = x.Location.Coordinates.Latitude,
                    lon = x.Location.Coordinates.Longitude,
                    cancellations = x.Cancellations ?? new Cancellation[0],
                    course = new
                    {
                        description = x.Course?.Description,
                        GoogleMapIds = x.Course?.GoogleMapIds ?? Array.Empty<string>()
                    },
                    specialEvents = new
                    {
                        christmasDay = x.SpecialEvents?.ChristmasDay ?? Array.Empty<int>(),
                        newYearsDay = x.SpecialEvents?.NewYearsDay ?? Array.Empty<int>(),
                    }
                }));
            }

            return new BadRequestObjectResult($"Unknown region '{region}'");
        }
    }
}