using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Graph.Routing
{
    /// <summary>
    /// Represents a routing target.
    /// </summary>
    /// <typeparam name="VertexType"></typeparam>
    public abstract class GraphRouteTarget<VertexType>
    {
        /// <summary>
        /// Holds the vertex the target exists at.
        /// </summary>
        private VertexType _vertex;

        /// <summary>
        /// Creates a new route target at the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        protected GraphRouteTarget(VertexType vertex)
        {
            _vertex = vertex;
        }

        /// <summary>
        /// Returns the vertex this target exists at.
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
