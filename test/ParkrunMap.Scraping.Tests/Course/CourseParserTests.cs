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
                new object[]
                {
                    "www.parkrun.dk",
                    @".\data\amager.course.html",
                    @"Sommerruten er to sløjfer á 2,45 km plus 100 meter enten på græs eller på stien alt efter vejret den dag. Den er meget løbevenlig. Stierne er hovedsageligbelagt med grus eller sandjord. I regnvejr kan der dannes vandpytter men overfladen er pæn og jævn. Ruten er hurtig og deltagerne kommer ikke ud på Ørestadsboulevarden.",
                    "1eQtWTqScHsieaKg8JkhaCoCQcJY"
                },
                new object[]
                {
                    "www.parkrun.ru",
                    @".\data\balashikhazarechnaya.course.html",
                    @"Трасса проходит по широкой пешеходной парковой зоне вдоль реки Пехорки - два круга по 2,5 км, маятниковая. Рельеф трассы плоский, без горок и спусков. Дорога вымощена плиткой.",
                    "194KmLVUahJO0D5rKV--qex_S6YtGw3ea"
                },
                new object[]
                {
                    "www.parkrun.com.de",
                    @".\data\alstervorland.course.html",
                    @"Start und Ziel befinden sich am Café AlsterCliff auf der westlichen Seite der Alster. Die Strecke besteht aus zwei Runden.",
                    "1msDEMpZD2tjwcoA0JVfJ5bewocBCTnkC"
                },
                new object[]
                {
                    "www.parkrun.fr",
                    @".\data\boisdeboulogne.course.html",
                    @"Le point de départ se situe tout près du panneau d’information se situant à l’intersection de la Route d’Auteuil sur Suresnes et la Route de Boulogne à Passy. Le parcours comprend une grande boucle (le périmètre extérieur du tracé), et deux répétitions de la boucle inférieure. Au départ, les parkrunneurs descendront le long chemin sans nom, parallèle à la Route de Boulogne à Passy, dans le sens des aiguilles d’une montre et sur une distance d’environ 400m (assujettie à un dénivelé de 10m). Une fois à la fin de ce chemin, ils tourneront à droite et se dirigeront vers l’Allée Saint-Denis (une soixantaine de mètres à l’Est). La montée de l’Allée Saint-Denis est de 780m, et est sujet à un dénivelé 15m+. Les coureurs tourneront ensuite à droite sur la Route d’Auteuil à Suresnes sur 400m. Une fois arrivés au carrefour, ils tourneront à droite et descendront l’Avenue Saint-Cloud sur 570m (dénivelé 14m-), pour ensuite retracer leurs pas en remontant le chemin parallèle à la Route de Boulogne à Passy dans le sens inverse du départ et jusqu’au bout du chemin qui se situe 750m plus loin. La remontée de ce chemin est assujettie à un dénivelé 11m+. Une fois arrivés au bout, ils tourneront à gauche sur le Chemin des Vieux Chênes - empruntant celui-ci sur 150m - puis à gauche sur la Route de la Seine à la Butte Mortemart, ce qui les mènera de nouveau au carrefour après 190m. Ils redescendront l’Avenue Saint-Cloud sur 570m, tourneront à nouveau à gauche sur le chemin parallèle à la Route de Boulogne à Passy, puis remonteront jusqu’au bout du chemin. Ils tourneront alors à gauche sur le Chemin des Vieux Chênes, puis à nouveau à gauche sur la Route de la Seine à la Butte Mortemart 150m plus tard. Apres 190 mètres, le sas d’arrivée se trouvera sur leur gauche. N.B: Ceux effectuant la boucle inférieure la première fois seront demandés de rester sur la droite du chemin, afin de laisser libre la voie gauche pour que ceux qui la courent la deuxième fois puissent rentrer dans le sas d’arrivé sans être gênés.",
                    "z1IWCRFkMAYk.kxXvfhdGAbGU"
                },
                new object[]
                {
                    "www.parkrun.it",
                    @".\data\etna.course.html",
                    @"Attualmente, a causa della impraticabilità del percorso principale, il percorso è dentro l’area attrezzata sottostante la “Pineta Monti Rossi”, ed è costituito da n. 4 giri antiorari di 1,25 km.",
                    "1zUEqOtkI8-E2sl2FJEU0nHNVS6wecDxm"
                },
            };
    }
}
