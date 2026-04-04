using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using ParkrunMap.Scraping;

namespace ParkrunMap.Scraping.Tests;

public class EventsJsonScraperTests
{
    private static HttpClient CreateMockClient(string json)
    {
        var handler = new MockHttpMessageHandler(json);
        return new HttpClient(handler) { BaseAddress = new Uri("https://images.parkrun.com") };
    }

    [Fact]
    public async Task ScrapeAsync_ParsesUkParkrun_Correctly()
    {
        var json = BuildEventsJson(new[]
        {
            new { id = 1, eventname = "bushy", name = "Bushy parkrun", lat = 51.4099, lon = -0.3348, countryCode = 97, seriesId = 1 }
        });

        var scraper = new EventsJsonScraper(CreateMockClient(json), NullLogger<EventsJsonScraper>.Instance);

        var results = await scraper.ScrapeAsync();

        var result = Assert.Single(results);
        Assert.Equal(1, result.Id);
        Assert.Equal("Bushy parkrun", result.Name);
        Assert.Equal("bushy", result.ShortName);
        Assert.Equal(51.4099, result.Latitude);
        Assert.Equal(-0.3348, result.Longitude);
        Assert.Equal("www.parkrun.org.uk", result.Domain);
        Assert.Equal("/bushy/", result.Path);
        // countries section maps code 97 to UK
        Assert.Equal("UK", result.Country);
    }

    [Fact]
    public async Task ScrapeAsync_ParsesAustralianParkrun_WithCorrectDomain()
    {
        var json = BuildEventsJson(new[]
        {
            new { id = 25, eventname = "stpeters", name = "St Peters parkrun", lat = -33.91, lon = 151.18, countryCode = 3, seriesId = 1 }
        });

        var scraper = new EventsJsonScraper(CreateMockClient(json), NullLogger<EventsJsonScraper>.Instance);

        var results = await scraper.ScrapeAsync();

        var result = Assert.Single(results);
        Assert.Equal("www.parkrun.com.au", result.Domain);
        // countries section maps code 3 to AU
        Assert.Equal("AU", result.Country);
    }

    [Fact]
    public async Task ScrapeAsync_ParsesCountry_FromCountriesSection()
    {
        var json = BuildEventsJsonWithCountries(
            new[]
            {
                new { id = 1, eventname = "bushy", name = "Bushy parkrun", lat = 51.41, lon = -0.34, countryCode = 97, seriesId = 1 }
            },
            new Dictionary<int, (string Url, string Cc)>
            {
                [97] = ("www.parkrun.org.uk", "UK")
            });

        var scraper = new EventsJsonScraper(CreateMockClient(json), NullLogger<EventsJsonScraper>.Instance);

        var results = await scraper.ScrapeAsync();

        var result = Assert.Single(results);
        Assert.Equal("www.parkrun.org.uk", result.Domain);
        Assert.Equal("UK", result.Country);
    }

    [Fact]
    public async Task ScrapeAsync_ParsesMultipleParkruns()
    {
        var json = BuildEventsJson(new[]
        {
            new { id = 1, eventname = "bushy",    name = "Bushy parkrun",    lat = 51.0, lon = -0.1, countryCode = 97, seriesId = 1 },
            new { id = 2, eventname = "dudley",   name = "Dudley parkrun",   lat = 52.0, lon = -0.2, countryCode = 97, seriesId = 1 },
            new { id = 3, eventname = "richmond", name = "Richmond parkrun", lat = 53.0, lon = -0.3, countryCode = 97, seriesId = 1 }
        });

        var scraper = new EventsJsonScraper(CreateMockClient(json), NullLogger<EventsJsonScraper>.Instance);

        var results = await scraper.ScrapeAsync();

        Assert.Equal(3, results.Count);
    }

    [Fact]
    public async Task ScrapeAsync_ReturnsEmptyList_WhenNoEvents()
    {
        var json = """{"events":{"features":[]}}""";

        var scraper = new EventsJsonScraper(CreateMockClient(json), NullLogger<EventsJsonScraper>.Instance);

        var results = await scraper.ScrapeAsync();

        Assert.Empty(results);
    }

    [Fact]
    public async Task ScrapeAsync_FiltersOutJuniorParkruns()
    {
        var json = BuildEventsJson(new[]
        {
            new { id = 1,  eventname = "bushy",         name = "Bushy parkrun",        lat = 51.41, lon = -0.34, countryCode = 97, seriesId = 1 },
            new { id = 32, eventname = "lloyd-juniors", name = "Lloyd junior parkrun", lat = 51.37, lon = -0.08, countryCode = 97, seriesId = 2 }
        });

        var scraper = new EventsJsonScraper(CreateMockClient(json), NullLogger<EventsJsonScraper>.Instance);

        var results = await scraper.ScrapeAsync();

        var result = Assert.Single(results);
        Assert.Equal("Bushy parkrun", result.Name);
    }

    [Theory]
    [InlineData(97, "www.parkrun.org.uk", "UK")]
    [InlineData(3,  "www.parkrun.com.au", "AU")]
    [InlineData(42, "www.parkrun.ie",     "IE")]
    [InlineData(85, "www.parkrun.co.za",  "ZA")]
    public async Task ScrapeAsync_ParsesDomainAndCountry_FromCountriesSection(
        int countryCode, string expectedDomain, string expectedCountry)
    {
        var json = BuildEventsJsonWithCountries(
            new[] { new { id = 1, eventname = "test", name = "Test parkrun", lat = 0.0, lon = 0.0, countryCode, seriesId = 1 } },
            new Dictionary<int, (string Url, string Cc)>
            {
                [countryCode] = (expectedDomain, expectedCountry)
            });

        var scraper = new EventsJsonScraper(CreateMockClient(json), NullLogger<EventsJsonScraper>.Instance);
        var results = await scraper.ScrapeAsync();

        var result = Assert.Single(results);
        Assert.Equal(expectedDomain, result.Domain);
        Assert.Equal(expectedCountry, result.Country);
    }

    [Fact]
    public async Task ScrapeAsync_ThrowsInvalidOperationException_WhenCountryCodeMissingFromCountriesSection()
    {
        // Country code 999 is not in the countries section, and there is no fallback.
        var json = BuildEventsJsonWithCountries(
            new[] { new { id = 1, eventname = "test", name = "Test parkrun", lat = 0.0, lon = 0.0, countryCode = 999, seriesId = 1 } },
            new Dictionary<int, (string Url, string Cc)>());

        var scraper = new EventsJsonScraper(CreateMockClient(json), NullLogger<EventsJsonScraper>.Instance);

        await Assert.ThrowsAsync<InvalidOperationException>(() => scraper.ScrapeAsync());
    }

    [Fact]
    public async Task ScrapeAsync_SkipsMalformedEvent_ContinuesWithValid()
    {
        var json = """
            {
              "events": {
                "features": [
                  {
                    "id": 1,
                    "properties": {
                      "eventname": "good",
                      "EventLongName": "Good parkrun",
                      "countrycode": 97,
                      "seriesid": 1
                    },
                    "geometry": { "coordinates": [-0.1, 51.0] }
                  },
                  {
                    "id": 2,
                    "properties": {},
                    "geometry": { "coordinates": [] }
                  }
                ]
              },
              "countries": { "97": { "url": "www.parkrun.org.uk", "cc": "UK" } }
            }
            """;

        var scraper = new EventsJsonScraper(CreateMockClient(json), NullLogger<EventsJsonScraper>.Instance);

        var results = await scraper.ScrapeAsync();

        Assert.Single(results);
        Assert.Equal("Good parkrun", results[0].Name);
    }

    private static string BuildEventsJson(IEnumerable<dynamic> events)
    {
        var defaultCountries = new Dictionary<int, (string Url, string Cc)>
        {
            [97] = ("www.parkrun.org.uk", "UK"),
            [3]  = ("www.parkrun.com.au", "AU"),
        };
        return BuildEventsJsonWithCountries(events, defaultCountries);
    }

    private static string BuildEventsJsonWithCountries(
        IEnumerable<dynamic> events,
        Dictionary<int, (string Url, string Cc)> countries)
    {
        var features = events.Select(e => $$"""
            {
              "id": {{e.id}},
              "type": "Feature",
              "geometry": {
                "coordinates": [{{e.lon}}, {{e.lat}}]
              },
              "properties": {
                "eventname": "{{e.eventname}}",
                "EventLongName": "{{e.name}}",
                "EventShortName": "{{e.name}}",
                "countrycode": {{e.countryCode}},
                "seriesid": {{e.seriesId}}
              }
            }
            """);

        var countriesJson = string.Join(",", countries.Select(kvp =>
            $"\"{kvp.Key}\":{{\"url\":\"{kvp.Value.Url}\",\"cc\":\"{kvp.Value.Cc}\"}}"));

        return $"{{\"events\":{{\"features\":[{string.Join(",", features)}]}},\"countries\":{{{countriesJson}}}}}";
    }

    private sealed class MockHttpMessageHandler(string responseBody) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody, System.Text.Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }
}