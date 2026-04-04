namespace ParkrunMap.Domain;

public record Features
{
    public bool? WheelchairFriendly { get; init; }
    public bool? BuggyFriendly { get; init; }
    public bool? VisuallyImpairedFriendly { get; init; }
    public bool? Toilets { get; init; }
    public bool? DogsAllowed { get; init; }
    public bool? Cafe { get; init; }
    public bool? PostRunCoffee { get; init; }
    public bool? DrinkingFountain { get; init; }
    public bool? ChangingRooms { get; init; }
    public bool? Lockers { get; init; }
    public bool? Showers { get; init; }
    public bool? BagDrop { get; init; }
    public bool? BabyChangingFacilities { get; init; }
    public bool? CarParking { get; init; }
    public bool? CycleParking { get; init; }
    public List<CarParkingOption> CarParkingOptions { get; init; } = [];
    public List<CycleParkingOption> CycleParkingOptions { get; init; } = [];
    public List<BuggyType> RecommendedBuggy { get; init; } = [];
}
