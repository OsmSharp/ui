using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routing.Core.Interpreter.Roads;
using Routing.Core;
using Tools.Math.Geo;
using Tools.Math.Units.Speed;

namespace Osm.Routing.Interpreter.Edge
{
    /// <summary>
    /// Default edge interpreter.
    /// </summary>
    public class EdgeInterpreter : IEdgeInterpreter
    {
        /// <summary>
        /// Creates a new edge interpreter.
        /// </summary>
        /// <param name="vehicle"></param>
        public EdgeInterpreter()
        {

        }

        /// <summary>
        /// Returns true if the edge with the given tags can be traversed by the given vehicle.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool CanBeTraversedBy(IDictionary<string, string> tags, VehicleEnum vehicle)
        {
            if (tags.ContainsKey("highway"))
            {
                // remove all restricted roads.
                // TODO: include other private roads.
                if (tags.ContainsKey("access"))
                {
                    if (tags["access"] == "private"
                        || tags["access"] == "official")
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                if (vehicle.IsMotorVehicle())
                {
                    if (tags.ContainsKey("motor_vehicle"))
                    {
                        if (tags["motor_vehicle"] == "no")
                        {
                            return false;
                        }
                    }
                }

                switch (vehicle)
                {
                    case VehicleEnum.Car:
                    case VehicleEnum.Bus:
                        if (tags.ContainsKey("bicycle"))
                        {
                            if (tags["bicycle"] == "designated")
                            {
                                return false;
                            }
                        }
                        if (tags.ContainsKey("foot"))
                        {
                            if (tags["foot"] == "designated")
                            {
                                return false;
                            }
                        }
                        break;
                }

                string highway_type = tags["highway"];
                switch (highway_type)
                {
                    case "proposed":
                        //case "service":
                        return false;
                    case "cycleway":
                    case "pedestrian":
                    case "steps":
                    case "path":
                    case "footway":
                        switch (vehicle)
                        {
                            case VehicleEnum.Bike:
                            case VehicleEnum.Pedestrian:
                                break;
                            case VehicleEnum.Car:
                            case VehicleEnum.Bus:
                                return false;
                        }
                        break;
                    case "track":
                        switch (vehicle)
                        {
                            case VehicleEnum.Bike:
                            case VehicleEnum.Pedestrian:
                                break;
                            case VehicleEnum.Car:
                                break;
                            case VehicleEnum.Bus:
                                return false;
                        }
                        break;
                    case "residential":
                        switch (vehicle)
                        {
                            case VehicleEnum.Bike:
                            case VehicleEnum.Car:
                            case VehicleEnum.Pedestrian:
                            case VehicleEnum.Bus:
                                break;
                        }
                        break;
                    case "motorway":
                    case "motorway_link":
                    case "trunk":
                    case "trunk_link":
                    case "primary":
                    case "primary_link":
                        switch (vehicle)
                        {
                            case VehicleEnum.Bike:
                            case VehicleEnum.Pedestrian:
                                return false;
                            case VehicleEnum.Car:
                            case VehicleEnum.Bus:
                                break;
                        }
                        break;
                    default: // service:
                        switch (vehicle)
                        {
                            case VehicleEnum.Bike:
                            case VehicleEnum.Car:
                            case VehicleEnum.Pedestrian:
                            case VehicleEnum.Bus:
                                break;
                        }
                        break;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the edge with the given tags is only accessible locally.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool IsOnlyLocalAccessible(IDictionary<string, string> tags)
        {
            if (tags.ContainsKey("highway")) // TODO: use trygetvalue!
            {
                if (tags["highway"] == "service")
                {
                    return true;
                }
            }
            if (tags.ContainsKey("access")) // TODO: use trygetvalue!
            {
                if (tags["access"] == "private"
                    || tags["access"] == "official")
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the weight between two points on an edge with the given tags for the given vehicle.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="vehicle"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public double Weight(IDictionary<string, string> tags, VehicleEnum vehicle, GeoCoordinate from, GeoCoordinate to)
        {
            double distance = from.DistanceEstimate(to).Value;

            return distance / (this.MaxSpeed(tags, vehicle).Value) * 3.6;
        }

        /// <summary>
        /// Returns true if the edge with the given tags is routable.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool IsRoutable(IDictionary<string, string> tags)
        {
            if (tags != null && tags.Count > 0)
            {
                return tags.ContainsKey("highway");
            }
            return false;
        }

        /// <summary>
        /// Returns true if the edge is one way forward, false if backward, null if bidirectional.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool? IsOneWay(IDictionary<string, string> tags)
        {
            string oneway;
            if (tags.TryGetValue("oneway", out oneway))
            {
                if (oneway == "yes")
                {
                    return true;
                }
                return false;
            }
            return null;
        }

        /// <summary>
        /// Returns the maximum speed.
        /// </summary>
        /// <returns></returns>
        public KilometerPerHour MaxSpeed(IDictionary<string, string> tags, VehicleEnum vehicle)
        {
            // THERE ARE THE MAX SPEEDS FOR BELGIUM. 
            // TODO: Find a way to make this all configurable.
            KilometerPerHour speed = null;

            KilometerPerHour pedestrian_speed = 5;
            KilometerPerHour bike_speed = 15;

            // get max-speed tag if any.
            double? max_speed_value = tags.GetNumericValue("maxspeed");
            if (max_speed_value.HasValue)
            {
                return max_speed_value.Value;
            }

            string highway_type;
            if (tags.TryGetValue("highway", out highway_type))
            {
                switch (highway_type)
                {
                    case "services":
                    case "proposed":
                        switch (vehicle)
                        {
                            case VehicleEnum.Bike:
                                speed = pedestrian_speed;
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
                    case "cycleway":
                    case "pedestrian":
                    case "steps":
                    case "path":
                    case "footway":
                        switch (vehicle)
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
                        switch (vehicle)
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
                        switch (vehicle)
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
                        switch (vehicle)
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
                        switch (vehicle)
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
                        switch (vehicle)
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
            }

            return speed;
        }

        /// <summary>
        /// Returns the name of a given way.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public string GetName(IDictionary<string, string> tags)
        {
            string name = string.Empty;
            if (tags.ContainsKey("name"))
            {
                name = tags["name"];
            }
            return name;
        }

        /// <summary>
        /// Returns all the names in all languages and alternatives.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetNamesInAllLanguages(IDictionary<string, string> tags)
        {
            Dictionary<string, string> names = new Dictionary<string, string>();
            if (tags != null)
            {
                foreach (KeyValuePair<string, string> pair in tags)
                {
                    System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(pair.Key, "name:[a-zA-Z]");
                    if (m.Success)
                    {
                        //throw new NotImplementedException();
                    }
                }
            }
            return names;
        }
    }
}
