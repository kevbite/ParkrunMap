using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ParkrunMap.Data.Mongo
{
    public class QueryFirstParkrunForWebsite
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
                var query = Builders<Domain.Parkrun>.Filter.Not(Builders<Domain.Parkrun>.Filter.In(x => x.Id, request.ExceptIds));

                var projection = Builders<Domain.Parkrun>.Projection.Expression(x =>
                    new Response.ParkrunResponse() {Id = x.Id, WebsiteDomain = x.Website.Domain, WebsitePath = x.Website.Path});

                var parkruns = await _collection.Aggregate()
                    .Match(query)
                    .Project(projection)
                    .Limit(1)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                return new Response() { Parkrun = parkruns.SingleOrDefault() };
            }
        }

        public class Response
        {
            public ParkrunResponse Parkrun { get; set; }

            public class ParkrunResponse
            {
                public ObjectId Id { get; set; }

                public string WebsiteDomain { get; set; }

                public string WebsitePath { get; set; }
            }
        }

        public class Request : IRequest<Response>
        {
            public IReadOnlyCollection<ObjectId> ExceptIds { get; set; } = new ObjectId[0];
        }
    }
}