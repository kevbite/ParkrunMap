using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using ParkrunMap.Domain;
using Xunit;

namespace ParkrunMap.Data.Mongo.Tests
{
    public class MongoDbFixture : IAsyncLifetime
    {
        public IMongoClient Client { get; private set; }

        public IMongoDatabase Database { get; private set; }

        public IMongoCollection<Parkrun> Collection { get; private set; }

        public Task InitializeAsync()
        {
            Client = new MongoClient();
            Database = Client.GetDatabase(Guid.NewGuid().ToString());
            Collection = Database.GetCollection<Parkrun>(Guid.NewGuid().ToString());

            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await Client.DropDatabaseAsync(Database.DatabaseNamespace.DatabaseName);
        }
    }
}