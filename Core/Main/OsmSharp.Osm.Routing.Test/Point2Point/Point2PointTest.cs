// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using System.Linq;
using System.Text;
using System.IO;
using OsmSharp.Tools.Math.Geo;
using System.Data;
using OsmSharp.Osm.Data;
using OsmSharp.Osm.Core.Xml;
using OsmSharp.Tools.Xml.Sources;
using OsmSharp.Osm.Data.Raw.XML.OsmSource;
using OsmSharp.Routing.Core;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Routing.Core.Router;
using OsmSharp.Routing.Core.Graph;
using OsmSharp.Osm.Core;
using OsmSharp.Osm.Routing.Interpreter;
using System.Reflection;
using System.Diagnostics;
using OsmSharp.Routing.Core.Graph.DynamicGraph;

namespace OsmSharp.Osm.Routing.Test.Point2Point
{
    internal abstract class Point2PointTest<EdgeData> 
        where EdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Executes some general random query performance evaluation(s).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="test_count"></param>
        public void ExecuteTest(string name, int test_count)
        {
            string xml_embedded = string.Format("OsmSharp.Osm.Routing.Test.TestData.{0}.osm", name);

            this.ExecuteTest(name, Assembly.GetExecutingAssembly().GetManifestResourceStream(xml_embedded), false, test_count);
        }

        /// <summary>
        /// Executes some general random query performance evaluation(s).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data_stream"></param>
        /// <param name="pbf"></param>
        /// <param name="test_count"></param>
        public void ExecuteTest(string name, Stream data_stream, bool pbf, int test_count)
        {
            OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Test {0} -> {1}x", name, test_count);

            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            IBasicRouterDataSource<EdgeData> data = this.BuildData(data_stream, pbf,
                interpreter, null);

            // build router;
            IRouter<RouterPoint> router = this.BuildRouter(data, interpreter);

            // generate random route pairs.
            List<KeyValuePair<RouterPoint, RouterPoint>> test_pairs =
                new List<KeyValuePair<RouterPoint, RouterPoint>>(test_count);
            while (test_pairs.Count < test_count)
            {
                uint first = (uint)OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(
                    (int)data.VertexCount - 1) + 1;
                uint second = (uint)OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(
                    (int)data.VertexCount - 1) + 1;

                float latitude_first, longitude_first;
                data.GetVertex(first, out latitude_first, out longitude_first);
                RouterPoint first_resolved = router.Resolve(
                    new GeoCoordinate(latitude_first, longitude_first));

                float latitude_second, longitude_second;
                data.GetVertex(second, out latitude_second, out longitude_second);
                RouterPoint second_resolved = router.Resolve(
                    new GeoCoordinate(latitude_second, longitude_second));


                if (((second_resolved != null) &&
                    (first_resolved != null)) &&
                    (router.CheckConnectivity(first_resolved, 30) &&
                    router.CheckConnectivity(second_resolved, 30)))
                {
                    test_pairs.Add(new KeyValuePair<RouterPoint, RouterPoint>(
                        first_resolved, second_resolved));
                }

                OsmSharp.Tools.Core.Output.OutputStreamHost.ReportProgress(test_pairs.Count, test_count, "Osm.Routing.Test.Point2Point.Point2PointTest<EdgeData>.Execute",
                    "Building pairs list...");
            }

            int successes = 0;
            long before = DateTime.Now.Ticks;
            foreach (KeyValuePair<RouterPoint, RouterPoint> pair in test_pairs)
            {
                OsmSharp.Routing.Core.Route.OsmSharpRoute route = router.Calculate(pair.Key, pair.Value);
                if (route != null)
                {
                    successes++;
                }
            }
            long after = DateTime.Now.Ticks;
            OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine();
            OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine(string.Format("Average calculation time for {0} random routes: {1}ms with {2} successes",
                test_count, (new TimeSpan((after - before) / test_count)).TotalMilliseconds.ToString(), successes));
        }

        /// <summary>
        /// Executes some general random query performance evaluation(s).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="test_count"></param>
        public void ExecuteTestIncrementalBoundingBox(string name, int test_count, GeoCoordinateBox outer_box)
        {
            string xml_embedded = string.Format("OsmSharp.Osm.Routing.Test.TestData.{0}.osm", name);

            this.ExecuteTestIncrementalBoundingBox(name, Assembly.GetExecutingAssembly().GetManifestResourceStream(xml_embedded), false, test_count,
                outer_box);
        }

        /// <summary>
        /// Executes a test using a incrementing bounding box.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data_stream"></param>
        /// <param name="pbf"></param>
        /// <param name="test_count"></param>
        /// <param name="outer_box"></param>
        public void ExecuteTestIncrementalBoundingBox(string name, Stream data_stream, bool pbf, int test_count, GeoCoordinateBox outer_box)
        {
            OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Incremental Test {0} -> {1}x", name, test_count);

            int box_count = 20;
            for (int idx = 1; idx < box_count; idx++)
            {
                GeoCoordinateBox box = new GeoCoordinateBox(
                        new GeoCoordinate(
                    outer_box.Center.Latitude - ((outer_box.DeltaLat / (float)box_count) * (float)idx),
                    outer_box.Center.Longitude - ((outer_box.DeltaLon / (float)box_count) * (float)idx)),
                        new GeoCoordinate(
                    outer_box.Center.Latitude + ((outer_box.DeltaLat / (float)box_count) * (float)idx),
                    outer_box.Center.Longitude + ((outer_box.DeltaLon / (float)box_count) * (float)idx)));

                OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
                OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("Testing for a box with total surface of {0}m²",
                    box.Corners[0].DistanceReal(box.Corners[1]).Value * box.Corners[0].DistanceReal(box.Corners[2]).Value);
                IBasicRouterDataSource<EdgeData> data = this.BuildData(data_stream, pbf,
                    interpreter, box);

                this.DoExecuteTests(data, interpreter, test_count);
            }
        }

        /// <summary>
        /// Executes the actual tests.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="interpreter"></param>
        /// <param name="test_count"></param>
        private void DoExecuteTests(IBasicRouterDataSource<EdgeData> data, OsmRoutingInterpreter interpreter, int test_count)
        {
            if (data.VertexCount == 0)
            {
                OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine("0 vertices in data source!");
                return;
            }

            // build router;
            IRouter<RouterPoint> router = this.BuildRouter(data, interpreter);

            // generate random route pairs.
            List<KeyValuePair<RouterPoint, RouterPoint>> test_pairs =
                new List<KeyValuePair<RouterPoint, RouterPoint>>(test_count);
            while (test_pairs.Count < test_count)
            {
                uint first = (uint)OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(
                    (int)data.VertexCount - 1) + 1;
                uint second = (uint)OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(
                    (int)data.VertexCount - 1) + 1;

                float latitude_first, longitude_first;
                data.GetVertex(first, out latitude_first, out longitude_first);
                RouterPoint first_resolved = router.Resolve(
                    new GeoCoordinate(latitude_first, longitude_first));

                float latitude_second, longitude_second;
                data.GetVertex(second, out latitude_second, out longitude_second);
                RouterPoint second_resolved = router.Resolve(
                    new GeoCoordinate(latitude_second, longitude_second));

                if (((second_resolved != null) &&
                    (first_resolved != null)) && 
                    (router.CheckConnectivity(first_resolved, 30) &&
                    router.CheckConnectivity(second_resolved, 30)))
                {
                    test_pairs.Add(new KeyValuePair<RouterPoint, RouterPoint>(
                        first_resolved, second_resolved));
                }

                OsmSharp.Tools.Core.Output.OutputStreamHost.ReportProgress(test_pairs.Count, test_count, "Osm.Routing.Test.Point2Point.Point2PointTest<EdgeData>.Execute",
                    "Building pairs list...");
            }

            long before = DateTime.Now.Ticks;
            for(int idx = 0; idx < test_pairs.Count; idx++)
            {
                KeyValuePair<RouterPoint, RouterPoint> pair = test_pairs[idx];
                router.Calculate(pair.Key, pair.Value);

                OsmSharp.Tools.Core.Output.OutputStreamHost.ReportProgress(idx, test_pairs.Count, "Osm.Routing.Test.Point2Point.Point2PointTest<EdgeData>.Execute",
                    "Routing pairs...");
            }
            long after = DateTime.Now.Ticks;
            OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine();
            OsmSharp.Tools.Core.Output.OutputStreamHost.WriteLine(string.Format("Average calculation time for {0} random routes: {1}ms",
                test_count, (new TimeSpan((after - before) / test_count)).TotalMilliseconds.ToString()));
        }

        /// <summary>
        /// Builds a router.
        /// </summary>
        /// <returns></returns>
        public abstract IRouter<RouterPoint> BuildRouter(IBasicRouterDataSource<EdgeData> data,
            IRoutingInterpreter interpreter);

        /// <summary>
        /// Builds data source.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        public abstract IBasicRouterDataSource<EdgeData> BuildData(Stream data, bool pbf, IRoutingInterpreter interpreter, GeoCoordinateBox box);

    }
}
