using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MediatR;
using MongoDB.Bson;
using ParkrunMap.Domain;
using Xunit;

namespace ParkrunMap.Data.Mongo.Tests
{
    public class QueryFirstParkrunForWebsiteTests : IAsyncLifetime
    {
        private readonly Fixture _fixture;
        private readonly IRequestHandler<QueryFirstParkrunForWebsite.Request, QueryFirstParkrunForWebsite.Response> _handler;
        private readonly MongoDbFixture _mongoDbFixture;

        public QueryFirstParkrunForWebsiteTests()
        {
            _mongoDbFixture = new MongoDbFixture();
            _fixture = new Fixture();
            _fixture.Customizations.Add(new UtcRandomDateTimeSequenceGenerator());
            _handler = new QueryFirstParkrunForWebsite.Handler(_mongoDbFixture.Collection);
        }

        public async Task InitializeAsync()
        {
            await _mongoDbFixture.InitializeAsync()
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task ShouldReturnParkrun()
        {
            var parkrun = _fixture.Build<Parkrun>()
                .With(x => x.Id, ObjectId.GenerateNewId)
                .With(x => x.Cancellations, new List<Cancellation>())
                .CreateMany()
                .ToArray();
            
            await _mongoDbFixture.Collection.InsertManyAsync(parkrun)
                .ConfigureAwait(false);

            var command = new QueryFirstParkrunForWebsite.Request();

            var response = await _handler.Handle(command, CancellationToken.None);

            var matchedResponse = parkrun.Single(x => x.Id == response.Parkrun.Id);

            response.Parkrun.Should().BeEquivalentTo(new
            {
                matchedResponse.Id,
                WebsiteDomain = matchedResponse.Website.Domain,
                WebsitePath = matchedResponse.Website.Path
            });
        }

        [Fact]
        public async Task ShouldNotReturnParkrunWithIdInExcept()
        {
            var parkrun1 = _fixture.Build<Parkrun>()
                .With(x => x.Id, ObjectId.GenerateNewId)
                .With(x => x.Cancellations, new List<Cancellation>())
                .Create();

            var parkrun2 = _fixture.Build<Parkrun>()
                .With(x => x.Id, ObjectId.GenerateNewId)
                .With(x => x.Cancellations, new List<Cancellation>())
                .Create();

            await _mongoDbFixture.Collection.InsertManyAsync(new[] {parkrun1, parkrun2})
                .ConfigureAwait(false);

            var command = new QueryFirstParkrunForWebsite.Request() {ExceptIds = new[] {parkrun1.Id}};

            var response = await _handler.Handle(command, CancellationToken.None);
            
            response.Parkrun.Should().BeEquivalentTo(new
            {
                parkrun2.Id,
                WebsiteDomain = parkrun2.Website.Domain,
                WebsitePath = parkrun2.Website.Path
            });
        }

        [Fact]
        public async Task ShouldNotReturnNullWhenNoParkruns()
        {
            var parkrun1 = _fixture.Build<Parkrun>()
                .With(x => x.Id, ObjectId.GenerateNewId)
                .With(x => x.Cancellations, new List<Cancellation>())
                .Create();

            var parkrun2 = _fixture.Build<Parkrun>()
                .With(x => x.Id, ObjectId.GenerateNewId)
                .With(x => x.Cancellations, new List<Cancellation>())
                .Create();

            await _mongoDbFixture.Collection.InsertManyAsync(new[] { parkrun1, parkrun2 })
                .ConfigureAwait(false);

            var command = new QueryFirstParkrunForWebsite.Request() { ExceptIds = new[] { parkrun1.Id, parkrun2.Id } };

            var response = await _handler.Handle(command, CancellationToken.None);

            response.Parkrun.Should().BeNull();
        }

        public async Task DisposeAsync()
        {
            await _mongoDbFixture.DisposeAsync()
                .ConfigureAwait(false);
        }
    }
}
