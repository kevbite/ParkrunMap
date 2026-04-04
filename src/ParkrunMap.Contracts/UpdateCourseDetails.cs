namespace ParkrunMap.Contracts;

public record UpdateCourseDetails(
    string Domain,
    string Path,
    string? Description,
    IReadOnlyList<string> GoogleMapIds,
    IReadOnlyList<string> Terrain);
