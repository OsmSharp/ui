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
//using System;
//using System.Text;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using OsmSharp.Osm.Routing.CH.Routing;
//using OsmSharp.Osm.Routing.Core;
//using OsmSharp.Osm.Routing.Core.Interpreter;
//using OsmSharp.Osm.Routing.Core.Constraints;
//using OsmSharp.Osm.Data.Core.DynamicGraph.Memory;
//using OsmSharp.Osm.Routing.CH.PreProcessing;
//using OsmSharp.Osm.Data.Core.DynamicGraph;
//using System.Reflection;
//using OsmSharp.Osm.Data.XML.Raw.Processor;
//using OsmSharp.Osm.Routing.CH.Processor;
//using OsmSharp.Osm.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
//using OsmSharp.Osm.Routing.CH.PreProcessing.Witnesses;
//using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;

//namespace OsmSharp.Osm.UnitTests.Routing.CH
//{
//    [TestClass]
//    public class CHEdgeDifferenceRoutingTest : SimpleRoutingTests<CHResolvedPoint>
//    {

//        /// <summary>
//        /// Returns a new router.
//        /// </summary>
//        /// <param name="interpreter"></param>
//        /// <param name="constraints"></param>
//        /// <returns></returns>
//        public override IRouter<CHResolvedPoint> BuildRouter(RoutingInterpreterBase interpreter,
//            IRoutingConstraints constraints)
//        {
//            // build the memory data source.
//            CHDataSource data = new CHDataSource();

//            // load the data.
//            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(
//                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.UnitTests.test_network.osm"));
//            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
//            sorter.RegisterSource(data_processor_source);
//            CHDataProcessorTarget ch_target = new CHDataProcessorTarget(data);
//            ch_target.RegisterSource(sorter);
//            ch_target.Pull();

//            // do the pre-processing part.
//            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(data.Graph);
//            INodeWeightCalculator ordering = new OsmSharp.Osm.Routing.CH.PreProcessing.Ordering.EdgeDifference(
//                data.Graph, witness_calculator);
//            CHPreProcessor pre_processor = new CHPreProcessor(data.Graph,
//                ordering, witness_calculator);
//            pre_processor.Start();

//            // create the router from the contracted data.
//            return new Router(data);
//        }

//        /// <summary>
//        /// Tests a simple shortest route calculation.
//        /// </summary>
//        [TestMethod]
//        public void TestCHEdgeDifferenceShortedDefault()
//        {
//            this.DoTestShortestDefault();
//        }

//        /// <summary>
//        /// Tests if the raw router preserves tags.
//        /// </summary>
//        [TestMethod]
//        public void TestCHEdgeDifferenceResolvedTags()
//        {
//            this.DoTestResolvedTags();
//        }

//        /// <summary>
//        /// Tests if the raw router preserves tags on arcs/ways.
//        /// </summary>
//        [TestMethod]
//        public void TestCHEdgeDifferenceArcTags()
//        {
//            this.DoTestArcTags();
//        }


//        /// <summary>
//        /// Test is the CH router can calculate another route.
//        /// </summary>
//        [TestMethod]
//        public void TestCHEdgeDifferenceShortest1()
//        {
//            this.DoTestShortest1();
//        }

//        /// <summary>
//        /// Test is the CH router can calculate another route.
//        /// </summary>
//        [TestMethod]
//        public void TestCHEdgeDifferenceShortest2()
//        {
//            this.DoTestShortest2();
//        }

//        /// <summary>
//        /// Test is the CH router can calculate another route.
//        /// </summary>
//        [TestMethod]
//        public void TestCHEdgeDifferenceShortest3()
//        {
//            this.DoTestShortest3();
//        }

//        /// <summary>
//        /// Test is the CH router can calculate another route.
//        /// </summary>
//        [TestMethod]
//        public void TestCHEdgeDifferenceShortest4()
//        {
//            this.DoTestShortest4();
//        }

//        /// <summary>
//        /// Test is the CH router can calculate another route.
//        /// </summary>
//        [TestMethod]
//        public void TestCHEdgeDifferenceShortest5()
//        {
//            this.DoTestShortest5();
//        }

//        /// <summary>
//        /// Test if the ch router many-to-many weights correspond to the point-to-point weights.
//        /// </summary>
//        [TestMethod]
//        public void TestCHEdgeDifferenceManyToMany1()
//        {
//            this.DoTestManyToMany1();
//        }

//        /// <summary>
//        /// Test if the ch router handles connectivity questions correctly.
//        /// </summary>
//        [TestMethod]
//        public void TestCHEdgeDifferenceConnectivity1()
//        {
//            this.DoTestConnectivity1();
//        }
//    }
//}