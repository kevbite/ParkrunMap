using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo
{
    public class UpdateParkrunFeatures
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
                    .Set(x => x.Course.Terrain, request.Terrain)
                    .Set(x => x.Features.WheelchairFriendly, request.WheelchairFriendly)
                    .Set(x => x.Features.BuggyFriendly, request.BuggyFriendly)
                    .Set(x => x.Features.VisuallyImpairedFriendly, request.VisuallyImpairedFriendly)
                    .Set(x => x.Features.Toilets, request.Toilets)
                    .Set(x => x.Features.DogsAllowed, request.DogsAllowed)
                    .Set(x => x.Features.Cafe, request.Cafe)
                    .Set(x => x.Features.PostRunCoffee, request.PostRunCoffee)
                    .Set(x => x.Features.DrinkingFountain, request.DrinkingFountain)
                    .Set(x => x.Features.ChangingRooms, request.ChangingRooms)
                    .Set(x => x.Features.Lockers, request.Lockers)
                    .Set(x => x.Features.Showers, request.Showers)
                    .Set(x => x.Features.BagDrop, request.BagDrop)
                    .Set(x => x.Features.BabyChangingFacilities, request.BabyChangingFacilities)
                    .Set(x => x.Features.CarParking, request.CarParking)
                    .Set(x => x.Features.CycleParking, request.CycleParking)
                    .Set(x => x.Features.CarParkingOptions, request.CarParkingOptions)
                    .Set(x => x.Features.CycleParkingOptions, request.CycleParkingOptions)
                    .Set(x => x.Features.RecommendedBuggy, request.RecommendedBuggy);
      
                var updateResult = await _collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

                if (updateResult.MatchedCount == 0)
                {
                    throw new Exception($"Could not find parkrun with website {request.WebsiteDomain}{request.WebsitePath}");
                }
            }
        }

        public class Request : IRequest
        {
            public Request()
            {
                Terrain = new TerrainType[0];
                CarParkingOptions = new CarParkingOption[0];
                CycleParkingOptions = new CycleParkingOption[0];
                RecommendedBuggy = new BuggyType[0];
            }

            public string WebsiteDomain { get; set; }

            public string WebsitePath { get; set; }

            public IReadOnlyCollection<TerrainType> Terrain { get; set; }

            public bool? WheelchairFriendly { get; set; }

            public bool? BuggyFriendly { get; set; }

            public bool? VisuallyImpairedFriendly { get; set; }

            public bool? Toilets { get; set; }

            public bool? DogsAllowed { get; set; }

            public bool? Cafe { get; set; }

            public bool? PostRunCoffee { get; set; }

            public bool? DrinkingFountain { get; set; }

            public bool? ChangingRooms { get; set; }

            public bool? Lockers { get; set; }

            public bool? Showers { get; set; }

            public bool? BagDrop { get; set; }

            public bool? BabyChangingFacilities { get; set; }

            public bool? CarParking { get; set; }

            public bool? CycleParking { get; set; }

            public IReadOnlyCollection<CarParkingOption> CarParkingOptions { get; set; }

            public IReadOnlyCollection<CycleParkingOption> CycleParkingOptions { get; set; }

            public IReadOnlyCollection<BuggyType> RecommendedBuggy { get; set; }
        }
    }
}