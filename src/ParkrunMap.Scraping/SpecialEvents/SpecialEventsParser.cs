using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using ParkrunMap.Scraping.Cancellations;

namespace ParkrunMap.Scraping.SpecialEvents;

public class SpecialEventsParser
{
    public IReadOnlyCollection<SpecialEvent> Parse(Stream stream)
    {
        var specialEvent = new List<SpecialEvent>();
        var htmlDoc = new HtmlDocument();
        htmlDoc.Load(stream);

        var contentHtml = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='content']");
        var h1 = contentHtml.SelectSingleNode(".//h1");
        var yearsMatch = Regex.Match(h1.InnerText, @"(\d{2,4})[\D]+(\d{2,4})");
        var christmasDayYear = yearsMatch.Groups[1].Value;
        var newYearsDayYear = christmasDayYear.Substring(0, 2) + (
            yearsMatch.Groups[2].Value.Length == 4
                ? yearsMatch.Groups[2].Value.Substring(2)
                : yearsMatch.Groups[2].Value);
        
        var rows = contentHtml.SelectNodes(".//table[@id='results']/tr");

        foreach (var row in rows.Skip(1))
        {
            var (websiteDomain, websitePath) = ParsePathAndDomain(row);

            if (websitePath.Contains("-juniors"))
            {
                // We're ignoring junior parkruns
                continue;
            }
            
            var tableData = row.SelectNodes("td");
            
            var christmasDay = tableData[2].InnerText.Contains("✅");
            var newYearsDay = tableData[3].InnerText.Contains("✅");
            
            specialEvent.Add(new SpecialEvent(websiteDomain, websitePath, int.Parse(christmasDayYear), SpecialEventType.ChristmasDay, christmasDay));
            specialEvent.Add(new SpecialEvent(websiteDomain, websitePath, int.Parse(newYearsDayYear), SpecialEventType.NewYearsDay, newYearsDay));
        }

        return specialEvent;
    }

    private static (string websiteDomain, string websitePath) ParsePathAndDomain(HtmlNode row)
    {
        var link = row.SelectSingleNode(".//a");
        var parkrunUri = link.Attributes["href"].Value;

        var uri = new Uri(parkrunUri);
        var websiteDomain = uri.Host;
        var websitePath = uri.PathAndQuery.TrimEnd('/');
        return (websiteDomain, websitePath);
    }
}

public class SpecialEvent
{
    public SpecialEvent(string websiteDomain, string websitePath, int year, SpecialEventType type, bool isRunning)
    {
        WebsiteDomain = websiteDomain;
        WebsitePath = websitePath;
        Year = year;
        Type = type;
        IsRunning = isRunning;
    }

    public string WebsiteDomain { get; }

    public string WebsitePath { get; }

    public int Year { get; }

    public SpecialEventType Type { get; }

    public bool IsRunning { get; }
}

public enum SpecialEventType
{
    ChristmasDay,
    NewYearsDay
}