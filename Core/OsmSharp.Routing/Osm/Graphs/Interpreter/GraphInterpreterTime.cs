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
//using OsmSharp.Tools.Math.Units.Distance;
//using OsmSharp.Tools.Math.Units.Speed;
//using OsmSharp.Osm;
//using OsmSharp.Tools.Math.Graph;
//using OsmSharp.Tools.Math.Geo;
//using OsmSharp.Routing.Osm.Core;
//using OsmSharp.Osm.Data;
//using OsmSharp.Routing.Osm.Core.Interpreter;

//namespace OsmSharp.Routing.Osm.Graphs.Interpreter
//{
//    /// <summary>
//    /// Interpreter return time as the weight for the graph.
//    /// </summary>
//    public class GraphInterpreterTime : GraphInterpreterBase
//    {
//        /// <summary>
//        /// Creates a new graph interpreter.
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="vehicle_type"></param>
//        public GraphInterpreterTime(RoutingInterpreterBase routing_interpreter, IDataSourceReadOnly source, VehicleEnum vehicle_type)
//            : base(routing_interpreter, source, vehicle_type)
//        {

//        }

//        ///// <summary>
//        ///// Calculate the weight in time.
//        ///// </summary>
//        ///// <param name="way"></param>
//        ///// <param name="from"></param>
//        ///// <param name="to"></param>
//        ///// <returns></returns>
//        //protected override float CalculateWeight(Osm.Way way, GeoCoordinate from, GeoCoordinate to)
//        //{
//        //    //Meter distance = from.DistanceReal(to);
//        //    Meter distance = from.DistanceEstimate(to);

//        //    //Second time = null;
//        //    KilometerPerHour speed = null;

//        //    KilometerPerHour pedestrian_speed = 5;
//        //    KilometerPerHour bike_speed = 15;

//        //    HighwayTypeEnum highway_type = way.HighwayType;
//        //    switch (highway_type)
//        //    {
//        //        case HighwayTypeEnum.service:
//        //        case HighwayTypeEnum.proposed:
//        //        case HighwayTypeEnum.cycleway:
//        //        case HighwayTypeEnum.pedestrian:
//        //        case HighwayTypeEnum.living_Street:
//        //            switch (this.Vehicle)
//        //            {
//        //                case VehicleEnum.Bike:
//        //                    speed = bike_speed;
//        //                    break;
//        //                case VehicleEnum.Pedestrian:
//        //                    speed = pedestrian_speed;
//        //                    break;
//        //                case VehicleEnum.Car:
//        //                case VehicleEnum.Bus:
//        //                    speed = pedestrian_speed;
//        //                    break;
//        //            }
//        //            break;
//        //        case HighwayTypeEnum.track:
//        //            switch (this.Vehicle)
//        //            {
//        //                case VehicleEnum.Bike:
//        //                    speed = bike_speed;
//        //                    break;
//        //                case VehicleEnum.Pedestrian:
//        //                    speed = pedestrian_speed;
//        //                    break;
//        //                case VehicleEnum.Car:
//        //                case VehicleEnum.Bus:
//        //                    speed = 40;
//        //                    break;
//        //            }
//        //            break;
//        //        case HighwayTypeEnum.residential:
//        //            switch (this.Vehicle)
//        //            {
//        //                case VehicleEnum.Bike:
//        //                    speed = bike_speed;
//        //                    break;
//        //                case VehicleEnum.Pedestrian:
//        //                    speed = pedestrian_speed;
//        //                    break;
//        //                case VehicleEnum.Car:
//        //                case VehicleEnum.Bus:
//        //                    speed = 50;
//        //                    break;
//        //            }
//        //            break;
//        //        case HighwayTypeEnum.motorway:
//        //            switch (this.Vehicle)
//        //            {
//        //                case VehicleEnum.Bike:
//        //                    speed = bike_speed;
//        //                    break;
//        //                case VehicleEnum.Pedestrian:
//        //                    speed = pedestrian_speed;
//        //                    break;
//        //                case VehicleEnum.Car:
//        //                case VehicleEnum.Bus:
//        //                    speed = 120;
//        //                    break;
//        //            }
//        //            break;
//        //        case HighwayTypeEnum.trunk:
//        //        case HighwayTypeEnum.primary:
//        //            switch (this.Vehicle)
//        //            {
//        //                case VehicleEnum.Bike:
//        //                    speed = bike_speed;
//        //                    break;
//        //                case VehicleEnum.Pedestrian:
//        //                    speed = pedestrian_speed;
//        //                    break;
//        //                case VehicleEnum.Car:
//        //                case VehicleEnum.Bus:
//        //                    speed = 90;
//        //                    break;
//        //            }
//        //            break;
//        //        default:
//        //            switch (this.Vehicle)
//        //            {
//        //                case VehicleEnum.Bike:
//        //                    speed = bike_speed;
//        //                    break;
//        //                case VehicleEnum.Pedestrian:
//        //                    speed = pedestrian_speed;
//        //                    break;
//        //                case VehicleEnum.Car:
//        //                case VehicleEnum.Bus:
//        //                    speed = 70;
//        //                    break;
//        //            }
//        //            break;
//        //    }


//        //    // construct weight class.
//        //    //double time_value = (distance.Value / speed.Value);
//        //    //double time_value = (distance/ speed).Value;
//        //    //double distance_value = distance.Value;
//        //    double time_value = (distance.Value / speed.Value) * 3.6; //in sec

//        //    return (float)time_value;
//        //}

//        ///// <summary>
//        ///// Underestimate the weight in time.
//        ///// </summary>
//        ///// <param name="from"></param>
//        ///// <param name="to"></param>
//        ///// <returns></returns>
//        //protected override float UnderestimateWeight(GeoCoordinate from, GeoCoordinate to)
//        //{
//        //    Meter distance = from.DistanceReal(to);

//        //    KilometerPerHour speed = null;

//        //    KilometerPerHour pedestrian_speed = 5;
//        //    KilometerPerHour bike_speed = 15;

//        //    switch (this.Vehicle)
//        //    {
//        //        case VehicleEnum.Bike:
//        //            speed = bike_speed;
//        //            break;
//        //        case VehicleEnum.Pedestrian:
//        //            speed = pedestrian_speed;
//        //            break;
//        //        case VehicleEnum.Car:
//        //        case VehicleEnum.Bus:
//        //            speed = 30;
//        //            break;
//        //    }

//        //    double time_value = (distance.Value / speed.Value);
//        //    double distance_value = distance.Value;

//        //    //System.Threading.Thread.Sleep(100);

//        //    //return 0;
//        //    return (float)time_value;
//        //}

//        /// <summary>
//        /// Calculate the weight in time.
//        /// </summary>
//        /// <param name="way"></param>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        protected override float CalculateWeight(Osm.Way way, GeoCoordinate from, GeoCoordinate to)
//        {
//            //Meter distance = from.DistanceReal(to);
//            Meter distance = from.DistanceEstimate(to);

//            //Second time = null;
//            KilometerPerHour speed = null;

//            KilometerPerHour pedestrian_speed = 5;
//            KilometerPerHour bike_speed = 15;

//            string highway_type = way.Tags["highway"];
//            switch (highway_type)
//            {
//                case "services":
//                case "proposed":
//                case "cycleway":
//                case "pedestrian":
//                case "steps":
//                case "path":
//                case "footway":
//                case "living_street":
//                    switch (this.Vehicle)
//                    {
//                        case VehicleEnum.Bike:
//                            speed = bike_speed;
//                            break;
//                        case VehicleEnum.Pedestrian:
//                            speed = pedestrian_speed;
//                            break;
//                        case VehicleEnum.Car:
//                        case VehicleEnum.Bus:
//                            speed = pedestrian_speed;
//                            break;
//                    }
//                    break;
//                case "track":
//                    switch (this.Vehicle)
//                    {
//                        case VehicleEnum.Bike:
//                            speed = bike_speed;
//                            break;
//                        case VehicleEnum.Pedestrian:
//                            speed = pedestrian_speed;
//                            break;
//                        case VehicleEnum.Car:
//                        case VehicleEnum.Bus:
//                            speed = 40;
//                            break;
//                    }
//                    break;
//                case "residential":
//                    switch (this.Vehicle)
//                    {
//                        case VehicleEnum.Bike:
//                            speed = bike_speed;
//                            break;
//                        case VehicleEnum.Pedestrian:
//                            speed = pedestrian_speed;
//                            break;
//                        case VehicleEnum.Car:
//                        case VehicleEnum.Bus:
//                            speed = 50;
//                            break;
//                    }
//                    break;
//                case "motorway":
//                case "motorway_link":
//                    switch (this.Vehicle)
//                    {
//                        case VehicleEnum.Bike:
//                            speed = bike_speed;
//                            break;
//                        case VehicleEnum.Pedestrian:
//                            speed = pedestrian_speed;
//                            break;
//                        case VehicleEnum.Car:
//                        case VehicleEnum.Bus:
//                            speed = 120;
//                            break;
//                    }
//                    break;
//                case "trunk":
//                case "trunk_link":
//                case "primary":
//                case "primary_link":
//                    switch (this.Vehicle)
//                    {
//                        case VehicleEnum.Bike:
//                            speed = bike_speed;
//                            break;
//                        case VehicleEnum.Pedestrian:
//                            speed = pedestrian_speed;
//                            break;
//                        case VehicleEnum.Car:
//                        case VehicleEnum.Bus:
//                            speed = 90;
//                            break;
//                    }
//                    break;
//                default:
//                    switch (this.Vehicle)
//                    {
//                        case VehicleEnum.Bike:
//                            speed = bike_speed;
//                            break;
//                        case VehicleEnum.Pedestrian:
//                            speed = pedestrian_speed;
//                            break;
//                        case VehicleEnum.Car:
//                        case VehicleEnum.Bus:
//                            speed = 70;
//                            break;
//                    }
//                    break;
//            }


//            // construct weight class.
//            //double time_value = (distance.Value / speed.Value);
//            //double time_value = (distance/ speed).Value;
//            //double distance_value = distance.Value;
//            double time_value = (distance.Value / speed.Value) * 3.6; //in sec

//            return (float)time_value;
//        }

//        /// <summary>
//        /// Underestimate the weight in time.
//        /// </summary>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        protected override float UnderestimateWeight(GeoCoordinate from, GeoCoordinate to)
//        {
//            Meter distance = from.DistanceReal(to);

//            KilometerPerHour speed = null;

//            KilometerPerHour pedestrian_speed = 5;
//            KilometerPerHour bike_speed = 15;

//            switch (this.Vehicle)
//            {
//                case VehicleEnum.Bike:
//                    speed = bike_speed;
//                    break;
//                case VehicleEnum.Pedestrian:
//                    speed = pedestrian_speed;
//                    break;
//                case VehicleEnum.Car:
//                case VehicleEnum.Bus:
//                    speed = 30;
//                    break;
//            }

//            double time_value = (distance.Value / speed.Value);
//            double distance_value = distance.Value;

//            //System.Threading.Thread.Sleep(100);

//            //return 0;
//            return (float)time_value;
//        }
//    }
//}
