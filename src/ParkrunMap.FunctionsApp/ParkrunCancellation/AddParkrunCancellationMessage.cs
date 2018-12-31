using System;

namespace ParkrunMap.FunctionsApp.AddParkrunCancellation
{
    public class AddParkrunCancellationMessage
    {
        public DateTime Date { get; set; }

        public string Name { get; set; }

        public Uri Uri { get; set; }

        public string Reason { get; set; }
    }
}