using AutoFixture.Dsl;
using ParkrunMap.FunctionsApp.ParkrunFeatures;

namespace ParkrunMap.FunctionsApp.Tests.ParkrunFeatures
{
    public static class QuestionnaireResponsePostprocessComposerExtentions
    {
        public static IPostprocessComposer<QuestionnaireResponse> WithAllYesAnswers(
            this IPostprocessComposer<QuestionnaireResponse> composer)
        {
            var value = "Yes";

            return composer
                .With(x => x.WheelchairFriendlyAnswer, value)
                .With(x => x.BuggyFriendlyAnswer, value)
                .With(x => x.HasToiletsAnswer, value)
                .With(x => x.DogsAllowedAnswer, value)
                .With(x => x.CafeAnswer, value)
                .With(x => x.PostRunCoffeeAnswer, value)
                .With(x => x.DrinkingFountainsAnswer, value)
                .With(x => x.ChangingRoomsAnswer, value)
                .With(x => x.LockersAvailableAnswer, value)
                .With(x => x.ShowersAvailableAnswer, value)
                .With(x => x.BagDropAnswer, value)
                .With(x => x.BabyChangingFacilitiesAnswer, value)
                .With(x => x.VisuallyImpairedFriendlyAnswer, value);
        }

        public static IPostprocessComposer<QuestionnaireResponse> WithAllNoAnswers(
            this IPostprocessComposer<QuestionnaireResponse> composer)
        {
            var value = "No";

            return composer
                .With(x => x.WheelchairFriendlyAnswer, value)
                .With(x => x.BuggyFriendlyAnswer, value)
                .With(x => x.HasToiletsAnswer, value)
                .With(x => x.DogsAllowedAnswer, value)
                .With(x => x.CafeAnswer, value)
                .With(x => x.PostRunCoffeeAnswer, value)
                .With(x => x.DrinkingFountainsAnswer, value)
                .With(x => x.ChangingRoomsAnswer, value)
                .With(x => x.LockersAvailableAnswer, value)
                .With(x => x.ShowersAvailableAnswer, value)
                .With(x => x.BagDropAnswer, value)
                .With(x => x.BabyChangingFacilitiesAnswer, value)
                .With(x => x.VisuallyImpairedFriendlyAnswer, value);
        }
    }
}