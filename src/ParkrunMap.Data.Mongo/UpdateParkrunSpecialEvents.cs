using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo
{
    public class UpdateParkrunSpecialEvents
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

                var update =
                    (request.Type, request.IsRunning) switch
                    {
                        (SpecialEventType.ChristmasDay, true) => Builders<Parkrun>.Update.AddToSet(
                            x => x.SpecialEvents.ChristmasDay, request.Year),
                        (SpecialEventType.ChristmasDay, false) => Builders<Parkrun>.Update.Pull(
                            x => x.SpecialEvents.ChristmasDay, request.Year),
                        (SpecialEventType.NewYearsDay, true) => Builders<Parkrun>.Update.AddToSet(
                            x => x.SpecialEvents.NewYearsDay, request.Year),
                        (SpecialEventType.NewYearsDay, false) => Builders<Parkrun>.Update.Pull(
                            x => x.SpecialEvents.NewYearsDay, request.Year),
                        _ => throw new InvalidOperationException($"Unknown special event type {request.Type}")
                    };

                var updateResult =
                    await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

                if (updateResult.MatchedCount == 0)
                {
                    throw new Exception(
                        $"Could not find parkrun with website {request.WebsiteDomain}{request.WebsitePath}");
                }
            }
        }

        public class Request : IRequest
        {
            public string WebsiteDomain { get; set; }

            public string WebsitePath { get; set; }

            public int Year { get; set; }

            public SpecialEventType Type { get; set; }

            public bool IsRunning { get; set; }
        }
    }

    public enum SpecialEventType
    {
        ChristmasDay,
        NewYearsDay
    }
}