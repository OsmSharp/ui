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

using System.Reflection;
using NUnit.Framework;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;

namespace OsmSharp.Test.Unittests.Routing
{
    /// <summary>
    /// Base class with tests around IRouter objects.
    /// </summary>
    public abstract class RoutingComparisonTests
    {
        /// <summary>
        /// Returns a router test object.
        /// </summary>
        /// <returns></returns>
        public abstract Router BuildRouter(IOsmRoutingInterpreter interpreter, string embeddedName, bool contract);

        /// <summary>
        /// Builds a raw data source.
        /// </summary>
        /// <returns></returns>
        public DynamicGraphRouterDataSource<LiveEdge> BuildDykstraDataSource(
            IOsmRoutingInterpreter interpreter, string embeddedName)
        {
            var tagsIndex = new SimpleTagsIndex();

            // do the data processing.
            var data = new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var targetData = new LiveGraphOsmStreamTarget(
                data, interpreter, tagsIndex, new Vehicle[] { Vehicle.Car });
            var dataProcessorSource = new XmlOsmStreamSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format(
                "OsmSharp.Test.Unittests.{0}", embeddedName)));
            var sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(dataProcessorSource);
            targetData.RegisterSource(sorter);
            targetData.Pull();

            return data;
        }

        /// <summary>
        /// Builds a raw router to compare against.
        /// </summary>
        /// <returns></returns>
        public Router BuildDykstraRouter(IBasicRouterDataSource<LiveEdge> data,
            IRoutingInterpreter interpreter, IBasicRouter<LiveEdge> basicRouter)
        {
            // initialize the router.
            return Router.CreateLiveFrom(data, basicRouter, interpreter);
        }

        /// <summary>
        /// Tests one route.
        /// </summary>
        /// <param name="embeddedName"></param>
        /// <param name="contract"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        protected void TestCompareOne(string embeddedName, bool contract, GeoCoordinate from, GeoCoordinate to)
        {
            // build the routing settings.
            IOsmRoutingInterpreter interpreter = new OsmSharp.Routing.Osm.Interpreter.OsmRoutingInterpreter();

            // get the osm data source.
            IBasicRouterDataSource<LiveEdge> data = this.BuildDykstraDataSource(interpreter, embeddedName);

            // build the reference router.;
            Router referenceRouter = this.BuildDykstraRouter(
                this.BuildDykstraDataSource(interpreter, embeddedName), interpreter,
                    new DykstraRoutingLive());

            // build the router to be tested.
            Router router = this.BuildRouter(interpreter, embeddedName, contract);

            RouterPoint referenceResolvedFrom = referenceRouter.Resolve(Vehicle.Car, from);
            RouterPoint referenceResolvedTo = referenceRouter.Resolve(Vehicle.Car, to);
            RouterPoint resolvedFrom = router.Resolve(Vehicle.Car, from);
            RouterPoint resolvedTo = router.Resolve(Vehicle.Car, to);

            Route referenceRoute = referenceRouter.Calculate(Vehicle.Car, referenceResolvedFrom, referenceResolvedTo);
            Route route = router.Calculate(Vehicle.Car, resolvedFrom, resolvedTo);

            this.CompareRoutes(referenceRoute, route);
        }

        /// <summary>
        /// Compares all routes against the reference router.
        /// </summary>
        protected void TestCompareAll(string embeddedName, bool contract)
        {
            // build the routing settings.
            IOsmRoutingInterpreter interpreter = new OsmSharp.Routing.Osm.Interpreter.OsmRoutingInterpreter();

            // get the osm data source.
            IBasicRouterDataSource<LiveEdge> data = this.BuildDykstraDataSource(interpreter, embeddedName);

            // build the reference router.;
            Router referenceRouter = this.BuildDykstraRouter(
                this.BuildDykstraDataSource(interpreter, embeddedName), interpreter, 
                    new DykstraRoutingLive());

            // build the router to be tested.
            Router router = this.BuildRouter(interpreter, embeddedName, contract);

            this.TestCompareAll(data, referenceRouter, router);
        }

        /// <summary>
        /// Tests the the given router class by comparing calculated routes agains a given reference router.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="referenceRouter"></param>
        /// <param name="router"></param>
        protected void TestCompareAll<TEdgeData>(IBasicRouterDataSource<TEdgeData> data, Router referenceRouter, Router router)
            where TEdgeData : IDynamicGraphEdgeData
        {       
            // loop over all nodes and resolve their locations.
            var resolvedReference = new RouterPoint[data.VertexCount - 1];
            var resolved = new RouterPoint[data.VertexCount - 1];
            for (uint idx = 1; idx < data.VertexCount; idx++)
            { // resolve each vertex.
                float latitude, longitude;
                if (data.GetVertex(idx, out latitude, out longitude))
                {
                    resolvedReference[idx - 1] = referenceRouter.Resolve(Vehicle.Car, new GeoCoordinate(latitude, longitude), true);
                    resolved[idx - 1] = router.Resolve(Vehicle.Car, new GeoCoordinate(latitude, longitude), true);
                }

                Assert.IsNotNull(resolvedReference[idx - 1]);
                Assert.IsNotNull(resolved[idx - 1]);

                Assert.AreEqual(resolvedReference[idx - 1].Location.Latitude,
                    resolved[idx - 1].Location.Latitude, 0.0001);
                Assert.AreEqual(resolvedReference[idx - 1].Location.Longitude,
                    resolved[idx - 1].Location.Longitude, 0.0001);
            }

            // limit tests to a fixed number.
            int maxTestCount = 1000;
            int testEveryOther = (resolved.Length * resolved.Length) / maxTestCount;
            testEveryOther = System.Math.Max(testEveryOther, 1);

            // check all the routes having the same weight(s).
            for (int fromIdx = 0; fromIdx < resolved.Length; fromIdx++)
            {
                for (int toIdx = 0; toIdx < resolved.Length; toIdx++)
                {
                    int testNumber = fromIdx * resolved.Length + toIdx;
                    if (testNumber % testEveryOther == 0)
                    {
                        Route referenceRoute = referenceRouter.Calculate(Vehicle.Car,
                            resolvedReference[fromIdx], resolvedReference[toIdx]);
                        Route route = router.Calculate(Vehicle.Car,
                            resolved[fromIdx], resolved[toIdx]);

                        if (referenceRoute != null)
                        {
                            Assert.IsNotNull(referenceRoute);
                            Assert.IsNotNull(route);
                            this.CompareRoutes(referenceRoute, route);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compares the two given routes.
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="route"></param>
        protected void CompareRoutes(Route reference, Route route)
        {
            if (reference.Entries == null)
            {
                Assert.IsNull(route.Entries);
            }
            else
            {
                Assert.AreEqual(reference.Entries.Length, route.Entries.Length);
                for (int idx = 0; idx < reference.Entries.Length; idx++)
                {
                    Assert.AreEqual(reference.Entries[idx].Distance,
                        route.Entries[idx].Distance);
                    Assert.AreEqual(reference.Entries[idx].Latitude,
                        route.Entries[idx].Latitude);
                    Assert.AreEqual(reference.Entries[idx].Longitude,
                        route.Entries[idx].Longitude);
                    Assert.AreEqual(reference.Entries[idx].Time,
                        route.Entries[idx].Time);
                    Assert.AreEqual(reference.Entries[idx].Type,
                        route.Entries[idx].Type);
                    Assert.AreEqual(reference.Entries[idx].WayFromName,
                        route.Entries[idx].WayFromName);
                }
            }
        }
    }
}