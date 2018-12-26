using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ParkrunMap.FunctionsApp.UpsertParkrun;
using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.FunctionsApp.ParseGeoXml
{
    public class ParseGeoXmlFunction
    {
        private readonly ILogger _logger;
        private readonly GeoXmlParser _parser;
        private readonly IMapper _mapper;

        public ParseGeoXmlFunction(ILogger logger, GeoXmlParser parser, IMapper mapper)
        {
            _logger = logger;
            _parser = parser;
            _mapper = mapper;
        }

        [FunctionName("ParseGeoXmlFunction")]
        public static async Task Run([BlobTrigger("downloads/geo.xml", Connection = "AzureWebJobsStorage")]Stream geoXml, [Queue("parkrun-upserts", Connection = "AzureWebJobsStorage")] ICollector<UpsertParkrunMessage> messageCollector, ILogger logger)
        {
            var func = Container.Instance.Resolve<ParseGeoXmlFunction>(logger);

            await func.Run(geoXml, messageCollector)
                .ConfigureAwait(false);
        }

        private async Task Run(Stream geoXml, ICollector<UpsertParkrunMessage> messageCollector)
        {
            var parkruns = _parser.Parse(geoXml);
            _logger.LogInformation("Scrapped {Count} parkruns", parkruns.Count);

            foreach (var parkrun in parkruns)
            {
                _logger.LogInformation("Sending upsert parkrun message for {ParkrunName}", parkrun.Name);

                var message = _mapper.Map<UpsertParkrunMessage>(parkrun);

                messageCollector.Add(message);
            }
        }
    }
}
