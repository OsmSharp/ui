using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Interpreter.Roads;
using OsmSharp.Routing;
using OsmSharp.Tools.Collections.Tags;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Tools.Math.Units.Speed;

namespace OsmSharp.Routing.Osm.Interpreter.Edge
{
    /// <summary>
    /// Default edge interpreter.
    /// </summary>
    public class EdgeInterpreter : IEdgeInterpreter
    {
        /// <summary>
        /// Holds a dictionary of access restrictions.
        /// </summary>
        private Dictionary<string, Dictionary<VehicleEnum, bool>> _access_restrictions;

        /// <summary>
        /// Creates a new edge interpreter.
        /// </summary>
        public EdgeInterpreter()
        {
            _access_restrictions = new Dictionary<string, Dictionary<VehicleEnum, bool>>();

            var highwayDic = new Dictionary<VehicleEnum,bool>();
            _access_restrictions.Add("footway", highwayDic);
            highwayDic.Add(VehicleEnum.Pedestrian, true);
            highwayDic.Add(VehicleEnum.Bicycle, false);
            highwayDic.Add(VehicleEnum.Moped, false);
            highwayDic.Add(VehicleEnum.MotorCycle, false);
            highwayDic.Add(VehicleEnum.Car, false);
            highwayDic.Add(VehicleEnum.SmallTruck, false);
            highwayDic.Add(VehicleEnum.BigTruck, false);
            highwayDic.Add(VehicleEnum.Bus, false);

            highwayDic = new Dictionary<VehicleEnum, bool>();
            _access_restrictions.Add("cycleway", highwayDic);
            highwayDic.Add(VehicleEnum.Pedestrian, false);
            highwayDic.Add(VehicleEnum.Bicycle, true);
            highwayDic.Add(VehicleEnum.Moped, false);
            highwayDic.Add(VehicleEnum.MotorCycle, false);
            highwayDic.Add(VehicleEnum.Car, false);
            highwayDic.Add(VehicleEnum.SmallTruck, false);
            highwayDic.Add(VehicleEnum.BigTruck, false);
            highwayDic.Add(VehicleEnum.Bus, false);

            highwayDic = new Dictionary<VehicleEnum, bool>();
            _access_restrictions.Add("bridleway", highwayDic);
            highwayDic.Add(VehicleEnum.Pedestrian, false);
            highwayDic.Add(VehicleEnum.Bicycle, false);
            highwayDic.Add(VehicleEnum.Moped, false);
            highwayDic.Add(VehicleEnum.MotorCycle, false);
            highwayDic.Add(VehicleEnum.Car, false);
            highwayDic.Add(VehicleEnum.SmallTruck, false);
            highwayDic.Add(VehicleEnum.BigTruck, false);
            highwayDic.Add(VehicleEnum.Bus, false);

            highwayDic = new Dictionary<VehicleEnum, bool>();
            _access_restrictions.Add("path", highwayDic);
            highwayDic.Add(VehicleEnum.Pedestrian, true);
            highwayDic.Add(VehicleEnum.Bicycle, true);
            highwayDic.Add(VehicleEnum.Moped, false);
            highwayDic.Add(VehicleEnum.MotorCycle, false);
            highwayDic.Add(VehicleEnum.Car, false);
            highwayDic.Add(VehicleEnum.SmallTruck, false);
            highwayDic.Add(VehicleEnum.BigTruck, false);
            highwayDic.Add(VehicleEnum.Bus, false);

            highwayDic = new Dictionary<VehicleEnum, bool>();
            _access_restrictions.Add("pedestrian", highwayDic);
            highwayDic.Add(VehicleEnum.Pedestrian, true);
            highwayDic.Add(VehicleEnum.Bicycle, false);
            highwayDic.Add(VehicleEnum.Moped, false);
            highwayDic.Add(VehicleEnum.MotorCycle, false);
            highwayDic.Add(VehicleEnum.Car, false);
            highwayDic.Add(VehicleEnum.SmallTruck, false);
            highwayDic.Add(VehicleEnum.BigTruck, false);
            highwayDic.Add(VehicleEnum.Bus, false);

            highwayDic = new Dictionary<VehicleEnum, bool>();
            _access_restrictions.Add("road", highwayDic);
            highwayDic.Add(VehicleEnum.Pedestrian, true);
            highwayDic.Add(VehicleEnum.Bicycle, true);
            highwayDic.Add(VehicleEnum.Moped, true);
            highwayDic.Add(VehicleEnum.MotorCycle, true);
            highwayDic.Add(VehicleEnum.Car, true);
            highwayDic.Add(VehicleEnum.SmallTruck, true);
            highwayDic.Add(VehicleEnum.BigTruck, true);
            highwayDic.Add(VehicleEnum.Bus, true);

            highwayDic = new Dictionary<VehicleEnum, bool>();
            _access_restrictions.Add("living_street", highwayDic);
            highwayDic.Add(VehicleEnum.Pedestrian, true);
            highwayDic.Add(VehicleEnum.Bicycle, true);
            highwayDic.Add(VehicleEnum.Moped, true);
            highwayDic.Add(VehicleEnum.MotorCycle, true);
            highwayDic.Add(VehicleEnum.Car, true);
            highwayDic.Add(VehicleEnum.SmallTruck, true);
            highwayDic.Add(VehicleEnum.BigTruck, true);
            highwayDic.Add(VehicleEnum.Bus, true);

            highwayDic = new Dictionary<VehicleEnum, bool>();
            _access_restrictions.Add("residential", highwayDic);
            highwayDic.Add(VehicleEnum.Pedestrian, true);
            highwayDic.Add(VehicleEnum.Bicycle, true);
            highwayDic.Add(VehicleEnum.Moped, true);
            highwayDic.Add(VehicleEnum.MotorCycle, true);
            highwayDic.Add(VehicleEnum.Car, true);
            highwayDic.Add(VehicleEnum.SmallTruck, true);
            highwayDic.Add(VehicleEnum.BigTruck, true);
            highwayDic.Add(VehicleEnum.Bus, true);

            highwayDic = new Dictionary<VehicleEnum, bool>();
            _access_restrictions.Add("unclassified", highwayDic);
            highwayDic.Add(VehicleEnum.Pedestrian, true);
            highwayDic.Add(VehicleEnum.Bicycle, true);
            highwayDic.Add(VehicleEnum.Moped, true);
            highwayDic.Add(VehicleEnum.MotorCycle, true);
            highwayDic.Add(VehicleEnum.Car, true);
            highwayDic.Add(VehicleEnum.SmallTruck, true);
            highwayDic.Add(VehicleEnum.BigTruck, true);
            highwayDic.Add(VehicleEnum.Bus, true);

            highwayDic = new Dictionary<VehicleEnum, bool>();
            _access_restrictions.Add("tertiary", highwayDic);
            highwayDic.Add(VehicleEnum.Pedestrian, true);
            highwayDic.Add(VehicleEnum.Bicycle, true);
            highwayDic.Add(VehicleEnum.Moped, true);
            highwayDic.Add(VehicleEnum.MotorCycle, true);
            highwayDic.Add(VehicleEnum.Car, true);
            highwayDic.Add(VehicleEnum.SmallTruck, true);
            highwayDic.Add(VehicleEnum.BigTruck, true);
            highwayDic.Add(VehicleEnum.Bus, true);

            highwayDic = new Dictionary<VehicleEnum, bool>();
            _access_restrictions.Add("secondary", highwayDic);
            highwayDic.Add(VehicleEnum.Pedestrian, true);
            highwayDic.Add(VehicleEnum.Bicycle, true);
            highwayDic.Add(VehicleEnum.Moped, true);
            highwayDic.Add(VehicleEnum.MotorCycle, true);
            highwayDic.Add(VehicleEnum.Car, true);
            highwayDic.Add(VehicleEnum.SmallTruck, true);
            highwayDic.Add(VehicleEnum.BigTruck, true);
            highwayDic.Add(VehicleEnum.Bus, true);

            highwayDic = new Dictionary<VehicleEnum, bool>();
            _access_restrictions.Add("primary", highwayDic);
            highwayDic.Add(VehicleEnum.Pedestrian, true);
            highwayDic.Add(VehicleEnum.Bicycle, true);
            highwayDic.Add(VehicleEnum.Moped, true);
            highwayDic.Add(VehicleEnum.MotorCycle, true);
            highwayDic.Add(VehicleEnum.Car, true);
            highwayDic.Add(VehicleEnum.SmallTruck, true);
            highwayDic.Add(VehicleEnum.BigTruck, true);
            highwayDic.Add(VehicleEnum.Bus, true);

            highwayDic = new Dictionary<VehicleEnum, bool>();
            _access_restrictions.Add("trunk", highwayDic);
            highwayDic.Add(VehicleEnum.Pedestrian, true);
            highwayDic.Add(VehicleEnum.Bicycle, true);
            highwayDic.Add(VehicleEnum.Moped, true);
            highwayDic.Add(VehicleEnum.MotorCycle, true);
            highwayDic.Add(VehicleEnum.Car, true);
            highwayDic.Add(VehicleEnum.SmallTruck, true);
            highwayDic.Add(VehicleEnum.BigTruck, true);
            highwayDic.Add(VehicleEnum.Bus, true);

            highwayDic = new Dictionary<VehicleEnum, bool>();
            _access_restrictions.Add("motorway_link", highwayDic);
            highwayDic.Add(VehicleEnum.Pedestrian, false);
            highwayDic.Add(VehicleEnum.Bicycle, false);
            highwayDic.Add(VehicleEnum.Moped, false);
            highwayDic.Add(VehicleEnum.MotorCycle, true);
            highwayDic.Add(VehicleEnum.Car, true);
            highwayDic.Add(VehicleEnum.SmallTruck, true);
            highwayDic.Add(VehicleEnum.BigTruck, true);
            highwayDic.Add(VehicleEnum.Bus, true);

            highwayDic = new Dictionary<VehicleEnum, bool>();
            _access_restrictions.Add("motorway", highwayDic);
            highwayDic.Add(VehicleEnum.Pedestrian, false);
            highwayDic.Add(VehicleEnum.Bicycle, false);
            highwayDic.Add(VehicleEnum.Moped, false);
            highwayDic.Add(VehicleEnum.MotorCycle, true);
            highwayDic.Add(VehicleEnum.Car, true);
            highwayDic.Add(VehicleEnum.SmallTruck, true);
            highwayDic.Add(VehicleEnum.BigTruck, true);
            highwayDic.Add(VehicleEnum.Bus, true);
        }

        /// <summary>
        /// Returns true if the edge with the given tags can be traversed by the given vehicle.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool CanBeTraversedBy(TagsCollection tags, VehicleEnum vehicle)
        {
            string highway = string.Empty;
            if (tags.TryGetValue("highway", out highway))
            { // there is a highway tag.
                // remove the motorized vehicles.
                if (vehicle.IsMotorVehicle())
                {
                    if (tags.ContainsKeyValue("motor_vehicle", "no"))
                    {
                        return false;
                    }

                    // check conditional constrains.
                    if (tags.ContainsKey("motor_vehicle:conditional"))
                    { // TODO: these conditional tags cannot be checked yet.
                        // TODO: add facilities and parameters to calculations to allow
                        // these tags to be checked.
                        return true;
                    }
                }

                // do the designated tags.
                if (tags.ContainsKey("bicycle"))
                {
                    if (tags["bicycle"] == "designated" &&
                        vehicle == VehicleEnum.Bicycle)
                    {
                        return true; // designated bicycle and vehicle is bicycle.
                    }
                    else if (tags["bicycle"] == "yes" &&
                        vehicle == VehicleEnum.Bicycle)
                    {
                        return true; // yes for bicycle and vehicle is bicycle.
                    }
                    else if (tags["bicycle"] == "no" &&
                        vehicle == VehicleEnum.Bicycle)
                    {
                        return false; //  no for bicycle and vehicle is bicycle.
                    }
                }
                if (tags.ContainsKey("foot"))
                {
                    if (tags["foot"] == "designated" &&
                        vehicle == VehicleEnum.Pedestrian)
                    {
                        return true; // designated foot and vehicle is pedestrian.
                    }
                    else if (tags["foot"] == "yes" &&
                        vehicle == VehicleEnum.Pedestrian)
                    {
                        return true; // yes for foot and vehicle is pedestrian.
                    }
                    else if (tags["foot"] == "no" &&
                        vehicle == VehicleEnum.Pedestrian)
                    {
                        return false; // no for foot and vehicle is pedestrian.
                    }
                }

                // returns the correct access value.
                bool access = false;
                Dictionary<VehicleEnum, bool> highway_restructions;
                if (_access_restrictions.TryGetValue(highway, out highway_restructions) &&
                    highway_restructions.TryGetValue(vehicle, out access) &&
                    access)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true if the edge with the given tags is only accessible locally.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool IsOnlyLocalAccessible(TagsCollection tags)
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
        public double Weight(TagsCollection tags, VehicleEnum vehicle, GeoCoordinate from, GeoCoordinate to)
        {
            double distance = from.DistanceEstimate(to).Value;

            return distance / (this.MaxSpeed(vehicle, tags).Value) * 3.6;
        }

        /// <summary>
        /// Returns true if the edge with the given tags is routable.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool IsRoutable(TagsCollection tags)
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
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool? IsOneWay(TagsCollection tags, VehicleEnum vehicle)
        {
            switch (vehicle)
            {
                case VehicleEnum.Pedestrian:
                case VehicleEnum.Bicycle:
                    return null;
            }
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
        public KilometerPerHour MaxSpeed(VehicleEnum vehicle, TagsCollection tags)
        {
            // THESE ARE THE MAX SPEEDS FOR BELGIUM. 
            // TODO: Find a way to make this all configurable.
            KilometerPerHour speed = 5;

            KilometerPerHour pedestrianSpeed = 5;
            KilometerPerHour bikeSpeed = 15;

            // get max-speed tag if any.
            double? maxSpeedValue = tags.GetNumericValue("maxspeed");
            if (maxSpeedValue.HasValue)
            {
                return maxSpeedValue.Value;
            }

            string highwayType;
            if (tags.TryGetValue("highway", out highwayType))
            {
                switch (highwayType)
                {
                    case "services":
                    case "proposed":
                        switch (vehicle)
                        {
                            case VehicleEnum.Bicycle:
                                speed = pedestrianSpeed;
                                break;
                            case VehicleEnum.Pedestrian:
                                speed = pedestrianSpeed;
                                break;
                            default:
                                speed = pedestrianSpeed;
                                break;
                        }
                        break;
                    case "cycleway":
                    case "pedestrian":
                    case "steps":
                    case "path":
                    case "footway":
                    case "living_street":
                        switch (vehicle)
                        {
                            case VehicleEnum.Bicycle:
                                speed = bikeSpeed;
                                break;
                            case VehicleEnum.Pedestrian:
                                speed = pedestrianSpeed;
                                break;
                                // TODO: improve speed calculations when using access tags.
                            case VehicleEnum.Car:
                            case VehicleEnum.Bus:
                                speed = 30;
                                break;
                        }
                        break;
                    case "track":
                        switch (vehicle)
                        {
                            case VehicleEnum.Bicycle:
                                speed = bikeSpeed;
                                break;
                            case VehicleEnum.Pedestrian:
                                speed = pedestrianSpeed;
                                break;
                            case VehicleEnum.Car:
                            case VehicleEnum.Bus:
                                speed = 40;
                                break;
                        }
                        break;
                    case "residential":
                    case "road":
                        switch (vehicle)
                        {
                            case VehicleEnum.Bicycle:
                                speed = bikeSpeed;
                                break;
                            case VehicleEnum.Pedestrian:
                                speed = pedestrianSpeed;
                                break;
                            case VehicleEnum.Car:
                            case VehicleEnum.Bus:
                                speed = 50;
                                break;
                        }
                        break;
                    case "motorway":
                        switch (vehicle)
                        {
                            case VehicleEnum.Bicycle:
                                speed = bikeSpeed;
                                break;
                            case VehicleEnum.Pedestrian:
                                speed = pedestrianSpeed;
                                break;
                            case VehicleEnum.Car:
                            case VehicleEnum.Bus:
                                speed = 120;
                                break;
                        }
                        break;
                    case "motorway_link":
                        switch (vehicle)
                        {
                            case VehicleEnum.Bicycle:
                                speed = bikeSpeed;
                                break;
                            case VehicleEnum.Pedestrian:
                                speed = pedestrianSpeed;
                                break;
                            case VehicleEnum.Car:
                            case VehicleEnum.Bus:
                                speed = 90;
                                break;
                        }
                        break;
                    case "trunk":
                    case "trunk_link":
                    case "primary":
                    case "primary_link":
                        switch (vehicle)
                        {
                            case VehicleEnum.Bicycle:
                                speed = bikeSpeed;
                                break;
                            case VehicleEnum.Pedestrian:
                                speed = pedestrianSpeed;
                                break;
                            case VehicleEnum.Car:
                            case VehicleEnum.Bus:
                                speed = 75;
                                break;
                        }
                        break;
                    case "secondary":
                    case "secondary_link":
                        switch (vehicle)
                        {
                            case VehicleEnum.Bicycle:
                                speed = bikeSpeed;
                                break;
                            case VehicleEnum.Pedestrian:
                                speed = pedestrianSpeed;
                                break;
                            case VehicleEnum.Car:
                            case VehicleEnum.Bus:
                                speed = 70;
                                break;
                        }
                        break;
                    case "tertiary":
                        switch (vehicle)
                        {
                            case VehicleEnum.Bicycle:
                                speed = bikeSpeed;
                                break;
                            case VehicleEnum.Pedestrian:
                                speed = pedestrianSpeed;
                                break;
                            case VehicleEnum.Car:
                            case VehicleEnum.Bus:
                                speed = 60;
                                break;
                        }
                        break;
                    case "unclassified":
                        switch (vehicle)
                        {
                            case VehicleEnum.Bicycle:
                                speed = bikeSpeed;
                                break;
                            case VehicleEnum.Pedestrian:
                                speed = pedestrianSpeed;
                                break;
                            case VehicleEnum.Car:
                            case VehicleEnum.Bus:
                                speed = 40;
                                break;
                        }
                        break;
                    default:
                        switch (vehicle)
                        {
                            case VehicleEnum.Bicycle:
                                speed = bikeSpeed;
                                break;
                            case VehicleEnum.Pedestrian:
                                speed = pedestrianSpeed;
                                break;
                            case VehicleEnum.Car:
                            case VehicleEnum.Bus:
                                speed = 50;
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
        public string GetName(TagsCollection tags)
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
        public Dictionary<string, string> GetNamesInAllLanguages(TagsCollection tags)
        {
            Dictionary<string, string> names = new Dictionary<string, string>();
            if (tags != null)
            {
                foreach (Tag pair in tags)
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

        /// <summary>
        /// Returns true if the edges with the given properties are equal for the given vehicle.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="tags1"></param>
        /// <param name="tags2"></param>
        /// <returns></returns>
        public bool IsEqualFor(VehicleEnum vehicle, TagsCollection tags1, TagsCollection tags2)
        {
            if (this.GetName(tags1) != this.GetName(tags2))
            { // the name have to be equal.
                return false;
            }

            // check the road properties relevant for each vehicle.
            switch (vehicle)
            {
                case VehicleEnum.Pedestrian:
                case VehicleEnum.Bicycle:
                case VehicleEnum.Car:
                case VehicleEnum.Bus:

                    break;
            }
            return true;
        }

        /// <summary>
        /// Returns true if the edge with the given properties represents a roundabout.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public bool IsRoundabout(TagsCollection tags)
        {
            string junction;
            return (tags != null && tags.TryGetValue("junction", out junction) &&
                junction == "roundabout");
        }
    }
}
