using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.Core;
using Osm.Routing.Core.Route;
using Osm.Routing.Core.Metrics.Time;
using Tools.Math.Geo;
using Osm.Data.Core.DynamicGraph;
using Osm.Routing.CH.PreProcessing;
using Osm.Routing.Core.Resolving;

namespace Osm.Routing.CH.Routing
{
    public class Router : IRouter<CHResolvedPoint>
    {
        private CHRouter _ch_router;

        private IDynamicGraph<CHEdgeData> _data;

        public Router(IDynamicGraph<CHEdgeData> data)
        {
            _data = data;
            _ch_router = new CHRouter(data);
            //_ch_router.NotifyCHPathSegmentEvent += new CHRouter.NotifyCHPathSegmentDelegate(_ch_router_NotifyCHPathSegmentEvent);
        }

        public bool SupportsVehicle(VehicleEnum vehicle)
        {
            return vehicle == VehicleEnum.Car;
        }

        public OsmSharpRoute Calculate(CHResolvedPoint source, CHResolvedPoint target)
        {
            if (source != null && target != null)
            {
                CHPathSegment result = _ch_router.Calculate(source.Id, target.Id);
                if (result != null)
                {
                    return this.ConstructRoute(result);
                }
            }
            return null;
        }

        public float CalculateWeight(CHResolvedPoint source, CHResolvedPoint target)
        {
            if (source != null && target != null)
            {
                OsmSharpRoute route = this.Calculate(source, target);
                if (route != null)
                {
                    return (float)route.TotalTime;
                }
            }
            return float.MaxValue;
        }

        public float[] CalculateOneToManyWeight(CHResolvedPoint source, CHResolvedPoint[] targets)
        {
            float[] weights = new float[targets.Length];
            for (int idx = 0; idx < targets.Length; idx++)
            {
                weights[idx] = this.CalculateWeight(source, targets[idx]);
            }
            return weights;
        }

        public float[][] CalculateManyToManyWeight(CHResolvedPoint[] sources, CHResolvedPoint[] targets)
        {
            return _ch_router.CalculateManyToManyWeights(sources, targets);

            //float[][] weights = new float[sources.Length][];
            //for (int i = 0; i < sources.Length; i++)
            //{
            //    weights[i] = new float[targets.Length];
            //    for (int j = 0; j < targets.Length; j++)
            //    {
            //        weights[i][j] = (float)results[i][j];
            //    }

            //}
            //return weights;
        }

        public bool CheckConnectivity(CHResolvedPoint point, float weight)
        {
            throw new NotImplementedException();
        }

        public bool[] CheckConnectivity(CHResolvedPoint[] point, float weight)
        {
            throw new NotImplementedException();
        }



        public bool IsCalculateRangeSupported
        {
            get
            {
                return false;
            }
        }

        public HashSet<Osm.Core.ILocationObject> CalculateRange(CHResolvedPoint orgin, float weight)
        {
            throw new NotSupportedException();
        }

        public CHResolvedPoint Resolve(GeoCoordinate coordinate)
        {
            return _ch_router.Resolve(coordinate);
        }

        public CHResolvedPoint[] Resolve(GeoCoordinate[] coordinates)
        {
            CHResolvedPoint[] vertices = new CHResolvedPoint[coordinates.Length];
            for (int idx = 0; idx < coordinates.Length; idx++)
            {
                vertices[idx] = _ch_router.Resolve(coordinates[idx]);
            }
            return vertices;
        }

        public CHResolvedPoint Resolve(GeoCoordinate coordinate, IResolveMatcher<CHResolvedPoint> matcher)
        {
            throw new NotImplementedException();
        }

        public CHResolvedPoint[] Resolve(GeoCoordinate[] coordinate, IResolveMatcher<CHResolvedPoint> matcher)
        {
            throw new NotImplementedException();
        }

        public GeoCoordinate Search(GeoCoordinate coordinate)
        {
            CHResolvedPoint vertex = this.Resolve(coordinate);
            if (vertex != null)
            {
                return vertex.Location;
            }
            return null;
        }

        #region Route Construction

        /// <summary>
        /// Constructs an OsmSharpRoute form the Sparse calculated route.
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        private OsmSharpRoute ConstructRoute(CHPathSegment route)
        {
            List<RouteVertex> route_list =
                new List<RouteVertex>();
            CHResolvedPoint to = this.GetCHVertex(route.VertexId);
            while (route.From != null)
            {
                KeyValuePair<uint, CHEdgeData>[] neighbours = this.GetArcs(to.Id);

                KeyValuePair<uint, CHEdgeData> arc = new KeyValuePair<uint,CHEdgeData>();
                foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours.Where<KeyValuePair<uint, CHEdgeData>>(
                    a => a.Value.Backward && a.Key == route.VertexId))
                {
                    if (arc.Value == null)
                    {
                        arc = neighbour;
                    }
                    else if (arc.Value.Weight > neighbour.Value.Weight)
                    {
                        arc = neighbour;
                    }
                }

                // get the edge by querying the forward neighbours of the from-vertex.
                CHResolvedPoint vertex = this.GetCHVertex(route.From.VertexId);

                // get the neighbours.
                neighbours = this.GetArcs(route.From.VertexId);

                //find the edge with lowest weight.
                foreach (KeyValuePair<uint, CHEdgeData> forward_arc in neighbours.Where<KeyValuePair<uint, CHEdgeData>>(
                    a => a.Value.Forward && a.Key == route.VertexId))
                {
                    if (arc.Value == null)
                    {
                        arc = forward_arc;
                    }
                    else if (arc.Value.Weight > forward_arc.Value.Weight)
                    {
                        arc = forward_arc;
                    }
                }
                if (arc.Value == null)
                {
                    //CHVertex to_vertex = this.GetCHVertex(route.VertexId);

                    // get the backward neighbours.
                    neighbours = this.GetArcs(route.From.VertexId);
                    foreach (KeyValuePair<uint, CHEdgeData> backward in neighbours.Where<KeyValuePair<uint, CHEdgeData>>(
                        a => a.Value.Backward && a.Key == route.From.VertexId))
                    {
                        if (arc.Value == null)
                        {
                            arc = backward;
                        }
                        else if (arc.Value.Weight > backward.Value.Weight)
                        {
                            arc = backward;
                        }
                    }
                }

                // add to the route.
                // TODO: get these tags from somewhere or find a better way to generate routing instructions?
                // probably there is a need to futher abstract the usages of OSM data into a more general format.
                route_list.Insert(0, new RouteVertex(vertex, new Dictionary<string, string>()));

                 //get the routes.
                route = route.From;
            }
            CHResolvedPoint from = this.GetCHVertex(route.VertexId);

             //construct the actual graph route.
            return this.Generate(from, to, route_list);
        }

        private KeyValuePair<uint, CHEdgeData>[] GetArcs(uint p)
        {
            return _data.GetArcs(p);
        }

        public CHResolvedPoint GetCHVertex(uint p)
        {
            float latitude;
            float longitude;
            if(_data.GetVertex(p, out latitude, out longitude))
            {
                return new CHResolvedPoint(p, new GeoCoordinate(latitude, longitude));
            }
            throw new NotImplementedException();
        }

        private class RouteVertex
        {
            private CHResolvedPoint _vertex;

            private IDictionary<string, string> _tags;

            public RouteVertex(CHResolvedPoint vertex, IDictionary<string, string> tags)
            {
                _tags = tags;

                _vertex = vertex;
            }

            public double Latitude
            {
                get
                {
                    return _vertex.Location.Latitude;
                }
            }

            public double Longitude
            {
                get
                {
                    return _vertex.Location.Longitude;
                }
            }
        }

        private OsmSharpRoute Generate(CHResolvedPoint from_point, CHResolvedPoint to_point,
           List<RouteVertex> route_list)
        {
             //create the route.
            OsmSharpRoute route = null;

            if (route_list != null)
            {
                route = new OsmSharpRoute();

                RoutePointEntry[] entries = this.GenerateEntries(from_point, to_point, route_list);

                 //create the from routing point.
                RoutePoint from = new RoutePoint();
                from.Name = from_point.Name;
                from.Latitude = (float)from_point.Location.Latitude;
                from.Longitude = (float)from_point.Location.Longitude;
                from.Tags = ConvertTo(from_point.Tags);
                entries[0].Points = new RoutePoint[1];
                entries[0].Points[0] = from;

                 //create the to routing point.
                RoutePoint to = new RoutePoint();
                to.Name = to_point.Name;
                to.Latitude = (float)to_point.Location.Latitude;
                to.Longitude = (float)to_point.Location.Longitude;
                to.Tags = ConvertTo(to_point.Tags);
                entries[entries.Length - 1].Points = new RoutePoint[1];
                entries[entries.Length - 1].Points[0] = to;

                 ////set the routing points.
                route.Entries = entries.ToArray();

                 //calculate metrics.
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

            if (list != null && list.Count > 0)
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

        private RoutePointEntry[] GenerateEntries(CHResolvedPoint from, CHResolvedPoint to,
            List<RouteVertex> route_list)
        {
            HashSet<CHResolvedPoint> empty = new HashSet<CHResolvedPoint>();

             //create an entries list.
            List<RoutePointEntry> entries = new List<RoutePointEntry>();

             //create the first entry.
            RoutePointEntry first = new RoutePointEntry();
            first.Latitude = (float)from.Location.Latitude;
            first.Longitude = (float)from.Location.Longitude;
            first.Type = RoutePointEntryType.Start;
            first.WayFromName = null;
            first.WayFromNames = null;

            entries.Add(first);

             //create all the other entries except the last one.
            RouteVertex node_previous = route_list[0];
            for (int idx = 1; idx < route_list.Count - 1; idx++)
            {
                 //get all the data needed to calculate the next route entry.
                RouteVertex node_current = route_list[idx];
                RouteVertex node_next = route_list[idx + 1];
                //Way way_current = route_list[idx].Edge;

                //FIRST CALCULATE ALL THE ENTRY METRICS!

                //STEP1: Get the names.
                // TODO: work out how to get proper instructions using CH!
                string name = string.Empty; // _interpreter.GetName(way_current);
                Dictionary<string, string> names = new Dictionary<string, string>(); //  _interpreter.GetNamesInAllLanguages(way_current);

                // TODO: work out how to get proper instructions using CH!
                // //STEP2: Get the side streets
                IList<RoutePointEntrySideStreet> side_streets = new List<RoutePointEntrySideStreet>();
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

                 //create the route entry.
                RoutePointEntry route_entry = new RoutePointEntry();
                route_entry.Latitude = (float)node_current.Latitude;
                route_entry.Longitude = (float)node_current.Longitude;
                route_entry.SideStreets = side_streets.ToArray<RoutePointEntrySideStreet>();
                route_entry.Tags = new RouteTags[0]; //route_entry.Tags = RouteTags.ConvertFrom(way_current.Tags);
                route_entry.Type = RoutePointEntryType.Along;
                route_entry.WayFromName = name;
                route_entry.WayFromNames = RouteTags.ConvertFrom(names);
                entries.Add(route_entry);

                 //set the previous node.
                node_previous = node_current;
            }

             //create the last entry.
            if (route_list.Count != 0)
            {
                int last_idx = route_list.Count - 1;
                //Way way_last = route_list[last_idx].Edge;
                RoutePointEntry last = new RoutePointEntry();
                last.Latitude = (float)to.Location.Latitude;
                last.Longitude = (float)to.Location.Longitude;
                last.Type = RoutePointEntryType.Stop;
                last.WayFromName = string.Empty; //_interpreter.GetName(way_last);
                last.WayFromNames = new RouteTags[0];//  RouteTags.ConvertFrom(_interpreter.GetNamesInAllLanguages(way_last));

                entries.Add(last);
            }

            // return the result.
            return entries.ToArray();
        }

       #endregion

    //    #region Notifications

    //     <summary>
    //     The delegate for arc notifications.
    //     </summary>
    //     <param name="arc"></param>
    //     <param name="contracted_id"></param>
    //    public delegate void NotifyCHPathSegmentDelegate(CHPathSegment route);

    //     <summary>
    //     The event.
    //     </summary>
    //    public event NotifyCHPathSegmentDelegate NotifyCHPathSegmentEvent;

    //     <summary>
    //     Notifies the arc.
    //     </summary>
    //     <param name="arc"></param>
    //     <param name="contracted_id"></param>
    //    private void NotifyCHPathSegment(CHPathSegment route)
    //    {
    //        if (this.NotifyCHPathSegmentEvent != null)
    //        {
    //            this.NotifyCHPathSegmentEvent(route);
    //        }
    //    }

    //     <summary>
    //     Notify the path segments.
    //     </summary>
    //     <param name="route"></param>
    //    void _ch_router_NotifyCHPathSegmentEvent(CHPathSegment route)
    //    {
    //        this.NotifyCHPathSegment(route);
    //    }

    //    #endregion



    }
}
