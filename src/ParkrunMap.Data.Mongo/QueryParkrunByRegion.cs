using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ParkrunMap.Data.Mongo
{
    public class QueryParkrunByRegion
    {
        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IRegionPolygonProvider _regionPolygonProvider;
            private readonly IMediator _mediator;

            public Handler(IRegionPolygonProvider regionPolygonProvider, IMediator mediator)
            {
                _regionPolygonProvider = regionPolygonProvider;
                _mediator = mediator;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var polygon = _regionPolygonProvider.GetPolygon(request.Region);

                var response = await _mediator.Send(new QueryParkrunByPolygon.Request() {Polygon = polygon}, cancellationToken)
                    .ConfigureAwait(false);

                return new Response() {Parkruns = response.Parkruns};
            }
        }

        public class Response
        {
            public IReadOnlyCollection<Domain.Parkrun> Parkruns { get; set; }   
        }

        public class Request : IRequest<Response> 
        {
            public Region Region { get; set; }
        }

        public enum Region
        {
            UK = 1,
        }
    }
}
 