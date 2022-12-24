using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace ParkrunMap.Scraping.Cancellations
{
    public class CancellationsParser
    {
        public IReadOnlyCollection<ParkrunCancellation> Parse(Stream stream)
        {
            var cancellations = new List<ParkrunCancellation>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(stream);

            var h2s = htmlDoc.DocumentNode.SelectNodes("//div[@id='content']//h2");

            foreach (var h2 in h2s.Skip(1))
            {
                if (DateTime.TryParseExact(h2.InnerText, "dddd, d MMMM yyyy", CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal, out var date))
                {
                    var listItems = h2.SelectSingleNode("following-sibling::ul")
                        .SelectNodes("./li");

                    foreach (var listItem in listItems)
                    {
                        var link = listItem.SelectSingleNode("a");
                        var parkrunTitle = link.InnerText;
                        var parkrunUri = link.Attributes["href"].Value;
                        
                        var regex = new Regex("[ \r\n]{2,}", RegexOptions.None);
                        var reason = regex.Replace(listItem.InnerText.Split(':').Last(), " ").Trim();;
                        
                        if (parkrunUri.EndsWith("-juniors"))
                        {
                            // We're ignoring junior parkruns
                            continue;
                        }

                        var uri = new Uri(parkrunUri);
                        var cancellation = new ParkrunCancellation(date, parkrunTitle.Trim(), uri.Host, uri.PathAndQuery.TrimEnd('/'), reason);

                        cancellations.Add(cancellation);
                    }

                }

            }

            return cancellations;
        }
    }
}