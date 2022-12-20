using AutoMapper;
using ParkrunMap.Scraping.SpecialEvents;

namespace ParkrunMap.FunctionsApp.SpecialEvents
{
    public class SpecialEventToUpdateParkrunSpecialEventsMessageProfile : Profile
    {
        public SpecialEventToUpdateParkrunSpecialEventsMessageProfile()
        {
            CreateMap<SpecialEvent, UpdateParkrunSpecialEventsMessage>();
        }
    }
}