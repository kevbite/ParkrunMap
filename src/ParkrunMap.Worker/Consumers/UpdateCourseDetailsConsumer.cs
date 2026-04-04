using MassTransit;
using ParkrunMap.Contracts;
using ParkrunMap.Data.Mongo.Commands;
using ParkrunMap.Domain;

namespace ParkrunMap.Worker.Consumers;

public class UpdateCourseDetailsConsumer(UpdateCourseDetailsCommand command, ILogger<UpdateCourseDetailsConsumer> logger)
    : IConsumer<UpdateCourseDetails>
{
    public async Task Consume(ConsumeContext<UpdateCourseDetails> context)
    {
        var msg = context.Message;
        logger.LogDebug("Updating course details for {Domain}{Path}", msg.Domain, msg.Path);

        var course = new Course
        {
            Description = msg.Description,
            GoogleMapIds = [..msg.GoogleMapIds],
            Terrain = msg.Terrain
                .Select(t => Enum.TryParse<TerrainType>(t, ignoreCase: true, out var tt) ? tt : (TerrainType?)null)
                .Where(t => t.HasValue)
                .Select(t => t!.Value)
                .ToList()
        };

        await command.ExecuteAsync(msg.Domain, msg.Path, course, context.CancellationToken);
    }
}
