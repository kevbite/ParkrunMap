namespace ParkrunMap.Contracts;

public record ScrapeCourseBatch(IReadOnlyList<CourseScrapeTarget> Targets);

public record CourseScrapeTarget(string Domain, string Path);
