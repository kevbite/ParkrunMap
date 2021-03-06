using System;
using System.IO;
using FluentAssertions;
using ParkrunMap.Scraping.Parkruns;
using Xunit;

namespace ParkrunMap.Scraping.Tests.Parkruns
{
    public class GeoXmlParserTests
    {
        [Fact]
        public void ShouldParseGeoFile()
        {
            using (var geoXml = File.OpenRead(@".\data\geo.xml"))
            {
                var parkruns = new GeoXmlParser(new ParkrunXElementValidator()).Parse(geoXml);

                parkruns.Should().BeEquivalentTo(new[]
                {
                    new Parkrun(1032, "Brightwater", "www.parkrun.com.au", "/brightwater", "Queensland",  "Australia", -26.708909, 153.113665),
                    new Parkrun(1744, "Firenze", "www.parkrun.it", "/firenze", null,  "Italy", 43.785132, 11.209331),
                    new Parkrun(174, "Mansfield", "www.parkrun.org.uk", "/mansfield", "East Midlands",  "UK", 53.174034, -1.183844),
                    new Parkrun(572, "Clumber Park", "www.parkrun.org.uk", "/clumberpark", "East Midlands",  "UK", 53.268022, -1.065717),
                    new Parkrun(96, "York", "www.parkrun.org.uk", "/york", "Yorkshire and the Humber",  "UK", 53.935375, -1.101379)
                });
            }
        }
    }
}
