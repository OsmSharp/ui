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

using NUnit.Framework;
using OsmSharp.Collections.Tags.Index;
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
using System.IO;
using System.Linq;
using System.Reflection;

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
            var data = this.BuildData(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.Test.Unittests.Routing.CH.Contraction.contraction_test1.osm"));

            // build the witness calculator.
            var witnessCalculator = new DykstraWitnessCalculator();

            // test the ordering operators.
            var sparseOrdering = new SparseOrdering(
                data);
            Assert.AreEqual(-1, sparseOrdering.Calculate(2));
            var edgeDifferenceOrdering = new EdgeDifference(
                data, witnessCalculator);
            Assert.AreEqual(2, edgeDifferenceOrdering.Calculate(2));

            // do the actual contraction.
            var preProcessor = new CHPreProcessor(
                data, edgeDifferenceOrdering, witnessCalculator);
            preProcessor.Contract(2);

            // check the neighbours of each vertex.
            var neighbours = data.GetEdges(2);
            Assert.AreEqual(2, neighbours.Length);
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 1 && x.Value.ToHigher;
            }));
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 3 && x.Value.ToHigher;
            }));

            neighbours = data.GetEdges(1);
            Assert.AreEqual(2, neighbours.Length);
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 2 && x.Value.ToLower;
            }));
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 3 && !x.Value.ToHigher && !x.Value.ToLower;
            }));

            neighbours = data.GetEdges(3);
            Assert.AreEqual(2, neighbours.Length);
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 2 && x.Value.ToLower;
            }));
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 1 && !x.Value.ToHigher && !x.Value.ToLower;
            }));
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
            var data = this.BuildData(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.Test.Unittests.Routing.CH.Contraction.contraction_test2.osm"));

            // build the witness calculator.
            var witnessCalculator = new DykstraWitnessCalculator();

            // test the ordering operators.
            var sparseOrdering = new SparseOrdering(
                data);
            Assert.AreEqual(-1, sparseOrdering.Calculate(2));
            var edgeDifferenceOrdering = new EdgeDifference(
                data, witnessCalculator);
            Assert.AreEqual(0, edgeDifferenceOrdering.Calculate(2));

            // do the actual contraction.
            var preProcessor = new CHPreProcessor(
                data, edgeDifferenceOrdering, witnessCalculator);
            preProcessor.Contract(2);

            // check the neighbours of each vertex.
            var neighbours = data.GetEdges(2);
            Assert.AreEqual(2, neighbours.Length);
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 1 && x.Value.ToHigher;
            }));
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 3 && x.Value.ToHigher;
            }));

            neighbours = data.GetEdges(1);
            Assert.AreEqual(2, neighbours.Length);
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 2 && x.Value.ToLower;
            }));
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 3 && !x.Value.ToHigher && !x.Value.ToLower;
            }));

            neighbours = data.GetEdges(3);
            Assert.AreEqual(2, neighbours.Length);
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 2 && x.Value.ToLower;
            }));
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 1 && !x.Value.ToHigher && !x.Value.ToLower;
            }));
        }

        /// <summary>
        /// Tests contraction of one node.
        /// </summary>
        [Test]
        public void TestCHContractionTest3()
        {
            // build the data.
            var data = this.BuildData(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.Test.Unittests.Routing.CH.Contraction.contraction_test3.osm"));

            // build the witness calculator.
            var witnessCalculator = new DykstraWitnessCalculator();

            // test the ordering operators.
            var sparseOrdering = new SparseOrdering(
                data);
            Assert.AreEqual(float.MaxValue, sparseOrdering.Calculate(2));
            var edgeDifferenceOrdering = new EdgeDifference(
                data, witnessCalculator);
            Assert.AreEqual(3, edgeDifferenceOrdering.Calculate(2));

            // do the actual contraction.
            var preProcessor = new CHPreProcessor(
                data, edgeDifferenceOrdering, witnessCalculator);
            preProcessor.Contract(2);

            // check the neighbours of each vertex.
            var neighbours = data.GetEdges(2);
            Assert.AreEqual(3, neighbours.Length);
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 1;
            }));
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 3;
            }));
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 4;
            }));

            neighbours = data.GetEdges(1);
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 3;
            }));
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 4;
            }));
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 2;
            }));

            neighbours = data.GetEdges(3);
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 1;
            }));
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 4;
            }));
            Assert.IsTrue(neighbours.Any((x) =>
            {
                return x.Key == 2;
            }));
        }

        /// <summary>
        /// Builds an in-memory data source from an xml data stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private DynamicGraphRouterDataSource<CHEdgeData> BuildData(Stream stream)
        {
            var interpreter = new OsmRoutingInterpreter();
            var tagsIndex = new TagsTableCollectionIndex();

            // do the data processing.
            var data = new DynamicGraphRouterDataSource<CHEdgeData>(tagsIndex);
            var targetData = new CHEdgeGraphOsmStreamTarget(
                data, interpreter, tagsIndex, Vehicle.Car);
            var dataProcessorSource = new XmlOsmStreamSource(stream);
            var sorter = new OsmStreamFilterSort();
            sorter.RegisterSource(dataProcessorSource);
            targetData.RegisterSource(sorter);
            targetData.Pull();

            return data;
        }
    }
}