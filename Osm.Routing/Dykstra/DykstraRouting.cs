using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Graph.Routing.Dykstra;
using Osm.Routing.Raw.Graphs;

namespace Osm.Routing.Dykstra
{
    /// <summary>
    /// A dykstra router working on 
    /// </summary>
    internal class DykstraRouting: DykstraRouting<GraphVertex>
    {
        /// <summary>
        /// Creates a dykstra routing graph.
        /// </summary>
        /// <param name="graph"></param>
        public DykstraRouting(Graph graph)
            : base(graph)
        {

        }
    }
}
