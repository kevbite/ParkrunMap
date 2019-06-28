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
    public class UpdateParkrunStatisticsTests : IAsyncLifetime
    {
        private readonly Fixture _fixture;
        private readonly IRequestHandler<UpdateParkrunStatistics.Request, Unit> _handler;
        private readonly MongoDbFixture _mongoDbFixture;

        public UpdateParkrunStatisticsTests()
        {
            _mongoDbFixture = new MongoDbFixture();
            _fixture = new Fixture();
            _fixture.Customizations.Add(new UtcRandomDateTimeSequenceGenerator());
            _handler = new UpdateParkrunStatistics.Handler(_mongoDbFixture.Collection);
        }

        [Fact]
        public async Task ShouldUpdateStatistics()
        {
            var parkrun = _fixture.Build<Parkrun>()
                .With(x => x.Cancellations, new Cancellation[0])
                .With(x => x.Statistics, new Statistics())
                .With(x => x.Id, ObjectId.GenerateNewId())
                .Create();

            await _mongoDbFixture.Collection.InsertOneAsync(parkrun)
                .ConfigureAwait(false);

            var command = _fixture.Build<UpdateParkrunStatistics.Request>()
                .With(x => x.WebsiteDomain, parkrun.Website.Domain)
                .With(x => x.WebsitePath, parkrun.Website.Path)
                .Create();

            await _handler.Handle(command, CancellationToken.None);

            var actual = await (await _mongoDbFixture.Collection.FindAsync(x => x.Id == parkrun.Id)).FirstOrDefaultAsync();

            using (new AssertionScope())
            {
                actual.Should().BeEquivalentTo(parkrun, opt => opt.Excluding(x => x.Statistics));

                actual.Statistics.Should().BeEquivalentTo(command, opt => opt.Excluding(x => x.WebsiteDomain).Excluding(x => x.WebsitePath));
            }
        }


        [Fact]
        public async Task ShouldThrowExceptionWhenNoParkrunUriMatch()
        {
            var command = _fixture.Build<UpdateParkrunStatistics.Request>()
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