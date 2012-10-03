using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.Sparse.Primitives;

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
        /// <param name="vertex"></param>
        /// <returns>A estimate of the benefit of contraction, when float.MaxValue the vertex will not be contracted.</returns>
        float Calculate(uint vertex);

        /// <summary>
        /// Notifies this calculator that the vertex was contracted.
        /// </summary>
        /// <param name="vertex"></param>
        void NotifyContracted(uint vertex);
    }
}
