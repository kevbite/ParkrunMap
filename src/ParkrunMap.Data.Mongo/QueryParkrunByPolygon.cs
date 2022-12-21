using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ParkrunMap.Data.Mongo
{
    public class QueryParkrunByPolygon
    {
        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IMongoCollection<Domain.Parkrun> _collection;
            private readonly Func<DateTime> _todayFunc;

            public Handler(IMongoCollection<Domain.Parkrun> collection, Func<DateTime> todayFunc)
            {
                _collection = collection;
                _todayFunc = todayFunc;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var filter = Builders<Domain.Parkrun>.Filter.GeoWithinPolygon(x => x.Location, request.Polygon);

                var today = _todayFunc();
                var cancellationsFilter = new BsonDocument("$filter", new BsonDocument
                {
                    {"input", "$Cancellations"},
                    {"as", "c"},
                    {"cond", new BsonDocument("$and", new BsonArray(new []
                    {
                        new BsonDocument("$gte", new BsonArray(new BsonValue []{"$$c.Date", today})),
                        new BsonDocument("$lte", new BsonArray(new BsonValue []{"$$c.Date", today.AddDays(7 * 3)})),
                    }))}
                });

                var project = new BsonDocument();
                project.Add("Cancellations", cancellationsFilter);
                project.Add("_id", 1);
                project.Add("Name", 1);
                project.Add("Website", 1);
                project.Add("Location", 1);
                project.Add("Country", 1);
                project.Add("Region", 1);
                project.Add("GeoXmlId", 1);
                project.Add("Course", 1);
                project.Add("Terrain", 1);
                project.Add("Features", 1);
                project.Add("Statistics", 1);
                project.Add("SpecialEvents", 1);

                var parkruns = await _collection.Aggregate()
                    .Match(filter)
                    .Project<Domain.Parkrun>(project)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                return new Response() { Parkruns = parkruns };
            }
        }

        public class Response
        {
            public IReadOnlyCollection<Domain.Parkrun> Parkruns { get; set; }
        }

        public class Request : IRequest<Response>
        {
            public double[,] Polygon { get; set; }
        }
    }
}