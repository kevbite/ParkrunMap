using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace ParkrunMap.Domain;

public record Parkrun
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; init; }
    public int GeoXmlId { get; init; }
    public string Name { get; init; } = string.Empty;
    public Website Website { get; init; } = new();
    public string Region { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; init; } = null!;
    public List<Cancellation> Cancellations { get; init; } = [];
    public Course Course { get; init; } = new();
    public Features Features { get; init; } = new();
    public SpecialEvents SpecialEvents { get; init; } = new();
    public Statistics Statistics { get; init; } = new();
}
