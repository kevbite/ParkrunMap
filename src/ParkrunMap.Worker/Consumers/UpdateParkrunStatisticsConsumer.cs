using MassTransit;
using ParkrunMap.Contracts;
using ParkrunMap.Data.Mongo.Commands;
using ParkrunMap.Domain;

namespace ParkrunMap.Worker.Consumers;

public class UpdateParkrunStatisticsConsumer(UpdateStatisticsCommand command, ILogger<UpdateParkrunStatisticsConsumer> logger)
    : IConsumer<UpdateParkrunStatistics>
{
    public async Task Consume(ConsumeContext<UpdateParkrunStatistics> context)
    {
        var msg = context.Message;
        logger.LogDebug("Updating statistics for {Domain}{Path}", msg.Domain, msg.Path);

        var stats = new Statistics
        {
            TotalEvents = msg.TotalEvents,
            TotalRunners = msg.TotalRunners,
            TotalRuns = msg.TotalRuns,
            AverageRunnersPerWeek = msg.AverageRunnersPerWeek,
            LargestEvent = msg.LargestEvent,
            SmallestEvent = msg.SmallestEvent,
            NumberOfVolunteers = msg.NumberOfVolunteers,
            TotalVolunteers = msg.TotalVolunteers,
            TotalKidsRuns = msg.TotalKidsRuns
        };

        await command.ExecuteAsync(msg.Domain, msg.Path, stats, context.CancellationToken);
    }
}
