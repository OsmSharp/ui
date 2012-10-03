using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.DynamicGraph;

namespace Osm.Routing.CH.PreProcessing
{
    public class CHEdgeData : IDynamicGraphEdgeData
    {
        public bool Forward { get; set; }

        public bool Backward { get; set; }

        public float Weight { get; set; }

        public uint ContractedVertexId { get; set; }
    }
}
