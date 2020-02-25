using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using ParkrunMap.Domain;
using Xunit;

namespace ParkrunMap.Data.Mongo.Tests
{
    public class UpsertParkrunHandlerTests : IAsyncLifetime
    {
        private readonly Fixture _fixture;
        private readonly MongoDbFixture _mongoDbFixture;
        private readonly IRequestHandler<UpsertParkrun.Request, Unit> _handler;

        public UpsertParkrunHandlerTests()
        {
            _mongoDbFixture = new MongoDbFixture();
            _fixture = new Fixture();
            _fixture.Customizations.Add(new UtcRandomDateTimeSequenceGenerator());
            _handler = new UpsertParkrun.Handler(_mongoDbFixture.Collection);
        }

        [Fact]
        public async Task ShouldInsertDocument()
        {
            var command = _fixture.Create<UpsertParkrun.Request>();

            await _handler.Handle(command, CancellationToken.None);

            var actual = await (await _mongoDbFixture.Collection.FindAsync(x => x.GeoXmlId == command.GeoXmlId)).FirstOrDefaultAsync();

            using var scope = new AssertionScope();
            
            actual.Should().BeEquivalentTo(command, opt => opt
                .Excluding(x => x.Latitude)
                .Excluding(x => x.Longitude)
                .Excluding(x => x.WebsiteDomain)
                .Excluding(x => x.WebsitePath));

            actual.Website.Domain.Should().Be(command.WebsiteDomain);
            actual.Website.Path.Should().Be(command.WebsitePath);

            actual.Location.Type.Should().Be(GeoJsonObjectType.Point);
            actual.Location.Coordinates.Latitude.Should().Be(command.Latitude);
            actual.Location.Coordinates.Longitude.Should().Be(command.Longitude);
        }

        public Task InitializeAsync() => _mongoDbFixture.InitializeAsync();

        public Task DisposeAsync() => _mongoDbFixture.DisposeAsync();

        [Fact]
        public async Task ShouldNotUpdateCountryAndRegionWhenNull()
        {
            var parkRun = _fixture.Build<Parkrun>()
                .Without(x => x.Id)
                .Create();

            await _mongoDbFixture.Collection.InsertOneAsync(parkRun);

            var command = _fixture.Build<UpsertParkrun.Request>()
                .With(x => x.GeoXmlId, parkRun.GeoXmlId)
                .Without(x => x.Country)
                .Without(x => x.Region)
                .Create();

            await _handler.Handle(command, CancellationToken.None);

            var actual = await (await _mongoDbFixture.Collection.FindAsync(x => x.GeoXmlId == parkRun.GeoXmlId)).SingleAsync();

            using var scope = new AssertionScope();

            actual.Should().BeEquivalentTo(command, opt => opt
                .Excluding(x => x.Latitude)
                .Excluding(x => x.Longitude)
                .Excluding(x => x.WebsiteDomain)
                .Excluding(x => x.WebsitePath)
                .Excluding(x => x.Country)
                .Excluding(x => x.Region));
            actual.Country.Should().Be(parkRun.Country);
            actual.Region.Should().Be(parkRun.Region);
        }
    }
}