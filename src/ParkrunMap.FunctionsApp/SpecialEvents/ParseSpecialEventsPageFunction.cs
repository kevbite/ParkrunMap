using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ParkrunMap.Scraping.Cancellations;
using ParkrunMap.Scraping.SpecialEvents;

namespace ParkrunMap.FunctionsApp.SpecialEvents
{
    public class ParseSpecialEventsPageFunction
    {
        private readonly ILogger _logger;
        private readonly SpecialEventsParser _parser;
        private readonly IMapper _mapper;

        public ParseSpecialEventsPageFunction(ILogger logger, SpecialEventsParser parser, IMapper mapper)
        {
            _logger = logger;
            _parser = parser;
            _mapper = mapper;
        }

        [FunctionName("ParseCancellationsPageFunction")]
        public static async Task Run([BlobTrigger(ParkrunCancellation.DownloadFilePaths.CancellationsHtml, Connection = "AzureWebJobsStorage")]Stream cancellationsStream,
            [Queue(QueueNames.UpdateParkrunSpecialEvents, Connection = "AzureWebJobsStorage")]
            IAsyncCollector<UpdateParkrunSpecialEventsMessage> messageCollector,
            ILogger logger)
        {
            var func = Container.Instance.Resolve<ParseSpecialEventsPageFunction>(logger);

            await func.Run(cancellationsStream, messageCollector)
                .ConfigureAwait(false);
        }

        private async Task Run(Stream cancellationsStream, IAsyncCollector<UpdateParkrunSpecialEventsMessage> messageCollector)
        {
            var specialEvents = _parser.Parse(cancellationsStream);
            _logger.LogInformation("Parsed {Count} special events", specialEvents.Count);

            foreach (var specialEvent in specialEvents)
            {
                _logger.LogInformation("Sending update parkrun special event message for {ParkrunName}", specialEvent.WebsitePath);

                var message = _mapper.Map<UpdateParkrunSpecialEventsMessage>(specialEvent);

                await messageCollector.AddAsync(message)
                    .ConfigureAwait(false);
            }
        }
    }
}
