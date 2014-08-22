using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Osm;
using OsmSharp.Units.Speed;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing
{
    /// <summary>
    /// Represents a MotorCycle
    /// </summary>
    public class MotorCycle : MotorVehicle
    {
        /// <summary>
        /// Returns a unique name this vehicle type.
        /// </summary>
        public override string UniqueName
        {
            get { return "MotorCycle"; }
        }
    }
}
