using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using AutoFixture;
using FluentAssertions;
using ParkrunMap.Domain;
using ParkrunMap.FunctionsApp.ParkrunFeatures;
using Xunit;

namespace ParkrunMap.FunctionsApp.Tests.ParkrunFeatures
{
    public class ParkrunQuestionnaireResponseAggregatorTests
    {
        private readonly Fixture _fixture;
        private readonly ParkrunQuestionnaireResponseAggregator _aggregator;

        public ParkrunQuestionnaireResponseAggregatorTests()
        {
            _fixture = new Fixture();
            _aggregator = new ParkrunQuestionnaireResponseAggregator();
        }

        [Fact]
        public void ShouldReturnNullsAndEmptyListsForUnknownAnswers()
        {
            var aggregator = new ParkrunQuestionnaireResponseAggregator();
            
            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .Create();

            var aggregation = aggregator.Aggregate(message);

            aggregation.Should().BeEquivalentTo(new
            {
                WheelchairFriendly = (bool?)null,
                BuggyFriendly = (bool?)null,
                VisuallyImpairedFriendly = (bool?)null,
                Toilets = (bool?)null,
                DogsAllowed = (bool?)null,
                Cafe = (bool?)null,
                PostRunCoffee = (bool?)null,
                DrinkingFountain = (bool?)null,
                ChangingRooms = (bool?)null,
                Lockers = (bool?)null,
                Showers = (bool?)null,
                BagDrop = (bool?)null,
                BabyChangingFacilities = (bool?)null,
                CarParking = (bool?)null,
                CycleParking = (bool?)null,
                CarParkingOptions = new CarParkingOption[0],
                CycleParkingOptions = new CycleParkingOption[0],
                RecommendedBuggy = new BuggyType[0],
                Terrain = new TerrainType[0]
            });

        }

        [Fact]
        public void ShouldReturnTrueForAggregatedYesNoQuestions()
        {
            var response = _fixture.Build<QuestionnaireResponse>()
                .WithAllYesAnswers()
                .Create();
            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .With(x => x.Responses, new []{ response })
                .Create();

            var aggregation = _aggregator.Aggregate(message);
     
            aggregation.Should().BeEquivalentTo(new {
                WheelchairFriendly = true,
                BuggyFriendly = true,
                VisuallyImpairedFriendly = true,
                Toilets = true,
                DogsAllowed = true,
                Cafe = true,
                PostRunCoffee = true,
                DrinkingFountain  =true,
                ChangingRooms = true,
                Lockers = true,
                Showers =true,
                BagDrop = true,
                BabyChangingFacilities = true,
            });
        }


        [Fact]
        public void ShouldReturnFalseForAggregatedYesNoQuestions()
        {
           var response = _fixture.Build<QuestionnaireResponse>()
                .WithAllNoAnswers()
                .Create();
            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .With(x => x.Responses, new[] { response })
                .Create();

            var aggregation = _aggregator.Aggregate(message);

            aggregation.Should().BeEquivalentTo(new
            {
                WheelchairFriendly = false,
                BuggyFriendly = false,
                VisuallyImpairedFriendly = false,
                Toilets = false,
                DogsAllowed = false,
                Cafe = false,
                PostRunCoffee = false,
                DrinkingFountain = false,
                ChangingRooms = false,
                Lockers = false,
                Showers = false,
                BagDrop = false,
                BabyChangingFacilities = false,
            });

        }

        [Fact]
        public void ShouldReturnTrueForAggregatedMultipleYesNoQuestion()
        {
            var yesResponses = _fixture.Build<QuestionnaireResponse>()
                .WithAllYesAnswers()
                .Create();
            var noResponses = _fixture.Build<QuestionnaireResponse>()
                .WithAllNoAnswers()
                .Create();
            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .With(x => x.Responses, new[] { noResponses, yesResponses, yesResponses })
                .Create();

            var aggregation = _aggregator.Aggregate(message);

            aggregation.Should().BeEquivalentTo(new
            {
                WheelchairFriendly = true,
                BuggyFriendly = true,
                VisuallyImpairedFriendly = true,
                Toilets = true,
                DogsAllowed = true,
                Cafe = true,
                PostRunCoffee = true,
                DrinkingFountain = true,
                ChangingRooms = true,
                Lockers = true,
                Showers = true,
                BagDrop = true,
                BabyChangingFacilities = true,
            });

        }

        [Fact]
        public void ShouldReturnFalseForAggregatedMultipleYesNoQuestion()
        {
            var yesResponses = _fixture.Build<QuestionnaireResponse>()
                .WithAllYesAnswers()
                .Create();
            var noResponses = _fixture.Build<QuestionnaireResponse>()
                .WithAllNoAnswers()
                .Create();
            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .With(x => x.Responses, new[] { yesResponses, noResponses, noResponses })
                .Create();

            var aggregation = _aggregator.Aggregate(message);

            aggregation.Should().BeEquivalentTo(new
            {
                WheelchairFriendly = false,
                BuggyFriendly = false,
                VisuallyImpairedFriendly = false,
                Toilets = false,
                DogsAllowed = false,
                Cafe = false,
                PostRunCoffee = false,
                DrinkingFountain = false,
                ChangingRooms = false,
                Lockers = false,
                Showers = false,
                BagDrop = false,
                BabyChangingFacilities = false,
            });

        }

        [Theory]
        [InlineData("Road", TerrainType.Road)]
        [InlineData("Trail", TerrainType.Trail)]
        [InlineData("Track", TerrainType.Track)]
        [InlineData("Grass/Field", TerrainType.Grass)]
        [InlineData("Turf", TerrainType.Turf)]
        [InlineData("Beach", TerrainType.Beach)]
        public void ShouldReturnSingleTerrain(string terrainAnswer, TerrainType terrainType)
        {
            var responses = _fixture.Build<QuestionnaireResponse>()
                .With(x => x.TerrainAnswer, terrainAnswer)
                .Create();
            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .With(x => x.Responses, new[] { responses })
                .Create();

            var aggregation = _aggregator.Aggregate(message);

            aggregation.Terrain.Should().BeEquivalentTo(terrainType);
        }

        [Theory]
        [InlineData("Road, Track", TerrainType.Road, TerrainType.Track)]
        [InlineData("Grass/Field, Trail, Turf, Track", TerrainType.Grass, TerrainType.Trail, TerrainType.Turf, TerrainType.Track)]
        public void ShouldReturnMultipleTerrain(string terrainAnswer, params TerrainType[] terrainTypes)
        {
            var responses = _fixture.Build<QuestionnaireResponse>()
                .With(x => x.TerrainAnswer, terrainAnswer)
                .Create();
            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .With(x => x.Responses, new[] { responses })
                .Create();

            var aggregation = _aggregator.Aggregate(message);

            aggregation.Terrain.Should().BeEquivalentTo(terrainTypes);
        }

        [Fact]
        public void ShouldReturnMultipleTerrainForMultipleResponses()
        {
            var terrainAnswers = new[] {"Road", "Grass/Field", "Trail"};
            var responses = terrainAnswers.Select(x =>
            {
                return _fixture.Build<QuestionnaireResponse>()
                    .With(y => y.TerrainAnswer, x)
                    .Create();
            }).ToArray();
            
            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .With(x => x.Responses, responses )
                .Create();

            var aggregation = _aggregator.Aggregate(message);

            aggregation.Terrain.Should().BeEquivalentTo(new []
            {
                TerrainType.Road,
                TerrainType.Grass,
                TerrainType.Trail
            });
        }

        [Fact]
        public void ShouldReturnNoCarParkingWithEmptyCarParkingOptions()
        {
            var response = _fixture.Build<QuestionnaireResponse>()
                .With(x => x.CarParkingAnswer, "No Car Parking")
                .Create();

            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .With(x => x.Responses, new []{ response })
                .Create();

            var aggregation = _aggregator.Aggregate(message);

            aggregation.Should().BeEquivalentTo(new
            {
                CarParking = false,
                CarParkingOptions = new CarParkingOption[0]
            });
        }

        [Theory]
        [InlineData("Free Car Park", CarParkingOption.FreeCarPark)]
        [InlineData("Free Street Parking", CarParkingOption.FreeStreetParking)]
        [InlineData("Paid Car Park", CarParkingOption.PaidCarPark)]
        [InlineData("Paid Street Parking", CarParkingOption.PaidStreetParking)]
        public void ShouldReturnSingleCarParkingOption(string carParkingAnswer, CarParkingOption expected)
        {
            var response = _fixture.Build<QuestionnaireResponse>()
                .With(x => x.CarParkingAnswer, carParkingAnswer)
                .Create();

            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .With(x => x.Responses, new[] { response })
                .Create();

            var aggregation = _aggregator.Aggregate(message);

            aggregation.Should().BeEquivalentTo(new
            {
                CarParking = true,
                CarParkingOptions = new[] { expected }
            });
        }

        [Theory]
        [InlineData("Free Car Park, Free Street Parking", CarParkingOption.FreeCarPark, CarParkingOption.FreeStreetParking)]
        [InlineData("Paid Car Park, Free Street Parking", CarParkingOption.PaidCarPark, CarParkingOption.FreeStreetParking)]
        public void ShouldReturnMultipleCarParkingOptions(string carParkingAnswer, params CarParkingOption[] expected)
        {
            var response = _fixture.Build<QuestionnaireResponse>()
                .With(x => x.CarParkingAnswer, carParkingAnswer)
                .Create();

            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .With(x => x.Responses, new[] { response })
                .Create();

            var aggregation = _aggregator.Aggregate(message);

            aggregation.Should().BeEquivalentTo(new
            {
                CarParking = true,
                CarParkingOptions = expected
            });
        }

     
        [Fact]
        public void ShouldReturnMultipleCarParkingOptionsForMultipleResponses()
        {
            var response1 = _fixture.Build<QuestionnaireResponse>()
                .With(x => x.CarParkingAnswer, "Free Car Park, Free Street Parking")
                .Create();
            var response2 = _fixture.Build<QuestionnaireResponse>()
                .With(x => x.CarParkingAnswer, "Paid Car Park")
                .Create();

            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .With(x => x.Responses, new[] { response1, response2 })
                .Create();

            var aggregation = _aggregator.Aggregate(message);

            aggregation.Should().BeEquivalentTo(new
            {
                CarParking = true,
                CarParkingOptions = new[]
                {
                    CarParkingOption.FreeCarPark,
                    CarParkingOption.FreeStreetParking,
                    CarParkingOption.PaidCarPark
                }
            });
        }

        [Fact]
        public void ShouldReturnNoCycleParkingWithEmptyCycleParkingOptions()
        {
            var response = _fixture.Build<QuestionnaireResponse>()
                .With(x => x.CycleParkingAnswer, "No Cycle Parking")
                .Create();

            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .With(x => x.Responses, new[] { response })
                .Create();

            var aggregation = _aggregator.Aggregate(message);

            aggregation.Should().BeEquivalentTo(new
            {
                CycleParking = false,
                CycleParkingOptions = new CycleParkingOption[0]
            });
        }

        [Theory]
        [InlineData("Open Cycle Racks", CycleParkingOption.OpenCycleRacks)]
        [InlineData("Covered Cycle Racks", CycleParkingOption.CoveredCycleRacks)]
        [InlineData("Indoor Cycle Racks", CycleParkingOption.IndoorCycleRacks)]
        [InlineData("Open Parking (Fence, Post, Railings)", CycleParkingOption.OpenParking)]
        public void ShouldReturnSingleCycleParkingOption(string cycleParkingAnswer, CycleParkingOption expected)
        {
            var response = _fixture.Build<QuestionnaireResponse>()
                .With(x => x.CycleParkingAnswer, cycleParkingAnswer)
                .Create();

            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .With(x => x.Responses, new[] { response })
                .Create();

            var aggregation = _aggregator.Aggregate(message);

            aggregation.Should().BeEquivalentTo(new
            {
                CycleParking = true,
                CycleParkingOptions = new[] { expected }
            });
        }

        [Theory]
        [InlineData("Open Cycle Racks, Open Parking (Fence, Post, Railings)", CycleParkingOption.OpenCycleRacks, CycleParkingOption.OpenParking)]
        [InlineData("Covered Cycle Racks, Indoor Cycle Racks, Open Parking (Fence, Post, Railings)", CycleParkingOption.CoveredCycleRacks, CycleParkingOption.IndoorCycleRacks, CycleParkingOption.OpenParking)]
        public void ShouldReturnMultipleCycleParkingOptions(string cycleParkingAnswer, params CycleParkingOption[] expected)
        {
            var response = _fixture.Build<QuestionnaireResponse>()
                .With(x => x.CycleParkingAnswer, cycleParkingAnswer)
                .Create();

            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .With(x => x.Responses, new[] { response })
                .Create();

            var aggregation = _aggregator.Aggregate(message);

            aggregation.Should().BeEquivalentTo(new
            {
                CycleParking = true,
                CycleParkingOptions = expected
            });
        }

        [Fact]
        public void ShouldReturnMultipleCycleParkingOptionsForMultipleResponses()
        {
            var response1 = _fixture.Build<QuestionnaireResponse>()
                .With(x => x.CycleParkingAnswer, "Open Cycle Racks, Open Parking (Fence, Post, Railings)")
                .Create();
            var response2 = _fixture.Build<QuestionnaireResponse>()
                .With(x => x.CycleParkingAnswer, "Indoor Cycle Racks")
                .Create();

            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .With(x => x.Responses, new[] { response1, response2 })
                .Create();

            var aggregation = _aggregator.Aggregate(message);

            aggregation.Should().BeEquivalentTo(new
            {
                CycleParking = true,
                CycleParkingOptions = new[]
                {
                    CycleParkingOption.OpenCycleRacks,
                    CycleParkingOption.OpenParking,
                    CycleParkingOption.IndoorCycleRacks
                }
            });
        }

        [Theory]
        [InlineData("Cross Country Buggy", BuggyType.CrossCountry)]
        [InlineData("Running Buggy", BuggyType.Running, BuggyType.CrossCountry)]
        [InlineData("Any Buggy", BuggyType.Running, BuggyType.CrossCountry, BuggyType.Standard)]
        public void ShouldReturnRecommendedBuggy(string typeOfBuggyAnswer, params BuggyType[] buggyTypes)
        {
            var responses = _fixture.Build<QuestionnaireResponse>()
                .With(x => x.TypeOfBuggyAnswer, typeOfBuggyAnswer)
                .Create();
            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .With(x => x.Responses, new[] { responses })
                .Create();

            var aggregation = _aggregator.Aggregate(message);

            aggregation.RecommendedBuggy.Should().BeEquivalentTo(buggyTypes);
        }

        [Fact]
        public void ShouldReturnRecommendedBuggyForMultipleResponses()
        {
            var response1 = _fixture.Build<QuestionnaireResponse>()
                .With(x => x.TypeOfBuggyAnswer, "Cross Country Buggy")
                .Create();

            var response2 = _fixture.Build<QuestionnaireResponse>()
                .With(x => x.TypeOfBuggyAnswer, "Running Buggy")
                .Create();

            var message = _fixture.Build<ParkrunQuestionnaireResponsesMessage>()
                .With(x => x.Responses, new[] { response1, response2 })
                .Create();

            var aggregation = _aggregator.Aggregate(message);

            aggregation.RecommendedBuggy.Should().BeEquivalentTo(BuggyType.Running, BuggyType.CrossCountry);
        }
    }
}
