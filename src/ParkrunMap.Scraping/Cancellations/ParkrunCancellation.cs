using System;

namespace ParkrunMap.Scraping.Cancellations
{
    public class ParkrunCancellation
    {
        public DateTime Date { get; }

        public string Name { get; }

        public Uri Uri { get; }

        public string Reason { get; }

        public ParkrunCancellation(DateTime date, string name, Uri uri, string reason)
        {
            Date = date;
            Name = name;
            Uri = uri;
            Reason = reason;
        }
    }
}