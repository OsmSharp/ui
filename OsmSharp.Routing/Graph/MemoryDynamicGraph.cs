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
using OsmSharp.Math.Geo.Simple;

namespace OsmSharp.Routing.Graph
{
    /// <summary>
    /// An implementation of an in-memory dynamic graph.
    /// </summary>
    public class MemoryDynamicGraph<TEdgeData> : IDynamicGraph<TEdgeData>
        where TEdgeData : IDynamicGraphEdgeData
    {
        private const int EDGE_SIZE = 4;
        private const uint NO_EDGE = uint.MaxValue;
        private const int NODEA = 0;
        private const int NODEB = 1;
        private const int NEXTNODEA = 2;
        private const int NEXTNODEB = 3;

        /// <summary>
        /// Holds the next id.
        /// </summary>
        private uint _nextVertexId;

        /// <summary>
        /// Holds the next edge id.
        /// </summary>
        private uint _nextEdgeId;

        /// <summary>
        /// Holds the coordinates of the vertices.
        /// </summary>
        private GeoCoordinateSimple[] _coordinates;

        /// <summary>
        /// Holds all vertices pointing to it's first edge.
        /// </summary>
        private uint[] _vertices;

        /// <summary>
        /// Holds all edges (meaning vertex1-vertex2)
        /// </summary>
        private uint[] _edges;

        /// <summary>
        /// Holds all data associated with edges.
        /// </summary>
        private TEdgeData[] _edgeData;

        /// <summary>
        /// Creates a new in-memory graph.
        /// </summary>
        public MemoryDynamicGraph()
            : this(1000)
        {

        }

        /// <summary>
        /// Creates a new in-memory graph.
        /// </summary>
        public MemoryDynamicGraph(int sizeEstimate)
        {
            _nextVertexId = 1;
            _nextEdgeId = 0;
            _vertices = new uint[sizeEstimate];
            for (int idx = 0; idx < sizeEstimate; idx++)
            {
                _vertices[idx] = NO_EDGE;
            }
            _coordinates = new GeoCoordinateSimple[sizeEstimate];
            _edges = new uint[sizeEstimate * 3 * EDGE_SIZE];
            _edgeData = new TEdgeData[sizeEstimate * 3];
        }

        /// <summary>
        /// Increases the memory allocation for this dynamic graph.
        /// </summary>
        private void IncreaseSize()
        {
            throw new NotImplementedException();
            //var oldLength = _coordinates.Length;
            //Array.Resize<GeoCoordinateSimple>(ref _coordinates, _coordinates.Length + 10000);
            //Array.Resize<uint>(ref _vertices, _vertices.Length + 10000);
            //for (int idx = oldLength; idx < oldLength + 10000; idx++)
            //{
            //    _vertices[idx] = NO_EDGE;
            //}
            //Array.Resize<uint>(ref _edges, _edges.Length + 10000);
        }

        /// <summary>
        /// Adds a new vertex to this graph.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="neighboursEstimate"></param>
        /// <returns></returns>
        public uint AddVertex(float latitude, float longitude, byte neighboursEstimate)
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
            if (_nextVertexId >= _vertices.Length)
            {
                this.IncreaseSize();
            }

            // create vertex.
            uint newId = _nextVertexId;
            _coordinates[newId] = new GeoCoordinateSimple()
            {
                Latitude = latitude,
                Longitude = longitude
            };
            _nextVertexId++; // increase for next vertex.
            return newId;
        }

        /// <summary>
        /// Sets a vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public void SetVertex(uint vertex, float latitude, float longitude)
        {
            _coordinates[vertex].Latitude = latitude;
            _coordinates[vertex].Longitude = longitude;
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
            if (_nextVertexId > 1)
            {
                return Range.UInt32(1, (uint)_nextVertexId - 1, 1U);
            }
            return new List<uint>();
        }

        /// <summary>
        /// Adds an edge with the associated data.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="data"></param>
        public void AddEdge(uint vertex1, uint vertex2, TEdgeData data)
        {
            this.AddEdge(vertex1, vertex2, data, null);
        }

        /// <summary>
        /// Adds an edge with the associated data.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="data"></param>
        /// <param name="comparer">Comparator to compare edges and replace obsolete ones.</param>
        public void AddEdge(uint vertex1, uint vertex2, TEdgeData data, IDynamicGraphEdgeComparer<TEdgeData> comparer)
        {
            if (!data.Forward) { throw new ArgumentOutOfRangeException("data", "Edge data has to be forward."); }

            if (_vertices.Length > vertex1 && 
                _vertices.Length > vertex2)
            {
                var edgeId = _vertices[vertex1];
                if (_vertices[vertex1] != NO_EDGE)
                { // check for an existing edge first.
                    // check if the arc exists already.
                    edgeId = _vertices[vertex1];
                    uint nextEdgeSlot = 0;
                    while (edgeId != NO_EDGE)
                    { // keep looping.
                        uint otherVertexId = 0;
                        uint previousEdgeId = edgeId;
                        bool forward = true;
                        if (_edges[edgeId + NODEA] == vertex1)
                        {
                            otherVertexId = _edges[edgeId + NODEB];
                            nextEdgeSlot = edgeId + NEXTNODEA;
                            edgeId = _edges[edgeId + NEXTNODEA];
                        }
                        else
                        {
                            otherVertexId = _edges[edgeId + NODEA];
                            nextEdgeSlot = edgeId + NEXTNODEB;
                            edgeId = _edges[edgeId + NEXTNODEB];
                            forward = false;
                        }
                        if (otherVertexId == vertex2)
                        { // this is the edge we need.
                            if (!forward)
                            {
                                data = (TEdgeData)data.Reverse();
                            }
                            if(comparer != null)
                            { // there is a comparer.
                                var existingData = _edgeData[previousEdgeId / 4];
                                if (comparer.Overlaps(data, existingData))
                                { // an arc was found that represents the same directional information.
                                    _edgeData[previousEdgeId / 4] = data;
                                }
                                return;
                            }
                            _edgeData[previousEdgeId / 4] = data;
                            return;
                        }
                    }

                    // create a new edge.
                    edgeId = _nextEdgeId;
                    _edges[_nextEdgeId + NODEA] = vertex1;
                    _edges[_nextEdgeId + NODEB] = vertex2;
                    _edges[_nextEdgeId + NEXTNODEA] = NO_EDGE;
                    _edges[_nextEdgeId + NEXTNODEB] = NO_EDGE;
                    _nextEdgeId = _nextEdgeId + EDGE_SIZE;

                    // append the new edge to the from list.
                    _edges[nextEdgeSlot] = edgeId;

                    // set data.
                    _edgeData[edgeId / 4] = data;
                }
                else
                { // create a new edge and set.
                    edgeId = _nextEdgeId;
                    _vertices[vertex1] = _nextEdgeId;

                    _edges[_nextEdgeId + NODEA] = vertex1;
                    _edges[_nextEdgeId + NODEB] = vertex2;
                    _edges[_nextEdgeId + NEXTNODEA] = NO_EDGE;
                    _edges[_nextEdgeId + NEXTNODEB] = NO_EDGE;
                    _nextEdgeId = _nextEdgeId + EDGE_SIZE;

                    // set data.
                    _edgeData[edgeId / 4] = data;
                }

                var toEdgeId = _vertices[vertex2];
                if (toEdgeId != NO_EDGE)
                { // there are existing edges.
                    uint nextEdgeSlot = 0;
                    while (toEdgeId != NO_EDGE)
                    { // keep looping.
                        uint otherVertexId = 0;
                        if (_edges[edgeId + NODEA] == vertex2)
                        {
                            otherVertexId = _edges[toEdgeId + NODEB];
                            toEdgeId = _edges[toEdgeId + NEXTNODEA];
                            nextEdgeSlot = toEdgeId + NEXTNODEA;
                        }
                        else
                        {
                            otherVertexId = _edges[toEdgeId + NODEA];
                            toEdgeId = _edges[toEdgeId + NEXTNODEB];
                            nextEdgeSlot = toEdgeId + NEXTNODEB;
                        }
                    }
                    _vertices[vertex2] = nextEdgeSlot;
                }
                else
                { // there are no existing edges.
                    _vertices[vertex2] = _vertices[vertex1];
                }

                return;
            }
            throw new ArgumentOutOfRangeException("from");
        }

        /// <summary>
        /// Deletes all edges leading from/to the given vertex. 
        /// </summary>
        /// <param name="vertex"></param>
        public void RemoveEdges(uint vertex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the edge between the two given vertices.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void RemoveEdge(uint from, uint to)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns all arcs starting at the given vertex.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        public KeyValuePair<uint, TEdgeData>[] GetEdges(uint vertexId)
        {
            if (_vertices.Length > vertexId)
            {
                var edgeId = _vertices[vertexId];
                if (edgeId == NO_EDGE)
                { // there are no edges.
                    return new KeyValuePair<uint, TEdgeData>[0];
                }

                // loop over edges until a NO_EDGE is encountered.
                var edges = new List<KeyValuePair<uint, TEdgeData>>();
                while (edgeId != NO_EDGE)
                { // keep looping.
                    if (_edges[edgeId + NODEA] == vertexId)
                    {
                        var otherVertexId = _edges[edgeId + NODEB];
                        edges.Add(
                            new KeyValuePair<uint, TEdgeData>(otherVertexId, _edgeData[edgeId / 4]));
                        edgeId = _edges[edgeId + NEXTNODEA];
                    }
                    else
                    {
                        var otherVertexId = _edges[edgeId + NODEA];
                        edges.Add(
                            new KeyValuePair<uint, TEdgeData>(otherVertexId, (TEdgeData)_edgeData[edgeId / 4].Reverse()));
                        edgeId = _edges[edgeId + NEXTNODEB];
                    }
                }

                return edges.ToArray();
            }
            return new KeyValuePair<uint, TEdgeData>[0]; // return empty data if the vertex does not exist!
        }

        /// <summary>
        /// Returns true if the given vertex has neighbour as a neighbour.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <param name="neighbour"></param>
        /// <returns></returns>
        public bool ContainsEdge(uint vertexId, uint neighbour)
        {
            if (_vertices.Length > vertexId)
            { // edge out of range.
                if (_vertices[vertexId] == NO_EDGE)
                { // no edges here!
                    return false;
                }
                var edgeId = _vertices[vertexId];
                uint nextEdgeSlot = 0;
                while (edgeId != NO_EDGE)
                { // keep looping.
                    uint otherVertexId = 0;
                    if (_edges[edgeId + NODEA] == vertexId)
                    {
                        otherVertexId = _edges[edgeId + NODEB];
                        edgeId = _edges[edgeId + NEXTNODEA];
                        nextEdgeSlot = edgeId + NEXTNODEA;
                    }
                    else
                    {
                        otherVertexId = _edges[edgeId + NODEA];
                        edgeId = _edges[edgeId + NEXTNODEB];
                        nextEdgeSlot = edgeId + NEXTNODEB;
                    }
                    if (otherVertexId == neighbour)
                    { // this is the edge we need.
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the data associated with the given edge and return true if it exists.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool GetEdge(uint vertex1, uint vertex2, out TEdgeData data)
        {
            if (_vertices.Length > vertex1 &&
                _vertices.Length > vertex2)
            { // edge out of range.
                if (_vertices[vertex1] == NO_EDGE)
                { // no edges here!
                    data = default(TEdgeData);
                    return false;
                }
                var edgeId = _vertices[vertex1];
                uint nextEdgeSlot = 0;
                while (edgeId != NO_EDGE)
                { // keep looping.
                    uint otherVertexId = 0;
                    var currentEdgeId = edgeId;
                    if (_edges[edgeId + NODEA] == vertex1)
                    {
                        otherVertexId = _edges[edgeId + NODEB];
                        edgeId = _edges[edgeId + NEXTNODEA];
                        nextEdgeSlot = edgeId + NEXTNODEA;
                    }
                    else
                    {
                        otherVertexId = _edges[edgeId + NODEA];
                        edgeId = _edges[edgeId + NEXTNODEB];
                        nextEdgeSlot = edgeId + NEXTNODEB;
                    }
                    if (otherVertexId == vertex2)
                    { // this is the edge we need.
                        data = _edgeData[currentEdgeId / EDGE_SIZE];
                        return true;
                    }
                }
            }
            data = default(TEdgeData);
            return false;
        }

        /// <summary>
        /// Trims the size of this graph.
        /// </summary>
        /// <param name="max"></param>
        public void Trim(uint max)
        {
            throw new NotImplementedException();
            //Array.Resize<GeoCoordinateSimple>(ref _coordinates, (int)max);
            //// Array.Resize<KeyValuePair<uint, TEdgeData>[]>(ref _vertices, (int)max);

            //_nextVertexId = max;
        }

        /// <summary>
        /// Returns the number of vertices in this graph.
        /// </summary>
        public uint VertexCount
        {
            get { return _nextVertexId - 1; }
        }


        public void Trim()
        {
            throw new NotImplementedException();
        }
    }
}