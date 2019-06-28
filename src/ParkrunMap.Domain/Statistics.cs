namespace ParkrunMap.Domain
{
    public class Statistics
    {
        public int? TotalEvents { get; set; }

        public int? TotalRunners { get; set; }

        public int? TotalRuns { get; set; }

        public double? AverageRunnersPerWeek { get; set; }

        public int? AverageSecondsRan { get; set; }

        public long? TotalSecondsRan { get; set; }

        public int? BiggestAttendance { get; set; }

        public int? TotalKmDistanceRan { get; set; }
    }
}