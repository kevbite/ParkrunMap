using System.Collections.Generic;
using ParkrunMap.Domain;

namespace ParkrunMap.FunctionsApp.ParkrunFeatures
{

    public class ParkrunQuestionnaireResponseAggregation
    {
        public string WebsiteDomain { get; set; }

        public string WebsitePath { get; set; }

        public IReadOnlyCollection<TerrainType> Terrain { get; set; }

        public bool? WheelchairFriendly { get; set; }

        public bool? BuggyFriendly { get; set; }

        public bool? VisuallyImpairedFriendly { get; set; }

        public bool? Toilets { get; set; }

        public bool? DogsAllowed { get; set; }

        public bool? Cafe { get; set; }

        public bool? PostRunCoffee { get; set; }

        public bool? DrinkingFountain { get; set; }

        public bool? ChangingRooms { get; set; }

        public bool? Lockers { get; set; }

        public bool? Showers { get; set; }

        public bool? BagDrop { get; set; }

        public bool? BabyChangingFacilities { get; set; }

        public bool? CarParking { get; set; }

        public bool? CycleParking { get; set; }

        public IReadOnlyCollection<CarParkingOption> CarParkingOptions { get; set; }

        public IReadOnlyCollection<CycleParkingOption> CycleParkingOptions { get; set; }

        public IReadOnlyCollection<BuggyType> RecommendedBuggy { get; set; }
    }
}
