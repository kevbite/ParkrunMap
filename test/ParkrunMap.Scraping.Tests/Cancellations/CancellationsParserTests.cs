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
                    new ParkrunCancellation(new DateTime(2018,12,29), "Bodelwyddan Castle parkrun",
                        "www.parkrun.org.uk","/bodelwyddancastle", "Estate closed for festive period"),
                    new ParkrunCancellation(new DateTime(2018,12,29), "Dudley parkrun",
                        "www.parkrun.org.uk","/dudley", "Dell Stadium closed due to DMBC shutdown over Xmas"),
                    new ParkrunCancellation(new DateTime(2018,12,29), "Hay Lodge parkrun",
                        "www.parkrun.org.uk","/haylodge", "High winds"),
                    new ParkrunCancellation(new DateTime(2018,12,29), "Lochore Meadows parkrun",
                        "www.parkrun.org.uk","/lochoremeadows", "Unknown (HQ)"),
                    new ParkrunCancellation(new DateTime(2018,12,29), "Mulbarton parkrun",
                        "www.parkrun.org.uk","/mulbarton", "Protocols to install new ED"),
                    new ParkrunCancellation(new DateTime(2018,12,29), "Newent parkrun, Forest of Dean",
                        "www.parkrun.org.uk","/newent", "School grounds are closed for Christmas holiday"),
                    new ParkrunCancellation(new DateTime(2018,12,29), "Omagh parkrun",
                        "www.parkrun.org.uk","/omagh", "course closed"),
                    new ParkrunCancellation(new DateTime(2018,12,29), "Sandwell Valley parkrun",
                        "www.parkrun.org.uk","/sandwellvalley", "No access to an AED as the Park Farm is closed."),
                    new ParkrunCancellation(new DateTime(2018,12,29), "Vogrie parkrun",
                        "www.parkrun.org.uk","/vogrie", "Run cancelled due to high winds forecast"),

                    new ParkrunCancellation(new DateTime(2019,01,05), "Bodelwyddan Castle parkrun",
                        "www.parkrun.org.uk","/bodelwyddancastle", "Run Director not available - Cross Country Champs"),
                    new ParkrunCancellation(new DateTime(2019,01,05), "Bramley parkrun",
                        "www.parkrun.org.uk","/bramley", "removal of asbestos from a park facility"),
                    new ParkrunCancellation(new DateTime(2019,01,05), "Witton parkrun",
                        "www.parkrun.org.uk","/witton", "Cross Country Event in the park"),

                    new ParkrunCancellation(new DateTime(2019,01,12), "Bramley parkrun",
                        "www.parkrun.org.uk","/bramley", "removal of asbestos from a park facility"),
                    new ParkrunCancellation(new DateTime(2019,01,12), "Prospect parkrun",
                        "www.parkrun.org.uk","/prospect", "Hampshire Cross Country League event in the park"),

                    new ParkrunCancellation(new DateTime(2019,01,19), "Bramley parkrun",
                        "www.parkrun.org.uk","/bramley", "removal of asbestos from a park facility"),

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
