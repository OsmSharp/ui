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
using OsmSharp.Routing.Graph.Serialization;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams;
using System.IO;

namespace OsmSharp.Test.Performance.Routing
{
    /// <summary>
    /// Holds tests for the edge serialization code.
    /// </summary>
    public static class GraphFlatFileSerializerTests
    {
        /// <summary>
        /// Tests the CH serializer.
        /// </summary>
        public static void Test()
        {
            GraphFlatFileSerializerTests.TestSerialization("SerializerFlatFile", "kempen.osm.pbf");
        }

        /// <summary>
        /// Tests preprocessing data from a PBF file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pbfFile"></param>
        public static void TestSerialization(string name, string pbfFile)
        {
            var testFile = new FileInfo(string.Format(@".\TestFiles\{0}", pbfFile));

            var performanceInfo = new PerformanceInfoConsumer("SerializerFlatFile.Serialize", 2000);
            performanceInfo.Start();
            performanceInfo.Report("Pulling from {0}...", testFile.Name);

            var stream = testFile.OpenRead();
            var source = new PBFOsmStreamSource(stream);
            var progress = new OsmStreamFilterProgress();
            progress.RegisterSource(source);

            var testOutputFile = new FileInfo(@"test.routing");
            testOutputFile.Delete();
            var writeStream = testOutputFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);

            var tagsIndex = new TagsIndex();
            var interpreter = new OsmRoutingInterpreter();
            var graph = new RouterDataSource<Edge>(new Graph<Edge>(), tagsIndex);
            var routingSerializer = new RoutingDataSourceSerializer();

            var memoryMappedGraph = new Graph<Edge>(1024);
            var coordinates = new HugeCoordinateIndex(1024);
            var memoryData = new RouterDataSource<Edge>(memoryMappedGraph, tagsIndex);
            var targetData = new GraphOsmStreamTarget(memoryData, new OsmRoutingInterpreter(), tagsIndex, coordinates);
            targetData.RegisterSource(progress);
            targetData.Pull();

            performanceInfo.Stop();

            memoryData.Compress();

            performanceInfo = new PerformanceInfoConsumer("SerializerFlatFile.Serialize", 100000);
            performanceInfo.Start();
            performanceInfo.Report("Writing file for {0}...", testFile.Name);

            var metaData = new TagsCollection();
            metaData.Add("some_key", "some_value");
            routingSerializer.Serialize(writeStream, memoryData, metaData);
            stream.Dispose();
            writeStream.Dispose();

            OsmSharp.Logging.Log.TraceEvent("SerializerFlatFile", OsmSharp.Logging.TraceEventType.Information,
                string.Format("Serialized file: {0}KB", testOutputFile.Length / 1024));
            performanceInfo.Stop();

            ////performanceInfo = new PerformanceInfoConsumer("SerializerFlatFile.Serialize", 100000);
            ////performanceInfo.Start();
            ////performanceInfo.Report("Reading file for {0}...", testFile.Name);

            //var testInputFile = new FileInfo(@"test.routing");
            //var readStream = testInputFile.OpenRead();

            //RoutingTest.TestSerialized(readStream);

            ////var deserializedGraph = routingSerializer.Deserialize(readStream, false);

            ////readStream.Dispose();

            ////OsmSharp.Logging.Log.TraceEvent("SerializerFlatFile", OsmSharp.Logging.TraceEventType.Information,
            ////    string.Format("Read: {0}KB", testInputFile.Length / 1024));

            ////OsmSharp.Logging.Log.TraceEvent("SerializerFlatFile", Logging.TraceEventType.Information, deserializedGraph.ToInvariantString());

            ////performanceInfo.Stop();
        }
    }
}
