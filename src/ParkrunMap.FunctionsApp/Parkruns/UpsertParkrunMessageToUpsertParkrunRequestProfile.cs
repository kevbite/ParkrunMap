using System;
using AutoMapper;

namespace ParkrunMap.FunctionsApp.Parkruns
{
    public class UpsertParkrunMessageToUpsertParkrunRequestProfile : Profile
    {
        public UpsertParkrunMessageToUpsertParkrunRequestProfile()
        {
            CreateMap<UpsertParkrunMessage, Data.Mongo.UpsertParkrun.Request>();
        }
    }
}
