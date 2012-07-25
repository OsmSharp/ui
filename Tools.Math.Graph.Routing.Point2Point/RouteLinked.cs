using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.Graph.Routing
{
    /// <summary>
    /// Represents a vertex linked to another.
    /// </summary>
    public class RouteLinked
    {
        /// <summary>
        /// Creates a vertex not linked to any others.
        /// </summary>
        /// <param name="vertex_id"></param>
        public RouteLinked(long vertex_id)
        {
            this.VertexId = vertex_id;
            this.Weight = 0;
            this.From = null;
        }

        /// <summary>
        /// Creates a new linked vertex.
        /// </summary>
        /// <param name="vertex_id"></param>
        /// <param name="weight"></param>
        /// <param name="from"></param>
        public RouteLinked(long vertex_id, float weight, RouteLinked from)
        {
            this.VertexId = vertex_id;
            this.Weight = weight;
            this.From = from;
        }

        /// <summary>
        /// The id of this vertex.
        /// </summary>
        public long VertexId { get; private set; }

        /// <summary>
        /// The weight from the source vertex.
        /// </summary>
        public float Weight { get; private set; }

        /// <summary>
        /// The vertex that came before this one.
        /// </summary>
        public RouteLinked From { get; private set; }
    }
}
