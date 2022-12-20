using AutoMapper;
using ParkrunMap.Data.Mongo;

namespace ParkrunMap.FunctionsApp.SpecialEvents
{
    public class UpdateParkrunSpecialEventsMessageToUpdateParkrunSpecialEventsRequestProfile : Profile
    {
        public UpdateParkrunSpecialEventsMessageToUpdateParkrunSpecialEventsRequestProfile()
        {
            CreateMap<UpdateParkrunSpecialEventsMessage, UpdateParkrunSpecialEvents.Request>();
        }
    }
}