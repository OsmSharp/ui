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

using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using OsmSharp.Collections;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;
using OsmSharp.Collections.Tags.Index;

namespace OsmSharp.Test.Unittests.Routing
{
    /// <summary>
    /// Holds some regression tests of previously detected routing issues.
    /// </summary>
    [TestFixture]
    public class RoutingRegressionTests
    {
        /// <summary>
        /// Issue with resolved points and incorrect routing.
        /// Some routes will pass through the same point twice.
        /// </summary>
        [Test]
        public void RoutingRegressionTest1()
        {
            var interpreter = new OsmRoutingInterpreter();
            var tagsIndex = new TagsTableCollectionIndex();

            // do the data processing.
            var memoryData = new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var targetData = new LiveGraphOsmStreamTarget(memoryData, interpreter, tagsIndex);
            var dataProcessorSource = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Test.Unittests.test_routing_regression1.osm"));
            var sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(dataProcessorSource);
            targetData.RegisterSource(sorter);
            targetData.Pull();

            var basicRouter = new DykstraRoutingLive();
            var router = Router.CreateLiveFrom(memoryData, basicRouter, interpreter);

            // resolve the three points in question.
            var point35 = new GeoCoordinate(51.01257, 4.000753);
            var point35resolved = router.Resolve(Vehicle.Car, point35);
            var point45 = new GeoCoordinate(51.01315, 3.999588);
            var point45resolved = router.Resolve(Vehicle.Car, point45);

            // route between 35 and 45.
            var routebefore = router.Calculate(Vehicle.Car, point35resolved, point45resolved);

            // route between 35 and 45.
            var routeafter = router.Calculate(Vehicle.Car, point35resolved, point45resolved);
            Assert.AreEqual(routebefore.TotalDistance, routeafter.TotalDistance);
        }

        /// <summary>
        /// Issue with high-density resolved points. Routes are incorrectly skipping nodes.
        /// Some routes are not on known roads as a result.
        /// </summary>
        [Test]
        public void RoutingRegressionTest2()
        {
            var interpreter = new OsmRoutingInterpreter();
            var tagsIndex = new TagsTableCollectionIndex();

            // do the data processing.
            var memoryData = new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var targetData = new LiveGraphOsmStreamTarget(memoryData, interpreter, tagsIndex);
            var dataProcessorSource = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Test.Unittests.test_network.osm"));
            var sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(dataProcessorSource);
            targetData.RegisterSource(sorter);
            targetData.Pull();

            var basicRouter = new DykstraRoutingLive();
            var router = Router.CreateLiveFrom(memoryData, basicRouter, interpreter);

            // build coordinates list of resolved points.
            var testPoints = new List<GeoCoordinate>();
            testPoints.Add(new GeoCoordinate(51.0582204, 3.7193524));
            testPoints.Add(new GeoCoordinate(51.0582199, 3.7194002));

            testPoints.Add(new GeoCoordinate(51.0581727, 3.7195833));
            testPoints.Add(new GeoCoordinate(51.0581483, 3.7195553));

            testPoints.Add(new GeoCoordinate(51.0581883, 3.7196617));
            testPoints.Add(new GeoCoordinate(51.0581628, 3.7196889));

            // build a matrix of routes between all points.
            var referenceRoutes = new Route[testPoints.Count][];
            var permuationArray = new int[testPoints.Count];
            for (int fromIdx = 0; fromIdx < testPoints.Count; fromIdx++)
            {
                permuationArray[fromIdx] = fromIdx;
                referenceRoutes[fromIdx] = new Route[testPoints.Count];
                for (int toIdx = 0; toIdx < testPoints.Count; toIdx++)
                {
                    // create router from scratch.
                    router = Router.CreateLiveFrom(
                        memoryData, basicRouter, interpreter);

                    // resolve points.
                    var from = router.Resolve(Vehicle.Car, testPoints[fromIdx]);
                    var to = router.Resolve(Vehicle.Car, testPoints[toIdx]);

                    // calculate route.
                    referenceRoutes[fromIdx][toIdx] = router.Calculate(Vehicle.Car, from, to);
                }
            }

            // resolve points in some order and compare the resulting routes.
            // they should be identical in length except for some numerical rounding errors.
            var enumerator = new PermutationEnumerable<int>(permuationArray);
            foreach (int[] permutation in enumerator)
            {
                // create router from scratch.
                router = Router.CreateLiveFrom(memoryData, basicRouter, interpreter);

                // resolve in the order of the permutation.
                var resolvedPoints = new RouterPoint[permutation.Length];
                for (int idx = 0; idx < permutation.Length; idx++)
                {
                    resolvedPoints[permutation[idx]] = router.Resolve(Vehicle.Car, testPoints[permutation[idx]]);
                }

                for (int fromIdx = 0; fromIdx < testPoints.Count; fromIdx++)
                {
                    for (int toIdx = 0; toIdx < testPoints.Count; toIdx++)
                    {
                        // calculate route.
                        var route = router.Calculate(Vehicle.Car, resolvedPoints[fromIdx], resolvedPoints[toIdx]);

                        // TODO: changed the resolve accuracy to .5m. Make sure this is more accurate in the future.
                        Assert.AreEqual(referenceRoutes[fromIdx][toIdx].TotalDistance, route.TotalDistance, 1);
                    }
                }
            }
        }

        /// <summary>
        /// Issue with high-density resolved points. Routes are incorrectly skipping nodes.
        /// Some routes are not on known roads as a result.
        /// </summary>
        [Test]
        public void RoutingRegressionTest3()
        {
            var interpreter = new OsmRoutingInterpreter();
            var tagsIndex = new TagsTableCollectionIndex();

            // do the data processing.
            var memoryData = new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var targetData = new LiveGraphOsmStreamTarget(memoryData, interpreter, tagsIndex);
            var dataProcessorSource = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Test.Unittests.test_network.osm"));
            var sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(dataProcessorSource);
            targetData.RegisterSource(sorter);
            targetData.Pull();

            var basicRouter = new DykstraRoutingLive();
            var router = Router.CreateLiveFrom(memoryData, basicRouter, interpreter);

            // build coordinates list of resolved points.
            var testPoints = new List<GeoCoordinate>();
            testPoints.Add(new GeoCoordinate(51.0581719, 3.7201622));
            testPoints.Add(new GeoCoordinate(51.0580439, 3.7202134));
            testPoints.Add(new GeoCoordinate(51.0580573, 3.7204378));
            testPoints.Add(new GeoCoordinate(51.0581862, 3.7203758));

            // build a matrix of routes between all points.
            var referenceRoutes = new Route[testPoints.Count][];
            var permuationArray = new int[testPoints.Count];
            for (int fromIdx = 0; fromIdx < testPoints.Count; fromIdx++)
            {
                permuationArray[fromIdx] = fromIdx;
                referenceRoutes[fromIdx] = new Route[testPoints.Count];
                for (int toIdx = 0; toIdx < testPoints.Count; toIdx++)
                {
                    // create router from scratch.
                    router = Router.CreateLiveFrom(
                        memoryData, basicRouter, interpreter);

                    // resolve points.
                    var from = router.Resolve(Vehicle.Car, testPoints[fromIdx]);
                    var to = router.Resolve(Vehicle.Car, testPoints[toIdx]);

                    // calculate route.
                    referenceRoutes[fromIdx][toIdx] = router.Calculate(Vehicle.Car, from, to);
                }
            }

            // resolve points in some order and compare the resulting routes.
            // they should be identical in length except for some numerical rounding errors.
            var enumerator = new PermutationEnumerable<int>(
                permuationArray);
            foreach (int[] permutation in enumerator)
            {
                // create router from scratch.
                router = Router.CreateLiveFrom(memoryData, basicRouter, interpreter);

                // resolve in the order of the permutation.
                var resolvedPoints = new RouterPoint[permutation.Length];
                for (int idx = 0; idx < permutation.Length; idx++)
                {
                    resolvedPoints[permutation[idx]] = router.Resolve(Vehicle.Car, testPoints[permutation[idx]]);
                }

                for (int fromIdx = 0; fromIdx < testPoints.Count; fromIdx++)
                {
                    for (int toIdx = 0; toIdx < testPoints.Count; toIdx++)
                    {
                        // calculate route.
                        var route = router.Calculate(Vehicle.Car, resolvedPoints[fromIdx], resolvedPoints[toIdx]);

                        Assert.AreEqual(referenceRoutes[fromIdx][toIdx].TotalDistance, route.TotalDistance, 0.1);
                    }
                }
            }
        }

        /// <summary>
        /// Issue with resolved points with different routing profiles.
        /// https://github.com/OsmSharp/OsmSharp/issues/194
        /// </summary>
        [Test]
        public void RoutingRegressionTest4()
        {
            var interpreter = new OsmRoutingInterpreter();
            var tagsIndex = new TagsTableCollectionIndex();

            // do the data processing.
            var memoryData = new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var targetData = new LiveGraphOsmStreamTarget(memoryData, interpreter, tagsIndex);
            var dataProcessorSource = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Test.Unittests.test_routing_regression1.osm"));
            var sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(dataProcessorSource);
            targetData.RegisterSource(sorter);
            targetData.Pull();

            var basicRouter = new DykstraRoutingLive();
            var router = Router.CreateLiveFrom(memoryData, basicRouter, interpreter);

            // resolve the three points in question.
            var point35 = new GeoCoordinate(51.01257, 4.000753);
            var point35ResolvedCar = router.Resolve(Vehicle.Car, point35);
            var point35ResolvedBicycle = router.Resolve(Vehicle.Bicycle, point35);
        }

        ///// <summary>
        ///// Issue with multiple edges between the same two vertices and with a different shape.
        ///// </summary>
        //[Test]
        //public void RoutingRegressionTest5()
        //{
        //    double e = 0.1; // 10 cm

        //    // network:
        //    //        x(1)------x(2)
        //    //       /           \
        //    // x(3)--x(4)---------x(5)--x(6)

        //    // 1: 50.98508962508, 4.82958530756
        //    // 2: 50.98509255957, 4.83009340615
        //    // 3: 50.98496931078, 4.82889075077
        //    // 4: 50.98496931078, 4.82939884936
        //    // 5: 50.98496931078, 4.83025189562
        //    // 6: 50.98496931078, 4.83079728585

        //    var vertex1 = new GeoCoordinate(50.98508962508, 4.82958530756);
        //    var vertex2 = new GeoCoordinate(50.98509255957, 4.83009340615);
        //    var vertex3 = new GeoCoordinate(50.98496931078, 4.82889075077);
        //    var vertex4 = new GeoCoordinate(50.98496931078, 4.82939884936);
        //    var vertex5 = new GeoCoordinate(50.98496931078, 4.83025189562);
        //    var vertex6 = new GeoCoordinate(50.98496931078, 4.83079728585);

        //    var source = new XmlOsmStreamSource(
        //        Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Test.Unittests.test_routing_regression3.osm"));
        //    var router = Router.CreateLiveFrom(source, new OsmRoutingInterpreter());

        //    var resolved3 = router.Resolve(Vehicle.Car, vertex3, true);
        //    var resolved6 = router.Resolve(Vehicle.Car, vertex6, true);

        //    var route = router.Calculate(Vehicle.Car, resolved3, resolved6);
        //    var points = new List<GeoCoordinate>(route.GetPoints());

        //    Assert.AreEqual(4, points.Count);
        //    Assert.AreEqual(0, points[0].DistanceReal(vertex3).Value, e);
        //    Assert.AreEqual(0, points[1].DistanceReal(vertex4).Value, e);
        //    Assert.AreEqual(0, points[2].DistanceReal(vertex5).Value, e);
        //    Assert.AreEqual(0, points[3].DistanceReal(vertex6).Value, e);

        //    route = router.Calculate(Vehicle.Car, resolved6, resolved3);
        //    points = new List<GeoCoordinate>(route.GetPoints());

        //    Assert.AreEqual(4, points.Count);
        //    Assert.AreEqual(0, points[0].DistanceReal(vertex6).Value, e);
        //    Assert.AreEqual(0, points[1].DistanceReal(vertex5).Value, e);
        //    Assert.AreEqual(0, points[2].DistanceReal(vertex4).Value, e);
        //    Assert.AreEqual(0, points[3].DistanceReal(vertex3).Value, e);

        //    // route.SaveAsGpx(new FileInfo(@"d:\OSM\bin\route.gpx").OpenWrite());

        //}

    }
}