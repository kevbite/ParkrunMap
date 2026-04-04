using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using ParkrunMap.Data.Mongo.Commands;
using ParkrunMap.Data.Mongo.Queries;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo;

public static class MongoExtensions
{
    public static IServiceCollection AddMongoClient(this IServiceCollection services, IConfiguration configuration, string connectionName)
    {
        services.AddSingleton<IMongoClient>(_ =>
        {
            var connectionString = configuration.GetConnectionString(connectionName)
                ?? $"mongodb://localhost:27017/{connectionName}";
            return new MongoClient(connectionString);
        });
        return services;
    }

    public static IServiceCollection AddParkrunMongo(this IServiceCollection services)
    {
        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var database = client.GetDatabase("parkrun-map");
            var collection = database.GetCollection<Parkrun>("parkruns");
            EnsureParkrunIndexes(collection);
            return collection;
        });

        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var database = client.GetDatabase("parkrun-map");
            var collection = database.GetCollection<CourseScrapeLogDocument>("course-scrape-log");
            EnsureCourseScrapeLogIndexes(collection);
            return collection;
        });

        // Queries
        services.AddSingleton<QueryParkrunsByPolygon>();
        services.AddSingleton<QueryParkrunsByRegion>();
        services.AddSingleton<GetUnprocessedCourseTargets>();

        // Commands
        services.AddSingleton<UpsertParkrunCommand>();
        services.AddSingleton<AddCancellationCommand>();
        services.AddSingleton<UpdateFeaturesCommand>();
        services.AddSingleton<UpdateStatisticsCommand>();
        services.AddSingleton<UpdateSpecialEventsCommand>();
        services.AddSingleton<UpdateCourseDetailsCommand>();

        return services;
    }

    private static void EnsureParkrunIndexes(IMongoCollection<Parkrun> collection)
    {
        var locationIndex = new CreateIndexModel<Parkrun>(
            Builders<Parkrun>.IndexKeys.Geo2DSphere(p => p.Location),
            new CreateIndexOptions { Background = true });

        var websiteIndex = new CreateIndexModel<Parkrun>(
            Builders<Parkrun>.IndexKeys
                .Ascending(p => p.Website.Domain)
                .Ascending(p => p.Website.Path),
            new CreateIndexOptions { Unique = true, Background = true });

        collection.Indexes.CreateMany([locationIndex, websiteIndex]);
    }

    private static void EnsureCourseScrapeLogIndexes(IMongoCollection<CourseScrapeLogDocument> collection)
    {
        var websiteIndex = new CreateIndexModel<CourseScrapeLogDocument>(
            Builders<CourseScrapeLogDocument>.IndexKeys
                .Ascending(l => l.Domain)
                .Ascending(l => l.Path),
            new CreateIndexOptions { Unique = true, Background = true });

        collection.Indexes.CreateOne(websiteIndex);
    }
}