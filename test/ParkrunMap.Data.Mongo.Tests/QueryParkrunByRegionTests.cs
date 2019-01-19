using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver.GeoJsonObjectModel;
using ParkrunMap.Domain;
using Xunit;

namespace ParkrunMap.Data.Mongo.Tests
{
    public class QueryParkrunByRegionTests : IAsyncLifetime
    {
        private readonly Fixture _fixture;
        private readonly IRequestHandler<QueryParkrunByRegion.Request, QueryParkrunByRegion.Response> _handler;
        private readonly MongoDbFixture _mongoDbFixture;
        private DateTime _today;

        public QueryParkrunByRegionTests()
        {
            _today = new DateTime(2018, 12, 31);
            _mongoDbFixture = new MongoDbFixture();
            _fixture = new Fixture();
            _fixture.Customizations.Add(new UtcRandomDateTimeSequenceGenerator());
            _handler = new QueryParkrunByRegion.Handler(_mongoDbFixture.Collection, new RegionPolygonProvider(), () => _today);
        }
        
        public async Task InitializeAsync()
        {
            await _mongoDbFixture.InitializeAsync()
                .ConfigureAwait(false);
        }


        [Fact]
        public async Task ShouldReturnParkrun()
        {
            var ukParkrun = _fixture.Build<Parkrun>()
                .With(x => x.Id, ObjectId.GenerateNewId)
                .With(x => x.Cancellations, new List<Cancellation>())
                .With(x => x.Location, GetYorkUkGeoJsonPoint())
                .CreateMany()
                .ToArray();

            var germanyParkrun = _fixture.Build<Parkrun>()
                .With(x => x.Id, ObjectId.GenerateNewId)
                .With(x => x.Cancellations, new List<Cancellation>())
                .With(x => x.Location, GetCologneGermanyGeoJsonPoint())
                .CreateMany()
                .ToArray();

            await _mongoDbFixture.Collection.InsertManyAsync(ukParkrun.Concat(germanyParkrun))
                .ConfigureAwait(false);

            var command = _fixture.Build<QueryParkrunByRegion.Request>()
                .With(x => x.Region, QueryParkrunByRegion.Region.UK)
                .Create();

            var response = await _handler.Handle(command, CancellationToken.None);

            response.Parkruns.Should().BeEquivalentTo(ukParkrun);
        }

        [Fact]
        public async Task ShouldReturnOnlyNextThreeWeeksCancellations()
        {
            var yesterday = _today.AddDays(-1);
            var dateInThreeWeeks = _today.AddDays(7 * 3);
            var dateAfterThreeWeeks = dateInThreeWeeks.AddDays(1);


            var expectedCancellations = new List<Cancellation>()
            {
                new Cancellation() {Date = _today, Reason = "1"},
                new Cancellation() {Date = dateInThreeWeeks, Reason = "1"}
            };

            var allCancellations = expectedCancellations.Concat(new[]
            {
                new Cancellation() {Date = yesterday, Reason = "1"},
                new Cancellation() {Date = dateAfterThreeWeeks, Reason = "1"}
            }).ToList();

            var parkrun = _fixture.Build<Parkrun>()
                .With(x => x.Id, ObjectId.GenerateNewId)
                .With(x => x.Cancellations, allCancellations)
                .With(x => x.Location, GetYorkUkGeoJsonPoint())
                .Create();

            await _mongoDbFixture.Collection.InsertOneAsync(parkrun)
                .ConfigureAwait(false);

            var command = _fixture.Build<QueryParkrunByRegion.Request>()
                .With(x => x.Region, QueryParkrunByRegion.Region.UK)
                .Create();

            var response = await _handler.Handle(command, CancellationToken.None);

            using (new AssertionScope())
            {
                response.Parkruns.Should().BeEquivalentTo(new []{parkrun}, opt => opt.Excluding(x => x.Cancellations));

                response.Parkruns.Select(x => x.Cancellations).Should().BeEquivalentTo(new[] { expectedCancellations });
            }
        }

        private static GeoJsonPoint<GeoJson2DGeographicCoordinates> GetYorkUkGeoJsonPoint()
        {
            return new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(-1.098535, 53.937199));
        }


        private static GeoJsonPoint<GeoJson2DGeographicCoordinates> GetCologneGermanyGeoJsonPoint()
        {
            return new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(6.958710, 50.941219));
        }

        public async Task DisposeAsync()
        {
            await _mongoDbFixture.DisposeAsync()
                .ConfigureAwait(false);
        }
    }
}
