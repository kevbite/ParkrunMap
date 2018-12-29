using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

            var h1s = htmlDoc.DocumentNode.SelectNodes("//div[@id='content']//h1");

            foreach (var h1 in h1s)
            {
                if (DateTime.TryParseExact(h1.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal, out var date))
                {
                    var listItems = h1.NextSibling.ChildNodes;

                    foreach (var listItem in listItems)
                    {
                        var link = listItem.SelectSingleNode("a");
                        var parkrunTitle = link.InnerText;
                        var parkrunUri = link.Attributes["href"].Value;
                        var reason = listItem.InnerText.Split(':').Last().Trim();

                        if (parkrunTitle.EndsWith("junior parkrun"))
                        {
                            // We're ignoring junior parkruns
                            continue;
                        }

                        var cancellation = new ParkrunCancellation(date, parkrunTitle.Trim(), new Uri(parkrunUri), reason);

                        cancellations.Add(cancellation);
                    }

                }

            }

            return cancellations;
        }
    }
}