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
using System.IO;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Tiles;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Collections.Cache;

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
        private ITagsCollectionIndex _tagsIndex;

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
            int startOfBlocks, CHBlockIndex blockIndex, uint blockSize)
        {
            _stream = stream;
            _serializer = serializer;
            _vehicles = new HashSet<string>(vehicles);

            this.InitializeRegions(startOfRegions, regionIndex, zoom);
            this.InitializeBlocks(startOfBlocks, blockIndex, blockSize);

            _blocks = new LRUCache<uint, CHBlock>(1000);
            _regions = new LRUCache<ulong, CHVertexRegion>(1000);
            _tagsIndex = new TagsTableCollectionIndex();
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
        public KeyValuePair<uint, KeyValuePair<uint, CHEdgeData>>[] GetArcs(
            GeoCoordinateBox box)
        {
            List<uint> vertices = this.LoadVerticesIn(box);
            KeyValuePair<uint, KeyValuePair<uint, CHEdgeData>>[] arcs =
                new KeyValuePair<uint, KeyValuePair<uint, CHEdgeData>>[vertices.Count * 3];
            int arcCount = 0;
            foreach (uint vertexId in vertices)
            {
                KeyValuePair<uint, CHEdgeData>[] vertexArcs = this.GetArcs(vertexId);
                foreach (KeyValuePair<uint, CHEdgeData> arc in vertexArcs)
                {
                    arcCount++;
                    if (arcs.Length <= arcCount)
                    { // resize array.
                        Array.Resize(ref arcs, arcs.Length + 100);
                    }
                    arcs[arcCount - 1] = new KeyValuePair<uint, KeyValuePair<uint, CHEdgeData>>(
                        vertexId, arc);
                }
            }
            Array.Resize(ref arcs, arcCount);
            return arcs;
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
        /// Returns all arcs for the given vertex.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        public KeyValuePair<uint, CHEdgeData>[] GetArcs(uint vertexId)
        {
            return this.LoadArcs(vertexId);
        }

        /// <summary>
        /// Returns true if the given vertex has the given neighbour.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <param name="neighbour"></param>
        /// <returns></returns>
        public bool HasArc(uint vertexId, uint neighbour)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the vertex count.
        /// </summary>
        public uint VertexCount
        {
            get { throw new NotSupportedException(); }
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
                if(idx == 0)
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
        private List<uint> LoadVerticesIn(GeoCoordinateBox box)
        {
            List<uint> vertices = new List<uint>();
            TileRange range = TileRange.CreateAroundBoundingBox(box, _zoom);
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
                    vertices.AddRange(region.Vertices);
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
            if(_regionStreamParts.TryGetValue(id, out part))
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
            if(!_blocks.TryGet(blockId, out block))
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
            if(block.Vertices != null &&
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
        /// Loads all arcs associated with the given vertex.
        /// </summary>
        /// <param name="vertexId"></param>
        /// <returns></returns>
        private KeyValuePair<uint, CHEdgeData>[] LoadArcs(uint vertexId)
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
                KeyValuePair<uint, CHEdgeData>[] arcs = new KeyValuePair<uint, CHEdgeData>[
                    block.Vertices[blockIdx].ArcCount];
                for (int arcIdx = block.Vertices[blockIdx].ArcIndex;
                    arcIdx < block.Vertices[blockIdx].ArcIndex + block.Vertices[blockIdx].ArcCount; arcIdx++)
                { // loop over all arcs.
                    CHArc chArc = block.Arcs[arcIdx];
                    CHEdgeData edgeData = new CHEdgeData();
                    edgeData.Direction = chArc.Direction;
                    edgeData.ContractedVertexId = chArc.ShortcutId;
                    edgeData.Weight = chArc.Weight;
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
                blockSize = _blocksIndex.LocationIndex[blockIdx];
            }
            else
            { // need to calculate offset and size.
                blockOffset = _startOfBlocks + _blocksIndex.LocationIndex[blockIdx - 1];
                blockSize = _blocksIndex.LocationIndex[blockIdx] - _blocksIndex.LocationIndex[blockIdx - 1];
            }

            return _serializer.DeserializeBlock(_stream, blockOffset, blockSize, false);
        }

        #endregion


        public void AddRestriction(uint[] route)
        {
            throw new NotImplementedException();
        }

        public void AddRestriction(Vehicle vehicle, uint[] route)
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
    }
}
