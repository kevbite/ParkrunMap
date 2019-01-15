using System.Collections.Generic;

namespace ParkrunMap.Scraping.Course
{
    public class CourseDetails
    {
        public CourseDetails(string description, IReadOnlyCollection<string> googleMapIds)
        {
            Description = description;
            GoogleMapIds = googleMapIds;
        }

        public string Description { get; }

        public IReadOnlyCollection<string> GoogleMapIds { get; }
    }
}