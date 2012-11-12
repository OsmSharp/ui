//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Osm.Routing.CH.Routing;
//using Osm.Routing.Core;
//using Osm.Routing.Core.Interpreter;
//using Osm.Routing.Core.Constraints;
//using Osm.Data.XML.Raw.Processor;
//using System.Reflection;
//using Osm.Data.Core.Processor.Filter.Sort;
//using Osm.Routing.CH.Processor;
//using Osm.Routing.CH.PreProcessing;
//using Osm.Routing.CH.PreProcessing.Witnesses;
//using Osm.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;

//namespace Osm.UnitTests.Routing.CH
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
