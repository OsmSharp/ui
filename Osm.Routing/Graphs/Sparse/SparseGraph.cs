//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Osm.Routing.Raw.Graphs.Sparse
//{
//    internal class SparseGraph : Tools.Math.Graph.Graph<SparseGraphEdge, GraphVertex>
//    {
//        private Dictionary<GraphVertex, IList<SparseGraphEdge>> _edges_per_vertex;

//        private Graph _graph;

//        private HashSet<GraphVertex> _resolved_vertices;

//        public SparseGraph(Graph graph)
//            :base(new SparseGraphInterpreter())
//        {
//            _graph = graph;
//            _resolved_vertices = new HashSet<GraphVertex>();
//            _edges_per_vertex = new Dictionary<GraphVertex, IList<SparseGraphEdge>>();
//        }

//        public void NotifyResolved(List<GraphVertex> points)
//        {
//            foreach (GraphVertex point in points)
//            {
//                _resolved_vertices.Add(point);
//            }
//        }

//        public void NotifyResolved(List<ResolvedPoint> points)
//        {
//            foreach (ResolvedPoint point in points)
//            {
//                _resolved_vertices.Add(point.Data);
//            }
//        }

//        public void NotifyResolved(ResolvedPoint point)
//        {
//            _resolved_vertices.Add(point.Data);
//        }

//        public override IList<SparseGraphEdge> GetEdgesForVertex(GraphVertex vertex)
//        {
//            IList<SparseGraphEdge> edges = null;
//            if (!_edges_per_vertex.TryGetValue(vertex, out edges))
//            {
//                // create the exceptions list.
//                HashSet<GraphVertex> exceptions = new HashSet<GraphVertex>();
//                exceptions.Add(vertex);

//                HashSet<Tools.Math.Graph.Helpers.VertexAlongEdge<Osm.Core.Way, GraphVertex>> neighbours =
//                    _graph.GetNeighbours(vertex, exceptions);
//                edges = new List<SparseGraphEdge>(neighbours.Count);

//                foreach (Tools.Math.Graph.Helpers.VertexAlongEdge<Osm.Core.Way, GraphVertex> vertex_along in
//                    neighbours)
//                {
//                    // create the exceptions list.
//                    exceptions = new HashSet<GraphVertex>();
//                    exceptions.Add(vertex);

//                    // create the new edge.
//                    SparseGraphEdge edge = new SparseGraphEdge();
//                    edge.Vertex1 = vertex;
//                    edge.Weight = vertex_along.Weight;

//                    // get the first vertex.
//                    GraphVertex neighbour = vertex_along.Vertex;
//                    exceptions.Add(neighbour);

//                    // find it's neighbours.
//                    HashSet<Tools.Math.Graph.Helpers.VertexAlongEdge<Osm.Core.Way, GraphVertex>> vertex_along_neighbours =
//                        _graph.GetNeighbours(neighbour, exceptions);
//                    while (vertex_along_neighbours.Count == 1
//                        && !_resolved_vertices.Contains(neighbour))
//                    {
//                        Tools.Math.Graph.Helpers.VertexAlongEdge<Osm.Core.Way, GraphVertex> along = 
//                            vertex_along_neighbours.ToList<Tools.Math.Graph.Helpers.VertexAlongEdge<Osm.Core.Way, GraphVertex>>()[0];

//                        edge.Weight = edge.Weight +
//                            along.Weight;
//                        neighbour = along.Vertex;
//                        exceptions.Add(neighbour);

//                        vertex_along_neighbours =
//                            _graph.GetNeighbours(neighbour, exceptions);
//                    }
//                    edge.Vertex2 = neighbour;

//                    edges.Add(edge);
//                }

//                _edges_per_vertex.Add(vertex, edges);
//            }
//            return edges;
//        }

//        public override IList<GraphVertex> GetNeighbourVerticesOnEdge(SparseGraphEdge edge, GraphVertex vertex)
//        {
//            List<GraphVertex> vertices = new List<GraphVertex>();
//            if (vertex == edge.Vertex1)
//            {
//                vertices.Add(edge.Vertex2);
//            }
//            return vertices;
//        }
//    }
//}