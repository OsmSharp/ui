using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Graph
{
    /// <summary>
    /// Basic interface representing a directed graph.
    /// </summary>
    /// <typeparam name="VertexType"></typeparam>
    public interface IGraphDirected<VertexType> : IGraph<VertexType>
        where VertexType : class, IEquatable<VertexType>
    {
        /// <summary>
        /// Returns all the neighbours for the given vertex.
        /// </summary>
        /// <param name="vertex_id">The vertex to return the neighbours for.</param>
        /// <returns>The vertices with their associated weights.</returns>
        Dictionary<long, float> GetNeighboursReversed(long vertex_id, HashSet<long> exceptions);

        /// <summary>
        /// Returns all the neighbours for the given vertex as if this graph was undirected.
        /// </summary>
        /// <param name="vertex_id">The vertex to return the neighbours for.</param>
        /// <returns>The vertices with their associated weights; counter directed edges have negative weights returned.</returns>
        Dictionary<long, float> GetNeighboursUndirected(long vertex_id, HashSet<long> exceptions);
    }
}
