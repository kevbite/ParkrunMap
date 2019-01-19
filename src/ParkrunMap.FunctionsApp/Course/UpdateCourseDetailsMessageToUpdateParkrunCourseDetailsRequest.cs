using AutoMapper;
using ParkrunMap.Data.Mongo;

namespace ParkrunMap.FunctionsApp.Course
{
    public class UpdateCourseDetailsMessageToUpdateParkrunCourseDetailsRequest : Profile
    {
        public UpdateCourseDetailsMessageToUpdateParkrunCourseDetailsRequest()
        {
            CreateMap<UpdateCourseDetailsMessage, UpdateParkrunCourseDetails.Request>();
        }
    }
}