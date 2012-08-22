using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Graph
{
    /// <summary>
    /// Basic interface representing a graph.
    /// </summary>
    /// <typeparam name="VertexType">A vertex located at one specific point in the graph.</typeparam>
    public interface IGraph<VertexType>
        where VertexType : class, IEquatable<VertexType>
    {
        /// <summary>
        /// Returns all the neighbours for the given vertex.
        /// </summary>
        /// <param name="vertex_id"></param>
        /// <returns></returns>
        Dictionary<long, float> GetNeighbours(long vertex_id, HashSet<long> exceptions);

        /// <summary>
        /// Returns the vertex with the given id.
        /// </summary>
        /// <param name="vertex_id"></param>
        /// <returns></returns>
        VertexType GetVertex(long vertex_id);
    }
}
