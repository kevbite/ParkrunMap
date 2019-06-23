using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ParkrunMap.Scraping.Stats
{
    public class StatsParser
    {
        public Task<ParkrunStats> Parse(FileStream stream, string domain)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(stream);

            int totalEvents = ParseTotalEvents(htmlDoc, domain);
            int totalRunners = ParseTotalRunners(htmlDoc, domain);
            int totalRuns = ParseTotalRuns(htmlDoc, domain);
            double averageRunnersPerWeek = ParseAverageRunnersPerWeek(htmlDoc, domain);
            TimeSpan averageRunTime = ParseAverageRunTime(htmlDoc, domain);
            TimeSpan totalRunTime = ParseTotalRunTime(htmlDoc, domain);
            int biggestAttendance = ParseBiggestAttendance(htmlDoc, domain);
            int totalKmDistanceRan = ParseTotalKmDistanceRan(htmlDoc, domain);

            return Task.FromResult(new ParkrunStats(totalEvents,
             totalRunners,
             totalRuns,
             averageRunnersPerWeek,
             averageRunTime,
             totalRunTime,
             biggestAttendance,
             totalKmDistanceRan));
        }

        private int ParseTotalKmDistanceRan(HtmlDocument htmlDoc, string domain)
        {
            var text = "Total distance run";
            var nodeValue = ParseNodeText(htmlDoc, text);
            var distance = nodeValue.Replace("km", string.Empty);
            return int.Parse(distance, NumberStyles.AllowThousands);
        }

        private int ParseBiggestAttendance(HtmlDocument htmlDoc, string domain)
        {
            var text = "Biggest Attendance";
            var nodeValue = ParseNodeText(htmlDoc, text);

            return int.Parse(nodeValue);
        }

        private TimeSpan ParseTotalRunTime(HtmlDocument htmlDoc, string domain)
        {
            var text = "Total hours run";
            var nodeValue = ParseNodeText(htmlDoc, text);

            // 0Years 278Days 7Hrs 28Min 41Secs
            var match = Regex.Match(nodeValue,
                @"(?<years>\d+)Years (?<days>\d+)Days (?<hours>\d+)Hrs (?<minutes>\d+)Min (?<seconds>\d+)Secs");

            return new TimeSpan(
                int.Parse(match.Groups["days"].Value),
                int.Parse(match.Groups["hours"].Value),
                int.Parse(match.Groups["minutes"].Value),
                int.Parse(match.Groups["seconds"].Value),
                0);
        }

        private TimeSpan ParseAverageRunTime(HtmlDocument htmlDoc, string domain)
        {
            var text = "Average run time";
            var nodeValue = ParseNodeText(htmlDoc, text);

            return TimeSpan.Parse(nodeValue);
        }

        private double ParseAverageRunnersPerWeek(HtmlDocument htmlDoc, string domain)
        {
            var text = "Average number of runners per week";
            var nodeValue = ParseNodeText(htmlDoc, text);

            return double.Parse(nodeValue);
        }

        private int ParseTotalRuns(HtmlDocument htmlDoc, string domain)
        {
            var text = "Number of runs";
            var nodeValue = ParseNodeText(htmlDoc, text);

            return int.Parse(nodeValue, NumberStyles.AllowThousands);
        }

        private int ParseTotalRunners(HtmlDocument htmlDoc, string domain)
        {
            var text = "Number of runners";
            var nodeValue = ParseNodeText(htmlDoc, text);

            return int.Parse(nodeValue, NumberStyles.AllowThousands);
        }

        private int ParseTotalEvents(HtmlDocument htmlDoc, string domain)
        {
            var text = "Number of events";
            var nodeValue = ParseNodeText(htmlDoc, text);

            return int.Parse(nodeValue);
        }

        private static string ParseNodeText(HtmlDocument htmlDoc, string text)
        {
            var node = htmlDoc.DocumentNode.SelectSingleNode($"//*[contains(text(),'{text}')]");

            var nodeValue = node.InnerText.Replace(text, string.Empty)
                .Trim(new[] { ':', ' ' });
            return nodeValue;
        }
    }
}
