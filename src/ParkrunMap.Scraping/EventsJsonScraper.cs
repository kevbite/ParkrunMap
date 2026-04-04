using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace ParkrunMap.Scraping;

public record EventsJsonParkrun(
    int Id,
    string Name,
    string ShortName,
    double Latitude,
    double Longitude,
    int CountryCode,
    string Country,
    string Domain,
    string Path);

public class EventsJsonScraper(HttpClient httpClient, ILogger<EventsJsonScraper> logger)
{
    private const string EventsJsonPath = "events.json";

    public async Task<IReadOnlyList<EventsJsonParkrun>> ScrapeAsync(CancellationToken ct = default)
    {
        logger.LogInformation("Downloading events.json from {BaseAddress}{Path}", httpClient.BaseAddress, EventsJsonPath);

        var json = await httpClient.GetStringAsync(EventsJsonPath, ct);
        using var document = JsonDocument.Parse(json);

        var countryLookup = ParseCountries(document.RootElement);

        var results = new List<EventsJsonParkrun>();
        var events = document.RootElement.GetProperty("events").GetProperty("features");

        foreach (var feature in events.EnumerateArray())
        {
            try
            {
                var props = feature.GetProperty("properties");

                // Skip junior parkruns (seriesid 2)
                if (props.GetProperty("seriesid").GetInt32() == 2)
                    continue;

                var coords = feature.GetProperty("geometry").GetProperty("coordinates");

                var id = feature.GetProperty("id").GetInt32();
                var name = props.GetProperty("EventLongName").GetString() ?? string.Empty;
                var shortName = props.GetProperty("eventname").GetString() ?? string.Empty;
                var countryCode = props.GetProperty("countrycode").GetInt32();
                var lon = coords[0].GetDouble();
                var lat = coords[1].GetDouble();

                string domain, country;
                if (countryLookup.TryGetValue(countryCode, out var info))
                {
                    domain = info.Url;
                    country = info.Country;
                }
                else
                {
                    throw new InvalidOperationException($"Country code {countryCode} is not present in the events.json countries section. Update the countries section or check the data source.");
                }

                results.Add(new EventsJsonParkrun(id, name, shortName, lat, lon, countryCode, country, domain, $"/{shortName}/"));
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                logger.LogWarning(ex, "Failed to parse event from events.json, skipping");
            }
        }

        logger.LogInformation("Parsed {Count} parkruns from events.json", results.Count);
        return results;
    }

    /// <summary>Parses the top-level "countries" dictionary into a lookup of country code  (url, cc).</summary>
    private static Dictionary<int, (string Url, string Country)> ParseCountries(JsonElement root)
    {
        var lookup = new Dictionary<int, (string, string)>();

        if (!root.TryGetProperty("countries", out var countries))
            return lookup;

        foreach (var entry in countries.EnumerateObject())
        {
            if (!int.TryParse(entry.Name, out var code))
                continue;

            var url = entry.Value.TryGetProperty("url", out var urlProp)
                ? urlProp.GetString() ?? string.Empty
                : string.Empty;

            var cc = entry.Value.TryGetProperty("cc", out var ccProp)
                ? ccProp.GetString() ?? code.ToString()
                : code.ToString();

            if (!string.IsNullOrEmpty(url))
                lookup[code] = (url, cc);
        }

        return lookup;
    }
}