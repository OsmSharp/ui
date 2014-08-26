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
using OsmSharp.Routing.CH.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Collections.PriorityQueues;
using OsmSharp.Collections;

namespace OsmSharp.Routing.CH.PreProcessing.Witnesses
{
    /// <summary>
    /// A simple dykstra witness calculator.
    /// </summary>
    public class DykstraWitnessCalculator : INodeWitnessCalculator
    {
        /// <summary>
        /// Holds the current hop limit.
        /// </summary>
        private int _hopLimit;

        /// <summary>
        /// Creates a new witness calculator.
        /// </summary>
        public DykstraWitnessCalculator()
        {
            _hopLimit = 20;
        }

        /// <summary>
        /// Creates a new witness calculator.
        /// </summary>
        public DykstraWitnessCalculator(int hopLimit)
        {
            _hopLimit = hopLimit;
        }

        /// <summary>
        /// Returns true if the given vertex has a witness calculator.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="via"></param>
        /// <param name="weight"></param>
        /// <param name="max_settles"></param>
        /// <returns></returns>
        public bool Exists(IBasicRouterDataSource<CHEdgeData> graph, uint from, uint to, uint via, float weight, int max_settles)
        {
            if (this.CalculateWeight(graph, from, to, via, weight, max_settles) <= weight)
            { // do verification.
                return true;
            }
            return false;
        }

        /// <summary>
        /// Implements a very simple dykstra version.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="via"></param>
        /// <param name="max_weight"></param>
        /// <param name="max_settles"></param>
        /// <returns></returns>
        private float CalculateWeight(IBasicRouterDataSource<CHEdgeData> graph, uint from, uint to, uint via, float max_weight, int max_settles)
        {
            int max_hops = _hopLimit;
            float weight = float.MaxValue;

            // creates the settled list.
            HashSet<uint> settled = new HashSet<uint>();
            settled.Add(via);

            // creates the priorty queue.
            BinairyHeap<SettledVertex> heap = new BinairyHeap<SettledVertex>();
            heap.Push(new SettledVertex(from, 0, 0), 0);

            // keep looping until the queue is empty or the target is found!
            while (heap.Count > 0)
            {
                // pop the first customer.
                SettledVertex current = heap.Pop();
                if (!settled.Contains(current.VertexId))
                { // the current vertex has net been settled.
                    settled.Add(current.VertexId); // settled the vertex.

                    // test stop conditions.
                    if (current.VertexId == to)
                    { // target is found!
                        return current.Weight;
                    }

                    // test the hop count.
                    if (current.Hops < max_hops)
                    { // the neighbours will only increase hops!
                        if (settled.Count >= max_settles)
                        { // do not continue searching.
                            return float.MaxValue;
                        }

                        // get the neighbours.
                        var neighbours = graph.GetEdges(current.VertexId);
                        foreach(var neighbourEdge in neighbours)
                        {
                            if (neighbourEdge.EdgeData.Forward && 
                                !neighbourEdge.EdgeData.ToHigher && 
                                !neighbourEdge.EdgeData.ToLower && 
                                (neighbourEdge.Neighbour == to || !settled.Contains(neighbourEdge.Neighbour)))
                            {
                                var neighbour = new SettledVertex(neighbourEdge.Neighbour,
                                    neighbourEdge.EdgeData.Weight + current.Weight, current.Hops + 1);
                                if (neighbour.Weight < max_weight)
                                {
                                    if (neighbourEdge.Neighbour == to)
                                    {
                                        return neighbour.Weight;
                                    }
                                    heap.Push(neighbour, neighbour.Weight);
                                }
                            }
                        }
                    }
                }
            }

            return weight;
        }

        /// <summary>
        /// Represents a settled vertex.
        /// </summary>
        private class SettledVertex
        {
            /// <summary>
            /// Creates a new settled vertex.
            /// </summary>
            /// <param name="vertex"></param>
            /// <param name="weight"></param>
            /// <param name="hops"></param>
            public SettledVertex(uint vertex, float weight, uint hops)
            {
                this.VertexId = vertex;
                this.Weight = weight;
                this.Hops = hops;
            }

            /// <summary>
            /// The vertex that was settled.
            /// </summary>
            public uint VertexId { get; set; }

            /// <summary>
            /// The weight this vertex was settled at.
            /// </summary>
            public float Weight { get; set; }

            /// <summary>
            /// The hop-count of this vertex.
            /// </summary>
            public uint Hops { get; set; }
        }
    }
}
