using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using ParkrunMap.Domain;
using Xunit;

namespace ParkrunMap.Data.Mongo.Tests
{
    public class UpdateParkrunFeaturesTests : IAsyncLifetime
    {
        private readonly Fixture _fixture;
        private readonly IRequestHandler<UpdateParkrunFeatures.Request, Unit> _handler;
        private readonly MongoDbFixture _mongoDbFixture;

        public UpdateParkrunFeaturesTests()
        {
            _mongoDbFixture = new MongoDbFixture();
            _fixture = new Fixture();
            _fixture.Customizations.Add(new UtcRandomDateTimeSequenceGenerator());
            _handler = new UpdateParkrunFeatures.Handler(_mongoDbFixture.Collection);
        }

        [Fact]
        public async Task ShouldUpdateFeatures()
        {
            var parkrun = _fixture.Build<Parkrun>()
                .With(x => x.Cancellations, new Cancellation[0])
                .With(x => x.Id, ObjectId.GenerateNewId())
                .With(x => x.Features, new Features())
                .Create();

            await _mongoDbFixture.Collection.InsertOneAsync(parkrun)
                .ConfigureAwait(false);

            var command = _fixture.Build<UpdateParkrunFeatures.Request>()
                .With(x => x.WebsiteDomain, parkrun.Website.Domain)
                .With(x => x.WebsitePath, parkrun.Website.Path)
                .Create();

            await _handler.Handle(command, CancellationToken.None);

            var actual = await (await _mongoDbFixture.Collection.FindAsync(x => x.Id == parkrun.Id)).FirstOrDefaultAsync();

            using (new AssertionScope())
            {
                actual.Should().BeEquivalentTo(parkrun, opt => opt.Excluding(x => x.Features).Excluding(x => x.Course.Terrain));

                actual.Course.Terrain.Should().BeEquivalentTo(command.Terrain);
                actual.Features.Should().BeEquivalentTo(command, opt => opt.Excluding(x => x.WebsiteDomain).Excluding(x => x.WebsitePath).Excluding(x => x.Terrain));
            }
        }


        [Fact]
        public async Task ShouldUpdateRemoveFeatures()
        {
            var parkrun = _fixture.Build<Parkrun>()
                .With(x => x.Cancellations, new Cancellation[0])
                .With(x => x.Id, ObjectId.GenerateNewId())
                .Create();

            await _mongoDbFixture.Collection.InsertOneAsync(parkrun)
                .ConfigureAwait(false);

            var command = new UpdateParkrunFeatures.Request()
            {
                WebsiteDomain = parkrun.Website.Domain,
                WebsitePath = parkrun.Website.Path
            };

            await _handler.Handle(command, CancellationToken.None);

            var actual = await (await _mongoDbFixture.Collection.FindAsync(x => x.Id == parkrun.Id)).FirstOrDefaultAsync();

            using (new AssertionScope())
            {
                actual.Should().BeEquivalentTo(parkrun, opt => opt.Excluding(x => x.Features).Excluding(x => x.Course.Terrain));

                actual.Course.Terrain.Should().BeEquivalentTo(command.Terrain);
                actual.Features.Should().BeEquivalentTo(new Features());
            }
        }

        [Fact]
        public async Task ShouldThrowExceptionWhenNoParkrunUriMatch()
        {
            var command = _fixture.Build<UpdateParkrunFeatures.Request>()
                .Create();

            await _handler.Awaiting(x => x.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<Exception>();
        }

        public async Task InitializeAsync()
        {
            await _mongoDbFixture.InitializeAsync()
                .ConfigureAwait(false);
        }

        public async Task DisposeAsync()
        {
            await _mongoDbFixture.DisposeAsync()
                .ConfigureAwait(false);
        }
    }
}
