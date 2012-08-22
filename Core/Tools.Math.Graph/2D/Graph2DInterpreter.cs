//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Tools.Math.Graph._2D
//{
//    /// <summary>
//    /// Class implementing a default 2D graph interpreter where all edges can be travelled and the weights are the distances.
//    /// </summary>
//    public class Default2DInterpreter<EdgeType, VertexType, PointType> : IGraph2DInterpreter<EdgeType, VertexType, PointType>
//        where PointType : PointF2D
//        where VertexType : Vertex2D<VertexType, PointType>
//        where EdgeType : Edge2D<VertexType,PointType>
//    {
//        /// <summary>
//        /// Search until one is found!
//        /// </summary>
//        public double EdgeSearchRadius
//        {
//            get
//            {
//                return double.MaxValue;
//            }
//        }

//        #region IGraphInterpreter<Edge<PointType>,Vertex<PointType>> Members

//        /// <summary>
//        /// Not used in this interpreter.
//        /// </summary>
//        /// <param name="graph"></param>
//        public void SetGraph(Graph<EdgeType, VertexType> graph)
//        {

//        }

//        /// <summary>
//        /// All can be travelled!
//        /// </summary>
//        /// <param name="edge"></param>
//        /// <returns></returns>
//        public bool CanBeTraversed(EdgeType edge)
//        {
//            return true;
//        }

//        /// <summary>
//        /// All directions can be travelled!
//        /// </summary>
//        /// <param name="edge"></param>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        public bool CanBeTraversed(EdgeType edge, VertexType from, VertexType to)
//        {
//            return true;
//        }

//        /// <summary>
//        /// All directions can be travelled!
//        /// </summary>
//        /// <param name="edge"></param>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        public bool CanBeTraversed(VertexType from, EdgeType edge_from, VertexType via, EdgeType edge_to, VertexType to)
//        {
//            return true;
//        }

//        /// <summary>
//        /// The weight is the distance.
//        /// </summary>
//        /// <param name="edge"></param>
//        /// <param name="from_vertex"></param>
//        /// <param name="to_vertex"></param>
//        /// <returns></returns>
//        public float Weight(EdgeType edge, VertexType from_vertex, VertexType to_vertex)
//        {
//            float weight = (float)from_vertex.Coordinate.Distance(to_vertex.Coordinate);
//            return weight;
//        }

//        /// <summary>
//        /// Returns true.
//        /// </summary>
//        /// <param name="edge"></param>
//        /// <returns></returns>
//        public bool CanBeStoppedOn(EdgeType edge)
//        {
//            return true;
//        }

//        #endregion

//        #region IGraphInterpreter<EdgeType,VertexType> Members


//        public float UnderestimateWeight(VertexType from, VertexType to)
//        {
//            return 0;
//        }

//        #endregion
//    }
//}
