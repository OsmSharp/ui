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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Reflection;
using OsmSharp.Osm.Data.XML.Processor;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Router;
using OsmSharp.Routing.Interpreter;
using OsmSharp.Routing.Osm.Data;
using OsmSharp.Routing.Osm.Data.Processing;
using OsmSharp.Osm;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.CH.Routing;
using OsmSharp.Routing.Graph.Router;
using OsmSharp.UnitTests;

namespace OsmSharp.Osm.UnitTests.Routing.CH
{
    /// <summary>
    /// Tests the sparse node ordering CH.
    /// </summary>
    [TestFixture]
    public class CHSparseRoutingTest : SimpleRoutingTests<RouterPoint, CHEdgeData>
    {
        /// <summary>
        /// Returns a new router.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="interpreter"></param>
        /// <param name="basic_router"></param>
        /// <returns></returns>
        public override IRouter<RouterPoint> BuildRouter(IBasicRouterDataSource<CHEdgeData> data,
            IRoutingInterpreter interpreter, IBasicRouter<CHEdgeData> basic_router)
        {
            return new Router<CHEdgeData>(data, interpreter, basic_router);
        }

        /// <summary>
        /// Returns a basic router.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override IBasicRouter<CHEdgeData> BuildBasicRouter(IBasicRouterDataSource<CHEdgeData> data)
        {
            return new CHRouter(data);
        }

        /// <summary>
        /// Builds the data.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="embedded_string"></param>
        /// <returns></returns>
        public override IBasicRouterDataSource<CHEdgeData> BuildData(IRoutingInterpreter interpreter,
            string embedded_string)
        {
            string key = string.Format("CHSparse.IBasicRouterDataSource<CHEdgeData>.OSM.{0}", 
                embedded_string);
            IBasicRouterDataSource<CHEdgeData> data = StaticDictionary.Get<IBasicRouterDataSource<CHEdgeData>>(
                key);
            //if (data == null)
            //{
                OsmTagsIndex tags_index = new OsmTagsIndex();

                // do the data processing.
                DynamicGraphRouterDataSource<CHEdgeData> memory_data =
                    new DynamicGraphRouterDataSource<CHEdgeData>(tags_index);
                CHEdgeDataGraphProcessingTarget target_data = new CHEdgeDataGraphProcessingTarget(
                    memory_data, interpreter, memory_data.TagsIndex, VehicleEnum.Car);
                XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(embedded_string));
                DataProcessorFilterSort sorter = new DataProcessorFilterSort();
                sorter.RegisterSource(data_processor_source);
                target_data.RegisterSource(sorter);
                target_data.Pull();

                // do the pre-processing part.
                CHPreProcessor pre_processor = new CHPreProcessor(memory_data,
                    new SparseOrdering(memory_data), new DykstraWitnessCalculator(memory_data));
                pre_processor.Start();

                data = memory_data;
                StaticDictionary.Add<IBasicRouterDataSource<CHEdgeData>>(key, data);
            //}
            return data;
        }

        /// <summary>
        /// Tests a simple shortest route calculation.
        /// </summary>
        [Test]
        public void TestCHSparseShortedDefault()
        {
            this.DoTestShortestDefault();
        }

        /// <summary>
        /// Tests if the raw router preserves tags.
        /// </summary>
        [Test]
        public void TestCHSparseResolvedTags()
        {
            this.DoTestResolvedTags();
        }

        /// <summary>
        /// Tests if the raw router preserves tags on arcs/ways.
        /// </summary>
        [Test]
        public void TestCHSparseArcTags()
        {
            this.DoTestArcTags();
        }

        /// <summary>
        /// Test is the CH router can calculate another route.
        /// </summary>
        [Test]
        public void TestCHSparseShortest1()
        {
            this.DoTestShortest1();
        }

        /// <summary>
        /// Test is the CH router can calculate another route.
        /// </summary>
        [Test]
        public void TestCHSparseShortest2()
        {
            this.DoTestShortest2();
        }

        /// <summary>
        /// Test is the CH router can calculate another route.
        /// </summary>
        [Test]
        public void TestCHSparseShortest3()
        {
            this.DoTestShortest3();
        }

        /// <summary>
        /// Test is the CH router can calculate another route.
        /// </summary>
        [Test]
        public void TestCHSparseShortest4()
        {
            this.DoTestShortest4();
        }

        /// <summary>
        /// Test is the CH router can calculate another route.
        /// </summary>
        [Test]
        public void TestCHSparseShortest5()
        {
            this.DoTestShortest5();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [Test]
        public void TestCHSparseResolvedShortest1()
        {
            this.DoTestShortestResolved1();
        }

        /// <summary>
        /// Test is the raw router can calculate another route.
        /// </summary>
        [Test]
        public void TestCHSparseResolvedShortest2()
        {
            this.DoTestShortestResolved2();
        }

        /// <summary>
        /// Test if the ch router many-to-many weights correspond to the point-to-point weights.
        /// </summary>
        [Test]
        public void TestCHSparseManyToMany1()
        {
            this.DoTestManyToMany1();
        }

        ///// <summary>
        ///// Test if the ch router handles connectivity questions correctly.
        ///// </summary>
        //[Test]
        //public void TestCHSparseConnectivity1()
        //{
        //    this.DoTestConnectivity1();
        //}

        /// <summary>
        /// Tests a simple shortest route calculation.
        /// </summary>
        [Test]
        public void TestCHSparseResolveAllNodes()
        {
            this.DoTestResolveAllNodes();
        }

        /// <summary>
        /// Tests a simple shortest route calculation.
        /// </summary>
        [Test]
        public void TestCHSparseResolveBetweenNodes()
        {
            this.DoTestResolveBetweenNodes();
        }

        /// <summary>
        /// Tests routing when resolving points.
        /// </summary>
        [Test]
        public void TestCHSparseResolveBetweenClose()
        {
            this.DoTestResolveBetweenClose();
        }

        /// <summary>
        /// Tests routing when resolving points.
        /// </summary>
        [Test]
        public void TestCHSparseResolveBetweenTwo()
        {
            this.DoTestResolveBetweenTwo();
        }

        /// <summary>
        /// Tests routing when resolving points.
        /// </summary>
        [Test]
        public void TestCHSparseResolveCase1()
        {
            this.DoTestResolveCase1();
        }

        /// <summary>
        /// Tests routing when resolving points.
        /// </summary>
        [Test]
        public void TestCHSparseResolveCase2()
        {
            this.DoTestResolveCase2();
        }
    }
}