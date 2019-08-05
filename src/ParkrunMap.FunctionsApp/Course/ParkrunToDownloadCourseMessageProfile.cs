using AutoMapper;
using ParkrunMap.Data.Mongo;

namespace ParkrunMap.FunctionsApp.Course
{
    public class ParkrunToDownloadCourseMessageProfile : Profile
    {
        public ParkrunToDownloadCourseMessageProfile()
        {
            CreateMap<QueryFirstParkrunForWebsite.Response.ParkrunResponse, DownloadCourseMessage>();
        }
    }
}