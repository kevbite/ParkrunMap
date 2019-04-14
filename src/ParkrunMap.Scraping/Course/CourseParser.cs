using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.WebUtilities;

namespace ParkrunMap.Scraping.Course
{
    public class CourseParser
    {
        private static readonly IReadOnlyDictionary<string, string> DomainToCourseMapHeaderMap =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"www.parkrun.org.uk", "Course Map"},
                {"www.parkrun.pl", "Mapa trasy"},
                {"www.parkrun.dk", "Kort over ruten"},
                {"www.parkrun.ru", "Карта трассы"},
                {"www.parkrun.com.de", "Streckenplan"},
                {"www.parkrun.fr", "Carte du parcours"},
                {"www.parkrun.it", "Mappa Percorso"},
                {"www.parkrun.co.za", "Course Map"},
                {"www.parkrun.com.au", "Course Map"},
                {"www.parkrun.ie", "Course Map"},
                { "www.parkrun.sg", "Course Map"},
                {"www.parkrun.us", "Course Map"},
                {"www.parkrun.co.nz", "Course Map"},
                {"www.parkrun.ca", "Course Map"},
                {"www.parkrun.no", "Course Map"},
                {"www.parkrun.fi", "Course Map"},
                {"www.parkrun.my", "Course Map"},
                {"www.parkrun.se", "Karta över banan"},
                {"www.parkrun.jp", "コースマップ" },
            };

        private static readonly IReadOnlyDictionary<string, string> DomainToCourseDescriptionHeaderMap =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"www.parkrun.org.uk", "Course Description"},
                {"www.parkrun.pl", "Opis trasy"},
                {"www.parkrun.dk", "Beskrivelse af ruten"},
                {"www.parkrun.ru", "Описание трассы"},
                {"www.parkrun.com.de", "Streckenbeschreibung"},
                {"www.parkrun.fr", "Description du parcours"},
                {"www.parkrun.it", "Descrizione Percorso"},
                {"www.parkrun.co.za", "Course Description"},
                {"www.parkrun.com.au", "Course Description"},
                {"www.parkrun.ie", "Course Description"},
                {"www.parkrun.sg", "Course Description"},
                {"www.parkrun.us", "Course Description"},
                {"www.parkrun.co.nz", "Course Description"},
                {"www.parkrun.ca", "Course Description"},
                {"www.parkrun.no", "Course Description"},
                {"www.parkrun.fi", "Course Description"},
                {"www.parkrun.my", "Course Description"},
                {"www.parkrun.se", "Beskrivning av banan"},
                {"www.parkrun.jp", "コースの詳細" },
            };

        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task<CourseDetails> Parse(Stream stream, string domain)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(stream);

            var description = ParseDescription(htmlDoc.DocumentNode, domain);

            var googleMapIds = await ParseGoogleMapIds(htmlDoc.DocumentNode, domain);

            return new CourseDetails(description, googleMapIds);
        }

        private static async Task<IReadOnlyCollection<string>> ParseGoogleMapIds(HtmlNode document, string domain)
        {
            var iframes = document.SelectNodes("//iframe")
                .Select(x => x.Attributes["src"].DeEntitizeValue)
                .Where(x => x.Contains("map"));

            var googleMapIds = new List<string>();
            foreach (var uri in iframes)
            {
                if (TryParseGoogleMapId(uri, out var stringValues))
                {
                    googleMapIds.Add(stringValues);
                    continue;
                }

                googleMapIds.Add(await FollowUri(uri)
                    .ConfigureAwait(false));
            }

            return googleMapIds;
        }

        private static async Task<string> FollowUri(string uri)
        {
            var httpResponseMessage = await HttpClient.GetAsync(uri)
                .ConfigureAwait(false);

            if (TryParseGoogleMapId(httpResponseMessage.RequestMessage.RequestUri, out var value))
            {
                return value;
            }

            throw new Exception($"Could not find Google Map Id after following course map iframe src {uri}");
        }

        private static bool TryParseGoogleMapId(Uri uri, out string value)
        {
            var query = QueryHelpers.ParseQuery(uri.Query);

            if (query.TryGetValue("mid", out var googleMapId))
            {
                value = googleMapId;
                return true;
            }

            value = null;
            return false;
        }


        private static bool TryParseGoogleMapId(string uri, out string value)
        {
            return TryParseGoogleMapId(new Uri(uri), out value);
        }

        private static string ParseDescription(HtmlNode documentNode, string domain)
        {
            var headers = documentNode.SelectNodes("//h2");
            var node = headers.First(x => x.InnerText == DomainToCourseDescriptionHeaderMap[domain]);

            string description = null;

            while (string.IsNullOrEmpty(description))
            {
                node = node.NextSibling;

                var strings = Regex.Split(node.InnerText, @"(\r\n|\r|\n) *");

                description = string.Join(" ", strings.Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)));
            }

            return description;
        }
    }
}