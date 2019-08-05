using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using MongoDB.Bson;
using ParkrunMap.Data.Mongo;

namespace ParkrunMap.FunctionsApp.Course
{
    public class QueueUpCourseDownloadsFunction
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public QueueUpCourseDownloadsFunction(IMediator mediator, IMapper mapper, ILogger logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [FunctionName("QueueUpCourseDownloadsFunction")]
        public static async Task Run([TimerTrigger("0 */1 8-23 * * *")] TimerInfo myTimer,
            [Queue(QueueNames.DownloadCoursePage, Connection = "AzureWebJobsStorage")]
            IAsyncCollector<DownloadCourseMessage> collector,
            [Table("TrackedCourseDownloadsStarted")]
            CloudTable courseDownloadsStartedTable,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            await Container.Instance.Resolve<QueueUpCourseDownloadsFunction>(logger).Run(collector, courseDownloadsStartedTable, cancellationToken);
        }

        private async Task Run(IAsyncCollector<DownloadCourseMessage> collector, CloudTable courseDownloadsStartedTable, CancellationToken cancellationToken)
        {
            var alreadyStarted = await GetAlreadyStarted(courseDownloadsStartedTable).ConfigureAwait(false);
            var response = await  _mediator.Send(new QueryFirstParkrunForWebsite.Request(){ ExceptIds = alreadyStarted }, cancellationToken);

            if(response.Parkrun != null)
            {
                _logger.LogInformation("Queueing up parkrun '{PakrunId}' to download course page", response.Parkrun.Id);
                var message = _mapper.Map<DownloadCourseMessage>(response.Parkrun);

                await collector.AddAsync(message, cancellationToken)
                    .ConfigureAwait(false);

                await courseDownloadsStartedTable.ExecuteAsync(TableOperation.Insert(
                        new TableEntity(CreateParitionKey(DateTime.UtcNow), response.Parkrun.Id.ToString())))
                    .ConfigureAwait(false);
            }
            else
            {
                _logger.LogInformation("No parkruns left to queue up to download course page");
                
            }
        }

        private async Task<IReadOnlyCollection<ObjectId>> GetAlreadyStarted(CloudTable courseDownloadsStartedTable)
        {
            var query = new TableQuery<TableEntity>().Where(
                TableQuery.GenerateFilterCondition(
                    "PartitionKey",
                    QueryComparisons.Equal,
                    CreateParitionKey(DateTime.UtcNow)
                )
            );

            var result = await courseDownloadsStartedTable.ExecuteQuerySegmentedAsync(query, null).ConfigureAwait(false);

            return result.Results.Select(x => ObjectId.Parse(x.RowKey)).ToArray();
        }

        private static string CreateParitionKey(DateTime utcNow)
        {
            return $"{utcNow:yyyy-MMMM}";
        }
    }
}
