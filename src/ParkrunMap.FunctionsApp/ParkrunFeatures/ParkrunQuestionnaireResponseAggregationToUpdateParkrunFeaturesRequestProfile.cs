using AutoMapper;
using ParkrunMap.Data.Mongo;

namespace ParkrunMap.FunctionsApp.ParkrunFeatures
{
    public class ParkrunQuestionnaireResponseAggregationToUpdateParkrunFeaturesRequestProfile : Profile
    {
        public ParkrunQuestionnaireResponseAggregationToUpdateParkrunFeaturesRequestProfile()
        {
            CreateMap<ParkrunQuestionnaireResponseAggregation, UpdateParkrunFeatures.Request>();
        }
    }
}
