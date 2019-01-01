using AutoMapper;
using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.FunctionsApp.Parkruns
{
    public class ParkrunToUpsertParkrunMessageProfile : Profile
    {
        public ParkrunToUpsertParkrunMessageProfile()
        {
            CreateMap<Parkrun, UpsertParkrunMessage>();
        }
    }
}
