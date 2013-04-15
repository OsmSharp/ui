//// OsmSharp - OpenStreetMap tools & library.
//// Copyright (C) 2012 Abelshausen Ben
//// 
//// This file is part of OsmSharp.
//// 
//// OsmSharp is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 2 of the License, or
//// (at your option) any later version.
//// 
//// OsmSharp is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License
//// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OsmSharp.Routing.Interpreter.Vehicles;

//namespace OsmSharp.Routing.Interpreter
//{
//    /// <summary>
//    /// Interpreters properties about a given vehicle.
//    /// </summary>
//    public class RoutingVehicleSimple : RoutingVehicleBase
//    {
//        /// <summary>
//        /// Holds the vehicle.
//        /// </summary>
//        private VehicleEnum _vehicle;

//        /// <summary>
//        /// Creates a new vehicle interpreter.
//        /// </summary>
//        /// <param name="vehicle"></param>
//        public RoutingVehicleSimple(VehicleEnum vehicle)
//        {
//            _vehicle = vehicle;
//        }

//        /// <summary>
//        /// Returns the configured vehicle.
//        /// </summary>
//        public VehicleEnum Vehicle
//        {
//            get
//            {
//                return _vehicle;
//            }
//        }

//        #region Vehicle Properties

//        /// <summary>
//        /// Returns true if the vehicle is a motor vehicle.
//        /// </summary>
//        public override bool IsMotorVehicle
//        {
//            get
//            {
//                switch (_vehicle)
//                {
//                    case VehicleEnum.Bike:
//                    case VehicleEnum.Pedestrian:
//                        return false;
//                }
//                return true;
//            }
//        }

//        /// <summary>
//        /// Returns the simple vehicle category.
//        /// </summary>
//        public override VehicleEnum VehicleCategory
//        {
//            get
//            {
//                return _vehicle;
//            }
//        }

//        #endregion

//    }
//}
