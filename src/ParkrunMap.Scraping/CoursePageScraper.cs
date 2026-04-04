using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using ParkrunMap.Domain;

namespace ParkrunMap.Scraping;

public record CourseScrapedResult(
    string Domain,
    string Path,
    string? Description,
    IReadOnlyList<string> GoogleMapIds,
    IReadOnlyList<string> Terrain,
    Statistics Statistics);

public class CoursePageScraper(IBrowserService browserService, ILogger<CoursePageScraper> logger)
{
    public async IAsyncEnumerable<CourseScrapedResult> ScrapeAsync(
        IReadOnlyList<(string Domain, string Path)> targets,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        if (targets.Count == 0) yield break;

        await using var context = await browserService.CreateContextAsync();

        foreach (var (domain, path) in targets)
        {
            ct.ThrowIfCancellationRequested();
            var url = $"https://{domain}{path}course/";

            CourseScrapedResult result;
            try
            {
                result = await ScrapeCoursePageAsync(context, domain, path, url, ct);
                logger.LogDebug("Scraped course page for {Domain}{Path}", domain, path);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to scrape course page for {Domain}{Path}, skipping", domain, path);
                continue;
            }

            yield return result;
        }
    }

    private static async Task<CourseScrapedResult> ScrapeCoursePageAsync(
        IBrowserContext context, string domain, string path, string url, CancellationToken ct)
    {
        var page = await context.NewPageAsync();
        try
        {
            await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 30_000 });

            var description = await ExtractDescriptionAsync(page);
            var googleMapIds = await ExtractGoogleMapIdsAsync(page);
            var terrain = await ExtractTerrainAsync(page);
            var statistics = await ExtractStatisticsAsync(page);

            return new CourseScrapedResult(domain, path, description, googleMapIds, terrain, statistics);
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    private static async Task<string?> ExtractDescriptionAsync(IPage page)
    {
        // Real parkrun sites (org.uk and all country domains) use p.paddedb for the course description.
        // WireMock stubs may use .course-description; main p is a broad fallback.
        var el = await page.QuerySelectorAsync("p.paddedb, .course-description, .EventDetail-description, main p");
        return el is not null ? (await el.InnerTextAsync()).Trim() : null;
    }

    private static async Task<IReadOnlyList<string>> ExtractGoogleMapIdsAsync(IPage page)
    {
        var iframes = await page.QuerySelectorAllAsync("iframe[src*='google.com/maps']");
        var ids = new List<string>();

        foreach (var iframe in iframes)
        {
            var src = await iframe.GetAttributeAsync("src");
            if (src is null) continue;

            var match = System.Text.RegularExpressions.Regex.Match(src, @"mid=([^&]+)");
            if (match.Success)
                ids.Add(match.Groups[1].Value);
        }

        return ids;
    }

    private static async Task<IReadOnlyList<string>> ExtractTerrainAsync(IPage page)
    {
        var terrainEls = await page.QuerySelectorAllAsync(".terrain-type, [data-terrain]");
        var terrain = new List<string>();
        foreach (var el in terrainEls)
        {
            var text = (await el.InnerTextAsync()).Trim();
            if (!string.IsNullOrWhiteSpace(text))
                terrain.Add(text);
        }
        return terrain;
    }

    private static async Task<Statistics> ExtractStatisticsAsync(IPage page)
    {
        int? totalEvents = null, totalRunners = null, totalRuns = null, totalVolunteers = null;
        double? averageRunnersPerWeek = null;

        var statElements = await page.QuerySelectorAllAsync(".Statistics-title, .Statistics-fact");
        for (var i = 0; i < statElements.Count - 1; i += 2)
        {
            var label = (await statElements[i].InnerTextAsync()).Trim().ToLowerInvariant();
            var valueText = (await statElements[i + 1].InnerTextAsync()).Trim().Replace(",", "");

            if (int.TryParse(valueText, out var intVal))
            {
                if (label.Contains("event")) totalEvents = intVal;
                else if (label.Contains("runner")) totalRunners = intVal;
                else if (label.Contains("run")) totalRuns = intVal;
                else if (label.Contains("volunteer")) totalVolunteers = intVal;
            }
            else if (double.TryParse(valueText, out var dblVal))
            {
                if (label.Contains("average")) averageRunnersPerWeek = dblVal;
            }
        }

        return new Statistics
        {
            TotalEvents = totalEvents,
            TotalRunners = totalRunners,
            TotalRuns = totalRuns,
            TotalVolunteers = totalVolunteers,
            AverageRunnersPerWeek = averageRunnersPerWeek
        };
    }
}