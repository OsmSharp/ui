//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tools.Math.Graph.Routing.Dykstra;

//namespace Tools.Math.Graph.Routing.Bidirectional
//{
//    internal class BidirectionAStarVisitList<EdgeType, VertexType>
//    {
//        private HashSet<VertexType> _vertices;

//        private Dictionary<VertexType, VertexReference<EdgeType, VertexType>> _references;

//        public BidirectionAStarVisitList()
//        {
//            _vertices = new HashSet<VertexType>();
//            _references = new Dictionary<VertexType,VertexReference<EdgeType,VertexType>>();
//        }

//        public HashSet<VertexType> Vertices
//        {
//            get
//            {
//                return _vertices;
//            }
//        }

//        public void Add(VertexReference<EdgeType, VertexType> vertex)
//        {
//            _vertices.Add(vertex.Vertex);
//            _references[vertex.Vertex] = vertex;
//        }

//        public VertexReference<EdgeType, VertexType> GetForVertex(VertexType vertex)
//        {
//            VertexReference<EdgeType, VertexType> vertex_reference = null;
//            _references.TryGetValue(vertex, out vertex_reference);
//            return vertex_reference;
//        }
//    }
//}
