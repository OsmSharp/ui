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

using OsmSharp.Collections.Tags.Index;
using OsmSharp.Logging;
using OsmSharp.Math.Geo;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Interpreter;
using System.IO;
using OsmSharp.Routing.Vehicles;
using OsmSharp.Routing.Osm.Streams;

namespace OsmSharp.Test.Performance.Routing.CH
{
    /// <summary>
    /// Contains test for the resolving code.
    /// </summary>
    public static class RoutingResolveTest
    {
        /// <summary>
        /// Tests the resolve for routing.
        /// </summary>
        /// <param name="box"></param>
        public static void Test(GeoCoordinateBox box)
        {
            RoutingResolveTest.TestResolved("CHRoutingResolveTest", "kempen-big.osm.pbf", box, 10000);
        }

        /// <summary>
        /// Tests routing from a serialized routing file.
        /// </summary>
        public static void TestResolved(string name, string osmPbfFile, GeoCoordinateBox box, int testCount)
        {
            var testFile = new FileInfo(string.Format(@".\TestFiles\{0}", osmPbfFile));
            var stream = testFile.OpenRead();

            RoutingResolveTest.TestResolved(name, stream, box, testCount);

            stream.Dispose();
        }

        /// <summary>
        /// Tests routing from a serialized routing file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stream"></param>
        /// <param name="testCount"></param>
        public static void TestResolved(string name, Stream stream, GeoCoordinateBox box, int testCount)
        {
            var vehicle = Vehicle.Car;

            var tagsIndex = new TagsIndex(); // creates a tagged index.

            // read from the OSM-stream.
            var source = new OsmSharp.Osm.Streams.Filters.OsmStreamFilterProgress();
            source.RegisterSource(new OsmSharp.Osm.PBF.Streams.PBFOsmStreamSource(stream));
            var data = GraphOsmStreamTarget.Preprocess(source,
                new OsmRoutingInterpreter());

            //(data.Graph as DirectedGraph<CHEdgeData>).Compress(true);

            RoutingResolveTest.TestResolved(data, testCount, box);
        }

        ///// <summary>
        ///// Tests routing from a serialized file.
        ///// </summary>
        ///// <param name="stream"></param>
        ///// <param name="box"></param>
        ///// <param name="lazy"></param>
        ///// <param name="testCount"></param>
        //public static void TestSerializedResolved(Stream stream, GeoCoordinateBox box, bool lazy = true, int testCount = 100)
        //{
        //    var routingSerializer = new EdgeSerializer();
        //    var data = routingSerializer.Deserialize(stream, lazy);

        //    CHRoutingResolveTest.TestResolved(data, testCount, box);
        //}

        public static void TestResolved(RouterDataSource<Edge> data, int testCount, GeoCoordinateBox box)
        {
            var router = Router.CreateFrom(data, new OsmRoutingInterpreter());

            var performanceInfo = new PerformanceInfoConsumer("CHRouting");
            performanceInfo.Start();
            performanceInfo.Report("Routing {0} routes...", testCount);

            var successCount = 0;
            var totalCount = testCount;
            var latestProgress = -1.0f;
            while (testCount > 0)
            {
                var from = box.GenerateRandomIn();
                //var to = box.GenerateRandomIn();

                var fromPoint = router.Resolve(Vehicle.Car, from);
                // var toPoint = router.Resolve(Vehicle.Car, to);

                if (fromPoint != null)
                {
                    //var route = router.Calculate(Vehicle.Car, fromPoint, toPoint);
                    //if (route != null)
                    //{
                    successCount++;
                    //}
                }
                testCount--;

                // report progress.
                var progress = (float)System.Math.Round(((double)(totalCount - testCount) / (double)totalCount) * 100);
                if (progress != latestProgress)
                {
                    OsmSharp.Logging.Log.TraceEvent("CHRouting", TraceEventType.Information,
                        "Resolving... {0}%", progress);
                    latestProgress = progress;
                }
            }
            performanceInfo.Stop();

            OsmSharp.Logging.Log.TraceEvent("CHRouting", OsmSharp.Logging.TraceEventType.Information,
                string.Format("{0}/{1} resolves successfull!", successCount, totalCount));
        }
    }
}