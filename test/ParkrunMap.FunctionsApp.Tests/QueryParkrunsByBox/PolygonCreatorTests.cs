using FluentAssertions;
using ParkrunMap.FunctionsApp.QueryParkrunsByBox;
using Xunit;

namespace ParkrunMap.FunctionsApp.Tests.QueryParkrunsByBox
{
    public class PolygonCreatorTests
    {
        [Fact]
        public void ShouldCreatePolygon()
        {
            var polygonCreator = new PolygonCreator();
            var bottomLeft = new {Lat = 49.156411, Lon = -2.256576};
            var upperRight = new { Lat = 49.262943, Lon = -2.012237 };
            var polygon = polygonCreator.FromBox(bottomLeft.Lat, bottomLeft.Lon, upperRight.Lat, upperRight.Lon);

            polygon.Should().BeEquivalentTo(new[,]
            {
                { bottomLeft.Lat, bottomLeft.Lon},
                { upperRight.Lat, bottomLeft.Lon},
                { upperRight.Lat, upperRight.Lon},
                { bottomLeft.Lat, upperRight.Lon},
                { bottomLeft.Lat, bottomLeft.Lon},
            });
        }
    }
}
