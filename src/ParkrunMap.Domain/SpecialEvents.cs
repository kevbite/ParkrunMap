namespace ParkrunMap.Domain;

public record SpecialEvents
{
    public List<int> ChristmasDay { get; init; } = [];
    public List<int> NewYearsDay { get; init; } = [];
}
