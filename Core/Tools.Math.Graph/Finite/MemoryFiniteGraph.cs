//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Tools.Math.Graph.Finite
//{
//    /// <summary>
//    /// Finite graph kept in-memory.
//    /// </summary>
//    /// <typeparam name="VertexType"></typeparam>
//    public class MemoryFiniteGraph<VertexType> : FiniteGraph<IMemoryEdge<VertexType>, VertexType>
//        where VertexType : class, IEquatable<VertexType>
//    {
//        private HashSet<IMemoryEdge<VertexType>> _edges;

//        private Dictionary<VertexType, HashSet<IMemoryEdge<VertexType>>> _edge_per_vertex;

//        public MemoryFiniteGraph()
//            : base(new MemoryFiniteGraphInterpreter<VertexType>())
//        {
//            _edges = new HashSet<IMemoryEdge<VertexType>>();
//            _edge_per_vertex = new Dictionary<VertexType, HashSet<IMemoryEdge<VertexType>>>();
//        }

//        public override IEnumerable<IMemoryEdge<VertexType>> GetEdges()
//        {
//            return _edges;
//        }

//        public override bool ContainsEdge(IMemoryEdge<VertexType> edge)
//        {
//            return _edges.Contains(edge);
//        }

//        public override void AddEdge(IMemoryEdge<VertexType> edge)
//        {
//            _edges.Add(edge);

//            HashSet<IMemoryEdge<VertexType>> edges = null;
//            if (!_edge_per_vertex.TryGetValue(edge.From, out edges))
//            {
//                edges = new HashSet<IMemoryEdge<VertexType>>();
//                _edge_per_vertex.Add(edge.From, edges);
//            }
//            edges.Add(edge);
//            if (!_edge_per_vertex.TryGetValue(edge.To, out edges))
//            {
//                edges = new HashSet<IMemoryEdge<VertexType>>();
//                _edge_per_vertex.Add(edge.To, edges);
//            }
//            edges.Add(edge);
//        }

//        public override void RemoveEdge(IMemoryEdge<VertexType> edge)
//        {
//            if (_edges.Remove(edge))
//            {
//                HashSet<IMemoryEdge<VertexType>> edges = null;
//                if (_edge_per_vertex.TryGetValue(edge.From, out edges))
//                {
//                    edges.Remove(edge);

//                    if (edges.Count == 0)
//                    {
//                        _edge_per_vertex.Remove(edge.From);
//                    }
//                }
//                if (_edge_per_vertex.TryGetValue(edge.To, out edges))
//                {
//                    edges.Remove(edge);

//                    if (edges.Count == 0)
//                    {
//                        _edge_per_vertex.Remove(edge.To);
//                    }
//                }
//            }
//        }

//        public override IEnumerable<VertexType> GetVertices()
//        {
//            return _edge_per_vertex.Keys;
//        }

//        public override IList<IMemoryEdge<VertexType>> GetEdgesForVertex(VertexType vertex)
//        {
//            HashSet<IMemoryEdge<VertexType>> edges = null;
//            _edge_per_vertex.TryGetValue(vertex, out edges);
//            return new List<IMemoryEdge<VertexType>>(edges);
//        }

//        public override IList<VertexType> GetNeighbourVerticesOnEdge(
//            IMemoryEdge<VertexType> edge, VertexType vertex)
//        {
//            IList<VertexType> vertices = new List<VertexType>();
//            if (edge.From.Equals(vertex))
//            {
//                vertices.Add(edge.To);
//            }
//            if (edge.To.Equals(vertex))
//            {
//                vertices.Add(edge.From);
//            }
//            return vertices;
//        }
//    }
//}
