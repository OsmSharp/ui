using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Routing.Graph.DynamicGraph.SimpleWeighed
{
    /// <summary>
    /// A simple weighed edge.
    /// </summary>
    public class SimpleWeighedEdge : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Flag indicating if this is a forward or backward edge relative to the tag descriptions.
        /// </summary>
        public bool IsForward 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The weight of this edge.
        /// </summary>
        public double Weight
        {
            get;
            set;
        }

        /// <summary>
        /// The properties of this edge.
        /// </summary>
        public uint Tags
        {
            get;
            set;
        }
    }
}