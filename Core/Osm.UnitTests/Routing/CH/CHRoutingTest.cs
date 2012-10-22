using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osm.Routing.CH.Routing;
using Osm.Routing.Core;
using Osm.Routing.Core.Interpreter;
using Osm.Routing.Core.Constraints;
using Osm.Data.Core.DynamicGraph.Memory;
using Osm.Routing.CH.PreProcessing;
using Osm.Data.Core.DynamicGraph;
using System.Reflection;
using Osm.Data.XML.Raw.Processor;
using Osm.Routing.CH.Processor;
using Osm.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
using Osm.Routing.CH.PreProcessing.Witnesses;

namespace Osm.UnitTests.Routing.CH
{
    [TestClass]
    public class CHRoutingTest : SimpleRoutingTests<CHResolvedPoint>
    {
        ///// <summary>
        ///// Tests a simple shortest route calculation.
        ///// </summary>
        //[TestMethod]
        //public void TestCHShortedDefault()
        //{
        //    this.DoTestShortestDefault();
        //}

        /// <summary>
        /// Returns a new router.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="constraints"></param>
        /// <returns></returns>
        public override IRouter<CHResolvedPoint> BuildRouter(RoutingInterpreterBase interpreter, 
            IRoutingConstraints constraints)
        {
            // build the memory data source.
            IDynamicGraph<CHEdgeData> data = new MemoryDynamicGraph<CHEdgeData>();

            // load the data.
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("Osm.UnitTests.test_network.osm"));
            CHDataProcessorTarget ch_target = new CHDataProcessorTarget(data);

            // do the pre-processing part.
            CHPreProcessor pre_processor = new CHPreProcessor(data, new SparseOrdering(data), new DykstraWitnessCalculator(data));
            pre_processor.Start();
            
            // create the router from the contracted data.
            return new Router(data);
        }
    }
}
