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

using OsmSharp.Collections.Arrays;
using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Math.Geo.Simple;
using System;

namespace OsmSharp.Routing.Graph
{
    /// <summary>
    /// An implementation of an in-memory dynamic graph using memory mapped files to handle huge graphs.
    /// </summary>
    /// <typeparam name="TEdgeData"></typeparam>
    public class MemoryMappedGraph<TEdgeData> : MemoryGraph<TEdgeData>, IDisposable
        where TEdgeData : struct, IGraphEdgeData
    {
        /// <summary>
        /// Holds the coordinates array.
        /// </summary>
        private MemoryMappedHugeArray<GeoCoordinateSimple> _coordinates;

        /// <summary>
        /// Holds the vertex array.
        /// </summary>
        private MemoryMappedHugeArray<uint> _vertices;

        /// <summary>
        /// Holds the edges array.
        /// </summary>
        private MemoryMappedHugeArray<uint> _edges;

        /// <summary>
        /// Holds the edge data array.
        /// </summary>
        private MemoryMappedHugeArray<TEdgeData> _edgeData;

        /// <summary>
        /// Holds the shapes index.
        /// </summary>
        private HugeCoordinateCollectionIndex _shapes;

        /// <summary>
        /// Creates a new memory mapped file dynamic graph.
        /// </summary>
        /// <param name="estimatedSize"></param>
        public MemoryMappedGraph(long estimatedSize)
            : this(estimatedSize,
            new MemoryMappedHugeArray<GeoCoordinateSimple>(estimatedSize),
            new MemoryMappedHugeArray<uint>(estimatedSize),
            new MemoryMappedHugeArray<uint>(estimatedSize),
            new MemoryMappedHugeArray<TEdgeData>(estimatedSize),
            new HugeCoordinateCollectionIndex(estimatedSize))
        {

        }

        /// <summary>
        /// Creates a new memory mapped file dynamic graph.
        /// </summary>
        /// <param name="estimatedSize"></param>
        /// <param name="arraySize"></param>
        public MemoryMappedGraph(long estimatedSize, int arraySize)
            : this(estimatedSize,
            new MemoryMappedHugeArray<GeoCoordinateSimple>(estimatedSize, arraySize),
            new MemoryMappedHugeArray<uint>(estimatedSize, arraySize),
            new MemoryMappedHugeArray<uint>(estimatedSize, arraySize),
            new MemoryMappedHugeArray<TEdgeData>(estimatedSize, arraySize),
            new HugeCoordinateCollectionIndex(estimatedSize))
        {

        }

        /// <summary>
        /// Creates a new memory mapped file dynamic graph.
        /// </summary>
        /// <param name="estimatedSize"></param>
        /// <param name="factory"></param>
        public MemoryMappedGraph(long estimatedSize, MemoryMappedFileFactory factory)
            : this(estimatedSize,
            new MemoryMappedHugeArray<GeoCoordinateSimple>(factory, estimatedSize),
            new MemoryMappedHugeArray<uint>(factory, estimatedSize),
            new MemoryMappedHugeArray<uint>(factory, estimatedSize),
            new MemoryMappedHugeArray<TEdgeData>(factory, estimatedSize),
            new HugeCoordinateCollectionIndex(factory, estimatedSize))
        {

        }

        /// <summary>
        /// Creates a new memory mapped file dynamic graph.
        /// </summary>
        /// <param name="estimatedSize"></param>
        /// <param name="factory"></param>
        /// <param name="arraySize"></param>
        public MemoryMappedGraph(long estimatedSize, int arraySize, MemoryMappedFileFactory factory)
            : this(estimatedSize,
            new MemoryMappedHugeArray<GeoCoordinateSimple>(factory, estimatedSize, arraySize),
            new MemoryMappedHugeArray<uint>(factory, estimatedSize, arraySize),
            new MemoryMappedHugeArray<uint>(factory, estimatedSize, arraySize),
            new MemoryMappedHugeArray<TEdgeData>(factory, estimatedSize, arraySize),
            new HugeCoordinateCollectionIndex(factory, estimatedSize))
        {

        }

        /// <summary>
        /// Creates a new memory mapped file dynamic graph.
        /// </summary>
        /// <param name="estimatedSize"></param>
        /// <param name="coordinates"></param>
        /// <param name="vertices"></param>
        /// <param name="edges"></param>
        /// <param name="edgeData"></param>
        /// <param name="edgeShapes"></param>
        public MemoryMappedGraph(long estimatedSize,
            MemoryMappedHugeArray<GeoCoordinateSimple> coordinates,
            MemoryMappedHugeArray<uint> vertices,
            MemoryMappedHugeArray<uint> edges,
            MemoryMappedHugeArray<TEdgeData> edgeData,
            HugeCoordinateCollectionIndex edgeShapes)
            : base(estimatedSize, coordinates, vertices, edges, edgeData, edgeShapes)
        {
            _coordinates = coordinates;
            _vertices = vertices;
            _edges = edges;
            _edgeData = edgeData;
            _shapes = edgeShapes;
        }

        /// <summary>
        /// Disposes of all native resources associated with this memory dynamic graph.
        /// </summary>
        public void Dispose()
        {
            _coordinates.Dispose();
            _coordinates = null;
            _edges.Dispose();
            _edges = null;
            _edgeData.Dispose();
            _edgeData = null;
            _vertices.Dispose();
            _vertices = null;
            _shapes.Dispose();
            _shapes = null;
        }
    }
}