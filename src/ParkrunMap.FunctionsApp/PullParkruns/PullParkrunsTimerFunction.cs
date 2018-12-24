using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ParkrunMap.FunctionsApp.UpsertParkrun;
using ParkrunMap.Scraping;
using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.FunctionsApp.PullParkruns
{
    public class PullParkrunsTimerFunction
    {
        private readonly ILogger _logger;
        private readonly ParkrunScraper _parkrunScraper;
        private readonly IMapper _mapper;

        public PullParkrunsTimerFunction(ILogger logger, ParkrunScraper parkrunScraper, IMapper mapper)
        {
            _logger = logger;
            _parkrunScraper = parkrunScraper;
            _mapper = mapper;
        }

        [FunctionName("PullParkrunsTimerFunction")]
        public static async Task Run([TimerTrigger("0 2 * * *", RunOnStartup=true)]TimerInfo myTimer, [Queue("parkrun-upserts", Connection = "AzureWebJobsStorage")] ICollector<UpsertParkrunMessage> messageCollector, ILogger logger)
        {
            var function1 = Container.Instance.Resolve<PullParkrunsTimerFunction>(logger);
            await function1.Run(messageCollector)
                .ConfigureAwait(false);
        }
    
        private async Task Run(ICollector<UpsertParkrunMessage> messageCollector)
        {
            var parkruns = await _parkrunScraper.GetParkruns()
                .ConfigureAwait(false);

            _logger.LogInformation("Scrapped {Count} parkruns", parkruns.Count);

            foreach (var parkrun in parkruns)
            {
                _logger.LogInformation("Sending upsert parkrun message for ", parkrun.Name);

                var message = _mapper.Map<UpsertParkrunMessage>(parkrun);

                messageCollector.Add(message);
            }
        }
    }
}
