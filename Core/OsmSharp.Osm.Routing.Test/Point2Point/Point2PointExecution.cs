using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OsmSharp.Osm.Routing.Test.Point2Point
{
    /// <summary>
    /// Executes the many-to-many tests.
    /// </summary>
    public static class Point2PointExecution
    {
        /// <summary>
        /// Executes the actual many-to-many tests.
        /// </summary>
        public static void Execute()
        {
            Point2Point.Point2PointRawTests raw_tests = new Point2Point.Point2PointRawTests();
            //raw_tests.ExecuteTest("tiny", 100);
            //raw_tests.ExecuteTest("matrix", 100);
            //raw_tests.ExecuteTest("matrix_big_area", 100);
            //raw_tests.ExecuteTest("eeklo", 100);
            //raw_tests.ExecuteTest("moscow", 100);
            //raw_tests.ExecuteTest("matrix_big_area", 100);
            //raw_tests.ExecuteTest("lebbeke", 100);
           
            //raw_tests.ExecuteTestIncrementalBoundingBox("eeklo", 100, new OsmSharp.Tools.Math.Geo.GeoCoordinateBox(
            //    new OsmSharp.Tools.Math.Geo.GeoCoordinate(51.10800,3.46400),
            //    new OsmSharp.Tools.Math.Geo.GeoCoordinate(51.24100,3.67300)));
            
            //raw_tests.ExecuteTestIncrementalBoundingBox("moscow", 250, new OsmSharp.Tools.Math.Geo.GeoCoordinateBox(
            //    new OsmSharp.Tools.Math.Geo.GeoCoordinate(55.42577,37.11831),
            //    new OsmSharp.Tools.Math.Geo.GeoCoordinate(56.05624,37.97562)));

            //raw_tests.ExecuteTestIncrementalBoundingBox("matrix_big_area", 250, new OsmSharp.Tools.Math.Geo.GeoCoordinateBox(
            //    new OsmSharp.Tools.Math.Geo.GeoCoordinate(50.93000, 3.48700),
            //    new OsmSharp.Tools.Math.Geo.GeoCoordinate(51.12400, 3.90700)));
            
            raw_tests.ExecuteTestIncrementalBoundingBox("belgium",
                (new FileInfo(@"C:\OSM\bin\belgium.osm.pbf")).OpenRead(), true, 25, new OsmSharp.Tools.Math.Geo.GeoCoordinateBox(
                new OsmSharp.Tools.Math.Geo.GeoCoordinate(51.182786,3.388367),
                new OsmSharp.Tools.Math.Geo.GeoCoordinate(50.562304, 5.640564)));
        }
    }
}
