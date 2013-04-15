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

namespace OsmSharp.Routing.Graph.DynamicGraph.PreProcessed
{
    /// <summary>
    /// An implementation of an in-memory dynamic graph containing edge with pre-processed weights and directions.
    /// </summary>
    public class PreProcessedDynamicGraph: IDynamicGraph<PreProcessedEdge>
    {
        /// <summary>
        /// Holds the next id.
        /// </summary>
        private uint _next_id;

        /// <summary>
        /// Holds the count of free edges.
        /// </summary>
        private uint _free_edge_space;

        /// <summary>
        /// The default space to allocate for edges.
        /// </summary>
        private const ushort _default_edge_space = 1;

        /// <summary>
        /// Holds all graph data.
        /// </summary>
        private Vertex[] _vertices;

        /// <summary>
        /// Holds all the edge data.
        /// </summary>
        private List<Edge> _edges;

        /// <summary>
        /// Creates a new in-memory graph.
        /// </summary>
        public PreProcessedDynamicGraph()
        {
            _next_id = 1;
            _vertices = new Vertex[1000];
            _edges = new List<Edge>(1000);
        }

        /// <summary>
        /// Increases the memory allocation for this dynamic graph.
        /// </summary>
        private void IncreaseSize()
        {
            Array.Resize<Vertex>(ref _vertices, _vertices.Length + 1000);
        }

        /// <summary>
        /// Adds a vertex to this graph.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="neighbours_estimate"></param>
        /// <returns></returns>
        public uint AddVertex(float latitude, float longitude, byte neighbours_estimate)
        {
            return this.AddVertex(latitude, longitude);
        }

        /// <summary>
        /// Adds a new vertex.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public uint AddVertex(float latitude, float longitude)
        {
            // make sure vertices array is large enough.
            if (_next_id >= _vertices.Length)
            {
                this.IncreaseSize();
            }

            // create vertex.
            uint new_id = _next_id;
            _vertices[new_id].Latitude = latitude;
            _vertices[new_id].Longitude = longitude;
            _next_id++; // increase for next vertex.

            // allocate edges.
            _vertices[new_id].EdgeIndex = (uint)_edges.Count;
            _vertices[new_id].EdgeCount = 0;
            _vertices[new_id].EdgeSpace = _default_edge_space;

            // allocate edges memory.
            _free_edge_space = _free_edge_space + _default_edge_space;
            for (int idx = 0; idx < _default_edge_space; idx++)
            {
                _edges.Add(new Edge() { VertexId = uint.MaxValue });
            }
            return new_id;
        }

        /// <summary>
        /// Returns the information in the current vertex.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public bool GetVertex(uint id, out float latitude, out float longitude)
        {
            if (_vertices.Length > id)
            {
                latitude = _vertices[id].Latitude;
                longitude = _vertices[id].Longitude;
                return true;
            }
            latitude = float.MaxValue;
            longitude = float.MaxValue;
            return false;
        }

        /// <summary>
        /// Returns an enumerable of all vertices.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<uint> GetVertices()
        {
            if (_next_id > 1)
            {
                return OsmSharp.Tools.Range.UInt32(1, (uint)_next_id - 1, 1U);
            }
            return new List<uint>();
        }

        /// <summary>
        /// Adds and arc to an existing vertex.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="data"></param>
        /// <param name="comparer"></param>
        public void AddArc(uint from, uint to, PreProcessedEdge data, IDynamicGraphEdgeComparer<PreProcessedEdge> comparer)
        {
            Edge edge;
            if (_vertices.Length > from)
            { // adds a new arc.
                Vertex vertex = _vertices[from];

                // check if edge exists already.
                for (int idx = 0; idx < vertex.EdgeCount; idx++)
                {
                    edge = _edges[(int)vertex.EdgeIndex + idx];
                    if (edge.VertexId == to)
                    { // edge exists; only add shorter edges.
                        if (edge.Weight > data.Weight)
                        {
                            edge.Weight = (float)data.Weight;
                            _edges[(int)vertex.EdgeIndex + idx] = edge;
                        }
                        return; // never allow duplicates
                    }
                }

                // check if there is still room.
                if (vertex.EdgeCount == vertex.EdgeSpace)
                { // allocate new edges.

                    _free_edge_space = _free_edge_space + vertex.EdgeCount;

                    // move edges.
                    for (int idx = 0; idx < vertex.EdgeCount; idx++)
                    {
                        _edges.Add(_edges[idx + (int)vertex.EdgeIndex]);
                        _edges[idx + (int)vertex.EdgeIndex] = new Edge() { VertexId = uint.MaxValue };
                    }
                    for (int idx = 0; idx < vertex.EdgeCount; idx++)
                    {
                        _edges.Add(new Edge() { VertexId = uint.MaxValue });
                    }

                    // point to the new edge positions.
                    vertex.EdgeSpace = (ushort)(vertex.EdgeSpace + vertex.EdgeSpace);
                    vertex.EdgeIndex = (uint)_edges.Count - vertex.EdgeSpace;
                }

                // increase the edge count.
                vertex.EdgeCount++;
                _free_edge_space = _free_edge_space--;

                // create the edge.
                edge = new Edge();
                edge.Backward = data.Backward;
                edge.Forward = data.Forward;
                edge.Weight = (float)data.Weight;
                edge.Tags = data.Tags;
                edge.VertexId = to;
                _edges[(int)vertex.EdgeIndex + vertex.EdgeCount - 1] = edge;

                // set the vertex.
                _vertices[from] = vertex;
                return;
            }
            throw new ArgumentOutOfRangeException("from");
        }

        /// <summary>
        /// Removes all arcs starting at from ending at to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void DeleteArc(uint from, uint to)
        {
            if (_vertices.Length > from)
            {
                // get the vertex.
                Vertex vertex = _vertices[from];

                // loop over all edge and remove.
                for (int idx = 0; idx < vertex.EdgeCount; idx++)
                {
                    while (_edges[(int)vertex.EdgeIndex + idx].VertexId == to)
                    { // remove the edge.
                        if (vertex.EdgeCount > idx)
                        { // the edge can still be replace by the last edge.
                            _edges[(int)vertex.EdgeIndex + idx] =
                                _edges[(int)vertex.EdgeIndex + vertex.EdgeCount - 1];
                        }
                        else
                        { // the current edge is the last edge; just decrease the edge count.

                        }
                        vertex.EdgeCount--;
                        _free_edge_space++;
                    }
                }

                // set the vertex.
                _vertices[from] = vertex;
            }
            throw new ArgumentOutOfRangeException("from");
        }

        /// <summary>
        /// Returns all arcs starting at the given vertex.
        /// </summary>
        /// <param name="vertex_id"></param>
        /// <returns></returns>
        public KeyValuePair<uint, PreProcessedEdge>[] GetArcs(uint vertex_id)
        {
            if (_vertices.Length > vertex_id)
            {
                // get the vertex.
                Vertex vertex = _vertices[vertex_id];

                KeyValuePair<uint, PreProcessedEdge>[] arcs = new KeyValuePair<uint, PreProcessedEdge>[
                    vertex.EdgeCount];
                // loop over all edges
                for (int idx = 0; idx < vertex.EdgeCount; idx++)
                {
                    Edge edge = _edges[(int)vertex.EdgeIndex + idx];

                    arcs[idx] = new KeyValuePair<uint, PreProcessedEdge>(
                        _edges[(int)vertex.EdgeIndex + idx].VertexId, 
                            new PreProcessedEdge(
                                edge.Weight,
                                edge.Forward,
                                edge.Backward,
                                edge.Tags));
                }
                return arcs;
            }
            return new KeyValuePair<uint, PreProcessedEdge>[0]; // return empty data if the vertex does not exist!
        }

        /// <summary>
        /// Returns true if the given vertex has neighbour as a neighbour.
        /// </summary>
        /// <param name="vertex_id"></param>
        /// <param name="neighbour"></param>
        /// <returns></returns>
        public bool HasNeighbour(uint vertex_id, uint neighbour)
        {
            if (_vertices.Length > vertex_id)
            {
                // get the vertex.
                Vertex vertex = _vertices[vertex_id];

                KeyValuePair<uint, PreProcessedEdge>[] arcs = new KeyValuePair<uint, PreProcessedEdge>[
                    vertex.EdgeCount];
                // loop over all edges
                for (int idx = 0; idx < vertex.EdgeCount; idx++)
                {
                    if (_edges[(int)vertex.EdgeIndex + idx].VertexId == vertex_id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Represents a simple vertex.
        /// </summary>
        private struct Vertex
        {
            /// <summary>
            /// Holds the latitude.
            /// </summary>
            public float Latitude { get; set; }

            /// <summary>
            /// Holds longitude.
            /// </summary>
            public float Longitude { get; set; }

            /// <summary>
            /// The edges index.
            /// </summary>
            public uint EdgeIndex { get; set; }

            /// <summary>
            /// The amount of edges.
            /// </summary>
            public ushort EdgeCount { get; set; }

            /// <summary>
            /// The amount of space for edges.
            /// </summary>
            public ushort EdgeSpace { get; set; }
        }

        /// <summary>
        /// Represents an edge.
        /// </summary>
        private struct Edge
        {
            /// <summary>
            /// The target vector for the edge.
            /// </summary>
            public uint VertexId { get; set; }

            /// <summary>
            /// The forward flag of the edge.
            /// </summary>
            public bool Forward { get; set; }

            /// <summary>
            /// The backward flag of the edge.
            /// </summary>
            public bool Backward { get; set; }

            /// <summary>
            /// The weight of the edge.
            /// </summary>
            public float Weight { get; set; }

            /// <summary>
            /// The tags for this edge.
            /// </summary>
            public uint Tags { get; set; }
        }

        /// <summary>
        /// Returns the number of vertices in this graph.
        /// </summary>
        public uint VertexCount
        {
            get { return _next_id - 1; }
        }
    }
}
