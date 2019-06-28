using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using ParkrunMap.Data.Mongo;
using ParkrunMap.FunctionsApp.Course;
using ParkrunMap.Scraping.Course;
using ParkrunMap.Scraping.Statistics;

namespace ParkrunMap.FunctionsApp.ParkrunStatistics
{
    public class ParseStatisticsFunction
    {
        private readonly StatisticsParser _parser;
        private readonly ILogger _logger;

        public ParseStatisticsFunction(StatisticsParser parser, ILogger logger)
        {
            _parser = parser;
            _logger = logger;
        }

        [FunctionName(nameof(ParseStatisticsFunction))]
        [return: Queue(QueueNames.UpdateParkrunStatistics, Connection = "AzureWebJobsStorage")]
        public static async Task<UpdateParkrunStatistics.Request> Run([BlobTrigger("downloads/course/{path}.html", Connection = "AzureWebJobsStorage")]Stream htmlStream,
            string path,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            return await Container.Instance.Resolve<ParseStatisticsFunction>(logger).Run(htmlStream, path, cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task<UpdateParkrunStatistics.Request> Run(Stream htmlStream, string path, CancellationToken cancellationToken)
        {
            var pathSplit = path.Split('/');
            var websiteDomain = pathSplit[0];
            var websitePath = '/' + pathSplit[1];

            _logger.LogInformation("Parsing parkrun statistics for {websiteDomain}{websitePath}", websiteDomain, websitePath);
            var statistics = await _parser.Parse(htmlStream, websiteDomain)
                .ConfigureAwait(false);

            return new UpdateParkrunStatistics.Request()
            {
                WebsiteDomain = websiteDomain,
                WebsitePath = websitePath,
                AverageRunnersPerWeek = statistics.AverageRunnersPerWeek,
                AverageSecondsRan = statistics.AverageSecondsRan,
                BiggestAttendance = statistics.BiggestAttendance,
                TotalEvents = statistics.TotalEvents,
                TotalKmDistanceRan = statistics.TotalKmDistanceRan,
                TotalRunners = statistics.TotalRunners,
                TotalRuns = statistics.TotalRuns,
                TotalSecondsRan = statistics.TotalSecondsRan
            };
        }
    }
}
