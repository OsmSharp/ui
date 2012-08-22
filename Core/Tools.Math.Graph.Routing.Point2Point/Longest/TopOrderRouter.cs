//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tools.Math.Graph.Helpers;

//namespace Tools.Math.Graph.Routing.Longest
//{
//    /// <summary>
//    /// This class will calculate the longest route in a graph that is limited in size and acyclic!
//    /// 
//    /// http://en.wikipedia.org/wiki/Longest_path_problem
//    /// </summary>
//    public class TopOrderRouter<EdgeType, VertexType> : IPoint2PointRouter<EdgeType, VertexType>
//        where EdgeType : class
//        where VertexType : class, IEquatable<VertexType>
//    {  
//        /// <summary>
//        /// The graph the routing is being done on.
//        /// </summary>
//        private Graph<EdgeType, VertexType> _graph;

//        /// <summary>
//        /// Creates a new instance of this class.
//        /// </summary>
//        /// <param name="graph"></param>
//        public TopOrderRouter(Graph<EdgeType, VertexType> graph)
//        {
//            _graph = graph;
//        }

//        public void RegisterController(IPoint2PointRouterController<EdgeType, VertexType> controller)
//        {

//        }

//        public GraphRoute<EdgeType, VertexType> CalculateRoute(VertexType from, VertexType to)
//        {
//            HashSet<int> visited_nodes = new HashSet<int>();
//            Dictionary<VertexType, GraphRoute<EdgeType, VertexType>> length_to = new Dictionary<VertexType, GraphRoute<EdgeType, VertexType>>();

//            // calculate topoplogical order.
//            List<VertexType> vertices_without_incoming = new List<VertexType>();
//            vertices_without_incoming.Add(from); // WARNING: this is a BIG assumption!
//            List<VertexType> top_order = TopOrderAlgorithm<EdgeType,VertexType>.Calculate(_graph,
//                vertices_without_incoming);
//            foreach (VertexType v in top_order)
//            {
//                // get edges starting in v
//                HashSet<VertexAlongEdge<EdgeType, VertexType>> list = 
//                    _graph.GetNeighbours(v, null);

//                // get route to v
//                GraphRoute<EdgeType, VertexType> route_v = null;
//                if (!length_to.TryGetValue(v, out route_v))
//                {
//                    route_v = new GraphRoute<EdgeType, VertexType>(
//                        new GraphRouteEntryPoint<VertexType>(v, true), new List<GraphRouteEntry<EdgeType, VertexType>>(), 0);
//                    length_to.Add(v, route_v);
//                }

//                // loop over neighbours. 
//                foreach(VertexAlongEdge<EdgeType, VertexType> w_along in list)
//                {
//                    // get w.
//                    VertexType w = w_along.Vertex;

//                    // get route to w.
//                    GraphRoute<EdgeType, VertexType> route_w = null;
//                    if (!length_to.TryGetValue(w, out route_w))
//                    {
//                        // add the v-w route to the v-route to create the w-route.
//                        IList<GraphRouteEntry<EdgeType, VertexType>> entries =
//                            new List<GraphRouteEntry<EdgeType, VertexType>>(route_v.Entries);
//                        entries.Add(new GraphRouteEntry<EdgeType, VertexType>(
//                            new GraphRouteEntryPoint<VertexType>(w, true), w_along.Edge, w_along.Weight));
//                        float weight = route_v.Weight + w_along.Weight;

//                        // set weight
//                        route_w = new GraphRoute<EdgeType, VertexType>(
//                            route_v.From, entries, weight);
//                        length_to.Add(w, route_w);
//                    }
//                    else
//                    {
//                        // update weight.
//                        if (route_w.Weight <= route_v.Weight + w_along.Weight)
//                        {
//                            // add the v-w route to the v-route to create the w-route.
//                            IList<GraphRouteEntry<EdgeType, VertexType>> entries =
//                                new List<GraphRouteEntry<EdgeType, VertexType>>(route_v.Entries);
//                            entries.Add(new GraphRouteEntry<EdgeType, VertexType>(
//                                new GraphRouteEntryPoint<VertexType>(w, false), w_along.Edge, w_along.Weight));
//                            float weight = route_v.Weight + w_along.Weight;

//                            // set weight
//                            route_w = new GraphRoute<EdgeType, VertexType>(
//                                route_v.From, entries, weight);

//                            length_to[w] = route_w;
//                        }
//                    }
//                }
//            }

//            GraphRoute<EdgeType, VertexType> route = null;
//            if(length_to.TryGetValue(to,out route))
//            {
//                return route;
//            }
//            return null;
//        }

//        public GraphRoute<EdgeType, VertexType> CalculateRoute(List<VertexType> in_order)
//        {
//            throw new NotImplementedException();
//        }

//        public Dictionary<VertexType, GraphRoute<EdgeType, VertexType>> Calculate(VertexType from, List<VertexType> to)
//        {
//            throw new NotImplementedException();
//        }

//        public SortedDictionary<float, Dictionary<VertexType, GraphRoute<EdgeType, VertexType>>> CalculateClosest(VertexType from, List<VertexType> to)
//        {
//            throw new NotImplementedException();
//        }

//        public float Calculate(List<VertexType> vertices)
//        {
//            return 0;
//        }
//    }
//}
