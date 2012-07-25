using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.HH.Primitives
{
    /// <summary>
    /// An highway edge.
    /// </summary>
    public class HighwayEdge
    {
        /// <summary>
        /// The distance of this edge.
        /// </summary>
        public float Weight { get; set; }

        /// <summary>
        /// The id of the vertex at the other end of this vertex.
        /// </summary>
        public int VertexId { get; set; }

        /// <summary>
        /// Holds the forward flag.
        /// </summary>
        public bool Forward { get; set; }

        /// <summary>
        /// Holds the backward flag.
        /// </summary>
        public bool Backward { get; set; }        

        /// <summary>
        /// Holds the highest level.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Holds the highest core level.
        /// </summary>
        public int LevelCore { get; set; }

        /// <summary>
        /// Holds the highest contracted level.
        /// </summary>
        public int LevelContracted { get; set; }

        /// <summary>
        /// The tags of this edge.
        /// </summary>
        public string Tags { get; set; }
    }
}
