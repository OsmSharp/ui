using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Osm;
using OsmSharp.Units.Speed;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing
{
    /// <summary>
    /// Represents a moped
    /// </summary>
    public class Moped : MotorVehicle
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public Moped()
        {
            AccessibleTags.Remove("motorway");
            AccessibleTags.Remove("motorway_link");
        }

        /// <summary>
        /// Returns the maximum possible speed this vehicle can achieve.
        /// </summary>
        /// <returns></returns>
        protected override KilometerPerHour MaxSpeed()
        {
            return 40;
        }

        /// <summary>
        /// Returns a unique name this vehicle type.
        /// </summary>
        public override string UniqueName
        {
            get { return "Moped"; }
        }
    }
}
