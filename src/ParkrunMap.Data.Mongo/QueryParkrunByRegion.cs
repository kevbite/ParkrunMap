using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ParkrunMap.Data.Mongo
{
    public class QueryParkrunByRegion
    {
        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IMongoCollection<Domain.Parkrun> _collection;
            private readonly IRegionPolygonProvider _regionPolygonProvider;
            private readonly Func<DateTime> _todayFunc;

            public Handler(IMongoCollection<Domain.Parkrun> collection, IRegionPolygonProvider regionPolygonProvider, Func<DateTime> todayFunc)
            {
                _collection = collection;
                _regionPolygonProvider = regionPolygonProvider;
                _todayFunc = todayFunc;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var polygon = _regionPolygonProvider.GetPolygon(request.Region);

                var filter = Builders<Domain.Parkrun>.Filter.GeoWithinPolygon(x => x.Location, polygon);

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

                var parkruns = await _collection.Aggregate()
                    .Match(filter)
                    .Project<Domain.Parkrun>(project)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
                
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
 