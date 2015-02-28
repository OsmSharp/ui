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
using OsmSharp.Routing.CH;
using OsmSharp.Routing.Instructions;
using System.Collections.Generic;
using OsmSharp.Collections.Tags;
using OsmSharp.Routing.CH.Serialization;
using OsmSharp.Routing.Vehicles;

namespace OsmSharp.Test.Performance.Routing.CH
{
    /// <summary>
    /// Contains test for the CH routing.
    /// </summary>
    public static class CHSerializedRoutingTest
    {
        /// <summary>
        /// Tests the CH pre-processor.
        /// </summary>
        public static void Test()
        {
            var box = new GeoCoordinateBox(
                new GeoCoordinate(51.20190, 4.66540),
                new GeoCoordinate(51.30720, 4.89820));
            CHSerializedRoutingTest.TestSerializedRouting("CHSerializedRouting",
                "kempen-big.osm.pbf.contracted.mobile.routing", box, 2000);

            // test instructions.
            //CHSerializedRoutingTest.TestInstructions("CHSerializedRouting");
        }

        /// <summary>
        /// Tests the CH pre-processor.
        /// </summary>
        public static void Test(Stream stream)
        {
            CHSerializedRoutingTest.Test(
                stream, 1000);
        }

        /// <summary>
        /// Tests the instructions.
        /// </summary>
        public static void TestInstructions(string name)
        {
            var testFile = new FileInfo(string.Format(@".\TestFiles\routing\{0}",
                "kempen-big.osm.pbf.routing"));
            var stream = testFile.OpenRead();

            CHSerializedRoutingTest.TestSerializeRoutingInstrictions(
                name, stream,
                new GeoCoordinate(51.261203, 4.780760),
                new GeoCoordinate(51.267797, 4.801362));
        }

        /// <summary>
        /// Tests routing from a serialized routing file.
        /// </summary>
        public static void Test(Stream stream, int testCount)
        {
            var box = new GeoCoordinateBox(
                new GeoCoordinate(51.20190, 4.66540),
                new GeoCoordinate(51.30720, 4.89820));
            CHSerializedRoutingTest.TestSerializedRouting("CHSerializedRouting",
                stream, box, testCount);
        }

        /// <summary>
        /// Tests routing from a serialized routing file.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stream"></param>
        /// <param name="box"></param>
        /// <param name="testCount"></param>
        public static void TestSerializedRouting(string name, Stream stream,
            GeoCoordinateBox box, int testCount)
        {
            TagsCollectionBase metaData = null;
            var routingSerializer = new CHEdgeSerializer();
            var graphDeserialized = routingSerializer.Deserialize(
                stream, out metaData, false);

            var router = Router.CreateCHFrom(
                graphDeserialized, new CHRouter(),
                new OsmRoutingInterpreter());

            CHSerializedRoutingTest.TestRouting(router, box, testCount);
        }

        /// <summary>
        /// Tests routing within a bounding box.
        /// </summary>
        /// <param name="router"></param>
        /// <param name="box"></param>
        /// <param name="testCount"></param>
        public static void TestRouting(Router router, GeoCoordinateBox box, int testCount)
        {
            var performanceInfo = new PerformanceInfoConsumer("CHSerializedRouting");
            performanceInfo.Start();
            performanceInfo.Report("Routing {0} routes: Resolving...", testCount);
            int successCount = 0;
            int totalCount = testCount;
            var resolvedPoints = new List<RouterPoint>();
            while (testCount > 0)
            {
                var from = box.GenerateRandomIn();
                var to = box.GenerateRandomIn();

                var fromPoint = router.Resolve(Vehicle.Car, from);
                var toPoint = router.Resolve(Vehicle.Car, to);

                resolvedPoints.Add(fromPoint);
                resolvedPoints.Add(toPoint);

                testCount--;
            }
            performanceInfo.Stop();

            performanceInfo = new PerformanceInfoConsumer("CHSerializedRouting");
            performanceInfo.Start();
            performanceInfo.Report("Routing {0} routes: Routing...", testCount);
            for (int idx = 0; idx < resolvedPoints.Count; idx = idx + 2)
            {
                var fromPoint = resolvedPoints[idx];
                var toPoint = resolvedPoints[idx + 1];
                if (fromPoint != null && toPoint != null)
                {
                    var route = router.Calculate(Vehicle.Car, fromPoint, toPoint, float.MaxValue, true);
                    if (route != null)
                    {
                        successCount++;
                    }
                }
            }
            performanceInfo.Stop();

            OsmSharp.Logging.Log.TraceEvent("CHSerializedRouting", OsmSharp.Logging.TraceEventType.Information,
                string.Format("{0}/{1} routes successfull!", successCount, totalCount));
        }

        /// <summary>
        /// Tests routing from a serialized routing file.
        /// </summary>
        public static void TestSerializedRouting(string name, string routeFile,
            GeoCoordinateBox box, int testCount)
        {
            FileInfo testFile = new FileInfo(string.Format(@".\TestFiles\routing\{0}", routeFile));
            Stream stream = testFile.OpenRead();

            CHSerializedRoutingTest.TestSerializedRouting(name, stream, box, testCount);

            stream.Dispose();
        }

        /// <summary>
        /// Tests routing between two points and the associated instructions.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="stream"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void TestSerializeRoutingInstrictions(string name, Stream stream,
            GeoCoordinate from, GeoCoordinate to)
        {
            PerformanceInfoConsumer performanceInfo = new PerformanceInfoConsumer("CHSerializedRouting");
            performanceInfo.Start();
            performanceInfo.Report("Routing & generating instructions...");

            TagsCollectionBase metaData = null;
            var routingSerializer = new CHEdgeSerializer();
            var graphDeserialized = routingSerializer.Deserialize(
                stream, out metaData, true);

            var interpreter = new OsmRoutingInterpreter();
            var router = Router.CreateCHFrom(
                graphDeserialized, new CHRouter(),
                interpreter);

            var fromPoint = router.Resolve(Vehicle.Car, from);
            var toPoint = router.Resolve(Vehicle.Car, to);

            var instructions = new List<Instruction>();
            if (fromPoint != null && toPoint != null)
            {
                Route route = router.Calculate(Vehicle.Car, fromPoint, toPoint);
                if (route != null)
                {
                    instructions = InstructionGenerator.Generate(route, interpreter);
                }
            }

            performanceInfo.Stop();

            if (instructions.Count == 0)
            {
                OsmSharp.Logging.Log.TraceEvent("CHSerializedRouting", OsmSharp.Logging.TraceEventType.Information,
                    "Routing unsuccesfull!");
            }
            else
            {
                foreach (Instruction instruction in instructions)
                {
                    OsmSharp.Logging.Log.TraceEvent("CHSerializedRouting", OsmSharp.Logging.TraceEventType.Information,
                        instruction.Text);
                }
            }
        }
    }
}