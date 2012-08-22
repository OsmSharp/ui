//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Tools.Math.Graph.Finite
//{    /// <summary>
//    /// Graph class representing a finite graph, edges and vertices can be enumerated.
//    /// </summary>
//    /// <typeparam name="EdgeType">An edge; this edge can contain multiple vertices.</typeparam>
//    /// <typeparam name="VertexType">A vertex located at one specific point in the graph and part of an edge.</typeparam>
//    public abstract class FiniteGraph<EdgeType, VertexType> : Graph<EdgeType, VertexType>
//        where EdgeType : class
//        where VertexType : class, IEquatable<VertexType>
//    {
//        protected FiniteGraph(FiniteGraphInterpreter<EdgeType, VertexType> interpreter)
//            : base(interpreter)
//        {

//        }

//        #region Edges

//        /// <summary>
//        /// Returns an enumerable of all edges.
//        /// </summary>
//        /// <returns></returns>
//        public abstract IEnumerable<EdgeType> GetEdges();

//        /// <summary>
//        /// Adds all the edges in the graph.
//        /// </summary>
//        /// <param name="edges"></param>
//        public virtual void AddEdges(IEnumerable<EdgeType> edges)
//        {
//            foreach (EdgeType edge in edges)
//            { // add the individual edge.
//                this.AddEdge(edge);
//            }
//        }

//        /// <summary>
//        /// Adds one edge to this finite graph.
//        /// </summary>
//        /// <param name="edge"></param>
//        public abstract void AddEdge(EdgeType edge);

//        /// <summary>
//        /// Removes one edge from this finite graph.
//        /// </summary>
//        /// <param name="edge"></param>
//        public abstract void RemoveEdge(EdgeType edge);

//        /// <summary>
//        /// Returns true if the graph already contains the given edge.
//        /// </summary>
//        /// <param name="edge"></param>
//        /// <returns></returns>
//        public abstract bool ContainsEdge(EdgeType edge);

//        /// <summary>
//        /// Removes all edges from this finite graph.
//        /// </summary>
//        /// <param name="edges"></param>
//        public virtual void RemoveEdges(IEnumerable<EdgeType> edges)
//        {
//            foreach (EdgeType edge in edges)
//            { // remove the individual edge.
//                this.RemoveEdge(edge);
//            }
//        }

//        #endregion

//        #region Edges

//        /// <summary>
//        /// Returns an enumerable of all vertices.
//        /// </summary>
//        /// <returns></returns>
//        public abstract IEnumerable<VertexType> GetVertices();

//        // adding and removing vertices is impossible, can only be done when removing/adding edges.

//        #endregion

//    }
//}
