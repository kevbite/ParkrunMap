using MongoDB.Driver.GeoJsonObjectModel;
using ParkrunMap.Api;
using ParkrunMap.Domain;

namespace ParkrunMap.Api.Tests;

public class ParkrunResponseTests
{
    [Fact]
    public void From_MapsBasicFields_Correctly()
    {
        var parkrun = BuildParkrun("p1", "Bushy parkrun", "www.parkrun.org.uk", "/bushy/", 51.4099, -0.3348);

        var response = ParkrunResponse.From(parkrun);

        Assert.Equal("p1", response.Id);
        Assert.Equal("Bushy parkrun", response.Name);
        Assert.Equal("https://www.parkrun.org.uk/bushy/", response.Uri);
        Assert.Equal(51.4099, response.Lat);
        Assert.Equal(-0.3348, response.Lon);
    }

    [Fact]
    public void From_MapsEmptyCancellations_Correctly()
    {
        var parkrun = BuildParkrun();

        var response = ParkrunResponse.From(parkrun);

        Assert.Empty(response.Cancellations);
    }

    [Fact]
    public void From_MapsCancellations_WithDateAndReason()
    {
        var parkrun = BuildParkrun() with
        {
            Cancellations = [new Cancellation { Date = new DateTime(2024, 12, 25), Reason = "Christmas Day" }]
        };

        var response = ParkrunResponse.From(parkrun);

        var cancellation = Assert.Single(response.Cancellations);
        Assert.Equal(new DateTime(2024, 12, 25), cancellation.Date);
        Assert.Equal("Christmas Day", cancellation.Reason);
    }

    [Fact]
    public void From_MapsCourseDetails_Correctly()
    {
        var parkrun = BuildParkrun() with
        {
            Course = new Course
            {
                Description = "Flat course through the park",
                GoogleMapIds = ["map1", "map2"],
                Terrain = [TerrainType.Grass, TerrainType.Road]
            }
        };

        var response = ParkrunResponse.From(parkrun);

        Assert.Equal("Flat course through the park", response.Course.Description);
        Assert.Equal(["map1", "map2"], response.Course.GoogleMapIds);
        Assert.Equal([TerrainType.Grass, TerrainType.Road], response.Course.Terrain);
    }

    [Fact]
    public void From_MapsFeatures_Correctly()
    {
        var parkrun = BuildParkrun() with
        {
            Features = new Features
            {
                WheelchairFriendly = true,
                BuggyFriendly = false,
                Cafe = true,
                DogsAllowed = true,
                CarParking = true,
                CarParkingOptions = [CarParkingOption.FreeCarPark],
                RecommendedBuggy = [BuggyType.Running]
            }
        };

        var response = ParkrunResponse.From(parkrun);

        Assert.True(response.Features.WheelchairFriendly);
        Assert.False(response.Features.BuggyFriendly);
        Assert.True(response.Features.Cafe);
        Assert.True(response.Features.DogsAllowed);
        Assert.True(response.Features.CarParking);
        Assert.Equal([CarParkingOption.FreeCarPark], response.Features.CarParkingOptions);
        Assert.Equal([BuggyType.Running], response.Features.RecommendedBuggy);
    }

    [Fact]
    public void From_MapsSpecialEvents_Correctly()
    {
        var parkrun = BuildParkrun() with
        {
            SpecialEvents = new SpecialEvents
            {
                ChristmasDay = [2022, 2023],
                NewYearsDay = [2023]
            }
        };

        var response = ParkrunResponse.From(parkrun);

        Assert.Equal([2022, 2023], response.SpecialEvents.ChristmasDay);
        Assert.Equal([2023], response.SpecialEvents.NewYearsDay);
    }

    [Fact]
    public void From_GeneratesCorrectUri_ForUkParkrun()
    {
        var parkrun = BuildParkrun(domain: "www.parkrun.org.uk", path: "/testvalley/");

        var response = ParkrunResponse.From(parkrun);

        Assert.Equal("https://www.parkrun.org.uk/testvalley/", response.Uri);
    }

    private static Parkrun BuildParkrun(
        string id = "test-id",
        string name = "Test parkrun",
        string domain = "www.parkrun.org.uk",
        string path = "/test/",
        double lat = 51.0,
        double lon = -0.1) => new()
    {
        Id = id,
        Name = name,
        GeoXmlId = 1,
        Website = new Website { Domain = domain, Path = path },
        Location = GeoJson.Point(GeoJson.Geographic(lon, lat)),
        Region = nameof(ParkrunRegion.London),
        Country = "UK"
    };
}
