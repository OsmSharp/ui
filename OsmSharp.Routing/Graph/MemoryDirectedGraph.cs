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
        protected const int VERTEX_SIZE = 2; // holds the first edge index and the edge count.
        protected const int FIRST_EDGE = 0;
        protected const int EDGE_COUNT = 1;
        protected const int EDGE_SIZE = 1; // holds only the target vertext.
        protected const uint NO_EDGE = uint.MaxValue; // a dummy value indication that there is no edge.

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
        private HugeArrayBase<GeoCoordinateSimple> _coordinates;

        /// <summary>
        /// Holds all vertices pointing to it's first edge.
        /// </summary>
        private HugeArrayBase<uint> _vertices;

        /// <summary>
        /// Holds all edges (meaning vertex1-vertex2)
        /// </summary>
        private HugeArrayBase<uint> _edges;

        /// <summary>
        /// Holds all data associated with edges.
        /// </summary>
        private HugeArrayBase<TEdgeData> _edgeData;

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
            : this(sizeEstimate, new HugeArray<GeoCoordinateSimple>(sizeEstimate), new HugeArray<uint>(sizeEstimate), 
                    new HugeArray<uint>(sizeEstimate * 3 * EDGE_SIZE), new HugeArray<TEdgeData>(sizeEstimate * 3), new HugeCoordinateCollectionIndex(sizeEstimate * 3))
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
        protected MemoryDirectedGraph(long sizeEstimate, HugeArrayBase<GeoCoordinateSimple> coordinateArray, 
            HugeArrayBase<uint> vertexArray, HugeArrayBase<uint> edgesArray, HugeArrayBase<TEdgeData> edgeDataArray, HugeCoordinateCollectionIndex edgeShapeArray)
        {
            _nextVertexId = 1;
            _nextEdgeId = 0;
            _vertices = vertexArray;
            _vertices.Resize(sizeEstimate);
            _coordinates = coordinateArray;
            _coordinates.Resize(sizeEstimate);
            _edges = edgesArray;
            _edges.Resize(sizeEstimate * 3 * EDGE_SIZE);
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
            // create vertex.
            var newId = _nextVertexId;
            var vertexIdx = newId * VERTEX_SIZE;
            if (vertexIdx + 1 >= _vertices.Length)
            {
                this.IncreaseVertexSize();
            }

            _coordinates[newId] = new GeoCoordinateSimple()
            {
                Latitude = latitude,
                Longitude = longitude
            };
            // _vertices[vertexIdx + FIRST_EDGE] = NO_EDGE;
            _vertices[vertexIdx + EDGE_COUNT] = 0;
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
        /// <remarks>This only adds edge vertex1->vertex2 NOT vertex2->vertex1</remarks>
        public void AddEdge(uint vertex1, uint vertex2, TEdgeData data, ICoordinateCollection coordinates)
        {
            if (vertex1 == vertex2) { throw new ArgumentException("Given vertices must be different."); }
            if (_nextVertexId <= vertex1) { throw new ArgumentOutOfRangeException("vertex1", "vertex1 is not part of this graph."); }
            if (_nextVertexId <= vertex2) { throw new ArgumentOutOfRangeException("vertex2", "vertex2 is not part of this graph."); }

            var vertex1Idx = vertex1 * VERTEX_SIZE;
            var edgeCount = _vertices[vertex1Idx + EDGE_COUNT];
            var edgeId = _vertices[vertex1Idx + FIRST_EDGE];

            if ((edgeCount & (edgeCount - 1)) == 0)
            { // edgeCount is a power of two, increase space.
                // update vertex.
                uint newEdgeId = _nextEdgeId;
                _vertices[vertex1Idx + FIRST_EDGE] = newEdgeId;

                // move edges.
                if (edgeCount > 0)
                {
                    if (newEdgeId + (2 * edgeCount) >= _edges.Length)
                    { // edges need to be increased.
                        this.IncreaseEdgeSize();
                    }

                    for (uint toMoveIdx = edgeId; toMoveIdx < edgeId + edgeCount; toMoveIdx = toMoveIdx + EDGE_SIZE)
                    {
                        _edges[newEdgeId] = _edges[toMoveIdx];
                        _edgeData[newEdgeId] = _edgeData[toMoveIdx];
                        _edgeShapes[newEdgeId] = _edgeShapes[toMoveIdx];

                        newEdgeId++;
                    }

                    // the edge id is the last new edge id.
                    edgeId = newEdgeId;

                    // increase the nextEdgeId, these edges have been added at the end of the edge-array.
                    _nextEdgeId = _nextEdgeId + (2 * edgeCount);
                }
                else
                { // just add next edge id.
                    if (_nextEdgeId + 1 >= _edges.Length)
                    { // edges need to be increased.
                        this.IncreaseEdgeSize();
                    }

                    edgeId = _nextEdgeId;
                    _nextEdgeId++;
                }
            }
            else
            { // calculate edgeId of new edge.
                if (_nextEdgeId + 1 >= _edges.Length)
                { // edges need to be increased.
                    this.IncreaseEdgeSize();
                }

                edgeId = edgeId + edgeCount;
                _nextEdgeId++;
            }

            // update edge count in vertex.
            edgeCount++;
            _vertices[vertex1Idx + EDGE_COUNT] = edgeCount;

            // update edge.
            _edges[edgeId] = vertex2;
            _edgeData[edgeId] = data;
            _edgeShapes[edgeId] = coordinates;
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

            var vertex1Idx = vertex1 * VERTEX_SIZE;
            var edgeCount = _vertices[vertex1Idx + EDGE_COUNT];
            var edgeId = _vertices[vertex1Idx + FIRST_EDGE];

            for(var removeIdx = edgeId; removeIdx < edgeId + edgeCount; removeIdx++)
            {
                if(_edges[removeIdx] == vertex2)
                {
                    edgeCount--;
                    _edges[removeIdx] = _edges[edgeId + edgeCount];
                    _edgeData[removeIdx] = _edgeData[edgeId + edgeCount];
                    _edgeShapes[removeIdx] = _edgeShapes[edgeId + edgeCount];
                }
            }
            _vertices[vertex1Idx + EDGE_COUNT] = edgeCount;
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

            var vertex1Idx = vertex1 * VERTEX_SIZE;
            var edgeCount = _vertices[vertex1Idx + EDGE_COUNT];
            var edgeId = _vertices[vertex1Idx + FIRST_EDGE];

            bool removed = false;
            for (var removeIdx = edgeId; removeIdx < edgeId + edgeCount; removeIdx++)
            {
                if (_edges[removeIdx] == vertex2 &&
                    _edgeData[removeIdx].Equals(data))
                {
                    edgeCount--;
                    _edges[removeIdx] = _edges[edgeId + edgeCount];
                    _edgeData[removeIdx] = _edgeData[edgeId + edgeCount];
                    _edgeShapes[removeIdx] = _edgeShapes[edgeId + edgeCount];
                    removed = true;
                }
            }
            _vertices[vertex1Idx + EDGE_COUNT] = edgeCount;

            if(!removed)
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Returns an empty edge enumerator.
        /// </summary>
        /// <returns></returns>
        public EdgeEnumerator<TEdgeData> GetEdgeEnumerator()
        {
            return new EdgeEnumerator(this);
        }

        /// <summary>
        /// Returns all arcs starting at the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public EdgeEnumerator<TEdgeData> GetEdges(uint vertex)
        {
            if (_nextVertexId <= vertex) { throw new ArgumentOutOfRangeException("vertex", "vertex is not part of this graph."); }

            var enumerator = new EdgeEnumerator(this);
            enumerator.MoveTo(vertex);
            return enumerator;
        }

        /// <summary>
        /// Gets the data associated with the given edge and return true if it exists.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <returns></returns>
        public EdgeEnumerator<TEdgeData> GetEdges(uint vertex1, uint vertex2)
        {
            if (_nextVertexId <= vertex1) { throw new ArgumentOutOfRangeException("vertex1", "vertex is not part of this graph."); }
            if (_nextVertexId <= vertex2) { throw new ArgumentOutOfRangeException("vertex2", "vertex is not part of this graph."); }

            var enumerator = new EdgeEnumerator(this);
            enumerator.MoveTo(vertex1, vertex2);
            return enumerator;
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

            var vertex1Idx = vertex1 * VERTEX_SIZE;
            var edgeCount = _vertices[vertex1Idx + EDGE_COUNT];
            var edgeId = _vertices[vertex1Idx + FIRST_EDGE];

            for (var searchIdx = edgeId; searchIdx < edgeId + edgeCount; searchIdx++)
            {
                if (_edges[searchIdx] == vertex2)
                {
                    return true;
                }
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

            var vertex1Idx = vertex1 * VERTEX_SIZE;
            var edgeCount = _vertices[vertex1Idx + EDGE_COUNT];
            var edgeId = _vertices[vertex1Idx + FIRST_EDGE];

            for (var searchIdx = edgeId; searchIdx < edgeId + edgeCount; searchIdx++)
            {
                if (_edges[searchIdx] == vertex2 &&
                    _edgeData[searchIdx].Equals(data))
                {
                    return true;
                }
            }
            return false;
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
            _coordinates.Resize(_nextVertexId * VERTEX_SIZE);
            _vertices.Resize(_nextVertexId * VERTEX_SIZE);

            // resize edges.
            _edgeData.Resize(_nextEdgeId / EDGE_SIZE);
            _edgeShapes.Resize(_nextEdgeId / EDGE_SIZE);
            _edges.Resize(_nextEdgeId * EDGE_SIZE);
        }

        /// <summary>
        /// Resizes the internal data structures of the graph to handle the number of vertices/edges estimated.
        /// </summary>
        /// <param name="vertexEstimate"></param>
        /// <param name="edgeEstimate"></param>
        public void Resize(long vertexEstimate, long edgeEstimate)
        {
            // resize coordinates/vertices.
            this.IncreaseVertexSize((int)vertexEstimate * VERTEX_SIZE);

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

        }

        /// <summary>
        /// Represents the internal edge enumerator.
        /// </summary>
        class EdgeEnumerator : EdgeEnumerator<TEdgeData>
        {
            /// <summary>
            /// Holds the graph.
            /// </summary>
            private MemoryDirectedGraph<TEdgeData> _graph;

            /// <summary>
            /// Holds the current edge id.
            /// </summary>
            private uint _currentEdgeId;

            /// <summary>
            /// Holds the current count (for performance reasons, this is duplicate information).
            /// </summary>
            private int _currentCount;

            /// <summary>
            /// Holds the start edge id.
            /// </summary>
            private uint _startEdgeId;

            /// <summary>
            /// Holds the edge count.
            /// </summary>
            private uint _count;

            /// <summary>
            /// Holds the neighbour.
            /// </summary>
            private uint _neighbour;

            /// <summary>
            /// Creates a new edge enumerator.
            /// </summary>
            /// <param name="graph"></param>
            public EdgeEnumerator(MemoryDirectedGraph<TEdgeData> graph)
            {
                _graph = graph;
                _startEdgeId = 0;
                _count = 0;
                _neighbour = 0;

                // reset.
                _currentEdgeId = uint.MaxValue;
                _currentCount = -1;
            }

            /// <summary>
            /// Move to the next edge.
            /// </summary>
            /// <returns></returns>
            public override bool MoveNext()
            {
                if(_currentCount < 0)
                {
                    _currentEdgeId = _startEdgeId;
                    _currentCount = 0;
                }
                else
                {
                    _currentEdgeId++;
                    _currentCount++;
                }
                if (_currentCount < _count)
                {
                    while (_neighbour != 0 &&
                        _neighbour != this.Neighbour)
                    {
                        _currentEdgeId++;
                        _currentCount++;

                        if(_currentCount >= _count)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Returns the current neighbour.
            /// </summary>
            public override uint Neighbour
            {
                get { return _graph._edges[_currentEdgeId]; }
            }

            /// <summary>
            /// Returns the current edge data.
            /// </summary>
            public override TEdgeData EdgeData
            {
                get
                {
                    return _graph._edgeData[_currentEdgeId];
                }
            }

            /// <summary>
            /// Returns true if the edge data is inverted by default.
            /// </summary>
            public override bool isInverted
            {
                get { return false; }
            }

            /// <summary>
            /// Returns the inverted edge data.
            /// </summary>
            public override TEdgeData InvertedEdgeData
            {
                get
                {
                    return (TEdgeData)_graph._edgeData[_currentEdgeId].Reverse();
                }
            }

            /// <summary>
            /// Returns the current intermediates.
            /// </summary>
            public override ICoordinateCollection Intermediates
            {

                get
                {
                    return _graph._edgeShapes[_currentEdgeId];
                }
            }

            /// <summary>
            /// Resets this enumerator.
            /// </summary>
            public override void Reset()
            {
                _currentEdgeId = uint.MaxValue;
                _currentCount = -1;
            }

            public override IEnumerator<Edge<TEdgeData>> GetEnumerator()
            {
                this.Reset();
                return this;
            }

            public override Edge<TEdgeData> Current
            {
                get { return new Edge<TEdgeData>(this); }
            }

            public override void Dispose()
            {

            }


            public override bool HasCount
            {
                get { return _neighbour == 0; }
            }

            public override int Count
            {
                get { return (int)_count; }
            }

            /// <summary>
            /// Moves this enumerator to the given vertex.
            /// </summary>
            /// <param name="vertex"></param>
            public override void MoveTo(uint vertex)
            {
                 var vertexId = vertex * VERTEX_SIZE;
                 _startEdgeId = _graph._vertices[vertexId + FIRST_EDGE];
                 _count = _graph._vertices[vertexId + EDGE_COUNT];
                _neighbour = 0;

                // reset.
                _currentEdgeId = uint.MaxValue;
                _currentCount = -1;
            }

            /// <summary>
            /// Moves this enumerator to the given vertex with the given neighbour.
            /// </summary>
            /// <param name="vertex1"></param>
            /// <param name="vertex2"></param>
            public override void MoveTo(uint vertex1, uint vertex2)
            {
                var vertexId = vertex1 * VERTEX_SIZE;
                _startEdgeId = _graph._vertices[vertexId + FIRST_EDGE];
                _count = _graph._vertices[vertexId + EDGE_COUNT];
                _neighbour = vertex2;

                // reset.
                _currentEdgeId = uint.MaxValue;
                _currentCount = -1;
            }
        }

        /// <summary>
        /// Returns true if this graph is directed.
        /// </summary>
        public bool IsDirected
        {
            get { return true; }
        }

        /// <summary>
        /// Returns true if this graph can have duplicate edges.
        /// </summary>
        public bool CanHaveDuplicates
        {
            get { return true; }
        }
    }
}