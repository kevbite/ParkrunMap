using System;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.AspNetCore.WebUtilities;

namespace ParkrunMap.Scraping.Course
{
    public class CourseParser
    {
        public object Parse(Stream stream)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(stream);

            var headers = htmlDoc.DocumentNode.SelectNodes("//h2");

            var description = ParseDescription(headers);

            var googleMapId = ParseGoogleMapId(headers);

            return new CourseDetails(description, googleMapId);
        }

        private static string ParseGoogleMapId(HtmlNodeCollection headers)
        {
            var courseMapH1 = headers.First(x => x.InnerText == "Course Map");

            var uri = courseMapH1.SelectSingleNode("following-sibling::iframe").GetAttributeValue("src", null);

            var query = QueryHelpers.ParseQuery(new Uri(uri).Query);
            var googleMapId = query["mid"];

            return googleMapId;
        }

        private static string ParseDescription(HtmlNodeCollection headers)
        {
            var theCourseH1 = headers.First(x => x.InnerText == "Course Description");

            var innerText = theCourseH1.SelectSingleNode("following-sibling::text()").InnerText;

            var description = innerText.Trim('\r', '\n', ' ');

            return description;
        }
    }
}