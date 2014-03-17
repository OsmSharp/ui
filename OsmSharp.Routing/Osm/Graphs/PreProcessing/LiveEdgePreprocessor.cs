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

using OsmSharp.Logging;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.PreProcessor;
using OsmSharp.Routing.Graph.Router;
using System.Collections.Generic;

namespace OsmSharp.Routing.Osm.Graphs.PreProcessing
{
    /// <summary>
    /// Pre-processor to simplify a graph made out of live edges.
    /// </summary>
    public class LiveEdgePreprocessor : IPreProcessor, IDynamicGraphEdgeComparer<LiveEdge>
    {
        /// <summary>
        /// Holds the graph.
        /// </summary>
        private IDynamicGraph<LiveEdge> _graph;

        /// <summary>
        /// Creates a new pre-processor.
        /// </summary>
        /// <param name="target"></param>
        public LiveEdgePreprocessor(IDynamicGraph<LiveEdge> graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Starts pre-processing all nodes.
        /// </summary>
        public void Start()
        {
            // build the empty coordinate list.
            var emptyCoordinateList = new GeoCoordinateSimple[0];
            var verticesList = new HashSet<uint>();

            // initialize status variables.
            uint nextToProcess = 0;
            uint nextPosition = 0;

            // search edge until a real node.
            double latestProgress = 0;
            while(nextToProcess < _graph.VertexCount)
            { // keep looping until all vertices have been processed.
                // select a new vertext to select.
                uint vertexToProcess = nextToProcess;
                KeyValuePair<uint, LiveEdge>[] edges = _graph.GetArcs(vertexToProcess);
                if(edges.Length == 2)
                { // find one of the neighbours that is usefull.
                    vertexToProcess = edges[0].Key;
                    edges = _graph.GetArcs(vertexToProcess);
                    verticesList.Clear();
                    verticesList.Add(vertexToProcess);
                    while(edges.Length == 2)
                    { // keep looping until there is a vertex that is usefull.
                        vertexToProcess = edges[0].Key;
                        if (verticesList.Contains(vertexToProcess))
                        { // take the other vertex.
                            vertexToProcess = edges[1].Key;
                            if (verticesList.Contains(vertexToProcess))
                            { // an island was detected with only vertices having two neighbours.
                                // TODO: find a way to handle this!
                                edges = new KeyValuePair<uint, LiveEdge>[0];
                                break;
                            }
                        }
                        verticesList.Add(vertexToProcess);
                        edges = _graph.GetArcs(vertexToProcess);
                    }
                }
                if(edges.Length > 0)
                { // ok, the vertex was not already processed.
                    nextPosition++;
                    var oldEdges = edges.Clone() as KeyValuePair<uint, LiveEdge>[];
                    var ignoreList = new HashSet<uint>();
                    foreach (var oldEdge in oldEdges)
                    { 
                        if(ignoreList.Contains(oldEdge.Key))
                        { // ignore this edge: already removed in a previous iteration.
                            break;
                        }

                        // don't re-process edges that already have coordinates.
                        if (oldEdge.Value.Coordinates != null)
                        { // this edge has already been processed.
                            break;
                        }

                        // STEP1: Build list of vertices that are only for form.

                        // set current/previous.
                        var distance = oldEdge.Value.Distance;
                        var current = oldEdge.Key;
                        var previous = vertexToProcess;

                        // build list of vertices.
                        var vertices = new List<uint>();
                        vertices.Add(previous);
                        vertices.Add(current);

                        // get next edges list.
                        var nextEdges = _graph.GetArcs(current);
                        while (nextEdges.Length == 2)
                        { // ok the current vertex can be removed.
                            var nextEdge = nextEdges[0];
                            if (nextEdge.Key == previous)
                            { // it's the other edge!
                                nextEdge = nextEdges[1];
                            }

                            // compare edges.
                            if(nextEdge.Value.Forward != oldEdge.Value.Forward ||
                                nextEdge.Value.Tags != oldEdge.Value.Tags)
                            { // oeps, edges are different!
                                break;
                            }

                            // check for intermediates.
                            if(nextEdge.Value.Coordinates != null)
                            { // oeps, there are intermediates already, this can occur when two osm-ways are drawn on top of eachother.
                                break;
                            }

                            // add distance.
                            distance = distance + nextEdge.Value.Distance;

                            // set current/previous.
                            previous = current;
                            current = nextEdge.Key;
                            vertices.Add(current);

                            // get next edges.
                            nextEdges = _graph.GetArcs(current);
                        }

                        // check if the edge contains intermediate points.
                        if (vertices.Count == 2)
                        { // no intermediate points: add the empty coordinate list.
                            var oldEdgeValue = oldEdge.Value;
                            oldEdgeValue.Coordinates = emptyCoordinateList;
                            
                            // keep edges that already have intermediates.
                            var edgesToKeep = new List<KeyValuePair<uint, LiveEdge>>();
                            foreach(var edgeToKeep in _graph.GetArcs(vertexToProcess))
                            {
                                if(edgeToKeep.Key == oldEdge.Key && 
                                    edgeToKeep.Value.Coordinates != null)
                                {
                                    edgesToKeep.Add(edgeToKeep);
                                }
                            }

                            // delete olds arcs.
                            _graph.DeleteArc(vertexToProcess, oldEdge.Key);

                            // add new arc.
                            _graph.AddArc(vertexToProcess, oldEdge.Key, oldEdgeValue, null);

                            // add edges to keep.
                            foreach(var edgeToKeep in edgesToKeep)
                            {
                                _graph.AddArc(vertexToProcess, edgeToKeep.Key, edgeToKeep.Value, null);
                            }
                        }
                        else
                        { // intermediate points: build array.
                            // STEP2: Build array of coordinates.
                            var coordinates = new GeoCoordinateSimple[vertices.Count - 2];
                            float latitude, longitude;
                            for (int idx = 1; idx < vertices.Count - 1; idx++)
                            {
                                _graph.GetVertex(vertices[idx], out latitude, out longitude);
                                coordinates[idx - 1] = new GeoCoordinateSimple()
                                {
                                    Latitude = latitude,
                                    Longitude = longitude
                                };
                            }

                            // STEP3: Remove all unneeded edges.
                            _graph.DeleteArc(vertices[0], vertices[1]); // remove first edge.
                            for (int idx = 1; idx < vertices.Count - 1; idx++)
                            { // delete all intermidiate arcs.
                                _graph.DeleteArc(vertices[idx]);
                            }
                            _graph.DeleteArc(vertices[vertices.Count - 1], vertices[vertices.Count - 2]); // remove last edge.
                            if (vertices[0] == vertices[vertices.Count - 1])
                            { // also remove outgoing edge.
                                ignoreList.Add(vertices[vertices.Count - 2]); // make sure this arc is ignored in next iteration.
                            }

                            // STEP4: Add new edges.
                            _graph.AddArc(vertices[0], vertices[vertices.Count - 1], new LiveEdge()
                            {
                                Coordinates = coordinates,
                                Forward = oldEdge.Value.Forward,
                                Tags = oldEdge.Value.Tags,
                                Distance = distance
                            }, this);
                            var reverse = new GeoCoordinateSimple[coordinates.Length];
                            coordinates.CopyToReverse(reverse, 0);
                            _graph.AddArc(vertices[vertices.Count - 1], vertices[0], new LiveEdge()
                            {
                                Coordinates = reverse,
                                Forward = !oldEdge.Value.Forward,
                                Tags = oldEdge.Value.Tags,
                                Distance = distance
                            }, this);
                        }
                    }
                }
                // move to the next position.
                nextToProcess++;

                // report progress.
                float progress = (float)System.Math.Round((((double)nextToProcess / (double)_graph.VertexCount) * 100));
                if (progress != latestProgress)
                {
                    OsmSharp.Logging.Log.TraceEvent("LiveEdgePreprocessor", TraceEventType.Information,
                        "Compressing graph... {0}%", progress);
                    latestProgress = progress;
                }
            }

            // compress the graph.
            this.CompressGraph();
        }

        /// <summary>
        /// Compresses the graph by deleting vertices.
        /// </summary>
        private void CompressGraph()
        {
            OsmSharp.Logging.Log.TraceEvent("", Logging.TraceEventType.Critical, "");

            // initialize status variables.
            uint vertex = 1;
            uint nextCompressedPosition = 1;

            // search edge until a real node.
            float latitude, longitude;
            while (vertex <= _graph.VertexCount)
            {
                var edges = _graph.GetArcs(vertex);
                if (edges != null && edges.Length > 0)
                { // ok, this vertex has edges.
                    if (nextCompressedPosition != vertex)
                    { // this vertex should go in another place.
                        _graph.GetVertex(vertex, out latitude, out longitude);

                        // set the next coordinates.
                        _graph.SetVertex(nextCompressedPosition, latitude, longitude);

                        // set the new edges.
                        _graph.DeleteArc(nextCompressedPosition);
                        foreach (var edge in edges)
                        { // add all arcs.
                            if (edge.Key != vertex)
                            { // this edge is not an edge that has the same end-start point.
                                _graph.AddArc(nextCompressedPosition, edge.Key, edge.Value, null);
                            }
                            else
                            { // this edge is an edge that has the same end-start point.
                                _graph.AddArc(nextCompressedPosition, nextCompressedPosition, edge.Value, null);
                            }

                            // update other arcs.
                            if (edge.Key != vertex)
                            { // do not update other arcs if other vertex is the same.
                                var reverseEdges = _graph.GetArcs(edge.Key);
                                if (reverseEdges != null)
                                { // there are reverse edges, check if there is a reference to vertex.
                                    reverseEdges = reverseEdges.Clone() as KeyValuePair<uint, LiveEdge>[];
                                    foreach (var reverseEdge in reverseEdges)
                                    { // check each edge for vertex.
                                        if (reverseEdge.Key == vertex)
                                        { // ok, replace this edge.
                                            _graph.DeleteArc(edge.Key, vertex);
                                            _graph.AddArc(edge.Key, nextCompressedPosition, reverseEdge.Value, null);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    nextCompressedPosition++;
                }

                // move to the next vertex.
                vertex++;
            }

            // remove all extra space.
            _graph.Trim(nextCompressedPosition);
        }

        /// <summary>
        /// Returns true if the given edge1 overlaps the given edge2.
        /// </summary>
        /// <param name="edge1"></param>
        /// <param name="edge2"></param>
        /// <returns></returns>
        public bool Overlaps(LiveEdge edge1, LiveEdge edge2)
        {
            if (edge1.Forward == edge2.Forward &&
                edge1.Tags == edge2.Tags)
            {
                return edge1.Coordinates != null && 
                    edge2.Coordinates == null;
            }
            return false;
        }
    }
}