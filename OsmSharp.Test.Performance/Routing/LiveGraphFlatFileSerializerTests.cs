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

using OsmSharp.Collections.Coordinates;
using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Graphs.Serialization;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;
using System.IO;

namespace OsmSharp.Test.Performance.Routing
{
    /// <summary>
    /// Holds tests for the live edge serialization code.
    /// </summary>
    public static class LiveEdgeGraphFlatFileSerializerTests
    {
        /// <summary>
        /// Tests the CH serializer.
        /// </summary>
        public static void Test()
        {
            LiveEdgeGraphFlatFileSerializerTests.TestSerialization("LiveSerializerFlatFile", "kempen-big.osm.pbf");
        }

        /// <summary>
        /// Tests preprocessing data from a PBF file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pbfFile"></param>
        public static void TestSerialization(string name, string pbfFile)
        {
            var testFile = new FileInfo(string.Format(@".\TestFiles\{0}", pbfFile));

            var performanceInfo = new PerformanceInfoConsumer("LiveSerializerFlatFile.Serialize", 2000);
            performanceInfo.Start();
            performanceInfo.Report("Pulling from {0}...", testFile.Name);

            var stream = testFile.OpenRead();
            var source = new PBFOsmStreamSource(stream);
            var progress = new OsmStreamFilterProgress();
            progress.RegisterSource(source);

            var testOutputFile = new FileInfo(@"test.routing");
            testOutputFile.Delete();
            var writeStream = testOutputFile.OpenWrite();

            var tagsIndex = new TagsTableCollectionIndex();
            var interpreter = new OsmRoutingInterpreter();
            var graph = new DynamicGraphRouterDataSource<LiveEdge>(new Graph<LiveEdge>(), tagsIndex);
            var routingSerializer = new LiveEdgeFlatfileSerializer();

            // read from the OSM-stream.
            using (var file = new MemoryMappedStream(new FileInfo(@"temp.map").Open(FileMode.OpenOrCreate, FileAccess.ReadWrite)))
            {
                var memoryMappedGraph = new Graph<LiveEdge>(file, 1024, LiveEdge.MapFromDelegate, LiveEdge.MapToDelegate, LiveEdge.SizeUints);
                var coordinates = new HugeCoordinateIndex(file, 1024);
                var memoryData = new DynamicGraphRouterDataSource<LiveEdge>(memoryMappedGraph, tagsIndex);
                var targetData = new LiveGraphOsmStreamTarget(memoryData, new OsmRoutingInterpreter(), tagsIndex, coordinates);
                targetData.RegisterSource(progress);
                targetData.Pull();

                performanceInfo.Stop();

                performanceInfo = new PerformanceInfoConsumer("LiveSerializerFlatFile.Serialize", 100000);
                performanceInfo.Start();
                performanceInfo.Report("Writing file for {0}...", testFile.Name);

                var metaData = new TagsCollection();
                metaData.Add("some_key", "some_value");
                routingSerializer.Serialize(writeStream, memoryData, metaData);
            }
            stream.Dispose();
            writeStream.Dispose();

            OsmSharp.Logging.Log.TraceEvent("LiveSerializerFlatFile", OsmSharp.Logging.TraceEventType.Information,
                string.Format("Serialized file: {0}KB", testOutputFile.Length / 1024));
            performanceInfo.Stop();

            performanceInfo = new PerformanceInfoConsumer("LiveSerializerFlatFile.Serialize", 100000);
            performanceInfo.Start();
            performanceInfo.Report("Reading file for {0}...", testFile.Name);

            var testInputFile = new FileInfo(@"europe-latest.osm.pbf.routing");
            Stream readStream = testInputFile.OpenRead();

            var deserializedGraph = routingSerializer.Deserialize(readStream, false);

            readStream.Dispose();

            OsmSharp.Logging.Log.TraceEvent("LiveSerializerFlatFile", OsmSharp.Logging.TraceEventType.Information,
                string.Format("Read: {0}KB", testInputFile.Length / 1024));

            OsmSharp.Logging.Log.TraceEvent("LiveSerializerFlatFile", Logging.TraceEventType.Information, deserializedGraph.ToInvariantString());

            performanceInfo.Stop();
        }
    }
}
