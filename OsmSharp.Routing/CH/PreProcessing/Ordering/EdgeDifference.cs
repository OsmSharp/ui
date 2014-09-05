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

using OsmSharp.Routing.Graph.Router;
using System.Collections.Generic;

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
            var edgesForContractions = new List<KeyValuePair<uint, CHEdgeData>>();
            var tos = new List<uint>();
            foreach(var neighbour in neighbours)
            {
                if (!neighbour.EdgeData.ToLower && neighbour.EdgeData.Forward)
                {
                    edgesForContractions.Add(new KeyValuePair<uint, CHEdgeData>(neighbour.Neighbour, neighbour.EdgeData));
                    tos.Add(neighbour.Neighbour);
                    removed++;
                }
            }

            // loop over all neighbours and check for witnesses.
            var witnesses = new bool[edgesForContractions.Count];
            var tosWeights = new List<float>(edgesForContractions.Count);
            foreach (var from in edgesForContractions)
            { // loop over all incoming neighbours.
                // calculate max weight.
                tosWeights.Clear();
                for (int idx = 0; idx < edgesForContractions.Count; idx++)
                {
                    // update maxWeight.
                    var to = edgesForContractions[idx];
                    if (from.Value.Backward && to.Value.Forward)
                    {
                        float weight = (float)from.Value.BackwardWeight + (float)to.Value.ForwardWeight;
                        witnesses[idx] = false;
                        tosWeights.Add(weight);
                    }
                    else
                    {
                        witnesses[idx] = true;
                        tosWeights.Add(0);
                    }
                }

                _witnessCalculator.Exists(_data, from.Key, tos, tosWeights, int.MaxValue, ref witnesses);
                for (int idx = 0; idx < edgesForContractions.Count; idx++)
                { // loop over all outgoing neighbours
                    var to = edgesForContractions[idx];
                    if (to.Key != from.Key &&
                        to.Value.Forward && from.Value.Backward &&
                        !witnesses[idx])
                    { // the neighbours point to different vertices.
                        // a new edge is needed.
                        // no witness exists.
                        newEdges++;
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