using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace ParkrunMap.Scraping;

public static class ScrapingExtensions
{
    public static IServiceCollection AddParkrunScraping(this IServiceCollection services)
    {
        services.AddSingleton<IBrowserService, PlaywrightBrowserService>();
        services.AddSingleton<CoursePageScraper>();
        services.AddSingleton<CancellationsScraper>();
        services.AddSingleton<SpecialEventsScraper>();
        services.AddSingleton<EventsJsonScraper>();
        services.AddSingleton<QuestionnaireScraper>();

        services.AddHttpClient<EventsJsonScraper>()
            .ConfigureHttpClient((sp, client) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var baseUrl = config["Scraping:EventsJsonBaseUrl"] ?? "https://images.parkrun.com";
                client.BaseAddress = new Uri(baseUrl);
            });

        services.AddHttpClient<QuestionnaireScraper>();

        return services;
    }
}