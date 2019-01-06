namespace ParkrunMap.Scraping.Course
{
    public class CourseDetails
    {
        public CourseDetails(string description, string googleMapId)
        {
            Description = description;
            GoogleMapId = googleMapId;
        }

        public string Description { get; }

        public string GoogleMapId { get; }
    }
}