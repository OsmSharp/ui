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

using OsmSharp.Routing;
using System.IO;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Instructions;
using System.Collections.Generic;
using OsmSharp.Collections.Tags;
using OsmSharp.Logging;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Routing;
using OsmSharp.Routing.Osm.Streams;
using OsmSharp.Routing.Graph.Serialization;
using OsmSharp.Routing.Vehicles;

namespace OsmSharp.Test.Performance.Routing
{
    /// <summary>
    /// Contains test for the CH routing.
    /// </summary>
    public static class RoutingTest
    {
        /// <summary>
        /// Tests routing from a raw OSM-file.
        /// </summary>
        public static void Test()
        {
            RoutingTest.Test("Routing",
                "kempen-big.osm.pbf", 50);
        }

        /// <summary>
        /// Tests routing from a raw OSM-file.
        /// </summary>
        public static void Test(Stream stream, int testCount)
        {
            RoutingTest.Test("Routing",
                stream, testCount);
        }

        /// <summary>
        /// Tests routing from a raw OSM-file.
        /// </summary>
        public static void Test(string name, string routeFile, int testCount)
        {
            var testFile = new FileInfo(string.Format(@".\TestFiles\{0}", routeFile));
            var stream = testFile.OpenRead();

            RoutingTest.Test(name, stream, testCount);

            stream.Dispose();
        }

        /// <summary>
        /// Tests routing from a serialized routing file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stream"></param>
        /// <param name="testCount"></param>
        public static void Test(string name, Stream stream, int testCount)
        {
            var vehicle = Vehicle.Pedestrian;

            var tagsIndex = new TagsIndex(); // creates a tagged index.

            // read from the OSM-stream.
            var reader = new OsmSharp.Osm.PBF.Streams.PBFOsmStreamSource(stream);
            var interpreter = new OsmRoutingInterpreter();
            var data = new RouterDataSource<Edge>(new Graph<Edge>(), tagsIndex);
            data.DropVertexIndex();
            var targetData = new GraphOsmStreamTarget(data, interpreter, tagsIndex);
            targetData.RegisterSource(reader);
            targetData.Pull();
            data.RebuildVertexIndex();

            RoutingTest.Test(data, vehicle, testCount);
        }

        /// <summary>
        /// Tests routing from a serialized file.
        /// </summary>
        /// <param name="stream"></param>
        public static void TestSerialized(Stream stream, bool lazy = true)
        {
            var routingSerializer = new RoutingDataSourceSerializer();
            var data = routingSerializer.Deserialize(stream, lazy);

            uint vertex = 1;
            uint significantVertices = 0;
            while(vertex < data.VertexCount)
            {
                var edges =  data.GetEdges(vertex).ToKeyValuePairs();
                if(edges.Length > 2)
                {
                    significantVertices++;
                }
                vertex++;
            }

            data.SortHilbert(1000);

            // copy.
            var graphCopy = new Graph<Edge>();
            graphCopy.CopyFrom(data);
            var dataCopy = new RouterDataSource<Edge>(graphCopy, data.TagsIndex);

            RoutingTest.Test(dataCopy, Vehicle.Pedestrian, 50);
        }

        public static void Test(IRoutingAlgorithmData<Edge> data, Vehicle vehicle, int testCount)
        {
            // creates the edge router.
            var router = new Dykstra();
            var interpreter = new OsmRoutingInterpreter();

            var performanceInfo = new PerformanceInfoConsumer("Routing");
            performanceInfo.Start();
            performanceInfo.Report("Routing {0} routes...", testCount);

            var successCount = 0;
            var totalCount = testCount;
            var latestProgress = -1.0f;
            while (testCount > 0)
            {
                var from = (uint)OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(data.VertexCount - 1) + 1;
                var to = (uint)OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(data.VertexCount - 1) + 1;

                var route = router.Calculate(data, interpreter, vehicle, from, to);

                if (route != null)
                {
                    successCount++;
                }
                testCount--;

                // report progress.
                var progress = (float)System.Math.Round(((double)(totalCount - testCount) / (double)totalCount) * 100);
                if (progress != latestProgress)
                {
                    OsmSharp.Logging.Log.TraceEvent("Routing", TraceEventType.Information,
                        "Routing... {0}%", progress);
                    latestProgress = progress;
                }
            }
            performanceInfo.Stop();

            OsmSharp.Logging.Log.TraceEvent("Routing", OsmSharp.Logging.TraceEventType.Information,
                string.Format("{0}/{1} routes successfull!", successCount, totalCount));
        }
    }
}