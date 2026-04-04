namespace ParkrunMap.Domain;

public record Cancellation
{
    public DateTime Date { get; init; }
    public string? Reason { get; init; }
}
