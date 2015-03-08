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

using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.Serialization;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams;
using OsmSharp.Routing.Vehicles;
using System.IO;

namespace OsmSharp.Test.Performance.Routing.CH
{
    /// <summary>
    /// Holds tests for the CH serialization code.
    /// </summary>
    public static class CHEdgeGraphFileStreamTargetTests
    {
        /// <summary>
        /// Tests the CH serializer.
        /// </summary>
        /// <returns>A stream to the file that was serialized.</returns>
        public static Stream Test()
        {
            return CHEdgeGraphFileStreamTargetTests.TestSerialization("CHSerializer", "kempen-big.osm.pbf");
        }

        /// <summary>
        /// Tests the CH serializer.
        /// </summary>
        /// <returns>A stream to the file that was serialized.</returns>
        public static Stream Test(RouterDataSource<CHEdgeData> data)
        {
            return CHEdgeGraphFileStreamTargetTests.TestSerialization("CHSerializer", "kempen-big.osm.pbf", data);
        }

        /// <summary>
        /// Tests preprocessing data from a PBF file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pbfFile"></param>
        public static Stream TestSerialization(string name, string pbfFile)
        {
            var testFile = new FileInfo(string.Format(@".\TestFiles\{0}", pbfFile));
            var stream = testFile.OpenRead();
            var source = new OsmSharp.Osm.Streams.Filters.OsmStreamFilterProgress();
            source.RegisterSource(new PBFOsmStreamSource(stream));

            var data = CHEdgeGraphOsmStreamTarget.Preprocess(
                source, new OsmRoutingInterpreter(), Vehicle.Car);
            stream.Dispose();

            return CHEdgeGraphFileStreamTargetTests.TestSerialization(name, pbfFile, data);
        }

        public static Stream TestSerialization(string name, string pbfFile, RouterDataSource<CHEdgeData> data)
        {
            var testOutputFile = new FileInfo(@"test.routing");
            testOutputFile.Delete();
            var writeStream = testOutputFile.OpenWrite();

            var performanceInfo = new PerformanceInfoConsumer("CHSerializer");
            performanceInfo.Start();
            performanceInfo.Report("Writing to {0}...", testOutputFile.Name);

            TagsCollectionBase metaData = new TagsCollection();
            metaData.Add("some_key", "some_value");
            var routingSerializer = new CHEdgeSerializer();
            routingSerializer.Serialize(writeStream, data, metaData);

            writeStream.Dispose();

            OsmSharp.Logging.Log.TraceEvent("CHSerializer", OsmSharp.Logging.TraceEventType.Information,
                string.Format("Serialized file: {0}KB", testOutputFile.Length / 1024));

            performanceInfo.Stop();

            return testOutputFile.OpenRead();
        }
    }
}