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
    public class UpsertParkrunHandlerTests
    {
        private Fixture _fixture;

        public UpsertParkrunHandlerTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public async Task ShouldInsertDocument()
        {
            var client = new MongoClient();
            var database = client.GetDatabase(Guid.NewGuid().ToString());
            var collection = database.GetCollection<Parkrun>(Guid.NewGuid().ToString());

            IRequestHandler<UpsertParkrun.Command, Unit> handler = new UpsertParkrun.Handler(collection);

            var command = _fixture.Create<UpsertParkrun.Command>();

            await handler.Handle(command, CancellationToken.None);

            var actual = await (await collection.FindAsync(x => x.GeoXmlId == command.GeoXmlId)).FirstOrDefaultAsync();

            using (new AssertionScope())
            {
                actual.Should().BeEquivalentTo(command,
                    opt => opt.Excluding(x => x.Latitude).Excluding(x => x.Longitude));

                actual.Location.Type.Should().Be(GeoJsonObjectType.Point);
                actual.Location.Coordinates.Latitude.Should().Be(command.Latitude);
                actual.Location.Coordinates.Longitude.Should().Be(command.Longitude);
            }
        }
    }
}
