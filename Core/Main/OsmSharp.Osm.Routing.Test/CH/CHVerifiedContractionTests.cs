using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Routing.Core.Graph.Memory;
using OsmSharp.Routing.Core.Interpreter;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Osm.Routing.Interpreter;
using OsmSharp.Osm.Core;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Osm.Data.XML.Raw.Processor;
using System.Reflection;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
using OsmSharp.Routing.Core.Graph.Path;
using OsmSharp.Routing.CH.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using OsmSharp.Routing.CH.PreProcessing.Ordering;

namespace OsmSharp.Osm.Routing.Test.CH
{

    /// <summary>
    /// Executes the CH contractions while verifying each step.
    /// </summary>
    [TestClass]
    public class CHVerifiedContractionBaseTests
    {
        /// <summary>
        /// Executes the tests.
        /// </summary>
        public static void Execute()
        {
            //CHVerifiedContractionBaseTests.ExecuteSparse("OsmSharp.Osm.Routing.Test.TestData.matrix.osm");
            CHVerifiedContractionBaseTests.ExecuteEdgeDifference("OsmSharp.Osm.Routing.Test.TestData.matrix.osm");
            //CHVerifiedContractionBaseTests.Execute("OsmSharp.Osm.Routing.Test.TestData.matrix_big_area.osm");
            //CHVerifiedContractionBaseTests.Execute("OsmSharp.Osm.Routing.Test.TestData.lebbeke.osm");
            //CHVerifiedContractionBaseTests.Execute("OsmSharp.Osm.Routing.Test.TestData.eeklo.osm");
            //CHVerifiedContractionBaseTests.Execute("OsmSharp.Osm.Routing.Test.TestData.moscow.osm");
        }

        /// <summary>
        /// Executes the tests.
        /// </summary>
        /// <param name="xml"></param>
        private static void ExecuteEdgeDifference(string xml)
        {
            Tools.Core.Output.OutputStreamHost.WriteLine(xml);

            CHVerifiedContractionBaseTests tester = new CHVerifiedContractionBaseTests();
            tester.DoTestCHEdgeDifferenceVerifiedContraction(xml);
        }

        /// <summary>
        /// Executes the tests.
        /// </summary>
        /// <param name="xml"></param>
        private static void ExecuteSparse(string xml)
        {
            Tools.Core.Output.OutputStreamHost.WriteLine(xml);

            CHVerifiedContractionBaseTests tester = new CHVerifiedContractionBaseTests();
            tester.DoTestCHSparseVerifiedContraction(xml);
        }

        #region Testing Code

        /// <summary>
        /// Holds the data.
        /// </summary>
        private MemoryRouterDataSource<CHEdgeData> _data;

        /// <summary>
        /// Holds the interpreter.
        /// </summary>
        private IRoutingInterpreter _interpreter;

        /// <summary>
        /// Executes the CH contractions while verifying each step.
        /// </summary>
        /// <param name="stream"></param>
        private void DoTestCHSparseVerifiedContraction(string xml)
        {
            this.DoTestCHSparseVerifiedContraction(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(xml));
        }

        /// <summary>
        /// Executes the CH contractions while verifying each step.
        /// </summary>
        /// <param name="xml"></param>
        public void DoTestCHSparseVerifiedContraction(Stream stream)
        {
            _interpreter = new OsmRoutingInterpreter();

            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            _data = new MemoryRouterDataSource<CHEdgeData>(tags_index);
            CHEdgeDataGraphProcessingTarget target_data = new CHEdgeDataGraphProcessingTarget(
                _data, _interpreter, _data.TagsIndex);
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(stream);
            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(data_processor_source);
            target_data.RegisterSource(sorter);
            target_data.Pull();

            // do the pre-processing part.
            CHPreProcessor pre_processor = new CHPreProcessor(_data,
                new SparseOrdering(_data), new DykstraWitnessCalculator(_data));
            pre_processor.OnBeforeContractionEvent += new CHPreProcessor.VertexDelegate(pre_processor_OnBeforeContractionEvent);
            pre_processor.OnAfterContractionEvent += new CHPreProcessor.VertexDelegate(pre_processor_OnAfterContractionEvent);
            pre_processor.Start();
        }
        
        /// <summary>
        /// Executes the CH contractions while verifying each step.
        /// </summary>
        /// <param name="stream"></param>
        private void DoTestCHEdgeDifferenceVerifiedContraction(string xml)
        {
            this.DoTestCHSparseVerifiedContraction(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(xml));
        }

        /// <summary>
        /// Executes the CH contractions while verifying each step.
        /// </summary>
        /// <param name="xml"></param>
        public void DoTestCHEdgeDifferenceVerifiedContraction(Stream stream)
        {
            _interpreter = new OsmRoutingInterpreter();

            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            _data = new MemoryRouterDataSource<CHEdgeData>(tags_index);
            CHEdgeDataGraphProcessingTarget target_data = new CHEdgeDataGraphProcessingTarget(
                _data, _interpreter, _data.TagsIndex);
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(stream);
            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(data_processor_source);
            target_data.RegisterSource(sorter);
            target_data.Pull();

            // do the pre-processing part.
            DykstraWitnessCalculator witness_calculator = new DykstraWitnessCalculator(
                _data);
            CHPreProcessor pre_processor = new CHPreProcessor(_data,
                new EdgeDifference(_data, witness_calculator), witness_calculator);
            pre_processor.OnBeforeContractionEvent += new CHPreProcessor.VertexDelegate(pre_processor_OnBeforeContractionEvent);
            pre_processor.OnAfterContractionEvent += new CHPreProcessor.VertexDelegate(pre_processor_OnAfterContractionEvent);
            pre_processor.Start();
        }

        /// <summary>
        /// Holds the paths calculate before contraction.
        /// </summary>
        private Dictionary<uint, Dictionary<uint, PathSegment<long>>> _paths_before_contraction;

        /// <summary>
        /// Called right after the contraction.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="edges"></param>
        void pre_processor_OnAfterContractionEvent(uint vertex, KeyValuePair<uint, CHEdgeData>[] edges)
        {
            // create a new CHRouter
            CHRouter router = new CHRouter(_data);

            // calculate all the routes between the neighbours of the contracted vertex.
            foreach (KeyValuePair<uint, CHEdgeData> from in edges)
            {
                // initialize the from-list.
                PathSegmentVisitList from_list = new PathSegmentVisitList();
                from_list.UpdateVertex(new PathSegment<long>(from.Key));

                // initalize the from dictionary.
                Dictionary<uint, PathSegment<long>> from_dic = _paths_before_contraction[from.Key];
                foreach (KeyValuePair<uint, CHEdgeData> to in edges)
                {
                    // initialize the to-list.
                    PathSegmentVisitList to_list = new PathSegmentVisitList();
                    to_list.UpdateVertex(new PathSegment<long>(to.Key));

                    // calculate the route.
                    PathSegment<long> route = router.Calculate(_data, _interpreter, from_list, to_list, double.MaxValue);
                    if ((from_dic[to.Key] == null && route != null) ||
                        (from_dic[to.Key] != null && route == null) ||
                        ((from_dic[to.Key] != null && route != null) && from_dic[to.Key] != route))
                    { // the route match!
                        Assert.Fail("Routes are different before/after contraction!");
                    }
                }
            }
        }

        /// <summary>
        /// Called left before the contraction.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="edges"></param>
        void pre_processor_OnBeforeContractionEvent(uint vertex, KeyValuePair<uint, CHEdgeData>[] edges)
        {
            // create a new CHRouter
            CHRouter router = new CHRouter(_data);

            // calculate all the routes between the neighbours of the contracted vertex.
            _paths_before_contraction =
                new Dictionary<uint, Dictionary<uint, PathSegment<long>>>();
            foreach (KeyValuePair<uint, CHEdgeData> from in edges)
            {
                // initialize the from-list.
                PathSegmentVisitList from_list = new PathSegmentVisitList();
                from_list.UpdateVertex(new PathSegment<long>(from.Key));

                // initalize the from dictionary.
                Dictionary<uint, PathSegment<long>> from_dic = new Dictionary<uint, PathSegment<long>>();
                _paths_before_contraction[from.Key] = from_dic;
                foreach (KeyValuePair<uint, CHEdgeData> to in edges)
                {
                    // initialize the to-list.
                    PathSegmentVisitList to_list = new PathSegmentVisitList();
                    to_list.UpdateVertex(new PathSegment<long>(to.Key));

                    // calculate the route.
                    PathSegment<long> route = router.Calculate(_data, _interpreter, from_list, to_list, double.MaxValue);
                    from_dic[to.Key] = route;
                }
            }
        }

        #endregion
    }
}
