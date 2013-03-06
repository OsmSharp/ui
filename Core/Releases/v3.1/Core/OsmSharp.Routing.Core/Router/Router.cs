// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Core.Graph;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Tools.Math;
using OsmSharp.Routing.Core.Graph.Router;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Core.Router;
using OsmSharp.Tools.Math.Units.Distance;
using OsmSharp.Routing.Core.Graph.Path;
using OsmSharp.Routing.Core.Route;
using OsmSharp.Routing.Core.Metrics.Time;
using OsmSharp.Routing.Core.Graph.Router.Dykstra;
using OsmSharp.Routing.Core.Graph.DynamicGraph;

namespace OsmSharp.Routing.Core
{
    /// <summary>
    /// A class that implements common functionality for any routing algorithm.
    /// </summary>
    public class Router<EdgeData> : IRouter<RouterPoint>
        where EdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Holds the graph object containing the routable network.
        /// </summary>
        private IBasicRouterDataSource<EdgeData> _data_graph;

        /// <summary>
        /// Holds the basic router that works on the dynamic graph.
        /// </summary>
        private IBasicRouter<EdgeData> _router;

        /// <summary>
        /// Interpreter for the routing network.
        /// </summary>
        private IRoutingInterpreter _interpreter;

        /// <summary>
        /// Creates a new router.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        public Router(IBasicRouterDataSource<EdgeData> graph, IRoutingInterpreter interpreter,
            IBasicRouter<EdgeData> router)
        {
            _data_graph = graph;
            _interpreter = interpreter;
            _router = router;

            _resolved_graph = new RouterResolvedGraph();
        }

        /// <summary>
        /// Returns true if the given vehicle is supported.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool SupportsVehicle(VehicleEnum vehicle)
        {
            return true;
        }

        /// <summary>
        /// Calculates a route from source to target.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public OsmSharpRoute Calculate(VehicleEnum vehicle, RouterPoint source, RouterPoint target)
        {
            return this.Calculate(vehicle, source, target, float.MaxValue);
        }

        /// <summary>
        /// Calculates a route from source to target but does not search more than max around source or target location.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public OsmSharpRoute Calculate(VehicleEnum vehicle, RouterPoint source, RouterPoint target, float max)
        {
            // calculate the route.
            PathSegment<long> route = _router.Calculate(_data_graph, _interpreter, vehicle,
                this.RouteResolvedGraph(source), this.RouteResolvedGraph(target), max);

            // convert to an OsmSharpRoute.
            return this.ConstructRoute(vehicle, route, source, target);
        }

        /// <summary>
        /// Calculates a route from source to the closest target point.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public OsmSharpRoute CalculateToClosest(VehicleEnum vehicle, RouterPoint source, RouterPoint[] targets)
        {
            return this.CalculateToClosest(vehicle, source, targets, float.MaxValue);
        }

        /// <summary>
        /// Calculates a route from source to the closest target point but does not search more than max around source location.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public OsmSharpRoute CalculateToClosest(VehicleEnum vehicle, RouterPoint source, RouterPoint[] targets, float max)
        {
            // calculate the route.
            PathSegment<long> route = _router.CalculateToClosest(_data_graph, _interpreter, vehicle,
                this.RouteResolvedGraph(source), this.RouteResolvedGraph(targets), max);

            // find the target.
            RouterPoint target = targets.First<RouterPoint>(x => x.Id == route.VertexId);

            // convert to an OsmSharpRoute.
            return this.ConstructRoute(vehicle, route, source, target);
        }

        /// <summary>
        /// Calculates the weight from source to target.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public double CalculateWeight(VehicleEnum vehicle, RouterPoint source, RouterPoint target)
        {
            // calculate the route.
            return _router.CalculateWeight(_data_graph, _interpreter, vehicle,
                this.RouteResolvedGraph(source), this.RouteResolvedGraph(target), float.MaxValue);
        }

        /// <summary>
        /// Calculates all the weights from source to all the targets.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public double[] CalculateOneToManyWeight(VehicleEnum vehicle, RouterPoint source, RouterPoint[] targets)
        {
            return _router.CalculateOneToManyWeight(_data_graph, _interpreter, vehicle, this.RouteResolvedGraph(source),
                this.RouteResolvedGraph(targets), double.MaxValue);
        }

        /// <summary>
        /// Calculates all the weights between all the sources and all the targets.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public double[][] CalculateManyToManyWeight(VehicleEnum vehicle, RouterPoint[] sources, RouterPoint[] targets)
        {
            return _router.CalculateManyToManyWeight(_data_graph, _interpreter, vehicle, this.RouteResolvedGraph(sources),
                this.RouteResolvedGraph(targets), double.MaxValue);
        }

        /// <summary>
        /// Returns true if range calculation is supported.
        /// </summary>
        public bool IsCalculateRangeSupported
        {
            get
            {
                return _router.IsCalculateRangeSupported;
            }
        }

        /// <summary>
        /// Calculates the locations around the origin that have a given weight.
        /// </summary>
        /// <param name="orgin"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public HashSet<GeoCoordinate> CalculateRange(VehicleEnum vehicle, RouterPoint orgin, float weight)
        {
            HashSet<long> objects_at_weight = _router.CalculateRange(_data_graph, _interpreter, vehicle, this.RouteResolvedGraph(orgin),
                weight);

            HashSet<GeoCoordinate> locations = new HashSet<GeoCoordinate>();
            foreach (long vertex in objects_at_weight)
            {
                GeoCoordinate coordinate = this.GetCoordinate(vertex);
                locations.Add(coordinate);
            }
            return locations;
        }

        /// <summary>
        /// Returns true if the given source is at least connected to vertices with at least a given weight.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool CheckConnectivity(VehicleEnum vehicle, RouterPoint point, float weight)
        {
            return _router.CheckConnectivity(_data_graph, _interpreter, vehicle, this.RouteResolvedGraph(point), weight);
        }

        /// <summary>
        /// Returns an array of connectivity check results.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool[] CheckConnectivity(VehicleEnum vehicle, RouterPoint[] point, float weight)
        {
            bool[] connectivity_array = new bool[point.Length];
            for (int idx = 0; idx < point.Length; idx++)
            {
                connectivity_array[idx] = this.CheckConnectivity(vehicle, point[idx], weight);

                // report progress.
                OsmSharp.Tools.Core.Output.OutputStreamHost.ReportProgress(idx, point.Length, "Router.Core.CheckConnectivity",
                    "Checking connectivity...");
            }
            return connectivity_array;
        }

        #region OsmSharpRoute Building

        /// <summary>
        /// Converts a linked route to an OsmSharpRoute.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private OsmSharpRoute ConstructRoute(VehicleEnum vehicle, PathSegment<long> route, RouterPoint source, RouterPoint target)
        {
            if (route != null)
            {
                long[] vertices = route.ToArray();

                // construct the actual graph route.
                return this.Generate(vehicle, source, target, vertices);
            }
            return null; // calculation failed!
        }

        /// <summary>
        /// Generates an osm sharp route from a graph route.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="from_point"></param>
        /// <param name="to_point"></param>
        /// <param name="route_list"></param>
        /// <returns></returns>
        internal OsmSharpRoute Generate(
            VehicleEnum vehicle, 
            RouterPoint from_resolved,
            RouterPoint to_resolved,
            long[] vertices)
        {
            // create the route.
            OsmSharpRoute route = null;

            if (vertices != null)
            {
                route = new OsmSharpRoute();

                RoutePointEntry[] entries;
                if (vertices.Length > 0)
                {
                    entries = this.GenerateEntries(vertices);
                }
                else
                {
                    entries = new RoutePointEntry[0];
                }

                // create the from routing point.
                RoutePoint from = new RoutePoint();
                //from.Name = from_point.Name;
                from.Latitude = (float)from_resolved.Location.Latitude;
                from.Longitude = (float)from_resolved.Location.Longitude;
                if (entries.Length > 0)
                {
                    entries[0].Points = new RoutePoint[1];
                    entries[0].Points[0] = from;
                    entries[0].Points[0].Tags = from_resolved.Tags.ConvertFrom();
                }

                // create the to routing point.
                RoutePoint to = new RoutePoint();
                //to.Name = to_point.Name;
                to.Latitude = (float)to_resolved.Location.Latitude;
                to.Longitude = (float)to_resolved.Location.Longitude;
                if (entries.Length > 0)
                {
                    //to.Tags = ConvertTo(to_point.Tags);
                    entries[entries.Length - 1].Points = new RoutePoint[1];
                    entries[entries.Length - 1].Points[0] = to;
                    entries[entries.Length - 1].Points[0].Tags = to_resolved.Tags.ConvertFrom();
                }

                // set the routing points.
                route.Entries = entries;

                // calculate metrics.
                TimeCalculator calculator = new TimeCalculator(_interpreter);
                Dictionary<string, double> metrics = calculator.Calculate(route);
                route.TotalDistance = metrics[TimeCalculator.DISTANCE_KEY];
                route.TotalTime = metrics[TimeCalculator.TIME_KEY];
            }

            // set the vehicle.
            route.Vehicle = vehicle;

            return route;
        }

        /// <summary>
        /// Generates a list of entries.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="graph_route"></param>
        /// <param name="graph"></param>
        /// <returns></returns>
        private RoutePointEntry[] GenerateEntries(long[] vertices)
        {
            // create an entries list.
            List<RoutePointEntry> entries = new List<RoutePointEntry>();

            // create the first entry.
            GeoCoordinate coordinate = this.GetCoordinate(vertices[0]);
            RoutePointEntry first = new RoutePointEntry();
            first.Latitude = (float)coordinate.Latitude;
            first.Longitude = (float)coordinate.Longitude;
            first.Type = RoutePointEntryType.Start;
            first.WayFromName = null;
            first.WayFromNames = null;

            entries.Add(first);

            // create all the other entries except the last one.
            long node_previous = vertices[0];
            for (int idx = 0; idx < vertices.Length - 2; idx++)
            {
                // get all the data needed to calculate the next route entry.
                long node_current = vertices[idx];
                long node_next = vertices[idx + 1];
                IDynamicGraphEdgeData edge = this.GetEdgeData(node_current, node_next);

                // FIRST CALCULATE ALL THE ENTRY METRICS!

                // STEP1: Get the names.
                IDictionary<string, string> current_tags = _data_graph.TagsIndex.Get(edge.Tags);
                string name = _interpreter.EdgeInterpreter.GetName(current_tags);
                Dictionary<string, string> names = _interpreter.EdgeInterpreter.GetNamesInAllLanguages(current_tags);

                // STEP2: Get the side streets
                IList<RoutePointEntrySideStreet> side_streets = new List<RoutePointEntrySideStreet>();
                Dictionary<long, IDynamicGraphEdgeData> neighbours = this.GetNeighboursUndirectedWithEdges(node_current);
                if (neighbours.Count > 2)
                {
                    // construct neighbours list.
                    foreach (KeyValuePair<long, IDynamicGraphEdgeData> neighbour in neighbours)
                    {
                        if (neighbour.Key != node_previous && neighbour.Key != node_next)
                        {
                            RoutePointEntrySideStreet side_street = new RoutePointEntrySideStreet();

                            GeoCoordinate neighbour_coordinate = this.GetCoordinate(neighbour.Key);
                            IDictionary<string, string> tags = _data_graph.TagsIndex.Get(neighbour.Value.Tags);

                            side_street.Latitude = (float)neighbour_coordinate.Latitude;
                            side_street.Longitude = (float)neighbour_coordinate.Longitude;
                            side_street.Tags = tags.ConvertFrom();
                            side_street.WayName = _interpreter.EdgeInterpreter.GetName(tags);
                            side_street.WayNames = _interpreter.EdgeInterpreter.GetNamesInAllLanguages(tags).ConvertFrom();

                            side_streets.Add(side_street);
                        }
                    }
                }

                // create the route entry.
                GeoCoordinate next_coordinate = this.GetCoordinate(node_next);
                RoutePointEntry route_entry = new RoutePointEntry();
                route_entry.Latitude = (float)next_coordinate.Latitude;
                route_entry.Longitude = (float)next_coordinate.Longitude;
                route_entry.SideStreets = side_streets.ToArray<RoutePointEntrySideStreet>();
                route_entry.Tags = current_tags.ConvertFrom();
                route_entry.Type = RoutePointEntryType.Along;
                route_entry.WayFromName = name;
                route_entry.WayFromNames = names.ConvertFrom();
                entries.Add(route_entry);

                // set the previous node.
                node_previous = node_current;
            }

            // create the last entry.
            if (vertices.Length > 1)
            {
                int last_idx = vertices.Length - 1;
                IDynamicGraphEdgeData edge = this.GetEdgeData(vertices[last_idx - 1], vertices[last_idx]);
                IDictionary<string, string> tags = _data_graph.TagsIndex.Get(edge.Tags);
                coordinate = this.GetCoordinate(vertices[last_idx]);
                RoutePointEntry last = new RoutePointEntry();
                last.Latitude = (float)coordinate.Latitude;
                last.Longitude = (float)coordinate.Longitude;
                last.Type = RoutePointEntryType.Stop;
                last.Tags = tags.ConvertFrom();
                last.WayFromName = _interpreter.EdgeInterpreter.GetName(tags);
                last.WayFromNames = _interpreter.EdgeInterpreter.GetNamesInAllLanguages(tags).ConvertFrom();

                entries.Add(last);
            }

            // return the result.
            return entries.ToArray();
        }

        /// <summary>
        /// Returns all the neighbours of the given vertex.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <returns></returns>
        private Dictionary<long, IDynamicGraphEdgeData> GetNeighboursUndirectedWithEdges(long vertex1)
        {
            Dictionary<long, IDynamicGraphEdgeData> neighbours = new Dictionary<long, IDynamicGraphEdgeData>();
            if (vertex1 > 0)
            {
                KeyValuePair<uint, EdgeData>[] arcs = _data_graph.GetArcs(Convert.ToUInt32(vertex1));
                foreach (KeyValuePair<uint, EdgeData> arc in arcs)
                {
                    neighbours[arc.Key] = arc.Value;
                }
            }
            else
            {
                KeyValuePair<long, RouterResolvedGraph.RouterResolvedGraphEdge>[] arcs = _resolved_graph.GetArcs(vertex1);
                foreach (KeyValuePair<long, RouterResolvedGraph.RouterResolvedGraphEdge> arc in arcs)
                {
                    neighbours[arc.Key] = arc.Value;
                }
            }
            return neighbours;
        }

        /// <summary>
        /// Returns the edge data between two neighbouring vertices.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <returns></returns>
        private IDynamicGraphEdgeData GetEdgeData(long vertex1, long vertex2)
        {
            if (vertex1 > 0 && vertex2 > 0)
            { // none of the vertixes was a resolved vertex.
                KeyValuePair<uint, EdgeData>[] arcs = _data_graph.GetArcs(Convert.ToUInt32(vertex1));
                foreach (KeyValuePair<uint, EdgeData> arc in arcs)
                {
                    if (arc.Key == vertex2)
                    {
                        return arc.Value;
                    }
                }         
                arcs = _data_graph.GetArcs(Convert.ToUInt32(vertex2));
                foreach (KeyValuePair<uint, EdgeData> arc in arcs)
                {
                    if (arc.Key == vertex1)
                    {
                        RouterResolvedGraph.RouterResolvedGraphEdge edge = new RouterResolvedGraph.RouterResolvedGraphEdge();
                        //edge.Backward = arc.Value.Forward;
                        //edge.Forward = arc.Value.Backward;
                        edge.Tags = arc.Value.Tags;
                        edge.Weight = arc.Value.Weight;
                        return edge;
                    }
                }
            }
            else
            { // one of the vertices was a resolved vertex.
                // edge should be in the resolved graph.
                KeyValuePair<long, RouterResolvedGraph.RouterResolvedGraphEdge>[] arcs = _resolved_graph.GetArcs(vertex1);
                foreach (KeyValuePair<long, RouterResolvedGraph.RouterResolvedGraphEdge> arc in arcs)
                {
                    if (arc.Key == vertex2)
                    {
                        return arc.Value;
                    }
                }
            }
            throw new Exception(string.Format("Edge {0}->{1} not found!",
                vertex1, vertex2));
        }

        /// <summary>
        /// Returns the coordinate of the given vertex.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private GeoCoordinate GetCoordinate(long vertex)
        {
            float latitude, longitude;
            if (vertex < 0)
            { // the vertex is resolved.
                if (!_resolved_graph.GetVertex(vertex, out latitude, out longitude))
                {
                    throw new Exception(string.Format("Vertex with id {0} not found in resolved graph!",
                        vertex));
                }
            }
            else
            { // the vertex should be in the data graph.
                if(!_data_graph.GetVertex(Convert.ToUInt32(vertex), out latitude, out longitude))
                {
                    throw new Exception(string.Format("Vertex with id {0} not found in graph!",
                        vertex));
                }
            }
            return new GeoCoordinate(latitude, longitude);
        }


        #endregion

        #region Resolving Points

        /// <summary>
        /// Resolves the given coordinate to the closest routable point.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public RouterPoint Resolve(VehicleEnum vehicle, GeoCoordinate coordinate)
        {
            return this.Resolve(vehicle, coordinate, null, null);
        }

        /// <summary>
        /// Resolves the given coordinate to the closest routable point.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="matcher"></param>
        /// <returns></returns>
        public RouterPoint Resolve(VehicleEnum vehicle, GeoCoordinate coordinate, 
            IEdgeMatcher matcher, IDictionary<string, string> point_tags)
        {
            SearchClosestResult result = _router.SearchClosest(_data_graph, _interpreter, vehicle, coordinate, matcher, point_tags); // search the closest routable object.
            if (result.Distance < double.MaxValue)
            { // a routable object was found.
                if (!result.Vertex2.HasValue)
                { // the result was a single vertex.
                    float latitude, longitude;
                    if (!_data_graph.GetVertex(result.Vertex1.Value, out latitude, out longitude))
                    { // the vertex exists.
                        throw new Exception(string.Format("Vertex with id {0} not found!",
                            result.Vertex1.Value));
                    }
                    return new RouterPoint(result.Vertex1.Value, new GeoCoordinate(latitude, longitude));
                }
                else
                { // the result is on an edge.
                    return this.AddResolvedPoint(result.Vertex1.Value, result.Vertex2.Value, result.Position);
                }
            }
            return null; // no routable object was found closeby.
        }

        /// <summary>
        /// Resolves the given coordinates to the closest routable points.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public RouterPoint[] Resolve(VehicleEnum vehicle, GeoCoordinate[] coordinate)
        {
            RouterPoint[] points = new RouterPoint[coordinate.Length];
            for (int idx = 0; idx < coordinate.Length; idx++)
            {
                points[idx] = this.Resolve(vehicle, coordinate[idx]);
            }
            return points;
        }

        /// <summary>
        /// Resolves the given coordinates to the closest routable points.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="matcher"></param>
        /// <returns></returns>
        public RouterPoint[] Resolve(VehicleEnum vehicle, GeoCoordinate[] coordinate, IEdgeMatcher matcher, IDictionary<string, string>[] point_tags)
        {
            RouterPoint[] points = new RouterPoint[coordinate.Length];
            for (int idx = 0; idx < coordinate.Length; idx++)
            {
                points[idx] = this.Resolve(vehicle, coordinate[idx], matcher, point_tags[idx]);
            }
            return points;
        }

        /// <summary>
        /// Find the coordinates of the closest routable point.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public GeoCoordinate Search(VehicleEnum vehicle, GeoCoordinate coordinate)
        {
            SearchClosestResult result = _router.SearchClosest(_data_graph, _interpreter, vehicle, coordinate, null, null); // search the closest routable object.
            if (result.Distance < double.MaxValue)
            { // a routable object was found.
                if (!result.Vertex2.HasValue)
                { // the result was a single vertex.
                    float latitude, longitude;
                    if (!_data_graph.GetVertex(result.Vertex1.Value, out latitude, out longitude))
                    { // the vertex exists.
                        throw new Exception(string.Format("Vertex with id {0} not found!",
                            result.Vertex1.Value));
                    }
                    return new GeoCoordinate(latitude, longitude);
                }
                else
                { // the result is on an edge.
                    throw new NotImplementedException();
                }
            }
            return null; // no routable object was found closeby.
        }

        #region Resolved Points Graph

        /// <summary>
        /// Holds the id of the next resolved point.
        /// </summary>
        private long _next_resolved_id = -1;

        /// <summary>
        /// Returns the next resolved id.
        /// </summary>
        /// <returns></returns>
        private long GetNextResolvedId()
        {
            long next = _next_resolved_id;
            _next_resolved_id--;
            return next;
        }

        /// <summary>
        /// Holds the resolved graph.
        /// </summary>
        private RouterResolvedGraph _resolved_graph;

        /// <summary>
        /// Adds a resolved point to the graph.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="edge"></param>
        /// <returns></returns>
        private RouterPoint AddResolvedPoint(uint vertex1, uint vertex2, double position)
        {
            PathSegment<long> path = this.Shortest(vertex1, vertex2);
            float longitude1, latitude1, longitude2, latitude2;
            long[] vertices = new long[0];
            if(path != null)
            { // the vertices in this path.
                vertices = path.ToArray();
            }
            // should contain vertex1 and vertex2.
            if (vertices.Length == 0 || 
                (vertices[0] == vertex1 && vertices[vertices.Length - 1] == vertex2))
            { // the vertices match.
                if (_data_graph.GetVertex(vertex1, out latitude1, out longitude1) &&
                   _data_graph.GetVertex(vertex2, out latitude2, out longitude2))
                { // the two vertices are contained in the home graph.
                    GeoCoordinate vertex1_coordinate = new GeoCoordinate(
                        latitude1, longitude1);
                    GeoCoordinate vertex2_coordinate = new GeoCoordinate(
                        latitude2, longitude2);

                    GeoCoordinate resolved_coordinate = new GeoCoordinate(
                        latitude1 * (1.0 - position) + latitude2 * position,
                        longitude1 * (1.0 - position) + longitude2 * position);
                    if (vertices.Length == 0)
                    { // the path has a length of 0; the vertices are not in the resolved graph yet!

                        // add the vertices in the resolved graph.
                        float latitude_dummy, longitude_dummy;
                        if (!_resolved_graph.GetVertex(vertex1, out latitude_dummy, out longitude_dummy))
                        {
                            _resolved_graph.AddVertex(vertex1, latitude1, longitude1);
                        }
                        if (!_resolved_graph.GetVertex(vertex2, out latitude_dummy, out longitude_dummy))
                        {
                            _resolved_graph.AddVertex(vertex2, latitude2, longitude2);
                        }

                        // find the arc(s).
                        KeyValuePair<uint, EdgeData>? arc = null;
                        bool forward = true;
                        KeyValuePair<uint, EdgeData>[] arcs = _data_graph.GetArcs(vertex1);
                        for (int idx = 0; idx < arcs.Length; idx++)
                        {
                            if (arcs[idx].Key == vertex2)
                            { // arc is found!
                                arc = arcs[idx];
                            }
                        }
                        // find backward arc if needed.
                        if (!arc.HasValue)
                        {
                            forward = true;
                            arcs = _data_graph.GetArcs(vertex2);
                            for (int idx = 0; idx < arcs.Length; idx++)
                            {
                                if (arcs[idx].Key == vertex1)
                                { // arc is found!
                                    arc = arcs[idx];
                                }
                            }
                        }

                        // check if an arc was found!
                        if (!arc.HasValue)
                        {
                            throw new Exception("A resolved position can only exist on an arc between two routable vertices.");
                        }

                        // add the arc (in both directions)
                        RouterResolvedGraph.RouterResolvedGraphEdge resolved_edge_forward = 
                            new RouterResolvedGraph.RouterResolvedGraphEdge();
                        //if (forward)
                        //{
                        //    resolved_edge_forward.Backward = arc.Value.Value.Backward;
                        //    resolved_edge_forward.Forward = arc.Value.Value.Forward;
                        //}
                        //else
                        //{
                        //    resolved_edge_forward.Backward = !arc.Value.Value.Backward;
                        //    resolved_edge_forward.Forward = !arc.Value.Value.Forward;
                        //}
                        resolved_edge_forward.Tags = arc.Value.Value.Tags;
                        resolved_edge_forward.Weight = arc.Value.Value.Weight;
                        _resolved_graph.AddArc(vertex1, vertex2, resolved_edge_forward);
                        RouterResolvedGraph.RouterResolvedGraphEdge resolved_edge_backward =
                            new RouterResolvedGraph.RouterResolvedGraphEdge();
                        //if (!forward)
                        //{
                        //    resolved_edge_backward.Backward = arc.Value.Value.Backward;
                        //    resolved_edge_backward.Forward = arc.Value.Value.Forward;
                        //}
                        //else
                        //{
                        //    resolved_edge_backward.Backward = !arc.Value.Value.Backward;
                        //    resolved_edge_backward.Forward = !arc.Value.Value.Forward;
                        //}
                        resolved_edge_backward.Tags = arc.Value.Value.Tags;
                        resolved_edge_backward.Weight = arc.Value.Value.Weight;
                        _resolved_graph.AddArc(vertex2, vertex1, resolved_edge_backward);

                        // create the route manually.
                        vertices = new long[2];
                        vertices[0] = vertex1;
                        vertices[1] = vertex2;
                    }
                    else if(vertices.Length == 2)
                    { // paths of length two are impossible!
                        throw new Exception("A resolved position can only exist on an arc between two routable vertices.");
                    }

                    // calculate positions.
                    int position_idx = 0;
                    double total = vertex1_coordinate.Distance(vertex2_coordinate);
                    for (int idx = 1; idx < vertices.Length; idx++)
                    { // calculate positions.
                        float latitude_resolved, longitude_resolved;
                        if (_resolved_graph.GetVertex(vertices[idx], out latitude_resolved, out longitude_resolved))
                        { // vertex found
                            double current_position = (vertex1_coordinate.Distance(new GeoCoordinate(
                                latitude_resolved, longitude_resolved))) / total;
                            if (current_position > position)
                            { // the position is found.
                                position_idx = idx - 1;
                            }
                        }
                        else
                        { // oeps; vertex was not found!
                            throw new Exception(string.Format("Vertex with id {0} not found!", vertices[idx]));
                        }
                    }

                    // get the vertices and the arc between them.
                    long vertex_from = vertices[position_idx];
                    long vertex_to = vertices[position_idx + 1];

                    KeyValuePair<long, RouterResolvedGraph.RouterResolvedGraphEdge>? from_to_arc = null;
                    KeyValuePair<long, RouterResolvedGraph.RouterResolvedGraphEdge>[] from_arcs = 
                        _resolved_graph.GetArcs(vertex_from);
                    for (int idx = 0; idx < from_arcs.Length; idx++)
                    {
                        if (from_arcs[idx].Key == vertex_to)
                        { // arc is found!
                            from_to_arc = from_arcs[idx];
                        }
                    }

                    // check if an arc was found!
                    if (!from_to_arc.HasValue)
                    {
                        throw new Exception("A resolved position can only exist on an arc between two vertices.");
                    }

                    // remove the arc.
                    _resolved_graph.DeleteArc(vertex_from, vertex_to);
                    _resolved_graph.DeleteArc(vertex_to, vertex_from);

                    // calculate the relative position.
                    double relative_position = 0;
                    GeoCoordinate vertex_from_coordinate = null;
                    GeoCoordinate vertex_to_coordinate = null;
                    float latitude_resolved1, longitude_resolved1, latitude_resolved2, longitude_resolved2;
                    if (_resolved_graph.GetVertex(vertex_from, out latitude_resolved1, out longitude_resolved1) &&
                        _resolved_graph.GetVertex(vertex_to, out latitude_resolved2, out longitude_resolved2))
                    {
                        vertex_from_coordinate = new GeoCoordinate(
                            latitude_resolved1, longitude_resolved1);
                        vertex_to_coordinate = new GeoCoordinate(
                            latitude_resolved2, longitude_resolved2);
                        relative_position = vertex_from_coordinate.Distance(resolved_coordinate) /
                            vertex_from_coordinate.Distance(vertex_to_coordinate);
                    }
                    else
                    { // oeps; vertices not found!
                        throw new Exception("A resolved position can only exist on an arc between two routable vertices.");
                    }

                    // add new vertex.
                    long resolved_vertex = this.GetNextResolvedId();
                    _resolved_graph.AddVertex(resolved_vertex, (float)resolved_coordinate.Latitude,
                        (float)resolved_coordinate.Longitude);

                    // add the arcs.
                    RouterResolvedGraph.RouterResolvedGraphEdge from_resolved_edge =
                        new RouterResolvedGraph.RouterResolvedGraphEdge();
                    //from_resolved_edge.Backward = from_to_arc.Value.Value.Backward;
                    //from_resolved_edge.Forward = from_to_arc.Value.Value.Forward;
                    from_resolved_edge.Tags = from_to_arc.Value.Value.Tags;
                    from_resolved_edge.Weight = from_to_arc.Value.Value.Weight * relative_position;
                    _resolved_graph.AddArc(vertex_from, resolved_vertex, from_resolved_edge);
                    RouterResolvedGraph.RouterResolvedGraphEdge resolved_from_edge =
                        new RouterResolvedGraph.RouterResolvedGraphEdge();
                    //resolved_from_edge.Backward = !from_to_arc.Value.Value.Backward;
                    //resolved_from_edge.Forward = !from_to_arc.Value.Value.Forward;
                    resolved_from_edge.Tags = from_to_arc.Value.Value.Tags;
                    resolved_from_edge.Weight = from_to_arc.Value.Value.Weight * relative_position;
                    _resolved_graph.AddArc(resolved_vertex, vertex_from, resolved_from_edge);

                    // add the new arcs.
                    RouterResolvedGraph.RouterResolvedGraphEdge resolved_to_edge =
                        new RouterResolvedGraph.RouterResolvedGraphEdge();
                    //resolved_to_edge.Backward = from_to_arc.Value.Value.Backward;
                    //resolved_to_edge.Forward = from_to_arc.Value.Value.Forward;
                    resolved_to_edge.Tags = from_to_arc.Value.Value.Tags;
                    resolved_to_edge.Weight = from_to_arc.Value.Value.Weight * (1.0 - position);
                    _resolved_graph.AddArc(vertex_to, resolved_vertex, resolved_to_edge);
                    RouterResolvedGraph.RouterResolvedGraphEdge to_resolved_edge =
                        new RouterResolvedGraph.RouterResolvedGraphEdge();
                    //to_resolved_edge.Backward = !from_to_arc.Value.Value.Backward;
                    //to_resolved_edge.Forward = !from_to_arc.Value.Value.Forward;
                    to_resolved_edge.Tags = from_to_arc.Value.Value.Tags;
                    to_resolved_edge.Weight = from_to_arc.Value.Value.Weight * (1.0 - position);
                    _resolved_graph.AddArc(resolved_vertex, vertex_to, to_resolved_edge);

                    return new RouterPoint(resolved_vertex, resolved_coordinate);
                }
                else
                {
                    throw new Exception("A resolved position can only exist on an arc between two routable vertices.");
                }
            }
            else
            {
                throw new Exception("A shortest path between two vertices has to contain at least the source and target!");
            }
        }

        /// <summary>
        /// Calculates all routes from a given resolved point to the routable graph.
        /// </summary>
        /// <param name="resolved_point"></param>
        /// <param name="resolved_points"></param>
        /// <param name="visit_list"></param>
        /// <returns></returns>
        private PathSegmentVisitList RouteResolvedGraph(RouterPoint resolved_point)
        {
            // initialize the resulting visit list.
            PathSegmentVisitList result = new PathSegmentVisitList();

            // do a simple dykstra search and add all found routable vertices to the visit list.
            HashSet<long> settled = new HashSet<long>();
            PathSegmentVisitList visit_list = new PathSegmentVisitList();

            PathSegment<long> current = new PathSegment<long>(resolved_point.Id);
            visit_list.UpdateVertex(current);

            while (true)
            {
                // return the vertex on top of the list.
                current = visit_list.GetFirst();
                // update the settled list.
                if (current != null) { settled.Add(current.VertexId); }
                while (current != null && current.VertexId > 0)
                {
                    // add to the visit list.
                    result.UpdateVertex(current);

                    // choose a new current one.
                    current = visit_list.GetFirst();
                    // update the settled list.
                    if (current != null) { settled.Add(current.VertexId); }
                }

                // check if it is the target.
                if (current == null)
                { // current is empty; target not found!
                    return result;
                }

                // get the neighbours.
                KeyValuePair<long, RouterResolvedGraph.RouterResolvedGraphEdge>[] arcs =
                    _resolved_graph.GetArcs(current.VertexId);
                for (int idx = 0; idx < arcs.Length; idx++)
                {
                    KeyValuePair<long, RouterResolvedGraph.RouterResolvedGraphEdge> arc = arcs[idx];
                    if (!settled.Contains(arc.Key))
                    {
                        visit_list.UpdateVertex(new PathSegment<long>(arc.Key,
                            arc.Value.Weight + current.Weight, current));
                    }
                }
            }
        }

        /// <summary>
        /// Calculates all routes from all the given resolved points to the routable graph.
        /// </summary>
        /// <param name="resolved_points"></param>
        /// <returns></returns>
        private PathSegmentVisitList[] RouteResolvedGraph(RouterPoint[] resolved_points)
        {
            PathSegmentVisitList[] visit_lists = new PathSegmentVisitList[resolved_points.Length];
            for (int idx = 0; idx < resolved_points.Length; idx++)
            {
                visit_lists[idx] = this.RouteResolvedGraph(resolved_points[idx]);
            }
            return visit_lists;
        }

        //private KeyValuePair<HashSet<long>, PathSegmentVisitList> 

        #region Resolved Graph Routing

        /// <summary>
        /// Calculates the shortest path between two points in the resolved vertex.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <returns></returns>
        private PathSegment<long> Shortest(long vertex1, long vertex2)
        {
            HashSet<long> settled = new HashSet<long>();
            PathSegmentVisitList visit_list = new PathSegmentVisitList();

            PathSegment<long> current = new PathSegment<long>(vertex1);
            visit_list.UpdateVertex(current);

            while (true)
            {
                // return the vertex on top of the list.
                current = visit_list.GetFirst();


                // check if it is the target.
                if (current == null)
                { // current is empty; target not found!
                    return null;
                }
                if (current.VertexId == vertex2)
                { // current is the target.
                    return current;
                }

                // update the settled list.
                settled.Add(current.VertexId);

                // get the neighbours.
                KeyValuePair<long, RouterResolvedGraph.RouterResolvedGraphEdge>[] arcs = 
                    _resolved_graph.GetArcs(current.VertexId);
                for (int idx = 0; idx < arcs.Length; idx++)
                {
                    KeyValuePair<long, RouterResolvedGraph.RouterResolvedGraphEdge> arc = arcs[idx];
                    if (!settled.Contains(arc.Key))
                    {
                        visit_list.UpdateVertex(new PathSegment<long>(arc.Key,
                            arc.Value.Weight + current.Weight, current));
                    }
                }
            }
        }

        #endregion

        #endregion

        #endregion
    }
}