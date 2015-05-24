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

using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.IO.MemoryMappedFiles;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.Serialization;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams;
using OsmSharp.Routing.Vehicles;
using System.IO;
using System.Reflection;

namespace OsmSharp.Test.Performance.Routing.CH
{
    /// <summary>
    /// Holds tests for the CH serialization code.
    /// </summary>
    public static class CHEdgeGraphSerializerTests
    {
        /// <summary>
        /// Tests the CH serializer.
        /// </summary>
        public static RouterDataSource<CHEdgeData> Test()
        {
            return CHEdgeGraphSerializerTests.TestSerialization("CHSerializerFlatFile", "belgium-latest.osm.pbf");
        }

        /// <summary>
        /// Tests preprocessing data from a PBF file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pbfFile"></param>
        public static RouterDataSource<CHEdgeData> TestSerialization(string name, string pbfFile)
        {
			var testFilePath = Path.Combine (
				Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
				"TestFiles", pbfFile);
			//var testFilePath = @"/Users/xivk/work/OSM/bin/africa-latest.osm.pbf";
			var testFile = new FileInfo(testFilePath);
            var stream = testFile.OpenRead();
            var source = new OsmSharp.Osm.Streams.Filters.OsmStreamFilterProgress();
            source.RegisterSource(new PBFOsmStreamSource(stream));

            var testOutputFile = new FileInfo(@"test.routing");
            testOutputFile.Delete();
            Stream writeStream = testOutputFile.Open(FileMode.CreateNew, FileAccess.ReadWrite);
            
            var performanceInfo = new PerformanceInfoConsumer("CHSerializerFlatFile.Serialize");
            performanceInfo.Start();
            performanceInfo.Report("Pulling from {0}...", testFile.Name);

            var data = CHEdgeGraphOsmStreamTarget.Preprocess(
                source, new TagsIndex(new MemoryMappedStream(new MemoryStream())),
                new OsmRoutingInterpreter(), Vehicle.Car);

            (data.Graph as DirectedGraph<CHEdgeData>).Compress(true);

            //var graphCopy = new DirectedGraph<CHEdgeData>();
            //graphCopy.CopyFrom(data);
            //data = new RouterDataSource<CHEdgeData>(graphCopy, data.TagsIndex);

            var metaData = new TagsCollection();
            metaData.Add("some_key", "some_value");
            var routingSerializer = new CHEdgeSerializer();
            routingSerializer.Serialize(writeStream, data, metaData);

            stream.Dispose();
            writeStream.Dispose();

            OsmSharp.Logging.Log.TraceEvent("CHSerializerFlatFile", OsmSharp.Logging.TraceEventType.Information,
                string.Format("Serialized file: {0}KB", testOutputFile.Length / 1024));

            performanceInfo.Stop();

            performanceInfo = new PerformanceInfoConsumer("CHSerializerFlatFile.Deserialize");
            performanceInfo.Start();
            performanceInfo.Report("Deserializing again...");

            // open file again and read.
            writeStream = testOutputFile.OpenRead();
            var deserializedGraph = routingSerializer.Deserialize(writeStream, false);

            performanceInfo.Stop();
            return data;
        }
    }
}
