using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ParkrunMap.Data.Mongo
{
    public class QueryAllParkrunForWebsite
    {
        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IMongoCollection<Domain.Parkrun> _collection;

            public Handler(IMongoCollection<Domain.Parkrun> collection)
            {
                _collection = collection;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var projection = Builders<Domain.Parkrun>.Projection.Expression(x =>
                    new Response.Parkrun() {Id = x.Id, WebsiteDomain = x.Website.Domain, WebsitePath = x.Website.Path});

                var parkruns = await _collection.Aggregate()
                    .Project(projection)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                return new Response() { Parkruns = parkruns };
            }
        }

        public class Response
        {
            public IReadOnlyCollection<Parkrun> Parkruns { get; set; }

            public class Parkrun
            {
                public ObjectId Id { get; set; }

                public string WebsiteDomain { get; set; }

                public string WebsitePath { get; set; }
            }
        }

        public class Request : IRequest<Response>
        {
        }
    }
}