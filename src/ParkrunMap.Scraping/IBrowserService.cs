using Microsoft.Playwright;

namespace ParkrunMap.Scraping;

public interface IBrowserService : IAsyncDisposable
{
    Task<IBrowserContext> CreateContextAsync();
}
