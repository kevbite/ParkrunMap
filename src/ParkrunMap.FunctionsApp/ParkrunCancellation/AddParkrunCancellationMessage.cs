using System;

namespace ParkrunMap.FunctionsApp.ParkrunCancellation
{
    public class AddParkrunCancellationMessage
    {
        public DateTime Date { get; set; }

        public string Name { get; set; }

        public string WebsiteDomain { get; set; }

        public string WebsitePath { get; set; }

        public string Reason { get; set; }
    }
}