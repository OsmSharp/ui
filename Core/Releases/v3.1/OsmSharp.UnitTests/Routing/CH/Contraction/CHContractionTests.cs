using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmSharp.Osm.Core;
using System.IO;
using OsmSharp.Routing.Core.Graph.Memory;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Osm.Routing.Data.Processing;
using OsmSharp.Osm.Routing.Interpreter;
using OsmSharp.Osm.Data.XML.Processor;
using OsmSharp.Osm.Data.Core.Processor.Filter.Sort;
using System.Reflection;
using OsmSharp.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;

namespace OsmSharp.UnitTests.Routing.CH.Contraction
{
    /// <summary>
    /// Contains regression tests on the contraction code.
    /// </summary>
    [TestClass]
    public class CHContractionTests
    {
        /// <summary>
        /// Tests contraction of one node.
        /// </summary>
        [TestMethod]
        public void TestCHContractionTest1()
        {
            // 
            // (-1/1)---------(-3/2)--------(-5/3)
            //

            // build the data.
            MemoryRouterDataSource<CHEdgeData> data = this.BuildData(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.UnitTests.Routing.CH.Contraction.contraction_test1.osm"));

            // build the witness calculator.
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(data);

            // test the ordering operators.
            SparseOrdering sparse_ordering = new SparseOrdering(
                data);
            Assert.AreEqual(-1, sparse_ordering.Calculate(2));
            EdgeDifference edge_difference_ordering = new EdgeDifference(
                data, witness_calculator);
            Assert.AreEqual(0, edge_difference_ordering.Calculate(2));

            // do the actual contraction.
            CHPreProcessor pre_processor = new CHPreProcessor(
                data, edge_difference_ordering, witness_calculator);
            pre_processor.Contract(2);

            // check the neighbours of each vertex.
            HashSet<uint> neighbours = this.BuildNeighboursSet(data.GetArcs(2));
            Assert.IsTrue(neighbours.Contains(1));
            Assert.IsTrue(neighbours.Contains(3));

            neighbours = this.BuildNeighboursSet(data.GetArcs(1));
            Assert.IsTrue(neighbours.Contains(3));
            Assert.IsFalse(neighbours.Contains(2));

            neighbours = this.BuildNeighboursSet(data.GetArcs(3));
            Assert.IsTrue(neighbours.Contains(1));
            Assert.IsFalse(neighbours.Contains(2));
        }

        /// <summary>
        /// Tests contraction of one node.
        /// </summary>
        [TestMethod]
        public void TestCHContractionTest2()
        {
            // 
            // (-1/1)-------->(-3/2)------->(-5/3)
            //

            // build the data.
            MemoryRouterDataSource<CHEdgeData> data = this.BuildData(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.UnitTests.Routing.CH.Contraction.contraction_test2.osm"));

            // build the witness calculator.
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(data);

            // test the ordering operators.
            SparseOrdering sparse_ordering = new SparseOrdering(
                data);
            Assert.AreEqual(-1, sparse_ordering.Calculate(2));
            EdgeDifference edge_difference_ordering = new EdgeDifference(
                data, witness_calculator);
            Assert.AreEqual(0, edge_difference_ordering.Calculate(2));

            // do the actual contraction.
            CHPreProcessor pre_processor = new CHPreProcessor(
                data, edge_difference_ordering, witness_calculator);
            pre_processor.Contract(2);

            // check the neighbours of each vertex.
            HashSet<uint> neighbours = this.BuildNeighboursSet(data.GetArcs(2));
            Assert.IsTrue(neighbours.Contains(1));
            Assert.IsTrue(neighbours.Contains(3));

            neighbours = this.BuildNeighboursSet(data.GetArcs(1));
            Assert.IsTrue(neighbours.Contains(3));
            Assert.IsFalse(neighbours.Contains(2));

            neighbours = this.BuildNeighboursSet(data.GetArcs(3));
            Assert.IsTrue(neighbours.Contains(1));
            Assert.IsFalse(neighbours.Contains(2));
        }

        /// <summary>
        /// Tests contraction of one node.
        /// </summary>
        [TestMethod]
        public void TestCHContractionTest3()
        {
            // build the data.
            MemoryRouterDataSource<CHEdgeData> data = this.BuildData(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.UnitTests.Routing.CH.Contraction.contraction_test3.osm"));

            // build the witness calculator.
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator(data);

            // test the ordering operators.
            SparseOrdering sparse_ordering = new SparseOrdering(
                data);
            Assert.AreEqual(float.MaxValue, sparse_ordering.Calculate(2));
            EdgeDifference edge_difference_ordering = new EdgeDifference(
                data, witness_calculator);
            Assert.AreEqual(3, edge_difference_ordering.Calculate(2));

            // do the actual contraction.
            CHPreProcessor pre_processor = new CHPreProcessor(
                data, edge_difference_ordering, witness_calculator);
            pre_processor.Contract(2);

            // check the neighbours of each vertex.
            HashSet<uint> neighbours = this.BuildNeighboursSet(data.GetArcs(2));
            Assert.IsTrue(neighbours.Contains(1));
            Assert.IsTrue(neighbours.Contains(3));
            Assert.IsTrue(neighbours.Contains(4));

            neighbours = this.BuildNeighboursSet(data.GetArcs(1));
            Assert.IsTrue(neighbours.Contains(3));
            Assert.IsTrue(neighbours.Contains(4));
            Assert.IsFalse(neighbours.Contains(2));

            neighbours = this.BuildNeighboursSet(data.GetArcs(3));
            Assert.IsTrue(neighbours.Contains(1));
            Assert.IsTrue(neighbours.Contains(4));
            Assert.IsFalse(neighbours.Contains(2));
        }

        /// <summary>
        /// Builds an in-memory data source from an xml data stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private MemoryRouterDataSource<CHEdgeData> BuildData(Stream stream)
        {
            OsmRoutingInterpreter interpreter = new OsmRoutingInterpreter();
            OsmTagsIndex tags_index = new OsmTagsIndex();

            // do the data processing.
            MemoryRouterDataSource<CHEdgeData> data =
                new MemoryRouterDataSource<CHEdgeData>(tags_index);
            CHEdgeDataGraphProcessingTarget target_data = new CHEdgeDataGraphProcessingTarget(
                data, interpreter, data.TagsIndex);
            XmlDataProcessorSource data_processor_source = new XmlDataProcessorSource(stream);
            DataProcessorFilterSort sorter = new DataProcessorFilterSort();
            sorter.RegisterSource(data_processor_source);
            target_data.RegisterSource(sorter);
            target_data.Pull();

            return data;
        }

        /// <summary>
        /// Builds the neighbours set.
        /// </summary>
        /// <param name="neighbours"></param>
        /// <returns></returns>
        private HashSet<uint> BuildNeighboursSet(KeyValuePair<uint, CHEdgeData>[] neighbours)
        {
            HashSet<uint> neighbours_set = new HashSet<uint>();
            foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours)
            {
                neighbours_set.Add(neighbour.Key);
            }
            return neighbours_set;
        }
    }
}
