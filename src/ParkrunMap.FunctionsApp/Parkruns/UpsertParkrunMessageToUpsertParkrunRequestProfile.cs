using AutoMapper;

namespace ParkrunMap.FunctionsApp.UpsertParkrun
{
    public class UpsertParkrunMessageToUpsertParkrunRequestProfile : Profile
    {
        public UpsertParkrunMessageToUpsertParkrunRequestProfile()
        {
            CreateMap<UpsertParkrunMessage, Data.Mongo.UpsertParkrun.Request>();
        }
    }
}
