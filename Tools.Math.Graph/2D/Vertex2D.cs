//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Tools.Math.Graph._2D
//{
//    /// <summary>
//    /// Represents a vertex in the osm graph.
//    /// </summary>
//    public abstract class Vertex2D<VertexType,PointType> : IEquatable<VertexType>
//        where PointType : PointF2D
//        where VertexType : Vertex2D<VertexType, PointType>
//    {
//        /// <summary>
//        /// Returns the coordinates this vertex exists at.
//        /// </summary>
//        public abstract PointType Coordinate
//        {
//            get;
//        }

//        #region IEquatable<Vertex> Members

//        /// <summary>
//        /// Returns true if the given vertex represents the same vertex as this one.
//        /// </summary>
//        /// <param name="other"></param>
//        /// <returns></returns>
//        public bool Equals(VertexType other)
//        {
//            if (other != null)
//            {
//                return this.Coordinate == other.Coordinate;
//            }
//            return false;
//        }

//        #endregion
//    }
//}
