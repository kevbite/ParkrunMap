using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo
{
    public class AddParkrunCancellation
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

                var cancellationsFilter = Builders<Cancellation>.Filter.Eq(x => x.Date, request.Date);

                var pullCancellation = Builders<Parkrun>.Update.PullFilter(x => x.Cancellations, cancellationsFilter);
                
                await _collection.UpdateOneAsync(filter, pullCancellation, cancellationToken: cancellationToken);

                var pushCancellation = Builders<Parkrun>.Update.Push(x => x.Cancellations,
                    new Cancellation() { Date = request.Date, Reason = request.Reason });

                var updateResult = await _collection.UpdateOneAsync(filter, pushCancellation, cancellationToken: cancellationToken);

                if (updateResult.MatchedCount == 0)
                {
                    throw new Exception($"Could not find parkrun with website {request.WebsiteDomain}{request.WebsitePath}");
                }
            }
        }

        public class Request : IRequest
        {
            public DateTime Date { get; set; }

            public string Name { get; set; }

            public string WebsiteDomain { get; set; }

            public string WebsitePath { get; set; }

            public string Reason { get; set; }
        }
    }
}