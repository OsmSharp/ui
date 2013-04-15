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
//using OsmSharp.Routing;
//using OsmSharp.Tools.Math.Units.Speed;
//using OsmSharp.Tools.Math.Geo;

//namespace OsmSharp.Routing.Roads.Tags
//{
//    /// <summary>
//    /// Interpreter for way tags that can be implemented to use a more configurable tag interpretation method.
//    /// </summary>
//    public class RoadTagsInterpreterBase
//    {
//        /// <summary>
//        /// The tags being interpreted.
//        /// </summary>
//        private IDictionary<string, string> _tags;

//        /// <summary>
//        /// Creates a new interpreter for the given tags.
//        /// </summary>
//        /// <param name="way"></param>
//        public RoadTagsInterpreterBase(IDictionary<string, string> tags)
//        {
//            _tags = tags;
//        }

//        /// <summary>
//        /// Creates a new interpreter for the given tags.
//        /// </summary>
//        /// <param name="way"></param>
//        public RoadTagsInterpreterBase(List<KeyValuePair<string, string>> tags)
//        {
//            _tags = new Dictionary<string, string>();
//            foreach (KeyValuePair<string, string> tag in tags)
//            {
//                _tags.Add(tag.Key, tag.Value);
//            }
//        }

//        /// <summary>
//        /// Returns the tags this interpreter is for.
//        /// </summary>
//        public IDictionary<string, string> Tags
//        {
//            get
//            {
//                return _tags;
//            }
//        }

//        /// <summary>
//        /// Returns true if this way can be travelled by the given vehicle.
//        /// </summary>
//        /// <param name="vehicle"></param>
//        /// <returns></returns>
//        public bool CanBeTravelledBy(VehicleEnum vehicle)
//        {
//            if (this.Tags.ContainsKey("highway"))
//            {
//                // remove all restricted roads.
//                // TODO: include other private roads.
//                if (this.Tags.ContainsKey("access"))
//                {
//                    if (this.Tags["access"] == "private"
//                        || this.Tags["access"] == "official")
//                    {
//                        return false;
//                    }
//                    else
//                    {
//                        return true;
//                    }
//                }

//                if (this.IsMotorVehicle(vehicle))
//                {
//                    if (this.Tags.ContainsKey("motor_vehicle"))
//                    {
//                        if (this.Tags["motor_vehicle"] == "no")
//                        {
//                            return false;
//                        }
//                    }
//                }

//                switch (vehicle)
//                {
//                    case VehicleEnum.Car:
//                    case VehicleEnum.Bus:
//                        if (this.Tags.ContainsKey("bicycle"))
//                        {
//                            if (this.Tags["bicycle"] == "designated")
//                            {
//                                return false;
//                            }
//                        }
//                        if (this.Tags.ContainsKey("foot"))
//                        {
//                            if (this.Tags["foot"] == "designated")
//                            {
//                                return false;
//                            }
//                        }
//                        break;
//                }

//                string highway_type = this.Tags["highway"];
//                switch (highway_type)
//                {
//                    case "proposed":
//                        //case "service":
//                        return false;
//                    case "cycleway":
//                    case "pedestrian":
//                    case "steps":
//                    case "path":
//                    case "footway":
//                        switch (vehicle)
//                        {
//                            case VehicleEnum.Bike:
//                            case VehicleEnum.Pedestrian:
//                                break;
//                            case VehicleEnum.Car:
//                            case VehicleEnum.Bus:
//                                return false;
//                        }
//                        break;
//                    case "track":
//                        switch (vehicle)
//                        {
//                            case VehicleEnum.Bike:
//                            case VehicleEnum.Pedestrian:
//                                break;
//                            case VehicleEnum.Car:
//                                break;
//                            case VehicleEnum.Bus:
//                                return false;
//                        }
//                        break;
//                    case "residential":
//                        switch (vehicle)
//                        {
//                            case VehicleEnum.Bike:
//                            case VehicleEnum.Car:
//                            case VehicleEnum.Pedestrian:
//                            case VehicleEnum.Bus:
//                                break;
//                        }
//                        break;
//                    case "motorway":
//                    case "motorway_link":
//                    case "trunk":
//                    case "trunk_link":
//                    case "primary":
//                    case "primary_link":
//                        switch (vehicle)
//                        {
//                            case VehicleEnum.Bike:
//                            case VehicleEnum.Pedestrian:
//                                return false;
//                            case VehicleEnum.Car:
//                            case VehicleEnum.Bus:
//                                break;
//                        }
//                        break;
//                    default: // service:
//                        switch (vehicle)
//                        {
//                            case VehicleEnum.Bike:
//                            case VehicleEnum.Car:
//                            case VehicleEnum.Pedestrian:
//                            case VehicleEnum.Bus:
//                                break;
//                        }
//                        break;
//                }
//                return true;
//            }
//            return false;
//        }

//        /// <summary>
//        /// Returns true if the tags represent a private accessible road.
//        /// </summary>
//        /// <returns></returns>
//        public bool IsPrivate()
//        {
//            if (this.Tags.ContainsKey("access"))
//            {
//                if (this.Tags["access"] == "private"
//                    || this.Tags["access"] == "official")
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        /// <summary>
//        /// Returns true if the tags represent a road accessible for services.
//        /// </summary>
//        /// <returns></returns>
//        public bool IsService()
//        {
//            if (this.Tags.ContainsKey("highway"))
//            {
//                if (this.Tags["highway"] == "service")
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        /// <summary>
//        /// Returns true if only vehicles with a destination can access.
//        /// </summary>
//        /// <returns></returns>
//        public bool IsOnlyLocalAccessible()
//        {
//            return this.IsService() || this.IsPrivate();
//        }

//        /// <summary>
//        /// Returns true if the given vehicle type is a motorized vehicle.
//        /// </summary>
//        /// <param name="vehicle"></param>
//        /// <returns></returns>
//        private bool IsMotorVehicle(VehicleEnum vehicle)
//        {
//            switch (vehicle)
//            {
//                case VehicleEnum.Bike:
//                case VehicleEnum.Pedestrian:
//                    return false;
//            }
//            return true;
//        }

//        /// <summary>
//        /// Returns true if the way is a roundabout.
//        /// </summary>
//        /// <returns></returns>
//        public bool IsRoundabout()
//        {
//            return this.ContainsTagValuePair("junction", "roundabout");
//        }

//        /// <summary>
//        /// Returns true if the way is important enough to be counted as a side street.                                                                              
//        /// </summary>
//        /// <returns></returns>
//        public bool IsImportantSideStreet()
//        {
//            return !this.ContainsTagValuePair("highway", "track");
//        }

//        /// <summary>
//        /// Returns true if this way is a road.
//        /// </summary>
//        /// <returns></returns>
//        public bool IsRoad()
//        {
//            return this.ContainsTag("highway");
//        }

//        /// <summary>
//        /// Returns the name of the way.
//        /// </summary>
//        /// <returns></returns>
//        public string Name()
//        {
//            return this.GetValue("name");
//        }

//        /// <summary>
//        /// Returns all the names of the way in all languages.
//        /// </summary>
//        /// <returns></returns>
//        public List<KeyValuePair<string, string>> Names()
//        {
//            Dictionary<string, string> names = new Dictionary<string, string>();
//            if (this.Tags != null)
//            {
//                foreach (KeyValuePair<string, string> pair in this.Tags)
//                {
//                    System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(pair.Key, "name:[a-zA-Z]");
//                    if (m.Success)
//                    {
//                        throw new NotImplementedException();
//                    }
//                }
//            }
//            return new List<KeyValuePair<string, string>>(names);
//        }

//        /// <summary>
//        /// Returns true if the way is oneway.
//        /// </summary>
//        /// <returns></returns>
//        public bool IsOneWay()
//        {
//            return this.ContainsTagValuePair("oneway", "yes", "reverse");
//        }

//        /// <summary>
//        /// Returns true if the way is oneway reverse.
//        /// </summary>
//        /// <returns></returns>
//        public bool IsOneWayReverse()
//        {
//            return this.ContainsTagValuePair("oneway", "reverse");
//        }

//        /// <summary>
//        /// Returns true if the properties of the way are the same considering the vehicle type.
//        /// </summary>
//        /// <param name="vehicle"></param>
//        /// <param name="other"></param>
//        /// <returns></returns>
//        public bool IsEqualForVehicle(VehicleEnum vehicle, RoadTagsInterpreterBase other)
//        {
//            if (this.Name() != other.Name())
//            { // the name have to be equal.
//                return false;
//            }

//            // check the road properties relevant for each vehicle.
//            switch (vehicle)
//            {
//                case VehicleEnum.Pedestrian:
//                case VehicleEnum.Bike:
//                case VehicleEnum.Car:
//                case VehicleEnum.Bus:

//                    break;
//            }
//            return true;
//        }

//        /// <summary>
//        /// Returns the maximum speed.
//        /// </summary>
//        /// <returns></returns>
//        public KilometerPerHour MaxSpeed(VehicleEnum vehicle)
//        {
//            // THERE ARE THE MAX SPEEDS FOR BELGIUM. 
//            // TODO: Find a way to make this all configurable.
//            KilometerPerHour speed = null;

//            KilometerPerHour pedestrian_speed = 5;
//            KilometerPerHour bike_speed = 15;

//            // get max-speed tag if any.
//            string max_speed_value = this.GetValue("maxspeed");
//            int max_speed;
//            if (int.TryParse(max_speed_value, out max_speed))
//            {
//                return max_speed; // TODO: maybe return a differend speed profile, max speed is not always the actual speed.
//            }

//            string highway_type = this.GetValue("highway");
//            switch (highway_type)
//            {
//                case "services":
//                case "proposed":
//                    switch (vehicle)
//                    {
//                        case VehicleEnum.Bike:
//                            speed = pedestrian_speed;
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
//                case "cycleway":
//                case "pedestrian":
//                case "steps":
//                case "path":
//                case "footway":
//                    switch (vehicle)
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
//                    switch (vehicle)
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
//                    switch (vehicle)
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
//                    switch (vehicle)
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
//                    switch (vehicle)
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
//                    switch (vehicle)
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

//            return speed;
//        }

//        /// <summary>
//        /// Calculates the time along these nodes.
//        /// </summary>
//        /// <param name="nodes"></param>
//        /// <param name="start"></param>
//        /// <param name="lenght"></param>
//        /// <returns></returns>
//        public double Time(VehicleEnum vehicle, GeoCoordinate[] nodes)
//        {
//            return this.Time(vehicle, nodes, 0, nodes.Length);
//        }

//        /// <summary>
//        /// Calculates the time along these nodes.
//        /// </summary>
//        /// <param name="nodes"></param>
//        /// <returns></returns>
//        public double Time(VehicleEnum vehicle, GeoCoordinate[] nodes, int start, int lenght)
//        {
//            double distance = nodes.DistanceEstimate(start, lenght);

//            return distance / (this.MaxSpeed(vehicle).Value) * 3.6;
//        }

//        /// <summary>
//        /// Calculates the time along these nodes.
//        /// </summary>
//        /// <param name="nodes"></param>
//        /// <returns></returns>
//        public double Time(VehicleEnum vehicle, GeoCoordinate from, GeoCoordinate to)
//        {
//            double distance = from.DistanceEstimate(to).Value;

//            return distance / (this.MaxSpeed(vehicle).Value) * 3.6;
//        }

//        #region Tags

//        /// <summary>
//        /// Returns true if the way contains the given tag.
//        /// </summary>
//        /// <param name="tag"></param>
//        /// <returns></returns>
//        private bool ContainsTag(string tag)
//        {
//            if (this.Tags != null)
//            {
//                if (this.Tags.ContainsKey(tag))
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        /// <summary>
//        /// Returns the value of a tag if any.
//        /// </summary>
//        /// <param name="tag"></param>
//        /// <returns></returns>
//        private string GetValue(string tag)
//        {
//            string value = string.Empty;
//            if (this.Tags != null)
//            {
//                this.Tags.TryGetValue(tag, out value);
//            }
//            return value;
//        }

//        /// <summary>
//        /// Returns true if the way containts the given tag and the given value(s) with it.
//        /// </summary>
//        /// <param name="tag"></param>
//        /// <param name="values"></param>
//        /// <returns></returns>
//        private bool ContainsTagValuePair(string tag, params string[] values)
//        {
//            if (this.Tags != null)
//            {
//                if (this.Tags.ContainsKey(tag))
//                {
//                    return values.Contains<string>(this.Tags[tag]);
//                }
//            }
//            return false;
//        }

//        #endregion
//    }
//}
