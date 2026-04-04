namespace ParkrunMap.Domain;

public record Statistics
{
    public int? TotalEvents { get; init; }
    public int? TotalRunners { get; init; }
    public int? TotalRuns { get; init; }
    public double? AverageRunnersPerWeek { get; init; }
    public int? LargestEvent { get; init; }
    public int? SmallestEvent { get; init; }
    public int? NumberOfVolunteers { get; init; }
    public int? TotalVolunteers { get; init; }
    public int? TotalKidsRuns { get; init; }
}
