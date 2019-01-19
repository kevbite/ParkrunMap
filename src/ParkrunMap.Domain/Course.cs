using System;
using System.Collections.Generic;

namespace ParkrunMap.Domain
{
    public class Course
    {
        public string Description { get; set; }

        public IReadOnlyCollection<string> GoogleMapIds { get; set; }
    }
}
