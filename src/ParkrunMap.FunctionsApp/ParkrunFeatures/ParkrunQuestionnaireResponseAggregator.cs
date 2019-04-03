using System;
using System.Collections.Generic;
using System.Linq;
using ParkrunMap.Domain;

namespace ParkrunMap.FunctionsApp.ParkrunFeatures
{
    public class ParkrunQuestionnaireResponseAggregator
    {
        private static readonly IReadOnlyDictionary<string, bool> YesNoAnswerMap =
            new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "Yes", true },
            { "No", false }
        };

        private static readonly IDictionary<string, CarParkingOption> CarParkingOptionMap =
            new Dictionary<string, CarParkingOption>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"Free Car Park", CarParkingOption.FreeCarPark},
                {"Free Street Parking", CarParkingOption.FreeStreetParking},
                {"Paid Car Park", CarParkingOption.PaidCarPark},
                {"Paid Street Parking", CarParkingOption.PaidStreetParking}
            };

        private static readonly IDictionary<string, CycleParkingOption> CycleParkingOptionMap =
            new Dictionary<string, CycleParkingOption>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"Open Cycle Racks", CycleParkingOption.OpenCycleRacks},
                {"Covered Cycle Racks", CycleParkingOption.CoveredCycleRacks},
                {"Indoor Cycle Racks", CycleParkingOption.IndoorCycleRacks},
                {"Open Parking (Fence, Post, Railings)", CycleParkingOption.OpenParking}
            };

        private static readonly IReadOnlyDictionary<string, BuggyType[]> BuggyTypeMap =
            new Dictionary<string, BuggyType[]>(StringComparer.InvariantCultureIgnoreCase)
            {
                {"Cross Country Buggy", new[] {BuggyType.CrossCountry}},
                {"Running Buggy", new[] {BuggyType.Running, BuggyType.CrossCountry}},
                {"Any Buggy", new[] {BuggyType.Running, BuggyType.CrossCountry, BuggyType.Standard}}
            };

        private static readonly IReadOnlyDictionary<string, TerrainType> TerrainTypeMap =
            new Dictionary<string, TerrainType>()
        {
            { "Road", TerrainType.Road},
            { "Grass/Field", TerrainType.Grass},
            { "Trail", TerrainType.Trail},
            { "Turf", TerrainType.Turf},
            { "Track", TerrainType.Track},
            { "Beach", TerrainType.Beach},
        };

        public ParkrunQuestionnaireResponseAggregation Aggregate(ParkrunQuestionnaireResponsesMessage message)
        {
            var wheelchairFriendly =
                AggregateWheelchairFriendlyAnswers(message.Responses.Select(y => y.WheelchairFriendlyAnswer));
            var buggyFriendly = AggregateBuggyFriendlyAnswers(message.Responses.Select(y => y.BuggyFriendlyAnswer));
            var terrainTypes = AggregateTerrainAnswers(message.Responses.Select(y => y.TerrainAnswer));
            var hasToilets = AggregateHasToiletsAnswers(message.Responses.Select(y => y.HasToiletsAnswer));
            var dogsAllowed = AggregateDogsAllowedAnswers(message.Responses.Select(y => y.DogsAllowedAnswer));
            var hasCafe = AggregateHasCafeAnswers(message.Responses.Select(y => y.CafeAnswer));
            var hasPostRunCoffee = AggregatePostRunCoffeeAnswers(message.Responses.Select(y => y.PostRunCoffeeAnswer));
            var hasDrinkingFountains =
                AggregateDrinkingFountainsAnswers(message.Responses.Select(y => y.DrinkingFountainsAnswer));
            var hasChangingRooms = AggregateChangingRoomsAnswers(message.Responses.Select(y => y.ChangingRoomsAnswer));
            var hasLockers = AggregateLockersAvailableAnswers(message.Responses.Select(y => y.LockersAvailableAnswer));
            var hasShowers = AggregateShowersAvailableAnswers(message.Responses.Select(y => y.ShowersAvailableAnswer));
            var carParking = AggregateCarParkingAnswers(message.Responses.Select(y => y.CarParkingAnswer));

            var cycleParking = AggregateCycleParkingAnswers(message.Responses.Select(y => y.CycleParkingAnswer));
            var hasBagDrop = AggregateBagDropAnswers(message.Responses.Select(y => y.BagDropAnswer));
            var hasBabyChangingFacilities =
                AggregateBabyChangingFacilitiesAnswers(message.Responses.Select(y => y.BabyChangingFacilitiesAnswer));
            var visuallyImpairedFriendly =
                AggregateVisuallyImpairedFriendlyAnswers(
                    message.Responses.Select(y => y.VisuallyImpairedFriendlyAnswer));
            var buggyTypes = AggregateTypeOfBuggyAnswers(message.Responses.Select(y => y.TypeOfBuggyAnswer));

            return new ParkrunQuestionnaireResponseAggregation()
            {
                WebsiteDomain = message.Responses.First().WebsiteDomain,
                WebsitePath = message.Responses.First().WebsitePath,
                WheelchairFriendly = wheelchairFriendly,
                BuggyFriendly = buggyFriendly,
                VisuallyImpairedFriendly = visuallyImpairedFriendly,
                Toilets = hasToilets,
                DogsAllowed = dogsAllowed,
                Cafe = hasCafe,
                PostRunCoffee = hasPostRunCoffee,
                DrinkingFountain = hasDrinkingFountains,
                ChangingRooms = hasChangingRooms,
                Lockers = hasLockers,
                Showers = hasShowers,
                BagDrop = hasBagDrop,
                BabyChangingFacilities = hasBabyChangingFacilities,
                CarParking = carParking.hasParking,
                CarParkingOptions = carParking.options,
                CycleParking = cycleParking.hasParking,
                CycleParkingOptions = cycleParking.options,
                RecommendedBuggy = buggyTypes,
                Terrain = terrainTypes,
            };
        }

        private static bool? AggregateYesNoAnswer(IEnumerable<string> answers)
        {
            var mostPopularAnswer = answers.Where(x => YesNoAnswerMap.ContainsKey(x))
                .GroupBy(x => x)
                .OrderByDescending(x => x.Count())
                .Select(x => x.Key)
                .FirstOrDefault();

            if (mostPopularAnswer == null)
            {
                return null;
            }

            return YesNoAnswerMap[mostPopularAnswer];
        }


        private static IReadOnlyCollection<BuggyType> AggregateTypeOfBuggyAnswers(IEnumerable<string> answers)
        {
            var list = new List<BuggyType>();
            foreach (var answer in answers)
            {
                if (BuggyTypeMap.TryGetValue(answer, out var value))
                {
                    list.AddRange(value);
                }
            }

            return list.Distinct().ToArray();
        }

        private static bool? AggregateVisuallyImpairedFriendlyAnswers(IEnumerable<string> answers)
        {
            return AggregateYesNoAnswer(answers);
        }

        private static bool? AggregateBabyChangingFacilitiesAnswers(IEnumerable<string> answers)
        {
            return AggregateYesNoAnswer(answers);
        }

        private static bool? AggregateBagDropAnswers(IEnumerable<string> answers)
        {
            return AggregateYesNoAnswer(answers);
        }

        private static (bool? hasParking, IReadOnlyCollection<CycleParkingOption> options) AggregateCycleParkingAnswers(IEnumerable<string> answers)
        {
            if (answers.Any(x => x.Contains("No Cycle Parking")))
            {
                return (false, new CycleParkingOption[0]);
            }

            var cycleParkingOptions = CycleParkingOptionMap.Keys
                .Where(x => answers.Any(y => y.Contains(x)))
                .Select(x => CycleParkingOptionMap[x])
                .ToArray();

            if (cycleParkingOptions.Any())
            {
                return (true, cycleParkingOptions);
            }

            return ((bool?)null, new CycleParkingOption[0]);
        }



        private static (bool? hasParking, IReadOnlyCollection<CarParkingOption> options) AggregateCarParkingAnswers(IEnumerable<string> answers)
        {
            if (answers.Any(x => x.Contains("No Car Parking")))
            {
                return (false, new CarParkingOption[0]);
            }

            var carParkingOptions = CarParkingOptionMap.Keys
                .Where(x => answers.Any(y => y.Contains(x)))
                .Select(x => CarParkingOptionMap[x])
                .ToArray();

            if (carParkingOptions.Any())
            {
                return (true, carParkingOptions);
            }

            return ((bool?) null, new CarParkingOption[0]);
        }

        private static bool? AggregateShowersAvailableAnswers(IEnumerable<string> answers)
        {
            return AggregateYesNoAnswer(answers);
        }

        private static bool? AggregateLockersAvailableAnswers(IEnumerable<string> answers)
        {
            return AggregateYesNoAnswer(answers);
        }

        private static bool? AggregateChangingRoomsAnswers(IEnumerable<string> answers)
        {
            return AggregateYesNoAnswer(answers);
        }

        private static bool? AggregateDrinkingFountainsAnswers(IEnumerable<string> answers)
        {
            return AggregateYesNoAnswer(answers);
        }

        private static bool? AggregatePostRunCoffeeAnswers(IEnumerable<string> answers)
        {
            return AggregateYesNoAnswer(answers);
        }

        private static bool? AggregateHasCafeAnswers(IEnumerable<string> answers)
        {
            return AggregateYesNoAnswer(answers);
        }

        private static bool? AggregateDogsAllowedAnswers(IEnumerable<string> answers)
        {
            return AggregateYesNoAnswer(answers);
        }

        private static bool? AggregateHasToiletsAnswers(IEnumerable<string> answers)
        {
            return AggregateYesNoAnswer(answers);
        }

        private static IReadOnlyCollection<TerrainType> AggregateTerrainAnswers(IEnumerable<string> answers)
        {
            return TerrainTypeMap.Keys.Where(key => answers.Any(x => x.Contains(key)))
                .Select(key => TerrainTypeMap[key]).ToList();
        }

        private static bool? AggregateBuggyFriendlyAnswers(IEnumerable<string> answers)
        {
            return AggregateYesNoAnswer(answers);
        }

        private static bool? AggregateWheelchairFriendlyAnswers(IEnumerable<string> answers)
        {
            return AggregateYesNoAnswer(answers);
        }
    }
}