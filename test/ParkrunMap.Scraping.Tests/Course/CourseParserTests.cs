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
        public async Task ShouldParseCourseDetails(string domain, string filePath, string description, string googleMapId)
        {
            using (var cancellationsPage = File.OpenRead(filePath))
            {
                var parser = new CourseParser();
                var course = await parser.Parse(cancellationsPage, domain);

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
                    "www.parkrun.org.uk",
                    @".\data\heslington.course.html",
                    "One lap of the 1km tarmac cycle circuit followed by 3km out and back on block paved Lakeside Way and a final lap of the 1km tarmac cycle circuit. The start and finish is approximately 10-minute walk from the car park.",
                    "1BscG9Q0CTyJzJ217yn-dtztpG8CAOa6h"
                },
                new object[]
                {
                    "www.parkrun.org.uk",
                    @".\data\york.course.html",
                    "1.5 laps (approximately) of the tarmac service road around the inside of the racecourse. Very flat, with few turns, making it a very fast course. On course map, start at green pin and head anti-clockwise round service road. Complete 1 full lap, then continue on round approximately another 1/2 lap to red Finish pin",
                    "1i-izH2wIlADvEGjfrjQ-zSfg3yc"
                },
                new object[]
                {
                    "www.parkrun.org.uk",
                    @".\data\blackpark.course.html",
                    @"The course is one lap on firm, flat and wide tracks of compacted earth and gravel. Fast times can be expected. The start is approximately 100m along the track at the east end of the car park. All course turns are marked with direction arrows - if there is no sign you should always stay on the path you are on.",
                    "1ImV8WgyJK51NLiTeZUC17IlnA4I"
                },
                new object[]
                {
                    "www.parkrun.pl",
                    @".\data\cieszyn.course.html",
                    @"Trasa umiejscowiona jest w SportParku po polskiej i czeskiej stronie granicy. Start przy bramie na stadion miejski w Cieszynie, następnie przebieg trasy aleją Łyska w stronę mostu sportowego, przez który wbiegamy do Czeskiego Cieszyna, dalej w prawo po łuku i wzdłóż rzeki Olzy biegniemy w kierunku Mostu Wolności. Przed Mostem Wolności nawracamy i biegniemy spowrotem wzdłóż rzeki Olzy aż do Parku Sikory, dalej ścieżką wokół Parku Sikory (wewnątrz parku), mijamy restaurację ""Sikorak"" przy wyjściu i wbiegamy na most sportowy w stronę Polski. Za mostem skręcamy w prawo po łuku do ścieżki wzdłóż rzeki Olzy w stronę boiska ""Pod Wałką"". Za boiskiem skręcamy w lewo, ścieżką wokół miejsca piknikowego i wracamy tą samą trasą w kierunku mostu sportowego, dalej aleją Łyska do mety przy boisku miejskim.",
                    "1m4rDKSNnQW-muQXOKZFLqXHOCVs"
                },
            };
    }
}
