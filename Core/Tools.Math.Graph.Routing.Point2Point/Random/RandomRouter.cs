//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tools.Math.Graph.Helpers;

//namespace Tools.Math.Graph.Routing.Random
//{
//    public class RandomRouter<EdgeType, VertexType> : IPoint2PointRouter<EdgeType, VertexType>
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
//        public RandomRouter(Graph<EdgeType, VertexType> graph)
//        {
//            _graph = graph;
//        }

//        public void RegisterController(IPoint2PointRouterController<EdgeType, VertexType> controller)
//        {

//        }

//        public GraphRoute<EdgeType, VertexType> CalculateRoute(VertexType from, VertexType to)
//        {
//            HashSet<VertexType> visited = new HashSet<VertexType>();
//            GraphRoute<EdgeType, VertexType> route =
//                new GraphRoute<EdgeType, VertexType>(
//                    new GraphRouteEntryPoint<VertexType>(from, true),
//                    new List<GraphRouteEntry<EdgeType, VertexType>>(),
//                    0);
//            VertexType v = from;
//            visited.Add(v);
//            while (route != null && (route.Entries.Count == 0 || route.Entries[route.Entries.Count - 1].To.Vertex != to))
//            {
//                // get the neighbours of the current v.
//                HashSet<VertexAlongEdge<EdgeType, VertexType>> list =
//                    _graph.GetNeighbours(v, visited);

//                // choose one of the neighbours.
//                if (list.Count > 0)
//                { // ok
//                    int idx = Tools.Math.Random.StaticRandomGenerator.Get().Generate(list.Count);

//                    // get the vertex and add to the route.
//                    VertexAlongEdge<EdgeType, VertexType> along =
//                        (new List<VertexAlongEdge<EdgeType, VertexType>>(list))[idx];

//                    // add to the route.
//                    IList<GraphRouteEntry<EdgeType, VertexType>> entries =
//                        new List<GraphRouteEntry<EdgeType, VertexType>>(route.Entries);
//                    entries.Add(new GraphRouteEntry<EdgeType, VertexType>(
//                        new GraphRouteEntryPoint<VertexType>(along.Vertex, true), along.Edge, along.Weight));
//                    float weight = route.Weight + along.Weight;

//                    // set weight
//                    route = new GraphRoute<EdgeType, VertexType>(
//                        route.From, entries, weight);

//                    // set the next node.
//                    v = along.Vertex;
//                    visited.Add(along.Vertex);
//                }
//                else
//                {
//                    if (v != from)
//                    {
//                        // remove one from the route and start again with a previous vertex.
//                        IList<GraphRouteEntry<EdgeType, VertexType>> entries =
//                            new List<GraphRouteEntry<EdgeType, VertexType>>(route.Entries);
//                        GraphRouteEntry<EdgeType, VertexType> removed_entry =
//                            entries[entries.Count - 1];
//                        entries.RemoveAt(entries.Count - 1);
//                        float weight = route.Weight - removed_entry.Weight;
//                        if (entries.Count > 0)
//                        {
//                            route = new GraphRoute<EdgeType, VertexType>(
//                                route.From, entries, weight);

//                            // set the next node.
//                            v = route.Entries[route.Entries.Count - 1].To.Vertex;
//                        }
//                        else
//                        {
//                            // go back to the origine node.
//                            v = from;
//                        }
//                    }
//                    else
//                    {
//                        // there are no routes left!
//                        route = null;
//                    }
//                }
//            }

//            return route;
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
//            throw new NotImplementedException();
//        }
//    }
//}
