using System;

namespace ParkrunMap.Scraping.Stats
{
    public class ParkrunStats
    {
        public ParkrunStats(int totalEvents,
            int totalRunners,
            int totalRuns,
            double averageRunnersPerWeek,
            int averageSecondsRan,
            long totalSecondsRan,
            int biggestAttendance,
            int totalKmDistanceRan)
        {
            TotalEvents = totalEvents;
            TotalRunners = totalRunners;
            TotalRuns = totalRuns;
            AverageRunnersPerWeek = averageRunnersPerWeek;
            AverageSecondsRan = averageSecondsRan;
            TotalSecondsRan = totalSecondsRan;
            BiggestAttendance = biggestAttendance;
            TotalKmDistanceRan = totalKmDistanceRan;
        }

        public int TotalEvents { get; }

        public int TotalRunners { get;}

        public int TotalRuns { get; }

        public double AverageRunnersPerWeek { get; }

        public int AverageSecondsRan { get; }

        public long TotalSecondsRan { get; }

        public int BiggestAttendance { get; }

        public int TotalKmDistanceRan { get; }

    }
}
