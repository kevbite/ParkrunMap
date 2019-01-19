using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using ParkrunMap.Scraping.Course;

namespace ParkrunMap.FunctionsApp.Course
{
    public class ParseCourseFunction
    {
        private readonly CourseParser _parser;
        private readonly ILogger _logger;

        public ParseCourseFunction(CourseParser parser, ILogger logger)
        {
            _parser = parser;
            _logger = logger;
        }

        [FunctionName("ParseCourseFunction")]
        [return: Queue("update-course-details", Connection = "AzureWebJobsStorage")]
        public static async Task<UpdateCourseDetailsMessage> Run([BlobTrigger("downloads/course/{path}.html", Connection = "AzureWebJobsStorage")]Stream htmlStream,
            string path,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            return await Container.Instance.Resolve<ParseCourseFunction>(logger).Run(htmlStream, path, cancellationToken)
                .ConfigureAwait(false);

        }

        private async Task<UpdateCourseDetailsMessage> Run(Stream htmlStream, string path, CancellationToken cancellationToken)
        {
            var pathSplit = path.Split('/');
            var websiteDomain = pathSplit[0];
            var websitePath = '/' + pathSplit[1];

            _logger.LogInformation("Parsing course details for {websiteDomain}{websitePath}", websiteDomain, websitePath);
            var courseDetails = await _parser.Parse(htmlStream, websiteDomain)
                .ConfigureAwait(false);

            return new UpdateCourseDetailsMessage()
            {
                WebsiteDomain = websiteDomain,
                WebsitePath = websitePath,
                Description = courseDetails.Description,
                GoogleMapIds = courseDetails.GoogleMapIds
            };
        }
    }
}
