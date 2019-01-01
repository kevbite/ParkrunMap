using AutoMapper;

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
