using System;
using System.IO;
using FluentAssertions;
using ParkrunMap.Scraping.Cancellations;
using Xunit;

namespace ParkrunMap.Scraping.Tests.Cancellations
{
    public class CancellationsParserTests
    {
        [Fact]
        public void ShouldParseCancellations()
        {
            using (var cancellationsPage = File.OpenRead(@"data/cancellations.html"))
            {
                var parser = new CancellationsParser();
                var cancellations = parser.Parse(cancellationsPage);

                cancellations.Should().BeEquivalentTo(new[]
                {
                    new ParkrunCancellation(new DateTime(2022, 12, 24), "Babbs Mill parkrun",
                        "www.parkrun.org.uk", "/babbsmill", "Following the tragic incident on Sunday 11th"),
                    new ParkrunCancellation(new DateTime(2022, 12, 24), "Bedworth parkrun",
                        "www.parkrun.org.uk", "/bedworth", "Ice, on the course"),
                    new ParkrunCancellation(new DateTime(2022, 12, 24), "Blickling parkrun",
                        "www.parkrun.org.uk", "/blickling", "Park unavailable to events"),

                    new ParkrunCancellation(new DateTime(2022, 12, 31), "Chasewater parkrun",
                        "www.parkrun.org.uk", "/chasewater", "Request by landowner due to other event in park"),
                    new ParkrunCancellation(new DateTime(2022, 12, 31), "Chipping Norton School parkrun",
                        "www.parkrun.org.uk", "/chippingnortonschool", "Core team unavailable"),
                });
            }
        }

        [Fact]
        public void ShouldParseCancellationsAndExcludeJuniorPakruns()
        {
            using (var cancellationsPage = File.OpenRead(@"data/cancellations.1.html"))
            {
                var parser = new CancellationsParser();
                var cancellations = parser.Parse(cancellationsPage);

                cancellations.Should().BeEmpty();
            }
        }
    }
}