namespace ParkrunMap.Domain;

public record Website
{
    public string Domain { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public string ToUrl() => $"https://{Domain}{Path}";
}
