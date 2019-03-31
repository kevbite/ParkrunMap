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

        public QuestionnaireResponseDownloader(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<QuestionnaireResponse>> DownloadQuestionnaireResponsesAsync()
        {
            var apiKey = "";
            var sheetId = "1ukN9-5t3s9heuL0bEMYvGkwIustlMRrWXwT9hRVV-_U";
            var sheetName = "Form%20Responses%201";
            
            var responseMessage =
                await _httpClient.GetAsync(
                    $"https://sheets.googleapis.com/v4/spreadsheets/{sheetId}/values/{sheetName}?key={apiKey}");

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

                    return new QuestionnaireResponse(name, websiteDomain, websitePath, x[4], x[5], x[6], x[7], x[8],
                        x[9], x[10], x[11], x[12], x[13], x[14], x[15], x[16], x[17], x[20], x[21], x[22]);
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
