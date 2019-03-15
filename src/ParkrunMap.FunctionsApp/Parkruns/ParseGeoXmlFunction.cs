using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.FunctionsApp.Parkruns
{
    public class ParseGeoXmlFunction
    {
        private readonly ILogger _logger;
        private readonly GeoXmlParser _parser;
        private readonly IMapper _mapper;
        private readonly ParkrunOverrides _parkrunOverrides;

        public ParseGeoXmlFunction(ILogger logger, GeoXmlParser parser, IMapper mapper, ParkrunOverrides parkrunOverrides)
        {
            _logger = logger;
            _parser = parser;
            _mapper = mapper;
            _parkrunOverrides = parkrunOverrides;
        }

        [FunctionName("ParseGeoXmlFunction")]
        public static async Task Run([BlobTrigger(DownloadFilePaths.GeoXml, Connection = "AzureWebJobsStorage")]Stream geoXml,
            [Queue(QueueNames.UpsertParkrun, Connection = "AzureWebJobsStorage")]
            IAsyncCollector<UpsertParkrunMessage> messageCollector, ILogger logger)
        {
            var func = Container.Instance.Resolve<ParseGeoXmlFunction>(logger);

            await func.Run(geoXml, messageCollector)
                .ConfigureAwait(false);
        }

        private async Task Run(Stream geoXml, IAsyncCollector<UpsertParkrunMessage> messageCollector)
        {
            var parkruns = _parser.Parse(geoXml);
            _logger.LogInformation("Scrapped {Count} parkruns", parkruns.Count);

            foreach (var parsedParkrun in parkruns)
            {
                _logger.LogInformation("Applying parkrun overrides for {ParkrunName}", parsedParkrun.Name);

                var parkrun = _parkrunOverrides.Apply(parsedParkrun);
                _logger.LogInformation("Sending upsert parkrun message for {ParkrunName}", parkrun.Name);

                var message = _mapper.Map<UpsertParkrunMessage>(parkrun);

                await messageCollector.AddAsync(message);
            }
        }
    }
}
