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
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Osm.Graphs;
using System.Collections.Generic;

namespace OsmSharp.Test.Unittests.Routing.Graph
{
    /// <summary>
    /// Tests a simple weighed write-only dynamic graph.
    /// </summary>
    [TestFixture]
    public class MemoryDynamicGraphWriteOnlyTests
    {
        /// <summary>
        /// Tests adding a vertex.
        /// </summary>
        [Test]
        public void TestLiveEdgeDynamicGraphVertex()
        {
            var graph = new MemoryDynamicGraphWriteOnly<LiveEdge>();
            var vertex = graph.AddVertex(51, 4);

            float latitude, longitude;
            graph.GetVertex(vertex, out latitude, out longitude);

            Assert.AreEqual(51, latitude);
            Assert.AreEqual(4, longitude);

            var arcs = graph.GetArcs(vertex);
            Assert.AreEqual(0, arcs.Length);
        }

        /// <summary>
        /// Tests adding 10000 vertices.
        /// </summary>
        [Test]
        public void TestLiveEdgeDynamicGraphVertex10000()
        {
            var graph = new MemoryDynamicGraphWriteOnly<LiveEdge>();
            int count = 10000;
            while (count > 0)
            {
                var vertex = graph.AddVertex(51, 4);

                float latitude, longitude;
                graph.GetVertex(vertex, out latitude, out longitude);

                Assert.AreEqual(51, latitude);
                Assert.AreEqual(4, longitude);

                var arcs = graph.GetArcs(vertex);
                Assert.AreEqual(0, arcs.Length);

                count--;
            }

            Assert.AreEqual((uint)10000, graph.VertexCount);
        }

        /// <summary>
        /// Tests adding an edge and the reverse edge.
        /// </summary>
        [Test]
        public void TestLiveEdgeDynamicGraphEdge()
        {
            uint tagsId = 10;
            var graph = new MemoryDynamicGraphWriteOnly<LiveEdge>();
            var vertex1 = graph.AddVertex(51, 1);
            var vertex2 = graph.AddVertex(51, 2);

            graph.AddArc(vertex1, vertex2, new LiveEdge()
            {
                Forward = true,
                Tags = tagsId
            }, null);

            // test forward edge.
            var arcs = graph.GetArcs(vertex1);
            Assert.AreEqual(1, arcs.Length);
            Assert.AreEqual(tagsId, arcs[0].Value.Tags);
            Assert.AreEqual(vertex2, arcs[0].Key);
            Assert.AreEqual(true, arcs[0].Value.Forward);

            // test backward edge: backward edge is added automatically.
            arcs = graph.GetArcs(vertex2);
            Assert.AreEqual(1, arcs.Length);
            Assert.AreEqual(tagsId, arcs[0].Value.Tags);
            Assert.AreEqual(vertex1, arcs[0].Key);
            Assert.AreEqual(false, arcs[0].Value.Forward);

            // add a third vertex.
            var vertex3 = graph.AddVertex(51, 2);
            var edge = new LiveEdge()
            {
                Forward = true,
                Tags = tagsId
            };
            graph.AddArc(vertex1, vertex3, edge, null);

            // test forward edges.
            arcs = graph.GetArcs(vertex1);
            Assert.AreEqual(2, arcs.Length);
            Assert.AreEqual(tagsId, arcs[0].Value.Tags);
            Assert.AreEqual(vertex2, arcs[0].Key);
            Assert.AreEqual(true, arcs[0].Value.Forward);
            Assert.AreEqual(tagsId, arcs[1].Value.Tags);
            Assert.AreEqual(vertex3, arcs[1].Key);
            Assert.AreEqual(true, arcs[1].Value.Forward);

            // test backward edge: backward edge is added automatically.
            arcs = graph.GetArcs(vertex3);
            Assert.AreEqual(1, arcs.Length);
            Assert.AreEqual(tagsId, arcs[0].Value.Tags);
            Assert.AreEqual(vertex1, arcs[0].Key);
            Assert.AreEqual(false, arcs[0].Value.Forward);
        }
    }
}