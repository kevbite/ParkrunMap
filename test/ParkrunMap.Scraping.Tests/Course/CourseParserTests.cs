using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using ParkrunMap.Scraping.Course;
using Xunit;

namespace ParkrunMap.Scraping.Tests.Course
{
    public class CourseParserTests
    {
        [Theory]
        [MemberData(nameof(Data))]
        public async Task ShouldParseCourseDetails(string filePath, string description, string googleMapId)
        {
            using (var cancellationsPage = File.OpenRead(filePath))
            {
                var parser = new CourseParser();
                var course = await parser.Parse(cancellationsPage);

                course.Should().BeEquivalentTo(new
                {
                    Description = description,
                    GoogleMapId = googleMapId
                });
            }

        }

        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[]
                {
                    @".\data\heslington.course.html",
                    "One lap of the 1km tarmac cycle circuit followed by 3km out and back on block paved Lakeside Way and a final lap of the 1km tarmac cycle circuit. The start and finish is approximately 10-minute walk from the car park.",
                    "1BscG9Q0CTyJzJ217yn-dtztpG8CAOa6h"
                },
                new object[]
                {
                    @".\data\york.course.html",
                    "1.5 laps (approximately) of the tarmac service road around the inside of the racecourse. Very flat, with few turns, making it a very fast course. On course map, start at green pin and head anti-clockwise round service road. Complete 1 full lap, then continue on round approximately another 1/2 lap to red Finish pin",
                    "1i-izH2wIlADvEGjfrjQ-zSfg3yc"
                },
            };
    }
}
