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

using System.IO;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.CH.Serialization.Sorted.v2;
using OsmSharp.Routing.Osm.Streams.Graphs;

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
        public static void Test()
        {
            CHEdgeGraphFileStreamTargetTests.TestSerialization("CHSerializer", "kempen-big.osm.pbf");
        }

        /// <summary>
        /// Tests preprocessing data from a PBF file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pbfFile"></param>
        public static void TestSerialization(string name, string pbfFile)
        {
            FileInfo testFile = new FileInfo(string.Format(@".\TestFiles\{0}", pbfFile));
            Stream stream = testFile.OpenRead();
            PBFOsmStreamSource source = new PBFOsmStreamSource(stream);
            
            FileInfo testOutputFile = new FileInfo(@"test.routing");
            testOutputFile.Delete();
            Stream writeStream = testOutputFile.OpenWrite();

            CHEdgeGraphFileStreamTarget target = new CHEdgeGraphFileStreamTarget(writeStream,
                Vehicle.Car);
            target.RegisterSource(source);

            PerformanceInfoConsumer performanceInfo = new PerformanceInfoConsumer("CHSerializer");
            performanceInfo.Start();
            performanceInfo.Report("Pulling from {0}...", testFile.Name);

            var data = CHEdgeGraphOsmStreamTarget.Preprocess(
                source, new OsmRoutingInterpreter(), Vehicle.Car);

            var routingSerializer = new CHEdgeDataDataSourceSerializer(true);
            routingSerializer.Serialize(writeStream, data);

            stream.Dispose();
            writeStream.Dispose();

            OsmSharp.Logging.Log.TraceEvent("CHSerializer", OsmSharp.Logging.TraceEventType.Information,
                string.Format("Serialized file: {0}KB", testOutputFile.Length / 1024));

            performanceInfo.Stop();
        }
    }
}
