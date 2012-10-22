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
using Osm.Routing.HH.Primitives;

namespace Osm.Routing.HH.Neigbourhoods
{
    /// <summary>
    /// Neighbourhood calculator.
    /// </summary>
    public class NeighhourhoodConstructor
    {
        /// <summary>
        /// Holds the maverick factor.
        /// </summary>
        public float _maverick_factor = 2;
        
        /// <summary>
        /// Holds the level being constructed.
        /// </summary>
        private int _level;

        /// <summary>
        /// Holds the progress reporter.
        /// </summary>
        private IHighwayHierarchyProgressReporter _progress_reporter;

        /// <summary>
        /// Holds the neighbourhood calculator.
        /// </summary>
        private NeighbourhoodCalculator _neighbourhood_calculator;

        /// <summary>
        /// Creates the neighbourhood constructor.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="progress_reporter"></param>
        public NeighhourhoodConstructor(int level, IHighwayHierarchyProgressReporter progress_reporter)
        {
            _level = level;
            _neighbourhood_calculator = new NeighbourhoodCalculator(_level);
            _progress_reporter = progress_reporter;
        }

        public void Construct(IHighwayHierarchy hh, HighwayVertex s0)
        {
            // calculate the s0 neighbourhood.
            Neighbourhood s0_neighbourhood =
                _neighbourhood_calculator.CalculateAndCache(hh, s0);

            // initialize the exceptions set.
            HashSet<long> exceptions = new HashSet<long>();

            // get all first-stage neighbours of s0.
            HashSet<HighwayVertexNeighbour> direct_neighbours =
                s0.GetNeighbours(_level, exceptions, false);

            // contruct highways from each neighbour.
            foreach (HighwayVertexNeighbour direct_neighbour in direct_neighbours)
            {
                this.ConstructFromS0(hh, s0_neighbourhood, direct_neighbour.VertexId);
            }
        }

        private void ConstructFromS0(IHighwayHierarchy hh,
            Neighbourhood s0_neighbourhood, long s1)
        {
            // get the s1 vertex.
            HighwayVertex s1_vertex = hh.GetVertex(s1);

            // get the source.
            HighwayVertex s0 = s0_neighbourhood.Source;

            // first calculate the neighbourhood of s1.
            Neighbourhood s1_neighbourhood =
                _neighbourhood_calculator.CalculateAndCache(hh, s1_vertex);

            // initialize the visitlist.
            SortedSet<HighwayVertexReference> visit_list =
                new SortedSet<HighwayVertexReference>(HighwayVertexReference.Comparer);

            // initialize the current.
            HashSet<long> forward = new HashSet<long>();
            forward.Add(s0.Id);
            HighwayVertexReference current = new HighwayVertexReference(s1, null, 0);
            forward.Add(current.VertexId);
            HighwayVertex current_vertex = null;

            // get the current node's neighbours.
            HashSet<HighwayVertexNeighbour> neighbours = 
                s1_vertex.GetNeighbours(_level, forward, false);

            // process all the neighbours.
            foreach (HighwayVertexNeighbour neighbour in neighbours)
            {
                HighwayVertexReference reference = 
                    new HighwayVertexReference(neighbour.VertexId, current, neighbour.Weight);
                visit_list.Add(reference);
            }

            // loop until there are no active nodes left.
            while (visit_list.Count > 0)
            {
                // select the first from the visit list.
                while (visit_list.Count > 0)
                { // keep selecting until an active node was selected.
                    // select the current from the visit list.
                    current = visit_list.Min;
                    visit_list.Remove(current);
                    forward.Add(current.VertexId);

                    // load the current vertex.
                    hh.GetVertex(current.VertexId);

                    // check the neighbourhood of the current vertex.
                    Neighbourhood current_neighbourhood =
                        _neighbourhood_calculator.CalculateAndCache(hh, current_vertex);

                    // check the conditions.
                    if (!s1_neighbourhood.ForwardNeighbourhood.Contains(current_vertex.Id)
                        && !current_neighbourhood.BackwardNeighbourhood.Contains(s0.Id))
                    { // calculate the overlap count.
                        HashSet<long> overlap = new HashSet<long>(s1_neighbourhood.ForwardNeighbourhood); // overlap set.
                        overlap.IntersectWith(current_neighbourhood.BackwardNeighbourhood);

                        // there is at least some overlap; trace along the route.
                        HashSet<long> intersection = new HashSet<long>();
                        // go along the route back from current to s1.
                        HighwayVertexReference reference = current;
                        while (overlap.Count > 0 && reference != null)
                        { // intersect with route to reference.
                            if (overlap.Contains(reference.VertexId))
                            {
                                intersection.Add(reference.VertexId);
                            }

                            // get the next reference.
                            reference = reference.From;
                        }

                        // if the intersection is small then the node will become passive.
                        if (intersection.Count <= 1)
                        { // the intersection is small; the node is passive.
                            // execute phase 2: edge selection.
                            this.SelectEdges(hh, current, s0_neighbourhood, current_neighbourhood);
                            // move on to the next one in the visit list and do not consider the current one anymore.
                            current = null;
                        }
                        else
                        { // the intersection is too big; the node is active.
                            break;
                        }
                    }
                    else
                    { // the nodes are not 'far' enough apart.
                        break;
                    }
                }

                if (current != null)
                {
                    // check for a maverick in the current node.
                    if (current.TotalWeight < s0_neighbourhood.Radius * _maverick_factor)
                    {
                        // get the neighbours.
                        neighbours = current_vertex.GetNeighbours(_level, forward, false);

                        // process all the neighbours.
                        foreach (HighwayVertexNeighbour neighbour in neighbours)
                        {
                            HighwayVertexReference reference =
                                new HighwayVertexReference(neighbour.VertexId, current, neighbour.Weight);
                            visit_list.Add(reference);
                        }
                    }
                }
            }
        }

        private void SelectEdges(IHighwayHierarchy hh, HighwayVertexReference passive,
            Neighbourhood origine, Neighbourhood passive_neighbourhood)
        {
            HighwayVertexReference reference = passive;
            while (reference != null)
            { // intersect with route to reference.
                if (reference.From != null)
                { // there is an edge.
                    // check edge conditions.
                    if (!origine.ForwardNeighbourhood.Contains(reference.VertexId)
                        && !passive_neighbourhood.BackwardNeighbourhood.Contains(reference.From.VertexId))
                    { // the conditions for the edge are met.
                        //HighwayEdge edge = new HighwayEdge();
                        //edge.From = reference.From.Vertex;
                        //edge.To = reference.Vertex;
                        //edge.Weight = reference.Weight;

                        //hh.AddEdge(_level + 1, edge);
                        HighwayVertex from = hh.GetVertex(reference.From.VertexId);
                        hh.PersistVertex(from);
                        HighwayVertex to = hh.GetVertex(reference.VertexId);
                        hh.PersistVertex(to);

                        //// report the new edge.
                        //_progress_reporter.HighwayEdge(edge);
                    }
                }

                // get the next reference.
                reference = reference.From;
            }
        }
    }
}
