using MassTransit;
using ParkrunMap.Contracts;
using ParkrunMap.Data.Mongo.Commands;
using ParkrunMap.Domain;

namespace ParkrunMap.Worker.Consumers;

public class UpdateParkrunFeaturesConsumer(UpdateFeaturesCommand command, ILogger<UpdateParkrunFeaturesConsumer> logger)
    : IConsumer<UpdateParkrunFeatures>
{
    public async Task Consume(ConsumeContext<UpdateParkrunFeatures> context)
    {
        var msg = context.Message;
        logger.LogDebug("Updating features for {Domain}{Path}", msg.Domain, msg.Path);

        var features = new Features
        {
            WheelchairFriendly = msg.WheelchairFriendly,
            BuggyFriendly = msg.BuggyFriendly,
            VisuallyImpairedFriendly = msg.VisuallyImpairedFriendly,
            Toilets = msg.Toilets,
            DogsAllowed = msg.DogsAllowed,
            Cafe = msg.Cafe,
            PostRunCoffee = msg.PostRunCoffee,
            DrinkingFountain = msg.DrinkingFountain,
            ChangingRooms = msg.ChangingRooms,
            Lockers = msg.Lockers,
            Showers = msg.Showers,
            BagDrop = msg.BagDrop,
            BabyChangingFacilities = msg.BabyChangingFacilities,
            CarParking = msg.CarParking,
            CycleParking = msg.CycleParking,
            CarParkingOptions = [..msg.CarParkingOptions],
            CycleParkingOptions = [..msg.CycleParkingOptions],
            RecommendedBuggy = [..msg.RecommendedBuggy]
        };

        await command.ExecuteAsync(msg.Domain, msg.Path, features, context.CancellationToken);
    }
}
