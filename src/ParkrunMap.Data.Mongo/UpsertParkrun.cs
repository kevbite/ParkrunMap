﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo
{
    public class UpsertParkrun
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
                var filter = Builders<Parkrun>.Filter.Eq(x => x.Website.Domain, request.WebsiteDomain)
                             & Builders<Parkrun>.Filter.Eq(x => x.Website.Path, request.WebsitePath);

                var update = Builders<Parkrun>.Update.Set(x => x.Name, request.Name)
                   .Set(x => x.Location, new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                            new GeoJson2DGeographicCoordinates(request.Longitude, request.Latitude)));

                await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true }, cancellationToken);
            }
        }

        public class Request : IRequest
        {
            public string Name { get; set; }

            public string WebsiteDomain { get; set; }

            public string WebsitePath { get; set; }
            
            public double Latitude { get; set; }

            public double Longitude { get; set; }
        }
    }
}