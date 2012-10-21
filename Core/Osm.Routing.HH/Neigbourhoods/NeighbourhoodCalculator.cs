// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
using System.Collections.Generic;
using Osm.Routing.HH.Primitives;

namespace Osm.Routing.HH.Neigbourhoods
{
    /// <summary>
    /// Class repsonsible for calculating neighbourhoods.
    /// </summary>
    internal class NeighbourhoodCalculator
    {
        /// <summary>
        /// Holds the neighbourhood size.
        /// </summary>
        public int _neighbourhood_size = 10;

        /// <summary>
        /// The current level.
        /// </summary>
        public int _level;

        /// <summary>
        /// Creates a new neighbourhood calculator;
        /// </summary>
        /// <param name="level"></param>
        public NeighbourhoodCalculator(int level)
        {
            _level = level;
        }

        /// <summary>
        /// Calculates the neighbourhood of some vertex.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="vertex"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public Neighbourhood Calculate(IHighwayHierarchy hh, HighwayVertex vertex)
        {
            // keep the neighbourhood structure here.
            HashSet<long> forward = new HashSet<long>();
            forward.Add(vertex.Id);

            // initialize the visitlist.
            SortedSet<HighwayVertexReference> visit_list =
                new SortedSet<HighwayVertexReference>(HighwayVertexReference.Comparer);

            // initialize the current.
            HighwayVertexReference current_reference = new HighwayVertexReference(vertex.Id, null, 0f);
            HighwayVertex current = vertex;

            // loop until the neighbourhood is reached.
            float radius = 0;
            while (forward.Count < _neighbourhood_size)
            {
                // get the current node's neighbours.
                HashSet<HighwayVertexNeighbour> neighbours = 
                    current.GetNeighbours(_level, forward, false);

                // process all the neighbours.
                foreach (HighwayVertexNeighbour neighbour in neighbours)
                { // loop over all neighbours.
                    HighwayVertexReference reference =
                        new HighwayVertexReference(neighbour.VertexId, current_reference, neighbour.Weight);
                    visit_list.Add(reference);
                }

                // select the first from the visit list.
                if (visit_list.Count > 0)
                { 
                    // get the first vertex.
                    current_reference = visit_list.Min;
                    visit_list.Remove(current_reference);
                    forward.Add(current_reference.VertexId);

                    // load the current vertex.
                    current = hh.GetVertex(current_reference.VertexId);

                    // get the radius.
                    radius = current_reference.TotalWeight;
                }
                else
                { // neighbourhood not yet completed but no other neighbours found yet.
                    break;
                }
            }

            // calculate the backwards neighbourhood.
            current_reference = new HighwayVertexReference(vertex.Id, null, 0f);
            current = vertex;
            HashSet<long> backward = new HashSet<long>();
            backward.Add(vertex.Id);
            float current_raduis = 0;
            while (current_raduis < radius)
            { // loop until the radius is bigger.
                // get the current node's neighbours.
                HashSet<HighwayVertexNeighbour> neighbours = 
                    current.GetNeighboursBackward(_level, backward, false);

                // process all the neighbours.
                foreach (HighwayVertexNeighbour neighbour in neighbours)
                { // loop over all neighbours.
                    HighwayVertexReference reference =
                        new HighwayVertexReference(neighbour.VertexId, current_reference, neighbour.Weight);
                    visit_list.Add(reference);
                }

                // select the first from the visit list.
                if (visit_list.Count > 0)
                {
                    // get the first vertex.
                    current_reference = visit_list.Min;
                    visit_list.Remove(current_reference);
                    forward.Add(current_reference.VertexId);

                    // load the current vertex.
                    current = hh.GetVertex(current_reference.VertexId);

                    // get the radius.
                    radius = current_reference.TotalWeight;
                }
                else
                { // neighbourhood not yet completed but no other neighbours found yet.
                    break;
                }
            }
            
            // instantiate the neighbourhood.
            Neighbourhood neighbourhood = new Neighbourhood();
            neighbourhood.BackwardNeighbourhood = backward;
            neighbourhood.ForwardNeighbourhood = forward;
            neighbourhood.Radius = radius;
            neighbourhood.Source = vertex;

            return neighbourhood;
        }

        /// <summary>
        /// Caches the neighbourhoods.
        /// </summary>
        private Dictionary<HighwayVertex, Neighbourhood> _neighbourhoods =
            new Dictionary<HighwayVertex, Neighbourhood>();

        /// <summary>
        /// Calculates and caches the neighbourhood/returns the cached neighbourhood.
        /// </summary>
        /// <param name="hh"></param>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public Neighbourhood CalculateAndCache(IHighwayHierarchy hh, HighwayVertex vertex)
        {
            Neighbourhood neighbourhood = null;
            if (!_neighbourhoods.TryGetValue(vertex, out neighbourhood))
            {
                neighbourhood = this.Calculate(hh, vertex);
                _neighbourhoods.Add(vertex,
                    neighbourhood);
            }
            return neighbourhood;
        }
    }
}
