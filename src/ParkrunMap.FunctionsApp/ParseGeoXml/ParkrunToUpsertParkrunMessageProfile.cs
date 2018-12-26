using AutoMapper;
using ParkrunMap.FunctionsApp.UpsertParkrun;
using ParkrunMap.Scraping.Parkruns;

namespace ParkrunMap.FunctionsApp.ParseGeoXml
{
    public class ParkrunToUpsertParkrunMessageProfile : Profile
    {
        public ParkrunToUpsertParkrunMessageProfile()
        {
            CreateMap<Parkrun, UpsertParkrunMessage>();
        }
    }
}
