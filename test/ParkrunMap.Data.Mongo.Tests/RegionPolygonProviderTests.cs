using ParkrunMap.Data.Mongo;
using ParkrunMap.Domain;

namespace ParkrunMap.Data.Mongo.Tests;

public class RegionPolygonProviderTests
{
    [Theory]
    [InlineData(ParkrunRegion.UK)]
    [InlineData(ParkrunRegion.Scotland)]
    [InlineData(ParkrunRegion.Wales)]
    [InlineData(ParkrunRegion.NorthernIreland)]
    [InlineData(ParkrunRegion.NorthEastEngland)]
    [InlineData(ParkrunRegion.NorthWestEngland)]
    [InlineData(ParkrunRegion.YorkshireAndHumber)]
    [InlineData(ParkrunRegion.EastMidlands)]
    [InlineData(ParkrunRegion.WestMidlands)]
    [InlineData(ParkrunRegion.EastEngland)]
    [InlineData(ParkrunRegion.London)]
    [InlineData(ParkrunRegion.SouthEastEngland)]
    [InlineData(ParkrunRegion.SouthWestEngland)]
    public void GetPolygon_ReturnsNonEmptyPolygon_ForEachRegion(ParkrunRegion region)
    {
        var polygon = RegionPolygonProvider.GetPolygon(region);

        Assert.NotNull(polygon);
        Assert.True(polygon.Length >= 4, "Polygon should have at least 4 points (3 + closing)");
    }

    [Theory]
    [InlineData(ParkrunRegion.UK)]
    [InlineData(ParkrunRegion.Scotland)]
    [InlineData(ParkrunRegion.London)]
    public void GetPolygon_ReturnsClosedRing(ParkrunRegion region)
    {
        var polygon = RegionPolygonProvider.GetPolygon(region);

        var first = polygon[0];
        var last = polygon[^1];
        Assert.Equal(first[0], last[0]);
        Assert.Equal(first[1], last[1]);
    }

    [Theory]
    [InlineData(ParkrunRegion.UK)]
    [InlineData(ParkrunRegion.Scotland)]
    [InlineData(ParkrunRegion.Wales)]
    public void GetPolygon_EachPointHasLongitudeAndLatitude(ParkrunRegion region)
    {
        var polygon = RegionPolygonProvider.GetPolygon(region);

        foreach (var point in polygon)
        {
            Assert.Equal(2, point.Length);
            // Longitude in range [-180, 180]
            Assert.InRange(point[0], -180.0, 180.0);
            // Latitude in range [-90, 90]
            Assert.InRange(point[1], -90.0, 90.0);
        }
    }

    [Fact]
    public void GetPolygon_UkPolygon_CoversUkLatitudeRange()
    {
        var polygon = RegionPolygonProvider.GetPolygon(ParkrunRegion.UK);

        var lats = polygon.Select(p => p[1]).ToList();
        Assert.True(lats.Min() < 51.0, "UK polygon should extend south of London");
        Assert.True(lats.Max() > 57.0, "UK polygon should extend into Scotland");
    }

    [Fact]
    public void GetPolygon_ThrowsForUnknownRegion()
    {
        var unknownRegion = (ParkrunRegion)999;
        Assert.Throws<ArgumentOutOfRangeException>(() => RegionPolygonProvider.GetPolygon(unknownRegion));
    }

    [Fact]
    public void GetPolygon_AllRegionsHaveDistinctPolygons()
    {
        var regions = Enum.GetValues<ParkrunRegion>();
        var polygons = regions.Select(r => RegionPolygonProvider.GetPolygon(r)).ToList();

        // Each region should have its own polygon (not all the same)
        var uniqueFirstPoints = polygons.Select(p => (p[0][0], p[0][1])).Distinct().Count();
        Assert.True(uniqueFirstPoints > 1, "Regions should have different polygons");
    }
}
