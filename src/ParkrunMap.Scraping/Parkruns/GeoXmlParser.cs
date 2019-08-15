using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ParkrunMap.Scraping.Parkruns
{
    public class GeoXmlParser
    {
        private readonly ParkrunXElementValidator _validator;

        public GeoXmlParser(ParkrunXElementValidator validator)
        {
            _validator = validator;
        }

        public IReadOnlyCollection<GeoXmlParkrun> Parse(Stream stream)
        {
            var document = XDocument.Load(stream);

            var regions = document.Descendants("r")
                .ToDictionary(x => (int) x.Attribute("id"),
                    x => new
                    {
                        Name = (string) x.Attribute("n"),
                        Uri = (string) x.Attribute("u"),
                        ParentRegionId = string.IsNullOrEmpty((string)x.Attribute("pid")) ? null : (int?)x.Attribute("pid"),
                    });

            var parkruns = new List<GeoXmlParkrun>();

            foreach (var element in document.Descendants("e").Where(_validator.IsValid))
            {
                var id = (int)element.Attribute("id");
                var name = (string)element.Attribute("m");
                var n = (string)element.Attribute("n");
                var latitude = (double) element.Attribute("la");
                var longitude= (double)element.Attribute("lo");
                var regionId = (int)element.Attribute("r");

                var region = regions[regionId];
                var country = regions[region.ParentRegionId.Value];
                if (!country.ParentRegionId.HasValue)
                {
                    country = region;
                    region = null;
                }

                var uri = new Uri(country.Uri);
                var websiteDomain = uri.Host;
                var websitePath = $"/{n}";

                var parkrun = new GeoXmlParkrun(id, name, websiteDomain, websitePath, region?.Name, country.Name, latitude, longitude);

                parkruns.Add(parkrun);
            }

            return parkruns;
        }
    }
}
