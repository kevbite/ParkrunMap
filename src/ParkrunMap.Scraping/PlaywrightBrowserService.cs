using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace ParkrunMap.Scraping;

public class PlaywrightBrowserService(ILogger<PlaywrightBrowserService> logger) : IBrowserService
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    public async Task<IBrowserContext> CreateContextAsync()
    {
        await EnsureInitializedAsync();
        return await _browser!.NewContextAsync(new BrowserNewContextOptions
        {
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            ExtraHTTPHeaders = new Dictionary<string, string>
            {
                ["Accept-Language"] = "en-GB,en;q=0.9"
            }
        });
    }

    private async Task EnsureInitializedAsync()
    {
        if (_browser is not null) return;

        await _initLock.WaitAsync();
        try
        {
            if (_browser is not null) return;

            logger.LogInformation("Initializing Playwright browser...");
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true,
                Args = ["--no-sandbox", "--disable-setuid-sandbox", "--disable-dev-shm-usage"]
            });
            logger.LogInformation("Playwright browser initialized.");
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.DisposeAsync();
            _browser = null;
        }
        _playwright?.Dispose();
        _playwright = null;
    }
}
