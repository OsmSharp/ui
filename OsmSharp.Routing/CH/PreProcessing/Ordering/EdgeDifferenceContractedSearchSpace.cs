// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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
using OsmSharp.Routing.Graph.Router;

namespace OsmSharp.Routing.CH.PreProcessing.Ordering
{
    /// <summary>
    /// The edge difference calculator.
    /// </summary>
    public class EdgeDifferenceContractedSearchSpace : INodeWeightCalculator
    {
        /// <summary>
        /// Holds the graph.
        /// </summary>
        private INodeWitnessCalculator _witness_calculator;

        /// <summary>
        /// Holds the data.
        /// </summary>
        private IDynamicGraphRouterDataSource<CHEdgeData> _data;

        /// <summary>
        /// Holds the contracted count.
        /// </summary>
        private Dictionary<uint, short> _contraction_count;

        /// <summary>
        /// Holds the depth.
        /// </summary>
        private Dictionary<long, long> _depth;

        /// <summary>
        /// Creates a new edge difference calculator.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="witness_calculator"></param>
        public EdgeDifferenceContractedSearchSpace(IDynamicGraphRouterDataSource<CHEdgeData> data, INodeWitnessCalculator witness_calculator)
        {
            _data = data;
            _witness_calculator = witness_calculator;
            _contraction_count = new Dictionary<uint, short>();
            _depth = new Dictionary<long, long>();
        }

        /// <summary>
        /// Calculates the edge-difference if u would be contracted.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public float Calculate(uint vertex)
        {
            short contracted = 0;
            _contraction_count.TryGetValue(vertex, out contracted);

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
                        if (!_witness_calculator.Exists(_data, from.Key, to.Key, vertex,
                            (float)from.Value.Weight + (float)to.Value.Weight, 50))
                        { // no witness exists.
                            new_edges++;
                        }
                    }
                }
            }

            // get the depth.                    
            long depth = 0;
            _depth.TryGetValue(vertex, out depth);
            return (3 * (new_edges - removed)) + (2 * contracted);// +depth;
            //return (new_edges - removed) + depth;
        }

        /// <summary>
        /// Notifies this calculator that the vertex was contracted.
        /// </summary>
        /// <param name="vertex"></param>
        public void NotifyContracted(uint vertex)
        {
            // removes the contractions count.
            _contraction_count.Remove(vertex);

            // loop over all neighbours.
            KeyValuePair<uint, CHEdgeData>[] neighbours = _data.GetArcs(vertex);
            foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours)
            {
                short count;
                if (!_contraction_count.TryGetValue(neighbour.Key, out count))
                {
                    _contraction_count[neighbour.Key] = 1;
                }
                else
                {
                    _contraction_count[neighbour.Key] = count++;
                }
            }

            long vertex_depth = 0;
            _depth.TryGetValue(vertex, out vertex_depth);
            _depth.Remove(vertex);
            vertex_depth++;

            // store the depth.
            foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours)
            {
                if (!_contraction_count.ContainsKey(neighbour.Key))
                {
                    long depth = 0;
                    _depth.TryGetValue(neighbour.Key, out depth);
                    if (vertex_depth > depth)
                    {
                        _depth[neighbour.Key] = depth;
                    }
                    else
                    {
                        _depth[neighbour.Key] = vertex_depth;
                    }
                }
            }
        }
    }
}
