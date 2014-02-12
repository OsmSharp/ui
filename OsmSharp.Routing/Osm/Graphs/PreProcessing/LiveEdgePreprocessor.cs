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

using OsmSharp.Routing.Graph.PreProcessor;
using OsmSharp.Routing.Graph.Router;
using System.Collections.Generic;

namespace OsmSharp.Routing.Osm.Graphs.PreProcessing
{
    /// <summary>
    /// Pre-processor to simplify a graph made out of live edges.
    /// </summary>
    public class LiveEdgePreprocessor : IPreProcessor
    {
        /// <summary>
        /// Holds the graph.
        /// </summary>
        private IDynamicGraphRouterDataSource<LiveEdge> _graph;

        /// <summary>
        /// Creates a new pre-processor.
        /// </summary>
        /// <param name="target"></param>
        public LiveEdgePreprocessor(IDynamicGraphRouterDataSource<LiveEdge> graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Starts pre-processing all nodes.
        /// </summary>
        public void Start()
        {
            uint nextToProcess = 0;
            uint nextPosition = 0;

            // search edge until a real node.
            while(nextToProcess < _graph.VertexCount)
            { // keep looping until all vertices have been processed.
                // select a new vertext to select.
                uint vertexToProcess = nextToProcess;
                KeyValuePair<uint, LiveEdge>[] edges = _graph.GetArcs(vertexToProcess);
                if(edges.Length == 2)
                { // find one of the neighbours that is usefull.
                    vertexToProcess = edges[0].Key;
                    edges = _graph.GetArcs(vertexToProcess);
                    while(edges.Length == 2)
                    { // keep looping until there is a vertex that is usefull.
                        vertexToProcess = edges[0].Key;
                        edges = _graph.GetArcs(vertexToProcess);
                    }
                }
                if(edges.Length > 0)
                { // ok, the vertex was not already processed.

                    nextPosition++;
                }
                // move to the next position.
                nextToProcess++;
            }
        }

        /// <summary>
        /// Simplify the edges starting at the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="edges"></param>
        private void SimplifyEdges(uint vertex, KeyValuePair<uint, LiveEdge>[] edges)
        {

        }
    }
}