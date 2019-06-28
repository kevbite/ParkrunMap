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
                        TotalEvents = 69,
                        TotalRunners = 5323,
                        TotalRuns = 22955,
                        AverageRunnersPerWeek = 332.7,
                        AverageSecondsRan = (int)new TimeSpan(0, 0, 28, 05, 0).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(82, 20, 4, 52,0).TotalSeconds + (long)TimeSpan.FromDays(365).TotalSeconds,
                        BiggestAttendance = 593,
                        TotalKmDistanceRan = 114775
                    }
                },
                new object[]
                {
                    "www.parkrun.pl",
                    @".\data\cieszyn.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 275,
                        TotalRunners = 1327,
                        TotalRuns = 14016,
                        AverageRunnersPerWeek = 51.0,
                        AverageSecondsRan = (int)new TimeSpan(0, 0, 25, 18, 0).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(246, 7, 37, 46,0).TotalSeconds,
                        BiggestAttendance = 167,
                        TotalKmDistanceRan = 70080
                    }
                },
                new object[]
                {
                    "www.parkrun.dk",
                    @".\data\amager.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 540,
                        TotalRunners = 5235,
                        TotalRuns = 35169,
                        AverageRunnersPerWeek = 65.1,
                        AverageSecondsRan = (int)new TimeSpan(0, 0, 26, 8, 0).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(273,8,42,23,0).TotalSeconds + (long)TimeSpan.FromDays(365).TotalSeconds,
                        BiggestAttendance = 181,
                        TotalKmDistanceRan = 175845
                    }
                },
                new object[]
                {
                    "www.parkrun.ru",
                    @".\data\balashikhazarechnaya.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 64,
                        TotalRunners = 411,
                        TotalRuns = 1395,
                        AverageRunnersPerWeek = 21.8,
                        AverageSecondsRan = (int)new TimeSpan(0, 0, 26, 30, 0).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(25,16,13,5, 0).TotalSeconds,
                        BiggestAttendance = 91,
                        TotalKmDistanceRan = 6975
                    }
                },
                new object[]
                {
                    "www.parkrun.com.de",
                    @".\data\alstervorland.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 24,
                        TotalRunners = 534,
                        TotalRuns = 1090,
                        AverageRunnersPerWeek = 45.4,
                        AverageSecondsRan = (int)new TimeSpan(0, 0, 28, 15, 0).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(21, 9, 28, 49, 0).TotalSeconds,
                        BiggestAttendance = 102,
                        TotalKmDistanceRan = 5450
                    }
                },
                new object[]
                {
                    "www.parkrun.fr",
                    @".\data\boisdeboulogne.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 172,
                        TotalRunners = 3515,
                        TotalRuns = 5908,
                        AverageRunnersPerWeek = 34.3,
                        AverageSecondsRan = (int)new TimeSpan(0, 0, 27, 40, 0).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(113,13,48,28, 0).TotalSeconds,
                        BiggestAttendance = 206,
                        TotalKmDistanceRan = 29540
                    }
                },
                new object[]
                {
                    "www.parkrun.it",
                    @".\data\etna.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 81,
                        TotalRunners = 727,
                        TotalRuns = 1891,
                        AverageRunnersPerWeek = 23.3,
                        AverageSecondsRan = (int)new TimeSpan(0, 0, 35, 7, 0).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(46,3,9,48, 0).TotalSeconds,
                        BiggestAttendance = 61,
                        TotalKmDistanceRan = 9455
                    }
                },
                new object[]
                {
                    "www.parkrun.co.za",
                    @".\data\aggeneys.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 40,
                        TotalRunners = 163,
                        TotalRuns = 498,
                        AverageRunnersPerWeek = 12.5,
                        AverageSecondsRan = (int)new TimeSpan(0,45, 3).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(15,13,58,16).TotalSeconds,
                        BiggestAttendance = 55,
                        TotalKmDistanceRan = 2490
                    }
                },
                new object[]
                {
                    "www.parkrun.com.au",
                    @".\data\albert-melbourne.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 396,
                        TotalRunners = 21411,
                        TotalRuns = 101050,
                        AverageRunnersPerWeek = 255.2,
                        AverageSecondsRan = (int)new TimeSpan(0,28,29).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(173, 22, 2, 23).TotalSeconds + (long)TimeSpan.FromDays(5 * 365).TotalSeconds,
                        BiggestAttendance = 997,
                        TotalKmDistanceRan = 505250
                    }
                },
                new object[]
                {
                    "www.parkrun.ie",
                    @".\data\bereisland.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 261,
                        TotalRunners = 1712,
                        TotalRuns = 11998,
                        AverageRunnersPerWeek = 46.0,
                        AverageSecondsRan = (int)new TimeSpan(0,36, 25).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(303, 10, 47, 45).TotalSeconds,
                        BiggestAttendance = 181,
                        TotalKmDistanceRan = 59990
                    }
                },
                new object[]
                {
                    "www.parkrun.sg",
                    @".\data\bishan.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 64,
                        TotalRunners = 1117,
                        TotalRuns = 3201,
                        AverageRunnersPerWeek = 50.0,
                        AverageSecondsRan = (int)new TimeSpan(0,30, 13).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(67,4,49,50).TotalSeconds,
                        BiggestAttendance = 150,
                        TotalKmDistanceRan = 16005
                    }
                },
                new object[]
                {
                    "www.parkrun.us",
                    @".\data\anacostia.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 75,
                        TotalRunners = 661,
                        TotalRuns = 1943,
                        AverageRunnersPerWeek = 25.9,
                        AverageSecondsRan = (int)new TimeSpan(0, 34, 23).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(46, 9, 33, 11).TotalSeconds,
                        BiggestAttendance = 59,
                        TotalKmDistanceRan = 9715
                    }
                },
                new object[]
                {
                    "www.parkrun.co.nz",
                    @".\data\taupo.course.html",
                    new ParkrunStats
                    {
                         TotalEvents = 124,
                        TotalRunners = 1808,
                        TotalRuns = 5470,
                        AverageRunnersPerWeek = 44.1,
                        AverageSecondsRan = (int)new TimeSpan(0,31, 1).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(117, 20, 28, 25).TotalSeconds,
                        BiggestAttendance = 136,
                        TotalKmDistanceRan = 27350
                    }
                },
                new object[]
                {
                    "www.parkrun.se",
                    @".\data\orebro.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 109,
                        TotalRunners = 1278,
                        TotalRuns = 6970,
                        AverageRunnersPerWeek = 63.9,
                        AverageSecondsRan = (int)new TimeSpan(0,29,31).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(142,21,3,33).TotalSeconds,
                        BiggestAttendance = 199,
                        TotalKmDistanceRan = 34850
                    }
                },
                new object[]
                {
                    "www.parkrun.ca",
                    @".\data\beachstrip.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 97,
                        TotalRunners = 652,
                        TotalRuns = 2126,
                        AverageRunnersPerWeek = 21.9,
                        AverageSecondsRan = (int)new TimeSpan(0,29,49).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(44,0,52,56).TotalSeconds,
                        BiggestAttendance = 53,
                        TotalKmDistanceRan = 10630
                    }
                },
                new object[]
                {
                    "www.parkrun.no",
                    @".\data\festningen.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 30,
                        TotalRunners = 317,
                        TotalRuns = 952,
                        AverageRunnersPerWeek = 31.7,
                        AverageSecondsRan = (int)new TimeSpan(0,28,51).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(19,2,0,24).TotalSeconds,
                        BiggestAttendance = 77,
                        TotalKmDistanceRan = 4760
                    }
                },
                new object[]
                {
                    "www.parkrun.fi",
                    @".\data\tokoinranta.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 33,
                        TotalRunners = 561,
                        TotalRuns = 1262,
                        AverageRunnersPerWeek = 38.2,
                        AverageSecondsRan = (int)new TimeSpan(0,30, 25).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(26,16,4,53).TotalSeconds,
                        BiggestAttendance = 78,
                        TotalKmDistanceRan = 6310
                    }
                },
                new object[]
                {
                    "www.parkrun.my",
                    @".\data\tamanpuduulu.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 58,
                        TotalRunners = 1078,
                        TotalRuns = 4076,
                        AverageRunnersPerWeek = 70.3,
                        AverageSecondsRan = (int)new TimeSpan(0,34,16).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(97,0,57,37).TotalSeconds,
                        BiggestAttendance = 129,
                        TotalKmDistanceRan = 20380
                    }
                },
                new object[]
                {
                    "www.parkrun.jp",
                    @".\data\futakotamagawa.course.html",
                    new ParkrunStats
                    {
                        TotalEvents = 12,
                        TotalRunners = 954,
                        TotalRuns = 2032,
                        AverageRunnersPerWeek = 169.3,
                        AverageSecondsRan = (int)new TimeSpan(0,31,30).TotalSeconds,
                        TotalSecondsRan = (long)new TimeSpan(44,11,7,23).TotalSeconds,
                        BiggestAttendance = 343,
                        TotalKmDistanceRan = 10160
                    }
                }
            };

        public class ParkrunStats
        {
            public int TotalEvents { get; internal set; }

            public int TotalRunners { get; internal set; }

            public int TotalRuns { get; internal set; }

            public double AverageRunnersPerWeek { get; internal set; }

            public int AverageSecondsRan { get; internal set; }

            public long TotalSecondsRan { get; internal set; }

            public int BiggestAttendance { get; internal set; }

            public int TotalKmDistanceRan { get; internal set; }
        }
    }
}
