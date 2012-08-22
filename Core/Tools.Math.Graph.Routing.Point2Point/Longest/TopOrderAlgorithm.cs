//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tools.Math.Graph.Helpers;

//namespace Tools.Math.Graph.Routing.Longest
//{
//    /// <summary>
//    /// Sorts the vertices in a graph topologically.
//    /// 
//    /// http://en.wikipedia.org/wiki/Topological_sort#Algorithms
//    /// </summary>
//    /// <typeparam name="EdgeType"></typeparam>
//    /// <typeparam name="VertexType"></typeparam>
//    public class TopOrderAlgorithm<EdgeType, VertexType>
//        where EdgeType : class
//        where VertexType : class, IEquatable<VertexType>
//    {  
//        public static List<VertexType> Calculate(
//            Graph<EdgeType, VertexType> graph,
//            List<VertexType> vertices_without_incoming_edge)
//        {
//            HashSet<VertexType> visited = new HashSet<VertexType>();
//            List<VertexType> s = vertices_without_incoming_edge;
//            List<VertexType> l = new List<VertexType>();

//            // keep going until s is empty.
//            foreach (VertexType v in s)
//            {
//                visit(l, graph, v, visited);
//            }

//            // reverse the list.
//            l.Reverse();
//            return l;
//        }

//        private static void visit(
//            List<VertexType> l,
//            Graph<EdgeType, VertexType> graph, 
//            VertexType v, HashSet<VertexType> visited)
//        {
//            if (!visited.Contains(v))
//            {
//                visited.Add(v);
//                HashSet<VertexAlongEdge<EdgeType, VertexType>> neighbours =
//                    graph.GetNeighbours(v, null);
//                foreach (VertexAlongEdge<EdgeType, VertexType> along in neighbours)
//                {
//                    visit(l, graph, along.Vertex, visited);
//                }
//                l.Add(v);
//            }
//        }
//    }
//}
