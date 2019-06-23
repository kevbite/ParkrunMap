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
                },
                new object[]
                {
                    "www.parkrun.pl",
                    @".\data\cieszyn.course.html",
                    new ParkrunStats
                    {

                    }
                },
                new object[]
                {
                    "www.parkrun.dk",
                    @".\data\amager.course.html",
                    new ParkrunStats
                    {

                    }
                },
                new object[]
                {
                    "www.parkrun.ru",
                    @".\data\balashikhazarechnaya.course.html",
                    new ParkrunStats
                    {

                    }
                },
                new object[]
                {
                    "www.parkrun.com.de",
                    @".\data\alstervorland.course.html",
                    new ParkrunStats
                    {

                    }
                },
                new object[]
                {
                    "www.parkrun.fr",
                    @".\data\boisdeboulogne.course.html",
                    new ParkrunStats
                    {

                    }
                },
                new object[]
                {
                    "www.parkrun.it",
                    @".\data\etna.course.html",
                    new ParkrunStats
                    {

                    }
                },
                new object[]
                {
                    "www.parkrun.co.za",
                    @".\data\aggeneys.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 18,
                        TotalRunners = 119,
                        TotalRuns = 266,
                        AverageRunnersPerWeek = 14.8,
                        AverageRunTime = new TimeSpan(0,44, 22),
                        TotalRunTime = new TimeSpan(8,4,45,10),
                        BiggestAttendance = 55,
                        TotalKmDistanceRan = 1330
                    }
                },
                new object[]
                {
                    "www.parkrun.com.au",
                    @".\data\albert-melbourne.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 374,
                        TotalRunners = 19162,
                        TotalRuns = 91019,
                        AverageRunnersPerWeek = 243.4,
                        AverageRunTime = new TimeSpan(0,28, 25),
                        TotalRunTime = new TimeSpan(336,7,53,15),
                        BiggestAttendance = 997,
                        TotalKmDistanceRan = 455095
                    }
                },
                new object[]
                {
                    "www.parkrun.ie",
                    @".\data\bereisland.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 238,
                        TotalRunners = 1544,
                        TotalRuns = 10891,
                        AverageRunnersPerWeek = 45.8,
                        AverageRunTime = new TimeSpan(0,36, 25),
                        TotalRunTime = new TimeSpan(275,12,34,1),
                        BiggestAttendance = 181,
                        TotalKmDistanceRan = 54455
                    }
                },
                new object[]
                {
                    "www.parkrun.sg",
                    @".\data\bishan.course.html",
                    new ParkrunStats
                    {

                    }
                },
                new object[]
                {
                    "www.parkrun.us",
                    @".\data\anacostia.course.html",
                    new ParkrunStats
                    {

                    }
                },
                new object[]
                {
                    "www.parkrun.co.nz",
                    @".\data\taupo.course.html",
                    new ParkrunStats
                    {
                         TotalEvents = 102,
                        TotalRunners = 1396,
                        TotalRuns = 4047,
                        AverageRunnersPerWeek = 39.7,
                        AverageRunTime = new TimeSpan(0,30, 57),
                        TotalRunTime = new TimeSpan(87,0,23,46),
                        BiggestAttendance = 128,
                        TotalKmDistanceRan = 20235
                    }
                },
                new object[]
                {
                    "www.parkrun.se",
                    @".\data\orebro.course.html",
                    new ParkrunStats
                    {

                    }
                },
                new object[]
                {
                    "www.parkrun.ca",
                    @".\data\beachstrip.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 77,
                        TotalRunners = 553,
                        TotalRuns = 1660,
                        AverageRunnersPerWeek = 21.6,
                        AverageRunTime = new TimeSpan(0,29,54),
                        TotalRunTime = new TimeSpan(34,11,22,3),
                        BiggestAttendance = 44,
                        TotalKmDistanceRan = 8300
                    }
                },
                new object[]
                {
                    "www.parkrun.no",
                    @".\data\festningen.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 9,
                        TotalRunners = 164,
                        TotalRuns = 372,
                        AverageRunnersPerWeek = 41.3,
                        AverageRunTime = new TimeSpan(0,28, 16),
                        TotalRunTime = new TimeSpan(7,7,19,57),
                        BiggestAttendance = 77,
                        TotalKmDistanceRan = 1860
                    }
                },
                new object[]
                {
                    "www.parkrun.fi",
                    @".\data\tokoinranta.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 15,
                        TotalRunners = 329,
                        TotalRuns = 637,
                        AverageRunnersPerWeek = 42.5,
                        AverageRunTime = new TimeSpan(0,30, 41),
                        TotalRunTime = new TimeSpan(13,13,51,45),
                        BiggestAttendance = 78,
                        TotalKmDistanceRan = 3185
                    }
                },
                new object[]
                {
                    "www.parkrun.my",
                    @".\data\tamanpuduulu.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 37,
                        TotalRunners = 763,
                        TotalRuns = 2538,
                        AverageRunnersPerWeek = 68.6,
                        AverageRunTime = new TimeSpan(0,34, 17),
                        TotalRunTime = new TimeSpan(60,10,18,42),
                        BiggestAttendance = 129,
                        TotalKmDistanceRan = 12690
                    }
                },
                new object[]
                {
                    "www.parkrun.jp",
                    @".\data\futakotamagawa.course.html",
                    new ParkrunStats
                    {

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
