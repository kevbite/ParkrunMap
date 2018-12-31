using System;

namespace ParkrunMap.Scraping.Cancellations
{
    public class ParkrunCancellation
    {
        public DateTime Date { get; }

        public string Name { get; }

        public string WebsiteDomain { get; }

        public string WebsitePath { get; }

        public string Reason { get; }

        public ParkrunCancellation(DateTime date, string name, string websiteDomain, string websitePath, string reason)
        {
            Date = date;
            Name = name;
            WebsiteDomain = websiteDomain;
            WebsitePath = websitePath;
            Reason = reason;
        }
    }
}