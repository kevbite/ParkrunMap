using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;

namespace ParkrunMap.Data.Mongo
{
    public class QueryParkrunByRegion
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
                var ukPolygon = new double[,]
                {
                    {
                        -10.8544921875,
                        49.82380908513249
                    },
                    {
                        -10.8544921875,
                        59.478568831926395
                    },
                    {
                        2.021484375,
                        59.478568831926395
                    },
                    {
                        2.021484375,
                        49.82380908513249
                    },
                    {
                        -10.8544921875,
                        49.82380908513249
                    }
                };
                var filter = Builders<Domain.Parkrun>.Filter.GeoWithinPolygon(x => x.Location, ukPolygon);

                var parkruns = await (await _collection.FindAsync(filter, cancellationToken: cancellationToken)).ToListAsync();

                return new Response() {Parkruns = parkruns};
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