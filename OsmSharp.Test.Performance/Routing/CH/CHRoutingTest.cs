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

using OsmSharp.Collections.Tags.Index;
using OsmSharp.Logging;
using OsmSharp.Math.Geo;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;
using System.IO;

namespace OsmSharp.Test.Performance.Routing.CH
{
    /// <summary>
    /// Contains test for the CH routing.
    /// </summary>
    public static class CHRoutingTest
    {
        /// <summary>
        /// Tests the live routing.
        /// </summary>
        public static void Test()
        {
            var box = new GeoCoordinateBox(
                new GeoCoordinate(51.20190, 4.66540),
                new GeoCoordinate(51.30720, 4.89820));
            CHRoutingTest.TestRouting("CHRouting", 
                "kempen-big.osm.pbf", box, 2500);
        }

        /// <summary>
        /// Tests routing from a serialized routing file.
        /// </summary>
        public static void Test(Stream stream, int testCount)
        {
            var box = new GeoCoordinateBox(
                new GeoCoordinate(51.20190, 4.66540),
                new GeoCoordinate(51.30720, 4.89820));
            CHRoutingTest.TestRouting("CHRouting",
                stream, box, testCount);
        }

        /// <summary>
        /// Tests routing from a serialized routing file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stream"></param>
        /// <param name="box"></param>
        /// <param name="testCount"></param>
        public static void TestRouting(string name, Stream stream,
            GeoCoordinateBox box, int testCount)
        {
            var vehicle = Vehicle.Car;

            var tagsIndex = new TagsTableCollectionIndex(); // creates a tagged index.

            // read from the OSM-stream.
            var data = CHEdgeGraphOsmStreamTarget.Preprocess(
                new OsmSharp.Osm.PBF.Streams.PBFOsmStreamSource(stream),
                new OsmRoutingInterpreter(), vehicle);

            var router = Router.CreateCHFrom(data, new CHRouter(), new OsmRoutingInterpreter());

            var performanceInfo = new PerformanceInfoConsumer("CHRouting");
            performanceInfo.Start();
            performanceInfo.Report("Routing {0} routes...", testCount);

            int successCount = 0;
            int totalCount = testCount;
            float latestProgress = -1;
            while (testCount > 0)
            {
                var from = box.GenerateRandomIn();
                var to = box.GenerateRandomIn();

                var fromPoint = router.Resolve(vehicle, from);
                var toPoint = router.Resolve(vehicle, to);

                if (fromPoint != null && toPoint != null)
                {
                    Route route = null;
                    try
                    {
                        route = router.Calculate(vehicle, fromPoint, toPoint);
                    }
                    catch
                    {

                    }
                    if (route != null)
                    {
                        successCount++;
                    }
                }

                testCount--;

                // report progress.
                float progress = (float)System.Math.Round(((double)(totalCount - testCount)  / (double)totalCount) * 100);
                if (progress != latestProgress)
                {
                    OsmSharp.Logging.Log.TraceEvent("CHRouting", TraceEventType.Information,
                        "Routing... {0}%", progress);
                    latestProgress = progress;
                }
            }
            performanceInfo.Stop();

            OsmSharp.Logging.Log.TraceEvent("CHRouting", OsmSharp.Logging.TraceEventType.Information,
                string.Format("{0}/{1} routes successfull!", successCount, totalCount));
        }

        /// <summary>
        /// Tests routing from a serialized routing file.
        /// </summary>
        public static void TestRouting(string name, string osmPbfFile, 
            GeoCoordinateBox box, int testCount)
        {
            var testFile = new FileInfo(string.Format(@".\TestFiles\{0}", osmPbfFile));
            var stream = testFile.OpenRead();

            CHRoutingTest.TestRouting(name, stream, box, testCount);

            stream.Dispose();
        }
    }
}