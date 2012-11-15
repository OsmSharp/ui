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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmSharp.Routing.Core;
using OsmSharp.Osm.Data.Raw.XML.OsmSource;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Routing.Core.Constraints;
using System.IO;
using System.Reflection;
using OsmSharp.Osm.Core.Xml;
using System.Xml;
using OsmSharp.Osm.Routing.Interpreter;
using OsmSharp.Routing.Core.Router.Memory;
using OsmSharp.Osm.Routing.Data;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Osm.Data.XML.Raw.Processor;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.Core.Router;
using OsmSharp.Osm.Core;

namespace OsmSharp.Osm.UnitTests.Routing.Raw
{
    [TestClass]
    public class RawRoutingTests : SimpleRoutingTests<RouterPoint, OsmEdgeData>
    {
        /// <summary>
        /// Builds a router.
        /// </summary>
        /// <returns></returns>
        public override IRouter<RouterPoint> BuildRouter(IRouterDataSource<OsmEdgeData> data, 
            IRoutingInterpreter interpreter)
        {
            // initialize the router.
            return new Router<OsmEdgeData>(
                    data, interpreter);
        }

        /// <summary>
        /// Builds data source.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <returns></returns>
        public override IRouterDataSource<OsmEdgeData> BuildData(IRoutingInterpreter interpreter)
        {
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            MemoryRouterDataSource<OsmEdgeData> data =
                new MemoryRouterDataSource<OsmEdgeData>(tags_index);
            OsmEdgeDataGraphProcessingTarget target_data = new OsmEdgeDataGraphProcessingTarget(
                data, interpreter, data.TagsIndex);
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("Osm.UnitTests.test_network.osm"));
            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(data_processor_source);
            target_data.RegisterSource(sorter);
            target_data.Pull();

            return data;
        }

        /// <summary>
        /// Tests a simple shortest route calculation.
        /// </summary>
        [TestMethod]
        public void TestRawShortedDefault()
        {
            this.DoTestShortestDefault();
        }

        /// <summary>
        /// Tests if the raw router preserves tags.
        /// </summary>
        [TestMethod]
        public void TestRawResolvedTags()
        {
            this.DoTestResolvedTags();
        }

        /// <summary>
        /// Tests if the raw router preserves tags on arcs/ways.
        /// </summary>
        [TestMethod]
        public void TestRawArcTags()
        {
            this.DoTestArcTags();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [TestMethod]
        public void TestRawShortest1()
        {
            this.DoTestShortest1();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [TestMethod]
        public void TestRawShortest2()
        {
            this.DoTestShortest2();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [TestMethod]
        public void TestRawShortest3()
        {
            this.DoTestShortest3();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [TestMethod]
        public void TestRawShortest4()
        {
            this.DoTestShortest4();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [TestMethod]
        public void TestRawShortest5()
        {
            this.DoTestShortest5();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [TestMethod]
        public void TestRawShortestResolved1()
        {
            this.DoTestShortestResolved1();
        }

        /// <summary>
        /// Test if the raw router many-to-many weights correspond to the point-to-point weights.
        /// </summary>
        [TestMethod]
        public void TestRawManyToMany1()
        {
            this.DoTestManyToMany1();
        }

        /// <summary>
        /// Test if the raw router handles connectivity questions correctly.
        /// </summary>
        [TestMethod]
        public void TestRawConnectivity1()
        {
            this.DoTestConnectivity1();
        }

        /// <summary>
        /// Tests a simple shortest route calculation.
        /// </summary>
        [TestMethod]
        public void TestRawResolveAllNodes()
        {
            this.DoTestResolveAllNodes();
        }
    }
}