//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tools.Math.Graph.Routing.Point2Point;
//using Osm.Core;
//using Osm.Routing.Raw.Graphs;
//using Tools.Math.Units.Distance;
//using Tools.Math.Graph.Routing.Point2Point.Dykstra;
//using Tools.Math.Geo;

//namespace Osm.Routing.Raw.Controllers
//{
//    /// <summary>
//    /// A heuristic controller improving the stopping time of the routing algorithm when a point cannot be found.
//    /// </summary>
//    public class StoppingHeuristicController : IPoint2PointRouterController<Way, GraphVertex>
//    {
//        /// <summary>
//        /// The maximum distance until the search terminates.
//        /// </summary>
//        private double _max = 0;

//        /// <summary>
//        /// The current distance.
//        /// </summary>
//        private double _current = 0;

//        /// <summary>
//        /// The maximum distance unweighed.
//        /// </summary>
//        private double _max_unweighed;

//        /// <summary>
//        /// Notifies this controller that new vertices are about to be routed.
//        /// </summary>
//        /// <param name="vertices"></param>
//        public void NotifyVerticesToRoute(List<GraphVertex> vertices)
//        {
//            // reset the maximum first.
//            _max = 0;
//            _max_unweighed = 0;
//            _current = 0;

//            // make a rough estimate of the point farthest away.
//            for (int i = 0; i < vertices.Count; i++)
//            {
//                for (int j = 0; j < vertices.Count; j++)
//                {
//                    if (j < i)
//                    {
//                        double distance = vertices[i].Coordinate.DistanceReal(
//                            vertices[j].Coordinate).Value;
//                        if (distance > _max_unweighed)
//                        {
//                            _max_unweighed = distance;
//                        }
//                    }
//                }
//            }

//            // increase the maximum to make sure all point are routable.
//            // the estimate will improve after more routeable points are found.
//            _max = _max_unweighed * 10;
//        }

//        /// <summary>
//        /// Notifies this controller that the routing algorithm has selected a new vertex.
//        /// </summary>
//        /// <param name="vertex"></param>
//        public void NotifyVertexSelected(VertexReference<Way, GraphVertex> vertex)
//        {
//            double distance = 0;
//            VertexReference<Way, GraphVertex> current = vertex;
//            while (current.From != null)
//            {
//                distance = distance + current.Vertex.Coordinate.DistanceReal(
//                    current.From.Vertex.Coordinate).Value;
//                current = current.From;
//            }
//            if (distance > _current)
//            {
//                _current = distance;
//            }
//        }

//        /// <summary>
//        /// Return true if the stopping condition is fulfilled.
//        /// </summary>
//        /// <returns></returns>
//        public bool QueryStopConditions()
//        {
//            bool result = (_current > _max);
            
            
//            return result;
//        }

//        /// <summary>
//        /// The number of routable points found until now.
//        /// </summary>
//        private int _found_count;

//        /// <summary>
//        /// The average ratio between route length and crows-lenght.
//        /// </summary>
//        private double _average_ratio;

//        /// <summary>
//        /// Notifies this controller a routable point was found.
//        /// </summary>
//        /// <param name="route"></param>
//        public void NotifyFound(Tools.Math.Graph.Routing.GraphRoute<Way, GraphVertex> route)
//        {
//            if (route != null)
//            {
//                // calculate the distance of the route.
//                double distance = 0;
//                GeoCoordinate from = route.From.Vertex.Coordinate;
//                for (int i = 0; i < route.Entries.Count; i++)
//                {
//                    Tools.Math.Graph.Routing.GraphRouteEntry<Way, GraphVertex> entry
//                        = route.Entries[0];
//                    distance = distance + from.DistanceReal(entry.To.Vertex.Coordinate).Value;
//                    from = entry.To.Vertex.Coordinate;
//                }

//                // update the average.                        
//                double raw_distance = route.From.Vertex.Coordinate.DistanceReal(
//                            route.Entries[route.Entries.Count - 1 ].To.Vertex.Coordinate).Value;

//                // calculate average ratio.
//                double ratio = raw_distance / distance;
//                _average_ratio = ((_average_ratio * _found_count) + ratio) / (_found_count + 1);
//                _found_count++;

//                // update max and imoprove estimate.
//                _max = _max_unweighed * _average_ratio;
//            }
//        }
        
//        /// <summary>
//        /// Notifies this controller that a new graph is used for routing.
//        /// </summary>
//        /// <param name="graph"></param>
//        public void NotifyGraph(Tools.Math.Graph.Graph<Way, GraphVertex> graph)
//        {

//        }
//    }
//}
