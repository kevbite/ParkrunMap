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
        public class Handler : AsyncRequestHandler<Request>
        {
            private readonly IMongoCollection<Parkrun> _collection;

            public Handler(IMongoCollection<Parkrun> collection)
            {
                _collection = collection;
            }

            protected override async Task Handle(Request request, CancellationToken cancellationToken)
            {
                var filter = Builders<Parkrun>.Filter.Eq(x => x.GeoXmlId, request.GeoXmlId);
                var update = Builders<Parkrun>.Update
                    .Set(x => x.Name, request.Name)
                    .Set(x => x.Website.Domain, request.WebsiteDomain)
                    .Set(x => x.Website.Path, request.WebsitePath)
                    .Set(x => x.Location,
                        new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                            new GeoJson2DGeographicCoordinates(request.Longitude, request.Latitude)));

                if (!string.IsNullOrEmpty(request.Country))
                    update = update.Set(x => x.Country, request.Country);
                if (!string.IsNullOrEmpty(request.Region))
                    update = update.Set(x => x.Region, request.Region);

                await _collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true }, cancellationToken);
            }
        }

        public class Request : IRequest
        {
            public int GeoXmlId { get; set; }

            public string Name { get; set; }

            public string WebsiteDomain { get; set; }

            public string WebsitePath { get; set; }

            public string Region { get; set; }

            public string Country { get; set; }

            public double Latitude { get; set; }

            public double Longitude { get; set; }
        }
    }
}