namespace ParkrunMap.Contracts;

public record UpdateParkrunStatistics(
    string Domain,
    string Path,
    int? TotalEvents,
    int? TotalRunners,
    int? TotalRuns,
    double? AverageRunnersPerWeek,
    int? LargestEvent,
    int? SmallestEvent,
    int? NumberOfVolunteers,
    int? TotalVolunteers,
    int? TotalKidsRuns);
