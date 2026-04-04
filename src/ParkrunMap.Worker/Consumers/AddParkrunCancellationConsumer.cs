using MassTransit;
using ParkrunMap.Contracts;
using ParkrunMap.Data.Mongo.Commands;
using ParkrunMap.Domain;

namespace ParkrunMap.Worker.Consumers;

public class AddParkrunCancellationConsumer(AddCancellationCommand command, ILogger<AddParkrunCancellationConsumer> logger)
    : IConsumer<AddParkrunCancellation>
{
    public async Task Consume(ConsumeContext<AddParkrunCancellation> context)
    {
        var msg = context.Message;
        logger.LogDebug("Adding cancellation for {Domain}{Path} on {Date}", msg.Domain, msg.Path, msg.Date);

        var cancellation = new Cancellation { Date = msg.Date, Reason = msg.Reason };
        await command.ExecuteAsync(msg.Domain, msg.Path, cancellation, context.CancellationToken);
    }
}
