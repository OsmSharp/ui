using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Osm;
using OsmSharp.Units.Speed;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing
{
    /// <summary>
    /// Represents a SmallTruck
    /// </summary>
    public class SmallTruck : MotorVehicle
    {
        /// <summary>
        /// Returns a unique name this vehicle type.
        /// </summary>
        public override string UniqueName
        {
            get { return "SmallTruck"; }
        }
        public override bool CanTraverse(TagsCollectionBase tags)
        {
            bool firstCheck = base.CanTraverse(tags);
            if (!firstCheck) return false;
            // Now Check if weight /height / length is allowed


            return true;
        }
    }
}
