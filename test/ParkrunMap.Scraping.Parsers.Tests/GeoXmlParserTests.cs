using System;
using System.IO;
using FluentAssertions;
using Xunit;

namespace ParkrunMap.Scraping.Parsers.Tests
{
    public class GeoXmlParserTests
    {
        [Fact]
        public void ShouldParseGeoFile()
        {
            using (var geoXml = File.OpenRead(@".\data\geo.xml"))
            {
                var parkruns = new GeoXmlParser().Parse(geoXml);

                parkruns.Should().BeEquivalentTo(new[]
                {
                    new Parkrun(1032, "Brightwater", new Uri("http://www.parkrun.com.au/brightwater"), "Queensland",  "Australia", -26.708909, 153.113665),
                    new Parkrun(1744, "Firenze", new Uri("http://www.parkrun.it/firenze"), null,  "Italy", 43.785132, 11.209331),
                    new Parkrun(174, "Mansfield", new Uri("http://www.parkrun.org.uk/mansfield"), "East Midlands",  "UK", 53.174034, -1.183844),
                    new Parkrun(572, "Clumber Park", new Uri("http://www.parkrun.org.uk/clumberpark"), "East Midlands",  "UK", 53.268022, -1.065717),
                    new Parkrun(96, "York", new Uri("http://www.parkrun.org.uk/york"), "Yorkshire and the Humber",  "UK", 53.935375, -1.101379)
                });
            }
        }
    }
}
