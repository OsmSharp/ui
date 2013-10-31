// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Metrics.Time;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;

namespace OsmSharp.Routing.Routers
{
    /// <summary>
    /// A class that implements common functionality for any routing algorithm.
    /// </summary>
    internal abstract class TypedRouter<TEdgeData> : ITypedRouter
        where TEdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// The default search delta.
        /// </summary>
        private const float DefaultSearchDelta = .01f;

        /// <summary>
        /// Holds the graph object containing the routable network.
        /// </summary>
        private readonly IBasicRouterDataSource<TEdgeData> _dataGraph;

        /// <summary>
        /// Holds the basic router that works on the dynamic graph.
        /// </summary>
        private readonly IBasicRouter<TEdgeData> _router;

        /// <summary>
        /// Interpreter for the routing network.
        /// </summary>
        private readonly IRoutingInterpreter _interpreter;

        /// <summary>
        /// Creates a new router.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="interpreter"></param>
        /// <param name="router"></param>
        public TypedRouter(IBasicRouterDataSource<TEdgeData> graph, IRoutingInterpreter interpreter,
            IBasicRouter<TEdgeData> router)
        {
            _dataGraph = graph;
            _interpreter = interpreter;
            _router = router;

            _routerPoints = new Dictionary<GeoCoordinate, RouterPoint>();
            _resolvedGraphs = new Dictionary<Vehicle, TypedRouterResolvedGraph>();
        }

        /// <summary>
        /// Returns the routing interpreter.
        /// </summary>
        protected IRoutingInterpreter Interpreter
        {
            get { return _interpreter; }
        }

        /// <summary>
        /// Returns the data.
        /// </summary>
        protected IBasicRouterDataSource<TEdgeData> Data
        {
            get { return _dataGraph; }
        }

        /// <summary>
        /// Returns true if the given vehicle is supported.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public virtual bool SupportsVehicle(Vehicle vehicle)
        {
            return _dataGraph.SupportsProfile(vehicle);
        }

        /// <summary>
        /// Calculates a route from source to target.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public Route Calculate(Vehicle vehicle, RouterPoint source, RouterPoint target)
        {
            return this.Calculate(vehicle, source, target, float.MaxValue);
        }

        /// <summary>
        /// Calculates a route from source to target but does not search more than max around source or target location.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public Route Calculate(Vehicle vehicle, RouterPoint source, RouterPoint target, float max)
        {
            // check routing profiles.
            if (!this.SupportsVehicle(vehicle))
            {
                throw new ArgumentOutOfRangeException("vehicle", string.Format("Routing profile {0} not supported by this router!",
                    vehicle.ToString()));
            }

            // calculate the route.
            PathSegment<long> route = _router.Calculate(_dataGraph, _interpreter, vehicle,
                this.RouteResolvedGraph(vehicle, source), this.RouteResolvedGraph(vehicle, target), max);

            // convert to an OsmSharpRoute.
            return this.ConstructRoute(vehicle, route, source, target);
        }

        /// <summary>
        /// Calculates a route from source to the closest target point.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public Route CalculateToClosest(Vehicle vehicle, RouterPoint source, RouterPoint[] targets)
        {
            return this.CalculateToClosest(vehicle, source, targets, float.MaxValue);
        }

        /// <summary>
        /// Calculates a route from source to the closest target point but does not search more than max around source location.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public Route CalculateToClosest(Vehicle vehicle, RouterPoint source, RouterPoint[] targets, float max)
        {
            // check routing profiles.
            if (!this.SupportsVehicle(vehicle))
            {
                throw new ArgumentOutOfRangeException("vehicle", string.Format("Routing profile {0} not supported by this router!",
                    vehicle.ToString()));
            }

            // calculate the route.
            PathSegment<long> route = _router.CalculateToClosest(_dataGraph, _interpreter, vehicle,
                this.RouteResolvedGraph(vehicle, source), this.RouteResolvedGraph(vehicle, targets), max);

            // find the target.
            var target = targets.First(x => x.Id == route.VertexId);

            // convert to an OsmSharpRoute.
            return this.ConstructRoute(vehicle, route, source, target);
        }

        /// <summary>
        /// Calculates all the routes between the source and all given targets.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public Route[] CalculateOneToMany(Vehicle vehicle, RouterPoint source, RouterPoint[] targets)
        {
            return this.CalculateManyToMany(vehicle, new[] { source }, targets)[0];
        }

        /// <summary>
        /// Calculates all the routes between all the sources and all the targets.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public Route[][] CalculateManyToMany(Vehicle vehicle, RouterPoint[] sources, RouterPoint[] targets)
        {
            // check routing profiles.
            if (!this.SupportsVehicle(vehicle))
            {
                throw new ArgumentOutOfRangeException("vehicle", string.Format("Routing profile {0} not supported by this router!",
                    vehicle.ToString()));
            }

            PathSegment<long>[][] routes = _router.CalculateManyToMany(_dataGraph, _interpreter, vehicle, this.RouteResolvedGraph(vehicle, sources),
                this.RouteResolvedGraph(vehicle, targets), double.MaxValue);

            var constructedRoutes = new Route[sources.Length][];
            for (int x = 0; x < sources.Length; x++)
            {
                constructedRoutes[x] = new Route[targets.Length];
                for (int y = 0; y < targets.Length; y++)
                {
                    constructedRoutes[x][y] =
                        this.ConstructRoute(vehicle, routes[x][y], sources[x], targets[y]);
                }
            }

            return constructedRoutes;
        }


        /// <summary>
        /// Calculates the weight from source to target.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public double CalculateWeight(Vehicle vehicle, RouterPoint source, RouterPoint target)
        {
            // check routing profiles.
            if (!this.SupportsVehicle(vehicle))
            {
                throw new ArgumentOutOfRangeException("vehicle", string.Format("Routing profile {0} not supported by this router!",
                    vehicle.ToString()));
            }

            // calculate the route.
            return _router.CalculateWeight(_dataGraph, _interpreter, vehicle,
                this.RouteResolvedGraph(vehicle, source), this.RouteResolvedGraph(vehicle, target), float.MaxValue);
        }

        /// <summary>
        /// Calculates all the weights from source to all the targets.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public double[] CalculateOneToManyWeight(Vehicle vehicle, RouterPoint source, RouterPoint[] targets)
        {
            // check routing profiles.
            if (!this.SupportsVehicle(vehicle))
            {
                throw new ArgumentOutOfRangeException("vehicle", string.Format("Routing profile {0} not supported by this router!",
                    vehicle.ToString()));
            }

            return _router.CalculateOneToManyWeight(_dataGraph, _interpreter, vehicle, this.RouteResolvedGraph(vehicle, source),
                this.RouteResolvedGraph(vehicle, targets), double.MaxValue);
        }

        /// <summary>
        /// Calculates all the weights between all the sources and all the targets.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public double[][] CalculateManyToManyWeight(Vehicle vehicle, RouterPoint[] sources, RouterPoint[] targets)
        {
            // check routing profiles.
            if (!this.SupportsVehicle(vehicle))
            {
                throw new ArgumentOutOfRangeException("vehicle", string.Format("Routing profile {0} not supported by this router!",
                    vehicle.ToString()));
            }

            return _router.CalculateManyToManyWeight(_dataGraph, _interpreter, vehicle, this.RouteResolvedGraph(vehicle, sources),
                this.RouteResolvedGraph(vehicle, targets), double.MaxValue);
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
        /// <param name="vehicle"></param>
        /// <param name="orgin"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public HashSet<GeoCoordinate> CalculateRange(Vehicle vehicle, RouterPoint orgin, float weight)
        {
            // check routing profiles.
            if (!this.SupportsVehicle(vehicle))
            {
                throw new ArgumentOutOfRangeException("vehicle", string.Format("Routing profile {0} not supported by this router!",
                    vehicle.ToString()));
            }

            HashSet<long> objectsAtWeight = _router.CalculateRange(_dataGraph, _interpreter, vehicle, this.RouteResolvedGraph(vehicle, orgin),
                weight);

            var locations = new HashSet<GeoCoordinate>();
            foreach (long vertex in objectsAtWeight)
            {
                GeoCoordinate coordinate = this.GetCoordinate(vehicle, vertex);
                locations.Add(coordinate);
            }
            return locations;
        }

        /// <summary>
        /// Returns true if the given source is at least connected to vertices with at least a given weight.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="point"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool CheckConnectivity(Vehicle vehicle, RouterPoint point, float weight)
        {
            // check routing profiles.
            if (!this.SupportsVehicle(vehicle))
            {
                throw new ArgumentOutOfRangeException("vehicle", string.Format("Routing profile {0} not supported by this router!",
                    vehicle.ToString()));
            }

            return _router.CheckConnectivity(_dataGraph, _interpreter, vehicle, this.RouteResolvedGraph(vehicle, point), weight);
        }

        /// <summary>
        /// Returns an array of connectivity check results.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="point"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool[] CheckConnectivity(Vehicle vehicle, RouterPoint[] point, float weight)
        {
            // check routing profiles.
            if (!this.SupportsVehicle(vehicle))
            {
                throw new ArgumentOutOfRangeException("vehicle", string.Format("Routing profile {0} not supported by this router!",
                    vehicle.ToString()));
            }

            var connectivityArray = new bool[point.Length];
            for (int idx = 0; idx < point.Length; idx++)
            {
                connectivityArray[idx] = this.CheckConnectivity(vehicle, point[idx], weight);

                Logging.Log.TraceEvent("TypedRouter<TEdgeData>", System.Diagnostics.TraceEventType.Information, "Checking connectivity... {0}%",
                    (int)(((float)idx / (float)point.Length) * 100));
            }
            return connectivityArray;
        }

        #region OsmSharpRoute Building

        /// <summary>
        /// Converts a linked route to an OsmSharpRoute.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="route"></param>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private Route ConstructRoute(Vehicle vehicle, PathSegment<long> route, RouterPoint source, RouterPoint target)
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
        /// <param name="fromResolved"></param>
        /// <param name="toResolved"></param>
        /// <param name="vertices"></param>
        /// <returns></returns>
        internal Route Generate(
            Vehicle vehicle,
            RouterPoint fromResolved,
            RouterPoint toResolved,
            long[] vertices)
        {
            // create the route.
            Route route = null;

            if (vertices != null)
            {
                route = new Route();

                // set the vehicle.
                route.Vehicle = vehicle;

                RoutePointEntry[] entries;
                if (vertices.Length > 0)
                {
                    entries = this.GenerateEntries(vehicle, vertices);
                }
                else
                {
                    entries = new RoutePointEntry[0];
                }

                // create the from routing point.
                var from = new RoutePoint();
                //from.Name = from_point.Name;
                from.Latitude = (float)fromResolved.Location.Latitude;
                from.Longitude = (float)fromResolved.Location.Longitude;
                if (entries.Length > 0)
                {
                    entries[0].Points = new RoutePoint[1];
                    entries[0].Points[0] = from;
                    entries[0].Points[0].Tags = RouteTagsExtensions.ConvertFrom(fromResolved.Tags);
                }

                // create the to routing point.
                var to = new RoutePoint();
                //to.Name = to_point.Name;
                to.Latitude = (float)toResolved.Location.Latitude;
                to.Longitude = (float)toResolved.Location.Longitude;
                if (entries.Length > 0)
                {
                    //to.Tags = ConvertTo(to_point.Tags);
                    entries[entries.Length - 1].Points = new RoutePoint[1];
                    entries[entries.Length - 1].Points[0] = to;
                    entries[entries.Length - 1].Points[0].Tags = RouteTagsExtensions.ConvertFrom(toResolved.Tags);
                }

                // set the routing points.
                route.Entries = entries;

                // calculate metrics.
                var calculator = new TimeCalculator(_interpreter);
                Dictionary<string, double> metrics = calculator.Calculate(route);
                route.TotalDistance = metrics[TimeCalculator.DISTANCE_KEY];
                route.TotalTime = metrics[TimeCalculator.TIME_KEY];
            }

            return route;
        }

        /// <summary>
        ///     Generates a list of entries.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="vertices"></param>
        /// <returns></returns>
        private RoutePointEntry[] GenerateEntries(Vehicle vehicle, long[] vertices)
        {
            // create an entries list.
            var entries = new List<RoutePointEntry>();

            // create the first entry.
            GeoCoordinate coordinate = this.GetCoordinate(vehicle, vertices[0]);
            var first = new RoutePointEntry();
            first.Latitude = (float)coordinate.Latitude;
            first.Longitude = (float)coordinate.Longitude;
            first.Type = RoutePointEntryType.Start;
            first.WayFromName = null;
            first.WayFromNames = null;

            entries.Add(first);

            // create all the other entries except the last one.
            long nodePrevious = vertices[0];
            for (int idx = 1; idx < vertices.Length - 1; idx++)
            {
                // get all the data needed to calculate the next route entry.
                long nodeCurrent = vertices[idx];
                //long nodeNext = vertices[idx + 1];
                IDynamicGraphEdgeData edge = this.GetEdgeData(vehicle, nodePrevious, nodeCurrent);

                // FIRST CALCULATE ALL THE ENTRY METRICS!

                // STEP1: Get the names.
                TagsCollection currentTags = _dataGraph.TagsIndex.Get(edge.Tags);
                string name = _interpreter.EdgeInterpreter.GetName(currentTags);
                Dictionary<string, string> names = _interpreter.EdgeInterpreter.GetNamesInAllLanguages(currentTags);

                // STEP2: Get the side streets
                IList<RoutePointEntrySideStreet> sideStreets = new List<RoutePointEntrySideStreet>();
                Dictionary<long, IDynamicGraphEdgeData> neighbours = this.GetNeighboursUndirectedWithEdges(
                    vehicle, nodeCurrent);
                if (neighbours.Count > 2)
                {
                    // construct neighbours list.
                    foreach (var neighbour in neighbours)
                    {
                        if (neighbour.Key != nodePrevious && neighbour.Key != vertices[idx + 1])
                        {
                            var sideStreet = new RoutePointEntrySideStreet();

                            GeoCoordinate neighbourCoordinate = this.GetCoordinate(vehicle, neighbour.Key);
                            TagsCollection tags = _dataGraph.TagsIndex.Get(neighbour.Value.Tags);

                            sideStreet.Latitude = (float)neighbourCoordinate.Latitude;
                            sideStreet.Longitude = (float)neighbourCoordinate.Longitude;
                            sideStreet.Tags = tags.ConvertFrom();
                            sideStreet.WayName = _interpreter.EdgeInterpreter.GetName(tags);
                            sideStreet.WayNames = _interpreter.EdgeInterpreter.GetNamesInAllLanguages(tags).ConvertFrom();

                            sideStreets.Add(sideStreet);
                        }
                    }
                }

                // create the route entry.
                GeoCoordinate nextCoordinate = this.GetCoordinate(vehicle, nodeCurrent);

                var routeEntry = new RoutePointEntry();
                routeEntry.Latitude = (float)nextCoordinate.Latitude;
                routeEntry.Longitude = (float)nextCoordinate.Longitude;
                routeEntry.SideStreets = sideStreets.ToArray();
                routeEntry.Tags = currentTags.ConvertFrom();
                routeEntry.Type = RoutePointEntryType.Along;
                routeEntry.WayFromName = name;
                routeEntry.WayFromNames = names.ConvertFrom();
                entries.Add(routeEntry);

                // set the previous node.
                nodePrevious = nodeCurrent;
            }

            // create the last entry.
            if (vertices.Length > 1)
            {
                int last_idx = vertices.Length - 1;
                IDynamicGraphEdgeData edge = this.GetEdgeData(vehicle, vertices[last_idx - 1], vertices[last_idx]);
                TagsCollection tags = _dataGraph.TagsIndex.Get(edge.Tags);
                coordinate = this.GetCoordinate(vehicle, vertices[last_idx]);
                var last = new RoutePointEntry();
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
        /// <param name="vehicle"></param>
        /// <param name="vertex1"></param>
        /// <returns></returns>
        private Dictionary<long, IDynamicGraphEdgeData> GetNeighboursUndirectedWithEdges(Vehicle vehicle, 
            long vertex1)
        {
            // get the resolved graph for the given profile.
            TypedRouterResolvedGraph graph = this.GetForProfile(vehicle);

            var neighbours = new Dictionary<long, IDynamicGraphEdgeData>();
            if (vertex1 > 0)
            {
                KeyValuePair<uint, TEdgeData>[] arcs = this.GetNeighboursUndirected(vertex1);
                foreach (KeyValuePair<uint, TEdgeData> arc in arcs)
                {
                    neighbours[arc.Key] = arc.Value;
                }
            }
            else
            {
                KeyValuePair<long, TypedRouterResolvedGraph.RouterResolvedGraphEdge>[] arcs = graph.GetArcs(vertex1);
                foreach (KeyValuePair<long, TypedRouterResolvedGraph.RouterResolvedGraphEdge> arc in arcs)
                {
                    neighbours[arc.Key] = arc.Value;
                }
            }
            return neighbours;
        }

        /// <summary>
        /// Returns all the arcs representing neighbours for the given vertex.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <returns></returns>
        protected virtual KeyValuePair<uint, TEdgeData>[] GetNeighboursUndirected(long vertex1)
        {
            return _dataGraph.GetArcs(Convert.ToUInt32(vertex1));
        }

        /// <summary>
        /// Returns the edge data between two neighbouring vertices.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <returns></returns>
        private IDynamicGraphEdgeData GetEdgeData(Vehicle vehicle, long vertex1, long vertex2)
        {
            // get the resolved graph for the given profile.
            TypedRouterResolvedGraph graph = this.GetForProfile(vehicle);

            if (vertex1 > 0 && vertex2 > 0)
            { // none of the vertixes was a resolved vertex.
                KeyValuePair<uint, TEdgeData>[] arcs = _dataGraph.GetArcs(Convert.ToUInt32(vertex1));
                foreach (KeyValuePair<uint, TEdgeData> arc in arcs)
                {
                    if (arc.Key == vertex2)
                    {
                        return arc.Value;
                    }
                }
                arcs = _dataGraph.GetArcs(Convert.ToUInt32(vertex2));
                foreach (KeyValuePair<uint, TEdgeData> arc in arcs)
                {
                    if (arc.Key == vertex1)
                    {
                        var edge = new TypedRouterResolvedGraph.RouterResolvedGraphEdge(
                            arc.Value.Tags, !arc.Value.Forward);
                        return edge;
                    }
                }
            }
            else
            { // one of the vertices was a resolved vertex.
                // edge should be in the resolved graph.
                KeyValuePair<long, TypedRouterResolvedGraph.RouterResolvedGraphEdge>[] arcs = graph.GetArcs(vertex1);
                foreach (KeyValuePair<long, TypedRouterResolvedGraph.RouterResolvedGraphEdge> arc in arcs)
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
        /// <param name="vehicle"></param>
        /// <param name="vertex"></param>
        /// <returns></returns>
        private GeoCoordinate GetCoordinate(Vehicle vehicle, long vertex)
        {
            // get the resolved graph for the given profile.
            TypedRouterResolvedGraph graph = this.GetForProfile(vehicle);

            float latitude, longitude;
            if (vertex < 0)
            { // the vertex is resolved.
                if (!graph.GetVertex(vertex, out latitude, out longitude))
                {
                    throw new Exception(string.Format("Vertex with id {0} not found in resolved graph!",
                        vertex));
                }
            }
            else
            { // the vertex should be in the data graph.
                if (!_dataGraph.GetVertex(Convert.ToUInt32(vertex), out latitude, out longitude))
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
        /// Holds all resolved points.
        /// </summary>
        private readonly Dictionary<GeoCoordinate, RouterPoint> _routerPoints;

        /// <summary>
        /// Normalizes the router point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private RouterPoint Normalize(RouterPoint point)
        {
            RouterPoint normalize;
            if (!_routerPoints.TryGetValue(point.Location, out normalize))
            {
                _routerPoints.Add(point.Location, point);
                normalize = point;
            }
            return normalize;
        }

        /// <summary>
        /// Resolves the given coordinate to the closest routable point.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, GeoCoordinate coordinate)
        {
            return this.Resolve(vehicle, coordinate, false);
        }

        /// <summary>
        /// Resolves the given coordinate to the closest routable point.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="coordinate"></param>
        /// <param name="verticesOnly"></param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, GeoCoordinate coordinate, bool verticesOnly)
        {
            return this.Resolve(vehicle, TypedRouter<TEdgeData>.DefaultSearchDelta, coordinate, null, null, verticesOnly);
        }

        /// <summary>
        /// Resolves the given coordinate to the closest routable point.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="delta"></param>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, float delta, GeoCoordinate coordinate)
        {
            return this.Resolve(vehicle, delta, coordinate, false);
        }

        /// <summary>
        /// Resolves the given coordinate to the closest routable point.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="delta"></param>
        /// <param name="coordinate"></param>
        /// <param name="verticesOnly"></param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, float delta, GeoCoordinate coordinate, bool verticesOnly)
        {
            return this.Resolve(vehicle, delta, coordinate, null, null, verticesOnly);
        }

        /// <summary>
        /// Resolves the given coordinate to the closest routable point.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="coordinate"></param>
        /// <param name="pointTags"></param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, GeoCoordinate coordinate, TagsCollection pointTags)
        {
            return this.Resolve(vehicle, coordinate, pointTags, false);
        }

        /// <summary>
        /// Resolves the given coordinate to the closest routable point.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="coordinate"></param>
        /// <param name="pointTags"></param>
        /// <param name="verticesOnly"></param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, GeoCoordinate coordinate, TagsCollection pointTags, bool verticesOnly)
        {
            return this.Resolve(vehicle, TypedRouter<TEdgeData>.DefaultSearchDelta, coordinate, pointTags, verticesOnly);
        }

        /// <summary>
        /// Resolves the given coordinate to the closest routable point.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="delta"></param>
        /// <param name="coordinate"></param>
        /// <param name="pointTags"></param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, float delta, GeoCoordinate coordinate, TagsCollection pointTags)
        {
            return this.Resolve(vehicle, delta, coordinate, pointTags, false);
        }

        /// <summary>
        /// Resolves the given coordinate to the closest routable point.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="delta"></param>
        /// <param name="coordinate"></param>
        /// <param name="pointTags"></param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, float delta, GeoCoordinate coordinate, TagsCollection pointTags, bool verticesOnly)
        {
            return this.Resolve(vehicle, delta, coordinate, null, pointTags, verticesOnly);
        }

        /// <summary>
        /// Resolves the given coordinate to the closest routable point.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="coordinate"></param>
        /// <param name="matcher"></param>
        /// <param name="matchingTags"></param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, GeoCoordinate coordinate,
            IEdgeMatcher matcher, TagsCollection matchingTags)
        {
            return this.Resolve(vehicle, coordinate, matcher, matchingTags, false);
        }

        /// <summary>
        /// Resolves the given coordinate to the closest routable point.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="coordinate"></param>
        /// <param name="matcher"></param>
        /// <param name="matchingTags"></param>
        /// <param name="verticesOnly"></param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, GeoCoordinate coordinate,
            IEdgeMatcher matcher, TagsCollection matchingTags, bool verticesOnly)
        {
            return this.Resolve(vehicle, TypedRouter<TEdgeData>.DefaultSearchDelta, coordinate,
                                matcher, matchingTags, verticesOnly);
        }

        /// <summary>
        /// Resolves the given coordinate to the closest routable point.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="delta"></param>
        /// <param name="coordinate"></param>
        /// <param name="matcher"></param>
        /// <param name="matchingTags"></param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, float delta, GeoCoordinate coordinate,
                                   IEdgeMatcher matcher, TagsCollection matchingTags)
        {
            return this.Resolve(vehicle, delta, coordinate, matcher, matchingTags, false);
        }

        /// <summary>
        /// Resolves the given coordinate to the closest routable point.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="delta"></param>
        /// <param name="coordinate"></param>
        /// <param name="matcher"></param>
        /// <param name="matchingTags"></param>
        /// <param name="verticesOnly"></param>
        /// <returns></returns>
        public RouterPoint Resolve(Vehicle vehicle, float delta, GeoCoordinate coordinate,
                                   IEdgeMatcher matcher, TagsCollection matchingTags, bool verticesOnly)
        {
            // check routing profiles.
            if (!this.SupportsVehicle(vehicle))
            {
                throw new ArgumentOutOfRangeException("vehicle", string.Format("Routing profile {0} not supported by this router!",
                    vehicle.ToString()));
            }

            SearchClosestResult result = _router.SearchClosest(_dataGraph, _interpreter,
                vehicle, coordinate, delta, matcher, matchingTags, verticesOnly); // search the closest routable object.
            if (result.Distance < double.MaxValue)
            { // a routable object was found.
                if (!result.Vertex2.HasValue)
                { // the result was a single vertex.
                    float latitude, longitude;
                    if (!_dataGraph.GetVertex(result.Vertex1.Value, out latitude, out longitude))
                    { // the vertex exists.
                        throw new Exception(string.Format("Vertex with id {0} not found!",
                            result.Vertex1.Value));
                    }
                    return this.Normalize(
                        new RouterPoint(result.Vertex1.Value, new GeoCoordinate(latitude, longitude)));
                }
                else
                { // the result is on an edge.
                    return this.Normalize(
                        this.AddResolvedPoint(vehicle, result.Vertex1.Value, result.Vertex2.Value, result.Position));
                }
            }
            return null; // no routable object was found closeby.
        }

        /// <summary>
        /// Resolves the given coordinates to the closest routable points.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public RouterPoint[] Resolve(Vehicle vehicle, GeoCoordinate[] coordinate)
        {
            return this.Resolve(vehicle, TypedRouter<TEdgeData>.DefaultSearchDelta, coordinate);
        }

        /// <summary>
        /// Resolves the given coordinates to the closest routable points.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="delta"></param>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public RouterPoint[] Resolve(Vehicle vehicle, float delta, GeoCoordinate[] coordinate)
        {
            var points = new RouterPoint[coordinate.Length];
            for (int idx = 0; idx < coordinate.Length; idx++)
            {
                points[idx] = this.Resolve(vehicle, delta, coordinate[idx]);
            }
            return points;
        }

        /// <summary>
        /// Resolves the given coordinates to the closest routable points.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="coordinate"></param>
        /// <param name="matcher"></param>
        /// <param name="matchingTags"></param>
        /// <returns></returns>
        public RouterPoint[] Resolve(Vehicle vehicle, GeoCoordinate[] coordinate,
            IEdgeMatcher matcher, TagsCollection[] matchingTags)
        {
            return this.Resolve(vehicle, TypedRouter<TEdgeData>.DefaultSearchDelta, coordinate,
                                matcher, matchingTags);
        }

        /// <summary>
        /// Resolves the given coordinates to the closest routable points.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="delta"></param>
        /// <param name="coordinate"></param>
        /// <param name="matcher"></param>
        /// <param name="matchingTags"></param>
        /// <returns></returns>
        public RouterPoint[] Resolve(Vehicle vehicle, float delta, GeoCoordinate[] coordinate,
                                     IEdgeMatcher matcher, TagsCollection[] matchingTags)
        {
            var points = new RouterPoint[coordinate.Length];
            for (int idx = 0; idx < coordinate.Length; idx++)
            {
                points[idx] = this.Resolve(vehicle, delta, coordinate[idx], matcher, matchingTags[idx]);
            }
            return points;
        }

        /// <summary>
        /// Find the coordinates of the closest routable point.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public GeoCoordinate Search(Vehicle vehicle, GeoCoordinate coordinate)
        {
            return this.Search(vehicle, coordinate, false);
        }

        /// <summary>
        /// Find the coordinates of the closest routable point.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="delta"></param>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public GeoCoordinate Search(Vehicle vehicle, float delta, GeoCoordinate coordinate)
        {
            return this.Search(vehicle, delta, coordinate, false);
        }

        /// <summary>
        /// Find the coordinates of the closest routable point.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="coordinate"></param>
        /// <param name="verticesOnly"></param>
        /// <returns></returns>
        public GeoCoordinate Search(Vehicle vehicle, GeoCoordinate coordinate, bool verticesOnly)
        {
            // check routing profiles.
            if (!this.SupportsVehicle(vehicle))
            {
                throw new ArgumentOutOfRangeException("vehicle", string.Format("Routing profile {0} not supported by this router!",
                    vehicle.ToString()));
            }

            return this.Search(vehicle, TypedRouter<TEdgeData>.DefaultSearchDelta, coordinate, verticesOnly);
        }

        /// <summary>
        /// Find the coordinates of the closest routable point.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="delta"></param>
        /// <param name="coordinate"></param>
        /// <param name="verticesOnly"></param>
        /// <returns></returns>
        public GeoCoordinate Search(Vehicle vehicle, float delta, GeoCoordinate coordinate, bool verticesOnly)
        {
            // check routing profiles.
            if (!this.SupportsVehicle(vehicle))
            {
                throw new ArgumentOutOfRangeException("vehicle", string.Format("Routing profile {0} not supported by this router!",
                    vehicle.ToString()));
            }

            SearchClosestResult result = _router.SearchClosest(_dataGraph, _interpreter, vehicle, coordinate,
                delta, null, null, verticesOnly); // search the closest routable object.
            if (result.Distance < double.MaxValue)
            { // a routable object was found.
                if (!result.Vertex2.HasValue)
                { // the result was a single vertex.
                    float latitude, longitude;
                    if (!_dataGraph.GetVertex(result.Vertex1.Value, out latitude, out longitude))
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
        private long _nextResolvedId = -1;

        /// <summary>
        /// Returns the next resolved id.
        /// </summary>
        /// <returns></returns>
        private long GetNextResolvedId()
        {
            long next = _nextResolvedId;
            _nextResolvedId--;
            return next;
        }

        /// <summary>
        /// Holds the resolved graph.
        /// </summary>
        private readonly Dictionary<Vehicle, TypedRouterResolvedGraph> _resolvedGraphs;

        /// <summary>
        /// Gets/creates a TypedRouterResolvedGraph for the given profile.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        private TypedRouterResolvedGraph GetForProfile(Vehicle vehicle)
        {
            TypedRouterResolvedGraph graph;
            if (!_resolvedGraphs.TryGetValue(vehicle, out graph))
            {
                graph = new TypedRouterResolvedGraph();
                _resolvedGraphs.Add(vehicle, graph);
            }
            return graph;
        }

        /// <summary>
        /// Adds a resolved point to the graph.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private RouterPoint AddResolvedPoint(Vehicle vehicle, uint vertex1, uint vertex2, double position)
        {
            // get the resolved graph for the given profile.
            TypedRouterResolvedGraph graph = this.GetForProfile(vehicle);

            PathSegment<long> path = this.Shortest(vehicle, vertex1, vertex2);
            var vertices = new long[0];
            if (path != null)
            { // the vertices in this path.
                vertices = path.ToArray();
            }
            // should contain vertex1 and vertex2.
            if (vertices.Length == 0 ||
                (vertices[0] == vertex1 && vertices[vertices.Length - 1] == vertex2))
            {
                // the vertices match.
                float longitude1, latitude1, longitude2, latitude2;
                if (_dataGraph.GetVertex(vertex1, out latitude1, out longitude1) &&
                   _dataGraph.GetVertex(vertex2, out latitude2, out longitude2))
                { // the two vertices are contained in the home graph.
                    var vertex1Coordinate = new GeoCoordinate(
                        latitude1, longitude1);
                    var vertex2Coordinate = new GeoCoordinate(
                        latitude2, longitude2);

                    var resolvedCoordinate = new GeoCoordinate(
                        latitude1 * (1.0 - position) + latitude2 * position,
                        longitude1 * (1.0 - position) + longitude2 * position);
                    if (vertices.Length == 0)
                    { // the path has a length of 0; the vertices are not in the resolved graph yet!

                        // add the vertices in the resolved graph.
                        float latitudeDummy, longitudeDummy;
                        if (!graph.GetVertex(vertex1, out latitudeDummy, out longitudeDummy))
                        {
                            graph.AddVertex(vertex1, latitude1, longitude1);
                        }
                        if (!graph.GetVertex(vertex2, out latitudeDummy, out longitudeDummy))
                        {
                            graph.AddVertex(vertex2, latitude2, longitude2);
                        }

                        // find the arc(s).
                        KeyValuePair<uint, TEdgeData>? arc = null;
                        KeyValuePair<uint, TEdgeData>[] arcs = _dataGraph.GetArcs(vertex1);
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
                            arcs = _dataGraph.GetArcs(vertex2);
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
                        graph.AddArc(vertex1, vertex2, 
                            new TypedRouterResolvedGraph.RouterResolvedGraphEdge(arc.Value.Value.Tags,
                                                                                 arc.Value.Value.Forward));
                        graph.AddArc(vertex2, vertex1, 
                            new TypedRouterResolvedGraph.RouterResolvedGraphEdge(arc.Value.Value.Tags,
                                                                                !arc.Value.Value.Forward));

                        // create the route manually.
                        vertices = new long[2];
                        vertices[0] = vertex1;
                        vertices[1] = vertex2;
                    }
                    else if (vertices.Length == 2)
                    { // paths of length two are impossible!
                        throw new Exception("A resolved position can only exist on an arc between two routable vertices.");
                    }

                    // calculate positions.
                    int positionIdx = 0;
                    double total = vertex1Coordinate.Distance(vertex2Coordinate);
                    for (int idx = 1; idx < vertices.Length; idx++)
                    { // calculate positions.
                        float latitudeResolved, longitudeResolved;
                        if (graph.GetVertex(vertices[idx], out latitudeResolved, out longitudeResolved))
                        { // vertex found
                            double currentPosition = (vertex1Coordinate.Distance(new GeoCoordinate(
                                latitudeResolved, longitudeResolved))) / total;
                            if (currentPosition > position)
                            { // the position is found.
                                positionIdx = idx - 1;
                                break;
                            }
                        }
                        else
                        { // oeps; vertex was not found!
                            throw new Exception(string.Format("Vertex with id {0} not found!", vertices[idx]));
                        }
                    }

                    // get the vertices and the arc between them.
                    long vertexFrom = vertices[positionIdx];
                    long vertexTo = vertices[positionIdx + 1];

                    KeyValuePair<long, TypedRouterResolvedGraph.RouterResolvedGraphEdge>? fromToArc = null;
                    KeyValuePair<long, TypedRouterResolvedGraph.RouterResolvedGraphEdge>[] fromArcs =
                        graph.GetArcs(vertexFrom);
                    for (int idx = 0; idx < fromArcs.Length; idx++)
                    {
                        if (fromArcs[idx].Key == vertexTo)
                        { // arc is found!
                            fromToArc = fromArcs[idx];
                        }
                    }

                    // check if an arc was found!
                    if (!fromToArc.HasValue)
                    {
                        throw new Exception("A resolved position can only exist on an arc between two vertices.");
                    }

                    // remove the arc.
                    graph.DeleteArc(vertexFrom, vertexTo);
                    graph.DeleteArc(vertexTo, vertexFrom);

                    // add new vertex.
                    long resolvedVertex = this.GetNextResolvedId();
                    graph.AddVertex(resolvedVertex, (float)resolvedCoordinate.Latitude,
                        (float)resolvedCoordinate.Longitude);

                    // add the arcs.
                    graph.AddArc(vertexFrom, resolvedVertex,
                                          new TypedRouterResolvedGraph.RouterResolvedGraphEdge(
                                              fromToArc.Value.Value.Tags,
                                              fromToArc.Value.Value.Forward));
                    graph.AddArc(resolvedVertex, vertexFrom,
                                          new TypedRouterResolvedGraph.RouterResolvedGraphEdge(
                                              fromToArc.Value.Value.Tags,
                                              !fromToArc.Value.Value.Forward));
                    graph.AddArc(resolvedVertex, vertexTo,
                                          new TypedRouterResolvedGraph.RouterResolvedGraphEdge(
                                              fromToArc.Value.Value.Tags,
                                              fromToArc.Value.Value.Forward));
                    graph.AddArc(vertexTo, resolvedVertex,
                                          new TypedRouterResolvedGraph.RouterResolvedGraphEdge(
                                              fromToArc.Value.Value.Tags,
                                              !fromToArc.Value.Value.Forward));

                    return new RouterPoint(resolvedVertex, resolvedCoordinate);
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
        /// <param name="vehicle"></param>
        /// <param name="resolvedPoint"></param>
        /// <returns></returns>
        private PathSegmentVisitList RouteResolvedGraph(Vehicle vehicle, RouterPoint resolvedPoint)
        {
            // get the resolved graph for the given profile.
            TypedRouterResolvedGraph graph = this.GetForProfile(vehicle);

            // initialize the resulting visit list.
            var result = new PathSegmentVisitList();

            // do a simple dykstra search and add all found routable vertices to the visit list.
            var settled = new HashSet<long>();
            var visitList = new PathSegmentVisitList();

            var current = new PathSegment<long>(resolvedPoint.Id);
            visitList.UpdateVertex(current);

            while (true)
            {
                // return the vertex on top of the list.
                current = visitList.GetFirst();
                // update the settled list.
                if (current != null) { settled.Add(current.VertexId); }
                while (current != null && current.VertexId > 0)
                {
                    // add to the visit list.
                    result.UpdateVertex(current);

                    // choose a new current one.
                    current = visitList.GetFirst();
                    // update the settled list.
                    if (current != null) { settled.Add(current.VertexId); }
                }

                // check if it is the target.
                if (current == null)
                { // current is empty; target not found!
                    return result;
                }

                // get the neighbours.
                KeyValuePair<long, TypedRouterResolvedGraph.RouterResolvedGraphEdge>[] arcs =
                    graph.GetArcs(current.VertexId);
                float latitude, longitude;
                graph.GetVertex(current.VertexId, out latitude, out longitude);
                var currentCoordinates = new GeoCoordinate(latitude, longitude);
                for (int idx = 0; idx < arcs.Length; idx++)
                {
                    KeyValuePair<long, TypedRouterResolvedGraph.RouterResolvedGraphEdge> arc = arcs[idx];
                    if (!settled.Contains(arc.Key))
                    {
                        // check oneway.
                        TagsCollection tags = _dataGraph.TagsIndex.Get(arc.Value.Tags);
                        bool? oneway = vehicle.IsOneWay(tags);
                        if (!oneway.HasValue || oneway.Value == arc.Value.Forward)
                        { // ok edge is not oneway or oneway in the right direction.
                            graph.GetVertex(arc.Key, out latitude, out longitude);
                            var neighbourCoordinates = new GeoCoordinate(latitude, longitude);

                            // calculate the weight.
                            double weight = vehicle.Weight(tags, currentCoordinates, neighbourCoordinates);

                            visitList.UpdateVertex(new PathSegment<long>(arc.Key,
                                                                         weight + current.Weight, current));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates all routes from all the given resolved points to the routable graph.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="resolvedPoints"></param>
        /// <returns></returns>
        private PathSegmentVisitList[] RouteResolvedGraph(Vehicle vehicle, RouterPoint[] resolvedPoints)
        {
            var visitLists = new PathSegmentVisitList[resolvedPoints.Length];
            for (int idx = 0; idx < resolvedPoints.Length; idx++)
            {
                visitLists[idx] = this.RouteResolvedGraph(vehicle, resolvedPoints[idx]);
            }
            return visitLists;
        }

        #region Resolved Graph Routing

        /// <summary>
        /// Calculates the shortest path between two points in the resolved vertex.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <returns></returns>
        private PathSegment<long> Shortest(Vehicle vehicle, long vertex1, long vertex2)
        {
            // get the resolved graph for the given profile.
            TypedRouterResolvedGraph graph = this.GetForProfile(vehicle);

            var settled = new HashSet<long>();
            var current = new PathSegment<long>(vertex1);
            var visit_list = new PathSegmentVisitList(vertex1, vertex1);
            visit_list.UpdateVertex(current);

            while (true)
            {
                // return the vertex on top of the list.
                current = visit_list.GetFirst();

                // check if it is the target.
                if (current == null)
                {
                    // current is empty; target not found!
                    return null;
                }
                if (current.VertexId == vertex2)
                {
                    // current is the target.
                    return current;
                }

                // update the settled list.
                settled.Add(current.VertexId);

                // get the neighbours.
                KeyValuePair<long, OsmSharp.Routing.Routers.TypedRouterResolvedGraph.RouterResolvedGraphEdge>[] arcs = 
                    graph.GetArcs(current.VertexId);
                float latitudeCurrent, longitudeCurrent;
                graph.GetVertex(current.VertexId, out latitudeCurrent, out longitudeCurrent);
                for (int idx = 0; idx < arcs.Length; idx++)
                {
                    KeyValuePair<long, OsmSharp.Routing.Routers.TypedRouterResolvedGraph.RouterResolvedGraphEdge> arc = arcs[idx];
                    if (!settled.Contains(arc.Key) && (arc.Key < 0 || arc.Key == vertex2))
                    {
                        float latitudeNeighbour, longitudeNeighbour;
                        graph.GetVertex(arc.Key, out latitudeNeighbour, out longitudeNeighbour);

                        double arcWeight = vehicle.Weight(_dataGraph.TagsIndex.Get(arc.Value.Tags),
                            new GeoCoordinate(latitudeCurrent, longitudeCurrent), new GeoCoordinate(latitudeNeighbour, longitudeNeighbour));

                        visit_list.UpdateVertex(new PathSegment<long>(arc.Key, arcWeight + current.Weight, current));
                    }
                }
            }
        }

        #endregion

        #endregion

        #endregion
    }
}