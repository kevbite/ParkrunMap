using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.FunctionsApp.Parkruns
{
    public class ParkrunBuilder
    {
        private readonly GeoXmlParkrun _parkrun;
        private (double latitude, double longitude)? _location;

        public ParkrunBuilder(GeoXmlParkrun parkrun)
        {
            _parkrun = parkrun;
        }

        public void SetLocation((double latitude, double longitude) location)
        {
            _location = location;
        }

        public GeoXmlParkrun Build()
        {
            return new GeoXmlParkrun(
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