using MassTransit;
using MongoDB.Driver.GeoJsonObjectModel;
using ParkrunMap.Contracts;
using ParkrunMap.Data.Mongo.Commands;
using ParkrunMap.Domain;

namespace ParkrunMap.Worker.Consumers;

public class UpsertParkrunConsumer(UpsertParkrunCommand command, ILogger<UpsertParkrunConsumer> logger)
    : IConsumer<UpsertParkrun>
{
    public async Task Consume(ConsumeContext<UpsertParkrun> context)
    {
        var msg = context.Message;
        logger.LogDebug("Upserting parkrun {Name} ({Domain}{Path})", msg.Name, msg.Domain, msg.Path);

        var parkrun = new Parkrun
        {
            GeoXmlId = msg.GeoXmlId,
            Name = msg.Name,
            Website = new Website { Domain = msg.Domain, Path = msg.Path },
            Region = msg.Region,
            Country = msg.Country,
            Location = GeoJson.Point(GeoJson.Geographic(msg.Longitude, msg.Latitude))
        };

        await command.ExecuteAsync(parkrun, context.CancellationToken);
    }
}
