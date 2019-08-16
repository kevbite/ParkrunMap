using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.FunctionsApp.Parkruns
{
    public class ParkrunBuilder
    {
        private readonly EventsJsonParkrun _parkrun;
        private (double latitude, double longitude)? _location;

        public ParkrunBuilder(EventsJsonParkrun parkrun)
        {
            _parkrun = parkrun;
        }

        public void SetLocation((double latitude, double longitude) location)
        {
            _location = location;
        }

        public EventsJsonParkrun Build()
        {
            return new EventsJsonParkrun(
                _parkrun.Name,
                _parkrun.WebsiteDomain,
                _parkrun.WebsitePath,
                _location?.latitude ?? _parkrun.Latitude,
                _location?.longitude ?? _parkrun.Longitude
            );
        }
    }
}