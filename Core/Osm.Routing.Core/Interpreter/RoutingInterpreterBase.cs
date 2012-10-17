using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;

namespace Osm.Routing.Core.Interpreter
{
    /// <summary>
    /// Interpreters the raw OSM data for routing purposes.
    /// </summary>
    /// <remarks>
    /// All the routing configurations should be behind this class. This is the one-entry point for routing configurations, no matter
    /// what routing technique will be used.
    /// </remarks>
    public abstract class RoutingInterpreterBase
    {
        /// <summary>
        /// Returns true if this node has a weight.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public abstract bool HasWeight(Node node);

        /// <summary>
        /// Returns the weight between two given nodes.
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="from_vertex"></param>
        /// <param name="to_vertex"></param>
        /// <returns></returns>
        public abstract float Weight(Way edge, Node from_vertex, Node to_vertex);

        #region Traversable

        /// <summary>
        /// Returns true if the way can be traversed.
        /// </summary>
        /// <param name="way"></param>
        /// <returns></returns>
        public abstract bool CanBeTraversed(Way way);

        /// <summary>
        /// Returns true if the way can be traversed from the given node to the given node.
        /// </summary>
        /// <param name="way"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        /// <remarks>from and to need to be neighbours</remarks>
        public abstract bool CanBeTraversed(Way way, Node from, Node to);

        /// <summary>
        /// Returns true if the node can be traversed coming from the given node along the given way to the given node along the given way.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="edge_from"></param>
        /// <param name="along"></param>
        /// <param name="edge_to"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public abstract bool CanBeTraversed(Node from, Way edge_from, Node along, Way edge_to, Node to);

        /// <summary>
        /// Returns true if a location on this way can serve as a starting or end point.
        /// </summary>
        /// <param name="way"></param>
        /// <returns></returns>
        public abstract bool CanBeStoppedOn(Way way);

        #endregion

        #region Way Tag Interpretations

        /// <summary>
        /// Returns a way interpretation.
        /// </summary>
        /// <param name="way"></param>
        /// <returns></returns>
        public abstract RoutingWayInterperterBase GetWayInterpretation(Way way);

        #endregion
    }
}
