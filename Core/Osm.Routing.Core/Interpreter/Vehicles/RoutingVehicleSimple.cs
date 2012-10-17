using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.Core.Interpreter.Vehicles;

namespace Osm.Routing.Core.Interpreter
{
    /// <summary>
    /// Interpreters properties about a given vehicle.
    /// </summary>
    public class RoutingVehicleSimple : RoutingVehicleBase
    {
        /// <summary>
        /// Holds the vehicle.
        /// </summary>
        private VehicleEnum _vehicle;

        /// <summary>
        /// Creates a new vehicle interpreter.
        /// </summary>
        /// <param name="vehicle"></param>
        public RoutingVehicleSimple(VehicleEnum vehicle)
        {
            _vehicle = vehicle;
        }

        /// <summary>
        /// Returns the configured vehicle.
        /// </summary>
        public VehicleEnum Vehicle
        {
            get
            {
                return _vehicle;
            }
        }

        #region Vehicle Properties

        /// <summary>
        /// Returns true if the vehicle is a motor vehicle.
        /// </summary>
        public override bool IsMotorVehicle
        {
            get
            {
                switch (_vehicle)
                {
                    case VehicleEnum.Bike:
                    case VehicleEnum.Pedestrian:
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Returns the simple vehicle category.
        /// </summary>
        public override VehicleEnum VehicleCategory
        {
            get
            {
                return _vehicle;
            }
        }

        #endregion

    }
}
