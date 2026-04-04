namespace ParkrunMap.Contracts;

public record AddParkrunCancellation(
    string Domain,
    string Path,
    DateTime Date,
    string? Reason);
