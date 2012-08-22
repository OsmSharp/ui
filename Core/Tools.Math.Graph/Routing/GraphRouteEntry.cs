using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Graph.Routing
{
    /// <summary>
    /// Represents a route entry; one atomic part of a route.
    /// </summary>
    /// <typeparam name="EdgeType"></typeparam>
    /// <typeparam name="VertexType"></typeparam>
    public class GraphRouteEntry<EdgeType, VertexType>
    {
        /// <summary>
        /// The edge the route travels along in this atomic part.
        /// </summary>
        private EdgeType _edge;

        /// <summary>
        /// The vertex being travelled to.
        /// </summary>
        private GraphRouteEntryPoint<VertexType> _to;

        /// <summary>
        /// The weight of this entry.
        /// </summary>
        private float _weight;

        /// <summary>
        /// Creates a new route entry.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="to"></param>
        public GraphRouteEntry(
            GraphRouteEntryPoint<VertexType> to,
            EdgeType edge,
            float weight)
        {
            _edge = edge;
            _to = to;
            _weight = weight;
        }

        /// <summary>
        /// Returns the edge being travelled along.
        /// </summary>
        public EdgeType Edge
        {
            get
            {
                return _edge;
            }
        }

        /// <summary>
        /// Returns the vertex being travelled to.
        /// </summary>
        public GraphRouteEntryPoint<VertexType> To
        {
            get
            {
                return _to;
            }
        }

        /// <summary>
        /// Returns the weight of this entry.
        /// </summary>
        public float Weight
        {
            get
            {
                return _weight;
            }
        }
    }
}
