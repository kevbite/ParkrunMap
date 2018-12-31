using AutoMapper;
using ParkrunMap.FunctionsApp.AddParkrunCancellation;

namespace ParkrunMap.FunctionsApp.ParkrunCancellation
{
    public class ParkrunCancellationToUpsertParkrunCancellationMessageProfile : Profile
    {
        public ParkrunCancellationToUpsertParkrunCancellationMessageProfile()
        {
            CreateMap<Scraping.Cancellations.ParkrunCancellation, AddParkrunCancellationMessage>();
        }
    }
}
