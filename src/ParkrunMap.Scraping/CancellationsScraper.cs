using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace ParkrunMap.Scraping;

public record CancellationScrapedResult(string ParkrunName, string? WebsiteUri, DateTime Date, string? Reason);

public class CancellationsScraper(IBrowserService browserService, ILogger<CancellationsScraper> logger)
{
    private const string CancellationsUrl = "https://www.parkrun.org.uk/cancellations/";

    public async Task<IReadOnlyList<CancellationScrapedResult>> ScrapeAsync(CancellationToken ct = default)
    {
        logger.LogInformation("Scraping cancellations from {Url}", CancellationsUrl);

        await using var context = await browserService.CreateContextAsync();
        var page = await context.NewPageAsync();
        try
        {
            await page.GotoAsync(CancellationsUrl, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 30_000
            });

            var results = new List<CancellationScrapedResult>();
            var rows = await page.QuerySelectorAllAsync("table tr, .cancellation-item");

            foreach (var row in rows)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    var cells = await row.QuerySelectorAllAsync("td");
                    if (cells.Count < 2) continue;

                    var nameEl = await cells[0].QuerySelectorAsync("a");
                    var name = nameEl is not null
                        ? (await nameEl.InnerTextAsync()).Trim()
                        : (await cells[0].InnerTextAsync()).Trim();
                    var uri = nameEl is not null ? await nameEl.GetAttributeAsync("href") : null;

                    var dateText = (await cells[1].InnerTextAsync()).Trim();
                    if (!DateTime.TryParse(dateText, out var date)) continue;

                    var reason = cells.Count > 2 ? (await cells[2].InnerTextAsync()).Trim() : null;

                    results.Add(new CancellationScrapedResult(name, uri, date, reason));
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to parse cancellation row, skipping");
                }
            }

            logger.LogInformation("Scraped {Count} cancellations", results.Count);
            return results;
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}
