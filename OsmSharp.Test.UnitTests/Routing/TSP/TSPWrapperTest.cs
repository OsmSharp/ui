// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2014 Abelshausen Ben
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

using NUnit.Framework;
using OsmSharp.IO.DelimitedFiles;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Optimization.TSP;
using OsmSharp.Routing.Optimization.TSP.Genetic;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using OsmSharp.Routing.Vehicles;

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
            var route = this.CalculateTSP(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(@"OsmSharp.Test.Unittests.tsp_real.osm"),
                Assembly.GetExecutingAssembly().GetManifestResourceStream(@"OsmSharp.Test.Unittests.tsp_real.csv"),
                false, Vehicle.Car);

            Assert.IsNotNull(route);
            Assert.AreEqual(Vehicle.Car.UniqueName, route.Vehicle);
        }

        /// <summary>
        /// Tests the vehicle type of the resulting route.
        /// </summary>
        [Test]
        public void TestTSPWrapperMetric()
        {
            // calculate TSP.
            var route = this.CalculateTSP(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(@"OsmSharp.Test.Unittests.tsp_real.osm"),
                Assembly.GetExecutingAssembly().GetManifestResourceStream(@"OsmSharp.Test.Unittests.tsp_real.csv"),
                false, Vehicle.Car);

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
            var route = this.CalculateTSP(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(@"OsmSharp.Test.Unittests.tsp_real.osm"),
                Assembly.GetExecutingAssembly().GetManifestResourceStream(@"OsmSharp.Test.Unittests.tsp_one.csv"),
                false, Vehicle.Car);

            Assert.IsNotNull(route);
        }

        /// <summary>
        /// Tests the a calculation with just two points.
        /// </summary>
        [Test]
        public void TestTSPWrapperTwo()
        {
            // calculate TSP.
            var route = this.CalculateTSP(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(@"OsmSharp.Test.Unittests.tsp_real.osm"),
                Assembly.GetExecutingAssembly().GetManifestResourceStream(@"OsmSharp.Test.Unittests.tsp_two.csv"),
                false, Vehicle.Car);

            Assert.IsNotNull(route);
        }

        /// <summary>
        /// Calculates the TSP.
        /// </summary>
        /// <param name="dataStream"></param>
        /// <param name="csvStream"></param>
        /// <param name="pbf"></param>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        private Route CalculateTSP(Stream dataStream, Stream csvStream, bool pbf, Vehicle vehicle)
        {
            // create the router.
            var interpreter = new OsmRoutingInterpreter();
            var router = Router.CreateFrom(
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

            // test if routes exists.
            var weights = router.CalculateManyToManyWeight(Vehicle.Car, points.ToArray(), points.ToArray());
            for(int fromIdx = 0; fromIdx < points.Count; fromIdx++)
            {
                for (int toIdx = 0; toIdx < points.Count; toIdx++)
                {
                    var weight = router.CalculateWeight(Vehicle.Car, points[fromIdx], points[toIdx]);
                    Assert.AreEqual(weight, weights[fromIdx][toIdx]);
                }
            }

            // create and execute solver.
            var tspSolver = new RouterTSPWrapper<RouterTSP>(
                new RouterTSPAEXGenetic(), router, interpreter);
            return tspSolver.CalculateTSP(vehicle, points.ToArray());
        }
    }
}
