using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ParkrunMap.Scraping.Statistics
{
    
    public class StatisticsParser
    {
        private enum Text
        {
            TotalDistanceRun,
            BiggestAttendance,
            TotalHoursRun,
            AverageRunTime,
            AverageRunnersPerWeek,
            NumberOfRuns,
            NumberOfRunners,
            NumberOfEvents
        }

        private static readonly IReadOnlyDictionary<(string, Text), string> DomainToTextMap =
            new Dictionary<(string, Text), string>()
            {
                {("www.parkrun.org.uk", Text.TotalDistanceRun), "Total distance run"},
                {("www.parkrun.org.uk", Text.BiggestAttendance), "Biggest Attendance"},
                {("www.parkrun.org.uk", Text.TotalHoursRun), "Total hours run"},
                {("www.parkrun.org.uk", Text.AverageRunTime), "Average run time"},
                {("www.parkrun.org.uk", Text.AverageRunnersPerWeek), "Average number of runners per week"},
                {("www.parkrun.org.uk", Text.NumberOfRuns), "Number of runs"},
                {("www.parkrun.org.uk", Text.NumberOfRunners), "Number of runners"},
                {("www.parkrun.org.uk", Text.NumberOfEvents), "Number of events"},

                {("www.parkrun.ie", Text.TotalDistanceRun), "Total distance run"},
                {("www.parkrun.ie", Text.BiggestAttendance), "Biggest Attendance"},
                {("www.parkrun.ie", Text.TotalHoursRun), "Total hours run"},
                {("www.parkrun.ie", Text.AverageRunTime), "Average run time"},
                {("www.parkrun.ie", Text.AverageRunnersPerWeek), "Average number of runners per week"},
                {("www.parkrun.ie", Text.NumberOfRuns), "Number of runs"},
                {("www.parkrun.ie", Text.NumberOfRunners), "Number of runners"},
                {("www.parkrun.ie", Text.NumberOfEvents), "Number of events"},
                
                {("www.parkrun.co.za", Text.TotalDistanceRun), "Total distance run"},
                {("www.parkrun.co.za", Text.BiggestAttendance), "Biggest Attendance"},
                {("www.parkrun.co.za", Text.TotalHoursRun), "Total hours run"},
                {("www.parkrun.co.za", Text.AverageRunTime), "Average run time"},
                {("www.parkrun.co.za", Text.AverageRunnersPerWeek), "Average number of runners per week"},
                {("www.parkrun.co.za", Text.NumberOfRuns), "Number of runs"},
                {("www.parkrun.co.za", Text.NumberOfRunners), "Number of runners"},
                {("www.parkrun.co.za", Text.NumberOfEvents), "Number of events"},

                {("www.parkrun.com.au", Text.TotalDistanceRun), "Total distance run"},
                {("www.parkrun.com.au", Text.BiggestAttendance), "Biggest Attendance"},
                {("www.parkrun.com.au", Text.TotalHoursRun), "Total hours run"},
                {("www.parkrun.com.au", Text.AverageRunTime), "Average run time"},
                {("www.parkrun.com.au", Text.AverageRunnersPerWeek), "Average number of runners per week"},
                {("www.parkrun.com.au", Text.NumberOfRuns), "Number of runs"},
                {("www.parkrun.com.au", Text.NumberOfRunners), "Number of runners"},
                {("www.parkrun.com.au", Text.NumberOfEvents), "Number of events"},

                {("www.parkrun.sg", Text.TotalDistanceRun), "Total distance run"},
                {("www.parkrun.sg", Text.BiggestAttendance), "Biggest Attendance"},
                {("www.parkrun.sg", Text.TotalHoursRun), "Total hours run"},
                {("www.parkrun.sg", Text.AverageRunTime), "Average run time"},
                {("www.parkrun.sg", Text.AverageRunnersPerWeek), "Average number of runners per week"},
                {("www.parkrun.sg", Text.NumberOfRuns), "Number of runs"},
                {("www.parkrun.sg", Text.NumberOfRunners), "Number of runners"},
                {("www.parkrun.sg", Text.NumberOfEvents), "Number of events"},

                {("www.parkrun.us", Text.TotalDistanceRun), "Total distance run"},
                {("www.parkrun.us", Text.BiggestAttendance), "Biggest Attendance"},
                {("www.parkrun.us", Text.TotalHoursRun), "Total hours run"},
                {("www.parkrun.us", Text.AverageRunTime), "Average run time"},
                {("www.parkrun.us", Text.AverageRunnersPerWeek), "Average number of runners per week"},
                {("www.parkrun.us", Text.NumberOfRuns), "Number of runs"},
                {("www.parkrun.us", Text.NumberOfRunners), "Number of runners"},
                {("www.parkrun.us", Text.NumberOfEvents), "Number of events"},

                {("www.parkrun.co.nz", Text.TotalDistanceRun), "Total distance run"},
                {("www.parkrun.co.nz", Text.BiggestAttendance), "Biggest Attendance"},
                {("www.parkrun.co.nz", Text.TotalHoursRun), "Total hours run"},
                {("www.parkrun.co.nz", Text.AverageRunTime), "Average run time"},
                {("www.parkrun.co.nz", Text.AverageRunnersPerWeek), "Average number of runners per week"},
                {("www.parkrun.co.nz", Text.NumberOfRuns), "Number of runs"},
                {("www.parkrun.co.nz", Text.NumberOfRunners), "Number of runners"},
                {("www.parkrun.co.nz", Text.NumberOfEvents), "Number of events"},

                {("www.parkrun.ca", Text.TotalDistanceRun), "Total distance run"},
                {("www.parkrun.ca", Text.BiggestAttendance), "Biggest Attendance"},
                {("www.parkrun.ca", Text.TotalHoursRun), "Total hours run"},
                {("www.parkrun.ca", Text.AverageRunTime), "Average run time"},
                {("www.parkrun.ca", Text.AverageRunnersPerWeek), "Average number of runners per week"},
                {("www.parkrun.ca", Text.NumberOfRuns), "Number of runs"},
                {("www.parkrun.ca", Text.NumberOfRunners), "Number of runners"},
                {("www.parkrun.ca", Text.NumberOfEvents), "Number of events"},

                {("www.parkrun.no", Text.TotalDistanceRun), "Total distance run"},
                {("www.parkrun.no", Text.BiggestAttendance), "Biggest Attendance"},
                {("www.parkrun.no", Text.TotalHoursRun), "Total hours run"},
                {("www.parkrun.no", Text.AverageRunTime), "Average run time"},
                {("www.parkrun.no", Text.AverageRunnersPerWeek), "Average number of runners per week"},
                {("www.parkrun.no", Text.NumberOfRuns), "Number of runs"},
                {("www.parkrun.no", Text.NumberOfRunners), "Number of runners"},
                {("www.parkrun.no", Text.NumberOfEvents), "Number of events"},

                {("www.parkrun.fi", Text.TotalDistanceRun), "Total distance run"},
                {("www.parkrun.fi", Text.BiggestAttendance), "Biggest Attendance"},
                {("www.parkrun.fi", Text.TotalHoursRun), "Total hours run"},
                {("www.parkrun.fi", Text.AverageRunTime), "Average run time"},
                {("www.parkrun.fi", Text.AverageRunnersPerWeek), "Average number of runners per week"},
                {("www.parkrun.fi", Text.NumberOfRuns), "Number of runs"},
                {("www.parkrun.fi", Text.NumberOfRunners), "Number of runners"},
                {("www.parkrun.fi", Text.NumberOfEvents), "Number of events"},


                {("www.parkrun.my", Text.TotalDistanceRun), "Total distance run"},
                {("www.parkrun.my", Text.BiggestAttendance), "Biggest Attendance"},
                {("www.parkrun.my", Text.TotalHoursRun), "Total hours run"},
                {("www.parkrun.my", Text.AverageRunTime), "Average run time"},
                {("www.parkrun.my", Text.AverageRunnersPerWeek), "Average number of runners per week"},
                {("www.parkrun.my", Text.NumberOfRuns), "Number of runs"},
                {("www.parkrun.my", Text.NumberOfRunners), "Number of runners"},
                {("www.parkrun.my", Text.NumberOfEvents), "Number of events"},

                {("www.parkrun.pl", Text.TotalDistanceRun), "Przebiegnięty dystans"},
                {("www.parkrun.pl", Text.BiggestAttendance), "Największa frekwencja"},
                {("www.parkrun.pl", Text.TotalHoursRun), "Przebiegnięty czas"},
                {("www.parkrun.pl", Text.AverageRunTime), "Średni uzyskany czas"},
                {("www.parkrun.pl", Text.AverageRunnersPerWeek), "Średnia liczba uczestników na tydzień"},
                {("www.parkrun.pl", Text.NumberOfRuns), "Liczba biegów"},
                {("www.parkrun.pl", Text.NumberOfRunners), "Liczba uczestników"},
                {("www.parkrun.pl", Text.NumberOfEvents), "Liczba edycji biegu"},

                {("www.parkrun.dk", Text.TotalDistanceRun), "Total tilbagelagt distance"},
                {("www.parkrun.dk", Text.BiggestAttendance), "Største deltagertal"},
                {("www.parkrun.dk", Text.TotalHoursRun), "Antal timer løbet"},
                {("www.parkrun.dk", Text.AverageRunTime), "Gns. løbetid"},
                {("www.parkrun.dk", Text.AverageRunnersPerWeek), "Gns. antal løbere pr. uge"},
                {("www.parkrun.dk", Text.NumberOfRuns), "Antal løb"},
                {("www.parkrun.dk", Text.NumberOfRunners), "Antal løbere"},
                {("www.parkrun.dk", Text.NumberOfEvents), "Antal events"},

                {("www.parkrun.ru", Text.TotalDistanceRun), "Суммарная дистанция"},
                {("www.parkrun.ru", Text.BiggestAttendance), "Наивысшая посещаемость"},
                {("www.parkrun.ru", Text.TotalHoursRun), "Суммарное время"},
                {("www.parkrun.ru", Text.AverageRunTime), "Среднее время на забеге"},
                {("www.parkrun.ru", Text.AverageRunnersPerWeek), "Среднее число бегунов в неделю"},
                {("www.parkrun.ru", Text.NumberOfRuns), "Совершено пробежек"},
                {("www.parkrun.ru", Text.NumberOfRunners), "Участвовало бегунов"},
                {("www.parkrun.ru", Text.NumberOfEvents), "Проведено забегов"},

                {("www.parkrun.com.de", Text.TotalDistanceRun), "Gesamte gelaufene Strecke"},
                {("www.parkrun.com.de", Text.BiggestAttendance), "Meiste Teilnehmer"},
                {("www.parkrun.com.de", Text.TotalHoursRun), "Gesamte gelaufene Zeit"},
                {("www.parkrun.com.de", Text.AverageRunTime), "Durchschnittliche Laufzeit"},
                {("www.parkrun.com.de", Text.AverageRunnersPerWeek), "Durchschnittliche Anzahl an Läufern pro Woche"},
                {("www.parkrun.com.de", Text.NumberOfRuns), "Anzahl an Läufen"},
                {("www.parkrun.com.de", Text.NumberOfRunners), "Anzahl an LäuferInnen"},
                {("www.parkrun.com.de", Text.NumberOfEvents), "Gesamtanzahl ausgetragener Läufe"},

                {("www.parkrun.fr", Text.TotalDistanceRun), "Distance totale parcourue"},
                {("www.parkrun.fr", Text.BiggestAttendance), "Record de fréquentation"},
                {("www.parkrun.fr", Text.TotalHoursRun), "Nombre d'heures de footing"},
                {("www.parkrun.fr", Text.AverageRunTime), "Temps moyen réalisé"},
                {("www.parkrun.fr", Text.AverageRunnersPerWeek), "Nombre moyen de coureurs par semaine"},
                {("www.parkrun.fr", Text.NumberOfRuns), "Nombre de footings réalisés"},
                {("www.parkrun.fr", Text.NumberOfRunners), "Nombre de coureurs"},
                {("www.parkrun.fr", Text.NumberOfEvents), "Nombre de parkruns"},

                {("www.parkrun.it", Text.TotalDistanceRun), "Distanza totale percorsa"},
                {("www.parkrun.it", Text.BiggestAttendance), "Partecipazione maggiore"},
                {("www.parkrun.it", Text.TotalHoursRun), "Totale delle ore corse"},
                {("www.parkrun.it", Text.AverageRunTime), "Tempo medio di corsa"},
                {("www.parkrun.it", Text.AverageRunnersPerWeek), "Numero medio di corridori a settimana"},
                {("www.parkrun.it", Text.NumberOfRuns), "Numero di corse"},
                {("www.parkrun.it", Text.NumberOfRunners), "Numero di corridori"},
                {("www.parkrun.it", Text.NumberOfEvents), "Numero di eventi"},

                {("www.parkrun.se", Text.TotalDistanceRun), "Totalt tillryggalagd sträcka"},
                {("www.parkrun.se", Text.BiggestAttendance), "Flest deltagare"},
                {("www.parkrun.se", Text.TotalHoursRun), "Totalt antal timmar"},
                {("www.parkrun.se", Text.AverageRunTime), "Genomsnittlig sluttid"},
                {("www.parkrun.se", Text.AverageRunnersPerWeek), "Genomsnittligt antal löpare per vecka"},
                {("www.parkrun.se", Text.NumberOfRuns), "Antal parkrunstarter"},
                {("www.parkrun.se", Text.NumberOfRunners), "Antal löpare"},
                {("www.parkrun.se", Text.NumberOfEvents), "Antal event"},

                {("www.parkrun.jp", Text.TotalDistanceRun), "参加者の累計距離数"},
                {("www.parkrun.jp", Text.BiggestAttendance), "最大参加者数"},
                {("www.parkrun.jp", Text.TotalHoursRun), "参加者の累計時間数"},
                {("www.parkrun.jp", Text.AverageRunTime), "平均タイム"},
                {("www.parkrun.jp", Text.AverageRunnersPerWeek), "週平均の参加者数"},
                {("www.parkrun.jp", Text.NumberOfRuns), "参加回数累計"},
                {("www.parkrun.jp", Text.NumberOfRunners), "参加者数"},
                {("www.parkrun.jp", Text.NumberOfEvents), "イベント数"},
            };

        private static readonly IReadOnlyDictionary<string, CultureInfo> DomainCultureInfoMap =
            new Dictionary<string, CultureInfo>
            {
                {"www.parkrun.org.uk", CultureInfo.GetCultureInfo("en-GB")},
                {"www.parkrun.ie", CultureInfo.GetCultureInfo("en-IE")},
                {"www.parkrun.co.za", CultureInfo.GetCultureInfo("za")},
                {"www.parkrun.com.au", CultureInfo.GetCultureInfo("en-AU")},
                {"www.parkrun.sg", CultureInfo.GetCultureInfo("en-GB")},
                {"www.parkrun.us", CultureInfo.GetCultureInfo("en-US")},
                {"www.parkrun.co.nz", CultureInfo.GetCultureInfo("en-NZ")},
                {"www.parkrun.ca", CultureInfo.GetCultureInfo("en-CA")},
                {"www.parkrun.no", CultureInfo.GetCultureInfo("en-GB")},
                {"www.parkrun.fi", CultureInfo.GetCultureInfo("en-GB")},
                {"www.parkrun.my", CultureInfo.GetCultureInfo("en-MY")},
                {"www.parkrun.pl", CultureInfo.GetCultureInfo("pl")},
                {"www.parkrun.dk", CultureInfo.GetCultureInfo("da-DK")},
                {"www.parkrun.ru", CultureInfo.GetCultureInfo("de")},
                {"www.parkrun.com.de", CultureInfo.GetCultureInfo("de")},
                {"www.parkrun.fr", CultureInfo.GetCultureInfo("fr")},
                {"www.parkrun.it", CultureInfo.GetCultureInfo("it")},
                {"www.parkrun.se", CultureInfo.GetCultureInfo("se")},
                {"www.parkrun.jp", CultureInfo.GetCultureInfo("jp")},
            };

        public Task<ParkrunStatistics> Parse(Stream stream, string domain)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(stream);

            var totalEvents = ParseTotalEvents(htmlDoc, domain);
            var totalRunners = ParseTotalRunners(htmlDoc, domain);
            var totalRuns = ParseTotalRuns(htmlDoc, domain);
            var averageRunnersPerWeek = ParseAverageRunnersPerWeek(htmlDoc, domain);
            var averageSecondsRan = ParseAverageSecondsRan(htmlDoc, domain);
            var totalSecondsRan = ParseTotalSecondsRan(htmlDoc, domain);
            var biggestAttendance = ParseBiggestAttendance(htmlDoc, domain);
            var totalKmDistanceRan = ParseTotalKmDistanceRan(htmlDoc, domain);

            return Task.FromResult(new ParkrunStatistics(totalEvents,
             totalRunners,
             totalRuns,
             averageRunnersPerWeek,
             averageSecondsRan,
             totalSecondsRan,
             biggestAttendance,
             totalKmDistanceRan));
        }

        private int ParseTotalKmDistanceRan(HtmlDocument htmlDoc, string domain)
        {
            var text = DomainToTextMap[(domain, Text.TotalDistanceRun)];
            var nodeValue = ParseNodeText(htmlDoc, text);
            var distance = nodeValue.Replace("km", string.Empty)
                .Replace("км", string.Empty)
                ;


            if (string.IsNullOrEmpty(distance))
            {
                return 0;
            }

            return int.Parse(distance, NumberStyles.AllowThousands, DomainCultureInfoMap[domain]);
        }

        private int ParseBiggestAttendance(HtmlDocument htmlDoc, string domain)
        {
            var text = DomainToTextMap[(domain, Text.BiggestAttendance)];
            var nodeValue = ParseNodeText(htmlDoc, text);

            if (string.IsNullOrEmpty(nodeValue))
            {
                return 0;
            }

            return int.Parse(nodeValue, DomainCultureInfoMap[domain]);
        }

        private long ParseTotalSecondsRan(HtmlDocument htmlDoc, string domain)
        {
            var text = DomainToTextMap[(domain, Text.TotalHoursRun)];
            var nodeValue = ParseNodeText(htmlDoc, text);

            // 0Years 278Days 7Hrs 28Min 41Secs
            // 0lat(a) 219dni 15godzin 54min. 58sek.

            var numbers = Regex.Replace(nodeValue, @"[^0-9 ]", "");
            var values = numbers.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if(values.Length == 0)
            {
                return 0;
            }

            var years = TimeSpan.FromDays(int.Parse(values[0]) * 365);
            var days = TimeSpan.FromDays(int.Parse(values[1]));
            var hours = TimeSpan.FromHours(int.Parse(values[2]));
            var minutes = TimeSpan.FromMinutes(int.Parse(values[3]));
            var seconds = int.Parse(values[4]);

            return (long)years.TotalSeconds + (long)days.TotalSeconds + (long)hours.TotalSeconds + (long)minutes.TotalSeconds + seconds;
        }

        private int ParseAverageSecondsRan(HtmlDocument htmlDoc, string domain)
        {
            var text = DomainToTextMap[(domain, Text.AverageRunTime)];
            var nodeValue = ParseNodeText(htmlDoc, text);
            if (string.IsNullOrEmpty(nodeValue))
            {
                return 0;
            }

            return (int)TimeSpan.Parse(nodeValue).TotalSeconds;
        }

        private double ParseAverageRunnersPerWeek(HtmlDocument htmlDoc, string domain)
        {
            var text = DomainToTextMap[(domain, Text.AverageRunnersPerWeek)];
            var nodeValue = ParseNodeText(htmlDoc, text);

            return double.Parse(nodeValue, DomainCultureInfoMap[domain]);
        }

        private int ParseTotalRuns(HtmlDocument htmlDoc, string domain)
        {
            var text = DomainToTextMap[(domain, Text.NumberOfRuns)];
            var nodeValue = ParseNodeText(htmlDoc, text);

            return int.Parse(nodeValue, NumberStyles.AllowThousands, DomainCultureInfoMap[domain]);
        }

        private int ParseTotalRunners(HtmlDocument htmlDoc, string domain)
        {
            var text = DomainToTextMap[(domain, Text.NumberOfRunners)];
            var nodeValue = ParseNodeText(htmlDoc, text);

            return int.Parse(nodeValue, NumberStyles.AllowThousands, DomainCultureInfoMap[domain]);
        }

        private int ParseTotalEvents(HtmlDocument htmlDoc, string domain)
        {
            var text = DomainToTextMap[(domain, Text.NumberOfEvents)];
            var nodeValue = ParseNodeText(htmlDoc, text);

            return int.Parse(nodeValue, DomainCultureInfoMap[domain]);
        }

        private static string ParseNodeText(HtmlDocument htmlDoc, string text)
        {
            var node = htmlDoc.DocumentNode.SelectSingleNode($"//*[contains(text(),\"{text}:\")]");

            var nodeValue = node.InnerText.Replace(text, string.Empty)
                .Trim(new[] {'\r', '\n', '\t', ':', ' ' });

            return nodeValue;
        }
    }
}
