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

using OsmSharp.Collections.Cache;
using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Simple;
using OsmSharp.Osm.Tiles;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Routing.CH.Serialization.Sorted
{
    /// <summary>
    /// A basic router datasource.
    /// </summary>
    internal class CHEdgeDataDataSource : IBasicRouterDataSource<CHEdgeData>
    {
        /// <summary>
        /// Holds the tags index.
        /// </summary>
        private ITagsCollectionIndexReadonly _tagsIndex;

        /// <summary>
        /// Holds the stream.
        /// </summary>
        private Stream _stream;

        /// <summary>
        /// Holds the serializer.
        /// </summary>
        private CHEdgeDataDataSourceSerializer _serializer;

        /// <summary>
        /// Holds the supported vehicles.
        /// </summary>
        private readonly HashSet<string> _vehicles;

        /// <summary>
        /// Creates a new CH edge data source.
        /// </summary>
        public CHEdgeDataDataSource(Stream stream, CHEdgeDataDataSourceSerializer serializer, IEnumerable<string> vehicles,
            int startOfRegions, CHVertexRegionIndex regionIndex, int zoom,
            int startOfBlocks, CHBlockIndex blockIndex, uint blockSize,
            int startOfShapes, CHBlockIndex shapeIndex,
            ITagsCollectionIndexReadonly tagsIndex)
        {
            _stream = stream;
            _serializer = serializer;
            _vehicles = new HashSet<string>(vehicles);

            this.InitializeRegions(startOfRegions, regionIndex, zoom);
            this.InitializeBlocks(startOfBlocks, blockIndex, blockSize);
            this.InitializeShapes(startOfShapes, shapeIndex);

            _blocks = new LRUCache<uint, CHBlock>(5000);
            _blockShapes = new LRUCache<uint, CHBlockCoordinates>(1000);
            _regions = new LRUCache<ulong, CHVertexRegion>(1000);
            _tagsIndex = tagsIndex;
        }

        /// <summary>
        /// Returns true if the given profile is supported.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public bool SupportsProfile(Vehicle vehicle)
        {
            return _vehicles.Contains(vehicle.UniqueName);
        }

        /// <summary>
        /// Returns true if the given profile is supported.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        public void AddSupportedProfile(Vehicle vehicle)
        {
            throw new InvalidOperationException("Cannot add extra vehicle profiles to a read-only source.");
        }

        /// <summary>
        /// Returns all edges inside the given boundingbox.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public INeighbourEnumerator<CHEdgeData> GetEdges(GeoCoordinateBox box)
        {
            // get all the vertices in the given box.
            var vertices = this.LoadVerticesIn(box);

            // loop over all vertices and get the arcs.
            var neighbours = new List<Tuple<uint, uint, uint, uint, CHEdgeData>>();
            foreach (uint vertexId in vertices)
            {
                var arcs = this.LoadArcs(vertexId);
                for (int arcIdx = 0; arcIdx < arcs.Length; arcIdx++)
                {
                    neighbours.Add(new Tuple<uint, uint, uint, uint, CHEdgeData>(arcs[arcIdx].Item1, arcs[arcIdx].Item2,
                        vertexId, arcs[arcIdx].Item3, arcs[arcIdx].Item4));
                }
            }
            return new NeighbourEnumerator(this, neighbours);
        }

        /// <summary>
        /// Returns the tags index.
        /// </summary>
        public ITagsCollectionIndexReadonly TagsIndex
        {
            get { return _tagsIndex; }
        }

        /// <summary>
        /// Returns the location of the vertex with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public bool GetVertex(uint id, out float latitude, out float longitude)
        {
            return this.LoadVertex(id, out latitude, out longitude);
        }

        /// <summary>
        /// Returns all vertices in this router data source.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<uint> GetVertices()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns an enumerator for edges for the given vertex.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        public IEdgeEnumerator<CHEdgeData> GetEdges(uint vertexId)
        {
            return new EdgeEnumerator(this, this.GetEdgePairs(vertexId));
        }

        /// <summary>
        /// Returns true if the given vertex has the given neighbour.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <param name="neighbour"></param>
        /// <returns></returns>
        public bool ContainsEdges(uint vertexId, uint neighbour)
        {
            return this.GetEdges(vertexId, neighbour).MoveNext();
        }

        /// <summary>
        /// Returns true if the given vertex has the given neighbour.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <returns></returns>
        public IEdgeEnumerator<CHEdgeData> GetEdges(uint vertex1, uint vertex2)
        {
            return new EdgeEnumerator(this, this.GetEdgePairs(vertex1, vertex2));
        }

        /// <summary>
        /// Returns true if the given vertex has the given neighbour.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        public bool GetEdgeShape(uint vertex1, uint vertex2, out ICoordinateCollection shape)
        {
            return this.LoadArcShape(vertex1, vertex2, out shape);
        }

        /// <summary>
        /// Returns the vertex count.
        /// </summary>
        public uint VertexCount
        {
            get
            { // calculate the number of vertices from the block-count and the content of the last block.
                var lastBlockId = (uint)(_blocksIndex.BlockLocationIndex.Length - 1);
                var count = lastBlockId * _blockSize;
                CHBlock block;
                if (!_blocks.TryGet(lastBlockId, out block))
                { // damn block not cached!
                    block = this.DeserializeBlock(lastBlockId);
                    if (block == null)
                    { // oops even now the block is not found!
                        throw new Exception("Last block not found for vertex count calculation!");
                    }
                    _blocks.Add(lastBlockId, block);
                }
                return (uint)(count + block.Vertices.Length);
            }
        }

        /// <summary>
        /// Represents a part of a stream.
        /// </summary>
        private class StreamPart
        {
            /// <summary>
            /// Gets/sets the offset.
            /// </summary>
            public long Offset { get; set; }

            /// <summary>
            /// Gets/sets the length.
            /// </summary>
            public int Length { get; set; }
        }

        /// <summary>
        /// Returns all arcs for the given vertices.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        private Tuple<uint, uint, uint, CHEdgeData>[] GetEdgePairs(uint vertex1, uint vertex2)
        {
            var arcs = this.LoadArcs(vertex1);
            var selectedArcs = new List<Tuple<uint, uint, uint, CHEdgeData>>(arcs.Length);
            for(int idx = 0; idx < arcs.Length; idx++)
            {
                if(arcs[idx].Item3 == vertex2)
                {
                    selectedArcs.Add(arcs[idx]);
                }
            }
            return selectedArcs.ToArray();
        }


        /// <summary>
        /// Returns all arcs for the given vertex.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        private Tuple<uint, uint, uint, CHEdgeData>[] GetEdgePairs(uint vertexId)
        {
            return this.LoadArcs(vertexId);
        }

        #region Regions

        /// <summary>
        /// The region zoom size.
        /// </summary>
        private int _zoom;

        /// <summary>
        /// Holds the regions.
        /// </summary>
        private LRUCache<ulong, CHVertexRegion> _regions;

        /// <summary>
        /// Holds the region stream parts.
        /// </summary>
        private Dictionary<ulong, StreamPart> _regionStreamParts;

        /// <summary>
        /// Initializes all region stuff.
        /// </summary>
        /// <param name="startOfRegions"></param>
        /// <param name="regionIndex"></param>
        /// <param name="zoom"></param>
        private void InitializeRegions(int startOfRegions, CHVertexRegionIndex regionIndex, int zoom)
        {
            _zoom = zoom;
            _regionStreamParts = new Dictionary<ulong, StreamPart>();

            for (int idx = 0; idx < regionIndex.LocationIndex.Length; idx++)
            {
                StreamPart streamPart = new StreamPart();
                if (idx == 0)
                { // start is at startOfRegions.
                    streamPart.Offset = startOfRegions;
                    streamPart.Length = regionIndex.LocationIndex[0];
                }
                else
                { // start is at startOfRegions + location end of previous block.
                    streamPart.Offset = startOfRegions + regionIndex.LocationIndex[idx - 1];
                    streamPart.Length = regionIndex.LocationIndex[idx] - regionIndex.LocationIndex[idx - 1];
                }
                _regionStreamParts.Add(regionIndex.RegionIds[idx],
                    streamPart);
            }
        }

        /// <summary>
        /// Loads all vertices inside the given boundingbox.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        private HashSet<uint> LoadVerticesIn(GeoCoordinateBox box)
        {
            var vertices = new HashSet<uint>();
            var range = TileRange.CreateAroundBoundingBox(box, _zoom);
            foreach (Tile tile in range)
            {
                CHVertexRegion region;
                if (!_regions.TryGet(tile.Id, out region))
                {
                    region = this.DeserializeRegion(tile.Id);
                    if (region != null)
                    {
                        _regions.Add(tile.Id, region);
                    }
                }
                if (region != null)
                {
                    for (int idx = 0; idx < region.Vertices.Length; idx++)
                    {
                        vertices.Add(region.Vertices[idx]);
                    }
                }
            }
            return vertices;
        }

        /// <summary>
        /// Deserializes a region with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private CHVertexRegion DeserializeRegion(ulong id)
        {
            StreamPart part;
            if (_regionStreamParts.TryGetValue(id, out part))
            {
                return _serializer.DeserializeRegion(_stream, part.Offset, part.Length, false);
            }
            return null;
        }

        #endregion

        #region Blocks

        /// <summary>
        /// Holds the blocksize.
        /// </summary>
        private uint _blockSize;

        /// <summary>
        /// Holds the cached blocks.
        /// </summary>
        private LRUCache<uint, CHBlock> _blocks;

        /// <summary>
        /// Holds the cached block shapes.
        /// </summary>
        private LRUCache<uint, CHBlockCoordinates> _blockShapes;

        /// <summary>
        /// Holds the start-position of the blocks.
        /// </summary>
        private int _startOfBlocks;

        /// <summary>
        /// Holds the blocks index.
        /// </summary>
        private CHBlockIndex _blocksIndex;

        /// <summary>
        /// Initializes the blocks stuff.
        /// </summary>
        /// <param name="startOfBlocks"></param>
        /// <param name="blocksIndex"></param>
        /// <param name="blockSize"></param>
        private void InitializeBlocks(int startOfBlocks, CHBlockIndex blocksIndex, uint blockSize)
        {
            _startOfBlocks = startOfBlocks;
            _blocksIndex = blocksIndex;
            _blockSize = blockSize;
        }

        /// <summary>
        /// Holds the start-position of the shapes.
        /// </summary>
        private int _startOfShapes;

        /// <summary>
        /// Holds the shapes index.
        /// </summary>
        private CHBlockIndex _shapesIndex;

        /// <summary>
        /// Holds the shapes index.
        /// </summary>
        private uint _shapesSize;

        /// <summary>
        /// Initializes the shapes stuff.
        /// </summary>
        /// <param name="startOfShapes"></param>
        /// <param name="shapesIndex"></param>
        private void InitializeShapes(int startOfShapes, CHBlockIndex shapesIndex)
        {
            _startOfShapes = startOfShapes;
            _shapesIndex = shapesIndex;
        }

        /// <summary>
        /// Loads a vertex and returns true if found.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        private bool LoadVertex(uint vertexId, out float latitude, out float longitude)
        {
            uint blockId = CHBlock.CalculateId(vertexId, _blockSize);
            CHBlock block;
            if (!_blocks.TryGet(blockId, out block))
            { // damn block not cached!
                block = this.DeserializeBlock(blockId);
                if (block == null)
                { // oops even now the block is not found!
                    longitude = 0;
                    latitude = 0;
                    return false;
                }
                _blocks.Add(blockId, block);
            }
            uint blockIdx = vertexId - blockId;
            if (block.Vertices != null &&
                blockIdx < block.Vertices.Length)
            { // block is found and the vertex is there!
                latitude = block.Vertices[blockIdx].Latitude;
                longitude = block.Vertices[blockIdx].Longitude;
                return true;
            }
            // oops even now the block is not found!
            longitude = 0;
            latitude = 0;
            return false;
        }

        /// <summary>
        /// Loads the edge between the given vertices.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool LoadArc(uint vertex1, uint vertex2, out CHEdgeData data)
        {
            uint blockId = CHBlock.CalculateId(vertex1, _blockSize);
            CHBlock block;
            if (!_blocks.TryGet(blockId, out block))
            { // damn block not cached!
                block = this.DeserializeBlock(blockId);
                if (block == null)
                { // oops even now the block is not found!
                    data = new CHEdgeData();
                    return false;
                }
                _blocks.Add(blockId, block);
            }
            uint blockIdx = vertex1 - blockId;
            if (block.Vertices != null &&
                blockIdx < block.Vertices.Length)
            { // block is found and the vertex is there!
                for (int arcIdx = block.Vertices[blockIdx].ArcIndex;
                    arcIdx < block.Vertices[blockIdx].ArcIndex + block.Vertices[blockIdx].ArcCount; arcIdx++)
                { // loop over all arcs.
                    var chArc = block.Arcs[arcIdx];
                    if (chArc.TargetId == vertex2)
                    {
                        data = new CHEdgeData(chArc.Value, chArc.Weight, chArc.Meta);
                        return true;
                    }
                }
            }
            blockId = CHBlock.CalculateId(vertex2, _blockSize);
            if (!_blocks.TryGet(blockId, out block))
            { // damn block not cached!
                block = this.DeserializeBlock(blockId);
                if (block == null)
                { // oops even now the block is not found!
                    data = new CHEdgeData();
                    return false;
                }
                _blocks.Add(blockId, block);
            }
            blockIdx = vertex2 - blockId;
            if (block.Vertices != null &&
                blockIdx < block.Vertices.Length)
            { // block is found and the vertex is there!
                for (int arcIdx = block.Vertices[blockIdx].ArcIndex;
                    arcIdx < block.Vertices[blockIdx].ArcIndex + block.Vertices[blockIdx].ArcCount; arcIdx++)
                { // loop over all arcs.
                    var chArc = block.Arcs[arcIdx];
                    if (chArc.TargetId == vertex1)
                    {
                        data = new CHEdgeData(chArc.Value, chArc.Weight, chArc.Meta);
                        return true;
                    }
                }
            }
            data = new CHEdgeData();
            return false;
        }

        /// <summary>
        /// Loads the edge between the given vertices.
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="shape"></param>
        /// <returns></returns>
        private bool LoadArcShape(uint vertex1, uint vertex2, out ICoordinateCollection shape)
        {
            uint blockId = CHBlock.CalculateId(vertex1, _blockSize);
            CHBlockCoordinates blockCoordinates;
            if (!_blockShapes.TryGet(blockId, out blockCoordinates))
            { // damn block not cached!
                blockCoordinates = this.DeserializeShape(blockId);
                if (blockCoordinates == null)
                { // oops even now the block is not found!
                    shape = null;
                    return false;
                }
                _blockShapes.Add(blockId, blockCoordinates);
            }
            CHBlock block;
            if (!_blocks.TryGet(blockId, out block))
            { // damn block not cached!
                block = this.DeserializeBlock(blockId);
                if (block == null)
                { // oops even now the block is not found!
                    shape = null;
                    return false;
                }
                _blocks.Add(blockId, block);
            }
            uint blockIdx = vertex1 - blockId;
            if (block.Vertices != null &&
                blockIdx < block.Vertices.Length)
            { // block is found and the vertex is there!
                for (int arcIdx = block.Vertices[blockIdx].ArcIndex;
                    arcIdx < block.Vertices[blockIdx].ArcIndex + block.Vertices[blockIdx].ArcCount; arcIdx++)
                { // loop over all arcs.
                    var chArc = block.Arcs[arcIdx];
                    if (chArc.TargetId == vertex2)
                    {
                        var arcCoordinates = blockCoordinates.Arcs[arcIdx];
                        shape = null;
                        if (arcCoordinates.Coordinates != null)
                        {
                            shape = new CoordinateArrayCollection<GeoCoordinateSimple>(arcCoordinates.Coordinates);
                        }
                        return true;
                    }
                }
            }
            blockId = CHBlock.CalculateId(vertex2, _blockSize);
            if (!_blocks.TryGet(blockId, out block))
            { // damn block not cached!
                block = this.DeserializeBlock(blockId);
                if (block == null)
                { // oops even now the block is not found!
                    shape = null;
                    return false;
                }
                _blocks.Add(blockId, block);
            }
            if (!_blockShapes.TryGet(blockId, out blockCoordinates))
            { // damn block not cached!
                blockCoordinates = this.DeserializeShape(blockId);
                if (blockCoordinates == null)
                { // oops even now the block is not found!
                    shape = null;
                    return false;
                }
                _blockShapes.Add(blockId, blockCoordinates);
            }
            blockIdx = vertex2 - blockId;
            if (block.Vertices != null &&
                blockIdx < block.Vertices.Length)
            { // block is found and the vertex is there!
                for (int arcIdx = block.Vertices[blockIdx].ArcIndex;
                    arcIdx < block.Vertices[blockIdx].ArcIndex + block.Vertices[blockIdx].ArcCount; arcIdx++)
                { // loop over all arcs.
                    var chArc = block.Arcs[arcIdx];
                    if (chArc.TargetId == vertex1)
                    {
                        var arcCoordinates = blockCoordinates.Arcs[arcIdx];
                        shape = null;
                        if (arcCoordinates.Coordinates != null)
                        {
                            shape = new CoordinateArrayCollection<GeoCoordinateSimple>(arcCoordinates.Coordinates);
                        }
                        return true;
                    }
                }
            }
            shape = null;
            return false;
        }

        /// <summary>
        /// Loads all arcs associated with the given vertex.
        /// </summary>
        /// <param name="vertex">The vertex to get all incident edges for.</param>
        /// <returns>A tuple with (block index, arc index, neighbour, neighbour data).</returns>
        private Tuple<uint, uint, uint, CHEdgeData>[] LoadArcs(uint vertex)
        {
            uint blockId = CHBlock.CalculateId(vertex, _blockSize);
            CHBlock block;
            if (!_blocks.TryGet(blockId, out block))
            { // damn block not cached!
                block = this.DeserializeBlock(blockId);
                if (block == null)
                { // oops the block is not found!
                    return new Tuple<uint, uint, uint, CHEdgeData>[0];
                }
                _blocks.Add(blockId, block);
            }
            uint blockIdx = vertex - blockId;
            if (block.Vertices != null &&
                blockIdx < block.Vertices.Length)
            { // block is found and the vertex is there!
                var arcs = new Tuple<uint, uint, uint, CHEdgeData>[block.Vertices[blockIdx].ArcCount];
                for (uint arcIdx = block.Vertices[blockIdx].ArcIndex;
                    arcIdx < block.Vertices[blockIdx].ArcIndex + block.Vertices[blockIdx].ArcCount; arcIdx++)
                { // loop over all arcs.
                    var chArc = block.Arcs[arcIdx];
                    var edgeData = new CHEdgeData(chArc.Value, chArc.Weight, chArc.Meta);

                    arcs[arcIdx - block.Vertices[blockIdx].ArcIndex] =
                        new Tuple<uint, uint, uint, CHEdgeData>(blockId, arcIdx, chArc.TargetId, edgeData);
                }
                return arcs;
            }
            // oops even now the block is not found!
            return new Tuple<uint, uint, uint, CHEdgeData>[0];
        }

        /// <summary>
        /// Gets the shape for the arc in the given block and at the given index.
        /// </summary>
        /// <param name="blockId">The index of the block to search.</param>
        /// <param name="arcIdx">The index of the arc in the block.</param>
        /// <param name="shape">The shape.</param>
        /// <returns>True if there was a shape.</returns>
        private bool GetShapeForArc(uint blockId, uint arcIdx, out ICoordinateCollection shape)
        {
            CHBlockCoordinates blockCoordinates;
            if (!_blockShapes.TryGet(blockId, out blockCoordinates))
            { // damn block not cached!
                blockCoordinates = this.DeserializeShape(blockId);
                if (blockCoordinates == null)
                { // oops even now the block is not found!
                    shape = null;
                    return false;
                }
                _blockShapes.Add(blockId, blockCoordinates);
            }
            var arcCoordinates = blockCoordinates.Arcs[arcIdx];
            shape = null;
            if (arcCoordinates.Coordinates != null)
            {
                shape = new CoordinateArrayCollection<GeoCoordinateSimple>(arcCoordinates.Coordinates);
            }
            return true;
        }

        /// <summary>
        /// Loads all arcs but no shapes associated with the given vertex.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        private KeyValuePair<uint, CHEdgeData>[] LoadArcsNoShapes(uint vertexId)
        {
            uint blockId = CHBlock.CalculateId(vertexId, _blockSize);
            CHBlock block;
            if (!_blocks.TryGet(blockId, out block))
            { // damn block not cached!
                block = this.DeserializeBlock(blockId);
                if (block == null)
                { // oops even now the block is not found!
                    return new KeyValuePair<uint, CHEdgeData>[0];
                }
                _blocks.Add(blockId, block);
            }
            uint blockIdx = vertexId - blockId;
            if (block.Vertices != null &&
                blockIdx < block.Vertices.Length)
            { // block is found and the vertex is there!
                var arcs = new KeyValuePair<uint, CHEdgeData>[
                    block.Vertices[blockIdx].ArcCount];
                for (int arcIdx = block.Vertices[blockIdx].ArcIndex;
                    arcIdx < block.Vertices[blockIdx].ArcIndex + block.Vertices[blockIdx].ArcCount; arcIdx++)
                { // loop over all arcs.
                    var chArc = block.Arcs[arcIdx];
                    var edgeData = new CHEdgeData(chArc.Value, chArc.Weight, chArc.Meta);
                    arcs[arcIdx - block.Vertices[blockIdx].ArcIndex] = new KeyValuePair<uint, CHEdgeData>(
                        chArc.TargetId, edgeData);
                }
                return arcs;
            }
            // oops even now the block is not found!
            return new KeyValuePair<uint, CHEdgeData>[0];
        }

        /// <summary>
        /// Deserialize the block with the given id.
        /// </summary>
        /// <param name="blockId"></param>
        /// <returns></returns>
        private CHBlock DeserializeBlock(uint blockId)
        {
            int blockOffset;
            int blockSize;
            uint blockIdx = blockId / _blockSize;
            if (blockIdx == 0)
            { // the block idx zero.
                blockOffset = _startOfBlocks;
                blockSize = _blocksIndex.BlockLocationIndex[blockIdx];
            }
            else
            { // need to calculate offset and size.
                blockOffset = _startOfBlocks + _blocksIndex.BlockLocationIndex[blockIdx - 1];
                blockSize = _blocksIndex.BlockLocationIndex[blockIdx] - _blocksIndex.BlockLocationIndex[blockIdx - 1];
            }

            return _serializer.DeserializeBlock(_stream, blockOffset, blockSize, true);
        }

        /// <summary>
        /// Deserialize the shape with the given id.
        /// </summary>
        /// <param name="blockId"></param>
        /// <returns></returns>
        private CHBlockCoordinates DeserializeShape(uint blockId)
        {
            int blockOffset;
            int blockSize;
            uint blockIdx = blockId / _blockSize;
            if (blockIdx == 0)
            { // the block idx zero.
                blockOffset = _startOfShapes;
                blockSize = _shapesIndex.BlockLocationIndex[blockIdx];
            }
            else
            { // need to calculate offset and size.
                blockOffset = _startOfShapes + _shapesIndex.BlockLocationIndex[blockIdx - 1];
                blockSize = _shapesIndex.BlockLocationIndex[blockIdx] - _shapesIndex.BlockLocationIndex[blockIdx - 1];
            }

            return _serializer.DeserializeBlockShape(_stream, blockOffset, blockSize, true);
        }

        #endregion

        /// <summary>
        /// An edge enumerator.
        /// </summary>
        private class EdgeEnumerator : IEdgeEnumerator<CHEdgeData>
        {
            /// <summary>
            /// Holds the edges.
            /// </summary>
            private Tuple<uint, uint, uint, CHEdgeData>[] _edges;

            /// <summary>
            /// Holds the source.
            /// </summary>
            private CHEdgeDataDataSource _source;

            /// <summary>
            /// Holds the current position.
            /// </summary>
            private int _current = -1;

            /// <summary>
            /// Creates a new enumerator.
            /// </summary>
            /// <param name="source">The datasource the edges come from.</param>
            /// <param name="edges">The edge data.</param>
            public EdgeEnumerator(CHEdgeDataDataSource source, Tuple<uint, uint, uint, CHEdgeData>[] edges)
            {
                _source = source;
                _edges = edges;
            }

            /// <summary>
            /// Moves to the next coordinate.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                _current++;
                return _edges.Length > _current;
            }

            /// <summary>
            /// Returns the current neighbour.
            /// </summary>
            public uint Neighbour
            {
                get { return _edges[_current].Item3; }
            }

            /// <summary>
            /// Returns the current edge data.
            /// </summary>
            public CHEdgeData EdgeData
            {
                get { return _edges[_current].Item4; }
            }

            /// <summary>
            /// Returns true if the edge data is inverted by default.
            /// </summary>
            public bool isInverted
            {
                get { return false; }
            }

            /// <summary>
            /// Returns the inverted edge data.
            /// </summary>
            public CHEdgeData InvertedEdgeData
            {
                get { return (CHEdgeData)this.EdgeData.Reverse(); }
            }

            /// <summary>
            /// Returns the current intermediates.
            /// </summary>
            public ICoordinateCollection Intermediates
            {
                get
                {
                    ICoordinateCollection shape;
                    if(_source.GetShapeForArc(_edges[_current].Item1, _edges[_current].Item2, out shape))
                    {
                        return shape;
                    }
                    return null;
                }
            }

            /// <summary>
            /// Returns the count.
            /// </summary>
            /// <returns></returns>
            public int Count()
            {
                return _edges.Length;
            }

            /// <summary>
            /// Resets this enumerator.
            /// </summary>
            public void Reset()
            {
                _current = -1;
            }

            public IEnumerator<Edge<CHEdgeData>> GetEnumerator()
            {
                this.Reset();
                return this;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                this.Reset();
                return this;
            }

            public Edge<CHEdgeData> Current
            {
                get { return new Edge<CHEdgeData>(this); }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this; }
            }

            public void Dispose()
            {

            }


            public bool HasCount
            {
                get { return true; }
            }

            int IEdgeEnumerator<CHEdgeData>.Count
            {
                get { return _edges.Length; }
            }
        }

        /// <summary>
        /// A neighbour enumerators.
        /// </summary>
        private class NeighbourEnumerator : INeighbourEnumerator<CHEdgeData>
        {
            /// <summary>
            /// Holds the edge and neighbours.
            /// </summary>
            private List<Tuple<uint, uint, uint, uint, CHEdgeData>> _neighbours;

            /// <summary>
            /// Holds the source.
            /// </summary>
            private CHEdgeDataDataSource _source;

            /// <summary>
            /// Holds the current position.
            /// </summary>
            private int _current = -1;

            /// <summary>
            /// Creates a new enumerators.
            /// </summary>
            /// <param name="source">The datasource the edges come from.</param>
            /// <param name="edges">The edge data.</param>
            public NeighbourEnumerator(CHEdgeDataDataSource source,
                List<Tuple<uint, uint, uint, uint, CHEdgeData>> neighbours)
            {
                _source = source;
                _neighbours = neighbours;
            }

            /// <summary>
            /// Moves to the next coordinate.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                _current++;
                return _neighbours.Count > _current;
            }

            /// <summary>
            /// Gets the first vector.
            /// </summary>
            public uint Vertex1
            {
                get { return _neighbours[_current].Item3; }
            }

            /// <summary>
            /// Gets the second vector.
            /// </summary>
            public uint Vertex2
            {
                get { return _neighbours[_current].Item4; }
            }

            /// <summary>
            /// Gets the edge data.
            /// </summary>
            public CHEdgeData EdgeData
            {
                get { return _neighbours[_current].Item5; }
            }

            /// <summary>
            /// Gets the current intermediates.
            /// </summary>
            public ICoordinateCollection Intermediates
            {
                get
                {
                    ICoordinateCollection shape;
                    if (_source.GetShapeForArc(_neighbours[_current].Item1, _neighbours[_current].Item2, out shape))
                    {
                        return shape;
                    }
                    return null;
                }
            }

            /// <summary>
            /// Returns true if this enumerator has a pre-calculated count.
            /// </summary>
            public bool HasCount
            {
                get { return true; }
            }

            /// <summary>
            /// Returns the count if any.
            /// </summary>
            public int Count
            {
                get { return _neighbours.Count; }
            }

            /// <summary>
            /// Resets this enumerator.
            /// </summary>
            public void Reset()
            {
                _current = -1;
            }

            public IEnumerator<Neighbour<CHEdgeData>> GetEnumerator()
            {
                this.Reset();
                return this;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                this.Reset();
                return this;
            }

            public Neighbour<CHEdgeData> Current
            {
                get { return new Neighbour<CHEdgeData>(this); }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this; }
            }

            public void Dispose()
            {

            }
        }

        public void AddRestriction(uint[] route)
        {
            throw new NotImplementedException();
        }

        public void AddRestriction(string vehicleType, uint[] route)
        {
            throw new NotImplementedException();
        }

        public bool TryGetRestrictionAsStart(Vehicle vehicle, uint vertex, out List<uint[]> routes)
        {
            throw new NotImplementedException();
        }

        public bool TryGetRestrictionAsEnd(Vehicle vehicle, uint vertex, out List<uint[]> routes)
        {
            throw new NotImplementedException();
        }

        public bool IsDirected
        {
            get { return true; }
        }


        public bool CanHaveDuplicates
        {
            get { throw new NotImplementedException(); }
        }

        public bool ContainsEdge(uint vertexId, uint neighbour, CHEdgeData data)
        {
            throw new NotImplementedException();
        }


        public bool GetEdge(uint vertex1, uint vertex2, out CHEdgeData data)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<Edge<CHEdgeData>> GetDirectNeighbours(uint vertex)
        {
            throw new NotImplementedException();
        }
    }
}