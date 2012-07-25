//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Tools.Math.Graph._2D
//{
//    /// <summary>
//    /// Represents a vertex that exists on a way in between two nodes. 
//    /// 
//    /// Cannot implement the coordinate property because of the lack of a factory or contructor for the PointType.
//    /// </summary>
//    public abstract class Vertex2DCompound<VertexType,PointType> : Vertex2D<VertexType, PointType>
//        where PointType : PointF2D
//        where VertexType : Vertex2D<VertexType, PointType>
//    {
//        /// <summary>
//        /// The edge this vertex exists on.
//        /// </summary>
//        private Edge2D<VertexType, PointType> _edge;

//        /// <summary>
//        /// The percentage that this vertex lies on the edge from vertex1.
//        /// 
//        /// Value 0-1.
//        /// </summary>
//        private double _between;

//        /// <summary>
//        /// Creates a new compounded vertex.
//        /// </summary>
//        /// <param name="edge"></param>
//        /// <param name="idx"></param>
//        /// <param name="between"></param>
//        public Vertex2DCompound(
//            Edge2D<VertexType, PointType> edge,
//            double between)
//        {
//            _edge = edge;
//        }

//        /// <summary>
//        /// Calculates the values of the coordinate of this compounded vertex.
//        /// </summary>
//        protected double[] CoordinateValues
//        {
//            get
//            {
//                double[] values = new double[2];
//                values[0] = _edge.Vertex1.Coordinate[0] * (1.0f - _between) + _edge.Vertex2.Coordinate[0] * _between;
//                values[1] = _edge.Vertex1.Coordinate[1] * (1.0f - _between) + _edge.Vertex2.Coordinate[1] * _between;
//                return values;
//            }
//        }
//    }
//}
