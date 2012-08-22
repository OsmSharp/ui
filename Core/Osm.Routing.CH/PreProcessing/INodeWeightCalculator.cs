using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.Sparse.Primitives;
using Osm.Data.Core.CH.Primitives;

namespace Osm.Routing.CH.PreProcessing
{
    /// <summary>
    /// A weight calculator for the node ordering.
    /// </summary>
    public interface INodeWeightCalculator
    {
        /// <summary>
        /// Calculates the weight of the given vertex u.
        /// </summary>
        /// <param name="u"></param>
        /// <returns>A estimate of the benefit of contraction, when float.MaxValue the vertex will not be contracted.</returns>
        float Calculate(int level, CHVertex u);

        /// <summary>
        /// Notifies this calculator that the vertex was contracted.
        /// </summary>
        /// <param name="vertex_id"></param>
        void NotifyContracted(CHVertex vertex);
    }
}
