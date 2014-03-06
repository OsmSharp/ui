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
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();

            TagsTableCollectionIndex tags_index = new TagsTableCollectionIndex();

            // do the data processing.
            DynamicGraphRouterDataSource<LiveEdge> memory_data =
                new DynamicGraphRouterDataSource<LiveEdge>(tags_index);
            LiveGraphOsmStreamTarget target_data = new LiveGraphOsmStreamTarget(
                memory_data, interpreter, tags_index);
            XmlOsmStreamSource data_processor_source = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Test.Unittests.test_routing_regression1.osm"));
            OsmStreamFilterSort sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(data_processor_source);
            target_data.RegisterSource(sorter);
            target_data.Pull();

            IBasicRouter<LiveEdge> basic_router = new DykstraRoutingLive();
            Router router = Router.CreateLiveFrom(memory_data, basic_router, interpreter);

            // resolve the three points in question.
            GeoCoordinate point35 = new GeoCoordinate(51.01257, 4.000753);
            RouterPoint point35resolved = router.Resolve(Vehicle.Car, point35);
//            GeoCoordinate point40 = new GeoCoordinate(51.01250, 4.000013);
//            RouterPoint point40resolved = router.Resolve(Vehicle.Car, point40);
            GeoCoordinate point45 = new GeoCoordinate(51.01315, 3.999588);
            RouterPoint point45resolved = router.Resolve(Vehicle.Car, point45);

            // route between 35 and 45.
            Route routebefore = router.Calculate(Vehicle.Car, point35resolved, point45resolved);

//            GeoCoordinate point129 = new GeoCoordinate(51.01239, 3.999573);
//            RouterPoint point129resolved = router.Resolve(Vehicle.Car, point129);

            // route between 35 and 45.
            Route routeafter = router.Calculate(Vehicle.Car, point35resolved, point45resolved);
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
            PermutationEnumerable<int> enumerator = new PermutationEnumerable<int>(
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
            LiveGraphOsmStreamTarget targetData = new LiveGraphOsmStreamTarget(
                memoryData, interpreter, tagsIndex);
            XmlOsmStreamSource data_processor_source = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Test.Unittests.test_network.osm"));
            OsmStreamFilterSort sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(data_processor_source);
            targetData.RegisterSource(sorter);
            targetData.Pull();

            IBasicRouter<LiveEdge> basic_router = new DykstraRoutingLive();
            Router router = Router.CreateLiveFrom(memoryData, basic_router, interpreter);

            // build coordinates list of resolved points.
            List<GeoCoordinate> testPoints = new List<GeoCoordinate>();
            testPoints.Add(new GeoCoordinate(51.0581719, 3.7201622));
            testPoints.Add(new GeoCoordinate(51.0580439, 3.7202134));
            testPoints.Add(new GeoCoordinate(51.0580573, 3.7204378));
            testPoints.Add(new GeoCoordinate(51.0581862, 3.7203758));

            // build a matrix of routes between all points.
            Route[][] referenceRoutes = new Route[testPoints.Count][];
            int[] permuationArray = new int[testPoints.Count];
            for (int fromIdx = 0; fromIdx < testPoints.Count; fromIdx++)
            {
                permuationArray[fromIdx] = fromIdx;
                referenceRoutes[fromIdx] = new Route[testPoints.Count];
                for (int toIdx = 0; toIdx < testPoints.Count; toIdx++)
                {
                    // create router from scratch.
                    router = Router.CreateLiveFrom(
                        memoryData, basic_router, interpreter);

                    // resolve points.
                    RouterPoint from = router.Resolve(Vehicle.Car, testPoints[fromIdx]);
                    RouterPoint to = router.Resolve(Vehicle.Car, testPoints[toIdx]);

                    // calculate route.
                    referenceRoutes[fromIdx][toIdx] = router.Calculate(Vehicle.Car, from, to);
                }
            }

            // resolve points in some order and compare the resulting routes.
            // they should be identical in length except for some numerical rounding errors.
            PermutationEnumerable<int> enumerator = new PermutationEnumerable<int>(
                permuationArray);
            foreach (int[] permutation in enumerator)
            {
                // create router from scratch.
                router = Router.CreateLiveFrom(memoryData, basic_router, interpreter);

                // resolve in the order of the permutation.
                RouterPoint[] resolvedPoints = new RouterPoint[permutation.Length];
                for (int idx = 0; idx < permutation.Length; idx++)
                {
                    resolvedPoints[permutation[idx]] = router.Resolve(Vehicle.Car, testPoints[permutation[idx]]);
                }

                for (int fromIdx = 0; fromIdx < testPoints.Count; fromIdx++)
                {
                    for (int toIdx = 0; toIdx < testPoints.Count; toIdx++)
                    {
                        // calculate route.
                        Route route = router.Calculate(Vehicle.Car, resolvedPoints[fromIdx], resolvedPoints[toIdx]);

                        Assert.AreEqual(referenceRoutes[fromIdx][toIdx].TotalDistance, route.TotalDistance, 0.1);
                    }
                }
            }
        }
    }
}