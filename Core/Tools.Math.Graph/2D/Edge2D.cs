//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Tools.Math.Graph._2D
//{
//    /// <summary>
//    /// Class representing an edge in a 2D graph.
//    /// 
//    /// An edge consists of two vertices and represents their connection.
//    /// </summary>
//    /// <typeparam name="PointType"></typeparam>
//    public class Edge2D<VertexType,PointType> 
//        where PointType : PointF2D
//        where VertexType : Vertex2D<VertexType, PointType>
//    {
//        /// <summary>
//        /// The first vertex in this edge.
//        /// </summary>
//        private VertexType _vertex1;

//        /// <summary>
//        /// The second vertex in this edge.
//        /// </summary>
//        private VertexType _vertex2;

//        /// <summary>
//        /// Creates a new edge.
//        /// </summary>
//        /// <param name="vertex1"></param>
//        /// <param name="vertex2"></param>
//        public Edge2D(
//            VertexType vertex1,
//            VertexType vertex2)
//        {
//            _vertex1 = vertex1;
//            _vertex2 = vertex2;
//        }

//        /// <summary>
//        /// Returns the first vertex in this edge.
//        /// </summary>
//        public VertexType Vertex1
//        {
//            get
//            {
//                return _vertex1;
//            }
//        }

//        /// <summary>
//        /// Returns the second vertex in this edge.
//        /// </summary>
//        public VertexType Vertex2
//        {
//            get
//            {
//                return _vertex2;
//            }
//        }
//    }
//}
