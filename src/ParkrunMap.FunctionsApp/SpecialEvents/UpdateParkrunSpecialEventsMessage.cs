using ParkrunMap.Data.Mongo;

namespace ParkrunMap.FunctionsApp.SpecialEvents
{
    public class UpdateParkrunSpecialEventsMessage
    {
        public string WebsiteDomain { get; set; }

        public string WebsitePath { get;  set;}

        public int Year { get;  set;}

        public SpecialEventType Type { get; set; }

        public bool IsRunning { get; set; }
    }
}