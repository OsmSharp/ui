using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Graph.Routing
{
    /// <summary>
    /// Represents a point in a route.
    /// </summary>
    /// <typeparam name="VertexType"></typeparam>
    public class GraphRouteEntryPoint<VertexType>
    {
        /// <summary>
        /// Holds the vertex this route point exists at.
        /// </summary>
        private VertexType _vertex;

        /// <summary>
        /// Creates a new route entry point at the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        public GraphRouteEntryPoint(VertexType vertex, bool poi)
        {
            _vertex = vertex;
        }

        /// <summary>
        /// Returns the vertext this route point exists at.
        /// </summary>
        public VertexType Vertex
        {
            get
            {
                return _vertex;
            }
        }
    }
}
