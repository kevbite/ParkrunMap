using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.DataAnnotations;
using FluentAssertions;
using FluentAssertions.Execution;
using MediatR;
using MongoDB.Bson;
using MongoDB.Driver;
using ParkrunMap.Domain;
using Xunit;

namespace ParkrunMap.Data.Mongo.Tests
{

    public class AddParkrunCancellationTests : IClassFixture<MongoDbFixture>
    {
        private readonly Fixture _fixture;
        private readonly IRequestHandler<AddParkrunCancellation.Request, Unit> _handler;
        private readonly MongoDbFixture _mongoDbFixture;

        public AddParkrunCancellationTests(MongoDbFixture mongoDbFixture)
        {
            _mongoDbFixture = mongoDbFixture;
            _fixture = new Fixture();
            _fixture.Customizations.Add(new UtcRandomDateTimeSequenceGenerator());
            _handler = new AddParkrunCancellation.Handler(mongoDbFixture.Collection);
        }

        [Fact]
        public async Task ShouldAddCancellation()
        {
            var cancellations = _fixture.Build<Cancellation>()
                .With(x => x.Date, () => _fixture.Create<DateTime>().Date)
                .CreateMany().ToArray();
            var parkrun = _fixture.Build<Parkrun>()
                .With(x => x.Id, ObjectId.GenerateNewId())
                .With(x => x.Cancellations, cancellations)
                .Create();

            await _mongoDbFixture.Collection.InsertOneAsync(parkrun)
                .ConfigureAwait(false);

            var command = _fixture.Build<AddParkrunCancellation.Request>()
                .With(x => x.Date, _fixture.Create<DateTime>().Date)
                .With(x => x.Uri, parkrun.Uri).Create();
            
            await _handler.Handle(command, CancellationToken.None);

            var actual = await (await _mongoDbFixture.Collection.FindAsync(x => x.Id == parkrun.Id)).FirstOrDefaultAsync();

            using (new AssertionScope())
            {
                var expectations = new[]
                {
                    new Cancellation() {Date = command.Date, Reason = command.Reason}
                }.Concat(parkrun.Cancellations.Select(x => new Cancellation(){Date = x.Date, Reason = x.Reason})).ToArray();

                actual.Cancellations.Should().BeEquivalentTo(expectations);
            }
        }


        [Fact]
        public async Task ShouldThrowExceptionWhenNoParkrunUriMatch()
        {
            var command = _fixture.Build<AddParkrunCancellation.Request>()
                .With(x => x.Date, _fixture.Create<DateTime>().Date)
                .Create();

            await _handler.Awaiting(x => x.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task ShouldUpdateCancellationReason()
        {
            var cancellations = _fixture.Build<Cancellation>()
                .With(x => x.Date, () => _fixture.Create<DateTime>().Date)
                .CreateMany().ToArray();
            var parkrun = _fixture.Build<Parkrun>()
                .With(x => x.Id, ObjectId.GenerateNewId())
                .With(x => x.Cancellations, cancellations)
                .Create();

            await _mongoDbFixture.Collection.InsertOneAsync(parkrun)
                .ConfigureAwait(false);

            var command = _fixture.Build<AddParkrunCancellation.Request>()
                .With(x => x.Date, parkrun.Cancellations[1].Date)
                .With(x => x.Uri, parkrun.Uri).Create();

            await _handler.Handle(command, CancellationToken.None);

            var actual = await (await _mongoDbFixture.Collection.FindAsync(x => x.Id == parkrun.Id)).FirstOrDefaultAsync();

            using (new AssertionScope())
            {
                var expectations = new[]
                {
                    new Cancellation() {Date = command.Date, Reason = command.Reason}
                }.Concat(new []
                {
                    parkrun.Cancellations[0],
                    parkrun.Cancellations[2],
                }).ToArray();

                actual.Cancellations.Should().BeEquivalentTo(expectations);
            }
        }
    }
}
