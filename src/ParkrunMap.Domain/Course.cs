using System;
using System.Collections.Generic;

namespace ParkrunMap.Domain
{
    public class Course
    {
        public Course()
        {
            Terrain = new TerrainType[0];
        }

        public string Description { get; set; }

        public IReadOnlyCollection<string> GoogleMapIds { get; set; }

        public IReadOnlyCollection<TerrainType> Terrain { get; set; }
    }
}
