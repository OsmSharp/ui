// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.DynamicGraph;

namespace OsmSharp.Routing.CH.PreProcessing.Ordering
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
        /// <param name="data"></param>
        /// <param name="witness_calculator"></param>
        public EdgeDifference(IDynamicGraph<CHEdgeData> data, INodeWitnessCalculator witness_calculator)
        {
            _data = data;
            _witness_calculator = witness_calculator;
        }

        /// <summary>
        /// Calculates the edge-difference if u would be contracted.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public float Calculate(uint vertex)
        {
            // get the neighbours.
            KeyValuePair<uint, CHEdgeData>[] neighbours = _data.GetArcs(vertex);

            // simulate the construction of new edges.
            int new_edges = 0;
            int removed = neighbours.Length;

            // loop over all neighbours and check for witnesses.
            foreach (KeyValuePair<uint, CHEdgeData> from in neighbours)
            { // loop over all incoming neighbours
                foreach (KeyValuePair<uint, CHEdgeData> to in neighbours)
                { // loop over all outgoing neighbours
                    if (to.Key != from.Key)
                    { // the neighbours point to different vertices.
                        // a new edge is needed.
                        if (!_witness_calculator.Exists(from.Key, to.Key, vertex,
                            (float)from.Value.Weight + (float)to.Value.Weight, int.MaxValue))
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
        /// <param name="vertex"></param>
        public void NotifyContracted(uint vertex)
        {

        }
    }
}
