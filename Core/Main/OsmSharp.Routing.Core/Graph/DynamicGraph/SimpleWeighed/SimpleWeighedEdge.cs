using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Routing.Core.Graph.DynamicGraph.SimpleWeighed
{
    public class SimpleWeighedEdge : IDynamicGraphEdgeData
    {
        public bool Forward
        {
            get;
            set;
        }

        public bool Backward
        {
            get;
            set;
        }

        public double Weight
        {
            get;
            set;
        }

        public uint Tags
        {
            get;
            set;
        }
    }
}
