using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo
{
    public class UpdateParkrunCourseDetails
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

                var update = Builders<Parkrun>.Update.Set(x => x.Course.Description, request.Description)
                    .Set(x => x.Course.GoogleMapIds, request.GoogleMapIds);

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

            public string Description { get; set; }

            public IReadOnlyCollection<string> GoogleMapIds { get; set; }
        }
    }
}