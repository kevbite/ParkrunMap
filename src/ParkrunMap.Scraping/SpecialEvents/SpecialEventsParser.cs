using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        var rows = htmlDoc.DocumentNode.SelectNodes("//div[@id='content']//table[@id='results']/tr");

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
            
            specialEvent.Add(new SpecialEvent(websiteDomain, websitePath, 2022, SpecialEventType.ChristmasDay, christmasDay));
            specialEvent.Add(new SpecialEvent(websiteDomain, websitePath, 2023, SpecialEventType.NewYearsDay, newYearsDay));
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