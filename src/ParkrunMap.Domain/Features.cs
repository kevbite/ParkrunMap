using System.Collections.Generic;

namespace ParkrunMap.Domain
{
    public class Features
    {
        public Features()
        {
            CarParkingOptions = new CarParkingOption[0];
            CycleParkingOptions = new CycleParkingOption[0];
            RecommendedBuggy = new BuggyType[0];
        }

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