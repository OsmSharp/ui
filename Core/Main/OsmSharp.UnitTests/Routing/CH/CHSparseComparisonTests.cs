using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmSharp.Osm.Data.XML.Raw.Processor;
using System.Reflection;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.Core;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Osm.Core;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
using OsmSharp.Routing.CH.Routing;
using OsmSharp.Routing.Core.Graph.Memory;

namespace OsmSharp.Osm.UnitTests.Routing.CH
{
    /// <summary>
    /// Tests the CH Sparse routing against a reference implementation.
    /// </summary>
    [TestClass]
    public class CHSparseComparisonTests : RoutingComparisonTests
    {
        /// <summary>
        /// Returns a new router.
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="embedded_name"></param>
        /// <returns></returns>
        public override IRouter<RouterPoint> BuildRouter(IRoutingInterpreter interpreter, string embedded_name)
        {
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            MemoryRouterDataSource<CHEdgeData> data =
                new MemoryRouterDataSource<CHEdgeData>(tags_index);
            CHEdgeDataGraphProcessingTarget target_data = new CHEdgeDataGraphProcessingTarget(
                data, interpreter, data.TagsIndex);
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format(
                "OsmSharp.UnitTests.{0}", embedded_name)));
            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(data_processor_source);
            target_data.RegisterSource(sorter);
            target_data.Pull();

            // do the pre-processing part.
            CHPreProcessor pre_processor = new CHPreProcessor(data,
                new SparseOrdering(data), new DykstraWitnessCalculator(data));
            pre_processor.Start();

            return new Router<CHEdgeData>(data, interpreter, new CHRouter(
                data));
        }

        /// <summary>
        /// Compares all routes possible against a reference implementation.
        /// </summary>
        [TestMethod]
        public void TestCHSparseAgainstReference()
        {
            this.TestCompareAll("test_network.osm");
        }

        /// <summary>
        /// Compares all routes possible against a reference implementation.
        /// </summary>
        [TestMethod]
        public void TestCHSparseAgainstReferenceRealNetwork()
        {
            this.TestCompareAll("test_network_real1.osm");
        }
    }
}
