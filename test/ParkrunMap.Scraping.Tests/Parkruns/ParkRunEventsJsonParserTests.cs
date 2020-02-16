using System.IO;
using System.Linq;
using FluentAssertions;
using ParkrunMap.Scraping.Parkruns;
using Xunit;

namespace ParkrunMap.Scraping.Tests.Parkruns
{
    public class ParkRunEventsJsonParserTests
    {
        [Fact]
        public void ShouldParseParkRunEventsJson()
        {
            using (var jsonEvents = File.OpenRead(@".\data\events.json"))
            {
                ParkRunEventsJsonParser.Parse(jsonEvents).Should().BeEquivalentTo(
                    new Parkrun(1032, "Brightwater", "www.parkrun.com.au", "/brightwater", null, null, -26.708909, 153.113665),
                    new Parkrun(1744, "Firenze", "www.parkrun.it", "/firenze", null, null, 43.785132, 11.209331),
                    new Parkrun(174, "Mansfield", "www.parkrun.org.uk", "/mansfield", null, null, 53.174034, -1.183844),
                    new Parkrun(572, "Clumber Park", "www.parkrun.org.uk", "/clumberpark", null, null, 53.268022, -1.065717),
                    new Parkrun(96, "York", "www.parkrun.org.uk", "/york", null, null, 53.935375, -1.101379));
            }
        }
    }
}