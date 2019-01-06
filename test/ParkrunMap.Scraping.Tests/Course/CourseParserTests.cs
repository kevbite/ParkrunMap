using System.IO;
using FluentAssertions;
using Xunit;

namespace ParkrunMap.Scraping.Tests.Course
{
    public class CourseParserTests
    {
        [Fact]
        public void ShouldParseCourseDetails()
        {
            using (var cancellationsPage = File.OpenRead(@".\data\heslington.course.html"))
            {
                var parser = new CourseParser();
                var course = parser.Parse(cancellationsPage);

                course.Should().BeEquivalentTo(new
                {
                    Description = "One lap of the 1km tarmac cycle circuit followed by 3km out and back on block paved Lakeside Way and a final lap of the 1km tarmac cycle circuit. The start and finish is approximately 10-minute walk from the car park.",
                    GoogleMapId = "1BscG9Q0CTyJzJ217yn-dtztpG8CAOa6h"
                });
            }

        }
    }
}
