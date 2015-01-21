// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using System.Reflection;
using NUnit.Framework;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Routing.CH.PreProcessing.Witnesses;
using OsmSharp.Routing.CH.PreProcessing.Ordering;
using OsmSharp.Routing.CH;

namespace OsmSharp.Test.Unittests.Routing.CH
{
    /// <summary>
    /// Executes the CH contractions while verifying each step.
    /// </summary>
    [TestFixture]
    public class CHVerifiedContractionTests : CHVerifiedContractionBaseTests
    {
        /// <summary>
        /// Tests the simplest contraction possible without any witnesses.
        /// </summary>
        /// <remarks>
        /// Network: 
        /// 
        ///              (1)        (2)
        ///                \        /
        ///                10s    10s
        ///                  \    /
        ///                   \  /
        ///                   (3)
        ///                   
        /// To test: After contraction of vertex 3 these routes should still be found:
        ///     - (1)->(2): (1)-10s-(3)-10s-(2) (20s in total).
        ///     - (2)->(1): (2)-10s-(3)-10s-(1) (20s in total).
        /// </remarks>
        [Test]
        public void TestVerifiedContraction1NoWitnesses()
        {
            var graph = new MemoryDirectedGraph<CHEdgeData>();
            var vertex1 = graph.AddVertex(1, 0);
            var vertex2 = graph.AddVertex(2, 0);
            var vertex3 = graph.AddVertex(3, 0);

            graph.AddEdge(vertex1, vertex3, new CHEdgeData(1, true, true, true, 10));
            graph.AddEdge(vertex3, vertex1, new CHEdgeData(1, false, true, true, 10));
            graph.AddEdge(vertex2, vertex3, new CHEdgeData(1, true, true, true, 10));
            graph.AddEdge(vertex3, vertex2, new CHEdgeData(1, false, true, true, 10));

            var witnessCalculator = new DykstraWitnessCalculator();
            var preProcessor = new CHPreProcessor(graph,
                new EdgeDifferenceContractedSearchSpace(graph, witnessCalculator), witnessCalculator);
            preProcessor.Contract(3);

            var router = new CHRouter();
            // expected: (1)-10s-(3)-10s-(2) (20s in total).
            var path = router.Calculate(graph, 1, 2);
            Assert.IsNotNull(path);
            Assert.AreEqual(20, path.Weight);
            var pathArray = path.ToArrayWithWeight();
            Assert.AreEqual(3, pathArray.Length);
            float latitude, longitude;
            Assert.AreEqual(0, pathArray[0].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[0].Item1, out latitude, out longitude));
            Assert.AreEqual(1, latitude);
            Assert.AreEqual(10, pathArray[1].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[1].Item1, out latitude, out longitude));
            Assert.AreEqual(3, latitude);
            Assert.AreEqual(20, pathArray[2].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[2].Item1, out latitude, out longitude));
            Assert.AreEqual(2, latitude);

            // expected: (2)-10s-(3)-10s-(1) (20s in total).
            path = router.Calculate(graph, 2, 1);
            Assert.IsNotNull(path);
            Assert.AreEqual(20, path.Weight);
            pathArray = path.ToArrayWithWeight();
            Assert.AreEqual(3, pathArray.Length);
            Assert.AreEqual(0, pathArray[0].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[0].Item1, out latitude, out longitude));
            Assert.AreEqual(2, latitude);
            Assert.AreEqual(10, pathArray[1].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[1].Item1, out latitude, out longitude));
            Assert.AreEqual(3, latitude);
            Assert.AreEqual(20, pathArray[2].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[2].Item1, out latitude, out longitude));
            Assert.AreEqual(1, latitude);
        }

        /// <summary>
        /// Tests the simplest contraction possible without any witnesses.
        /// </summary>
        /// <remarks>
        /// Network: 
        /// 
        ///              (1)---15s---(2)
        ///                \        /
        ///                10s    10s
        ///                  \    /
        ///                   \  /
        ///                   (3)
        ///                   
        /// To test: After contraction of vertex 3 these routes should still be found:
        ///     - (1)->(2): (1)-15s-(2) (15s in total).
        ///     - (2)->(1): (2)-15s-(1) (15s in total).
        /// </remarks>
        [Test]
        public void TestVerifiedContraction2Witness()
        {
            var graph = new MemoryDirectedGraph<CHEdgeData>();
            var vertex1 = graph.AddVertex(1, 0);
            var vertex2 = graph.AddVertex(2, 0);
            var vertex3 = graph.AddVertex(3, 0);

            graph.AddEdge(vertex1, vertex2, new CHEdgeData(1, true, true, true, 15));
            graph.AddEdge(vertex2, vertex1, new CHEdgeData(1, false, true, true, 15));
            graph.AddEdge(vertex1, vertex3, new CHEdgeData(1, true, true, true, 10));
            graph.AddEdge(vertex3, vertex1, new CHEdgeData(1, false, true, true, 10));
            graph.AddEdge(vertex2, vertex3, new CHEdgeData(1, true, true, true, 10));
            graph.AddEdge(vertex3, vertex2, new CHEdgeData(1, false, true, true, 10));

            var witnessCalculator = new DykstraWitnessCalculator();
            var preProcessor = new CHPreProcessor(graph,
                new EdgeDifferenceContractedSearchSpace(graph, witnessCalculator), witnessCalculator);
            preProcessor.Contract(3);

            var router = new CHRouter();
            // expected: (1)-15s-(2) (15s in total).
            var path = router.Calculate(graph, 1, 2);
            Assert.IsNotNull(path);
            Assert.AreEqual(15, path.Weight);
            var pathArray = path.ToArrayWithWeight();
            Assert.AreEqual(2, pathArray.Length);
            float latitude, longitude;
            Assert.AreEqual(0, pathArray[0].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[0].Item1, out latitude, out longitude));
            Assert.AreEqual(1, latitude);
            Assert.AreEqual(15, pathArray[1].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[1].Item1, out latitude, out longitude));
            Assert.AreEqual(2, latitude);

            // expected: (2)-15s-(1) (15s in total).
            path = router.Calculate(graph, 2, 1);
            Assert.IsNotNull(path);
            Assert.AreEqual(15, path.Weight);
            pathArray = path.ToArrayWithWeight();
            Assert.AreEqual(2, pathArray.Length);
            Assert.AreEqual(0, pathArray[0].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[0].Item1, out latitude, out longitude));
            Assert.AreEqual(2, latitude);
            Assert.AreEqual(15, pathArray[1].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[1].Item1, out latitude, out longitude));
            Assert.AreEqual(1, latitude);
        }

        /// <summary>
        /// Tests a tiny network with a contraction that is supposed to replace a one-way edge in only oneway.
        /// </summary>
        /// <remarks>
        /// Network: 
        /// 
        ///    (4)--10s--(1)->-10s->-(2)--10s--(5)
        ///                \        /
        ///                10s    10s
        ///                  \    /
        ///                   \  /
        ///                   (3)
        ///                   
        /// To test: After contraction of vertex 3 these routes should still be found:
        ///     - (4)->(5): (4)-10s-(1)-10s-(2)-10s-(5) (30s in total).
        ///     - (5)->(4): (5)-10s-(2)-10s-(3)-10s-(1)-10s-(4) (40s in total).
        /// </remarks>
        [Test]
        public void TestVerifiedContraction3TinyOneWay()
        {
            var graph = new MemoryDirectedGraph<CHEdgeData>();
            var vertex1 = graph.AddVertex(1, 0);
            var vertex2 = graph.AddVertex(2, 0);
            var vertex3 = graph.AddVertex(3, 0);
            var vertex4 = graph.AddVertex(4, 0);
            var vertex5 = graph.AddVertex(5, 0);

            graph.AddEdge(vertex1, vertex4, new CHEdgeData(1, true, true, true, 10));
            graph.AddEdge(vertex4, vertex1, new CHEdgeData(1, false, true, true, 10));
            graph.AddEdge(vertex1, vertex2, new CHEdgeData(1, true, true, false, 10)); // oneway forward
            graph.AddEdge(vertex2, vertex1, new CHEdgeData(1, false, false, true, 10)); // oneway backward.
            graph.AddEdge(vertex1, vertex3, new CHEdgeData(1, true, true, true, 10));
            graph.AddEdge(vertex3, vertex1, new CHEdgeData(1, false, true, true, 10));
            graph.AddEdge(vertex2, vertex3, new CHEdgeData(1, true, true, true, 10));
            graph.AddEdge(vertex3, vertex2, new CHEdgeData(1, false, true, true, 10));
            graph.AddEdge(vertex2, vertex5, new CHEdgeData(1, true, true, true, 10));
            graph.AddEdge(vertex5, vertex2, new CHEdgeData(1, false, true, true, 10));

            var witnessCalculator = new DykstraWitnessCalculator();
            var preProcessor = new CHPreProcessor(graph, 
                new EdgeDifferenceContractedSearchSpace(graph, witnessCalculator), witnessCalculator);
            preProcessor.Start();

            var router = new CHRouter();
            // expected: (4)-10s-(1)-10s-(2)-10s-(3) (30s in total).
            var path = router.Calculate(graph, 4, 5);
            Assert.IsNotNull(path);
            Assert.AreEqual(30, path.Weight);
            var pathArray = path.ToArrayWithWeight();
            Assert.AreEqual(4, pathArray.Length);
            float latitude, longitude;
            Assert.AreEqual(0, pathArray[0].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[0].Item1, out latitude, out longitude));
            Assert.AreEqual(4, latitude);
            Assert.AreEqual(10, pathArray[1].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[1].Item1, out latitude, out longitude));
            Assert.AreEqual(1, latitude);
            Assert.AreEqual(20, pathArray[2].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[2].Item1, out latitude, out longitude));
            Assert.AreEqual(2, latitude);
            Assert.AreEqual(30, pathArray[3].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[3].Item1, out latitude, out longitude));
            Assert.AreEqual(5, latitude);

            // expected: (5)-10s-(2)-10s-(3)-10s-(1)-10s-(4) (40s in total).
            path = router.Calculate(graph, 5, 4);
            Assert.IsNotNull(path);
            Assert.AreEqual(40, path.Weight);
            pathArray = path.ToArrayWithWeight();
            Assert.AreEqual(5, pathArray.Length);
            Assert.AreEqual(0, pathArray[0].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[0].Item1, out latitude, out longitude));
            Assert.AreEqual(5, latitude);
            Assert.AreEqual(10, pathArray[1].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[1].Item1, out latitude, out longitude));
            Assert.AreEqual(2, latitude);
            Assert.AreEqual(20, pathArray[2].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[2].Item1, out latitude, out longitude));
            Assert.AreEqual(3, latitude);
            Assert.AreEqual(30, pathArray[3].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[3].Item1, out latitude, out longitude));
            Assert.AreEqual(1, latitude);
            Assert.AreEqual(40, pathArray[4].Item2);
            Assert.IsTrue(graph.GetVertex((uint)pathArray[4].Item1, out latitude, out longitude));
            Assert.AreEqual(4, latitude);
        }

        /// <summary>
        /// Tests a tiny network with a contraction that is supposed to replace a one-way edge in only oneway.
        /// </summary>
        /// <remarks>
        /// Network: 
        ///                   (4)
        ///                   /  \
        ///                  /    \
        ///                15s    15s
        ///                /        \
        ///               (1)       (2)
        ///                \        /
        ///                10s    10s
        ///                  \    /
        ///                   \  /
        ///                   (3)
        ///                   
        /// To test: 
        ///  - After contraction of vertex 4 there should be a new edge (1)-(2) with a weight of 30.
        ///  - After contraction of vertex 3 there should be a new edge (1)-(2) with a weight of 20 and this edge should have replace the previous one.
        /// </remarks>
        [Test]
        public void TestVerifiedContraction4ReplacePrevious()
        {
            var graph = new MemoryDirectedGraph<CHEdgeData>();
            var vertex1 = graph.AddVertex(1, 0);
            var vertex2 = graph.AddVertex(2, 0);
            var vertex3 = graph.AddVertex(3, 0);
            var vertex4 = graph.AddVertex(4, 0);

            graph.AddEdge(vertex1, vertex4, new CHEdgeData(1, true, true, true, 15));
            graph.AddEdge(vertex4, vertex1, new CHEdgeData(1, false, true, true, 15));
            graph.AddEdge(vertex2, vertex4, new CHEdgeData(1, true, true, true, 15));
            graph.AddEdge(vertex4, vertex2, new CHEdgeData(1, false, true, true, 15));

            var witnessCalculator = new DykstraWitnessCalculator();
            var preProcessor = new CHPreProcessor(graph,
                new EdgeDifferenceContractedSearchSpace(graph, witnessCalculator), witnessCalculator);
            preProcessor.Contract(4);

            // add edges later to prevent witnesses from being found!
            graph.AddEdge(vertex1, vertex3, new CHEdgeData(1, true, true, true, 10));
            graph.AddEdge(vertex3, vertex1, new CHEdgeData(1, false, true, true, 10));
            graph.AddEdge(vertex2, vertex3, new CHEdgeData(1, true, true, true, 10));
            graph.AddEdge(vertex3, vertex2, new CHEdgeData(1, false, true, true, 10));

            CHEdgeData data;
            Assert.IsTrue(graph.GetEdge(vertex1, vertex2, out data));
            Assert.IsTrue(data.CanMoveBackward);
            Assert.IsTrue(data.CanMoveForward);
            Assert.AreEqual(4, data.ContractedId);
            Assert.AreEqual(30, data.Weight);

            Assert.IsTrue(graph.GetEdge(vertex2, vertex1, out data));
            Assert.IsTrue(data.CanMoveBackward);
            Assert.IsTrue(data.CanMoveForward);
            Assert.AreEqual(4, data.ContractedId);
            Assert.AreEqual(30, data.Weight);

            preProcessor.Contract(3);
            Assert.IsTrue(graph.GetEdge(vertex1, vertex2, out data));
            Assert.IsTrue(data.CanMoveBackward);
            Assert.IsTrue(data.CanMoveForward);
            Assert.AreEqual(3, data.ContractedId);
            Assert.AreEqual(20, data.Weight);

            Assert.IsTrue(graph.GetEdge(vertex2, vertex1, out data));
            Assert.IsTrue(data.CanMoveBackward);
            Assert.IsTrue(data.CanMoveForward);
            Assert.AreEqual(3, data.ContractedId);
            Assert.AreEqual(20, data.Weight);
        }

        /// <summary>
        /// Tests the contraction of the default test network by comparing all routes between all neigbours of a contracted vertex before and after contraction.
        /// </summary>
        [Test]
        public void TestVerifiedContraction5TestNetwork()
        {
            this.DoTestCHSparseVerifiedContraction(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Test.Unittests.test_network.osm"));
        }

        /// <summary>
        /// Tests the contraction of a realistic test network by comparing all routes between all neigbours of a contracted vertex before and after contraction.
        /// </summary>
        [Test]
        public void TestCHVerifiedContractionTestNetworkReal()
        {
            this.DoTestCHSparseVerifiedContraction(
                Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Test.Unittests.test_network_real1.osm"));
        }

        ///// <summary>
        ///// Executes the CH contractions while verifying each step.
        ///// </summary>
        //[Test]
        //public void TestCHVerifiedContractionTestNetworkOneWay()
        //{
        //    this.DoTestCHSparseVerifiedContraction(
        //        Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Test.Unittests.test_network_oneway.osm"));
        //}
    }
}