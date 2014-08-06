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
    public class EdgeDifference : INodeWeightCalculator
    {
        /// <summary>
        /// Holds the witness calculator.
        /// </summary>
        private INodeWitnessCalculator _witnessCalculator;

        /// <summary>
        /// Holds the data.
        /// </summary>
        private IDynamicGraphRouterDataSource<CHEdgeData> _data;

        /// <summary>
        /// Creates a new edge difference calculator.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="witnessCalculator"></param>
        public EdgeDifference(IDynamicGraphRouterDataSource<CHEdgeData> data, INodeWitnessCalculator witnessCalculator)
        {
            _data = data;
            _witnessCalculator = witnessCalculator;
        }

        /// <summary>
        /// Calculates the edge-difference if u would be contracted.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public float Calculate(uint vertex)
        {
            // get the neighbours.
            var neighbours = _data.GetEdges(vertex);

            // simulate the construction of new edges.
            int newEdges = 0;
            int removed = 0;
            var edgesForContractions = new List<KeyValuePair<uint, CHEdgeData>>(neighbours.Count());
            foreach(var neighbour in neighbours.ToKeyValuePairs())
            {
                if (!neighbour.Value.ToLower && !neighbour.Value.ToHigher)
                {
                    edgesForContractions.Add(neighbour);
                    removed++;
                }
            }

            // loop over all neighbours and check for witnesses.
            foreach (var from in edgesForContractions)
            { // loop over all incoming neighbours
                foreach (var to in edgesForContractions)
                { // loop over all outgoing neighbours
                    if (to.Key != from.Key &&
                        to.Value.Forward && from.Value.Backward)
                    { // the neighbours point to different vertices.
                        // a new edge is needed.
                        if (!_witnessCalculator.Exists(_data, from.Key, to.Key, vertex,
                            (float)from.Value.Weight + (float)to.Value.Weight, 1000))
                        { // no witness exists.
                            newEdges++;
                        }
                    }
                }
            }
            return (2 * newEdges) - removed;
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
