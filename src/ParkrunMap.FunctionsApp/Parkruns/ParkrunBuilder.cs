using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.FunctionsApp.Parkruns
{
    public class ParkrunBuilder
    {
        private readonly Parkrun _parkrun;
        private (double latitude, double longitude)? _location;

        public ParkrunBuilder(Parkrun parkrun)
        {
            _parkrun = parkrun;
        }

        public void SetLocation((double latitude, double longitude) location)
        {
            _location = location;
        }

        public Parkrun Build()
        {
            return new Parkrun(
                _parkrun.GeoXmlId,
                _parkrun.Name,
                _parkrun.WebsiteDomain,
                _parkrun.WebsitePath,
                _parkrun.Region,
                _parkrun.Country,
                _location?.latitude ?? _parkrun.Latitude,
                _location?.longitude ?? _parkrun.Longitude
            );
        }
    }
}