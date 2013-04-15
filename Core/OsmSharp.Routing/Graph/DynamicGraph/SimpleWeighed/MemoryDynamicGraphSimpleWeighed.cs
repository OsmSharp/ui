// OsmSharp - OpenStreetMap tools & library.
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
using OsmSharp.Tools.Math.Geo.Simple;

namespace OsmSharp.Routing.Graph.DynamicGraph.SimpleWeighed
{
    /// <summary>
    /// An implementation of an in-memory dynamic graph.
    /// </summary>
    public class MemoryDynamicGraphSimpleWeighed : IDynamicGraph<SimpleWeighedEdge>
    {
        /// <summary>
        /// Holds the next id.
        /// </summary>
        private uint _next_id;

        /// <summary>
        /// Holds all edges.
        /// </summary>
        private SimpleWeighedEdgeStruct[] _arcs;

        /// <summary>
        /// Holds the initial edge capacity.
        /// </summary>
        private byte _initial_edge_capactity = 2;

        /// <summary>
        /// Holds the next avialable edge position.
        /// </summary>
        private int _next_available_edge_position = 0;

        /// <summary>
        /// Holds all vertices.
        /// </summary>
        private Vertex[] _vertices;

        /// <summary>
        /// Holds the coordinates of the vertices.
        /// </summary>
        private GeoCoordinateSimple[] _coordinates;

        /// <summary>
        /// Creates a new in-memory graph.
        /// </summary>
        public MemoryDynamicGraphSimpleWeighed()
        {
            _next_id = 1;
            _vertices = new Vertex[1000];
            _coordinates = new GeoCoordinateSimple[1000];
            _arcs = new SimpleWeighedEdgeStruct[1000];
        }

        /// <summary>
        /// Increases the memory allocation for this dynamic graph.
        /// </summary>
        private void IncreaseSizeVertices()
        {
            Array.Resize<GeoCoordinateSimple>(ref _coordinates, _coordinates.Length + 1000);
            Array.Resize<Vertex>(ref _vertices, _vertices.Length + 1000);
        }

        /// <summary>
        /// Increases the size of the edges array.
        /// </summary>
        private void IncreaseSizeEdges()
        {
            Array.Resize<SimpleWeighedEdgeStruct>(ref _arcs, _arcs.Length + 1000);
        }

        /// <summary>
        /// Adds a new vertex this this graph.
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
                this.IncreaseSizeVertices();
            }

            // create vertex.
            uint new_id = _next_id;
            _vertices[new_id] = new Vertex()
            {
                Capacity = _initial_edge_capactity,
                Count = 0,
                Index = _next_available_edge_position
            };
            _next_available_edge_position = _next_available_edge_position + _initial_edge_capactity;
            while (_arcs.Length <= _next_available_edge_position)
            {
                this.IncreaseSizeEdges();
            }
            _coordinates[new_id] = new GeoCoordinateSimple()
            {
                Latitude = latitude,
                Longitude = longitude
            };
            _next_id++; // increase for next vertex.
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
                latitude = _coordinates[id].Latitude;
                longitude = _coordinates[id].Longitude;
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
        public void AddArc(uint from, uint to, SimpleWeighedEdge data, 
            IDynamicGraphEdgeComparer<SimpleWeighedEdge> comparer)
        {
            if (_vertices.Length > from)
            {
                // get vertex.
                Vertex vertex = _vertices[from];

                // get existing arcs.
                for (int idx = 0; idx < vertex.Count; idx++)
                {
                    if (_arcs[vertex.Index + idx].Id == to &&
                        _arcs[vertex.Index + idx].Weight > data.Weight &&
                        (comparer != null && comparer.Overlaps(this.ToClass(_arcs[vertex.Index + idx]).Value, data)))
                    { // an arc was found that represents the same directional information.
                        _arcs[vertex.Index + idx] = this.ToStruct(to, data);
                        return;
                    }
                }

                // if there: there does not exist a valid arc yet!
                if (vertex.Count == vertex.Capacity)
                { // move vertex.
                    int new_idx = _next_available_edge_position;
                    _next_available_edge_position = _next_available_edge_position + vertex.Capacity * 2;
                    while (_arcs.Length <= _next_available_edge_position)
                    {
                        this.IncreaseSizeEdges();
                    }
                    for (int idx = 0; idx < vertex.Count; idx++)
                    {
                        _arcs[new_idx + idx] = _arcs[vertex.Index + idx];
                    }
                    vertex.Index = new_idx;
                    vertex.Capacity = (byte)(vertex.Capacity * 2);
                }

                // add edge.
                _arcs[vertex.Index + vertex.Count] = this.ToStruct(to, data);

                // update vertex.
                vertex.Count++;
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
                // get vertex.
                Vertex vertex = _vertices[from];

                if (vertex.Count > 0)
                {
                    for (int idx = vertex.Count - 1; idx >= 0; idx--)
                    {
                        if (_arcs[vertex.Index + idx].Id ==
                            to)
                        { // remove this by switching the last edge to this location.
                            if (idx != vertex.Count - 1)
                            {
                                _arcs[vertex.Index + idx] =
                                    _arcs[vertex.Index + vertex.Count - 1];
                            }

                            // ... and decrease the edge count.
                            vertex.Count--;
                        }
                    }
                }

                _vertices[from] = vertex;
                return;
            }
            throw new ArgumentOutOfRangeException("from");
        }

        /// <summary>
        /// Returns all arcs starting at the given vertex.
        /// </summary>
        /// <param name="vertex_id"></param>
        /// <returns></returns>
        public KeyValuePair<uint, SimpleWeighedEdge>[] GetArcs(uint vertex_id)
        {
            if (_vertices.Length > vertex_id)
            {
                // get vertex.
                Vertex vertex = _vertices[vertex_id];

                if (vertex.Count == 0)
                {
                    return new KeyValuePair<uint, SimpleWeighedEdge>[0];
                }

                KeyValuePair<uint, SimpleWeighedEdge>[] arcs = new KeyValuePair<uint, SimpleWeighedEdge>[vertex.Count];
                for (int idx = 0; idx < vertex.Count; idx++)
                {
                    arcs[idx] = this.ToClass(_arcs[vertex.Index + idx]);
                }
                return arcs;
            }
            return new KeyValuePair<uint, SimpleWeighedEdge>[0]; // return empty data if the vertex does not exist!
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
                // get vertex.
                Vertex vertex = _vertices[vertex_id];

                if (vertex.Count == 0)
                {
                    return false;
                }

                KeyValuePair<uint, SimpleWeighedEdge>[] arcs = new KeyValuePair<uint, SimpleWeighedEdge>[vertex.Count];
                for (int idx = 0; idx < vertex.Count; idx++)
                {
                    if (_arcs[vertex.Index + idx].Id == neighbour)
                    {
                        return true;
                    }
                }
            }
            return false; // return empty data if the vertex does not exist!
        }

        private SimpleWeighedEdgeStruct ToStruct(uint id, SimpleWeighedEdge edge)
        {
            return new SimpleWeighedEdgeStruct()
            {
                Id = id,
                //IsForward = edge.IsForward,
                //Tags = edge.Tags,
                Weight = (float)edge.Weight
            };
        }

        private KeyValuePair<uint, SimpleWeighedEdge> ToClass(SimpleWeighedEdgeStruct edge)
        {
            return new KeyValuePair<uint,SimpleWeighedEdge>(edge.Id, new SimpleWeighedEdge()
            {
                //IsForward = edge.IsForward,
                //Tags = edge.Tags,
                Weight = edge.Weight
            });
        }

        /// <summary>
        /// Represents a simple vertex.
        /// </summary>
        private struct SimpleWeighedEdgeStruct
        {
            public uint Id { get; set; }
            ///// <summary>
            ///// Flag indicating if this is a forward or backward edge relative to the tag descriptions.
            ///// </summary>
            //public bool IsForward
            //{
            //    get;
            //    set;
            //}

            /// <summary>
            /// The weight of this edge.
            /// </summary>
            public float Weight
            {
                get;
                set;
            }

            ///// <summary>
            ///// The properties of this edge.
            ///// </summary>
            //public uint Tags
            //{
            //    get;
            //    set;
            //}
        }

        /// <summary>
        /// Represents a simple vertex.
        /// </summary>
        private struct Vertex
        {
            /// <summary>
            /// The index of the vertex edges.
            /// </summary>
            public int Index { get; set; }

            /// <summary>
            /// The number of edges for this vertex.
            /// </summary>
            public byte Count { get; set; }

            /// <summary>
            /// The capacity of the storage space for edges.
            /// </summary>
            public byte Capacity { get; set; }
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