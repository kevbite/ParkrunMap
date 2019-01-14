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
                {"www.parkrun.dk", "Kort over ruten" },
                {"www.parkrun.ru", "Карта трассы" },
                {"www.parkrun.com.de", "Streckenplan" },
                {"www.parkrun.fr", "Carte du parcours" },
                {"www.parkrun.it", "Mappa Percorso" }
            };

        private static readonly IReadOnlyDictionary<string, string> DomainToCourseDescriptionHeaderMap =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"www.parkrun.org.uk", "Course Description"},
                {"www.parkrun.pl", "Opis trasy"},
                {"www.parkrun.dk", "Beskrivelse af ruten" },
                {"www.parkrun.ru", "Описание трассы" },
                {"www.parkrun.com.de", "Streckenbeschreibung" },
                {"www.parkrun.fr", "Description du parcours" },
                {"www.parkrun.it", "Descrizione Percorso" }
            };

        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task<CourseDetails> Parse(Stream stream, string domain)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(stream);

            var headers = htmlDoc.DocumentNode.SelectNodes("//h2");

            var description = ParseDescription(headers, domain);

            var googleMapId = await ParseGoogleMapId(headers, domain);

            return new CourseDetails(description, googleMapId);
        }

        private static async Task<string> ParseGoogleMapId(HtmlNodeCollection headers, string domain)
        {
            var courseMapH1 = headers.First(x => x.InnerText == DomainToCourseMapHeaderMap[domain]);

            var uri = courseMapH1.SelectSingleNode("following-sibling::iframe").Attributes["src"].DeEntitizeValue;

            if (TryParseGoogleMapId(uri, out var stringValues))
            {
                return stringValues;
            }

            return await FollowUri(uri);
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

        private static string ParseDescription(HtmlNodeCollection headers, string domain)
        {
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