using MassTransit;
using ParkrunMap.Contracts;
using ParkrunMap.Data.Mongo.Commands;

namespace ParkrunMap.Worker.Consumers;

public class UpdateParkrunSpecialEventsConsumer(UpdateSpecialEventsCommand command, ILogger<UpdateParkrunSpecialEventsConsumer> logger)
    : IConsumer<UpdateParkrunSpecialEvents>
{
    public async Task Consume(ConsumeContext<UpdateParkrunSpecialEvents> context)
    {
        var msg = context.Message;
        logger.LogDebug("Updating special events for {Domain}{Path} year {Year}", msg.Domain, msg.Path, msg.Year);

        await command.ExecuteAsync(
            msg.Domain, msg.Path, msg.Year,
            msg.ChristmasDayRun, msg.NewYearsDayRun,
            context.CancellationToken);
    }
}
