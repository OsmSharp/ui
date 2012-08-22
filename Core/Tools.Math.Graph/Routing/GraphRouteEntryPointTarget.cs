using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Graph.Routing
{
    /// <summary>
    /// Represents a route entry at a routing target.
    /// </summary>
    /// <typeparam name="VertexType"></typeparam>
    public class GraphRouteEntryPointTarget<VertexType> : GraphRouteEntryPoint<VertexType>
    {
        /// <summary>
        /// Holds the routing target of this route entry point.
        /// </summary>
        private GraphRouteTarget<VertexType> _target;

        /// <summary>
        /// Creates a new route entry target point.
        /// </summary>
        /// <param name="target"></param>
        public GraphRouteEntryPointTarget(GraphRouteTarget<VertexType> target)
            : base(target.Vertex, true)
        {
            _target = target;
        }

        /// <summary>
        /// Returns the routing target of this route entry.
        /// </summary>
        public GraphRouteTarget<VertexType> Target
        {
            get
            {
                return _target;
            }
        }
    }
}
