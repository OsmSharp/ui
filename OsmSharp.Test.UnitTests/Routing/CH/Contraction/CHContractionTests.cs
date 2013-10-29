// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Routing.CH.PreProcessing.Ordering.LimitedLevelOrdering;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;

namespace OsmSharp.Test.Unittests.Routing.CH.Contraction
{
    /// <summary>
    /// Contains regression tests on the contraction code.
    /// </summary>
    [TestFixture]
    public class CHContractionTests
    {
        /// <summary>
        /// Tests contraction of one node.
        /// </summary>
        [Test]
        public void TestCHContractionTest1()
        {
            // 
            // (-1/1)---------(-3/2)--------(-5/3)
            //

            // build the data.
            DynamicGraphRouterDataSource<CHEdgeData> data = this.BuildData(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.Test.Unittests.Routing.CH.Contraction.contraction_test1.osm"));

            // build the witness calculator.
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator();

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
        [Test]
        public void TestCHContractionTest2()
        {
            // 
            // (-1/1)-------->(-3/2)------->(-5/3)
            //

            // build the data.
            DynamicGraphRouterDataSource<CHEdgeData> data = this.BuildData(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.Test.Unittests.Routing.CH.Contraction.contraction_test2.osm"));

            // build the witness calculator.
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator();

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
        [Test]
        public void TestCHContractionTest3()
        {
            // build the data.
            DynamicGraphRouterDataSource<CHEdgeData> data = this.BuildData(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.Test.Unittests.Routing.CH.Contraction.contraction_test3.osm"));

            // build the witness calculator.
            INodeWitnessCalculator witness_calculator = new DykstraWitnessCalculator();

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
        private DynamicGraphRouterDataSource<CHEdgeData> BuildData(Stream stream)
        {
            var interpreter = new OsmRoutingInterpreter();
            var tagsIndex = new SimpleTagsIndex();

            // do the data processing.
            var data = new DynamicGraphRouterDataSource<CHEdgeData>(tagsIndex);
            var targetData = new CHEdgeGraphOsmStreamTarget(
                data, interpreter, data.TagsIndex, Vehicle.Car);
            var dataProcessorSource = new XmlOsmStreamSource(stream);
            var sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(dataProcessorSource);
            targetData.RegisterSource(sorter);
            targetData.Pull();

            return data;
        }

        /// <summary>
        /// Builds the neighbours set.
        /// </summary>
        /// <param name="neighbours"></param>
        /// <returns></returns>
        private HashSet<uint> BuildNeighboursSet(KeyValuePair<uint, CHEdgeData>[] neighbours)
        {
            neighbours = neighbours.RemoveInformativeEdges();
            var neighboursSet = new HashSet<uint>();
            foreach (KeyValuePair<uint, CHEdgeData> neighbour in neighbours)
            {
                neighboursSet.Add(neighbour.Key);
            }
            return neighboursSet;
        }
    }
}
