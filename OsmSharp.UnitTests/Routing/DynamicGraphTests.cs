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
using NUnit.Framework;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Graphs;

namespace OsmSharp.UnitTests.Routing
{
    /// <summary>
    /// Tests a simple weighed dynamic graph.
    /// </summary>
    [TestFixture]
    public class LiveEdgeDynamicGraphTests
    {
        /// <summary>
        /// Returns a graph.
        /// </summary>
        /// <returns></returns>
        protected IDynamicGraph<LiveEdge> CreateGraph()
        {
            return new MemoryDynamicGraph<LiveEdge>();
        }

        /// <summary>
        /// Tests adding a vertex.
        /// </summary>
        [Test]
        public void TestLiveEdgeDynamicGraphVertex()
        {
            IDynamicGraph<LiveEdge> graph = this.CreateGraph();
            uint vertex = graph.AddVertex(51, 4);

            float latitude, longitude;
            graph.GetVertex(vertex, out latitude, out longitude);

            Assert.AreEqual(51, latitude);
            Assert.AreEqual(4, longitude);

            KeyValuePair<uint, LiveEdge>[] arcs = graph.GetArcs(vertex);
            Assert.AreEqual(0, arcs.Length);
        }

        /// <summary>
        /// Tests adding 10000 vertices.
        /// </summary>
        [Test]
        public void TestLiveEdgeDynamicGraphVertex10000()
        {
            IDynamicGraph<LiveEdge> graph = this.CreateGraph();
            int count = 10000;
            while (count > 0)
            {
                uint vertex = graph.AddVertex(51, 4);

                float latitude, longitude;
                graph.GetVertex(vertex, out latitude, out longitude);

                Assert.AreEqual(51, latitude);
                Assert.AreEqual(4, longitude);

                KeyValuePair<uint, LiveEdge>[] arcs = graph.GetArcs(vertex);
                Assert.AreEqual(0, arcs.Length);

                count--;
            }

            Assert.AreEqual((uint)10000, graph.VertexCount);
        }

        /// <summary>
        /// Tests adding an edge.
        /// </summary>
        [Test]
        public void TestLiveEdgeDynamicGraphEdge()
        {
            IDynamicGraph<LiveEdge> graph = this.CreateGraph();
            uint vertex1 = graph.AddVertex(51, 1);
            uint vertex2 = graph.AddVertex(51, 2);

            graph.AddArc(vertex1, vertex2, new LiveEdge()
                                               {
                                                   Forward = true,
                                                   Tags = 0
                                               }, null);

            KeyValuePair<uint, LiveEdge>[] arcs = graph.GetArcs(vertex1);
            Assert.AreEqual(1, arcs.Length);
            Assert.AreEqual(0, arcs[0].Value.Tags);
            Assert.AreEqual(vertex2, arcs[0].Key);

            graph.AddArc(vertex2, vertex1, new LiveEdge()
            {
                Forward = true,
                Tags = 0
            }, null);

            arcs = graph.GetArcs(vertex2);
            Assert.AreEqual(1, arcs.Length);
            Assert.AreEqual(0, arcs[0].Value.Tags);
            Assert.AreEqual(vertex1, arcs[0].Key);
        }


        /// <summary>
        /// Tests adding 10000 edges.
        /// </summary>
        [Test]
        public void TestLiveEdgeDynamicGraphEdge10000()
        {
            int count = 10000;
            IDynamicGraph<LiveEdge> graph = this.CreateGraph();
            uint vertex1 = graph.AddVertex(51, 1);
            while (count > 0)
            {
                uint vertex2 = graph.AddVertex(51, 1);

                graph.AddArc(vertex1, vertex2, new LiveEdge()
                                                   {
                                                       Tags = 0,
                                                       Forward =  false
                                                   }, null);

                KeyValuePair<uint, LiveEdge>[] arcs = graph.GetArcs(vertex1);
                Assert.AreEqual(10000 - count + 1, arcs.Length);

                graph.AddArc(vertex2, vertex1, new LiveEdge()
                                                    {
                                                        Tags = 0,
                                                        Forward = false
                                                    }, null);

                arcs = graph.GetArcs(vertex2);
                Assert.AreEqual(1, arcs.Length);
                Assert.AreEqual(0, arcs[0].Value.Tags);
                Assert.AreEqual(vertex1, arcs[0].Key);

                count--;
            }
        }
    }
}
