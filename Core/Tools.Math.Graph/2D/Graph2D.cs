//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Tools.Math.Graph._2D
//{
//    /// <summary>
//    /// Class representing a graph with nodes and edges located in a two dimension plane.
//    /// </summary>
//    /// <typeparam name="PointType"></typeparam>
//    public abstract class Graph2D<EdgeType, VertexType, PointType> : Graph<EdgeType, VertexType>
//        where PointType : PointF2D
//        where VertexType : Vertex2D<VertexType, PointType>
//        where EdgeType : Edge2D<VertexType, PointType>
//    {

//        /// <summary>
//        /// Creates a new graph with the given interpreter.
//        /// </summary>
//        /// <param name="interpreter"></param>
//        public Graph2D(IGraphInterpreter<EdgeType, VertexType> interpreter)
//            : base(interpreter)
//        {

//        }
//    }
//}
