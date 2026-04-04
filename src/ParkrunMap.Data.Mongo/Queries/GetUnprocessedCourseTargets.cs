using MongoDB.Driver;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo.Queries;

public class GetUnprocessedCourseTargets(
    IMongoCollection<Parkrun> collection,
    IMongoCollection<CourseScrapeLogDocument> scrapeLogCollection)
{
    public async Task<List<(string Domain, string Path)>> ExecuteAsync(int batchSize, CancellationToken ct = default)
    {
        var cutoff = DateTime.UtcNow.AddDays(-30);
        var recentFilter = Builders<CourseScrapeLogDocument>.Filter.Gte(l => l.LastScrapedAt, cutoff);
        var recentDocs = await scrapeLogCollection.Find(recentFilter)
            .Project(l => new { l.Domain, l.Path })
            .ToListAsync(ct);
        var recentlyScraped = recentDocs.Select(x => (x.Domain, x.Path)).ToHashSet();

        var projection = Builders<Parkrun>.Projection
            .Include(p => p.Website)
            .Exclude(p => p.Id);
        var allParkruns = await collection.Find(FilterDefinition<Parkrun>.Empty)
            .Project<Parkrun>(projection)
            .ToListAsync(ct);

        return allParkruns
            .Select(d => (d.Website.Domain, d.Website.Path))
            .Where(t => !recentlyScraped.Contains(t))
            .Take(batchSize)
            .ToList();
    }
}
