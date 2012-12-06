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
using OsmSharp.Osm.Routing.Data;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Osm.Data.XML.Raw.Processor;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.Core.Router;
using OsmSharp.Osm.Core;
using OsmSharp.Routing.Core.Graph.Memory;
using OsmSharp.Routing.Core.Graph.Router;
using OsmSharp.Tools.Math;
using OsmSharp.Routing.Core.Graph.Router.Dykstra;
using OsmSharp.Routing.Core.Graph.DynamicGraph.PreProcessed;

namespace OsmSharp.Osm.UnitTests.Routing.Dykstra
{
    /// <summary>
    /// Does some raw routing tests testing for oneway constraint.
    /// </summary>
    [TestClass]
    public class DykstraPreProcessingRoutingOneWayTests : RoutingOneWayTests<RouterPoint, PreProcessedEdge>
    {
        /// <summary>
        /// Builds a router.
        /// </summary>
        /// <returns></returns>
        public override IRouter<RouterPoint> BuildRouter(IBasicRouterDataSource<PreProcessedEdge> data, IRoutingInterpreter interpreter,
            IBasicRouter<PreProcessedEdge> basic_router)
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
        /// <returns></returns>
        public override IBasicRouterDataSource<PreProcessedEdge> BuildData(IRoutingInterpreter interpreter)
        {
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            MemoryRouterDataSource<PreProcessedEdge> data =
                new MemoryRouterDataSource<PreProcessedEdge>(tags_index);
            PreProcessedDataGraphProcessingTarget target_data = new PreProcessedDataGraphProcessingTarget(
                data, interpreter, data.TagsIndex);
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test_network_oneway.osm"));
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
        public void TestDykstraPreProcessingOneWayShortestWithDirection()
        {
            this.DoTestShortestWithDirection();
        }

        /// <summary>
        /// Tests a simple shortest route calculation.
        /// </summary>
        [TestMethod]
        public void TestDykstraPreProcessingOneWayShortestAgainstDirection()
        {
            this.DoTestShortestAgainstDirection();
        }

        /// <summary>
        /// Test if the raw router many-to-many weights correspond to the point-to-point weights.
        /// </summary>
        [TestMethod]
        public void TestDykstraPreProcessingOneWayManyToMany1()
        {
            this.DoTestManyToMany1();
        }

        /// <summary>
        /// Test if the raw router handles connectivity questions correctly.
        /// </summary>
        [TestMethod]
        public void TestDykstraPreProcessingOneWayConnectivity1()
        {
            this.DoTestConnectivity1();
        }

        /// <summary>
        /// Tests a simple shortest route calculation.
        /// </summary>
        [TestMethod]
        public void TestDykstraPreProcessingOneWayResolveAllNodes()
        {
            this.DoTestResolveAllNodes();
        }

        /// <summary>
        /// Tests resolving coordinates to routable points.
        /// </summary>
        [TestMethod]
        public void TestDykstraPreProcessingOneWayResolveBetweenNodes()
        {
            this.DoTestResolveBetweenNodes();
        }
    }
}