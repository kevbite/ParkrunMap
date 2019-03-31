using System.Collections.Generic;

namespace ParkrunMap.FunctionsApp.ParkrunFeatures
{
    public class ParkrunQuestionnaireResponsesMessage
    {
        public string WebsiteDomain { get; set; }

        public string WebsitePath { get; set; }

        public IReadOnlyCollection<QuestionnaireResponse> Responses { get; set; }
    }
}