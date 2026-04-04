namespace ParkrunMap.Domain;

public record Course
{
    public string? Description { get; init; }
    public List<string> GoogleMapIds { get; init; } = [];
    public List<TerrainType> Terrain { get; init; } = [];
}
