using System.Collections.Generic;

namespace ParkrunMap.FunctionsApp.Course
{
    public class UpdateCourseDetailsMessage
    {
        public string WebsiteDomain { get; set; }

        public string WebsitePath { get; set; }

        public string Description { get; set; }

        public IReadOnlyCollection<string> GoogleMapIds { get; set; }
    }
}