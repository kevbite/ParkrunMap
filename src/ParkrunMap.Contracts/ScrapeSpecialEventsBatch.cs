namespace ParkrunMap.Contracts;

public record ScrapeSpecialEventsBatch(IReadOnlyList<SpecialEventsScrapeTarget> Targets);

public record SpecialEventsScrapeTarget(string CountryBaseUrl, string CountryCode);
