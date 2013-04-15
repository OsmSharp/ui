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
using NUnit.Framework;
using OsmSharp.Routing.Router;
using OsmSharp.Routing;
using OsmSharp.Routing.Osm.Data;
using OsmSharp.Osm;
using OsmSharp.Osm.Data.Raw.XML.OsmSource;
using System.Reflection;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Graph.DynamicGraph.PreProcessed;

namespace OsmSharp.Osm.UnitTests.Routing.Dykstra
{
    /// <summary>
    /// Does some tests on an OsmSource routing implementation.
    /// </summary>
    [TestFixture]
    public class OsmSourceRoutingTests : SimpleRoutingTests<RouterPoint, PreProcessedEdge>
    {
        /// <summary>
        /// Builds a router.
        /// </summary>
        /// <returns></returns>
        public override IRouter<RouterPoint> BuildRouter(IBasicRouterDataSource<PreProcessedEdge> data, 
            IRoutingInterpreter interpreter, IBasicRouter<PreProcessedEdge> basic_router)
        {
            // initialize the router.
            return new Router<PreProcessedEdge>(
                    data, interpreter, basic_router);
        }

        /// <summary>
        /// Builds a basic router.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override IBasicRouter<PreProcessedEdge> BuildBasicRouter(IBasicRouterDataSource<PreProcessedEdge> data)
        {
            return new DykstraRoutingPreProcessed(data.TagsIndex);
        }

        /// <summary>
        /// Builds data source.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="embedded_string"></param>
        /// <returns></returns>
        public override IBasicRouterDataSource<PreProcessedEdge> BuildData(IRoutingInterpreter interpreter,
            string embedded_string)
        {
            OsmTagsIndex tags_index = new OsmTagsIndex();
            
            // do the data processing.
            OsmDataSource source = new OsmDataSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(embedded_string)); // "OsmSharp.UnitTests.test_network.osm"));
            return new OsmSharp.Routing.Osm.Data.OsmSourceRouterDataSource(interpreter,
                tags_index, source, VehicleEnum.Car);
        }

        /// <summary>
        /// Tests a simple shortest route calculation.
        /// </summary>
        [Test]
        public void TestOsmSourceShortedDefault()
        {
            this.DoTestShortestDefault();
        }

        /// <summary>
        /// Tests if the raw router preserves tags.
        /// </summary>
        [Test]
        public void TestOsmSourceResolvedTags()
        {
            this.DoTestResolvedTags();
        }

        /// <summary>
        /// Tests if the raw router preserves tags on arcs/ways.
        /// </summary>
        [Test]
        public void TestOsmSourceArcTags()
        {
            this.DoTestArcTags();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [Test]
        public void TestOsmSourceShortest1()
        {
            this.DoTestShortest1();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [Test]
        public void TestOsmSourceShortest2()
        {
            this.DoTestShortest2();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [Test]
        public void TestOsmSourceShortest3()
        {
            this.DoTestShortest3();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [Test]
        public void TestOsmSourceShortest4()
        {
            this.DoTestShortest4();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [Test]
        public void TestOsmSourceShortest5()
        {
            this.DoTestShortest5();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [Test]
        public void TestOsmSourceShortestResolved1()
        {
            this.DoTestShortestResolved1();
        }

        /// <summary>
        /// Test if the raw router many-to-many weights correspond to the point-to-point weights.
        /// </summary>
        [Test]
        public void TestOsmSourceManyToMany1()
        {
            this.DoTestManyToMany1();
        }

        /// <summary>
        /// Test if the raw router handles connectivity questions correctly.
        /// </summary>
        [Test]
        public void TestOsmSourceConnectivity1()
        {
            this.DoTestConnectivity1();
        }
    }
}
