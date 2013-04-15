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
using OsmSharp.Routing;
using OsmSharp.Osm.Data.Raw.XML.OsmSource;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Constraints;
using System.IO;
using System.Reflection;
using OsmSharp.Osm.Xml;
using System.Xml;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Data;
using OsmSharp.Routing.Osm.Data.Processing;
using OsmSharp.Osm.Data.XML.Processor;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.Router;
using OsmSharp.Osm;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.Tools.Math;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Graph.DynamicGraph.PreProcessed;
using OsmSharp.UnitTests;

namespace OsmSharp.Osm.UnitTests.Routing.DykstraPreProcessing
{
    /// <summary>
    /// Does some raw routing tests.
    /// </summary>
    [TestFixture]
    public class DykstraPreProcessingRoutingTests : SimpleRoutingTests<RouterPoint, PreProcessedEdge>
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
        /// <param name="embedded_string"></param>
        /// <returns></returns>
        public override IBasicRouterDataSource<PreProcessedEdge> BuildData(IRoutingInterpreter interpreter,
            string embedded_string)
        {
            string key = string.Format("Dykstra.Routing.IBasicRouterDataSource<PreProcessedEdge>.OSM.{0}",
                embedded_string);
            IBasicRouterDataSource<PreProcessedEdge> data = StaticDictionary.Get<IBasicRouterDataSource<PreProcessedEdge>>(
                key);
            if (data == null)
            {
                OsmTagsIndex tags_index = new OsmTagsIndex();

                // do the data processing.
                DynamicGraphRouterDataSource<PreProcessedEdge> memory_data =
                    new DynamicGraphRouterDataSource<PreProcessedEdge>(tags_index);
                PreProcessedDataGraphProcessingTarget target_data = new PreProcessedDataGraphProcessingTarget(
                    memory_data, interpreter, memory_data.TagsIndex, VehicleEnum.Car);
                XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(embedded_string));
                DataProcessorFilterSort sorter = new DataProcessorFilterSort();
                sorter.RegisterSource(data_processor_source);
                target_data.RegisterSource(sorter);
                target_data.Pull();

                data = memory_data;
                StaticDictionary.Add<IBasicRouterDataSource<PreProcessedEdge>>(key,
                    data);
            }
            return data;
        }

        /// <summary>
        /// Tests a simple shortest route calculation.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingShortedDefault()
        {
            this.DoTestShortestDefault();
        }

        /// <summary>
        /// Tests if the raw router preserves tags.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingResolvedTags()
        {
            this.DoTestResolvedTags();
        }

        /// <summary>
        /// Tests if the raw router preserves tags on arcs/ways.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingArcTags()
        {
            this.DoTestArcTags();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingShortest1()
        {
            this.DoTestShortest1();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingShortest2()
        {
            this.DoTestShortest2();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingShortest3()
        {
            this.DoTestShortest3();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingShortest4()
        {
            this.DoTestShortest4();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingShortest5()
        {
            this.DoTestShortest5();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingShortestResolved1()
        {
            this.DoTestShortestResolved1();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingShortestResolved2()
        {
            this.DoTestShortestResolved2();
        }

        /// <summary>
        /// Test if the raw router many-to-many weights correspond to the point-to-point weights.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingManyToMany1()
        {
            this.DoTestManyToMany1();
        }

        /// <summary>
        /// Test if the raw router handles connectivity questions correctly.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingConnectivity1()
        {
            this.DoTestConnectivity1();
        }

        /// <summary>
        /// Tests a simple shortest route calculation.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingResolveAllNodes()
        {
            this.DoTestResolveAllNodes();
        }

        /// <summary>
        /// Tests resolving coordinates to routable points.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingResolveBetweenNodes()
        {
            this.DoTestResolveBetweenNodes();
        }

        /// <summary>
        /// Regression test on routing resolved nodes.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingResolveBetweenRouteToSelf()
        {
            this.DoTestResolveBetweenRouteToSelf();
        }

        /// <summary>
        /// Tests routing when resolving points.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingResolveBetweenClose()
        {
            this.DoTestResolveBetweenClose();
        }

        /// <summary>
        /// Tests routing when resolving points.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingResolveCase1()
        {
            this.DoTestResolveCase1();
        }

        /// <summary>
        /// Tests routing when resolving points.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingResolveCase2()
        {
            this.DoTestResolveCase2();
        }

        /// <summary>
        /// Tests routing when resolving points.
        /// </summary>
        [Test]
        public void TestDykstraPreProcessingResolveSameLocation()
        {
            this.DoTestResolveSameLocation();
        }
    }
}