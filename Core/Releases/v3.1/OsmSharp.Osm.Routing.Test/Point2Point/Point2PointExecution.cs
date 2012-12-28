using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OsmSharp.Routing.Core.Graph.DynamicGraph;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Routing.Core.Graph.DynamicGraph.PreProcessed;
using OsmSharp.Routing.Core.Graph.DynamicGraph.SimpleWeighed;

namespace OsmSharp.Osm.Routing.Test.Point2Point
{
    /// <summary>
    /// Executes the many-to-many tests.
    /// </summary>
    static class Point2PointExecution
    {
        /// <summary>
        /// Executes the actual point-to-point tests.
        /// </summary>
        public static void Execute()
        {
            //Point2PointExecution.Execute<PreProcessedEdge>(
            //    new Point2Point.Point2PointDykstraPreProcessedTests());
            Point2PointExecution.Execute<SimpleWeighedEdge>(
                new Point2Point.Point2PointDykstraLiveTests());
            Point2PointExecution.Execute<OsmSharp.Routing.CH.PreProcessing.CHEdgeData>(
                new Point2Point.Point2PointCHTests());
        }

        /// <summary>
        /// Executes the testers.
        /// </summary>
        /// <typeparam name="EdgeData"></typeparam>
        /// <param name="tester"></param>
        static void Execute<EdgeData>(Point2PointTest<EdgeData> tester)
            where EdgeData : IDynamicGraphEdgeData
        {
            int test_count = 1000;

            //tester.ExecuteComparisonTest("matrix", 
            //    new GeoCoordinate(51.01302, 4.005095), new GeoCoordinate(51.01415, 4.006043));

            //tester.ExecuteComparisonTests("tiny", test_count);
            //tester.ExecuteComparisonTests("matrix", 1000);
            //tester.ExecuteComparisonTests("eeklo", 100);
            //tester.ExecuteComparisonTests("matrix_big_area", 100);
            //tester.ExecuteComparisonTests("moscow", 100);

            //tester.ExecuteTest("tiny", test_count);
            //tester.ExecuteTest("matrix", test_count);
            //tester.ExecuteTest("eeklo", test_count);
            //tester.ExecuteTest("lebbeke", test_count);
            //tester.ExecuteTest("matrix_big_area", test_count);
            //tester.ExecuteTest("moscow", test_count);
            //tester.ExecuteTest("flanders_highway", (new FileInfo(@"c:\OSM\bin\flanders_highway.osm.pbf")).OpenRead(),
            //    true, test_count);

            tester.ExecuteTestIncrementalBoundingBox("eeklo", 100, new OsmSharp.Tools.Math.Geo.GeoCoordinateBox(
                new OsmSharp.Tools.Math.Geo.GeoCoordinate(51.10800, 3.46400),
                new OsmSharp.Tools.Math.Geo.GeoCoordinate(51.24100, 3.67300)));

            //tester.ExecuteTestIncrementalBoundingBox("moscow", 250, new OsmSharp.Tools.Math.Geo.GeoCoordinateBox(
            //    new OsmSharp.Tools.Math.Geo.GeoCoordinate(55.42577, 37.11831),
            //    new OsmSharp.Tools.Math.Geo.GeoCoordinate(56.05624, 37.97562)));

            //tester.ExecuteTestIncrementalBoundingBox("matrix_big_area", 250, new OsmSharp.Tools.Math.Geo.GeoCoordinateBox(
            //    new OsmSharp.Tools.Math.Geo.GeoCoordinate(50.93000, 3.48700),
            //    new OsmSharp.Tools.Math.Geo.GeoCoordinate(51.12400, 3.90700)));

            //tester.ExecuteTestIncrementalBoundingBox("belgium",
            //    (new FileInfo(@"C:\OSM\bin\belgium.osm.pbf")).OpenRead(), true, 25, new OsmSharp.Tools.Math.Geo.GeoCoordinateBox(
            //    new OsmSharp.Tools.Math.Geo.GeoCoordinate(51.182786, 3.388367),
            //    new OsmSharp.Tools.Math.Geo.GeoCoordinate(50.562304, 5.640564)));
        }
    }
}
