using AutoMapper;
using ParkrunMap.Data.Mongo;

namespace ParkrunMap.FunctionsApp.ParkrunCancellation
{
    public class AddParkrunCancellationMessageToAddParkrunCancellationRequestProfile : Profile
    {
        public AddParkrunCancellationMessageToAddParkrunCancellationRequestProfile()
        {
            CreateMap<AddParkrunCancellationMessage, AddParkrunCancellation.Request>();
        }
    }
}