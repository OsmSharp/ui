// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using OsmSharp.Collections.Arrays;
using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.Math.Geo.Simple;
using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Graph
{
    /// <summary>
    /// An implementation of an in-memory dynamic graph but with explicitly directed edges.
    /// </summary>
    public class MemoryDirectedGraph<TEdgeData> : IGraph<TEdgeData>
        where TEdgeData : IGraphEdgeData
    {
        protected const int EDGE_SIZE = 2;
        protected const uint NO_EDGE = uint.MaxValue;
        protected const int NODEA = 0;
        protected const int NEXTNODEA = 1;

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
        private IHugeArray<GeoCoordinateSimple> _coordinates;

        /// <summary>
        /// Holds all vertices pointing to it's first edge.
        /// </summary>
        private IHugeArray<uint> _vertices;

        /// <summary>
        /// Holds all edges (meaning vertex1-vertex2)
        /// </summary>
        private IHugeArray<uint> _edges;

        /// <summary>
        /// Holds all data associated with edges.
        /// </summary>
        private IHugeArray<TEdgeData> _edgeData;

        /// <summary>
        /// Holds all shapes associated with edges.
        /// </summary>
        private HugeCoordinateCollectionIndex _edgeShapes;

        /// <summary>
        /// Creates a new in-memory graph.
        /// </summary>
        public MemoryDirectedGraph()
            : this(1000)
        {

        }

        /// <summary>
        /// Creates a new in-memory graph.
        /// </summary>
        public MemoryDirectedGraph(long sizeEstimate)
            : this(sizeEstimate, new HugeArray<GeoCoordinateSimple>(sizeEstimate), new HugeArray<uint>(sizeEstimate), new HugeArray<uint>(sizeEstimate * 3 * EDGE_SIZE), new HugeArray<TEdgeData>(sizeEstimate * 3), new HugeCoordinateCollectionIndex(sizeEstimate * 3))
        {

        }

        /// <summary>
        /// Creates a new in-memory graph.
        /// </summary>
        /// <param name="sizeEstimate"></param>
        /// <param name="coordinateArray"></param>
        /// <param name="vertexArray"></param>
        /// <param name="edgesArray"></param>
        /// <param name="edgeDataArray"></param>
        /// <param name="edgeShapeArray"></param>
        protected MemoryDirectedGraph(long sizeEstimate, IHugeArray<GeoCoordinateSimple> coordinateArray, IHugeArray<uint> vertexArray, IHugeArray<uint> edgesArray, IHugeArray<TEdgeData> edgeDataArray, HugeCoordinateCollectionIndex edgeShapeArray)
        {
            _nextVertexId = 1;
            _nextEdgeId = 0;
            _vertices = vertexArray;
            _vertices.Resize(sizeEstimate);
            for (int idx = 0; idx < sizeEstimate; idx++)
            {
                _vertices[idx] = NO_EDGE;
            }
            _coordinates = coordinateArray;
            _coordinates.Resize(sizeEstimate);
            _edges = edgesArray;
            _edges.Resize(sizeEstimate * 3 * EDGE_SIZE);
            for (int idx = 0; idx < sizeEstimate * 3 * EDGE_SIZE; idx++)
            {
                _edges[idx] = NO_EDGE;
            }
            _edgeData = edgeDataArray;
            _edgeData.Resize(sizeEstimate * 3);
            _edgeShapes = edgeShapeArray;
            _edgeShapes.Resize(sizeEstimate * 3);
        }

        /// <summary>
        /// Increases the memory allocation.
        /// </summary>
        private void IncreaseVertexSize()
        {
            this.IncreaseVertexSize(_coordinates.Length + 10000);
        }

        /// <summary>
        /// Increases the memory allocation.
        /// </summary>
        /// <param name="size"></param>
        private void IncreaseVertexSize(long size)
        {
            var oldLength = _coordinates.Length;
            _coordinates.Resize(size);
            _vertices.Resize(size);
            for (long idx = oldLength; idx < size; idx++)
            {
                _vertices[idx] = NO_EDGE;
            }
        }

        /// <summary>
        /// Increases the memory allocation.
        /// </summary>
        private void IncreaseEdgeSize()
        {
            this.IncreaseEdgeSize(_edges.Length + 10000);
        }

        /// <summary>
        /// Increases the memory allocation.
        /// </summary>
        private void IncreaseEdgeSize(long size)
        {
            var oldLength = _edges.Length;
            _edges.Resize(size);
            for (long idx = oldLength; idx < size; idx++)
            {
                _edges[idx] = NO_EDGE;
            }
            _edgeData.Resize(size / EDGE_SIZE);
            _edgeShapes.Resize(size / EDGE_SIZE);
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
                this.IncreaseVertexSize();
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
            if (_nextVertexId <= vertex) { throw new ArgumentOutOfRangeException("vertex", "vertex is not part of this graph."); }

            var coordinate = _coordinates[vertex];
            coordinate.Latitude = latitude;
            coordinate.Longitude = longitude;
            _coordinates[vertex] = coordinate;
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
            if (_nextVertexId > id)
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
        /// <param name="coordinates"></param>
        /// <param name="comparer">Comparator to compare edges and replace obsolete ones.</param>
        /// <remarks>This only adds edge vertex1->vertex2 NOT vertex2->vertex1</remarks>
        public void AddEdge(uint vertex1, uint vertex2, TEdgeData data, ICoordinateCollection coordinates)
        {
            if (vertex1 == vertex2) { throw new ArgumentException("Given vertices must be different."); }
            if (_nextVertexId <= vertex1) { throw new ArgumentOutOfRangeException("vertex1", "vertex1 is not part of this graph."); }
            if (_nextVertexId <= vertex2) { throw new ArgumentOutOfRangeException("vertex2", "vertex2 is not part of this graph."); }

            var edgeId = _vertices[vertex1];
            if (_vertices[vertex1] != NO_EDGE)
            { // check for an existing edge first.
                // check if the arc exists already.
                uint? latestContainingEdgeId = null;
                edgeId = _vertices[vertex1];
                uint nextEdgeSlot = 0;
                while (edgeId != NO_EDGE)
                { // keep looping.
                    uint previousEdgeId = edgeId;
                    uint otherVertexId = _edges[edgeId + NODEA];
                    nextEdgeSlot = edgeId + NEXTNODEA;
                    edgeId = _edges[nextEdgeSlot];
                    if (otherVertexId == vertex2)
                    { // this is the edge we need.
                        if(_edgeData[previousEdgeId / EDGE_SIZE].Equals(data))
                        { // just update the shapes.
                            _edgeShapes[previousEdgeId / EDGE_SIZE] = coordinates;
                            return;
                        }
                        latestContainingEdgeId = previousEdgeId;
                    }
                }

                // create a new edge.
                edgeId = _nextEdgeId;
                if (_nextEdgeId + NEXTNODEA >= _edges.Length)
                { // there is a need to increase edges array.
                    this.IncreaseEdgeSize();
                }
                _edges[_nextEdgeId + NODEA] = vertex2;
                _edges[_nextEdgeId + NEXTNODEA] = NO_EDGE;

                if (latestContainingEdgeId.HasValue)
                { // the edge has been found already. insert right after the last existing one.
                    _edges[_nextEdgeId + NEXTNODEA] = _edges[latestContainingEdgeId.Value + NEXTNODEA]; // point new to next of previous.
                    _edges[latestContainingEdgeId.Value + NEXTNODEA] = _nextEdgeId + NODEA;
                }
                else
                { // just append the new edge to the from list.
                    _edges[nextEdgeSlot] = edgeId;
                }
                _nextEdgeId = _nextEdgeId + EDGE_SIZE;

                // set data.
                _edgeData[edgeId / EDGE_SIZE] = data;
                _edgeShapes[edgeId / EDGE_SIZE] = coordinates;
            }
            else
            { // create a new edge and set.
                edgeId = _nextEdgeId;
                _vertices[vertex1] = _nextEdgeId;

                if (_nextEdgeId + NEXTNODEA >= _edges.Length)
                { // there is a need to increase edges array.
                    this.IncreaseEdgeSize();
                }
                _edges[_nextEdgeId + NODEA] = vertex2;
                _edges[_nextEdgeId + NEXTNODEA] = NO_EDGE;
                _nextEdgeId = _nextEdgeId + EDGE_SIZE;

                // set data.
                _edgeData[edgeId / EDGE_SIZE] = data;
                _edgeShapes[edgeId / EDGE_SIZE] = coordinates;
            }
            return;
        }

        /// <summary>
        /// Deletes all edges leading from/to the given vertex. 
        /// </summary>
        /// <param name="vertex"></param>
        /// <remarks>Only deletes all edges vertex->* NOT *->vertex</remarks>
        public void RemoveEdges(uint vertex)
        {
            var edges = this.GetEdges(vertex);
            while (edges.MoveNext())
            {
                this.RemoveEdge(vertex, edges.Neighbour);
            }
        }

        /// <summary>
        /// Deletes the edge between the two given vertices.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <remarks>Only deletes edge vertex1->vertex2 NOT vertex2 -> vertex1.</remarks>
        public void RemoveEdge(uint vertex1, uint vertex2)
        {
            if (_nextVertexId <= vertex1) { throw new ArgumentOutOfRangeException("vertex1", "vertex1 is not part of this graph."); }
            if (_nextVertexId <= vertex2) { throw new ArgumentOutOfRangeException("vertex2", "vertex2 is not part of this graph."); }

            if (_vertices[vertex1] == NO_EDGE)
            { // no edge to remove here!
                return;
            }

            // remove for vertex1.
            var nextEdgeId = _vertices[vertex1];
            uint nextEdgeSlot = 0;
            uint previousEdgeSlot = 0;
            uint currentEdgeId = 0;
            while (nextEdgeId != NO_EDGE)
            { // keep looping.
                currentEdgeId = nextEdgeId;
                previousEdgeSlot = nextEdgeSlot;
                uint otherVertexId = _edges[nextEdgeId + NODEA];
                nextEdgeSlot = nextEdgeId + NEXTNODEA;
                nextEdgeId = _edges[nextEdgeSlot];
                if (otherVertexId == vertex2)
                { // this is the edge we need.
                    if (_vertices[vertex1] == currentEdgeId)
                    { // the edge being remove if the 'first' edge.
                        // point to the next edge.
                        _vertices[vertex1] = nextEdgeId;
                    }
                    else
                    { // the edge being removed is not the 'first' edge.
                        // set the previous edge slot to the current edge id being the next one.
                        _edges[previousEdgeSlot] = nextEdgeId;
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// Deletes the edge between the two given vertices.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="data"></param>
        /// <remarks>Only deletes edge vertex1->vertex2 NOT vertex2 -> vertex1.</remarks>
        public void RemoveEdge(uint vertex1, uint vertex2, TEdgeData data)
        {
            if (_nextVertexId <= vertex1) { throw new ArgumentOutOfRangeException("vertex1", "vertex1 is not part of this graph."); }
            if (_nextVertexId <= vertex2) { throw new ArgumentOutOfRangeException("vertex2", "vertex2 is not part of this graph."); }

            if (_vertices[vertex1] == NO_EDGE)
            { // no edge to remove here!
                return;
            }

            // remove for vertex1.
            var nextEdgeId = _vertices[vertex1];
            uint nextEdgeSlot = 0;
            uint previousEdgeSlot = 0;
            uint currentEdgeId = 0;
            while (nextEdgeId != NO_EDGE)
            { // keep looping.
                currentEdgeId = nextEdgeId;
                previousEdgeSlot = nextEdgeSlot;
                uint otherVertexId = _edges[nextEdgeId + NODEA];
                nextEdgeSlot = nextEdgeId + NEXTNODEA;
                nextEdgeId = _edges[nextEdgeSlot];
                if (otherVertexId == vertex2)
                { // this is the edge we need.
                    if (_edgeData[currentEdgeId / EDGE_SIZE].Equals(data))
                    {
                        if (_vertices[vertex1] == currentEdgeId)
                        { // the edge being remove if the 'first' edge.
                            // point to the next edge.
                            _vertices[vertex1] = nextEdgeId;
                        }
                        else
                        { // the edge being removed is not the 'first' edge.
                            // set the previous edge slot to the current edge id being the next one.
                            _edges[previousEdgeSlot] = nextEdgeId;
                        }
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Returns all arcs starting at the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public IEdgeEnumerator<TEdgeData> GetEdges(uint vertex)
        {
            if (_nextVertexId <= vertex) { throw new ArgumentOutOfRangeException("vertex", "vertex is not part of this graph."); }

            return new EdgeEnumerator(this, _vertices[vertex], vertex);
        }

        /// <summary>
        /// Returns true if the given vertex has neighbour as a neighbour.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <returns></returns>
        /// <remarks>Returns true ONLY when edge vertex1->vertex2 is there NOT when only vertex2->vertex1.</remarks>
        public bool ContainsEdges(uint vertex1, uint vertex2)
        {
            if (_nextVertexId <= vertex1) { throw new ArgumentOutOfRangeException("vertex1", "vertex1 is not part of this graph."); }
            if (_nextVertexId <= vertex2) { throw new ArgumentOutOfRangeException("vertex2", "vertex2 is not part of this graph."); }

            if (_vertices[vertex1] == NO_EDGE)
            { // no edges here!
                return false;
            }
            var edgeId = _vertices[vertex1];
            uint nextEdgeSlot = 0;
            while (edgeId != NO_EDGE)
            { // keep looping.
                uint otherVertexId = _edges[edgeId + NODEA];
                nextEdgeSlot = edgeId + NEXTNODEA;
                if (otherVertexId == vertex2)
                { // this is the edge we need.
                    return true;
                }
                edgeId = _edges[nextEdgeSlot];
            }
            return false;
        }

        /// <summary>
        /// Returns true if the given vertex has neighbour as a neighbour.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <remarks>Returns true ONLY when edge vertex1->vertex2 is there NOT when only vertex2->vertex1.</remarks>
        public bool ContainsEdge(uint vertex1, uint vertex2, TEdgeData data)
        {
            if (_nextVertexId <= vertex1) { throw new ArgumentOutOfRangeException("vertex1", "vertex1 is not part of this graph."); }
            if (_nextVertexId <= vertex2) { throw new ArgumentOutOfRangeException("vertex2", "vertex2 is not part of this graph."); }

            if (_vertices[vertex1] == NO_EDGE)
            { // no edges here!
                return false;
            }
            var edgeId = _vertices[vertex1];
            uint nextEdgeSlot = 0;
            while (edgeId != NO_EDGE)
            { // keep looping.
                uint otherVertexId = _edges[edgeId + NODEA];
                nextEdgeSlot = edgeId + NEXTNODEA;
                if (otherVertexId == vertex2)
                { // this may be the edge we need.
                    if(_edgeData[edgeId / EDGE_SIZE].Equals(data))
                    { // this is the edge we need.
                        return true;
                    }
                }
                edgeId = _edges[nextEdgeSlot];
            }
            return false;
        }

        /// <summary>
        /// Gets the index associated with the given edge and return true if it exists.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="edgeDataIdx"></param>
        /// <param name="edgeDataForward"></param>
        /// <returns></returns>
        private bool GetEdgeIdx(uint vertex1, uint vertex2, out long edgeDataIdx, out bool edgeDataForward)
        {
            if (_nextVertexId <= vertex1) { throw new ArgumentOutOfRangeException("vertex1", "vertex1 is not part of this graph."); }
            if (_nextVertexId <= vertex2) { throw new ArgumentOutOfRangeException("vertex2", "vertex2 is not part of this graph."); }

            if (_vertices[vertex1] == NO_EDGE)
            { // no edges here!
                edgeDataIdx = -1;
                edgeDataForward = false;
                return false;
            }
            var edgeId = _vertices[vertex1];
            uint nextEdgeSlot = 0;
            while (edgeId != NO_EDGE)
            { // keep looping.
                uint otherVertexId = 0;
                var currentEdgeId = edgeId;
                edgeDataForward = true;
                otherVertexId = _edges[edgeId + NODEA];
                nextEdgeSlot = edgeId + NEXTNODEA;
                if (otherVertexId == vertex2)
                { // this is the edge we need.
                    edgeDataIdx = currentEdgeId / EDGE_SIZE;
                    return true;
                }
                edgeId = _edges[nextEdgeSlot];
            }
            edgeDataForward = false;
            edgeDataIdx = -1;
            return false;
        }

        /// <summary>
        /// Gets the data associated with the given edge and return true if it exists.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public IEdgeEnumerator<TEdgeData> GetEdges(uint vertex1, uint vertex2)
        {
            if (_nextVertexId <= vertex1) { throw new ArgumentOutOfRangeException("vertex1", "vertex is not part of this graph."); }
            if (_nextVertexId <= vertex2) { throw new ArgumentOutOfRangeException("vertex2", "vertex is not part of this graph."); }

            return new EdgeEnumerator(this, _vertices[vertex1], vertex1, vertex2);
        }

        /// <summary>
        /// Gets the data associated with the given edge and return true if it exists. Throw exception if this graph allows duplicates.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool GetEdge(uint vertex1, uint vertex2, out TEdgeData data)
        {
            throw new InvalidOperationException("Cannot use GetEdge on a graph that can have duplicate edges.");
        }

        /// <summary>
        /// Gets the shape associated with the given edge and returns true if it exists.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public bool GetEdgeShape(uint vertex1, uint vertex2, out ICoordinateCollection shape)
        {
            throw new InvalidOperationException("Cannot use GetEdgeShape on a graph that can have duplicate edges.");
        }

        /// <summary>
        /// Trims the internal data structures of this graph.
        /// </summary>
        public void Trim()
        {
            // resize coordinates/vertices.
            _coordinates.Resize(_nextVertexId);
            // Array.Resize<GeoCoordinateSimple>(ref _coordinates, (int)_nextVertexId);
            _vertices.Resize(_nextVertexId);
            // Array.Resize<uint>(ref _vertices, (int)_nextVertexId);

            // resize edges.
            _edgeData.Resize(_nextEdgeId / EDGE_SIZE);
            _edgeShapes.Resize(_nextEdgeId / EDGE_SIZE);
            _edges.Resize(_nextEdgeId);
        }

        /// <summary>
        /// Resizes the internal data structures of the graph to handle the number of vertices/edges estimated.
        /// </summary>
        /// <param name="vertexEstimate"></param>
        /// <param name="edgeEstimate"></param>
        public void Resize(long vertexEstimate, long edgeEstimate)
        {
            // resize coordinates/vertices.
            this.IncreaseVertexSize((int)vertexEstimate);

            // resize edges.
            this.IncreaseEdgeSize((int)(edgeEstimate * EDGE_SIZE));
        }

        /// <summary>
        /// Returns the number of vertices in this graph.
        /// </summary>
        public uint VertexCount
        {
            get { return _nextVertexId - 1; }
        }

        /// <summary>
        /// Trims the size of this graph to it's smallest possible size.
        /// </summary>
        public void Compress()
        {
            // trim edges.
            uint maxAllocatedEdgeId = 0;
            for (uint edgeId = 0; edgeId < _nextEdgeId; edgeId = edgeId + EDGE_SIZE)
            {
                if (_edges[edgeId] != NO_EDGE)
                { // this edge is allocated.
                    if (edgeId != maxAllocatedEdgeId)
                    { // there is data here.
                        this.MoveEdge(edgeId, maxAllocatedEdgeId);
                    }
                    maxAllocatedEdgeId = maxAllocatedEdgeId + EDGE_SIZE;
                }
            }
            _nextEdgeId = maxAllocatedEdgeId;

            // trim vertices.
            uint minUnAllocatedVertexId = 0;
            for (uint vertexId = 0; vertexId < _nextVertexId; vertexId++)
            {
                if (_vertices[vertexId] != NO_EDGE)
                {
                    minUnAllocatedVertexId = vertexId;
                }
            }
            _nextVertexId = minUnAllocatedVertexId + 1;
        }

        /// <summary>
        /// Moves an edge from one location to another.
        /// </summary>
        /// <param name="oldEdgeId"></param>
        /// <param name="newEdgeId"></param>
        private void MoveEdge(uint oldEdgeId, uint newEdgeId)
        {
            // first copy the data.
            _edges[newEdgeId + NODEA] = _edges[oldEdgeId + NODEA];
            _edges[newEdgeId + NEXTNODEA] = _edges[oldEdgeId + NEXTNODEA];
            _edgeData[newEdgeId / EDGE_SIZE] = _edgeData[oldEdgeId / EDGE_SIZE];
            _edgeShapes[newEdgeId / EDGE_SIZE] = _edgeShapes[oldEdgeId / EDGE_SIZE];

            // loop over all edges of vertex1 and replace the oldEdgeId with the new one.
            uint vertex1 = _edges[oldEdgeId + NODEA];
            var edgeId = _vertices[vertex1];
            if (edgeId == oldEdgeId)
            { // edge is the first one, easy!
                _vertices[vertex1] = newEdgeId;
            }
            else
            { // edge is somewhere in the edges list.
                while (edgeId != NO_EDGE)
                { // keep looping.
                    var edgeIdLocation = edgeId + NEXTNODEA;
                    edgeId = _edges[edgeIdLocation];
                    if (edgeId == oldEdgeId)
                    {
                        _edges[edgeIdLocation] = newEdgeId;
                        break;
                    }
                }
            }

            // remove the old data.
            _edges[oldEdgeId + NODEA] = NO_EDGE;
            _edges[oldEdgeId + NEXTNODEA] = NO_EDGE;
            _edgeData[oldEdgeId / EDGE_SIZE] = default(TEdgeData);
            _edgeShapes[oldEdgeId / EDGE_SIZE] = null;
        }

        /// <summary>
        /// Represents the internal edge enumerator.
        /// </summary>
        class EdgeEnumerator : IEdgeEnumerator<TEdgeData>
        {
            /// <summary>
            /// Holds the graph.
            /// </summary>
            private MemoryDirectedGraph<TEdgeData> _graph;

            /// <summary>
            /// Holds the next edgeId.
            /// </summary>
            private uint _nextEdgeId;

            /// <summary>
            /// Holds the current edgeId.
            /// </summary>
            private uint _currentEdgeId;

            /// <summary>
            /// Holds the direction flag for the current edge.
            /// </summary>
            private bool _currentEdgeInverted = false;

            /// <summary>
            /// Holds the current vertex.
            /// </summary>
            private uint _vertex;

            /// <summary>
            /// Holds the start vertex.
            /// </summary>
            private uint _startVertex1;

            /// <summary>
            /// Holds the start vertex.
            /// </summary>
            private uint _startVertex2;

            /// <summary>
            /// Holds the start edge.
            /// </summary>
            private uint _startEdge;

            /// <summary>
            /// Creates a new edge enumerator.
            /// </summary>
            /// <param name="graph"></param>
            /// <param name="edgeId"></param>
            /// <param name="vertex"></param>
            public EdgeEnumerator(MemoryDirectedGraph<TEdgeData> graph, uint edgeId, uint vertex)
            {
                _graph = graph;
                _nextEdgeId = edgeId;
                _currentEdgeId = 0;
                _vertex = vertex;

                _startVertex1 = vertex;
                _startEdge = edgeId;
                _currentEdgeInverted = false;
            }

            /// <summary>
            /// Creates a new edge enumerator.
            /// </summary>
            /// <param name="graph"></param>
            /// <param name="edgeId"></param>
            /// <param name="vertex1"></param>
            /// <param name="vertex2"></param>
            public EdgeEnumerator(MemoryDirectedGraph<TEdgeData> graph, uint edgeId, uint vertex1, uint vertex2)
            {
                _graph = graph;
                _nextEdgeId = edgeId;
                _currentEdgeId = 0;
                _vertex = vertex1;

                _startVertex1 = vertex1;
                _startVertex2 = vertex2;
                _startEdge = edgeId;
                _currentEdgeInverted = false;
            }

            /// <summary>
            /// Move to the next edge.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if (_nextEdgeId != NO_EDGE)
                { // there is a next edge.
                    do
                    { // keep looping if there is a second vertex set.
                        _currentEdgeId = _nextEdgeId;
                        _neighbour = _graph._edges[_nextEdgeId + NODEA];
                        _nextEdgeId = _graph._edges[_nextEdgeId + NEXTNODEA];
                    } while (_nextEdgeId != NO_EDGE && _startVertex2 != 0 && _neighbour != _startVertex2);
                    return _startVertex2 == 0 || _neighbour == _startVertex2;
                }
                return false;
            }

            /// <summary>
            /// Holds the current neigbour.
            /// </summary>
            private uint _neighbour;

            /// <summary>
            /// Returns the current neighbour.
            /// </summary>
            public uint Neighbour
            {
                get { return _neighbour; }
            }

            /// <summary>
            /// Returns the current edge data.
            /// </summary>
            public TEdgeData EdgeData
            {
                get
                {
                    return _graph._edgeData[_currentEdgeId / MemoryDirectedGraph<TEdgeData>.EDGE_SIZE];
                }
            }

            /// <summary>
            /// Returns true if the edge data is inverted by default.
            /// </summary>
            public bool isInverted
            {
                get { return _currentEdgeInverted; }
            }

            /// <summary>
            /// Returns the inverted edge data.
            /// </summary>
            public TEdgeData InvertedEdgeData
            {
                get
                {
                    return (TEdgeData)_graph._edgeData[_currentEdgeId / MemoryDirectedGraph<TEdgeData>.EDGE_SIZE].Reverse();
                }
            }

            /// <summary>
            /// Returns the current intermediates.
            /// </summary>
            public ICoordinateCollection Intermediates
            {

                get
                {
                    if (_currentEdgeInverted)
                    {
                        var intermediates = _graph._edgeShapes[_currentEdgeId / MemoryDirectedGraph<TEdgeData>.EDGE_SIZE];
                        if (intermediates != null)
                        {
                            return intermediates.Reverse();
                        }
                        return null;
                    }
                    return _graph._edgeShapes[_currentEdgeId / MemoryDirectedGraph<TEdgeData>.EDGE_SIZE];
                }
            }

            /// <summary>
            /// Returns the count.
            /// </summary>
            /// <returns></returns>
            public int Count()
            {
                int count = 0;
                while (this.MoveNext())
                {
                    count++;
                }
                return count;
            }

            /// <summary>
            /// Resets this enumerator.
            /// </summary>
            public void Reset()
            {
                _nextEdgeId = _startEdge;
                _currentEdgeId = 0;
                _vertex = _startVertex1;
            }

            public IEnumerator<Edge<TEdgeData>> GetEnumerator()
            {
                this.Reset();
                return this;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                this.Reset();
                return this;
            }

            public Edge<TEdgeData> Current
            {
                get { return new Edge<TEdgeData>(this); }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return new Edge<TEdgeData>(this); }
            }

            public void Dispose()
            {

            }
        }

        public bool IsDirected
        {
            get { return true; }
        }

        public bool CanHaveDuplicates
        {
            get { return true; }
        }
    }
}