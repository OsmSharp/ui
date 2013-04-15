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
//using OsmSharp.Tools.Math.Graph;
//using OsmSharp.Osm;
//using OsmSharp.Osm.Data;
//using OsmSharp.Routing.Osm.Graphs;
//using OsmSharp.Tools.Math.Geo;
//using OsmSharp.Routing.Osm.Core;
//using OsmSharp.Routing.Osm.Core.Interpreter;

//namespace OsmSharp.Routing.Osm.Graphs.Interpreter
//{
//    /// <summary>
//    /// Class used to interpret osm data.
//    /// </summary>
//    public abstract class GraphInterpreterBase
//    {
//        /// <summary>
//        /// Holds the vehicle type.
//        /// </summary>
//        private VehicleEnum _vehicle;

//        /// <summary>
//        /// Holds the graph.
//        /// </summary>
//        private IDataSourceReadOnly _source;

//        /// <summary>
//        /// Holds the routing interpreter.
//        /// </summary>
//        private RoutingInterpreterBase _routing_interpreter;

//        /// <summary>
//        /// Creates a new interpreter.
//        /// </summary>
//        public GraphInterpreterBase(RoutingInterpreterBase routing_interpreter, IDataSourceReadOnly source, VehicleEnum vehicle)
//        {
//            _routing_interpreter = routing_interpreter;
//            _vehicle = vehicle;
//            _source = source;
//        }

//        /// <summary>
//        /// Returns the routing interpreter.
//        /// </summary>
//        public RoutingInterpreterBase RoutingInterpreter
//        {
//            get
//            {
//                return _routing_interpreter;
//            }
//        }

//        /// <summary>
//        /// Returns the vehicle type this interpreter is interpreting for.
//        /// </summary>
//        public VehicleEnum Vehicle
//        {
//            get
//            {
//                return _vehicle;
//            }
//        }

//        /// <summary>
//        /// Returns the name of a given way.
//        /// </summary>
//        /// <param name="way"></param>
//        /// <returns></returns>
//        public string GetName(Way way)
//        {
//            string name = string.Empty;
//            if (way.Tags.ContainsKey("name"))
//            {
//                name = way.Tags["name"];
//            }
//            return name;
//        }

//        /// <summary>
//        /// Returns all the names in all languages and alternatives.
//        /// </summary>
//        /// <param name="way"></param>
//        /// <returns></returns>
//        public Dictionary<string, string> GetNamesInAllLanguages(Way way)
//        {
//            Dictionary<string, string> names = new Dictionary<string, string>();
//            if (way.Tags != null)
//            {
//                foreach (KeyValuePair<string, string> pair in way.Tags)
//                {
//                    System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(pair.Key, "name:[a-zA-Z]");
//                    if (m.Success)
//                    {
//                        //throw new NotImplementedException();
//                    }
//                }
//            }
//            return names;
//        }

//        /// <summary>
//        /// Returns true if a node has some special weights and needs to be split into an edge.
//        /// </summary>
//        /// <returns></returns>
//        public virtual bool HasWeight(Node node)
//        {
//            IList<Relation> relations_for_node = _source.GetRelationsFor(node);
//            if (relations_for_node != null 
//                && relations_for_node.Count > 0)
//            {
//                return false;
//            }
//            return false;
//        }

//        #region IGraphInterpreter<Way,Node> Members

//        /// <summary>
//        /// Returns true if the given way can be traversed from a given node to a given node.
//        /// </summary>
//        /// <param name="way"></param>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        public bool CanBeTraversed(Way way, Node from, Node to)
//        {
//            if (from == null || to == null)
//            { // return true if one of the nodes is null.
//                return true;
//            }
//            else
//            { // test the traversability.
//                if (this.Vehicle != VehicleEnum.Pedestrian)
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
//        /// Returns true if the way can be traversed.
//        /// </summary>
//        /// <param name="way"></param>
//        /// <returns></returns>
//        public virtual bool CanBeTraversed(Way way)
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

//                if (this.IsMotorVehicle(this.Vehicle))
//                {
//                    if (way.Tags.ContainsKey("motor_vehicle"))
//                    {
//                        if (way.Tags["motor_vehicle"] == "no")
//                        {
//                            return false;
//                        }
//                    }
//                }

//                switch (this.Vehicle)
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
//                    //case "service":
//                        switch (_vehicle)
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
//                        switch (_vehicle)
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
//                        switch (_vehicle)
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
//                        switch (_vehicle)
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
//                        switch (_vehicle)
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
//                        switch (_vehicle)
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
//        /// Returns true if the given way can be stopped on.
//        /// </summary>
//        /// <param name="way"></param>
//        /// <returns></returns>
//        public virtual bool CanBeStoppedOn(Way way)
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
        
//        /// <summary>
//        /// Returns true if the route from the from-node via the via-node to the to_node can be traversed over the given edges.
//        /// </summary>
//        /// <param name="from"></param>
//        /// <param name="edge_from"></param>
//        /// <param name="via"></param>
//        /// <param name="edge_to"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        public bool CanBeTraversed(Node from, Way edge_from, Node via, Way edge_to, Node to)
//        {
//            if (edge_from.Tags.ContainsKey("osmsharp_weighed_node")
//                && edge_to.Tags.ContainsKey("osmsharp_weighed_node"))
//            {
//                return false;
//            }
//            if (VehicleEnum.Pedestrian != this.Vehicle)
//            {
//                if (via.Id > 0)
//                {
//                    IList<Relation> relations_for_node = _source.GetRelationsFor(via);
//                    if (relations_for_node != null
//                        && relations_for_node.Count > 0)
//                    {
//                        foreach (Relation relation in relations_for_node)
//                        {
//                            if (relation.Tags.Contains(new KeyValuePair<string, string>("type", "restriction")))
//                            {
//                                // TODO: do we need the specific type here?
//                                OsmBase via_member = relation.FindMember("via");
//                                OsmBase from_member = relation.FindMember("from");
//                                OsmBase to_member = relation.FindMember("to");
//                                if (via_member != null
//                                    && from_member != null
//                                    && to_member != null
//                                    && via_member.Id == via.Id
//                                    && from_member.Id == edge_from.Id
//                                    && to_member.Id == edge_to.Id)
//                                {
//                                    return false;
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//            return true;
//        }

//        /// <summary>
//        /// Returns true if the vehicle is a motor vehicle.
//        /// </summary>
//        /// <param name="vehicle"></param>
//        /// <returns></returns>
//        public bool IsMotorVehicle(VehicleEnum vehicle)
//        {
//            switch (vehicle)
//            {
//                case VehicleEnum.Bike:
//                case VehicleEnum.Pedestrian:
//                    return false;
//            }
//            return true;
//        }

//        #endregion

//        #region IGraphInterpreter<Way,GraphVertex> Members

//        /// <summary>
//        /// Returns true if the vertices can be travelled in the order given along the edges given.
//        /// </summary>
//        /// <param name="from"></param>
//        /// <param name="edge_from"></param>
//        /// <param name="via"></param>
//        /// <param name="edge_to"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        public bool CanBeTraversed(GraphVertex from, Way edge_from, GraphVertex via, Way edge_to, GraphVertex to)
//        {
//            // check if the via vertex is a node.
//            if (via.Type != GraphVertexEnum.Node)
//            { // always true; no restrictions on these nodes!
//                return true;
//            }
//            Node node_from = from.Node;
//            if (from.Type == GraphVertexEnum.Resolved)
//            {
//                node_from = edge_from.Nodes[from.Resolved.Idx];
//            } 
//            Node node_to = to.Node;
//            if (to.Type == GraphVertexEnum.Resolved)
//            {
//                node_to = edge_to.Nodes[to.Resolved.Idx + 1];
//            }
//            return this.CanBeTraversed(node_from, edge_from, via.Node, edge_to, node_to);
//        }

//        /// <summary>
//        /// Returns true if the vertices can be traversed in the given direction on the given edge.
//        /// </summary>
//        /// <param name="edge"></param>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        public bool CanBeTraversed(Way edge, GraphVertex from, GraphVertex to)
//        {
//            Node from_node = null;
//            Node to_node = null;
//            if (from.Type == GraphVertexEnum.Resolved && to.Type == GraphVertexEnum.Resolved)
//            { // both are special.
//                if (from.Resolved.Idx != to.Resolved.Idx)
//                {
//                    throw new ArgumentOutOfRangeException("CanBeTraversed can only be called on adjacent vertices!");
//                }
//                else if (from.Resolved.Way != edge || to.Resolved.Way != edge)
//                {
//                    throw new ArgumentOutOfRangeException("CanBeTraversed can only be called on vertices actually on the given edge!");
//                }
//                if (from.Resolved.Position > to.Resolved.Position)
//                {
//                    from_node = edge.Nodes[from.Resolved.Idx + 1];
//                    to_node = edge.Nodes[from.Resolved.Idx];
//                }
//                else
//                {
//                    from_node = edge.Nodes[from.Resolved.Idx];
//                    to_node = edge.Nodes[from.Resolved.Idx + 1];
//                }
//            }
//            else if (from.Type == GraphVertexEnum.Resolved)
//            { // from is special.
//                if (from.Resolved.Way != edge)
//                {
//                    throw new ArgumentOutOfRangeException("CanBeTraversed can only be called on vertices actually on the given edge!");
//                }

//                // check if it the next node or the previous node to get.
//                if (edge.Nodes[from.Resolved.Idx] == to.Node)
//                { // from node is the idx + 1 node.
//                    from_node = edge.Nodes[from.Resolved.Idx + 1];
//                }
//                else if(edge.Nodes[from.Resolved.Idx + 1] == to.Node)
//                { // from node is the idx node.
//                    from_node = edge.Nodes[from.Resolved.Idx];
//                }
//                else
//                {
//                    throw new ArgumentOutOfRangeException("CanBeTraversed can only be called on adjacent vertices!");
//                }
//                to_node = to.Node;
//            }
//            else if (to.Type == GraphVertexEnum.Resolved)
//            { // to is special.
//                if (to.Resolved.Way != edge)
//                {
//                    throw new ArgumentOutOfRangeException("CanBeTraversed can only be called on vertices actually on the given edge!");
//                }                
//                // check if it the next node or the previous node to get.
//                if (edge.Nodes[to.Resolved.Idx] == from.Node)
//                { // to node is the idx + 1 node.
//                    to_node = edge.Nodes[to.Resolved.Idx + 1];
//                }
//                else if (edge.Nodes[to.Resolved.Idx + 1] == from.Node)
//                { // to node is the idx node.
//                    to_node = edge.Nodes[to.Resolved.Idx];
//                }
//                else
//                {
//                    throw new ArgumentOutOfRangeException("CanBeTraversed can only be called on adjacent vertices!");
//                }
//                from_node = from.Node;
//            }
//            else
//            { // none is special.
//                from_node = from.Node;
//                to_node = to.Node;
//            }
//            return this.CanBeTraversed(edge, from_node, to_node);
//        }

//        /// <summary>
//        /// Calculates the weight of a specific part of an edge.
//        /// </summary>
//        /// <param name="edge"></param>
//        /// <param name="from_vertex"></param>
//        /// <param name="to_vertex"></param>
//        /// <returns></returns>
//        public float Weight(Way edge, GraphVertex from_vertex, GraphVertex to_vertex)
//        {
//            GeoCoordinate from = from_vertex.Coordinate;
//            GeoCoordinate to = to_vertex.Coordinate;

//            return this.CalculateWeight(edge, from, to);
//        }

//        #endregion
        
//        /// <summary>
//        /// Calculates the weight of travelling from one coordinate to another on a given way.
//        /// </summary>
//        /// <param name="edge"></param>
//        /// <param name="from_vertex"></param>
//        /// <param name="to_vertex"></param>
//        /// <returns></returns>
//        protected abstract float CalculateWeight(Way edge, GeoCoordinate from_vertex, GeoCoordinate to_vertex);

//        /// <summary>
//        /// Underestimates the weight to a given vertex.
//        /// </summary>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        protected abstract float UnderestimateWeight(GeoCoordinate from, GeoCoordinate to);

//        #region IGraphInterpreter<Way,GraphVertex> Members

//        /// <summary>
//        /// Underestimates the weight to a given vertex.
//        /// </summary>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        public float UnderestimateWeight(GraphVertex from, GraphVertex to)
//        {
//            return this.UnderestimateWeight(from.Coordinate, to.Coordinate);
//        }

//        #endregion
//    }
//}
