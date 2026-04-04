using System.Text.Json;
using Microsoft.Extensions.Logging;
using ParkrunMap.Domain;

namespace ParkrunMap.Scraping;

public record QuestionnaireEntry(
    string Domain,
    string Path,
    bool? WheelchairFriendly,
    bool? BuggyFriendly,
    bool? VisuallyImpairedFriendly,
    bool? Toilets,
    bool? DogsAllowed,
    bool? Cafe,
    bool? PostRunCoffee,
    bool? DrinkingFountain,
    bool? ChangingRooms,
    bool? Lockers,
    bool? Showers,
    bool? BagDrop,
    bool? BabyChangingFacilities,
    bool? CarParking,
    bool? CycleParking,
    IReadOnlyList<CarParkingOption> CarParkingOptions,
    IReadOnlyList<CycleParkingOption> CycleParkingOptions,
    IReadOnlyList<BuggyType> RecommendedBuggy);

public class QuestionnaireScraper(HttpClient httpClient, ILogger<QuestionnaireScraper> logger)
{
    private const string SheetsApiBase = "https://sheets.googleapis.com/v4/spreadsheets";

    public async Task<IReadOnlyList<QuestionnaireEntry>> ScrapeAsync(
        string spreadsheetId,
        string apiKey,
        CancellationToken ct = default)
    {
        logger.LogInformation("Downloading questionnaire responses from Google Sheets {SheetId}", spreadsheetId);

        var url = $"{SheetsApiBase}/{spreadsheetId}/values/A:Z?key={apiKey}";
        var json = await httpClient.GetStringAsync(url, ct);
        using var document = JsonDocument.Parse(json);

        var values = document.RootElement.GetProperty("values").EnumerateArray().ToList();
        if (values.Count < 2) return [];

        var headers = values[0].EnumerateArray().Select(h => h.GetString()?.ToLowerInvariant() ?? "").ToList();
        var results = new List<QuestionnaireEntry>();

        foreach (var row in values.Skip(1))
        {
            try
            {
                var cells = row.EnumerateArray().Select(c => c.GetString() ?? "").ToList();
                var get = (string header) =>
                {
                    var idx = headers.IndexOf(header);
                    return idx >= 0 && idx < cells.Count ? cells[idx] : null;
                };

                var domain = get("domain") ?? string.Empty;
                var path = get("path") ?? string.Empty;
                if (string.IsNullOrWhiteSpace(domain) || string.IsNullOrWhiteSpace(path)) continue;

                results.Add(new QuestionnaireEntry(
                    domain, path,
                    ParseBool(get("wheelchair friendly")),
                    ParseBool(get("buggy friendly")),
                    ParseBool(get("visually impaired friendly")),
                    ParseBool(get("toilets")),
                    ParseBool(get("dogs allowed")),
                    ParseBool(get("cafe")),
                    ParseBool(get("post run coffee")),
                    ParseBool(get("drinking fountain")),
                    ParseBool(get("changing rooms")),
                    ParseBool(get("lockers")),
                    ParseBool(get("showers")),
                    ParseBool(get("bag drop")),
                    ParseBool(get("baby changing facilities")),
                    ParseBool(get("car parking")),
                    ParseBool(get("cycle parking")),
                    ParseEnumList<CarParkingOption>(get("car parking options")),
                    ParseEnumList<CycleParkingOption>(get("cycle parking options")),
                    ParseEnumList<BuggyType>(get("recommended buggy"))));
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to parse questionnaire row, skipping");
            }
        }

        logger.LogInformation("Parsed {Count} questionnaire responses", results.Count);
        return results;
    }

    private static bool? ParseBool(string? value) => value?.ToLowerInvariant() switch
    {
        "yes" or "true" or "1" or "y" => true,
        "no" or "false" or "0" or "n" => false,
        _ => null
    };

    private static IReadOnlyList<T> ParseEnumList<T>(string? value) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value)) return [];
        return value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(v => Enum.TryParse<T>(v, ignoreCase: true, out var result) ? result : (T?)null)
            .Where(v => v.HasValue)
            .Select(v => v!.Value)
            .ToList();
    }
}
