//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using OsmSharp.Osm.Routing.CH.Routing;
//using OsmSharp.Osm.Routing.Core;
//using OsmSharp.Osm.Routing.Core.Interpreter;
//using OsmSharp.Osm.Routing.Core.Constraints;
//using OsmSharp.Osm.Data.XML.Raw.Processor;
//using System.Reflection;
//using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
//using OsmSharp.Osm.Routing.CH.Processor;
//using OsmSharp.Osm.Routing.CH.PreProcessing;
//using OsmSharp.Osm.Routing.CH.PreProcessing.Witnesses;
//using OsmSharp.Osm.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;

//namespace OsmSharp.Osm.UnitTests.Routing.CH
//{
//    [TestClass]
//    public class CHSparseComparisonTests : RoutingComparisonTests<CHResolvedPoint>
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
//                Assembly.GetExecutingAssembly().GetManifestResourceStream("Osm.UnitTests.test_network.osm"));
//            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
//            sorter.RegisterSource(data_processor_source);
//            CHDataProcessorTarget ch_target = new CHDataProcessorTarget(data);
//            ch_target.RegisterSource(sorter);
//            ch_target.Pull();

//            // do the pre-processing part.
//            CHPreProcessor pre_processor = new CHPreProcessor(data.Graph,
//                new SparseOrdering(data.Graph), new DykstraWitnessCalculator(data.Graph));
//            pre_processor.Start();

//            // create the router from the contracted data.
//            return new Router(data);
//        }

//        /// <summary>
//        /// Compares all routes possible against a reference implementation.
//        /// </summary>
//        [TestMethod]
//        public void TestCHSparseAgainstReference()
//        {
//            this.TestCompareAll();
//        }
//    }
//}
