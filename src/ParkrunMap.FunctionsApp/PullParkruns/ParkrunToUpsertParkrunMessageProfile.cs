using AutoMapper;
using ParkrunMap.FunctionsApp.UpsertParkrun;
using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.FunctionsApp.PullParkruns
{
    public class ParkrunToUpsertParkrunMessageProfile : Profile
    {
        public ParkrunToUpsertParkrunMessageProfile()
        {
            CreateMap<Parkrun, UpsertParkrunMessage>();
        }
    }
}
