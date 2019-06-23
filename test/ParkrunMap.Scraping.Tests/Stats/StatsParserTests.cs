using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using ParkrunMap.Scraping.Course;
using ParkrunMap.Scraping.Stats;
using Xunit;

namespace ParkrunMap.Scraping.Tests.Stats
{
    public class StatsParserTests
    {
        [Theory]
        [MemberData(nameof(Data))]
        public async Task ShouldParseStatsFromCourseDetails(string domain, string filePath, ParkrunStats expectedStats)
        {
            using (var cancellationsPage = File.OpenRead(filePath))
            {
                var parser = new StatsParser();
                var stats = await parser.Parse(cancellationsPage, domain);

                stats.Should().BeEquivalentTo(expectedStats);
            }

        }

        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[]
                {
                    "www.parkrun.org.uk",
                    @".\data\heslington.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 47,
                        TotalRunners = 3966,
                        TotalRuns = 14363,
                        AverageRunnersPerWeek = 305.6,
                        AverageRunTime = new TimeSpan(0, 0, 27, 54, 0),
                        TotalRunTime = new TimeSpan(278, 7,28,41,0),
                        BiggestAttendance = 581,
                        TotalKmDistanceRan = 71815
                    }
                }
            };

        public class ParkrunStats
        {
            public int TotalEvents { get; internal set; }

            public int TotalRunners { get; internal set; }

            public int TotalRuns { get; internal set; }

            public double AverageRunnersPerWeek { get; internal set; }

            public TimeSpan AverageRunTime { get; internal set; }

            public TimeSpan TotalRunTime { get; internal set; }

            public int BiggestAttendance { get; internal set; }

            public int TotalKmDistanceRan { get; internal set; }
        }
    }
}
