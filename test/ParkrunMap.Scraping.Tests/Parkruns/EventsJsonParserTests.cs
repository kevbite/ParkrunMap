using System;
using System.IO;
using FluentAssertions;
using ParkrunMap.Scraping.Parkruns;
using Xunit;

namespace ParkrunMap.Scraping.Tests.Parkruns
{
    public class EventsJsonParserTests
    {
        [Fact]
        public void ShouldParseEventsFile()
        {
            using (var fileStream = File.OpenRead(@"data/events.json"))
            {
                var parkruns = new EventsJsonParser().Parse(fileStream);

                parkruns.Should().BeEquivalentTo(new[]
                {
                    new EventsJsonParkrun("Bushy Park", "www.parkrun.org.uk", "/bushy",  51.410992, -0.335791),
                    new EventsJsonParkrun("York", "www.parkrun.org.uk", "/york",  53.935375, -1.101379),
                    new EventsJsonParkrun("Heslington", "www.parkrun.org.uk", "/heslington",  53.949319, -1.01799),
                    new EventsJsonParkrun("Shiraz Trail", "www.parkrun.com.au", "/shiraztrail",  -35.217671, 138.547611)
                });
            }
        }
    }
}
