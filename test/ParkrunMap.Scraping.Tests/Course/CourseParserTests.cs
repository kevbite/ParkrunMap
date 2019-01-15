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
        public async Task ShouldParseCourseDetails(string domain, string filePath, string description, string[] googleMapIds)
        {
            using (var cancellationsPage = File.OpenRead(filePath))
            {
                var parser = new CourseParser();
                var course = await parser.Parse(cancellationsPage, domain);

                course.Should().BeEquivalentTo(new
                {
                    Description = description,
                    GoogleMapIds = googleMapIds
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
                    new []{"1BscG9Q0CTyJzJ217yn-dtztpG8CAOa6h"}
                },
                new object[]
                {
                    "www.parkrun.org.uk",
                    @".\data\york.course.html",
                    "1.5 laps (approximately) of the tarmac service road around the inside of the racecourse. Very flat, with few turns, making it a very fast course. On course map, start at green pin and head anti-clockwise round service road. Complete 1 full lap, then continue on round approximately another 1/2 lap to red Finish pin",
                    new []{"1i-izH2wIlADvEGjfrjQ-zSfg3yc"}
                },
                new object[]
                {
                    "www.parkrun.org.uk",
                    @".\data\blackpark.course.html",
                    @"The course is one lap on firm, flat and wide tracks of compacted earth and gravel. Fast times can be expected. The start is approximately 100m along the track at the east end of the car park. All course turns are marked with direction arrows - if there is no sign you should always stay on the path you are on.",
                    new []{"1ImV8WgyJK51NLiTeZUC17IlnA4I"}
                },
                new object[]
                {
                    "www.parkrun.pl",
                    @".\data\cieszyn.course.html",
                    @"Trasa umiejscowiona jest w SportParku po polskiej i czeskiej stronie granicy. Start przy bramie na stadion miejski w Cieszynie, następnie przebieg trasy aleją Łyska w stronę mostu sportowego, przez który wbiegamy do Czeskiego Cieszyna, dalej w prawo po łuku i wzdłóż rzeki Olzy biegniemy w kierunku Mostu Wolności. Przed Mostem Wolności nawracamy i biegniemy spowrotem wzdłóż rzeki Olzy aż do Parku Sikory, dalej ścieżką wokół Parku Sikory (wewnątrz parku), mijamy restaurację ""Sikorak"" przy wyjściu i wbiegamy na most sportowy w stronę Polski. Za mostem skręcamy w prawo po łuku do ścieżki wzdłóż rzeki Olzy w stronę boiska ""Pod Wałką"". Za boiskiem skręcamy w lewo, ścieżką wokół miejsca piknikowego i wracamy tą samą trasą w kierunku mostu sportowego, dalej aleją Łyska do mety przy boisku miejskim.",
                    new []{"1m4rDKSNnQW-muQXOKZFLqXHOCVs"}
                },
                new object[]
                {
                    "www.parkrun.dk",
                    @".\data\amager.course.html",
                    @"Sommerruten er to sløjfer á 2,45 km plus 100 meter enten på græs eller på stien alt efter vejret den dag. Den er meget løbevenlig. Stierne er hovedsageligbelagt med grus eller sandjord. I regnvejr kan der dannes vandpytter men overfladen er pæn og jævn. Ruten er hurtig og deltagerne kommer ikke ud på Ørestadsboulevarden.",
                    new []{"1eQtWTqScHsieaKg8JkhaCoCQcJY"}
                },
                new object[]
                {
                    "www.parkrun.ru",
                    @".\data\balashikhazarechnaya.course.html",
                    @"Трасса проходит по широкой пешеходной парковой зоне вдоль реки Пехорки - два круга по 2,5 км, маятниковая. Рельеф трассы плоский, без горок и спусков. Дорога вымощена плиткой.",
                    new []{ "194KmLVUahJO0D5rKV--qex_S6YtGw3ea"}
                },
                new object[]
                {
                    "www.parkrun.com.de",
                    @".\data\alstervorland.course.html",
                    @"Start und Ziel befinden sich am Café AlsterCliff auf der westlichen Seite der Alster. Die Strecke besteht aus zwei Runden.",
                    new []{"1msDEMpZD2tjwcoA0JVfJ5bewocBCTnkC"}
                },
                new object[]
                {
                    "www.parkrun.fr",
                    @".\data\boisdeboulogne.course.html",
                    @"Le point de départ se situe tout près du panneau d’information se situant à l’intersection de la Route d’Auteuil sur Suresnes et la Route de Boulogne à Passy. Le parcours comprend une grande boucle (le périmètre extérieur du tracé), et deux répétitions de la boucle inférieure. Au départ, les parkrunneurs descendront le long chemin sans nom, parallèle à la Route de Boulogne à Passy, dans le sens des aiguilles d’une montre et sur une distance d’environ 400m (assujettie à un dénivelé de 10m). Une fois à la fin de ce chemin, ils tourneront à droite et se dirigeront vers l’Allée Saint-Denis (une soixantaine de mètres à l’Est). La montée de l’Allée Saint-Denis est de 780m, et est sujet à un dénivelé 15m+. Les coureurs tourneront ensuite à droite sur la Route d’Auteuil à Suresnes sur 400m. Une fois arrivés au carrefour, ils tourneront à droite et descendront l’Avenue Saint-Cloud sur 570m (dénivelé 14m-), pour ensuite retracer leurs pas en remontant le chemin parallèle à la Route de Boulogne à Passy dans le sens inverse du départ et jusqu’au bout du chemin qui se situe 750m plus loin. La remontée de ce chemin est assujettie à un dénivelé 11m+. Une fois arrivés au bout, ils tourneront à gauche sur le Chemin des Vieux Chênes - empruntant celui-ci sur 150m - puis à gauche sur la Route de la Seine à la Butte Mortemart, ce qui les mènera de nouveau au carrefour après 190m. Ils redescendront l’Avenue Saint-Cloud sur 570m, tourneront à nouveau à gauche sur le chemin parallèle à la Route de Boulogne à Passy, puis remonteront jusqu’au bout du chemin. Ils tourneront alors à gauche sur le Chemin des Vieux Chênes, puis à nouveau à gauche sur la Route de la Seine à la Butte Mortemart 150m plus tard. Apres 190 mètres, le sas d’arrivée se trouvera sur leur gauche. N.B: Ceux effectuant la boucle inférieure la première fois seront demandés de rester sur la droite du chemin, afin de laisser libre la voie gauche pour que ceux qui la courent la deuxième fois puissent rentrer dans le sas d’arrivé sans être gênés.",
                    new []{"z1IWCRFkMAYk.kxXvfhdGAbGU"}
                },
                new object[]
                {
                    "www.parkrun.it",
                    @".\data\etna.course.html",
                    @"Attualmente, a causa della impraticabilità del percorso principale, il percorso è dentro l’area attrezzata sottostante la “Pineta Monti Rossi”, ed è costituito da n. 4 giri antiorari di 1,25 km.",
                    new []{"1zUEqOtkI8-E2sl2FJEU0nHNVS6wecDxm"}
                },
                new object[]
                {
                    "www.parkrun.co.za",
                    @".\data\aggeneys.course.html",
                    @"Our course is an out and back, 1 way vehicle accessible, rocky path with the most beautiful views of the Boesmanland veld, mountains and the Kokerboom.",
                    new []{"19W-cF3unI6O4CGcDHoIRL9XsWCGeNysM"}
                },
                new object[]
                {
                    "www.parkrun.com.au",
                    @".\data\albert-melbourne.course.html",
                    @"We meet at 7.50am every Saturday for the pre-run briefing at the Coot Picnic Area, Albert Park, in the BBQ shed directly across the road from the Melbourne Sports and Aquatic Centre (MSAC). We then walk 300m to the Palms Lawn for the start. The course follows the iconic Albert Park Lake path anticlockwise, finishing back at the Coot Picnic Area. The boat shed at the southern end of the lake is halfway. See the map above. There are public toilets at the Coot Picnic Area and at the 4.5km mark. Water fountains are positioned about every kilometre around the course as well as at the Coot Picnic Area.",
                    new []{"1NfqlpN73QMEq534Z94E4PUkTHHM"}
                },
                new object[]
                {
                    "www.parkrun.co.za",
                    @".\data\bettysbay.course.html",
                    @"Start in front of the Red Disa Restaurant,. The first part of the course is in the formal garden, then crosses the bridge into the indigenous forest for a double loop before returning via another loop in the formal garden to finish at the restaurant. Unfortunately dogs are not allowed at this event.",
                    new []{"1jpIzvsuiuNxCiSML3pL5HZqbsY0vlZD2"}
                },
                new object[]
                {
                    "www.parkrun.ie",
                    @".\data\bereisland.course.html",
                    @"The run starts and finishes at the local GAA pitch near Rerrin at the eastern end of the Island. It goes in an anti-clockwise direction and you head east first. The course starts with a hill for about 400m and then levels off, on the hill you pass by a wedge tomb. At the top of the hill the course levels off and you can take in some breath taking views of the Atlantic ocean and Lonehort Viking harbour. You take the next right turn and head up to a military fort that was built in 1898. The course turns left here and you are now heading west going downhill. Next you begin a climb up to an Irish army training camp and take a turn right and head downhill again to Rerrin village. In the village you head south and back to the finish line at the GAA pitch.",
                    new []{"125capdMTerR_zmWnJsNJIpL3E6E"}
                },
                new object[]
                {
                    "www.parkrun.sg",
                    @".\data\bishan.course.html",
                    @"Bishan parkrun is two laps run anti-clockwise around the perimeter of Bishan-Ang Mo Kio Park on tarmac footpaths. The meeting/briefing point for the run is 250m east of Car Park A and Aramsa - The Garden Spa. Starting on the main footpath close to Bishan-Ang Mo Kio Site Office, head west towards Marymont Rd, before heading back east parallel to Kallang River towards Bishan Rd. The course then loops back west towards the start line to complete the first loop. Then completing the second loop and finishing at the briefing point.",
                    new []{"1QXlymhYNyeQPJKF1hvrfueaVFlIddXxu"}
                },
                new object[]
                {
                    "www.parkrun.us",
                    @".\data\anacostia.course.html",
                    @"A simple out and back course along the asphalt Anacostia Riverwalk Trail. Begin directly across from the turn off from Anacostia Drive into the US Park Police Facility (no parking allowed there). Follow the Trail east then north east until it dead ends (do not take the fork to the right that crosses the road). Turn around and come back to the start. It is fast and flat throughout.",
                    new []{"1n964vWt2CErIFO1C70jdM6RxMoI"}
                },
                new object[]
                {
                    "www.parkrun.co.nz",
                    @".\data\taupo.course.html",
                    @"Out and back course starting at 2 Mile Bay Reserve heading south along path for 2.5 km, turning back at 4 Mile Bay and keeping to the left of path at all times. The course is flat with stunning views of Lake Taupo.",
                    new []{"16V8xYY15KLSd6StXGkC787AnHvs"}
                },
                new object[]
                {
                    "www.parkrun.se",
                    @".\data\orebro.course.html",
                    @"Denna envarvsbana startar bredvid oljehamnen och fortsätter över träbron, förbi värmestugan i Rynningeviken, längs med Rävgången och i vänstercirkel runt det vackra naturreservatet.",
                    new []{"18C_dVIuotfzufy5Ocs4j1Q7O80E"}
                },
                new object[]
                {
                    "www.parkrun.ca",
                    @".\data\beachstrip.course.html",
                    @"Out and Back route starting at the 4600m mark on the Hamilton Beach Trail, directly in front of The Lakeview building and basketball court. Participants will head east towards Stoney Creek until 100m beyond the 7200m mark on the trail (also the park boundary of Wild Waterworks). Participants will turn around and travel back to the 4800m mark on the trail for the finish.",
                    new []{"1189FRDrAhUfMc8_v0cwRw9-jNok"}
                },
                new object[]
                {
                    "www.parkrun.no",
                    @".\data\festningen.course.html",
                    @"The start/finish area lies in the northern end of the park at Festningen, by the junction of Festningsgata and Båhus Gate. Starting with a moderate climb, the course consists of 3 laps within the park. The course is run over a mixture of gravel paths and grass and ends with a nice descent back to the start/finish area.",
                    new []{"1iE3l7UN_ZKy_i63z9v471r4qHLCsvyV-"}
                },
                new object[]
                {
                    "www.parkrun.fi",
                    @".\data\tokoinranta.course.html",
                    @"Out & back, varying surface gravel and asphalt. The start is flat for 300 meters and leads to an uphill, on top of the hill you turn left onto the bridge crossing the railway, with a beautiful view of the city centre. After the bridge, you turn left and 50m later a descent leading you into the Töölönlahti area. Running along the Töölönlahti Bay you pass the Alvar Aalto Finlandia house in white marble. The turnaround point is past the Opera House. Turn around at the top of Töölönlahti, run back and you’re at the finish line.",
                    new []{"1Fy75t0S2uax80xMqIU2OvjptfiYmyAf7"}
                },
                new object[]
                {
                    "www.parkrun.my",
                    @".\data\tamanpuduulu.course.html",
                    @"Start and Finish near the large white sail structure which is visible anywhere in park. Course consists of 2 small laps and two large laps of the park",
                    new []{"1UzrOWsSWExpTLJ8IF4QCm7_cthQq8fuF"}
                },
                new object[]
                {
                    "www.parkrun.org.uk",
                    @".\data\hampsteadheath.course.html",
                    @"Hampstead Heath parkrun is a clockwise 2 laps course that will take you through some wonderful ancient woodlands, along the chain of ponds and up through Parliament Hill Fields. There are actually two courses that we use, and change depending on conditions - please check",
                    new []{"1AOcvRs7LjPK0JIHlRfNnpEQDPSw", "1Qf-YyM_zG9iBncp0Ba4x8SQq0_k" }
                }
            };
    }
}
