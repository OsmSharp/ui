using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.Core;
using Osm.Routing.Core.Route;
using Tools.Math.Geo;
using Osm.Routing.Sparse.Routing.Graph;
using Osm.Routing.Sparse.Routing.Dykstra;
using Tools.Math.Graph.Routing;
using Osm.Routing.Core.Metrics.Time;
using Osm.Data.Core.Sparse;
using Osm.Data.Core.Sparse.Primitives;

namespace Osm.Routing.Sparse.Routing
{
    /// <summary>
    /// Router for the sparse vertex.
    /// </summary>
    public class Router : IRouter<SparseVertex>
    {
        /// <summary>
        /// The sparse graph.
        /// </summary>
        private SparseGraph _graph;

        /// <summary>
        /// The sparse data.
        /// </summary>
        private ISparseData _data;

        /// <summary>
        /// Creates a router.
        /// </summary>
        /// <param name="data"></param>
        public Router(ISparseData data)
        { // create a graph.
            _data = data;

            _graph = new SparseGraph(data);
        }

        #region Capabilities

        /// <summary>
        /// Returns true if the given vehicle type is supported.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool SupportsVehicle(VehicleEnum vehicle)
        {
            return vehicle == VehicleEnum.Car;
        }

        #endregion

        #region Routing

        /// <summary>
        /// Calculates a route between two given points.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public OsmSharpRoute Calculate(SparseVertex source, SparseVertex target)
        {
            DykstraRouting routing = new DykstraRouting(_graph);
            RouteLinked route = routing.Calculate(source.Id, target.Id);
            return this.ConstructRoute(route);
        }

        /// <summary>
        /// Calculates the weight between two given points.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public float CalculateWeight(SparseVertex source, SparseVertex target)
        {
            DykstraRouting routing = new DykstraRouting(_graph);
            RouteLinked route = routing.Calculate(source.Id, target.Id);
            return route.Weight;
        }

        /// <summary>
        /// Calculates a route between one source and many target points.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public float[] CalculateOneToManyWeight(SparseVertex source, SparseVertex[] targets)
        {
            Dykstra.DykstraRouting routing = new Dykstra.DykstraRouting(_graph);
            long[] tos = new long[targets.Length];
            for (int idx = 0; idx < targets.Length; idx++)
            {
                tos[idx] = targets[idx].Id;
            }
            return routing.CalculateOneToMany(source.Id, tos);
        }

        /// <summary>
        /// Calculates all routes between many sources/targets.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public float[][] CalculateManyToManyWeight(SparseVertex[] sources, SparseVertex[] targets)
        {
            Dykstra.DykstraRouting routing = new Dykstra.DykstraRouting(_graph);
            long[] froms = new long[sources.Length];
            for (int idx = 0; idx < sources.Length; idx++)
            {
                froms[idx] = sources[idx].Id;
            }
            long[] tos = new long[targets.Length];
            for (int idx = 0; idx < targets.Length; idx++)
            {
                tos[idx] = targets[idx].Id;
            }
            return routing.CalculateManyToMany(froms, tos);
        }

        #endregion

        #region Error Detection/Error Handling

        /// <summary>
        /// Returns true if the given point is connected for a radius of at least the given weight.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool CheckConnectivity(SparseVertex point, float weight)
        {
            Dykstra.DykstraRouting routing = new Dykstra.DykstraRouting(_graph);
            return routing.CheckConnectivity(point.Id, weight);
        }

        /// <summary>
        /// Returns an array of connectivity flags.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public bool[] CheckConnectivity(SparseVertex[] point, float weight)
        {
            bool[] connectivities = new bool[point.Length];
            for (int idx = 0; idx < point.Length; idx++)
            {
                connectivities[idx] = this.CheckConnectivity(point[idx], weight);
            }
            return connectivities;
        }

        #endregion

        #region Resolving

        /// <summary>
        /// Resolves the given coordinate.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public SparseVertex Resolve(GeoCoordinate coordinate)
        {
            return _graph.Resolve(coordinate, 0.0006f, null);
        }

        /// <summary>
        /// Resolves all the given coordinates.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public SparseVertex[] Resolve(GeoCoordinate[] coordinate)
        {
            SparseVertex[] resolved_vertices = new SparseVertex[coordinate.Length];
            for (int idx = 0; idx < coordinate.Length; idx++)
            {
                resolved_vertices[idx] = this.Resolve(coordinate[idx]);
            }
            return resolved_vertices;
        }

        public GeoCoordinate Search(GeoCoordinate search)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Route Construction

        /// <summary>
        /// Constructs an OsmSharpRoute form the Sparse calculated route.
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        private OsmSharpRoute ConstructRoute(RouteLinked route)
        {
            List<RouteVertex> route_list =
                new List<RouteVertex>();
            SparseVertex to = _graph.GetVertex(route.VertexId);
            while (route.From != null)
            {
                SparseVertex vertex = _graph.GetVertex(route.From.VertexId);

                if (vertex is SparseVertex)
                {
                    IDictionary<string, string> latest_tags = null;
                    foreach (SparseVertexNeighbour neighbour in (vertex as SparseVertex).Neighbours)
                    {
                        if (neighbour.Id == route.VertexId
                            && neighbour.Weight > 0)
                        {
                            for (int idx = neighbour.Nodes.Length - 1; idx >= 0; idx--)
                            //for (int idx = 0; idx < neighbour.Nodes.Length; idx++)
                            {
                                // get the vertex.
                                SparseSimpleVertex bypassed_vertex = _data.GetSparseSimpleVertex(neighbour.Nodes[idx]);
                                if (bypassed_vertex != null)
                                {
                                    // add to the route.
                                    route_list.Insert(0, new RouteVertex(bypassed_vertex, neighbour.Tags));
                                }
                            }
                            break;
                        }
                    }

                    // add to the route.
                    route_list.Insert(0, new RouteVertex(vertex as SparseVertex, latest_tags));
                }


                // get the routes.
                route = route.From;
            }
            SparseVertex from = _graph.GetVertex(route.VertexId);

            // construct the actual graph route.
            return this.Generate(from, to, route_list);
        }

        private class RouteVertex
        {
            private RouteVertexType _type;

            private SparseVertex _vertex;

            private SparseSimpleVertex _simple_vertex;

            private IDictionary<string, string> _tags;

            public RouteVertex(SparseVertex vertex, IDictionary<string, string> tags)
            {
                _type = RouteVertexType.Sparse;
                _tags = tags;

                _vertex = vertex;
            }

            public RouteVertex(SparseSimpleVertex simple_vertex, IDictionary<string, string> tags)
            {
                _type = RouteVertexType.Bypassed;
                _tags = tags;

                _simple_vertex = simple_vertex;
            }

            public double Latitude
            {
                get
                {
                    if (_type == RouteVertexType.Bypassed)
                    {
                        return _simple_vertex.Latitude;
                    }
                    else
                    {
                        return _vertex.Coordinates[0];
                    }
                }
            }
            
            public double Longitude
            {
                get
                {
                    if (_type == RouteVertexType.Bypassed)
                    {
                        return _simple_vertex.Longitude;
                    }
                    else
                    {
                        return _vertex.Coordinates[1];
                    }
                }
            }

            public enum RouteVertexType
            {
                Sparse,
                Bypassed
            }
        }

        /// <summary>
        /// Generates an osm sharp route from a graph route.
        /// </summary>
        /// <param name="from_point"></param>
        /// <param name="to_point"></param>
        /// <param name="route_list"></param>
        /// <returns></returns>
        private OsmSharpRoute Generate(SparseVertex from_point, SparseVertex to_point,
           List<RouteVertex> route_list)
        {
            // create the route.
            OsmSharpRoute route = null;

            if (route_list != null)
            {
                route = new OsmSharpRoute();

                RoutePointEntry[] entries = this.GenerateEntries(from_point, to_point, route_list);

                // create the from routing point.
                RoutePoint from = new RoutePoint();
                //from.Name = from_point.Name;
                from.Latitude = (float)from_point.Location.Latitude;
                from.Longitude = (float)from_point.Location.Longitude;
                //from.Tags = ConvertTo(from_point.Tags);
                entries[0].Points = new RoutePoint[1];
                entries[0].Points[0] = from;

                // create the to routing point.
                RoutePoint to = new RoutePoint();
                //to.Name = to_point.Name;
                to.Latitude = (float)to_point.Location.Latitude;
                to.Longitude = (float)to_point.Location.Longitude;
                //to.Tags = ConvertTo(to_point.Tags);
                entries[entries.Length - 1].Points = new RoutePoint[1];
                entries[entries.Length - 1].Points[0] = to;

                // set the routing points.
                route.Entries = entries.ToArray();

                // calculate metrics.
                TimeCalculator calculator = new TimeCalculator();
                Dictionary<string, double> metrics = calculator.Calculate(route);
                route.TotalDistance = metrics[TimeCalculator.DISTANCE_KEY];
                route.TotalTime = metrics[TimeCalculator.TIME_KEY];
            }

            return route;
        }

        private static RouteTags[] ConvertTo(List<KeyValuePair<string, string>> list)
        {
            RouteTags[] tags = null;

            if (list.Count > 0)
            {
                tags = new RouteTags[list.Count];
                for (int idx = 0; idx < list.Count; idx++)
                {
                    tags[idx] = new RouteTags()
                    {
                        Key = list[idx].Key,
                        Value = list[idx].Value
                    };
                }
            }

            return tags;
        }

        /// <summary>
        /// Generates a list of entries.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="graph_route"></param>
        /// <param name="graph"></param>
        /// <returns></returns>
        private RoutePointEntry[] GenerateEntries(SparseVertex from, SparseVertex to,
            List<RouteVertex> route_list)
        {
            HashSet<SparseVertex> empty = new HashSet<SparseVertex>();

            // create an entries list.
            List<RoutePointEntry> entries = new List<RoutePointEntry>();

            // create the first entry.
            RoutePointEntry first = new RoutePointEntry();
            first.Latitude = (float)from.Location.Latitude;
            first.Longitude = (float)from.Location.Longitude;
            first.Type = RoutePointEntryType.Start;
            first.WayFromName = null;
            first.WayFromNames = null;

            entries.Add(first);

            // create all the other entries except the last one.
            if (route_list.Count > 0)
            {
                RouteVertex node_previous = route_list[0];
                for (int idx = 1; idx < route_list.Count - 1; idx++)
                {
                    // get all the data needed to calculate the next route entry.
                    RouteVertex node_current = route_list[idx];
                    RouteVertex node_next = route_list[idx + 1];
                    //Way way_current = route_list[idx].Edge;

                    // FIRST CALCULATE ALL THE ENTRY METRICS!

                    // STEP1: Get the names.
                    //string name = _interpreter.GetName(way_current);
                    //Dictionary<string, string> names = _interpreter.GetNamesInAllLanguages(way_current);

                    // STEP2: Get the side streets
                    //IList<RoutePointEntrySideStreet> side_streets = new List<RoutePointEntrySideStreet>();
                    //Dictionary<long, VertexAlongEdge> neighbours = _graph.GetNeighboursUndirectedWithEdges(node_current.Id, null);
                    //if (neighbours.Count > 2)
                    //{
                    //    // construct neighbours list.
                    //    foreach (KeyValuePair<long, VertexAlongEdge> neighbour in neighbours)
                    //    {
                    //        if (neighbour.Value.Vertex != node_previous && neighbour.Value.Vertex != node_next)
                    //        {
                    //            RoutePointEntrySideStreet side_street = new RoutePointEntrySideStreet();

                    //            side_street.Latitude = (float)neighbour.Value.Vertex.Coordinate.Latitude;
                    //            side_street.Longitude = (float)neighbour.Value.Vertex.Coordinate.Longitude;
                    //            side_street.Tags = RouteTags.ConvertFrom(neighbour.Value.Edge.Tags);
                    //            side_street.WayName = _interpreter.GetName(neighbour.Value.Edge);
                    //            side_street.WayNames = RouteTags.ConvertFrom(
                    //                _interpreter.GetNamesInAllLanguages(neighbour.Value.Edge));

                    //            side_streets.Add(side_street);
                    //        }
                    //    }
                    //}

                    // create the route entry.
                    RoutePointEntry route_entry = new RoutePointEntry();
                    route_entry.Latitude = (float)node_current.Latitude;
                    route_entry.Longitude = (float)node_current.Longitude;
                    //route_entry.SideStreets = side_streets.ToArray<RoutePointEntrySideStreet>();
                    //route_entry.Tags = RouteTags.ConvertFrom(way_current.Tags);
                    route_entry.Type = RoutePointEntryType.Along;
                    //route_entry.WayFromName = name;
                    //route_entry.WayFromNames = RouteTags.ConvertFrom(names);
                    entries.Add(route_entry);

                    // set the previous node.
                    node_previous = node_current;
                }
            }

            // create the last entry.
            if (route_list.Count != 0)
            {
                int last_idx = route_list.Count - 1;
                //Way way_last = route_list[last_idx].Edge;
                RoutePointEntry last = new RoutePointEntry();
                last.Latitude = (float)to.Location.Latitude;
                last.Longitude = (float)to.Location.Longitude;
                last.Type = RoutePointEntryType.Stop;
                //last.WayFromName = _interpreter.GetName(way_last);
                //last.WayFromNames = RouteTags.ConvertFrom(_interpreter.GetNamesInAllLanguages(way_last));

                entries.Add(last);
            }

            // return the result.
            return entries.ToArray();
        }

        #endregion

    }
}
