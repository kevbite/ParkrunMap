using ParkrunMap.Domain;

namespace ParkrunMap.Api;

public record ParkrunResponse(
    string Id,
    string Name,
    string Uri,
    double Lat,
    double Lon,
    IEnumerable<CancellationResponse> Cancellations,
    CourseResponse Course,
    FeaturesResponse Features,
    SpecialEventsResponse SpecialEvents)
{
    public static ParkrunResponse From(Parkrun p) => new(
        p.Id ?? string.Empty,
        p.Name,
        p.Website.ToUrl(),
        p.Location.Coordinates.Latitude,
        p.Location.Coordinates.Longitude,
        p.Cancellations.Select(c => new CancellationResponse(c.Date, c.Reason)),
        new CourseResponse(p.Course.Description, p.Course.GoogleMapIds, p.Course.Terrain),
        new FeaturesResponse(
            p.Features.WheelchairFriendly,
            p.Features.BuggyFriendly,
            p.Features.VisuallyImpairedFriendly,
            p.Features.Toilets,
            p.Features.DogsAllowed,
            p.Features.Cafe,
            p.Features.PostRunCoffee,
            p.Features.DrinkingFountain,
            p.Features.ChangingRooms,
            p.Features.Lockers,
            p.Features.Showers,
            p.Features.BagDrop,
            p.Features.BabyChangingFacilities,
            p.Features.CarParking,
            p.Features.CycleParking,
            p.Features.CarParkingOptions,
            p.Features.CycleParkingOptions,
            p.Features.RecommendedBuggy),
        new SpecialEventsResponse(p.SpecialEvents.ChristmasDay, p.SpecialEvents.NewYearsDay));
}

public record CancellationResponse(DateTime Date, string? Reason);

public record CourseResponse(string? Description, IReadOnlyList<string> GoogleMapIds, IReadOnlyList<TerrainType> Terrain);

public record FeaturesResponse(
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

public record SpecialEventsResponse(IReadOnlyList<int> ChristmasDay, IReadOnlyList<int> NewYearsDay);
