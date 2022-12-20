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


                UpdateDefinition<Parkrun> update = null;
                if (request.Type == SpecialEventType.ChristmasDay)
                {
                    update =
                        request.IsRunning
                            ? Builders<Parkrun>.Update.AddToSet(x => x.SpecialEvents.ChristmasDay, request.Year)
                            : Builders<Parkrun>.Update.Pull(x => x.SpecialEvents.ChristmasDay, request.Year);
                }
                else if (request.Type == SpecialEventType.NewYearsDay)
                {
                    update =
                        request.IsRunning
                            ? Builders<Parkrun>.Update.AddToSet(x => x.SpecialEvents.NewYearsDay, request.Year)
                            : Builders<Parkrun>.Update.Pull(x => x.SpecialEvents.NewYearsDay, request.Year);
                }
                else
                {
                    throw new InvalidOperationException($"Unknown special event type {request.Type}");
                }

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