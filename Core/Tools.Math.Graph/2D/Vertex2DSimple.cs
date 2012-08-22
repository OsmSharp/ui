//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Tools.Math.Graph._2D
//{
//    /// <summary>
//    /// Represents a vertex that corresponds to an osm node.
//    /// </summary>
//    internal class Vertex2DSimple<VertexType,PointType> : Vertex2D<VertexType, PointType>
//        where PointType : PointF2D
//        where VertexType : Vertex2D<VertexType, PointType>
//    {
//        /// <summary>
//        /// The point this vertex exists at.
//        /// </summary>
//        private PointType _point;

//        /// <summary>
//        /// Creates a new simple vertex.
//        /// </summary>
//        /// <param name="node"></param>
//        public Vertex2DSimple(PointType point)
//        {
//            _point = point;
//        }

//        /// <summary>
//        /// Returns the geo coordinate.
//        /// </summary>
//        public override PointType Coordinate
//        {
//            get
//            {
//                return _point;
//            }
//        }
//    }
//}
