using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;
using Osm.Data.Core.Sparse.Primitives;
using Osm.Data.Core.CH.Primitives;

namespace Osm.Routing.CH.PreProcessing.Ordering
{
    /// <summary>
    /// The edge difference calculator.
    /// </summary>
    public class EdgeDifference : INodeWeightCalculator
    {
        /// <summary>
        /// Holds the graph.
        /// </summary>
        private INodeWitnessCalculator _witness_calculator;

        /// <summary>
        /// Creates a new edge difference calculator.
        /// </summary>
        /// <param name="graph"></param>
        public EdgeDifference(INodeWitnessCalculator witness_calculator)
        {
            _witness_calculator = witness_calculator;
        }

        /// <summary>
        /// Calculates the edge-difference if u would be contracted.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public float Calculate(int level, CHVertex u)
        {
            // simulate the construction of new edges.
            int new_edges = 0;
            int removed = u.BackwardNeighbours.Count + u.ForwardNeighbours.Count;
            foreach (CHVertexNeighbour from in u.BackwardNeighbours)
            { // loop over all incoming neighbours
                foreach (CHVertexNeighbour to in u.ForwardNeighbours)
                { // loop over all outgoing neighbours
                    if (to.Id != from.Id)
                    { // the neighbours point to different vertices.
                        // a new edge is needed.
                        if (!_witness_calculator.Exists(level, from.Id, to.Id, u.Id,
                            from.Weight + to.Weight))
                        { // no witness exists.
                            new_edges++;
                        }
                    }
                }
            }
            return new_edges - removed;
        }

        /// <summary>
        /// Notifies this calculator that the vertex was contracted.
        /// </summary>
        /// <param name="vertex_id"></param>
        public void NotifyContractedNeighbour(long vertex_id)
        {

        }
    }
}
