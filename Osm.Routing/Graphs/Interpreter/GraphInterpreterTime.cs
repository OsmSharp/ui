using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Units.Distance;
using Tools.Math.Units.Speed;
using Osm.Core;
using Tools.Math.Graph;
using Tools.Math.Geo;
using Osm.Routing.Core;
using Osm.Data;

namespace Osm.Routing.Raw.Graphs.Interpreter
{
    /// <summary>
    /// Interpreter return time as the weight for the graph.
    /// </summary>
    public class GraphInterpreterTime : GraphInterpreterBase
    {
        /// <summary>
        /// Creates a new graph interpreter.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="vehicle_type"></param>
        public GraphInterpreterTime(IDataSourceReadOnly source, VehicleEnum vehicle_type)
            : base(source, vehicle_type)
        {

        }

        ///// <summary>
        ///// Calculate the weight in time.
        ///// </summary>
        ///// <param name="way"></param>
        ///// <param name="from"></param>
        ///// <param name="to"></param>
        ///// <returns></returns>
        //protected override float CalculateWeight(Osm.Core.Way way, GeoCoordinate from, GeoCoordinate to)
        //{
        //    //Meter distance = from.DistanceReal(to);
        //    Meter distance = from.DistanceEstimate(to);

        //    //Second time = null;
        //    KilometerPerHour speed = null;

        //    KilometerPerHour pedestrian_speed = 5;
        //    KilometerPerHour bike_speed = 15;

        //    HighwayTypeEnum highway_type = way.HighwayType;
        //    switch (highway_type)
        //    {
        //        case HighwayTypeEnum.service:
        //        case HighwayTypeEnum.proposed:
        //        case HighwayTypeEnum.cycleway:
        //        case HighwayTypeEnum.pedestrian:
        //        case HighwayTypeEnum.living_Street:
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
        //                    speed = pedestrian_speed;
        //                    break;
        //            }
        //            break;
        //        case HighwayTypeEnum.track:
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
        //                    speed = 40;
        //                    break;
        //            }
        //            break;
        //        case HighwayTypeEnum.residential:
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
        //                    speed = 50;
        //                    break;
        //            }
        //            break;
        //        case HighwayTypeEnum.motorway:
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
        //                    speed = 120;
        //                    break;
        //            }
        //            break;
        //        case HighwayTypeEnum.trunk:
        //        case HighwayTypeEnum.primary:
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
        //                    speed = 90;
        //                    break;
        //            }
        //            break;
        //        default:
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
        //                    speed = 70;
        //                    break;
        //            }
        //            break;
        //    }


        //    // construct weight class.
        //    //double time_value = (distance.Value / speed.Value);
        //    //double time_value = (distance/ speed).Value;
        //    //double distance_value = distance.Value;
        //    double time_value = (distance.Value / speed.Value) * 3.6; //in sec

        //    return (float)time_value;
        //}

        ///// <summary>
        ///// Underestimate the weight in time.
        ///// </summary>
        ///// <param name="from"></param>
        ///// <param name="to"></param>
        ///// <returns></returns>
        //protected override float UnderestimateWeight(GeoCoordinate from, GeoCoordinate to)
        //{
        //    Meter distance = from.DistanceReal(to);

        //    KilometerPerHour speed = null;

        //    KilometerPerHour pedestrian_speed = 5;
        //    KilometerPerHour bike_speed = 15;

        //    switch (this.Vehicle)
        //    {
        //        case VehicleEnum.Bike:
        //            speed = bike_speed;
        //            break;
        //        case VehicleEnum.Pedestrian:
        //            speed = pedestrian_speed;
        //            break;
        //        case VehicleEnum.Car:
        //        case VehicleEnum.Bus:
        //            speed = 30;
        //            break;
        //    }

        //    double time_value = (distance.Value / speed.Value);
        //    double distance_value = distance.Value;

        //    //System.Threading.Thread.Sleep(100);

        //    //return 0;
        //    return (float)time_value;
        //}

        /// <summary>
        /// Calculate the weight in time.
        /// </summary>
        /// <param name="way"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        protected override float CalculateWeight(Osm.Core.Way way, GeoCoordinate from, GeoCoordinate to)
        {
            //Meter distance = from.DistanceReal(to);
            Meter distance = from.DistanceEstimate(to);

            //Second time = null;
            KilometerPerHour speed = null;

            KilometerPerHour pedestrian_speed = 5;
            KilometerPerHour bike_speed = 15;

            string highway_type = way.Tags["highway"];
            switch (highway_type)
            {
                case "services":
                case "proposed":
                case "cycleway":
                case "pedestrian":
                case "steps":
                case "path":
                case "footway":
                case "living_street":
                    switch (this.Vehicle)
                    {
                        case VehicleEnum.Bike:
                            speed = bike_speed;
                            break;
                        case VehicleEnum.Pedestrian:
                            speed = pedestrian_speed;
                            break;
                        case VehicleEnum.Car:
                        case VehicleEnum.Bus:
                            speed = pedestrian_speed;
                            break;
                    }
                    break;
                case "track":
                    switch (this.Vehicle)
                    {
                        case VehicleEnum.Bike:
                            speed = bike_speed;
                            break;
                        case VehicleEnum.Pedestrian:
                            speed = pedestrian_speed;
                            break;
                        case VehicleEnum.Car:
                        case VehicleEnum.Bus:
                            speed = 40;
                            break;
                    }
                    break;
                case "residential":
                    switch (this.Vehicle)
                    {
                        case VehicleEnum.Bike:
                            speed = bike_speed;
                            break;
                        case VehicleEnum.Pedestrian:
                            speed = pedestrian_speed;
                            break;
                        case VehicleEnum.Car:
                        case VehicleEnum.Bus:
                            speed = 50;
                            break;
                    }
                    break;
                case "motorway":
                case "motorway_link":
                    switch (this.Vehicle)
                    {
                        case VehicleEnum.Bike:
                            speed = bike_speed;
                            break;
                        case VehicleEnum.Pedestrian:
                            speed = pedestrian_speed;
                            break;
                        case VehicleEnum.Car:
                        case VehicleEnum.Bus:
                            speed = 120;
                            break;
                    }
                    break;
                case "trunk":
                case "trunk_link":
                case "primary":
                case "primary_link":
                    switch (this.Vehicle)
                    {
                        case VehicleEnum.Bike:
                            speed = bike_speed;
                            break;
                        case VehicleEnum.Pedestrian:
                            speed = pedestrian_speed;
                            break;
                        case VehicleEnum.Car:
                        case VehicleEnum.Bus:
                            speed = 90;
                            break;
                    }
                    break;
                default:
                    switch (this.Vehicle)
                    {
                        case VehicleEnum.Bike:
                            speed = bike_speed;
                            break;
                        case VehicleEnum.Pedestrian:
                            speed = pedestrian_speed;
                            break;
                        case VehicleEnum.Car:
                        case VehicleEnum.Bus:
                            speed = 70;
                            break;
                    }
                    break;
            }


            // construct weight class.
            //double time_value = (distance.Value / speed.Value);
            //double time_value = (distance/ speed).Value;
            //double distance_value = distance.Value;
            double time_value = (distance.Value / speed.Value) * 3.6; //in sec

            return (float)time_value;
        }

        /// <summary>
        /// Underestimate the weight in time.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        protected override float UnderestimateWeight(GeoCoordinate from, GeoCoordinate to)
        {
            Meter distance = from.DistanceReal(to);

            KilometerPerHour speed = null;

            KilometerPerHour pedestrian_speed = 5;
            KilometerPerHour bike_speed = 15;

            switch (this.Vehicle)
            {
                case VehicleEnum.Bike:
                    speed = bike_speed;
                    break;
                case VehicleEnum.Pedestrian:
                    speed = pedestrian_speed;
                    break;
                case VehicleEnum.Car:
                case VehicleEnum.Bus:
                    speed = 30;
                    break;
            }

            double time_value = (distance.Value / speed.Value);
            double distance_value = distance.Value;

            //System.Threading.Thread.Sleep(100);

            //return 0;
            return (float)time_value;
        }
    }
}
