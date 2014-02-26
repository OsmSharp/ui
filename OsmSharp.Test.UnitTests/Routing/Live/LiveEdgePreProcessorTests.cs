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
using OsmSharp.Routing.Osm.Graphs.PreProcessing;

namespace OsmSharp.Test.Unittests.Routing.Live
{
    /// <summary>
    /// Contains tests for the live edge pre processor tests.
    /// </summary>
    [TestFixture]
    public class LiveEdgePreProcessorTests
    {
        /// <summary>
        /// Tests a simple removal of one sparse vertex.
        /// from    1 <-> 2 <-> 3
        ///     
        /// to      1 <-> 3
        /// </summary>
        [Test]
        public void TestSparseRemoval1()
        {
            // use one edge definition everywhere.
            var edge = new LiveEdge();
            edge.Forward = true;
            edge.Tags = 1;

            var graph = new MemoryDynamicGraph<LiveEdge>();
            uint vertex1 = graph.AddVertex(0, 0);
            uint vertex2 = graph.AddVertex(1, 1);
            uint vertex3 = graph.AddVertex(2, 2);

            graph.AddArc(vertex1, vertex2, edge, null);
            graph.AddArc(vertex2, vertex1, edge, null);
            graph.AddArc(vertex2, vertex3, edge, null);
            graph.AddArc(vertex3, vertex2, edge, null);

            // execute pre-processor.
            var preProcessor = new LiveEdgePreprocessor(graph);
            preProcessor.Start();

            // test resulting graph.
            Assert.AreEqual(2, graph.VertexCount);
            Assert.AreEqual(1, graph.GetArcs(1).Length);
            Assert.AreEqual(2, graph.GetArcs(1)[0].Key);
            Assert.AreEqual(1, graph.GetArcs(2).Length);
            Assert.AreEqual(1, graph.GetArcs(2)[0].Key);
        }
    }
}