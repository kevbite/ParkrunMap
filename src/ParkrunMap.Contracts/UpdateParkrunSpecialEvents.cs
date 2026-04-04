namespace ParkrunMap.Contracts;

public record UpdateParkrunSpecialEvents(
    string Domain,
    string Path,
    int Year,
    bool ChristmasDayRun,
    bool NewYearsDayRun);
