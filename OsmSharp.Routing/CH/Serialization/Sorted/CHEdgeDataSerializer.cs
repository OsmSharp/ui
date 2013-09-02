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
using Ionic.Zlib;
using OsmSharp.Osm;
using OsmSharp.Osm.Tiles;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Serialization;
using OsmSharp.Collections.Tags;
using OsmSharp.IO;
using OsmSharp.Math.Geo;
using ProtoBuf;
using ProtoBuf.Meta;

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
        private const int RegionZoom = 15;

        /// <summary>
        /// Holds the size of the height-bins to be sorted in.
        /// </summary>
        private const int HeightBinSize = 3;

        /// <summary>
        /// Holds the maximum number of vertices in a block.
        /// </summary>
        private const int BlockVertexSize = 100;

        /// <summary>
        /// Holds the compression flag.
        /// </summary>
        private readonly bool _compress;

        /// <summary>
        /// Holds the runtime type model.
        /// </summary>
        private readonly RuntimeTypeModel _runtimeTypeModel;

        /// <summary>
        /// Creates a new v2 serializer.
        /// </summary>
        /// <param name="compress">Flag telling this serializer to compress it's data.</param>
        public CHEdgeDataDataSourceSerializer(bool compress)
        {
            _compress = compress;

            RuntimeTypeModel typeModel = TypeModel.Create();
            typeModel.Add(typeof(CHBlockIndex), true); // the index containing all blocks.
            typeModel.Add(typeof(CHBlock), true); // the block definition.
            typeModel.Add(typeof(CHVertex), true); // a vertex definition.
            typeModel.Add(typeof(CHArc), true); // an arc definition.

            typeModel.Add(typeof(CHVertexRegionIndex), true); // the index containing all regions.
            typeModel.Add(typeof(CHVertexRegion), true); // the region definition.

            _runtimeTypeModel = typeModel;
        }

        /// <summary>
        /// Returns the version string.
        /// </summary>
        public override string VersionString
        {
            get { return "CHEdgeData.v2"; }
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
            IDynamicGraph<CHEdgeData> sortedGraph = CHEdgeDataDataSourceSerializer.SortGraph(graph);

            // create the regions.
            SortedDictionary<ulong, List<uint>> regions = new SortedDictionary<ulong, List<uint>>();
            for (uint newVertexId = 1; newVertexId < sortedGraph.VertexCount + 1; newVertexId++)
            {
                // add to the CHRegions.
                float latitude, longitude;
                sortedGraph.GetVertex(newVertexId, out latitude, out longitude);
                Tile tile = Tile.CreateAroundLocation(new GeoCoordinate(
                    latitude, longitude), RegionZoom);
                List<uint> regionVertices;
                if (!regions.TryGetValue(tile.Id, out regionVertices))
                {
                    regionVertices = new List<uint>();
                    regions.Add(tile.Id, regionVertices);
                }
                regionVertices.Add(newVertexId);
            }

            // serialize the sorted graph.
            // [START_OF_BLOCKS][[SIZE_OF_REGION_INDEX][REGION_INDEX][REGIONS]][[SIZE_OF_BLOCK_INDEX][BLOCK_INDEX][BLOCKS]]
            // STRART_OF_BLOCKS:        4bytes

            // SIZE_OF_REGION_INDEX:    4bytes
            // REGION_INDEX:            see SIZE_OF_REGION_INDEX
            // REGIONS:                 see START_OF_BLOCKS - 4bytes

            // SIZE_OF_BLOCK_INDEX:     4bytes
            // BLOCK_INDEX:             see SIZE_OF_BLOCK_INDEX.
            // BLOCKS:                  from START_OF_BLOCKS + 4bytes + SIZE_OF_BLOCKS_INDEX until END.

            // serialize regions and build their index.
            CHVertexRegionIndex chRegionIndex = new CHVertexRegionIndex();
            chRegionIndex.LocationIndex = new int[regions.Count];
            chRegionIndex.RegionIds = new ulong[regions.Count];
            var memoryStream = new MemoryStream();
            int regionIdx = 0;
            foreach(KeyValuePair<ulong, List<uint>> region in regions)
            {
                // serialize.
                CHVertexRegion chRegion = new CHVertexRegion();
                chRegion.Vertices = region.Value.ToArray();
                _runtimeTypeModel.Serialize(memoryStream, chRegion);
 
                // set index.
                chRegionIndex.LocationIndex[regionIdx] = (int)memoryStream.Position;
                chRegionIndex.RegionIds[regionIdx] = region.Key;
                regionIdx++;
            }
            stream.Seek(8, SeekOrigin.Begin); // move to beginning of [REGION_INDEX]
            _runtimeTypeModel.Serialize(stream, chRegionIndex); // write region index.
            int sizeRegionIndex = (int)(stream.Position - 8); // now at beginning of [REGIONS]
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.WriteTo(stream); // write regions.
            memoryStream.Dispose();
            int startOfBlocks = (int)stream.Position; // now at beginning of [SIZE_OF_BLOCK_INDEX]
            stream.Seek(0, SeekOrigin.Begin); // move to beginning
            stream.Write(BitConverter.GetBytes(startOfBlocks), 0, 4); // write start position of blocks. Now at [SIZE_OF_REGION_INDEX]
            stream.Write(BitConverter.GetBytes(sizeRegionIndex), 0, 4); // write size of blocks index.

            // serialize the blocks and build their index.
            memoryStream = new MemoryStream();
            List<int> blockLocations = new List<int>();
            uint vertexId = 1;
            while (vertexId < sortedGraph.VertexCount)
            {
                uint blockId = vertexId;
                List<CHArc> blockArcs = new List<CHArc>();
                List<CHVertex> blockVertices = new List<CHVertex>();
                while (vertexId < blockId + BlockVertexSize &&
                    vertexId < sortedGraph.VertexCount + 1)
                { // create this block.
                    CHVertex chVertex = new CHVertex();
                    float latitude, longitude;
                    sortedGraph.GetVertex(vertexId, out latitude, out longitude);
                    chVertex.Latitude = latitude;
                    chVertex.Longitude = longitude;
                    chVertex.ArcIndex = (ushort)(blockArcs.Count);
                    foreach (KeyValuePair<uint, CHEdgeData> sortedArc in sortedGraph.GetArcs(vertexId))
                    {
                        CHArc chArc = new CHArc();
                        chArc.TargetId = sortedArc.Key;
                        chArc.ShortcutId = sortedArc.Value.ContractedVertexId;
                        chArc.Weight = sortedArc.Value.Weight;
                        chArc.Direction = sortedArc.Value.Direction;
                        blockArcs.Add(chArc);
                    }
                    chVertex.ArcCount = (ushort)(blockArcs.Count - chVertex.ArcIndex);
                    blockVertices.Add(chVertex);

                    vertexId++; // move to the next vertex.
                }

                // create block.
                CHBlock block = new CHBlock();
                block.Arcs = blockArcs.ToArray();
                block.Vertices = blockVertices.ToArray(); // TODO: get rid of the list and create an array to begin with.
                
                // write blocks.
                _runtimeTypeModel.Serialize(memoryStream, block);
                blockLocations.Add((int)memoryStream.Position);
            }
            CHBlockIndex blockIndex = new CHBlockIndex();
            blockIndex.LocationIndex = blockLocations.ToArray();

            stream.Seek(startOfBlocks + 4, SeekOrigin.Begin); // move to beginning of [BLOCK_INDEX]
            _runtimeTypeModel.Serialize(stream, blockIndex); // write region index.
            int sizeBlockIndex = (int)(stream.Position - (startOfBlocks + 4)); // now at beginning of [BLOCKS]
            memoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.WriteTo(stream); // write blocks.
            memoryStream.Dispose();
            stream.Seek(startOfBlocks, SeekOrigin.Begin); // move to [SIZE_OF_BLOCK_INDEX]
            stream.Write(BitConverter.GetBytes(sizeBlockIndex), 0, 4); // write start position of blocks. Now at [SIZE_OF_REGION_INDEX]
            stream.Flush();
        }

        /// <summary>
        /// Returns a topologically sorted version of the given graph.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static IDynamicGraph<CHEdgeData> SortGraph(IDynamicGraph<CHEdgeData> graph)
        {
            // also add all downward edges.
            graph.AddDownwardEdges();

            // sort the topologically ordered vertices into bins representing a certain height range.
            List<uint>[] heightBins = new List<uint>[1000];
            foreach (var vertexDepth in new CHDepthFirstEnumerator(graph))
            { // enumerates all vertixes depth-first.
                int binIdx = (int)(vertexDepth.Depth / HeightBinSize);
                if (heightBins.Length < binIdx)
                { // resize bin array if needed.
                    Array.Resize(ref heightBins, System.Math.Max(heightBins.Length + 1000, binIdx + 1));
                }

                // add to the current bin.
                List<uint> bin = heightBins[binIdx];
                if (bin == null)
                { // create new bin.
                    bin = new List<uint>();
                    heightBins[binIdx] = bin;
                }
                bin.Add(vertexDepth.VertexId);
            }

            // temp test.
            MemoryDynamicGraph<CHEdgeData> sortedGraph = new MemoryDynamicGraph<CHEdgeData>();
            Dictionary<uint, uint> currentBinIds = new Dictionary<uint, uint>();
            uint newVertexId;
            for (int idx = 0; idx < heightBins.Length; idx++)
            {
                List<uint> bin = heightBins[idx];
                if (bin != null)
                { // translate ids.
                    // fill current bin ids and add vertices to the new graph.
                    foreach (uint binVertexId in bin)
                    {
                        float latitude, longitude;
                        graph.GetVertex(binVertexId, out latitude, out longitude);
                        newVertexId = sortedGraph.AddVertex(latitude, longitude);

                        currentBinIds.Add(binVertexId, newVertexId); // add to the current bin index.
                    }
                }
            }

            // rebuild the CH graph based on the new ordering and build the CHRegions.
            newVertexId = 0;
            for (int idx = 0; idx < heightBins.Length; idx++)
            {
                List<uint> bin = heightBins[idx];
                if (bin != null)
                { // translate ids.
                    // fill current bin ids and add vertices to the new graph.
                    //foreach (uint binVertexId in bin)
                    //{
                    //    float latitude, longitude;
                    //    graph.GetVertex(binVertexId, out latitude, out longitude);
                    //    newVertexId = sortedGraph.AddVertex(latitude, longitude);

                    //    currentBinIds.Add(binVertexId, newVertexId); // add to the current bin index.
                    //}
                    foreach (uint binVertexId in bin)
                    {
                        currentBinIds.TryGetValue(binVertexId, out newVertexId);

                        // get the higher arcs and convert their ids.
                        KeyValuePair<uint, CHEdgeData>[] arcs = graph.GetArcsHigher(binVertexId);
                        foreach (KeyValuePair<uint, CHEdgeData> arc in arcs)
                        {
                            // get target vertex.
                            uint nextVertexArcId = CHEdgeDataDataSourceSerializer.SearchVertex(arc.Key, currentBinIds, heightBins);
                            // convert edge.
                            CHEdgeData newEdge = new CHEdgeData();
                            newEdge.Direction = arc.Value.Direction;
                            if (arc.Value.HasContractedVertex)
                            { // contracted info.
                                newEdge.ContractedVertexId = CHEdgeDataDataSourceSerializer.SearchVertex(arc.Value.ContractedVertexId, currentBinIds, heightBins);
                            }
                            else
                            { // no contracted info.
                                newEdge.ContractedVertexId = 0;
                            }
                            newEdge.Tags = arc.Value.Tags;
                            newEdge.Weight = arc.Value.Weight;
                            sortedGraph.AddArc(newVertexId, nextVertexArcId, newEdge, null);
                        }
                    }
                }
            }
            return sortedGraph;
        }

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
        /// <returns></returns>
        protected override IBasicRouterDataSource<CHEdgeData> DoDeserialize(
            LimitedStream stream, bool lazy)
        {
            var intBytes = new byte[4];
            stream.Read(intBytes, 0, 4);
            int startOfBlocks = BitConverter.ToInt32(intBytes, 0);

            // deserialize regions index.
            stream.Read(intBytes, 0, 4);
            int sizeRegionIndex = BitConverter.ToInt32(intBytes, 0);
            var chVertexRegionIndex = (CHVertexRegionIndex)_runtimeTypeModel.Deserialize(
                new CappedStream(stream, stream.Position, sizeRegionIndex), null,
                    typeof(CHVertexRegionIndex));

            // deserialize blocks index.
            stream.Seek(startOfBlocks, SeekOrigin.Begin);
            stream.Read(intBytes, 0, 4);
            int sizeBlockIndex = BitConverter.ToInt32(intBytes, 0);
            var chBlockIndex = (CHBlockIndex)_runtimeTypeModel.Deserialize(
                new CappedStream(stream, stream.Position, sizeBlockIndex), null,
                    typeof(CHBlockIndex));

            return new CHEdgeDataDataSource(stream, this, sizeRegionIndex + 8,
                chVertexRegionIndex, RegionZoom, startOfBlocks + sizeBlockIndex + 4, chBlockIndex, BlockVertexSize);
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
            if (decompress)
            { // decompress the data.
                var buffer = new byte[length];
                stream.Seek(offset, SeekOrigin.Begin);
                stream.Read(buffer, 0, length);

                var memoryStream = new MemoryStream(buffer);
                var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
                return (CHBlock)_runtimeTypeModel.Deserialize(gZipStream
                                                                             , null,
                                                                             typeof(CHBlock));
            }
            return (CHBlock)_runtimeTypeModel.Deserialize(
                new CappedStream(stream, offset, length), null,
                    typeof(CHBlock));
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
                return (CHVertexRegion)_runtimeTypeModel.Deserialize(gZipStream
                                                                             , null,
                                                                             typeof(CHVertexRegion));
            }
            return (CHVertexRegion)_runtimeTypeModel.Deserialize(
                new CappedStream(stream, offset, length), null,
                    typeof(CHVertexRegion));
        }
    }
}