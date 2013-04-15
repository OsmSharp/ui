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
//using System.Collections.Generic;
//using System.Linq;
//using OsmSharp.Osm;
//using OsmSharp.Osm.Data;
//using OsmSharp.Routing.Osm.Core;
//using OsmSharp.Routing.Osm.Core.Route;
//using OsmSharp.Routing.Osm.Graphs;
//using OsmSharp.Routing.Osm.Graphs;
//using OsmSharp.Routing.Osm.Graphs.Interpreter;
//using OsmSharp.Tools.Math;
//using OsmSharp.Tools.Math.Geo;
//using OsmSharp.Tools.Math.Graph.Routing;
//using OsmSharp.Routing.Osm.Core.Exceptions;
//using OsmSharp.Routing.Osm.Core.Metrics.Time;
//using System;
//using OsmSharp.Routing.Osm.Core.Resolving;
//using OsmSharp.Routing.Osm.Core.Constraints;

//namespace OsmSharp.Routing.Osm
//{
//    /// <summary>
//    /// Handles all routing on osm data.
//    /// </summary>
//    public class Router : IRouter<ResolvedPoint>
//    {
//        /// <summary>
//        /// Holds the data source for this graph.
//        /// </summary>
//        private IDataSourceReadOnly _source;

//        /// <summary>
//        /// The interpreter for this graph.
//        /// /// </summary>
//        private GraphInterpreterBase _interpreter;

//        /// <summary>
//        /// The graph to route on.
//        /// </summary>
//        private Graph _graph;

//        /// <summary>
//        /// The routing constraints.
//        /// </summary>
//        private IRoutingConstraints _constraints;

//        /// <summary>
//        /// Creates a router based on the graph.
//        /// </summary>
//        /// <param name="source"></param>
//        public Router(IDataSourceReadOnly source)
//        {
//            _source = source;

//            _interpreter = new GraphInterpreterTime(new OsmSharp.Routing.Interpreter.Default.DefaultVehicleInterpreter(
//                new OsmSharp.Routing.Interpreter.RoutingVehicleSimple(VehicleEnum.Car)), 
//                _source, VehicleEnum.Car);
//            _graph = new Graph(_interpreter, _source);

//            _resolved_list = new Dictionary<long, ResolvedPoint>();

//            _constraints = null;
//        }

//        /// <summary>
//        /// Creates a router based on the graph.
//        /// </summary>
//        /// <param name="source"></param>
//        public Router(IDataSourceReadOnly source, OsmSharp.Routing.Interpreter.RoutingInterpreterBase interpreter)
//        {
//            _source = source;
//            _interpreter = new GraphInterpreterTime(interpreter, _source, VehicleEnum.Car); 

//            _graph = new Graph(_interpreter, _source);

//            _resolved_list = new Dictionary<long, ResolvedPoint>();

//            _constraints = null;
//        }

//        /// <summary>
//        /// Creates a router based on the graph.
//        /// </summary>
//        /// <param name="source"></param>
//        public Router(IDataSourceReadOnly source, IRoutingConstraints constraints)
//        {
//            _source = source;

//            _interpreter = new GraphInterpreterTime(new OsmSharp.Routing.Interpreter.Default.DefaultVehicleInterpreter(
//                new OsmSharp.Routing.Interpreter.RoutingVehicleSimple(VehicleEnum.Car)),
//                _source, VehicleEnum.Car);
//            _graph = new Graph(_interpreter, _source);

//            _resolved_list = new Dictionary<long, ResolvedPoint>();

//            _constraints = constraints;
//        }

//        /// <summary>
//        /// Creates a router based on the graph.
//        /// </summary>
//        /// <param name="source"></param>
//        public Router(IDataSourceReadOnly source, OsmSharp.Routing.Interpreter.RoutingInterpreterBase interpreter, IRoutingConstraints constraints)
//        {
//            _source = source;
//            _interpreter = new GraphInterpreterTime(interpreter, _source, VehicleEnum.Car); 

//            _graph = new Graph(_interpreter, _source);

//            _resolved_list = new Dictionary<long, ResolvedPoint>();

//            _constraints = constraints;
//        }

//        #region Capabilities

//        /// <summary>
//        /// Returns true if the given vehicle type is supported.
//        /// </summary>
//        /// <param name="vehicle"></param>
//        /// <returns></returns>
//        public bool SupportsVehicle(VehicleEnum vehicle)
//        {
//            return vehicle == VehicleEnum.Car;
//        }

//        #endregion

//        #region Routing
        
//        /// <summary>
//        /// Calculates a route between two given points.
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public OsmSharpRoute Calculate(ResolvedPoint source, ResolvedPoint target)
//        {
//            return this.Calculate(source, target, float.MaxValue);
//        }

//        /// <summary>
//        /// Calculates a route between two given points.
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public OsmSharpRoute Calculate(ResolvedPoint source, ResolvedPoint target, float max)
//        {
//            Dykstra.DykstraRouting routing = new Dykstra.DykstraRouting(_graph, _constraints);

//            try
//            { 
//                // try to calculate this roure.
//                RouteLinked route = OsmSharp.Routing.Calculate(source.VertexId, target.VertexId, max);
//                return this.ConstructRoute(route, source, target);
//            }
//            catch (OsmSharp.Tools.Math.Graph.Routing.Point2Point.Exceptions.RoutingException ex)
//            {
//                // get the resolved points.
//                HashSet<ILocationObject> from_resolved = new HashSet<ILocationObject>();
//                foreach(long from in ex.From)
//                {
//                    from_resolved.Add(_resolved_list[from]);
//                }
//                HashSet<ILocationObject> to_resolved = new HashSet<ILocationObject>();
//                foreach(long to in ex.To)
//                {
//                    to_resolved.Add(_resolved_list[to]);
//                }
//                throw new RoutingException(ex, from_resolved, to_resolved);
//            }
//        }

//        /// <summary>
//        /// Calculates a route between two given points.
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public OsmSharpRoute CalculateToClosest(ResolvedPoint source, ResolvedPoint[] targets)
//        {
//            return this.CalculateToClosest(source, targets, float.MaxValue);
//        }

//        /// <summary>
//        /// Calculates a route between two given points.
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="targets"></param>
//        /// <returns></returns>
//        public OsmSharpRoute CalculateToClosest(ResolvedPoint source, ResolvedPoint[] targets, float max)
//        {
//            Dykstra.DykstraRouting routing = new Dykstra.DykstraRouting(_graph, _constraints);

//            long[] tos = new long[targets.Length];
//            for (int idx = 0; idx < targets.Length; idx++)
//            {
//                tos[idx] = targets[idx].VertexId;
//            }

//            // try to calculate this roure.
//            RouteLinked route = OsmSharp.Routing.CalculateToClosest(source.VertexId, tos, max);

//            // get the target point.
//            if (route != null)
//            {
//                ResolvedPoint target = null;
//                for (int idx = 0; idx < tos.Length; idx++)
//                {
//                    if (route.VertexId == tos[idx])
//                    {
//                        target = targets[idx];
//                    }
//                }
//                return this.ConstructRoute(route, source, target);
//            }
//            return null;
//        }

//        /// <summary>
//        /// Calculates the weight between two given points.
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public float CalculateWeight(ResolvedPoint source, ResolvedPoint target)
//        {
//            try
//            {
//                Dykstra.DykstraRouting routing = new Dykstra.DykstraRouting(_graph);
//                RouteLinked route = OsmSharp.Routing.Calculate(source.VertexId, target.VertexId, float.MaxValue);
//                return route.Weight;
//            }
//            catch (OsmSharp.Tools.Math.Graph.Routing.Point2Point.Exceptions.RoutingException ex)
//            {
//                // get the resolved points.
//                HashSet<ILocationObject> from_resolved = new HashSet<ILocationObject>();
//                foreach (long from in ex.From)
//                {
//                    from_resolved.Add(_resolved_list[from]);
//                }
//                HashSet<ILocationObject> to_resolved = new HashSet<ILocationObject>();
//                foreach (long to in ex.To)
//                {
//                    to_resolved.Add(_resolved_list[to]);
//                }
//                throw new RoutingException(ex, from_resolved, to_resolved);
//            }
//        }

//        /// <summary>
//        /// Calculates a route between one source and many target points.
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="targets"></param>
//        /// <returns></returns>
//        public float[] CalculateOneToManyWeight(ResolvedPoint source, ResolvedPoint[] targets)
//        {
//            try
//            {
//                Dykstra.DykstraRouting routing = new Dykstra.DykstraRouting(_graph);
//                long[] tos = new long[targets.Length];
//                for (int idx = 0; idx < targets.Length; idx++)
//                {
//                    tos[idx] = targets[idx].VertexId;
//                }
//                return OsmSharp.Routing.CalculateOneToMany(source.VertexId, tos);
//            }
//            catch (OsmSharp.Tools.Math.Graph.Routing.Point2Point.Exceptions.RoutingException ex)
//            {
//                // get the resolved points.
//                HashSet<ILocationObject> from_resolved = new HashSet<ILocationObject>();
//                foreach(long from in ex.From)
//                {
//                    from_resolved.Add(_resolved_list[from]);
//                }
//                HashSet<ILocationObject> to_resolved = new HashSet<ILocationObject>();
//                foreach(long to in ex.To)
//                {
//                    to_resolved.Add(_resolved_list[to]);
//                }
//                throw new RoutingException(ex, from_resolved, to_resolved);
//            }
//        }

//        /// <summary>
//        /// Calculates all routes between many sources/targets.
//        /// </summary>
//        /// <param name="sources"></param>
//        /// <param name="targets"></param>
//        /// <returns></returns>
//        public float[][] CalculateManyToManyWeight(ResolvedPoint[] sources, ResolvedPoint[] targets)
//        {
//            try
//            {
//                Dykstra.DykstraRouting routing = new Dykstra.DykstraRouting(_graph);
//                long[] froms = new long[sources.Length];
//                for (int idx = 0; idx < sources.Length; idx++)
//                {
//                    froms[idx] = sources[idx].VertexId;
//                }
//                long[] tos = new long[targets.Length];
//                for (int idx = 0; idx < targets.Length; idx++)
//                {
//                    tos[idx] = targets[idx].VertexId;
//                }
//                return OsmSharp.Routing.CalculateManyToMany(froms, tos);
//            }
//            catch (OsmSharp.Tools.Math.Graph.Routing.Point2Point.Exceptions.RoutingException ex)
//            {
//                // get the resolved points.
//                HashSet<ILocationObject> from_resolved = new HashSet<ILocationObject>();
//                foreach (long from in ex.From)
//                {
//                    from_resolved.Add(_resolved_list[from]);
//                }
//                HashSet<ILocationObject> to_resolved = new HashSet<ILocationObject>();
//                foreach (long to in ex.To)
//                {
//                    to_resolved.Add(_resolved_list[to]);
//                }
//                throw new RoutingException(ex, from_resolved, to_resolved);
//            }
//        }

//        public bool IsCalculateRangeSupported
//        {
//            get
//            {
//                return false;
//            }
//        }

//        public HashSet<ILocationObject> CalculateRange(ResolvedPoint orgin, float weight)
//        {
//            throw new NotSupportedException();
//        }

//        #endregion

//        #region Error Detection/Error Handling

//        /// <summary>
//        /// Returns true if the given point is connected for a radius of at least the given weight.
//        /// </summary>
//        /// <param name="point"></param>
//        /// <param name="weight"></param>
//        /// <returns></returns>
//        public bool CheckConnectivity(ResolvedPoint point, float weight)
//        {
//            try
//            {
//                Dykstra.DykstraRouting routing = new Dykstra.DykstraRouting(_graph);
//                return OsmSharp.Routing.CheckConnectivity(point.VertexId, weight);
//            }
//            catch (OsmSharp.Tools.Math.Graph.Routing.Point2Point.Exceptions.RoutingException ex)
//            {
//                // get the resolved points.
//                HashSet<ILocationObject> from_resolved = new HashSet<ILocationObject>();
//                foreach (long from in ex.From)
//                {
//                    from_resolved.Add(_resolved_list[from]);
//                }
//                HashSet<ILocationObject> to_resolved = new HashSet<ILocationObject>();
//                foreach (long to in ex.To)
//                {
//                    to_resolved.Add(_resolved_list[to]);
//                }
//                throw new RoutingException(ex, from_resolved, to_resolved);
//            }
//        }

//        /// <summary>
//        /// Returns an array of connectivity flags.
//        /// </summary>
//        /// <param name="point"></param>
//        /// <param name="weight"></param>
//        /// <returns></returns>
//        public bool[] CheckConnectivity(ResolvedPoint[] point, float weight)
//        {
//            try
//            {
//                bool[] connectivities = new bool[point.Length];
//                for (int idx = 0; idx < point.Length; idx++)
//                {
//                    connectivities[idx] = this.CheckConnectivity(point[idx], weight);

//                    // report progress.
//                    OsmSharp.Tools.Output.OutputStreamHost.ReportProgress(idx, point.Length, "Router.Raw.CheckConnectivity", 
//                        "Checking connectivity...");
//                }
//                return connectivities;
//            }
//            catch (OsmSharp.Tools.Math.Graph.Routing.Point2Point.Exceptions.RoutingException ex)
//            {
//                // get the resolved points.
//                HashSet<ILocationObject> from_resolved = new HashSet<ILocationObject>();
//                foreach(long from in ex.From)
//                {
//                    from_resolved.Add(_resolved_list[from]);
//                }
//                HashSet<ILocationObject> to_resolved = new HashSet<ILocationObject>();
//                foreach(long to in ex.To)
//                {
//                    to_resolved.Add(_resolved_list[to]);
//                }
//                throw new RoutingException(ex, from_resolved, to_resolved);
//            }
//        }

//        #endregion

//        #region Resolving
        
//        /// <summary>
//        /// Holds the list of extra positions to route to.
//        /// </summary>
//        private Dictionary<long, ResolvedPoint> _resolved_list;

//        /// <summary>
//        /// Returns a resolved vertex at a given node.
//        /// </summary>
//        /// <param name="vertex_id"></param>
//        /// <returns></returns>
//        public ResolvedPoint ResolveAt(long vertex_id)
//        {
//            GraphVertex vertex = _graph.GetVertex(vertex_id);

//            // don't return a resolved point if no data was found!
//            if (vertex == null)
//            {
//                return null;
//            }
//            ResolvedPoint resolved_point = new ResolvedPoint(vertex.Coordinate, vertex);
//            return resolved_point;
//        }

//        /// <summary>
//        /// Resolves a point.
//        /// </summary>
//        /// <param name="coordinate"></param>
//        /// <returns></returns>
//        public ResolvedPoint Resolve(GeoCoordinate coordinate)
//        {
//            return this.Resolve(coordinate, null);
//        }

//        /// <summary>
//        /// Resolves all the given points.
//        /// </summary>
//        /// <param name="coordinates"></param>
//        /// <returns></returns>
//        public ResolvedPoint[] Resolve(GeoCoordinate[] coordinates)
//        {
//            return this.Resolve(coordinates, null);
//        }

//        /// <summary>
//        /// Resolves a point.
//        /// </summary>
//        /// <param name="coordinate"></param>
//        /// <returns></returns>
//        public ResolvedPoint Resolve(GeoCoordinate coordinate, IResolveMatcher matcher)
//        {
//            GraphVertex vertex = null;
//            if (matcher == null)
//            {
//                vertex = _graph.DoResolve(coordinate, 0.001f, null);
//            }
//            else
//            {
//                vertex = _graph.DoResolve(coordinate, 0.001f, matcher);
//            }

//            // don't return a resolved point if no data was found!
//            if(vertex == null)
//            {
//                return null;
//            }
//            ResolvedPoint resolved_point = new ResolvedPoint(coordinate, vertex);
//            _resolved_list[resolved_point.VertexId] = resolved_point;
//            return resolved_point;
//        }

//        /// <summary>
//        /// Resolves all the given points.
//        /// </summary>
//        /// <param name="coordinates"></param>
//        /// <returns></returns>
//        public ResolvedPoint[] Resolve(GeoCoordinate[] coordinates, IResolveMatcher matcher)
//        {
//            ResolvedPoint[] resolved_points = new ResolvedPoint[coordinates.Length];
//            for (int idx = 0; idx < coordinates.Length; idx++)
//            {
//                resolved_points[idx] = this.Resolve(coordinates[idx], matcher);

//                // report progress.
//                OsmSharp.Tools.Output.OutputStreamHost.ReportProgress(idx, coordinates.Length, "Router.Raw.Resolve", "Resolving Points...");
//            }
//            return resolved_points;
//        }

//        public GeoCoordinate Search(GeoCoordinate coordinate)
//        {
//            throw new NotImplementedException();
//            //return _graph.Search(coordinate);
//        }

//        #endregion

//        #region Route Construction

//        /// <summary>
//        /// Converts a linked route to an OsmSharpRoute.
//        /// </summary>
//        /// <param name="route"></param>
//        /// <returns></returns>
//        private OsmSharpRoute ConstructRoute(RouteLinked route, ResolvedPoint source, ResolvedPoint target)
//        {
//            List<VertexAlongEdge> route_list = 
//                new List<VertexAlongEdge>();
//            GraphVertex to = _graph.GetVertex(route.VertexId);
//            while (route.From != null)
//            {
//                GraphVertex vertex = _graph.GetVertex(route.VertexId);

//                // get the neigbours.
//                Dictionary<long, VertexAlongEdge> neigbours =
//                    _graph.GetNeighboursReversedWithEdges(route.VertexId, null);

//                // returns the correct neighbour.
//                VertexAlongEdge neighbour =
//                    neigbours[route.From.VertexId];

//                // create the reverse of the above edge.
//                VertexAlongEdge reverse = new VertexAlongEdge(neighbour.Edge, vertex, neighbour.Weight);

//                // add to the route.
//                route_list.Insert(0, reverse);

//                // get the routes.
//                route = route.From;
//            }
//            GraphVertex from = _graph.GetVertex(route.VertexId);

//            // construct the actual graph route.
//            return this.Generate(source, from, target, to, route_list);
//        }


//        /// <summary>
//        /// Generates an osm sharp route from a graph route.
//        /// </summary>
//        /// <param name="from_point"></param>
//        /// <param name="to_point"></param>
//        /// <param name="route_list"></param>
//        /// <returns></returns>
//        internal OsmSharpRoute Generate(
//            ResolvedPoint from_resolved, GraphVertex from_point,
//            ResolvedPoint to_resolved, GraphVertex to_point,
//           List<VertexAlongEdge> route_list)
//        {
//            // create the route.
//            OsmSharpRoute route = null;

//            if (route_list != null)
//            {
//                route = new OsmSharpRoute();

//                RoutePointEntry[] entries;
//                if(route_list.Count > 0)
//                {
//                    entries = this.GenerateEntries(from_point, to_point, route_list);
//                }
//                else
//                {
//                    entries = new RoutePointEntry[0];
//                }

//                // create the from routing point.
//                RoutePoint from = new RoutePoint();
//                //from.Name = from_point.Name;
//                from.Latitude = (float)from_point.Coordinate.Latitude;
//                from.Longitude = (float)from_point.Coordinate.Longitude;
//                if (entries.Length > 0)
//                {
//                    entries[0].Points = new RoutePoint[1];
//                    entries[0].Points[0] = from;
//                    entries[0].Points[0].Tags = RouteTags.ConvertFrom(from_resolved.Tags);
//                }

//                // create the to routing point.
//                RoutePoint to = new RoutePoint();
//                //to.Name = to_point.Name;
//                to.Latitude = (float)to_point.Coordinate.Latitude;
//                to.Longitude = (float)to_point.Coordinate.Longitude;
//                if (entries.Length > 0)
//                {
//                    //to.Tags = ConvertTo(to_point.Tags);
//                    entries[entries.Length - 1].Points = new RoutePoint[1];
//                    entries[entries.Length - 1].Points[0] = to;
//                    entries[entries.Length - 1].Points[0].Tags = RouteTags.ConvertFrom(to_resolved.Tags);
//                }

//                // set the routing points.
//                route.Entries = entries;

//                // calculate metrics.
//                TimeCalculator calculator = new TimeCalculator();
//                Dictionary<string, double> metrics = calculator.Calculate(route);
//                route.TotalDistance = metrics[TimeCalculator.DISTANCE_KEY];
//                route.TotalTime = metrics[TimeCalculator.TIME_KEY];
//            }

//            return route;
//        }

//        private static RouteTags[] ConvertTo(List<KeyValuePair<string, string>> list)
//        {
//            RouteTags[] tags = null;

//            if (list.Count > 0)
//            {
//                tags = new RouteTags[list.Count];
//                for (int idx = 0; idx < list.Count; idx++)
//                {
//                    tags[idx] = new RouteTags()
//                    {
//                        Key = list[idx].Key,
//                        Value = list[idx].Value
//                    };
//                }
//            }

//            return tags;
//        }

//        /// <summary>
//        /// Generates a list of entries.
//        /// </summary>
//        /// <param name="interpreter"></param>
//        /// <param name="graph_route"></param>
//        /// <param name="graph"></param>
//        /// <returns></returns>
//        private RoutePointEntry[] GenerateEntries(GraphVertex from, GraphVertex to, 
//            List<VertexAlongEdge> route_list)
//        {
//            HashSet<GraphVertex> empty = new HashSet<GraphVertex>();

//            // create an entries list.
//            List<RoutePointEntry> entries = new List<RoutePointEntry>();

//            // create the first entry.
//            RoutePointEntry first = new RoutePointEntry();
//            first.Latitude = (float)from.Coordinate.Latitude;
//            first.Longitude = (float)from.Coordinate.Longitude;
//            first.Type = RoutePointEntryType.Start;
//            first.WayFromName = null;
//            first.WayFromNames = null;

//            entries.Add(first);

//            // create all the other entries except the last one.
//            GraphVertex node_previous = route_list[0].Vertex;
//            for (int idx = 0; idx < route_list.Count - 1; idx++)
//            {
//                // get all the data needed to calculate the next route entry.
//                GraphVertex node_current = route_list[idx].Vertex;
//                GraphVertex node_next = route_list[idx + 1].Vertex;
//                Way way_current = route_list[idx].Edge;

//                // FIRST CALCULATE ALL THE ENTRY METRICS!

//                // STEP1: Get the names.
//                string name = _interpreter.GetName(way_current);
//                Dictionary<string, string> names = _interpreter.GetNamesInAllLanguages(way_current);

//                // STEP2: Get the side streets
//                IList<RoutePointEntrySideStreet> side_streets = new List<RoutePointEntrySideStreet>();
//                Dictionary<long, VertexAlongEdge> neighbours = _graph.GetNeighboursUndirectedWithEdges(node_current.Id, null);
//                if (neighbours.Count > 2)
//                {
//                    // construct neighbours list.
//                    foreach (KeyValuePair<long, VertexAlongEdge> neighbour in neighbours)
//                    {
//                        if (neighbour.Value.Vertex != node_previous && neighbour.Value.Vertex != node_next)
//                        {
//                            RoutePointEntrySideStreet side_street = new RoutePointEntrySideStreet();

//                            side_street.Latitude = (float)neighbour.Value.Vertex.Coordinate.Latitude;
//                            side_street.Longitude = (float)neighbour.Value.Vertex.Coordinate.Longitude;
//                            side_street.Tags = RouteTags.ConvertFrom(neighbour.Value.Edge.Tags);
//                            side_street.WayName = _interpreter.GetName(neighbour.Value.Edge);
//                            side_street.WayNames = RouteTags.ConvertFrom(
//                                _interpreter.GetNamesInAllLanguages(neighbour.Value.Edge));

//                            side_streets.Add(side_street);
//                        }
//                    }
//                }

//                // create the route entry.
//                RoutePointEntry route_entry = new RoutePointEntry();
//                route_entry.Latitude = (float)node_current.Coordinate.Latitude;
//                route_entry.Longitude = (float)node_current.Coordinate.Longitude;
//                route_entry.SideStreets = side_streets.ToArray<RoutePointEntrySideStreet>();
//                route_entry.Tags = RouteTags.ConvertFrom(way_current.Tags);
//                route_entry.Type = RoutePointEntryType.Along;
//                route_entry.WayFromName = name;
//                route_entry.WayFromNames = RouteTags.ConvertFrom(names);
//                entries.Add(route_entry);

//                // set the previous node.
//                node_previous = node_current;
//            }

//            // create the last entry.
//            if (route_list.Count != 0)
//            {
//                int last_idx = route_list.Count - 1;
//                Way way_last = route_list[last_idx].Edge;
//                RoutePointEntry last = new RoutePointEntry();
//                last.Latitude = (float)route_list[last_idx].Vertex.Coordinate.Latitude;
//                last.Longitude = (float)route_list[last_idx].Vertex.Coordinate.Longitude;
//                last.Type = RoutePointEntryType.Stop;
//                last.Tags = RouteTags.ConvertFrom(route_list[route_list.Count - 1].Edge.Tags);
//                last.WayFromName = _interpreter.GetName(way_last);
//                last.WayFromNames = RouteTags.ConvertFrom(_interpreter.GetNamesInAllLanguages(way_last));

//                entries.Add(last);
//            }

//            // return the result.
//            return entries.ToArray();
//        }

//        #endregion


//    }

//    /// <summary>
//    /// Represents a resolved point. A hook for the router to route on.
//    /// </summary>
//    public class ResolvedPoint : IPointF2D, IResolvedPoint
//    {
//        /// <summary>
//        /// Creates a new resolved point.
//        /// </summary>
//        /// <param name="original"></param>
//        /// <param name="data"></param>
//        internal ResolvedPoint(GeoCoordinate original, GraphVertex data)
//        {
//            this.Original = original;
//            this.Data = data;
//            this.Tags = new List<KeyValuePair<string, string>>();
//        }

//        /// <summary>
//        /// The original location of this point.
//        /// </summary>
//        public GeoCoordinate Original { get; private set; }

//        /// <summary>
//        /// The name of the point if any.
//        /// </summary>
//        public string Name { get; set; }

//        /// <summary>
//        /// The location of the resolved point.
//        /// </summary>
//        public GeoCoordinate Location 
//        {
//            get
//            {
//                return this.Data.Coordinate;
//            }
//        }

//        /// <summary>
//        /// Returns the way that was resolved.
//        /// </summary>
//        public Way Way
//        {
//            get
//            {
//                if (this.Data != null)
//                {
//                    if (this.Data.Resolved != null)
//                    {
//                        return this.Data.Resolved.Way;
//                    }
//                }
//                return null;
//            }
//        }

//        public long VertexId
//        {
//            get
//            {
//                return this.Data.Id;
//            }
//        }

//        /// <summary>
//        /// The data used to link this resolved point to the road network.
//        /// </summary>
//        internal GraphVertex Data { get; private set; }

//        /// <summary>
//        /// The tags of this resolved point.
//        /// </summary>
//        public List<KeyValuePair<string, string>> Tags { get; private set; }

//        double IPointF2D.this[int dim]
//        {
//            get 
//            {
//                return this.Data.Coordinate[dim];
//            }
//        }
//    }
//}
