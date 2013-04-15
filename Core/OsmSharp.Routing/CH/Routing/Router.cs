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
//using OsmSharp.Routing.Osm.Core;
//using OsmSharp.Routing.Osm.Core.Route;
//using OsmSharp.Routing.Osm.Core.Metrics.Time;
//using OsmSharp.Tools.Math.Geo;
//using OsmSharp.Osm.Data.Core.DynamicGraph;
//using OsmSharp.Routing.CH.PreProcessing;
//using OsmSharp.Routing.Osm.Core.Resolving;
//using OsmSharp.Routing;

//namespace OsmSharp.Routing.CH.Routing
//{
//    public class Router : IRouter<CHResolvedPoint>
//    {
//        private CHRouter _ch_router;

//        private CHDataSource _data;

//        public Router(CHDataSource data)
//        {
//            _data = data;
//            _ch_router = new CHRouter(data.Graph);
//            //_ch_router.NotifyPathSegment<long>Event += new CHRouter.NotifyPathSegment<long>Delegate(_ch_router_NotifyPathSegment<long>Event);
//        }

//        public bool SupportsVehicle(VehicleEnum vehicle)
//        {
//            return vehicle == VehicleEnum.Car;
//        }

//        public OsmSharpRoute Calculate(CHResolvedPoint source, CHResolvedPoint target)
//        {
//            return this.Calculate(source, target, float.MaxValue);
//        }

//        public OsmSharpRoute Calculate(CHResolvedPoint source, CHResolvedPoint target, float max)
//        {
//            if (source != null && target != null)
//            {
//                PathSegment<long> result = _ch_router.Calculate(source.Id, target.Id);
//                if (result != null)
//                {
//                    return this.ConstructRoute(source, target, result);
//                }
//            }
//            return null;
//        }

//        /// <summary>
//        /// Calculates a route between two given points.
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public OsmSharpRoute CalculateToClosest(CHResolvedPoint source, CHResolvedPoint[] targets)
//        {
//            return this.CalculateToClosest(source, targets, float.MaxValue);
//        }

//        /// <summary>
//        /// Calculates a route between two given points.
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public OsmSharpRoute CalculateToClosest(CHResolvedPoint source, CHResolvedPoint[] targets, float max)
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// Returns the weight between source and target.
//        /// </summary>
//        /// <param name="source"></param>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        public float CalculateWeight(CHResolvedPoint source, CHResolvedPoint target)
//        {
//            if (source != null && target != null)
//            { // the source/targets exist!
//                // quickwin: check for identical start/end points.
//                if (source.Id == target.Id ||
//                    (source.Location == target.Location))
//                { // the weight will always be zero when source and target are indentical.
//                    return 0;
//                }

//                // calculate the weight.
//                return (float)_ch_router.CalculateWeight(source.Id, target.Id);
//            }
//            return float.MaxValue;
//        }

//        public float[] CalculateOneToManyWeight(CHResolvedPoint source, CHResolvedPoint[] targets)
//        {
//            float[] weights = new float[targets.Length];
//            for (int idx = 0; idx < targets.Length; idx++)
//            {
//                weights[idx] = this.CalculateWeight(source, targets[idx]);
//            }
//            return weights;
//        }

//        public float[][] CalculateManyToManyWeight(CHResolvedPoint[] sources, CHResolvedPoint[] targets)
//        {
//            return _ch_router.CalculateManyToManyWeights(sources, targets);
//        }

//        public bool CheckConnectivity(CHResolvedPoint point, float weight)
//        {
//            return _ch_router.CheckConnectivity(point.Id, weight);
//        }

//        public bool[] CheckConnectivity(CHResolvedPoint[] point, float weight)
//        {
//            bool[] connectivities = new bool[point.Length];
//            for (int idx = 0; idx < point.Length; idx++)
//            {
//                connectivities[idx] = this.CheckConnectivity(point[idx], weight);

//                // report progress.
//                OsmSharp.Tools.Output.OutputStreamHost.ReportProgress(idx, point.Length, "Router.CH.CheckConnectivity",
//                    "Checking connectivity...");
//            }
//            return connectivities;
//        }

//        public bool IsCalculateRangeSupported
//        {
//            get
//            {
//                return false;
//            }
//        }

//        public HashSet<Osm.Core.ILocationObject> CalculateRange(CHResolvedPoint orgin, float weight)
//        {
//            throw new NotSupportedException();
//        }

//        public CHResolvedPoint Resolve(GeoCoordinate coordinate)
//        {
//            return _ch_router.Resolve(coordinate);
//        }

//        public CHResolvedPoint[] Resolve(GeoCoordinate[] coordinates)
//        {
//            CHResolvedPoint[] vertices = new CHResolvedPoint[coordinates.Length];
//            for (int idx = 0; idx < coordinates.Length; idx++)
//            {
//                vertices[idx] = _ch_router.Resolve(coordinates[idx]);
//            }
//            return vertices;
//        }

//        public CHResolvedPoint Resolve(GeoCoordinate coordinate, IResolveMatcher matcher)
//        {
//            throw new NotImplementedException();
//        }

//        public CHResolvedPoint[] Resolve(GeoCoordinate[] coordinate, IResolveMatcher matcher)
//        {
//            throw new NotImplementedException();
//        }

//        public GeoCoordinate Search(GeoCoordinate coordinate)
//        {
//            CHResolvedPoint vertex = this.Resolve(coordinate);
//            if (vertex != null)
//            {
//                return vertex.Location;
//            }
//            return null;
//        }

//        #region Route Construction

//        /// <summary>
//        /// Constructs an OsmSharpRoute form the Sparse calculated route.
//        /// </summary>
//        /// <param name="route"></param>
//        /// <param name="source"></param>
//        /// <param name="target"></param>
//        /// <returns></returns>
//        private OsmSharpRoute ConstructRoute(CHResolvedPoint source, CHResolvedPoint target, PathSegment<long> route)
//        {
//            if (route != null && route.From == null)
//            { // path contains just one single point.
//                // build the entries list.
//                List<RoutePointEntry> entries =
//                    new List<RoutePointEntry>();

//                // get the start point.
//                CHResolvedPoint from = this.GetCHVertex(route.VertexId);
//                RoutePointEntry entry = new RoutePointEntry();
//                entry.Type = RoutePointEntryType.Start;
//                entry.Latitude = (float)from.Location.Latitude;
//                entry.Longitude = (float)from.Location.Longitude;
//                entry.Points = new RoutePoint[1];
//                entry.Points[0] = new RoutePoint();
//                entry.Points[0].Distance = 0;
//                entry.Points[0].Latitude = (float)source.Location.Latitude;
//                entry.Points[0].Longitude = (float)source.Location.Longitude;
//                entry.Points[0].Name = source.Name;
//                entry.Points[0].Tags = RouteTags.ConvertFrom(source.Tags);
//                entry.Points[0].Time = 0;
//                entries.Add(entry);

//                // insert the stop point.
//                CHResolvedPoint to = this.GetCHVertex(route.VertexId);
//                entry = new RoutePointEntry();
//                entry.Distance = 0;
//                entry.Latitude = (float)to.Location.Latitude;
//                entry.Longitude = (float)to.Location.Longitude;
//                entry.Points = new RoutePoint[1];
//                entry.Points[0] = new RoutePoint();
//                entry.Points[0].Distance = 0;
//                entry.Points[0].Latitude = (float)target.Location.Latitude;
//                entry.Points[0].Longitude = (float)target.Location.Longitude;
//                entry.Points[0].Name = target.Name;
//                entry.Points[0].Tags = RouteTags.ConvertFrom(target.Tags);
//                entry.Points[0].Time = 0;
//                entry.Type = RoutePointEntryType.Stop;
//                entries.Add(entry);

//                //construct the actual route.
//                OsmSharpRoute osm_sharp_route = new OsmSharpRoute();

//                // set the routing points.
//                osm_sharp_route.Entries = entries.ToArray();

//                //calculate metrics.
//                TimeCalculator calculator = new TimeCalculator();
//                Dictionary<string, double> metrics = calculator.Calculate(osm_sharp_route);
//                osm_sharp_route.TotalDistance = metrics[TimeCalculator.DISTANCE_KEY];
//                osm_sharp_route.TotalTime = metrics[TimeCalculator.TIME_KEY];

//                return osm_sharp_route;
//            }
//            else
//            { // path contains more than one single point or none at all.
//                if (route != null && route.From != null)
//                {
//                    // build the entries list.
//                    List<RoutePointEntry> entries =
//                        new List<RoutePointEntry>();

//                    // get the start point.
//                    CHResolvedPoint to = this.GetCHVertex(route.VertexId);

//                    // insert the start point.
//                    RoutePointEntry entry = new RoutePointEntry();
//                    entry.Distance = 0;
//                    entry.Latitude = (float)to.Location.Latitude;
//                    entry.Longitude = (float)to.Location.Longitude;
//                    entry.Points = new RoutePoint[1];
//                    entry.Points[0] = new RoutePoint();
//                    entry.Points[0].Distance = 0;
//                    entry.Points[0].Latitude = (float)target.Location.Latitude;
//                    entry.Points[0].Longitude = (float)target.Location.Longitude;
//                    entry.Points[0].Name = target.Name;
//                    entry.Points[0].Tags = RouteTags.ConvertFrom(target.Tags);
//                    entry.Points[0].Time = 0;
//                    entry.Type = RoutePointEntryType.Stop;
//                    entries.Insert(0, entry);

//                    // loop over the route, insert the others.
//                    while (route.From != null)
//                    {
//                        // get the neighbours and retrieve the tags.
//                        KeyValuePair<uint, CHEdgeData>[] neighbours = this.GetArcs(route.VertexId);
//                        KeyValuePair<uint, CHEdgeData> arc = new KeyValuePair<uint, CHEdgeData>();
//                        foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours.Where<KeyValuePair<uint, CHEdgeData>>(
//                            a => a.Value.Backward && a.Key == route.From.VertexId))
//                        {
//                            if (arc.Value == null)
//                            {
//                                arc = neighbour;
//                            }
//                            else if (arc.Value.Weight > neighbour.Value.Weight)
//                            {
//                                arc = neighbour;
//                            }
//                        }

//                        // get the neighbours.
//                        neighbours = this.GetArcs(route.From.VertexId);

//                        //find the edge with lowest weight.
//                        foreach (KeyValuePair<uint, CHEdgeData> forward_arc in neighbours.Where<KeyValuePair<uint, CHEdgeData>>(
//                            a => a.Value.Forward && a.Key == route.VertexId))
//                        {
//                            if (arc.Value == null)
//                            {
//                                arc = forward_arc;
//                            }
//                            else if (arc.Value.Weight > forward_arc.Value.Weight)
//                            {
//                                arc = forward_arc;
//                            }
//                        }
//                        if (arc.Value == null)
//                        {
//                            // get the backward neighbours.
//                            neighbours = this.GetArcs(route.From.VertexId);
//                            foreach (KeyValuePair<uint, CHEdgeData> backward in neighbours.Where<KeyValuePair<uint, CHEdgeData>>(
//                                a => a.Value.Backward && a.Key == route.From.VertexId))
//                            {
//                                if (arc.Value == null)
//                                {
//                                    arc = backward;
//                                }
//                                else if (arc.Value.Weight > backward.Value.Weight)
//                                {
//                                    arc = backward;
//                                }
//                            }
//                        }

//                        // add the tags of the current arc to the previous point.
//                        if (arc.Value != null && arc.Value.HasTags)
//                        {
//                            entry.Tags = RouteTags.ConvertFrom(_data.Get(arc.Value.Tags));
//                        }

//                        // get the edge by querying the forward neighbours of the from-vertex.
//                        CHResolvedPoint vertex = this.GetCHVertex(route.From.VertexId);

//                        // add to the route.
//                        // TODO: get these tags from somewhere or find a better way to generate routing instructions?
//                        // probably there is a need to futher abstract the usages of OSM data into a more general format.
//                        entry = new RoutePointEntry();
//                        entry.Distance = 0;
//                        entry.Latitude = (float)vertex.Location.Latitude;
//                        entry.Longitude = (float)vertex.Location.Longitude;
//                        //entry.Tags = RouteTags.ConvertFrom( // 
//                        entry.Time = 0;
//                        entry.Type = RoutePointEntryType.Along;
//                        //entry.

//                        entries.Insert(0, entry);

//                        //get the routes.
//                        route = route.From;
//                    }

//                    // get the source points and add everything to the latest.
//                    CHResolvedPoint from = this.GetCHVertex(route.VertexId);
//                    if (entry != null)
//                    { // adjust the last entry to be the start entry.
//                        entry.Type = RoutePointEntryType.Start;
//                        entry.Points = new RoutePoint[1];
//                        entry.Points[0] = new RoutePoint();
//                        entry.Points[0].Distance = 0;
//                        entry.Points[0].Latitude = (float)source.Location.Latitude;
//                        entry.Points[0].Longitude = (float)source.Location.Longitude;
//                        entry.Points[0].Name = source.Name;
//                        entry.Points[0].Tags = RouteTags.ConvertFrom(source.Tags);
//                        entry.Points[0].Time = 0;
//                    }

//                    //construct the actual route.
//                    OsmSharpRoute osm_sharp_route = new OsmSharpRoute();

//                    // set the routing points.
//                    osm_sharp_route.Entries = entries.ToArray();

//                    //calculate metrics.
//                    TimeCalculator calculator = new TimeCalculator();
//                    Dictionary<string, double> metrics = calculator.Calculate(osm_sharp_route);
//                    osm_sharp_route.TotalDistance = metrics[TimeCalculator.DISTANCE_KEY];
//                    osm_sharp_route.TotalTime = metrics[TimeCalculator.TIME_KEY];

//                    return osm_sharp_route;
//                }
//                return null; // there was no route!
//            }
//        }

//        /// <summary>
//        /// Returns all arcs for a given vertex.
//        /// </summary>
//        /// <param name="vertex"></param>
//        /// <returns></returns>
//        private KeyValuePair<uint, CHEdgeData>[] GetArcs(uint vertex)
//        {
//            return _data.Graph.GetArcs(vertex);
//        }

//        /// <summary>
//        /// Returns the vertex that has the given id.
//        /// </summary>
//        /// <param name="vertex"></param>
//        /// <returns></returns>
//        public CHResolvedPoint GetCHVertex(uint vertex)
//        {
//            float latitude;
//            float longitude;
//            if (_data.Graph.GetVertex(vertex, out latitude, out longitude))
//            {
//                return new CHResolvedPoint(vertex, new GeoCoordinate(latitude, longitude));
//            }
//            throw new NotImplementedException();
//        }

//        //private class RouteVertex
//        //{
//        //    private CHResolvedPoint _vertex;

//        //    private IDictionary<string, string> _tags;

//        //    public RouteVertex(CHResolvedPoint vertex, IDictionary<string, string> tags)
//        //    {
//        //        _tags = tags;

//        //        _vertex = vertex;
//        //    }

//        //    public double Latitude
//        //    {
//        //        get
//        //        {
//        //            return _vertex.Location.Latitude;
//        //        }
//        //    }

//        //    public double Longitude
//        //    {
//        //        get
//        //        {
//        //            return _vertex.Location.Longitude;
//        //        }
//        //    }
//        //}

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        ///// <param name="route_list"></param>
//        ///// <returns></returns>
//        //private OsmSharpRoute Generate(List<RoutePointEntry> route_list)
//        //{
//        //     //create the route.
//        //    OsmSharpRoute route = null;

//        //    if (route_list != null)
//        //    {
//        //    }

//        //    return route;
//        //}

//        //private RoutePointEntry[] GenerateEntries(CHResolvedPoint from, CHResolvedPoint to,
//        //    List<RouteVertex> route_list)
//        //{
//        //    HashSet<CHResolvedPoint> empty = new HashSet<CHResolvedPoint>();

//        //     //create an entries list.
//        //    List<RoutePointEntry> entries = new List<RoutePointEntry>();

//        //     //create the first entry.
//        //    RoutePointEntry first = new RoutePointEntry();
//        //    first.Latitude = (float)from.Location.Latitude;
//        //    first.Longitude = (float)from.Location.Longitude;
//        //    first.Type = RoutePointEntryType.Start;
//        //    first.WayFromName = null;
//        //    first.WayFromNames = null;

//        //    entries.Add(first);

//        //     //create all the other entries except the last one.
//        //    RouteVertex node_previous = route_list[0];
//        //    for (int idx = 1; idx < route_list.Count - 1; idx++)
//        //    {
//        //         //get all the data needed to calculate the next route entry.
//        //        RouteVertex node_current = route_list[idx];
//        //        RouteVertex node_next = route_list[idx + 1];
//        //        //Way way_current = route_list[idx].Edge;

//        //        //FIRST CALCULATE ALL THE ENTRY METRICS!

//        //        //STEP1: Get the names.
//        //        // TODO: work out how to get proper instructions using CH!
//        //        string name = string.Empty; // _interpreter.GetName(way_current);
//        //        Dictionary<string, string> names = new Dictionary<string, string>(); //  _interpreter.GetNamesInAllLanguages(way_current);

//        //        // TODO: work out how to get proper instructions using CH!
//        //        // //STEP2: Get the side streets
//        //        IList<RoutePointEntrySideStreet> side_streets = new List<RoutePointEntrySideStreet>();
//        //        //Dictionary<long, VertexAlongEdge> neighbours = _graph.GetNeighboursUndirectedWithEdges(node_current.Id, null);
//        //        //if (neighbours.Count > 2)
//        //        //{
//        //        //    // construct neighbours list.
//        //        //    foreach (KeyValuePair<long, VertexAlongEdge> neighbour in neighbours)
//        //        //    {
//        //        //        if (neighbour.Value.Vertex != node_previous && neighbour.Value.Vertex != node_next)
//        //        //        {
//        //        //            RoutePointEntrySideStreet side_street = new RoutePointEntrySideStreet();

//        //        //            side_street.Latitude = (float)neighbour.Value.Vertex.Coordinate.Latitude;
//        //        //            side_street.Longitude = (float)neighbour.Value.Vertex.Coordinate.Longitude;
//        //        //            side_street.Tags = RouteTags.ConvertFrom(neighbour.Value.Edge.Tags);
//        //        //            side_street.WayName = _interpreter.GetName(neighbour.Value.Edge);
//        //        //            side_street.WayNames = RouteTags.ConvertFrom(
//        //        //                _interpreter.GetNamesInAllLanguages(neighbour.Value.Edge));

//        //        //            side_streets.Add(side_street);
//        //        //        }
//        //        //    }
//        //        //}

//        //         //create the route entry.
//        //        RoutePointEntry route_entry = new RoutePointEntry();
//        //        route_entry.Latitude = (float)node_current.Latitude;
//        //        route_entry.Longitude = (float)node_current.Longitude;
//        //        route_entry.SideStreets = side_streets.ToArray<RoutePointEntrySideStreet>();
//        //        route_entry.Tags = new RouteTags[0]; //route_entry.Tags = RouteTags.ConvertFrom(way_current.Tags);
//        //        route_entry.Type = RoutePointEntryType.Along;
//        //        route_entry.WayFromName = name;
//        //        route_entry.WayFromNames = RouteTags.ConvertFrom(names);
//        //        entries.Add(route_entry);

//        //         //set the previous node.
//        //        node_previous = node_current;
//        //    }

//        //     //create the last entry.
//        //    if (route_list.Count != 0)
//        //    {
//        //        int last_idx = route_list.Count - 1;
//        //        //Way way_last = route_list[last_idx].Edge;
//        //        RoutePointEntry last = new RoutePointEntry();
//        //        last.Latitude = (float)to.Location.Latitude;
//        //        last.Longitude = (float)to.Location.Longitude;
//        //        last.Type = RoutePointEntryType.Stop;
//        //        last.WayFromName = string.Empty; //_interpreter.GetName(way_last);
//        //        last.WayFromNames = new RouteTags[0];//  RouteTags.ConvertFrom(_interpreter.GetNamesInAllLanguages(way_last));

//        //        entries.Add(last);
//        //    }

//        //    // return the result.
//        //    return entries.ToArray();
//        //}

//        #endregion

//    //    #region Notifications

//    //     <summary>
//    //     The delegate for arc notifications.
//    //     </summary>
//    //     <param name="arc"></param>
//    //     <param name="contracted_id"></param>
//    //    public delegate void NotifyPathSegment<long>Delegate(PathSegment<long> route);

//    //     <summary>
//    //     The event.
//    //     </summary>
//    //    public event NotifyPathSegment<long>Delegate NotifyPathSegment<long>Event;

//    //     <summary>
//    //     Notifies the arc.
//    //     </summary>
//    //     <param name="arc"></param>
//    //     <param name="contracted_id"></param>
//    //    private void NotifyPathSegment<long>(PathSegment<long> route)
//    //    {
//    //        if (this.NotifyPathSegment<long>Event != null)
//    //        {
//    //            this.NotifyPathSegment<long>Event(route);
//    //        }
//    //    }

//    //     <summary>
//    //     Notify the path segments.
//    //     </summary>
//    //     <param name="route"></param>
//    //    void _ch_router_NotifyPathSegment<long>Event(PathSegment<long> route)
//    //    {
//    //        this.NotifyPathSegment<long>(route);
//    //    }

//    //    #endregion



//    }
//}
