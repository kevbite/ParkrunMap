using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace ParkrunMap.Scraping;

public record SpecialEventResult(string Domain, string Path, int Year, bool ChristmasDay, bool NewYearsDay);

public class SpecialEventsScraper(IBrowserService browserService, ILogger<SpecialEventsScraper> logger)
{
    public async Task<IReadOnlyList<SpecialEventResult>> ScrapeAsync(
        IReadOnlyList<(string CountryBaseUrl, string CountryCode)> targets,
        CancellationToken ct = default)
    {
        if (targets.Count == 0) return [];

        await using var context = await browserService.CreateContextAsync();
        var results = new List<SpecialEventResult>();

        foreach (var (countryBaseUrl, countryCode) in targets)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                var countryResults = await ScrapeCountryAsync(context, countryBaseUrl, countryCode, ct);
                results.AddRange(countryResults);
                logger.LogDebug("Scraped special events for {CountryCode}: {Count} results", countryCode, countryResults.Count);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to scrape special events for {CountryCode}", countryCode);
            }
        }

        return results;
    }

    private static async Task<List<SpecialEventResult>> ScrapeCountryAsync(
        IBrowserContext context, string baseUrl, string countryCode, CancellationToken ct)
    {
        var url = $"{baseUrl.TrimEnd('/')}/special-events/";
        var page = await context.NewPageAsync();
        try
        {
            await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30_000
            });

            var results = new List<SpecialEventResult>();
            var rows = await page.QuerySelectorAllAsync("table tbody tr");

            foreach (var row in rows)
            {
                ct.ThrowIfCancellationRequested();
                var cells = await row.QuerySelectorAllAsync("td");
                if (cells.Count < 3) continue;

                var linkEl = await cells[0].QuerySelectorAsync("a");
                if (linkEl is null) continue;

                var href = await linkEl.GetAttributeAsync("href") ?? string.Empty;
                var domain = ExtractDomain(href, baseUrl);
                var path = ExtractPath(href);

                if (string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(path)) continue;

                var yearText = (await cells[1].InnerTextAsync()).Trim();
                if (!int.TryParse(yearText, out var year)) continue;

                var christmasText = (await cells[2].InnerTextAsync()).Trim().ToLowerInvariant();
                var newYearsText = cells.Count > 3 ? (await cells[3].InnerTextAsync()).Trim().ToLowerInvariant() : string.Empty;

                results.Add(new SpecialEventResult(
                    domain, path, year,
                    christmasText is "yes" or "✓" or "running",
                    newYearsText is "yes" or "✓" or "running"));
            }

            return results;
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    private static string ExtractDomain(string href, string fallbackBaseUrl)
    {
        if (Uri.TryCreate(href, UriKind.Absolute, out var uri))
            return uri.Host;
        if (Uri.TryCreate(fallbackBaseUrl, UriKind.Absolute, out var baseUri))
            return baseUri.Host;
        return string.Empty;
    }

    private static string ExtractPath(string href)
    {
        if (Uri.TryCreate(href, UriKind.Absolute, out var uri))
            return uri.AbsolutePath.TrimEnd('/') + "/";
        return href.StartsWith('/') ? href : "/" + href;
    }
}
