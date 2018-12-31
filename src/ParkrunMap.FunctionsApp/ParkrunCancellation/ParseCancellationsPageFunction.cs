using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ParkrunMap.Scraping.Cancellations;

namespace ParkrunMap.FunctionsApp.ParkrunCancellation
{
    public class ParseCancellationsPageFunction
    {
        private readonly ILogger _logger;
        private readonly CancellationsParser _parser;
        private readonly IMapper _mapper;

        public ParseCancellationsPageFunction(ILogger logger, CancellationsParser parser, IMapper mapper)
        {
            _logger = logger;
            _parser = parser;
            _mapper = mapper;
        }

        [FunctionName("ParseCancellationsPageFunction")]
        public static async Task Run([BlobTrigger(DownloadFilePaths.CancellationsHtml, Connection = "AzureWebJobsStorage")]Stream cancellationsStream,
            [Queue(QueueNames.AddParkrunCancellation, Connection = "AzureWebJobsStorage")]
            IAsyncCollector<AddParkrunCancellationMessage> messageCollector,
            ILogger logger)
        {
            var func = Container.Instance.Resolve<ParseCancellationsPageFunction>(logger);

            await func.Run(cancellationsStream, messageCollector)
                .ConfigureAwait(false);
        }

        private async Task Run(Stream cancellationsStream, IAsyncCollector<AddParkrunCancellationMessage> messageCollector)
        {
            var cancellations = _parser.Parse(cancellationsStream);
            _logger.LogInformation("Parsed {Count} cancellations", cancellations.Count);

            foreach (var cancellation in cancellations)
            {
                _logger.LogInformation("Sending add parkrun cancellation message for {ParkrunName}", cancellation.Name);

                var message = _mapper.Map<AddParkrunCancellationMessage>(cancellation);

                await messageCollector.AddAsync(message)
                    .ConfigureAwait(false);
            }
        }
    }
}
