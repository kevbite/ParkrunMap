using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo
{
    public class UpsertParkrun
    {
        public class Handler : AsyncRequestHandler<UpsertParkrun.Request>
        {
            private readonly IMongoCollection<Domain.Parkrun> _collection;

            public Handler(IMongoCollection<Domain.Parkrun> collection)
            {
                _collection = collection;
            }

            protected override async Task Handle(UpsertParkrun.Request request, CancellationToken cancellationToken)
            {
                var filter = Builders<Domain.Parkrun>.Filter.Eq(x => x.GeoXmlId, request.GeoXmlId);
                var update = Builders<Domain.Parkrun>.Update.Set(x => x.Name, request.Name)
                    .Set(x => x.Uri, request.Uri)
                    .Set(x => x.Region, request.Region)
                    .Set(x => x.Country, request.Country)
                    .Set(x => x.Location,
                        new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                            new GeoJson2DGeographicCoordinates(request.Longitude, request.Latitude)));

                await _collection.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true }, cancellationToken);
            }
        }

        public class Request : IRequest
        {
            public int GeoXmlId { get; set; }

            public string Name { get; set; }

            public Uri Uri { get; set; }

            public string Region { get; set; }

            public string Country { get; set; }

            public double Latitude { get; set; }

            public double Longitude { get; set; }
        }
    }
}