using System.Collections.Generic;
using Osm.Routing.HH.Primitives;

namespace Osm.Routing.HH.Neigbourhoods
{
    /// <summary>
    /// Represents the neighbourhood of a vertex.
    /// </summary>
    internal class Neighbourhood
    {
        /// <summary>
        /// The source of the neigbourhood.
        /// </summary>
        public HighwayVertex Source { get; set; }

        /// <summary>
        /// Holds all vertices in the forward neighbourhood.
        /// </summary>
        public HashSet<long> ForwardNeighbourhood { get; set; }

        /// <summary>
        /// Holds all vertices in the backward neighbourhood.
        /// </summary>
        public HashSet<long> BackwardNeighbourhood { get; set; }

        /// <summary>
        /// The neigbourhood radius.
        /// </summary>
        public float Radius { get; set; }
    }
}
