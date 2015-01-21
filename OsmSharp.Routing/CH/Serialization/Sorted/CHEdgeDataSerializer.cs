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

using Ionic.Zlib;
using OsmSharp.Collections.Coordinates.Collections;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Collections.Tags.Serializer.Index;
using OsmSharp.IO;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Tiles;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Serialization;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;

namespace OsmSharp.Routing.CH.Serialization.Sorted
{
    /// <summary>
    /// A v2 routing serializer.
    /// </summary>
    /// <remarks>Versioning is implemented in the file format to guarantee backward compatibility.</remarks>
    public class CHEdgeDataDataSourceSerializer : RoutingDataSourceSerializer<CHEdgeData>
    {
        /// <summary>
        /// Holds the zoom-level of the regions.
        /// </summary>
        private int _regionZoom = 18;

        /// <summary>
        /// Holds the size of the height-bins to be sorted in.
        /// </summary>
        private int _heightBinSize = 3;

        /// <summary>
        /// Holds the maximum number of vertices in a block.
        /// </summary>
        private int _blockVertexSize = 2;

        /// <summary>
        /// Holds the runtime type model.
        /// </summary>
        private readonly RuntimeTypeModel _runtimeTypeModel;

        /// <summary>
        /// Holds the sorted
        /// </summary>
        private bool _sort = false;

        /// <summary>
        /// Creates a new v2 serializer.
        /// </summary>
        public CHEdgeDataDataSourceSerializer()
            : this(true)
        {

        }

        /// <summary>
        /// Creates a new v2 serializer.
        /// </summary>
        /// <param name="sort"></param>
        public CHEdgeDataDataSourceSerializer(bool sort)
        {
            RuntimeTypeModel typeModel = TypeModel.Create();
            typeModel.Add(typeof(CHBlockIndex), true); // the index containing all blocks.
            typeModel.Add(typeof(CHBlock), true); // the block definition.
            typeModel.Add(typeof(CHBlockCoordinates), true);
            typeModel.Add(typeof(CHVertex), true); // a vertex definition.
            typeModel.Add(typeof(CHArc), true); // an arc definition.
            typeModel.Add(typeof(CHArcCoordinates), true);

            typeModel.Add(typeof(CHVertexRegionIndex), true); // the index containing all regions.
            typeModel.Add(typeof(CHVertexRegion), true); // the region definition.

            _runtimeTypeModel = typeModel;
            _sort = sort;
        }

        /// <summary>
        /// Creates a new v2 serializer.
        /// </summary>
        /// <param name="regionZoom"></param>
        /// <param name="heightBinSize"></param>
        /// <param name="blockVertexSize"></param>
        public CHEdgeDataDataSourceSerializer(int regionZoom, int heightBinSize, int blockVertexSize)
            : this(true, regionZoom, heightBinSize, blockVertexSize)
        {

        }

        /// <summary>
        /// Creates a new v2 serializer.
        /// </summary>
        /// <param name="sort"></param>
        /// <param name="regionZoom"></param>
        /// <param name="heightBinSize"></param>
        /// <param name="blockVertexSize"></param>
        public CHEdgeDataDataSourceSerializer(bool sort, int regionZoom, int heightBinSize, int blockVertexSize)
        {
            _regionZoom = regionZoom;
            _heightBinSize = heightBinSize;
            _blockVertexSize = blockVertexSize;

            RuntimeTypeModel typeModel = TypeModel.Create();
            typeModel.Add(typeof(CHBlockIndex), true); // the index containing all blocks.
            typeModel.Add(typeof(CHBlock), true); // the block definition.
            typeModel.Add(typeof(CHBlockCoordinates), true);
            typeModel.Add(typeof(CHVertex), true); // a vertex definition.
            typeModel.Add(typeof(CHArc), true); // an arc definition.
            typeModel.Add(typeof(CHArcCoordinates), true);

            typeModel.Add(typeof(CHVertexRegionIndex), true); // the index containing all regions.
            typeModel.Add(typeof(CHVertexRegion), true); // the region definition.

            _runtimeTypeModel = typeModel;
            _sort = sort;
        }

        /// <summary>
        /// Returns the version string.
        /// </summary>
        public override string VersionString
        {
            get { return "CHEdgeData.v3"; }
        }

        /// <summary>
        /// Does the v2 serialization.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="graph"></param>
        /// <returns></returns>
        protected override void DoSerialize(LimitedStream stream,
            DynamicGraphRouterDataSource<CHEdgeData> graph)
        {
            // sort the graph.
            IGraph<CHEdgeData> sortedGraph = graph;
            //if (_sort)
            //{ // sort the graph.
            //    sortedGraph = this.SortGraph(graph);
            //}

            // create the regions.
            var regions = new SortedDictionary<ulong, List<uint>>();
            for (uint newVertexId = 1; newVertexId < sortedGraph.VertexCount + 1; newVertexId++)
            {
                // add to the CHRegions.
                float latitude, longitude;
                sortedGraph.GetVertex(newVertexId, out latitude, out longitude);
                var tile = Tile.CreateAroundLocation(new GeoCoordinate(
                    latitude, longitude), _regionZoom);
                List<uint> regionVertices;
                if (!regions.TryGetValue(tile.Id, out regionVertices))
                {
                    regionVertices = new List<uint>();
                    regions.Add(tile.Id, regionVertices);
                }
                regionVertices.Add(newVertexId);
            }

            // serialize the sorted graph.
            // [START_OF_BLOCKS][START_OF_SHAPES][START_OF_TAGS][[SIZE_OF_REGION_INDEX][REGION_INDEX][REGIONS]][[SIZE_OF_BLOCK_INDEX][BLOCK_INDEX][BLOCKS]][[SIZE_OF_SHAPE_INDEX][SHAPE_INDEX][SHAPES]][TAGS]
            // START_OF_BLOCKS:         4bytes
            // START_OF_SHAPES:         4bytes
            // START_OF_STAGS:          4bytes

            // SIZE_OF_REGION_INDEX:    4bytes
            // REGION_INDEX:            see SIZE_OF_REGION_INDEX
            // REGIONS:                 see START_OF_BLOCKS - 4bytes

            // SIZE_OF_BLOCK_INDEX:     4bytes
            // BLOCK_INDEX:             see SIZE_OF_BLOCK_INDEX.
            // BLOCKS:                  from START_OF_BLOCKS + 4bytes + SIZE_OF_BLOCKS_INDEX until END.

            // serialize regions and build their index.
            var chRegionIndex = new CHVertexRegionIndex();
            chRegionIndex.LocationIndex = new int[regions.Count];
            chRegionIndex.RegionIds = new ulong[regions.Count];
            var memoryStream = new MemoryStream();
            int regionIdx = 0;
            foreach (var region in regions)
            {
                // serialize.
                var chRegion = new CHVertexRegion();
                chRegion.Vertices = region.Value.ToArray();
                _runtimeTypeModel.Serialize(memoryStream, chRegion);

                // set index.
                chRegionIndex.LocationIndex[regionIdx] = (int)memoryStream.Position;
                chRegionIndex.RegionIds[regionIdx] = region.Key;
                regionIdx++;
            }
            stream.Seek(16, SeekOrigin.Begin); // move to beginning of [REGION_INDEX]
            _runtimeTypeModel.Serialize(stream, chRegionIndex); // write region index.
            var sizeRegionIndex = (int)(stream.Position - 16); // now at beginning of [REGIONS]
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.WriteTo(stream); // write regions.
            memoryStream.Dispose();
            var startOfBlocks = (int)stream.Position; // now at beginning of [SIZE_OF_BLOCK_INDEX]

            // serialize the blocks and build their index.
            memoryStream = new MemoryStream();
            var blockLocations = new List<int>();
            var blockShapeLocations = new List<int>();
            var blockShapes = new List<CHBlockCoordinates>();
            uint vertexId = 1;
            while (vertexId < sortedGraph.VertexCount + 1)
            {
                uint blockId = vertexId;
                var blockArcs = new List<CHArc>();
                var blockArcCoordinates = new List<CHArcCoordinates>();
                var blockVertices = new List<CHVertex>();
                while (vertexId < blockId + _blockVertexSize &&
                    vertexId < sortedGraph.VertexCount + 1)
                { // create this block.
                    var chVertex = new CHVertex();
                    float latitude, longitude;
                    sortedGraph.GetVertex(vertexId, out latitude, out longitude);
                    chVertex.Latitude = latitude;
                    chVertex.Longitude = longitude;
                    chVertex.ArcIndex = (ushort)(blockArcs.Count);
                    foreach (var sortedArc in sortedGraph.GetEdges(vertexId))
                    {
                        var chArc = new CHArc();
                        chArc.TargetId = sortedArc.Neighbour;
                        chArc.Meta = sortedArc.EdgeData.Meta;
                        chArc.Value = sortedArc.EdgeData.Value;
                        chArc.Weight = sortedArc.EdgeData.Weight;
                        blockArcs.Add(chArc);

                        var chArcCoordinates = new CHArcCoordinates();
                        chArcCoordinates.Coordinates = sortedArc.Intermediates.ToSimpleArray();
                        blockArcCoordinates.Add(chArcCoordinates);
                    }
                    chVertex.ArcCount = (ushort)(blockArcs.Count - chVertex.ArcIndex);
                    blockVertices.Add(chVertex);

                    vertexId++; // move to the next vertex.
                }

                // create block.
                var block = new CHBlock();
                block.Arcs = blockArcs.ToArray();
                block.Vertices = blockVertices.ToArray(); // TODO: get rid of the list and create an array to begin with.
                var blockCoordinates = new CHBlockCoordinates();
                blockCoordinates.Arcs = blockArcCoordinates.ToArray();

                // write blocks.
                var blockStream = new MemoryStream();
                _runtimeTypeModel.Serialize(blockStream, block);
                var compressed = GZipStream.CompressBuffer(blockStream.ToArray());
                blockStream.Dispose();

                memoryStream.Write(compressed, 0, compressed.Length);
                blockLocations.Add((int)memoryStream.Position);

                blockShapes.Add(blockCoordinates);
            }
            var blockIndex = new CHBlockIndex();
            blockIndex.BlockLocationIndex = blockLocations.ToArray();

            stream.Seek(startOfBlocks + 4, SeekOrigin.Begin); // move to beginning of [BLOCK_INDEX]
            _runtimeTypeModel.Serialize(stream, blockIndex); // write region index.
            int sizeBlockIndex = (int)(stream.Position - (startOfBlocks + 4)); // now at beginning of [BLOCKS]
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.WriteTo(stream); // write blocks.
            memoryStream.Dispose();

            // write shapes and index.
            memoryStream = new MemoryStream();
            var startOfShapes = stream.Position;
            foreach (var blockCoordinates in blockShapes)
            {
                // write shape blocks.
                var blockShapeStream = new MemoryStream();
                _runtimeTypeModel.Serialize(blockShapeStream, blockCoordinates);
                var compressed = GZipStream.CompressBuffer(blockShapeStream.ToArray());
                blockShapeStream.Dispose();

                memoryStream.Write(compressed, 0, compressed.Length);
                blockShapeLocations.Add((int)memoryStream.Position);
            }
            blockIndex = new CHBlockIndex();
            blockIndex.BlockLocationIndex = blockShapeLocations.ToArray();

            stream.Seek(startOfShapes + 4, SeekOrigin.Begin); // move to beginning of [SHAPE_INDEX]
            _runtimeTypeModel.Serialize(stream, blockIndex); // write shape index.
            int sizeShapeIndex = (int)(stream.Position - (startOfShapes + 4)); // now at beginning of [SHAPE]
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.WriteTo(stream); // write shapes.
            memoryStream.Dispose();

            // write tags after blocks.
            int tagsPosition = (int)stream.Position;
            TagIndexSerializer.SerializeBlocks(stream, graph.TagsIndex, 10);

            stream.Seek(startOfBlocks, SeekOrigin.Begin); // move to [SIZE_OF_BLOCK_INDEX]
            stream.Write(BitConverter.GetBytes(sizeBlockIndex), 0, 4); // write start position of blocks.
            stream.Seek(startOfShapes, SeekOrigin.Begin); // move to [SIZE_OF_BLOCK_INDEX]
            stream.Write(BitConverter.GetBytes(sizeShapeIndex), 0, 4); // write start position of blocks.

            // write index.
            stream.Seek(0, SeekOrigin.Begin); // move to beginning
            stream.Write(BitConverter.GetBytes(startOfBlocks), 0, 4); // write start position of blocks. 
            stream.Write(BitConverter.GetBytes(startOfShapes), 0, 4); // write start position of shapes. 
            stream.Write(BitConverter.GetBytes(tagsPosition), 0, 4);
            stream.Write(BitConverter.GetBytes(sizeRegionIndex), 0, 4); // write size of region index.
            stream.Flush();
        }

        ///// <summary>
        ///// Returns a topologically sorted version of the given graph.
        ///// </summary>
        ///// <param name="graph"></param>
        ///// <returns></returns>
        //public IGraph<CHEdgeData> SortGraph(IGraph<CHEdgeData> graph)
        //{
        //    //// also add all downward edges.
        //    //graph.AddDownwardEdges();

        //    // sort the topologically ordered vertices into bins representing a certain height range.
        //    var heightBins = new List<uint>[1000];
        //    foreach (var vertexDepth in new CHDepthFirstEnumerator(graph))
        //    { // enumerates all vertixes depth-first.
        //        int binIdx = (int)(vertexDepth.Depth / _heightBinSize);
        //        if (heightBins.Length < binIdx)
        //        { // resize bin array if needed.
        //            Array.Resize(ref heightBins, System.Math.Max(heightBins.Length + 1000, binIdx + 1));
        //        }

        //        // add to the current bin.
        //        var bin = heightBins[binIdx];
        //        if (bin == null)
        //        { // create new bin.
        //            bin = new List<uint>();
        //            heightBins[binIdx] = bin;
        //        }
        //        bin.Add(vertexDepth.VertexId);
        //    }

        //    // temp test.
        //    var sortedGraph = new MemoryGraph<CHEdgeData>();
        //    var currentBinIds = new Dictionary<uint, uint>();
        //    uint newVertexId;
        //    for (int idx = 0; idx < heightBins.Length; idx++)
        //    {
        //        var bin = heightBins[idx];
        //        if (bin != null)
        //        { // translate ids.
        //            // fill current bin ids and add vertices to the new graph.
        //            foreach (uint binVertexId in bin)
        //            {
        //                float latitude, longitude;
        //                graph.GetVertex(binVertexId, out latitude, out longitude);
        //                newVertexId = sortedGraph.AddVertex(latitude, longitude);

        //                currentBinIds.Add(binVertexId, newVertexId); // add to the current bin index.
        //            }
        //        }
        //    }

        //    // rebuild the CH graph based on the new ordering and build the CHRegions.
        //    newVertexId = 0;
        //    for (int idx = 0; idx < heightBins.Length; idx++)
        //    {
        //        var bin = heightBins[idx];
        //        if (bin != null)
        //        { // translate ids.
        //            foreach (uint binVertexId in bin)
        //            {
        //                currentBinIds.TryGetValue(binVertexId, out newVertexId);

        //                // get the higher arcs and convert their ids.
        //                var arcs = graph.GetEdges(binVertexId);
        //                foreach (var arc in arcs)
        //                {
        //                    // get target vertex.
        //                    uint nextVertexArcId = CHEdgeDataDataSourceSerializer.SearchVertex(arc.Neighbour, currentBinIds, heightBins);
        //                    // convert edge.
        //                    var newEdge = new CHEdgeData();
        //                    newEdge.CanMoveBackward = arc.EdgeData.CanMoveBackward;
        //                    newEdge.CanMoveForward = arc.EdgeData.CanMoveForward;
        //                    newEdge.Forward = arc.EdgeData.Forward;
        //                    if (arc.EdgeData.ContractedId != 0)
        //                    { // contracted info.
        //                        newEdge.ContractedId = CHEdgeDataDataSourceSerializer.SearchVertex(arc.EdgeData.ContractedId, currentBinIds, heightBins);
        //                    }
        //                    else
        //                    { // no contracted info.
        //                        newEdge.ContractedId = 0;
        //                        newEdge.Tags = arc.EdgeData.Tags;
        //                    }
        //                    newEdge.Weight = arc.EdgeData.Weight;
        //                    sortedGraph.AddEdge(newVertexId, nextVertexArcId, newEdge, arc.Intermediates);
        //                }
        //            }
        //        }
        //    }
        //    return sortedGraph;
        //}

        /// <summary>
        /// Searches for a vertex and returns it's new id.
        /// </summary>
        /// <param name="oldVertexId"></param>
        /// <param name="currentBin"></param>
        /// <param name="heightBins"></param>
        /// <returns></returns>
        private static uint SearchVertex(uint oldVertexId, Dictionary<uint, uint> currentBin, List<uint>[] heightBins)
        {
            uint newVertexId;
            if (!currentBin.TryGetValue(oldVertexId, out newVertexId))
            { // search vertex somewhere in the higher bins.
                uint currentBinStartId = 1;
                for (int binIdx = 0; binIdx < heightBins.Length; binIdx++)
                {
                    if (heightBins[binIdx] != null)
                    {
                        int indexOf = heightBins[binIdx].IndexOf(oldVertexId);
                        if (indexOf > 0)
                        { // vertex was found in this bin.
                            newVertexId = currentBinStartId + (uint)indexOf;
                            break;
                        }
                        currentBinStartId = currentBinStartId +
                            (uint)heightBins[binIdx].Count;
                    }
                }
            }
            return newVertexId;
        }

        /// <summary>
        /// Does the v2 deserialization.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lazy"></param>
        /// <param name="vehicles"></param>
        /// <returns></returns>
        protected override IBasicRouterDataSource<CHEdgeData> DoDeserialize(
            LimitedStream stream, bool lazy, IEnumerable<string> vehicles)
        {
            var intBytes = new byte[4];
            stream.Read(intBytes, 0, 4);
            int startOfBlocks = BitConverter.ToInt32(intBytes, 0);
            stream.Read(intBytes, 0, 4);
            int startOfShapes = BitConverter.ToInt32(intBytes, 0);
            stream.Read(intBytes, 0, 4);
            int startOfTags = BitConverter.ToInt32(intBytes, 0);
            stream.Read(intBytes, 0, 4);
            int sizeRegionIndex = BitConverter.ToInt32(intBytes, 0);

            // deserialize regions index.
            var chVertexRegionIndex = (CHVertexRegionIndex)_runtimeTypeModel.Deserialize(
                new CappedStream(stream, stream.Position, sizeRegionIndex), null,
                    typeof(CHVertexRegionIndex));

            // deserialize blocks index.
            stream.Seek(startOfBlocks, SeekOrigin.Begin);
            stream.Read(intBytes, 0, 4);
            int sizeBlockIndex = BitConverter.ToInt32(intBytes, 0);
            var blockIndex = (CHBlockIndex)_runtimeTypeModel.Deserialize(
                new CappedStream(stream, stream.Position, sizeBlockIndex), null,
                    typeof(CHBlockIndex));

            // deserialize shapes index.
            stream.Seek(startOfShapes, SeekOrigin.Begin);
            stream.Read(intBytes, 0, 4);
            int sizeShapesIndex = BitConverter.ToInt32(intBytes, 0);
            var shapesIndex = (CHBlockIndex)_runtimeTypeModel.Deserialize(
                new CappedStream(stream, stream.Position, sizeShapesIndex), null,
                    typeof(CHBlockIndex));

            // deserialize tags.
            stream.Seek(startOfTags, SeekOrigin.Begin);
            ITagsCollectionIndexReadonly tagsIndex = TagIndexSerializer.DeserializeBlocks(stream);

            return new CHEdgeDataDataSource(stream, this, vehicles, sizeRegionIndex + 16,
                chVertexRegionIndex, _regionZoom,
                startOfBlocks + sizeBlockIndex + 4, blockIndex, (uint)_blockVertexSize,
                startOfShapes + sizeShapesIndex + 4, shapesIndex,
                tagsIndex);
        }

        /// <summary>
        /// Deserializes the given block of data.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="decompress"></param>
        /// <returns></returns>
        internal CHBlock DeserializeBlock(Stream stream, long offset, int length, bool decompress)
        {
            CHBlock value = null;
            //for (int idx = 0; idx < 10; idx++)
            //{
            if (decompress)
            { // decompress the data.
                var buffer = new byte[length];
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Read(buffer, 0, length);

                var memoryStream = new MemoryStream(buffer);
                var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
                value = (CHBlock)_runtimeTypeModel.Deserialize(gZipStream, null, typeof(CHBlock));
            }
            else
            {
                value = (CHBlock)_runtimeTypeModel.Deserialize(
                    new CappedStream(stream, offset, length), null,
                        typeof(CHBlock));
            }
            //}
            return value;
        }

        /// <summary>
        /// Deserializes the given block shape of data.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="decompress"></param>
        /// <returns></returns>
        internal CHBlockCoordinates DeserializeBlockShape(Stream stream, long offset, int length, bool decompress)
        {
            CHBlockCoordinates value = null;
            //for (int idx = 0; idx < 10; idx++)
            //{
            if (decompress)
            { // decompress the data.
                var buffer = new byte[length];
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Read(buffer, 0, length);

                var memoryStream = new MemoryStream(buffer);
                var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
                value = (CHBlockCoordinates)_runtimeTypeModel.Deserialize(gZipStream, null, typeof(CHBlockCoordinates));
            }
            else
            {
                value = (CHBlockCoordinates)_runtimeTypeModel.Deserialize(
                        new CappedStream(stream, offset, length), null,
                            typeof(CHBlockCoordinates));
            }
            //}
            return value;
        }

        /// <summary>
        /// Deserialize the given region of data.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="decompress"></param>
        /// <returns></returns>
        internal CHVertexRegion DeserializeRegion(Stream stream, long offset, int length, bool decompress)
        {
            if (decompress)
            { // decompress the data.
                var buffer = new byte[length];
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Read(buffer, 0, length);

                var memoryStream = new MemoryStream(buffer);
                var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
                return (CHVertexRegion)_runtimeTypeModel.Deserialize(gZipStream, null,
                                                                             typeof(CHVertexRegion));
            }
            return (CHVertexRegion)_runtimeTypeModel.Deserialize(
                new CappedStream(stream, offset, length), null,
                    typeof(CHVertexRegion));
        }
    }
}