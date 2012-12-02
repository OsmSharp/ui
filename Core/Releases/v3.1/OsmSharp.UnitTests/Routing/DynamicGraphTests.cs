using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OsmSharp.Routing.Core.Graph.DynamicGraph.SimpleWeighed;
using OsmSharp.Routing.Core.Graph.DynamicGraph;
using OsmSharp.Routing.Core.Graph.DynamicGraph.Memory;

namespace OsmSharp.UnitTests.Routing
{
    /// <summary>
    /// Tests a simple weighed dynamic graph.
    /// </summary>
    [TestClass]
    public class SimpleWeighedDynamicGraphTests
    {
        /// <summary>
        /// Returns a graph.
        /// </summary>
        /// <returns></returns>
        protected IDynamicGraph<SimpleWeighedEdge> CreateGraph()
        {
            return new MemoryDynamicGraph<SimpleWeighedEdge>();
        }

        /// <summary>
        /// Tests adding a vertex.
        /// </summary>
        [TestMethod]
        public void TestSimpleWeighedDynamicGraphVertex()
        {
            SimpleWeighedDynamicGraph graph = new SimpleWeighedDynamicGraph();
            uint vertex = graph.AddVertex(51, 4);

            float latitude, longitude;
            graph.GetVertex(vertex, out latitude, out longitude);

            Assert.AreEqual(51, latitude);
            Assert.AreEqual(4, longitude);

            KeyValuePair<uint, SimpleWeighedEdge>[] arcs = graph.GetArcs(vertex);
            Assert.AreEqual(0, arcs.Length);
        }

        /// <summary>
        /// Tests adding 10000 vertices.
        /// </summary>
        [TestMethod]
        public void TestSimpleWeighedDynamicGraphVertex10000()
        {
            SimpleWeighedDynamicGraph graph = new SimpleWeighedDynamicGraph();
            int count = 10000;
            while (count > 0)
            {
                uint vertex = graph.AddVertex(51, 4);

                float latitude, longitude;
                graph.GetVertex(vertex, out latitude, out longitude);

                Assert.AreEqual(51, latitude);
                Assert.AreEqual(4, longitude);

                KeyValuePair<uint, SimpleWeighedEdge>[] arcs = graph.GetArcs(vertex);
                Assert.AreEqual(0, arcs.Length);

                count--;
            }

            Assert.AreEqual((uint)10000, graph.VertexCount);
        }

        /// <summary>
        /// Tests adding an edge.
        /// </summary>
        [TestMethod]
        public void TestSimpleWeighedDynamicGraphEdge()
        {
            SimpleWeighedDynamicGraph graph = new SimpleWeighedDynamicGraph();
            uint vertex1 = graph.AddVertex(51, 1);
            uint vertex2 = graph.AddVertex(51, 2);

            graph.AddArc(vertex1, vertex2, new SimpleWeighedEdge(
                100, true, true, 0));

            KeyValuePair<uint, SimpleWeighedEdge>[] arcs = graph.GetArcs(vertex1);
            Assert.AreEqual(1, arcs.Length);
            Assert.AreEqual(100, arcs[0].Value.Weight);
            Assert.AreEqual(vertex2, arcs[0].Key);

            graph.AddArc(vertex2, vertex1, new SimpleWeighedEdge(
                200, true, true, 0));

            arcs = graph.GetArcs(vertex2);
            Assert.AreEqual(1, arcs.Length);
            Assert.AreEqual(200, arcs[0].Value.Weight);
            Assert.AreEqual(vertex1, arcs[0].Key);
        }


        /// <summary>
        /// Tests adding 10000 edges.
        /// </summary>
        [TestMethod]
        public void TestSimpleWeighedDynamicGraphEdge10000()
        {
            int count = 10000;
            SimpleWeighedDynamicGraph graph = new SimpleWeighedDynamicGraph();
            uint vertex1 = graph.AddVertex(51, 1);
            while (count > 0)
            {
                uint vertex2 = graph.AddVertex(51, 1);

                graph.AddArc(vertex1, vertex2, new SimpleWeighedEdge(
                    100, true, true, 0));

                KeyValuePair<uint, SimpleWeighedEdge>[] arcs = graph.GetArcs(vertex1);
                Assert.AreEqual(10000 - count + 1, arcs.Length);

                graph.AddArc(vertex2, vertex1, new SimpleWeighedEdge(
                    200, true, true, 0));

                arcs = graph.GetArcs(vertex2);
                Assert.AreEqual(1, arcs.Length);
                Assert.AreEqual(200, arcs[0].Value.Weight);
                Assert.AreEqual(vertex1, arcs[0].Key);

                count--;
            }
        }
    }
}
