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
    public class QueryAllParkrunForWebsiteTests : IAsyncLifetime
    {
        private readonly Fixture _fixture;
        private readonly IRequestHandler<QueryAllParkrunForWebsite.Request, QueryAllParkrunForWebsite.Response> _handler;
        private readonly MongoDbFixture _mongoDbFixture;

        public QueryAllParkrunForWebsiteTests()
        {
            _mongoDbFixture = new MongoDbFixture();
            _fixture = new Fixture();
            _fixture.Customizations.Add(new UtcRandomDateTimeSequenceGenerator());
            _handler = new QueryAllParkrunForWebsite.Handler(_mongoDbFixture.Collection);
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

            var command = new QueryAllParkrunForWebsite.Request();

            var response = await _handler.Handle(command, CancellationToken.None);

            response.Parkruns.Should().BeEquivalentTo(parkrun.Select(x => new
            {
                x.Id,
                WebsiteDomain = x.Website.Domain,
                WebsitePath = x.Website.Path
            }));
        }

        public async Task DisposeAsync()
        {
            await _mongoDbFixture.DisposeAsync()
                .ConfigureAwait(false);
        }
    }
}
