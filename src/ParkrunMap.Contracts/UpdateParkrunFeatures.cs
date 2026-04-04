using ParkrunMap.Domain;

namespace ParkrunMap.Contracts;

public record UpdateParkrunFeatures(
    string Domain,
    string Path,
    bool? WheelchairFriendly,
    bool? BuggyFriendly,
    bool? VisuallyImpairedFriendly,
    bool? Toilets,
    bool? DogsAllowed,
    bool? Cafe,
    bool? PostRunCoffee,
    bool? DrinkingFountain,
    bool? ChangingRooms,
    bool? Lockers,
    bool? Showers,
    bool? BagDrop,
    bool? BabyChangingFacilities,
    bool? CarParking,
    bool? CycleParking,
    IReadOnlyList<CarParkingOption> CarParkingOptions,
    IReadOnlyList<CycleParkingOption> CycleParkingOptions,
    IReadOnlyList<BuggyType> RecommendedBuggy);
