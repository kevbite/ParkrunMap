using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ParkrunMap.Domain;

namespace ParkrunMap.FunctionsApp.QueryParkrunsByBox
{
    public class ParkrunFeaturesResponseObject
    {
        [JsonProperty(PropertyName = "wheelchairFriendly")]
        public bool? WheelchairFriendly { get; set; }

        [JsonProperty(PropertyName = "buggyFriendly")]
        public bool? BuggyFriendly { get; set; }

        [JsonProperty(PropertyName = "visuallyImpairedFriendly")]
        public bool? VisuallyImpairedFriendly { get; set; }

        [JsonProperty(PropertyName = "toilets")]
        public bool? Toilets { get; set; }

        [JsonProperty(PropertyName = "dogsAllowed")]
        public bool? DogsAllowed { get; set; }

        [JsonProperty(PropertyName = "cafe")]
        public bool? Cafe { get; set; }

        [JsonProperty(PropertyName = "postRunCoffee")]
        public bool? PostRunCoffee { get; set; }

        [JsonProperty(PropertyName = "drinkingFountain")]
        public bool? DrinkingFountain { get; set; }

        [JsonProperty(PropertyName = "changingRooms")]
        public bool? ChangingRooms { get; set; }

        [JsonProperty(PropertyName = "lockers")]
        public bool? Lockers { get; set; }

        [JsonProperty(PropertyName = "showers")]
        public bool? Showers { get; set; }

        [JsonProperty(PropertyName = "bagDrop")]
        public bool? BagDrop { get; set; }

        [JsonProperty(PropertyName = "babyChangingFacilities")]
        public bool? BabyChangingFacilities { get; set; }

        [JsonProperty(PropertyName = "carParking")]
        public bool? CarParking { get; set; }

        [JsonProperty(PropertyName = "cycleParking")]
        public bool? CycleParking { get; set; }

        [JsonProperty(PropertyName = "carParkingOptions", ItemConverterType = typeof(StringEnumConverter))]
        public IReadOnlyCollection<CarParkingOption> CarParkingOptions { get; set; }

        [JsonProperty(PropertyName = "cycleParkingOptions", ItemConverterType = typeof(StringEnumConverter))]
        public IReadOnlyCollection<CycleParkingOption> CycleParkingOptions { get; set; }

        [JsonProperty(PropertyName = "recommendedBuggy", ItemConverterType = typeof(StringEnumConverter))]
        public IReadOnlyCollection<BuggyType> RecommendedBuggy { get; set; }
    }
}