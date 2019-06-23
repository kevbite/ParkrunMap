using System;

namespace ParkrunMap.Scraping.Stats
{
    public class ParkrunStats
    {
        public ParkrunStats(int totalEvents,
            int totalRunners,
            int totalRuns,
            double averageRunnersPerWeek,
            TimeSpan averageRunTime,
            TimeSpan totalRunTime,
            int biggestAttendance,
            int totalKmDistanceRan)
        {
            TotalEvents = totalEvents;
            TotalRunners = totalRunners;
            TotalRuns = totalRuns;
            AverageRunnersPerWeek = averageRunnersPerWeek;
            AverageRunTime = averageRunTime;
            TotalRunTime = totalRunTime;
            BiggestAttendance = biggestAttendance;
            TotalKmDistanceRan = totalKmDistanceRan;
        }

        public int TotalEvents { get; }

        public int TotalRunners { get;}

        public int TotalRuns { get; }

        public double AverageRunnersPerWeek { get; }

        public TimeSpan AverageRunTime { get; }

        public TimeSpan TotalRunTime { get; }

        public int BiggestAttendance { get; }

        public int TotalKmDistanceRan { get; }

    }
}
