using System.IO;
using FluentAssertions;
using ParkrunMap.Scraping.SpecialEvents;
using Xunit;

namespace ParkrunMap.Scraping.Tests.SpecialEvents
{
    public class SpecialEventsParserTests
    {
        [Fact]
        public void ShouldParseSpecialEvents()
        {
            using (var cancellationsPage = File.OpenRead(@"data/uk.special-events.html"))
            {
                var parser = new SpecialEventsParser();
                var specialEvents = parser.Parse(cancellationsPage);

                specialEvents.Should().BeEquivalentTo(new[]
                {
                    new SpecialEvent("www.parkrun.org.uk", "/aberbeeg", 2022, SpecialEventType.ChristmasDay, false),
                    new SpecialEvent("www.parkrun.org.uk", "/aberbeeg", 2023, SpecialEventType.NewYearsDay, false),

                    new SpecialEvent("www.parkrun.org.uk", "/aberdare", 2022, SpecialEventType.ChristmasDay, true),
                    new SpecialEvent("www.parkrun.org.uk", "/aberdare", 2023, SpecialEventType.NewYearsDay, false),

                    new SpecialEvent("www.parkrun.org.uk", "/aberdeen", 2022, SpecialEventType.ChristmasDay, true),
                    new SpecialEvent("www.parkrun.org.uk", "/aberdeen", 2023, SpecialEventType.NewYearsDay, true),

                    new SpecialEvent("www.parkrun.org.uk", "/aberystwyth", 2022, SpecialEventType.ChristmasDay, false),
                    new SpecialEvent("www.parkrun.org.uk", "/aberystwyth", 2023, SpecialEventType.NewYearsDay, true),
                });
            }
        }
    }
}