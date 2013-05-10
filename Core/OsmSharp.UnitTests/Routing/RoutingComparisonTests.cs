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

using System.Reflection;
using NUnit.Framework;
using OsmSharp.Osm.Data.Streams.Filters;
using OsmSharp.Osm.Data.Xml.Processor;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Osm.Data.Processing;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Route;
using OsmSharp.Routing.Routers;
using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Graph.Router;

namespace OsmSharp.Osm.UnitTests.Routing
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
        public abstract Router BuildRouter(IRoutingInterpreter interpreter, string embeddedName);

        /// <summary>
        /// Builds a raw data source.
        /// </summary>
        /// <returns></returns>
        public DynamicGraphRouterDataSource<LiveEdge> BuildDykstraDataSource(
            IRoutingInterpreter interpreter, string embeddedName)
        {
            var tagsIndex = new SimpleTagsIndex();

            // do the data processing.
            var data = new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
            var targetData = new LiveGraphOsmStreamWriter(
                data, interpreter, data.TagsIndex);
            var dataProcessorSource = new XmlOsmStreamReader(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format(
                "OsmSharp.UnitTests.{0}", embeddedName)));
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
        /// Compares all routes against the reference router.
        /// </summary>
        public void TestCompareAll(string embeddedName)
        {
            // build the routing settings.
            IRoutingInterpreter interpreter = new OsmSharp.Routing.Osm.Interpreter.OsmRoutingInterpreter();

            // get the osm data source.
            IBasicRouterDataSource<LiveEdge> data = this.BuildDykstraDataSource(interpreter, embeddedName);

            // build the reference router.;
            Router referenceRouter = this.BuildDykstraRouter(
                this.BuildDykstraDataSource(interpreter, embeddedName), interpreter, 
                    new DykstraRoutingLive(data.TagsIndex));

            // build the router to be tested.
            Router router = this.BuildRouter(interpreter, embeddedName);

            // loop over all nodes and resolve their locations.
            var resolvedReference = new RouterPoint[data.VertexCount - 1];
            var resolved = new RouterPoint[data.VertexCount - 1];
            for (uint idx = 1; idx < data.VertexCount; idx++)
            { // resolve each vertex.
                float latitude, longitude;
                if(data.GetVertex(idx, out latitude, out longitude))
                {
                    resolvedReference[idx - 1] = referenceRouter.Resolve(VehicleEnum.Car, new GeoCoordinate(latitude, longitude));
                    resolved[idx - 1] = router.Resolve(VehicleEnum.Car, new GeoCoordinate(latitude, longitude));
                }

                Assert.IsNotNull(resolvedReference[idx - 1]);
                Assert.IsNotNull(resolved[idx - 1]);

                Assert.AreEqual(resolvedReference[idx - 1].Location.Latitude,
                    resolved[idx - 1].Location.Latitude, 0.0001);
                Assert.AreEqual(resolvedReference[idx - 1].Location.Longitude,
                    resolved[idx - 1].Location.Longitude, 0.0001);
            }

            // check all the routes having the same weight(s).
            for (int fromIdx = 0; fromIdx < resolved.Length; fromIdx++)
            {
                for (int toIdx = 0; toIdx < resolved.Length; toIdx++)
                {
                    OsmSharpRoute referenceRoute = referenceRouter.Calculate(VehicleEnum.Car, 
                        resolvedReference[fromIdx], resolvedReference[toIdx]);
                    OsmSharpRoute route = router.Calculate(VehicleEnum.Car, 
                        resolved[fromIdx], resolved[toIdx]);

                    Assert.IsNotNull(referenceRoute);
                    Assert.IsNotNull(route);
                    Assert.AreEqual(referenceRoute.TotalDistance, route.TotalDistance, 0.0001);
                    // TODO: meta data is missing in some CH routing; see issue 
                    //Assert.AreEqual(reference_route.TotalTime, route.TotalTime, 0.0001);
                }
            }
        }
    }
}