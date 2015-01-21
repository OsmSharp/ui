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
            _hopLimit = 1;
        }

        /// <summary>
        /// Creates a new witness calculator.
        /// </summary>
        public DykstraWitnessCalculator(int hopLimit)
        {
            _hopLimit = hopLimit;
        }

        /// <summary>
        /// Holds a reusable heap.
        /// </summary>
        private BinairyHeap<SettledVertex> _reusableHeap = new BinairyHeap<SettledVertex>();

        /// <summary>
        /// Returns true if the given vertex has a witness calculator.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxWeight"></param>
        /// <param name="maxSettles"></param>
        /// <returns></returns>
        public bool Exists(IBasicRouterDataSource<CHEdgeData> graph, uint from, uint to, float maxWeight, int maxSettles)
        {
            var tos = new List<uint>(1);
            tos.Add(to);
            var tosWeights = new List<float>(1);
            tosWeights.Add(maxWeight);
            var exists = new bool[1];
            this.Exists(graph, true, from, tos, tosWeights, maxSettles, ref exists);
            return exists[0];
        }

        /// <summary>
        /// Calculates witnesses from on source to multiple targets at once.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="searchForward"></param>
        /// <param name="from"></param>
        /// <param name="tos"></param>
        /// <param name="tosWeights"></param>
        /// <param name="maxSettles"></param>
        /// <param name="exists"></param>
        public void Exists(IBasicRouterDataSource<CHEdgeData> graph, bool searchForward, uint from, List<uint> tos, List<float> tosWeights, int maxSettles, ref bool[] exists)
        {
            int maxHops = _hopLimit;

            if (maxHops == 1)
            {
                this.ExistsOneHop(graph, searchForward, from, tos, tosWeights, maxSettles, ref exists);
                return;
            }

            // creates the settled list.
            var settled = new HashSet<uint>();
            var toSet = new HashSet<uint>();
            float maxWeight = 0;
            for (int idx = 0; idx < tosWeights.Count; idx++)
            {
                if(!exists[idx])
                {
                    toSet.Add(tos[idx]);
                    if(maxWeight < tosWeights[idx])
                    {
                        maxWeight = tosWeights[idx];
                    }
                }
            }

            // creates the priorty queue.
            var heap = _reusableHeap;
            heap.Clear();
            heap.Push(new SettledVertex(from, 0, 0), 0);

            // keep looping until the queue is empty or the target is found!
            while (heap.Count > 0)
            {
                // pop the first customer.
                var current = heap.Pop();
                if (!settled.Contains(current.VertexId))
                { // the current vertex has net been settled.
                    settled.Add(current.VertexId); // settled the vertex.

                    // check if this is a to.
                    if(toSet.Contains(current.VertexId))
                    {
                        int index = tos.IndexOf(current.VertexId);
                        exists[index] = current.Weight <= tosWeights[index];
                        toSet.Remove(current.VertexId);

                        if(toSet.Count == 0)
                        {
                            break;
                        }
                    }

                    if (settled.Count >= maxSettles)
                    { // do not continue searching.
                        break;
                    }

                    // get the neighbours.
                    var neighbours = graph.GetEdges(current.VertexId);
                    while (neighbours.MoveNext())
                    { // move next.
                        if (!settled.Contains(neighbours.Neighbour))
                        { // neighbour not yet settled, good!
                            var edgeData = neighbours.EdgeData;
                            if ((searchForward && edgeData.CanMoveForward) ||
                                (!searchForward && edgeData.CanMoveBackward))
                            { // direction is ok.
                                var neighbour = new SettledVertex(neighbours.Neighbour,
                                    edgeData.Weight + current.Weight, current.Hops + 1);
                                if (neighbour.Weight <= maxWeight && neighbour.Hops < maxHops)
                                { // push to heap.
                                    heap.Push(neighbour, neighbour.Weight);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates witnesses from one source to multiple targets at once but using only one hop.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="from"></param>
        /// <param name="tos"></param>
        /// <param name="tosWeights"></param>
        /// <param name="maxSettles"></param>
        /// <param name="exists"></param>
        private void ExistsOneHop(IBasicRouterDataSource<CHEdgeData> graph, bool searchForward, uint from, List<uint> tos, List<float> tosWeights, int maxSettles, ref bool[] exists)
        {
            var toSet = new HashSet<uint>();
            float maxWeight = 0;
            for (int idx = 0; idx < tosWeights.Count; idx++)
            {
                if (!exists[idx])
                {
                    toSet.Add(tos[idx]);
                    if (maxWeight < tosWeights[idx])
                    {
                        maxWeight = tosWeights[idx];
                    }
                }
            }

            var neighbours = graph.GetEdges(from);
            while(neighbours.MoveNext())
            {
                if (toSet.Contains(neighbours.Neighbour))
                { // ok, this is a to-edge.
                    int index = tos.IndexOf(neighbours.Neighbour);
                    toSet.Remove(neighbours.Neighbour);

                    var edgeData = neighbours.EdgeData;
                    if (((searchForward && edgeData.CanMoveForward) || 
                        (!searchForward && edgeData.CanMoveBackward)) &&
                        edgeData.Weight < tosWeights[index])
                    { // direction ok and weight smaller.
                        exists[index] = true;
                    }

                    if (toSet.Count == 0)
                    {
                        break;
                    }
                }
            }
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
