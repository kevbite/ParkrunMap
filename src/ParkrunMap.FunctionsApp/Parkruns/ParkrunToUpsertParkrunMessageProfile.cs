using AutoMapper;
using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.FunctionsApp.Parkruns
{
    public class EventsJsonParkrunToUpsertParkrunMessageProfile : Profile
    {
        public EventsJsonParkrunToUpsertParkrunMessageProfile()
        {
            CreateMap<EventsJsonParkrun, UpsertParkrunMessage>();
        }
    }
}
