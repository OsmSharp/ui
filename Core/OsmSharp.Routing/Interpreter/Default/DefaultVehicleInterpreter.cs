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
//using OsmSharp.Tools.Math.Units.Distance;
//using OsmSharp.Tools.Math.Geo;
//using OsmSharp.Tools.Math.Units.Speed;

//namespace OsmSharp.Routing.Interpreter.Default
//{
//    /// <summary>
//    /// Translates OSM data into an image of the road network represented by it for difference vehicle types.
//    /// </summary>
//    public class DefaultVehicleInterpreter : RoutingInterpreterBase
//    {
//        /// <summary>
//        /// Holds the vehicle.
//        /// </summary>
//        private RoutingVehicleBase _vehicle;

//        /// <summary>
//        /// Creates a new default vehicle interpreter.
//        /// </summary>
//        /// <param name="vehicle"></param>
//        public DefaultVehicleInterpreter(RoutingVehicleBase vehicle)
//        {
//            _vehicle = vehicle;
//        }

//        /// <summary>
//        /// Creates a new default vehicle interpreter.
//        /// </summary>
//        /// <param name="vehicle"></param>
//        public DefaultVehicleInterpreter(VehicleEnum vehicle)
//            :this(new OsmSharp.Routing.Interpreter.RoutingVehicleSimple(vehicle))
//        {

//        }


//        public override bool HasWeight(Node node)
//        {
//            return false;
//        }

//        public override float Weight(Way way, Node node_from, Node node_to)
//        {
//            GeoCoordinate from = node_from.Coordinate;
//            GeoCoordinate to = node_to.Coordinate;

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
//                    switch (_vehicle.VehicleCategory)
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
//                    switch (_vehicle.VehicleCategory)
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
//                    switch (_vehicle.VehicleCategory)
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
//                    switch (_vehicle.VehicleCategory)
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
//                    switch (_vehicle.VehicleCategory)
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
//                    switch (_vehicle.VehicleCategory)
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
//        /// Returns true if the way can traversed.
//        /// </summary>
//        /// <param name="way"></param>
//        /// <returns></returns>
//        public override bool CanBeTraversed(Way way)
//        {
//            if (way.Tags.ContainsKey("highway"))
//            {
//                // remove all restricted roads.
//                // TODO: include other private roads.
//                if (way.Tags.ContainsKey("access"))
//                {
//                    if (way.Tags["access"] == "private"
//                        || way.Tags["access"] == "official")
//                    {
//                        return false;
//                    }
//                    else
//                    {
//                        return true;
//                    }
//                }

//                if (_vehicle.IsMotorVehicle)
//                {
//                    if (way.Tags.ContainsKey("motor_vehicle"))
//                    {
//                        if (way.Tags["motor_vehicle"] == "no")
//                        {
//                            return false;
//                        }
//                    }
//                }

//                switch (_vehicle.VehicleCategory)
//                {
//                    case VehicleEnum.Car:
//                    case VehicleEnum.Bus:
//                        if (way.Tags.ContainsKey("bicycle"))
//                        {
//                            if (way.Tags["bicycle"] == "designated")
//                            {
//                                return false;
//                            }
//                        }
//                        if (way.Tags.ContainsKey("foot"))
//                        {
//                            if (way.Tags["foot"] == "designated")
//                            {
//                                return false;
//                            }
//                        }
//                        break;
//                }

//                string highway_type = way.Tags["highway"];
//                switch (highway_type)
//                {
//                    case "proposed":
//                        //case "service":
//                        switch (_vehicle.VehicleCategory)
//                        {
//                            case VehicleEnum.Bike:
//                            case VehicleEnum.Car:
//                            case VehicleEnum.Pedestrian:
//                            case VehicleEnum.Bus:
//                                break;
//                        }
//                        break;
//                    case "cycleway":
//                    case "steps":
//                    case "path":
//                    case "footway":
//                        switch (_vehicle.VehicleCategory)
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
//                        switch (_vehicle.VehicleCategory)
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
//                    case "pedestrian":
//                        switch (_vehicle.VehicleCategory)
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
//                        switch (_vehicle.VehicleCategory)
//                        {
//                            case VehicleEnum.Bike:
//                            case VehicleEnum.Pedestrian:
//                                return false;
//                            case VehicleEnum.Car:
//                            case VehicleEnum.Bus:
//                                break;
//                        }
//                        break;
//                    case "platform":
//                        return false;
//                    default:
//                        switch (_vehicle.VehicleCategory)
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
//            else if (way.Tags.ContainsKey("osmsharp_resolved"))
//            {
//                return true;
//            }
//            else if (way.Tags.ContainsKey("osmsharp_weighed_node"))
//            {
//                return true;
//            }
//            return false;
//        }

//        /// <summary>
//        /// Returns true if the way can be traversed.
//        /// </summary>
//        /// <param name="way"></param>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        public override bool CanBeTraversed(Way way, Node from, Node to)
//        {
//            if (from == null || to == null)
//            { // return true if one of the nodes is null.
//                return true;
//            }
//            else
//            { // test the traversability.
//                if (_vehicle.VehicleCategory != VehicleEnum.Pedestrian)
//                { // TODO: include support for bike and one-way streets supporting bike traffic in both directions.
//                    if (way.Tags.ContainsKey("oneway") && way.Tags["oneway"] != "no")
//                    {
//                        bool order = true;

//                        // try until the nodes are found.
//                        int from_idx = way.Nodes.IndexOf(from);
//                        while (from_idx > 0)
//                        {
//                            if (from_idx > 0 && way.Nodes[from_idx - 1] == to)
//                            {
//                                order = false;
//                                break;
//                            }
//                            else if (from_idx < way.Nodes.Count - 1 && way.Nodes[from_idx + 1] == to)
//                            {
//                                break;
//                            }
//                            else
//                            {
//                                from_idx = way.Nodes.IndexOf(from, from_idx + 1);
//                            }
//                        }

//                        // check if the nodes are found as neighbours.
//                        if (from_idx < 0)
//                        {
//                            throw new Exception("CanBeTraversed can only be applied to neigbouring nodes!");
//                        }

//                        // the nodes have been found; check oneway property.
//                        if (way.Tags["oneway"] == "true"
//                            || way.Tags["oneway"] == "yes"
//                            || way.Tags["oneway"] == "1")
//                        {
//                            return order;
//                        }

//                        return !order;
//                    }
//                }
//            }
//            return true;
//        }

//        /// <summary>
//        /// Returns true if the way(s) can be traversed along all the given nodes.
//        /// </summary>
//        /// <param name="from"></param>
//        /// <param name="edge_from"></param>
//        /// <param name="along"></param>
//        /// <param name="edge_to"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        public override bool CanBeTraversed(Node from, Way edge_from, Node via, Way edge_to, Node to)
//        {
//            if (edge_from.Tags.ContainsKey("osmsharp_weighed_node")
//                   && edge_to.Tags.ContainsKey("osmsharp_weighed_node"))
//            {
//                return false;
//            }
//            if (VehicleEnum.Pedestrian != _vehicle.VehicleCategory)
//            {
//                if (via.Id > 0)
//                {
//                    // TODO: find a way to handle turn restrictions in a common way over all routing algorithms.
//                    //IList<Relation> relations_for_node = _source.GetRelationsFor(via);
//                    //if (relations_for_node != null
//                    //    && relations_for_node.Count > 0)
//                    //{
//                    //    foreach (Relation relation in relations_for_node)
//                    //    {
//                    //        if (relation.Tags.Contains(new KeyValuePair<string, string>("type", "restriction")))
//                    //        {
//                    //            // TODO: do we need the specific type here?
//                    //            OsmBase via_member = relation.FindMember("via");
//                    //            OsmBase from_member = relation.FindMember("from");
//                    //            OsmBase to_member = relation.FindMember("to");
//                    //            if (via_member != null
//                    //                && from_member != null
//                    //                && to_member != null
//                    //                && via_member.Id == via.Id
//                    //                && from_member.Id == edge_from.Id
//                    //                && to_member.Id == edge_to.Id)
//                    //            {
//                    //                return false;
//                    //            }
//                    //        }
//                    //    }
//                    //}
//                }
//            }
//            return true;
//        }

//        /// <summary>
//        /// Returns true if the current vehicle can stop on the given way.
//        /// </summary>
//        /// <param name="way"></param>
//        /// <returns></returns>
//        public override bool CanBeStoppedOn(Way way)
//        {
//            if (way.Tags.ContainsKey("highway"))
//            {
//                switch (way.Tags["highway"])
//                {
//                    case "motorway":
//                    case "motorway_link":
//                    case "trunk":
//                    case "trunk_link":
//                        return false;
//                }
//            }
//            if (way.Tags.ContainsKey("tunnel")
//                && way.Tags["tunnel"] == "yes")
//            {
//                return false;
//            }

//            // all other roads can be stopped one.
//            // check CanBeTraversed first!
//            return true;
//        }

//        public override RoutingWayInterperterBase GetWayInterpretation(IDictionary<string, string> tags)
//        {
//            return new DefaultWayInterpreter(tags);
//        }
//    }
//}