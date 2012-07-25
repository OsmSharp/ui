//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tools.Math.Graph.Routing.Longest;
//using Tools.Math.Graph.Helpers;

//namespace Tools.Math.Graph.Routing.All
//{
//    public class AllRouter<EdgeType, VertexType>
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
//        public AllRouter(Graph<EdgeType, VertexType> graph)
//        {
//            _graph = graph;
//        }

//        public List<GraphRoute<EdgeType, VertexType>> CalculateAll(VertexType from, VertexType to)
//        {
//            Dictionary<VertexType, List<AllBackRoute<VertexType, EdgeType>>> length_to = 
//                new Dictionary<VertexType, List<AllBackRoute<VertexType, EdgeType>>>();
//            length_to.Add(from, new List<AllBackRoute<VertexType, EdgeType>>());

//            // calculate topoplogical order.
//            List<VertexType> vertices_without_incoming = new List<VertexType>();
//            vertices_without_incoming.Add(from); // WARNING: this is a BIG assumption!
//            List<VertexType> top_order = TopOrderAlgorithm<EdgeType, VertexType>.Calculate(_graph,
//                vertices_without_incoming);

//            foreach (VertexType v in top_order)
//            {
//                // get edges starting in v
//                HashSet<VertexAlongEdge<EdgeType, VertexType>> list =
//                    _graph.GetNeighbours(v, null);

//                // loop over neighbours. 
//                foreach (VertexAlongEdge<EdgeType, VertexType> w_along in list)
//                {
//                    // get w.
//                    VertexType w = w_along.Vertex;

//                    // get routes list for w.
//                    List<AllBackRoute<VertexType, EdgeType>> back_routes_w = null;
//                    if (!length_to.TryGetValue(w, out back_routes_w))
//                    {
//                        back_routes_w = new List<AllBackRoute<VertexType, EdgeType>>();
//                        length_to.Add(w, back_routes_w);
//                    }
                    
//                    // create back route.
//                    AllBackRoute<VertexType, EdgeType> back_route = new AllBackRoute<VertexType, EdgeType>();
//                    back_route.Previous = v;
//                    back_route.WeightToPrevious = w_along.Weight;
//                    back_route.PreviousEdge = w_along.Edge;

//                    back_routes_w.Add(back_route);
//                }
//            }

//            List<AllBackRoute<VertexType, EdgeType>> back_routes = null;
//            if (length_to.TryGetValue(to, out back_routes))
//            {
//                return this.ConvertBackRoutes(length_to,back_routes,to);
//            }
//            return null;
//        }

//        private List<GraphRoute<EdgeType, VertexType>> ConvertBackRoutes(
//            Dictionary<VertexType, List<AllBackRoute<VertexType,EdgeType>>> length_to, 
//            List<AllBackRoute<VertexType,EdgeType>> back_routes,
//            VertexType v)
//        {
//            List<GraphRoute<EdgeType, VertexType>> routes = new List<GraphRoute<EdgeType, VertexType>>();
//            if (back_routes.Count == 0)
//            { // can only be the orgine that has been reached without any back routes.
//                GraphRoute<EdgeType, VertexType> route =
//                    new GraphRoute<EdgeType, VertexType>(
//                        new GraphRouteEntryPoint<VertexType>(v, true),
//                        new List<GraphRouteEntry<EdgeType, VertexType>>(),
//                        0);
//                routes.Add(route);
//            }
//            else
//            {
//                foreach (AllBackRoute<VertexType, EdgeType> back_route in back_routes)
//                {
//                    List<GraphRoute<EdgeType, VertexType>> routes_from_back =
//                        this.ConvertBackRoutes(length_to, length_to[back_route.Previous], back_route.Previous);

//                    foreach (GraphRoute<EdgeType, VertexType> route_from_back in routes_from_back)
//                    {
//                        GraphRouteEntry<EdgeType, VertexType> new_entry =
//                            new GraphRouteEntry<EdgeType, VertexType>(
//                                new GraphRouteEntryPoint<VertexType>(v, false),
//                                back_route.PreviousEdge,
//                                back_route.WeightToPrevious);

//                        // add to the route.
//                        IList<GraphRouteEntry<EdgeType, VertexType>> entries =
//                            new List<GraphRouteEntry<EdgeType, VertexType>>(route_from_back.Entries);
//                        entries.Add(new_entry);
//                        float weight = route_from_back.Weight + back_route.WeightToPrevious;

//                        // set weight
//                        routes.Add(new GraphRoute<EdgeType, VertexType>(
//                            route_from_back.From, entries, weight));
//                    }
//                }
//            }
//            return routes;
//        }
//    }
//}
