﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ParkrunMap.Scraping.Parsers
{
    public class GeoXmlParser
    {
        public IReadOnlyCollection<Parkrun> Parse(Stream stream)
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

            var parkruns = new List<Parkrun>();
            foreach (var element in document.Descendants("e"))
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

                var uri = BuildParkrunUri(country.Uri, n);

                var parkrun = new Parkrun(id, name, uri, region?.Name, country.Name, latitude, longitude);

                parkruns.Add(parkrun);
            }

            return parkruns;
        }

        private static Uri BuildParkrunUri(string regionUri, string n)
        {
            var uriBuilder = new UriBuilder(regionUri);

            uriBuilder.Path = n;

            return uriBuilder.Uri;
        }
    }
}
