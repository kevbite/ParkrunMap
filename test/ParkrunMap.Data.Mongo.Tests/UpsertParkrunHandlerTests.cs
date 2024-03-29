using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using ParkrunMap.Domain;
using Xunit;

namespace ParkrunMap.Data.Mongo.Tests
{
    public class UpsertParkrunHandlerTests
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

            var actual = await (await _mongoDbFixture.Collection.FindAsync(x =>
                    x.Website.Domain == command.WebsiteDomain && x.Website.Path == command.WebsitePath))
                .FirstOrDefaultAsync();

            using var scope = new AssertionScope();

            actual.Should().BeEquivalentTo(command, opt => opt
                .Excluding(x => x.Latitude)
                .Excluding(x => x.Longitude)
                .Excluding(x => x.WebsiteDomain)
                .Excluding(x => x.WebsitePath));

            actual.Location.Type.Should().Be(GeoJsonObjectType.Point);
            actual.Location.Coordinates.Latitude.Should().Be(command.Latitude);
            actual.Location.Coordinates.Longitude.Should().Be(command.Longitude);
        }


        [Fact]
        public async Task ShouldUpdateDocument()
        {
            var client = new MongoClient();
            var database = client.GetDatabase(Guid.NewGuid().ToString());
            var collection = database.GetCollection<Parkrun>(Guid.NewGuid().ToString());

            var initial = _fixture.Build<Parkrun>()
                .With(x => x.Id, ObjectId.Empty)
                .With(x => x.Cancellations, new Cancellation[0])
                .Create();
            await collection.InsertOneAsync(initial);

            IRequestHandler<UpsertParkrun.Request, Unit> handler = new UpsertParkrun.Handler(collection);

            var command = _fixture.Build<UpsertParkrun.Request>()
                    .With(x => x.WebsitePath, initial.Website.Path)
                    .With(x => x.WebsiteDomain, initial.Website.Domain)
                    .Create()
                ;

            await handler.Handle(command, CancellationToken.None);

            var actual = await (await collection.FindAsync(x =>
                    x.Website.Domain == command.WebsiteDomain && x.Website.Path == command.WebsitePath))
                .FirstOrDefaultAsync();

            using (new AssertionScope())
            {
                actual.Should().BeEquivalentTo(initial, opt => opt.Excluding(x => x.Location).Excluding(x => x.Name));

                actual.Name.Should().Be(command.Name);
                actual.Location.Type.Should().Be(GeoJsonObjectType.Point);
                actual.Location.Coordinates.Latitude.Should().Be(command.Latitude);
                actual.Location.Coordinates.Longitude.Should().Be(command.Longitude);
            }
        }
    }
}