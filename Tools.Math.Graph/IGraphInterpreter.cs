//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Tools.Math.Graph.Helpers;

//namespace Tools.Math.Graph
//{
//    /// <summary>
//    /// Interface providing functionality to interpret edges and vertices based on some routing rules.
//    /// </summary>
//    public interface IGraphInterpreter<EdgeType, VertexType>
//        where EdgeType : class
//        where VertexType : class, IEquatable<VertexType>
//    {
//        /// <summary>
//        /// Sets the graph on the intepreter.
//        /// 
//        /// Can be used in a more advanced interpreter to get more information about a vertex or edge.
//        /// </summary>
//        /// <param name="graph"></param>
//        void SetGraph(Graph<EdgeType, VertexType> graph);

//        /// <summary>
//        /// Returns true if the edge can be linked to.
//        /// </summary>
//        /// <param name="edge"></param>
//        /// <returns></returns>
//        bool CanBeStoppedOn(EdgeType edge);

//        /// <summary>
//        /// Returns true if the edge can be traversed.
//        /// </summary>
//        /// <param name="edge"></param>
//        /// <returns></returns>
//        bool CanBeTraversed(
//            EdgeType edge);

//        /// <summary>
//        /// Returns true if the edge is travesible from the from-vertex to the to-vertex.
//        /// </summary>
//        /// <param name="edge"></param>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        bool CanBeTraversed(EdgeType edge, VertexType from, VertexType to);

//        /// <summary>
//        /// Returns true if the edge is traversible when coming from a given vertex.
//        /// </summary>
//        /// <param name="from"></param>
//        /// <param name="edge_from"></param>
//        /// <param name="via"></param>
//        /// <param name="edge_to"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        bool CanBeTraversed(VertexType from, EdgeType edge_from, VertexType via, EdgeType edge_to, VertexType to);

//        /// <summary>
//        /// Calculates the weight of one vertex to another.
//        /// </summary>
//        /// <param name="edge"></param>
//        /// <param name="from_vertex"></param>
//        /// <param name="to_vertex"></param>
//        /// <returns></returns>
//        float Weight(
//            EdgeType edge,
//            VertexType from_vertex,
//            VertexType to_vertex);

//        /// <summary>
//        /// Returns an underestimate of the weights between two vertices.
//        /// </summary>
//        /// <param name="from"></param>
//        /// <param name="to"></param>
//        /// <returns></returns>
//        float UnderestimateWeight(VertexType from, VertexType to);
//    }
//}
