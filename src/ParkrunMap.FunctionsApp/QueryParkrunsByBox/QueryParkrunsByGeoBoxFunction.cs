using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using ParkrunMap.Data.Mongo;
using ParkrunMap.Domain;

namespace ParkrunMap.FunctionsApp.QueryParkrunsByBox
{
    public class QueryParkrunsByGeoBoxFunction
    {
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IPolygonCreator _polygonCreator;

        public QueryParkrunsByGeoBoxFunction(ILogger logger, IMediator mediator, IPolygonCreator polygonCreator)
        {
            _logger = logger;
            _mediator = mediator;
            _polygonCreator = polygonCreator;
        }

        [FunctionName("QueryParkrunsByGeoBox")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "parkruns/geobox")]
            HttpRequest req,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            return await Container.Instance.Resolve<QueryParkrunsByGeoBoxFunction>(logger).Run(req, cancellationToken);
        }

        private async Task<IActionResult> Run(HttpRequest req, CancellationToken cancellationToken)
        {
            var latValues = req.Query["lat"]
                .Select(x => new { Parsed = double.TryParse(x, out var o), Value = o})
                .ToArray();
            var lonValues = req.Query["lon"]
                .Select(x => new { Parsed = double.TryParse(x, out var o), Value = o })
                .ToArray();

            if (latValues.Length != 2 && latValues.All(x => x.Parsed)
                                      && lonValues.Length != 2 && lonValues.All(x => x.Parsed))
            {
                return new BadRequestObjectResult("Requires 2x lat, 2x lon");
            }

            var polygon = _polygonCreator.FromBox(latValues[0].Value, lonValues[0].Value, latValues[1].Value, lonValues[1].Value);
            var request = new QueryParkrunByPolygon.Request()
            {
                Polygon = polygon
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
                    GoogleMapIds = x.Course?.GoogleMapIds ?? new string[0]
                }
            }));

        }
    }
}
