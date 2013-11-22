using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using OsmSharp.Routing;
using NUnit.Framework;
using System.IO;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Collections.Tags;
using OsmSharp.Routing.Graph;
using OsmSharp.IO.DelimitedFiles;
using OsmSharp.Routing.Routers;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.TSP;
using OsmSharp.Routing.TSP.Genetic;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Osm.Xml.Streams;

namespace OsmSharp.Test.Unittests.Routing.TSP
{
    /// <summary>
    /// Some tests on the TSP wrapper.
    /// </summary>
    [TestFixture]
    public class TSPWrapperTest
    {
        /// <summary>
        /// Tests the vehicle type of the resulting route.
        /// </summary>
        [Test]
        public void TestTSPWrapperVehicle()
        {
            // calculate TSP.
            Route route = this.CalculateTSP(Assembly.GetExecutingAssembly()
                                                            .GetManifestResourceStream(
                                                                @"OsmSharp.Test.Unittests.tsp_real.osm"),
                                                    Assembly.GetExecutingAssembly()
                                                            .GetManifestResourceStream(
                                                                @"OsmSharp.Test.Unittests.tsp_real.csv"),
                                                    false,
                                                    Vehicle.Car);

            Assert.IsNotNull(route);
            Assert.AreEqual(Vehicle.Car, route.Vehicle);
        }

        /// <summary>
        /// Tests the vehicle type of the resulting route.
        /// </summary>
        [Test]
        public void TestTSPWrapperMetric()
        {
            // calculate TSP.
            Route route = this.CalculateTSP(Assembly.GetExecutingAssembly()
                                                            .GetManifestResourceStream(
                                                                @"OsmSharp.Test.Unittests.tsp_real.osm"),
                                                    Assembly.GetExecutingAssembly()
                                                            .GetManifestResourceStream(
                                                                @"OsmSharp.Test.Unittests.tsp_real.csv"),
                                                    false,
                                                    Vehicle.Car);

            Assert.IsNotNull(route);
            Assert.AreNotEqual(0, route.TotalDistance);
            Assert.AreNotEqual(0, route.TotalTime);
        }

        /// <summary>
        /// Tests the a calculation with just one point.
        /// </summary>
        [Test]
        public void TestTSPWrapperOne()
        {
            // calculate TSP.
            Route route = this.CalculateTSP(Assembly.GetExecutingAssembly()
                                                            .GetManifestResourceStream(
                                                                @"OsmSharp.Test.Unittests.tsp_real.osm"),
                                                    Assembly.GetExecutingAssembly()
                                                            .GetManifestResourceStream(
                                                                @"OsmSharp.Test.Unittests.tsp_one.csv"),
                                                    false,
                                                    Vehicle.Car);

            Assert.IsNotNull(route);
        }

        /// <summary>
        /// Tests the a calculation with just two points.
        /// </summary>
        [Test]
        public void TestTSPWrapperTwo()
        {
            // calculate TSP.
            Route route = this.CalculateTSP(Assembly.GetExecutingAssembly()
                                                            .GetManifestResourceStream(
                                                                @"OsmSharp.Test.Unittests.tsp_real.osm"),
                                                    Assembly.GetExecutingAssembly()
                                                            .GetManifestResourceStream(
                                                                @"OsmSharp.Test.Unittests.tsp_two.csv"),
                                                    false,
                                                    Vehicle.Car);

            Assert.IsNotNull(route);
        }

        /// <summary>
        /// Calculates the TSP.
        /// </summary>
        /// <param name="dataStream"></param>
        /// <param name="csvStream"></param>
        /// <param name="pbf"></param>
        /// <param name="vehicleEnum"></param>
        /// <returns></returns>
        private Route CalculateTSP(Stream dataStream, Stream csvStream, bool pbf, Vehicle vehicleEnum)
        {
            // create the router.
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            Router router = Router.CreateLiveFrom(
                new XmlOsmStreamSource(dataStream), interpreter);

            // read the source files.
            const int latitudeIdx = 2;
            const int longitudeIdx = 3;
            string[][] pointStrings = DelimitedFileHandler.ReadDelimitedFileFromStream(
                csvStream,
                DelimiterType.DotCommaSeperated);
            var points = new List<RouterPoint>();
            int cnt = 10;
            foreach (string[] line in pointStrings)
            {
                if (points.Count >= cnt)
                {
                    break;
                }
                var latitudeString = (string)line[latitudeIdx];
                var longitudeString = (string)line[longitudeIdx];

                //string route_ud = (string)line[1];

                double longitude = 0;
                double latitude = 0;
                if (double.TryParse(longitudeString, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out longitude) &&
                   double.TryParse(latitudeString, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out latitude))
                {
                    var point = new GeoCoordinate(latitude, longitude);

                    RouterPoint resolved = router.Resolve(Vehicle.Car, point);
                    if (resolved != null && router.CheckConnectivity(Vehicle.Car, resolved, 100))
                    {
                        points.Add(resolved);
                    }
                }
            }

            var tspSolver = new RouterTSPWrapper<RouterTSP>(
                new RouterTSPAEXGenetic(), router, interpreter);
            return tspSolver.CalculateTSP(vehicleEnum, points.ToArray());
        }
    }
}
