using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using Azure.Data.Tables;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
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
            TableClient courseDownloadsStartedTable,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            await Container.Instance.Resolve<QueueUpCourseDownloadsFunction>(logger)
                .Run(collector, courseDownloadsStartedTable, cancellationToken);
        }

        private async Task Run(IAsyncCollector<DownloadCourseMessage> collector,
            TableClient courseDownloadsStartedTable, CancellationToken cancellationToken)
        {
            var alreadyStarted = await GetAlreadyStarted(courseDownloadsStartedTable).ConfigureAwait(false);
            var response =
                await _mediator.Send(
                    new QueryFirstParkrunForWebsite.Request()
                        { ExceptIds = alreadyStarted.GetAllStartedIds().Select(ObjectId.Parse).ToArray() },
                    cancellationToken);

            if (response.Parkrun != null)
            {
                _logger.LogInformation("Queueing up parkrun '{PakrunId}' to download course page", response.Parkrun.Id);
                var message = _mapper.Map<DownloadCourseMessage>(response.Parkrun);

                await collector.AddAsync(message, cancellationToken)
                    .ConfigureAwait(false);

                alreadyStarted.AddStartedId(response.Parkrun.Id.ToString());

                await courseDownloadsStartedTable
                    .UpsertEntityAsync(alreadyStarted, cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            else
            {
                _logger.LogInformation("No parkruns left to queue up to download course page");
            }
        }

        private async Task<AlreadyStartedTableEntity> GetAlreadyStarted(TableClient courseDownloadsStartedTable)
        {
            var dateTime = DateTime.UtcNow;
            var partitionKey = CreateParitionKey(dateTime);
            var rowKey = CreateRowKey(dateTime);

            var retrievedResult =
                await courseDownloadsStartedTable.GetEntityIfExistsAsync<AlreadyStartedTableEntity>(partitionKey,
                    rowKey);

            return retrievedResult.HasValue
                ? retrievedResult.Value
                : new AlreadyStartedTableEntity()
                {
                    PartitionKey = partitionKey,
                    RowKey = rowKey
                };
        }

        private static string CreateParitionKey(DateTime utcNow)
        {
            return $"{utcNow:yyyy}";
        }

        private static string CreateRowKey(DateTime utcNow)
        {
            return $"{utcNow:MMMM}";
        }
    }

    public class AlreadyStartedTableEntity : ITableEntity
    {
        public AlreadyStartedTableEntity()
        {
            StartedIds = string.Empty;
        }

        public string StartedIds { get; set; }

        public void AddStartedId(string id)
        {
            if (StartedIds == string.Empty)
            {
                StartedIds = id;
            }
            else
            {
                StartedIds += $",{id}";
            }
        }

        public IReadOnlyCollection<string> GetAllStartedIds()
        {
            return StartedIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}