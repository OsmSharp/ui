using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Routing.Sparse.Routing.Graph;
using Tools.Math.Graph.Routing.Dykstra;
using Osm.Data.Core.Sparse.Primitives;

namespace Osm.Routing.Sparse.Routing.Dykstra
{
    /// <summary>
    /// A dykstra router working on a sparse graph.
    /// </summary>
    public class DykstraRouting : DykstraRouting<SparseVertex>
    {
        /// <summary>
        /// Creates a dykstra routing graph.
        /// </summary>
        /// <param name="graph"></param>
        public DykstraRouting(SparseGraph graph)
            : base(graph)
        {

        }
    }
}
