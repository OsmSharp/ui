using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;
using Osm.Data.Core.Sparse.Primitives;
using Osm.Data.Core.DynamicGraph;

namespace Osm.Routing.CH.PreProcessing.Ordering
{
    /// <summary>
    /// The edge difference calculator.
    /// </summary>
    public class EdgeDifference : INodeWeightCalculator
    {
        /// <summary>
        /// Holds the witness calculator.
        /// </summary>
        private INodeWitnessCalculator _witness_calculator;

        /// <summary>
        /// Holds the data.
        /// </summary>
        private IDynamicGraph<CHEdgeData> _data;

        /// <summary>
        /// Creates a new edge difference calculator.
        /// </summary>
        /// <param name="graph"></param>
        public EdgeDifference(IDynamicGraph<CHEdgeData> data, INodeWitnessCalculator witness_calculator)
        {
            _data = data;
            _witness_calculator = witness_calculator;
        }

        /// <summary>
        /// Calculates the edge-difference if u would be contracted.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public float Calculate(uint vertex)
        {
            // simulate the construction of new edges.
            int new_edges = 0;
            int removed = 0;

            // get the neighbours.
            KeyValuePair<uint, CHEdgeData>[] neighbours = _data.GetArcs(vertex);

            foreach (KeyValuePair<uint, CHEdgeData> from in neighbours)
            { // loop over all incoming neighbours
                if(!from.Value.Backward) {continue;}

                foreach (KeyValuePair<uint, CHEdgeData> to in neighbours)
                { // loop over all outgoing neighbours
                    if(!to.Value.Forward) {continue;}

                    if (to.Key != from.Key)
                    { // the neighbours point to different vertices.
                        // a new edge is needed.
                        if (!_witness_calculator.Exists(from.Key, to.Key, vertex,
                            from.Value.Weight + to.Value.Weight))
                        { // no witness exists.
                            new_edges++;
                        }
                    }
                }

                // count the edges.
                if (from.Value.Forward)
                {
                    removed++;
                }
                if (from.Value.Backward)
                {
                    removed++;
                }
            }
            return new_edges - removed;
        }

        /// <summary>
        /// Notifies this calculator that the vertex was contracted.
        /// </summary>
        /// <param name="vertex_id"></param>
        public void NotifyContracted(uint vertex)
        {

        }
    }
}
