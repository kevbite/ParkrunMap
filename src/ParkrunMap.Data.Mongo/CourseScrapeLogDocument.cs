using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ParkrunMap.Data.Mongo;

public record CourseScrapeLogDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; init; }
    public string Domain { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public DateTime LastScrapedAt { get; init; }
}
