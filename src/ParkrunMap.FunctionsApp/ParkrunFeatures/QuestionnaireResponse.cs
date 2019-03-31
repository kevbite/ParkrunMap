namespace ParkrunMap.FunctionsApp.ParkrunFeatures
{
    public class QuestionnaireResponse
    {
        public string ParkrunName { get; set; }

        public string WebsiteDomain { get; set; }

        public string WebsitePath { get; set; }

        public string WheelchairFriendlyAnswer { get; set; }

        public string BuggyFriendlyAnswer { get; set; }

        public string TerrainAnswer { get; set; }

        public string HasToiletsAnswer { get; set; }

        public string DogsAllowedAnswer { get; set; }

        public string CafeAnswer { get; set; }

        public string PostRunCoffeeAnswer { get; set; }

        public string DrinkingFountainsAnswer { get; set; }

        public string ChangingRoomsAnswer { get; set; }

        public string LockersAvailableAnswer { get; set; }

        public string ShowersAvailableAnswer { get; set; }

        public string CarParkingAnswer { get; set; }

        public string CycleParkingAnswer { get; set; }

        public string BagDropAnswer { get; set; }

        public string BabyChangingFacilitiesAnswer { get; set; }

        public string VisuallyImpairedFriendlyAnswer { get; set; }

        public string TypeOfBuggyAnswer { get; set; }

        public QuestionnaireResponse(string parkrunName,
            string websiteDomain,
            string websitePath,
            string wheelchairFriendlyAnswer,
            string buggyFriendlyAnswer,
            string terrainAnswer,
            string hasToiletsAnswer,
            string dogsAllowedAnswer,
            string cafeAnswer,
            string postRunCoffeeAnswer,
            string drinkingFountainsAnswer,
            string changingRoomsAnswer,
            string lockersAvailableAnswer,
            string showersAvailableAnswer,
            string carParkingAnswer,
            string cycleParkingAnswer,
            string bagDropAnswer,
            string babyChangingFacilitiesAnswer,
            string visuallyImpairedFriendlyAnswer,
            string typeOfBuggyAnswer)
        {
            ParkrunName = parkrunName;
            WebsiteDomain = websiteDomain;
            WebsitePath = websitePath;
            WheelchairFriendlyAnswer = wheelchairFriendlyAnswer;
            BuggyFriendlyAnswer = buggyFriendlyAnswer;
            TerrainAnswer = terrainAnswer;
            HasToiletsAnswer = hasToiletsAnswer;
            DogsAllowedAnswer = dogsAllowedAnswer;
            CafeAnswer = cafeAnswer;
            PostRunCoffeeAnswer = postRunCoffeeAnswer;
            DrinkingFountainsAnswer = drinkingFountainsAnswer;
            ChangingRoomsAnswer = changingRoomsAnswer;
            LockersAvailableAnswer = lockersAvailableAnswer;
            ShowersAvailableAnswer = showersAvailableAnswer;
            CarParkingAnswer = carParkingAnswer;
            CycleParkingAnswer = cycleParkingAnswer;
            BagDropAnswer = bagDropAnswer;
            BabyChangingFacilitiesAnswer = babyChangingFacilitiesAnswer;
            VisuallyImpairedFriendlyAnswer = visuallyImpairedFriendlyAnswer;
            TypeOfBuggyAnswer = typeOfBuggyAnswer;
        }
    }
}