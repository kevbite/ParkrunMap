using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using ParkrunMap.Domain;
using Xunit;

namespace ParkrunMap.Data.Mongo.Tests
{

    public class UpdateParkrunSpecialEventsTests : IClassFixture<MongoDbFixture>
    {
        private readonly Fixture _fixture;
        private readonly IRequestHandler<UpdateParkrunSpecialEvents.Request, Unit> _handler;
        private readonly MongoDbFixture _mongoDbFixture;

        public UpdateParkrunSpecialEventsTests(MongoDbFixture mongoDbFixture)
        {
            _mongoDbFixture = mongoDbFixture;
            _fixture = new Fixture();
            _fixture.Customizations.Add(new UtcRandomDateTimeSequenceGenerator());
            _handler = new UpdateParkrunSpecialEvents.Handler(mongoDbFixture.Collection);
        }

        [Fact]
        public async Task ShouldAddChristmasDaySpecialEvent()
        {
            var parkrun = _fixture.Build<Parkrun>()
                .With(x => x.Id, ObjectId.GenerateNewId())
                .With(x => x.SpecialEvents, new SpecialEvents())
                .Create();

            await _mongoDbFixture.Collection.InsertOneAsync(parkrun)
                .ConfigureAwait(false);

            var command = _fixture.Build<UpdateParkrunSpecialEvents.Request>()
                .With(x => x.WebsiteDomain, parkrun.Website.Domain)
                .With(x => x.WebsitePath, parkrun.Website.Path)
                .With(x => x.IsRunning, true)
                .With(x => x.Type, SpecialEventType.ChristmasDay)
                .Create();
            
            await _handler.Handle(command, CancellationToken.None);

            var actual = await (await _mongoDbFixture.Collection.FindAsync(x => x.Id == parkrun.Id)).FirstOrDefaultAsync();

            actual.SpecialEvents.ChristmasDay.Should()
                .HaveCount(1)
                .And.Contain(command.Year);
        }


        [Fact]
        public async Task ShouldThrowExceptionWhenNoParkrunUriMatch()
        {
            var command = _fixture.Build<UpdateParkrunSpecialEvents.Request>()
                .Create();

            await _handler.Awaiting(x => x.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<Exception>();
        }
    }
}
