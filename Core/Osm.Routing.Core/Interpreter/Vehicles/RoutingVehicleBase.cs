using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.Core.Interpreter.Vehicles
{
    /// <summary>
    /// Represents a routing vehicle with all it's properties.
    /// </summary>
    public abstract class RoutingVehicleBase
    {
        /// <summary>
        /// Returns a simpler vehicle category.
        /// </summary>
        /// <remarks>TODO: Find a way to get this depricated and make a more complex relationship between vehicles and ways.</remarks>
        public abstract VehicleEnum VehicleCategory
        {
            get;
        }

        /// <summary>
        /// Returns true if the vehicle is a motor vehicle.
        /// </summary>
        public abstract bool IsMotorVehicle
        {
            get;
        }
    }
}
