using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ParkrunMap.FunctionsApp.Course
{
    public class UpdateCourseDetailsFunction
    {
        [FunctionName("UpdateCourseDetailsFunction")]
        public static void Run([QueueTrigger("update-course-details", Connection = "AzureWebJobsStorage")]UpdateCourseDetailsMessage message, ILogger logger)
        {
            logger.LogInformation("C# Queue trigger function processed: {message}", message);
        }
    }
}
