//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Osm.Core;
//using Tools.Math.Graph;
//using Tools.Math.Graph.Helpers;
//using Tools.Math.Graph.Routing;
//using Osm.Routing.Core.Graphs.Interpreter;
//using Osm.Routing.Core.Graphs;
//using Osm.Routing.Core.Metrics.Time;
////using Osm.Routing.Core.Metrics.Time;

//namespace Osm.Routing.Core.Route
//{
//    internal class OsmSharpRouteGenerator
//    {
//        /// <summary>
//        /// Generates an osm sharp route from a graph route.
//        /// </summary>
//        /// <param name="interpreter"></param>
//        /// <param name="graph_route"></param>
//        /// <param name="graph"></param>
//        /// <returns></returns>
//        internal static OsmSharpRoute Generate(GraphInterpreterBase interpreter, ResolvedPoint from_point, ResolvedPoint to_point,
//            GraphRoute<Way, GraphVertex> graph_route, Graph graph)
//        {
//            // create the route.
//            OsmSharpRoute route = null;

//            if (graph_route != null)
//            {
//                route = new OsmSharpRoute();

//                RoutePointEntry[] entries = OsmSharpRouteGenerator.GenerateEntries(interpreter, graph_route, graph);

//                // create the from routing point.
//                RoutePoint from = new RoutePoint();
//                from.Name = from_point.Name;
//                from.Latitude = (float)from_point.Original.Latitude;
//                from.Longitude = (float)from_point.Original.Longitude;
//                from.Tags = ConvertTo(from_point.Tags);
//                entries[0].Points = new RoutePoint[1];
//                entries[0].Points[0] = from;

//                // create the to routing point.
//                RoutePoint to = new RoutePoint();
//                to.Name = to_point.Name;
//                to.Latitude = (float)to_point.Original.Latitude;
//                to.Longitude = (float)to_point.Original.Longitude;
//                to.Tags = ConvertTo(to_point.Tags);
//                entries[entries.Length - 1].Points = new RoutePoint[1];
//                entries[entries.Length - 1].Points[0] = to;

//                // set the routing points.
//                route.Entries = entries.ToArray();

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
//        private static RoutePointEntry[] GenerateEntries(GraphInterpreterBase interpreter, GraphRoute<Way, GraphVertex> graph_route, Graph graph)
//        {
//            HashSet<GraphVertex> empty = new HashSet<GraphVertex>();

//            // create an entries list.
//            List<RoutePointEntry> entries = new List<RoutePointEntry>();

//            // create the first entry.
//            RoutePointEntry first = new RoutePointEntry();
//            first.Latitude = (float)graph_route.From.Vertex.Coordinate.Latitude;
//            first.Longitude = (float)graph_route.From.Vertex.Coordinate.Longitude;
//            first.Type = RoutePointEntryType.Start;
//            first.WayFromName = null;
//            first.WayFromNames = null;

//            entries.Add(first);

//            // create all the other entries except the last one.
//            GraphVertex node_previous = graph_route.From.Vertex;
//            for (int idx = 0; idx < graph_route.Entries.Count - 1; idx++)
//            {
//                // get all the data needed to calculate the next route entry.
//                GraphVertex node_current = graph_route.Entries[idx].To.Vertex;
//                GraphVertex node_next = graph_route.Entries[idx + 1].To.Vertex;
//                Way way_current = graph_route.Entries[idx].Edge;

//                // FIRST CALCULATE ALL THE ENTRY METRICS!

//                // STEP1: Get the names.
//                string name = interpreter.GetName(way_current);
//                Dictionary<string, string> names = interpreter.GetNamesInAllLanguages(way_current);

//                // STEP2: Get the side streets
//                IList<RoutePointEntrySideStreet> side_streets = new List<RoutePointEntrySideStreet>();
//                List<KeyValuePair<Way, GraphVertex>> neighbours = graph.GetNeighboursIgnoreOneWay(node_current);
//                if (neighbours.Count > 2)
//                {
//                    // construct neighbours list.
//                    foreach (KeyValuePair<Way, GraphVertex> neighbour in neighbours)
//                    {
//                        if (neighbour.Value != node_previous && neighbour.Value != node_next)
//                        {
//                            RoutePointEntrySideStreet side_street = new RoutePointEntrySideStreet();

//                            side_street.Latitude = (float)neighbour.Value.Coordinate.Latitude;
//                            side_street.Longitude = (float)neighbour.Value.Coordinate.Longitude;
//                            side_street.Tags = RouteTags.ConvertFrom(neighbour.Key.Tags);
//                            side_street.WayName = interpreter.GetName(neighbour.Key);
//                            side_street.WayNames = RouteTags.ConvertFrom(
//                                interpreter.GetNamesInAllLanguages(neighbour.Key));

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
//            if (graph_route.Entries.Count != 0)
//            {
//                int last_idx = graph_route.Entries.Count - 1;
//                Way way_last = graph_route.Entries[last_idx].Edge;
//                RoutePointEntry last = new RoutePointEntry();
//                last.Latitude = (float)graph_route.Entries[last_idx].To.Vertex.Coordinate.Latitude;
//                last.Longitude = (float)graph_route.Entries[last_idx].To.Vertex.Coordinate.Longitude;
//                last.Type = RoutePointEntryType.Stop;
//                last.WayFromName = interpreter.GetName(way_last);
//                last.WayFromNames = RouteTags.ConvertFrom(interpreter.GetNamesInAllLanguages(way_last));

//                entries.Add(last);
//            }

//            // return the result.
//            return entries.ToArray();
//        }
//    }
//}
