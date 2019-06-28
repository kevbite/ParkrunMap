using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo
{
    public class UpdateParkrunStatistics
    {
        public class Handler : AsyncRequestHandler<Request>
        {
            private readonly IMongoCollection<Parkrun> _collection;

            public Handler(IMongoCollection<Parkrun> collection)
            {
                _collection = collection;
            }

            protected override async Task Handle(Request request, CancellationToken cancellationToken)
            {
                var filter = Builders<Parkrun>.Filter.Eq(x => x.Website.Path, request.WebsitePath)
                             & Builders<Parkrun>.Filter.Eq(x => x.Website.Domain, request.WebsiteDomain);

                var update = Builders<Parkrun>.Update
                        .Set(x => x.Statistics.TotalEvents, request.TotalEvents)
                        .Set(x => x.Statistics.TotalRunners, request.TotalRunners)
                        .Set(x => x.Statistics.TotalRuns, request.TotalRuns)
                        .Set(x => x.Statistics.AverageRunnersPerWeek, request.AverageRunnersPerWeek)
                        .Set(x => x.Statistics.AverageSecondsRan, request.AverageSecondsRan)
                        .Set(x => x.Statistics.TotalSecondsRan, request.TotalSecondsRan)
                        .Set(x => x.Statistics.BiggestAttendance, request.BiggestAttendance)
                        .Set(x => x.Statistics.TotalKmDistanceRan, request.TotalKmDistanceRan)
                        .Set(x => x.Statistics.TotalEvents, request.TotalEvents)
                    ;

                var updateResult = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

                if (updateResult.MatchedCount == 0)
                {
                    throw new Exception($"Could not find parkrun with website {request.WebsiteDomain}{request.WebsitePath}");
                }
            }
        }

        public class Request : IRequest
        {
            public string WebsiteDomain { get; set; }

            public string WebsitePath { get; set; }

            public int TotalEvents { get; set; }

            public int TotalRunners { get; set; }

            public int TotalRuns { get; set; }

            public double AverageRunnersPerWeek { get; set; }

            public int AverageSecondsRan { get; set; }

            public long TotalSecondsRan { get; set; }

            public int BiggestAttendance { get; set; }

            public int TotalKmDistanceRan { get; set; }
        }
    }
}