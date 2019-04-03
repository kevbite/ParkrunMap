using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ParkrunMap.FunctionsApp.ParkrunFeatures
{
    public class QuestionnaireResponseDownloader
    {
        private readonly HttpClient _httpClient;
        private readonly string _googleApiKey;

        public QuestionnaireResponseDownloader(HttpClient httpClient, string googleApiKey)
        {
            _httpClient = httpClient;
            _googleApiKey = googleApiKey;
        }

        public async Task<IReadOnlyCollection<QuestionnaireResponse>> DownloadQuestionnaireResponsesAsync()
        {
            var sheetId = "1ukN9-5t3s9heuL0bEMYvGkwIustlMRrWXwT9hRVV-_U";
            var sheetName = "Form%20Responses%201";
            
            var responseMessage =
                await _httpClient.GetAsync(
                    $"https://sheets.googleapis.com/v4/spreadsheets/{sheetId}/values/{sheetName}?key={_googleApiKey}");

            responseMessage.EnsureSuccessStatusCode();
            var body = await responseMessage.Content.ReadAsStringAsync();

            var queryResponse = JsonConvert.DeserializeObject<SpreadsheetQueryResponse>(body);
            var questionnaireResponses = queryResponse.Values.Skip(1)
                .Select(x =>
                {
                    var match = Regex.Match(x[3], @"(?<name>.+ )\((?<websiteDomain>.+)(?<websitePath>/.+)\)");
                    var name = match.Groups["name"].Value;
                    var websiteDomain = match.Groups["websiteDomain"].Value;
                    var websitePath = match.Groups["websitePath"].Value;
                    
                    var wheelchairFriendlyAnswer = x.ElementAtOrDefault(4) ?? string.Empty;
                    var buggyFriendlyAnswer = x.ElementAtOrDefault(5) ?? string.Empty;
                    var terrainAnswer = x.ElementAtOrDefault(6) ?? string.Empty;
                    var hasToiletsAnswer = x.ElementAtOrDefault(7) ?? string.Empty;
                    var dogsAllowedAnswer = x.ElementAtOrDefault(8) ?? string.Empty;
                    var cafeAnswer = x.ElementAtOrDefault(9) ?? string.Empty;
                    var postRunCoffeeAnswer = x.ElementAtOrDefault(10) ?? string.Empty;
                    var drinkingFountainsAnswer = x.ElementAtOrDefault(11) ?? string.Empty;
                    var changingRoomsAnswer = x.ElementAtOrDefault(12) ?? string.Empty;
                    var lockersAvailableAnswer = x.ElementAtOrDefault(13) ?? string.Empty;
                    var showersAvailableAnswer = x.ElementAtOrDefault(14) ?? string.Empty;
                    var carParkingAnswer = x.ElementAtOrDefault(15) ?? string.Empty;
                    var cycleParkingAnswer = x.ElementAtOrDefault(16) ?? string.Empty;
                    var bagDropAnswer = x.ElementAtOrDefault(17) ?? string.Empty;
                    var babyChangingFacilitiesAnswer = x.ElementAtOrDefault(20) ?? string.Empty;
                    var visuallyImpairedFriendlyAnswer = x.ElementAtOrDefault(21) ?? string.Empty;
                    var typeOfBuggyAnswer = x.ElementAtOrDefault(22) ?? string.Empty;

                    return new QuestionnaireResponse(name, websiteDomain, websitePath, wheelchairFriendlyAnswer,
                        buggyFriendlyAnswer, terrainAnswer, hasToiletsAnswer, dogsAllowedAnswer,
                        cafeAnswer, postRunCoffeeAnswer, drinkingFountainsAnswer, changingRoomsAnswer,
                        lockersAvailableAnswer, showersAvailableAnswer, carParkingAnswer, cycleParkingAnswer,
                        bagDropAnswer, babyChangingFacilitiesAnswer, visuallyImpairedFriendlyAnswer, typeOfBuggyAnswer);

                }).ToArray();

            return questionnaireResponses;
        }

        internal class SpreadsheetQueryResponse
        {
            [JsonProperty("values")]
            public string[][] Values { get; set; }
        }
    }
}
