using MongoDB.Driver;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo.Commands;

public class UpdateCourseDetailsCommand(
    IMongoCollection<Parkrun> collection,
    IMongoCollection<CourseScrapeLogDocument> scrapeLogCollection)
{
    public async Task ExecuteAsync(string domain, string path, Course course, CancellationToken ct = default)
    {
        var filter = WebsiteFilter(domain, path);
        var update = Builders<Parkrun>.Update
            .Set(p => p.Course.Description, course.Description)
            .Set(p => p.Course.GoogleMapIds, course.GoogleMapIds)
            .Set(p => p.Course.Terrain, course.Terrain);

        await collection.UpdateOneAsync(filter, update, cancellationToken: ct);

        // Only record in the scrape log when a description was found so that
        // parkruns where scraping returned nothing can be retried on the next cycle.
        if (!string.IsNullOrWhiteSpace(course.Description))
        {
            var logFilter = Builders<CourseScrapeLogDocument>.Filter.And(
                Builders<CourseScrapeLogDocument>.Filter.Eq(l => l.Domain, domain),
                Builders<CourseScrapeLogDocument>.Filter.Eq(l => l.Path, path));
            var logUpdate = Builders<CourseScrapeLogDocument>.Update
                .Set(l => l.Domain, domain)
                .Set(l => l.Path, path)
                .Set(l => l.LastScrapedAt, DateTime.UtcNow);
            await scrapeLogCollection.UpdateOneAsync(logFilter, logUpdate, new UpdateOptions { IsUpsert = true }, ct);
        }
    }

    private static FilterDefinition<Parkrun> WebsiteFilter(string domain, string path) =>
        Builders<Parkrun>.Filter.And(
            Builders<Parkrun>.Filter.Eq(p => p.Website.Domain, domain),
            Builders<Parkrun>.Filter.Eq(p => p.Website.Path, path));
}
