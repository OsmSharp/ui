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

namespace OsmSharp.Routing.CH.PreProcessing.Ordering
{
    /// <summary>
    /// The edge difference calculator.
    /// </summary>
    public class EdgeDifferenceContracted : INodeWeightCalculator
    {
        /// <summary>
        /// Holds the graph.
        /// </summary>
        private INodeWitnessCalculator _witness_calculator;

        /// <summary>
        /// Holds the contracted count.
        /// </summary>
        private Dictionary<long, long> _contraction_count;

        /// <summary>
        /// Creates a new edge difference calculator.
        /// </summary>
        /// <param name="witness_calculator"></param>
        public EdgeDifferenceContracted(INodeWitnessCalculator witness_calculator)
        {
            _witness_calculator = witness_calculator;
            _contraction_count = new Dictionary<long, long>();
        }

        /// <summary>
        /// Calculates the edge-difference if u would be contracted.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public float Calculate(uint u)
        {
            //long contracted = 0;
            //_contraction_count.TryGetValue(u.Id, out contracted);

            //// simulate the construction of new edges.
            //int new_edges = 0;
            //int removed = u.BackwardNeighbours.Count + u.ForwardNeighbours.Count;
            //foreach (CHVertexNeighbour from in u.BackwardNeighbours)
            //{ // loop over all incoming neighbours
            //    foreach (CHVertexNeighbour to in u.ForwardNeighbours)
            //    { // loop over all outgoing neighbours
            //        if (to.Id != from.Id)
            //        { // the neighbours point to different vertices.
            //            // a new edge is needed.
            //            if (!_witness_calculator.Exists(level, from.Id, to.Id, u.Id,
            //                from.Weight + to.Weight))
            //            { // no witness exists.
            //                new_edges++;
            //            }
            //        }
            //    }
            //}
            //return (new_edges - removed) + (contracted * 3);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Notifies this calculator that the vertex was contracted.
        /// </summary>
        /// <param name="vertex"></param>
        public void NotifyContracted(uint vertex)
        {
            //_contraction_count.Remove(vertex.Id);

            //foreach (CHVertexNeighbour neighbour in vertex.ForwardNeighbours)
            //{
            //    long count;
            //    if (!_contraction_count.TryGetValue(neighbour.Id, out count))
            //    {
            //        _contraction_count[neighbour.Id] = 1;
            //    }
            //    else
            //    {
            //        _contraction_count[neighbour.Id] = count++;
            //    }
            //}

            //foreach (CHVertexNeighbour neighbour in vertex.BackwardNeighbours)
            //{
            //    long count;
            //    if (!_contraction_count.TryGetValue(neighbour.Id, out count))
            //    {
            //        _contraction_count[neighbour.Id] = 1;
            //    }
            //    else
            //    {
            //        _contraction_count[neighbour.Id] = count++;
            //    }
            //}
            throw new NotImplementedException();
        }
    }
}
