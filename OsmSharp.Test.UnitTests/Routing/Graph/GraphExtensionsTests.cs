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

using NUnit.Framework;
using OsmSharp.Math.Algorithms;
using OsmSharp.Math.Geo;
using OsmSharp.Routing.Graph;
using System;
using System.Collections.Generic;

namespace OsmSharp.Test.Unittests.Routing.Graph
{
    /// <summary>
    /// Contains tests for the GraphExtensions.
    /// </summary>
    [TestFixture]
    public class GraphExtensionsTests
    {
        /// <summary>
        /// Tests swichting two vertices.
        /// </summary>
        [Test]
        public void TestGraphSwitch()
        {
            var graph = new Graph<Edge>();
            var vertex1 = graph.AddVertex(1, 1);
            var vertex2 = graph.AddVertex(2, 2);

            graph.Switch(vertex1, vertex2);

            float latitude, longitude;
            graph.GetVertex(1, out latitude, out longitude);
            Assert.AreEqual(2, latitude);
            Assert.AreEqual(2, longitude);
            graph.GetVertex(2, out latitude, out longitude);
            Assert.AreEqual(1, latitude);
            Assert.AreEqual(1, longitude);

            graph = new Graph<Edge>();
            vertex1 = graph.AddVertex(1, 1);
            vertex2 = graph.AddVertex(2, 2);
            graph.AddEdge(1, 2, new Edge()
            {
                Tags = 1,
                Forward = true
            });

            graph.Switch(vertex1, vertex2);

            graph.GetVertex(1, out latitude, out longitude);
            Assert.AreEqual(2, latitude);
            Assert.AreEqual(2, longitude);
            graph.GetVertex(2, out latitude, out longitude);
            Assert.AreEqual(1, latitude);
            Assert.AreEqual(1, longitude);
            var edges = graph.GetEdges(1);
            Assert.AreEqual(1, edges.Count);
            foreach (var edge in edges)
            {
                Assert.AreEqual(2, edge.Neighbour);
                Assert.AreEqual(1, edge.EdgeData.Tags);
                Assert.IsFalse(edge.EdgeData.Forward);
            }
            edges = graph.GetEdges(2);
            Assert.AreEqual(1, edges.Count);
            foreach(var edge in edges)
            {
                Assert.AreEqual(1, edge.Neighbour);
                Assert.AreEqual(1, edge.EdgeData.Tags);
                Assert.IsTrue(edge.EdgeData.Forward);
            }
        }

        /// <summary>
        /// Tests swichting two vertices.
        /// </summary>
        [Test]
        public void TestDirectedGraphSwitch()
        {
            var graph = new DirectedGraph<Edge>();
            var vertex1 = graph.AddVertex(1, 1);
            var vertex2 = graph.AddVertex(2, 2);

            graph.Switch(vertex1, vertex2);

            float latitude, longitude;
            graph.GetVertex(1, out latitude, out longitude);
            Assert.AreEqual(2, latitude);
            Assert.AreEqual(2, longitude);
            graph.GetVertex(2, out latitude, out longitude);
            Assert.AreEqual(1, latitude);
            Assert.AreEqual(1, longitude);

            graph = new DirectedGraph<Edge>();
            vertex1 = graph.AddVertex(1, 1);
            vertex2 = graph.AddVertex(2, 2);
            graph.AddEdge(1, 2, new Edge()
            {
                Tags = 1,
                Forward = true
            });

            graph.Switch(vertex1, vertex2);

            graph.GetVertex(1, out latitude, out longitude);
            Assert.AreEqual(2, latitude);
            Assert.AreEqual(2, longitude);
            graph.GetVertex(2, out latitude, out longitude);
            Assert.AreEqual(1, latitude);
            Assert.AreEqual(1, longitude);
            var edges = graph.GetEdges(1);
            Assert.AreEqual(0, edges.Count);
            edges = graph.GetEdges(2);
            Assert.AreEqual(1, edges.Count);
            foreach (var edge in edges)
            {
                Assert.AreEqual(1, edge.Neighbour);
                Assert.AreEqual(1, edge.EdgeData.Tags);
                Assert.IsTrue(edge.EdgeData.Forward);
            }
        }


        /// <summary>
        /// Tests the sort hilbert function with order #4.
        /// </summary>
        [Test]
        public void SortHilbertTest1()
        {
            var n = 4;

            // build locations.
            var locations = new List<GeoCoordinate>();
            locations.Add(new GeoCoordinate(-90, -180));
            locations.Add(new GeoCoordinate(-90, -60));
            locations.Add(new GeoCoordinate(-90, 60));
            locations.Add(new GeoCoordinate(-90, 180));
            locations.Add(new GeoCoordinate(-30, -180));
            locations.Add(new GeoCoordinate(-30, -60));
            locations.Add(new GeoCoordinate(-30, 60));
            locations.Add(new GeoCoordinate(-30, 180));
            locations.Add(new GeoCoordinate(30, -180));
            locations.Add(new GeoCoordinate(30, -60));
            locations.Add(new GeoCoordinate(30, 60));
            locations.Add(new GeoCoordinate(30, 180));
            locations.Add(new GeoCoordinate(90, -180));
            locations.Add(new GeoCoordinate(90, -60));
            locations.Add(new GeoCoordinate(90, 60));
            locations.Add(new GeoCoordinate(90, 180));

            // build graph.
            var graph = new Graph<Edge>();
            for (var idx = 0; idx < locations.Count; idx++)
            {
                graph.AddVertex((float)locations[idx].Latitude, 
                    (float)locations[idx].Longitude);
            }

            // sort vertices in-place.
            graph.SortHilbert(n);

            // sort locations.
            locations.Sort((x, y) =>
            {
                return HilbertCurve.HilbertDistance((float)x.Latitude, (float)x.Longitude, n).CompareTo(
                     HilbertCurve.HilbertDistance((float)y.Latitude, (float)y.Longitude, n));
            });

            // confirm sort.
            for(uint vertex = 1; vertex <= graph.VertexCount; vertex++)
            {
                float latitude, longitude;
                graph.GetVertex(vertex, out latitude, out longitude);
                Assert.AreEqual(latitude, locations[(int)(vertex - 1)].Latitude);
                Assert.AreEqual(longitude, locations[(int)(vertex - 1)].Longitude);
            }
        }

        /// <summary>
        /// Tests the sort hilbert function with the default order.
        /// </summary>
        [Test]
        public void SortHilbertTest2()
        {
            var n = GraphExtensions.DefaultHilbertSteps;

            // build locations.
            var locations = new List<GeoCoordinate>();
            locations.Add(new GeoCoordinate(-90, -180));
            locations.Add(new GeoCoordinate(-90, -60));
            locations.Add(new GeoCoordinate(-90, 60));
            locations.Add(new GeoCoordinate(-90, 180));
            locations.Add(new GeoCoordinate(-30, -180));
            locations.Add(new GeoCoordinate(-30, -60));
            locations.Add(new GeoCoordinate(-30, 60));
            locations.Add(new GeoCoordinate(-30, 180));
            locations.Add(new GeoCoordinate(30, -180));
            locations.Add(new GeoCoordinate(30, -60));
            locations.Add(new GeoCoordinate(30, 60));
            locations.Add(new GeoCoordinate(30, 180));
            locations.Add(new GeoCoordinate(90, -180));
            locations.Add(new GeoCoordinate(90, -60));
            locations.Add(new GeoCoordinate(90, 60));
            locations.Add(new GeoCoordinate(90, 180));

            // build graph.
            var graph = new Graph<Edge>();
            for (var idx = 0; idx < locations.Count; idx++)
            {
                graph.AddVertex((float)locations[idx].Latitude,
                    (float)locations[idx].Longitude);
            }

            // sort vertices in-place.
            graph.SortHilbert(n);

            // sort locations.
            locations.Sort((x, y) =>
            {
                return HilbertCurve.HilbertDistance((float)x.Latitude, (float)x.Longitude, n).CompareTo(
                     HilbertCurve.HilbertDistance((float)y.Latitude, (float)y.Longitude, n));
            });

            // confirm sort.
            for (uint vertex = 1; vertex <= graph.VertexCount; vertex++)
            {
                float latitude, longitude;
                graph.GetVertex(vertex, out latitude, out longitude);
                Assert.AreEqual(latitude, locations[(int)(vertex - 1)].Latitude);
                Assert.AreEqual(longitude, locations[(int)(vertex - 1)].Longitude);
            }
        }


        /// <summary>
        /// Tests the sort hilbert function with the default order.
        /// </summary>
        [Test]
        public void SortHilbertTest3()
        {
            var n = GraphExtensions.DefaultHilbertSteps;

            // build locations.
            var locations = new List<Tuple<GeoCoordinate, uint>>();
            locations.Add(new Tuple<GeoCoordinate, uint>(new GeoCoordinate(-90, -180), 1));
            locations.Add(new Tuple<GeoCoordinate, uint>(new GeoCoordinate(-90, -60), 2));
            locations.Add(new Tuple<GeoCoordinate, uint>(new GeoCoordinate(-90, 60), 3));
            locations.Add(new Tuple<GeoCoordinate, uint>(new GeoCoordinate(-90, 180), 4));
            locations.Add(new Tuple<GeoCoordinate, uint>(new GeoCoordinate(-30, -180), 5));
            locations.Add(new Tuple<GeoCoordinate, uint>(new GeoCoordinate(-30, -60), 6));
            locations.Add(new Tuple<GeoCoordinate, uint>(new GeoCoordinate(-30, 60), 7));
            locations.Add(new Tuple<GeoCoordinate, uint>(new GeoCoordinate(-30, 180), 8));
            locations.Add(new Tuple<GeoCoordinate, uint>(new GeoCoordinate(30, -180), 9));
            locations.Add(new Tuple<GeoCoordinate, uint>(new GeoCoordinate(30, -60), 10));
            locations.Add(new Tuple<GeoCoordinate, uint>(new GeoCoordinate(30, 60), 11));
            locations.Add(new Tuple<GeoCoordinate, uint>(new GeoCoordinate(30, 180), 12));
            locations.Add(new Tuple<GeoCoordinate, uint>(new GeoCoordinate(90, -180), 13));
            locations.Add(new Tuple<GeoCoordinate, uint>(new GeoCoordinate(90, -60), 14));
            locations.Add(new Tuple<GeoCoordinate, uint>(new GeoCoordinate(90, 60), 15));
            locations.Add(new Tuple<GeoCoordinate, uint>(new GeoCoordinate(90, 180), 16));

            // build graph.
            var graph = new Graph<Edge>();
            for (var idx = 0; idx < locations.Count; idx++)
            {
                graph.AddVertex((float)locations[idx].Item1.Latitude,
                    (float)locations[idx].Item1.Longitude);
            }

            // add edges.
            graph.AddEdge(1, 2, new Edge() { Tags = 1 });
            graph.AddEdge(2, 3, new Edge() { Tags = 2 });
            graph.AddEdge(3, 4, new Edge() { Tags = 3 });
            graph.AddEdge(4, 5, new Edge() { Tags = 4 });
            graph.AddEdge(5, 6, new Edge() { Tags = 5 });
            graph.AddEdge(6, 7, new Edge() { Tags = 6 });
            graph.AddEdge(7, 8, new Edge() { Tags = 7 });
            graph.AddEdge(8, 9, new Edge() { Tags = 8 });
            graph.AddEdge(9, 10, new Edge() { Tags = 9 });
            graph.AddEdge(10, 11, new Edge() { Tags = 10 });
            graph.AddEdge(11, 12, new Edge() { Tags = 11 });
            graph.AddEdge(12, 13, new Edge() { Tags = 12 });
            graph.AddEdge(13, 14, new Edge() { Tags = 13 });
            graph.AddEdge(14, 15, new Edge() { Tags = 14 });

            // sort vertices in-place.
            graph.SortHilbert(n);

            // sort locations.
            locations.Sort((x, y) =>
            {
                return HilbertCurve.HilbertDistance((float)x.Item1.Latitude, (float)x.Item1.Longitude, n).CompareTo(
                     HilbertCurve.HilbertDistance((float)y.Item1.Latitude, (float)y.Item1.Longitude, n));
            });

            // confirm sort.
            float latitude, longitude;
            var convert = new Dictionary<uint, uint>();
            for (uint vertex = 1; vertex <= graph.VertexCount; vertex++)
            {
                graph.GetVertex(vertex, out latitude, out longitude);
                Assert.AreEqual(latitude, locations[(int)(vertex - 1)].Item1.Latitude);
                Assert.AreEqual(longitude, locations[(int)(vertex - 1)].Item1.Longitude);

                convert.Add(vertex, locations[(int)(vertex - 1)].Item2);
            }

            for (uint vertex = 1; vertex <= graph.VertexCount; vertex++)
            {
                var edges = graph.GetEdges(vertex);
                var originalVertex = convert[vertex];
                foreach (var edge in edges)
                {
                    var originalNeighbour = convert[edges.Neighbour];
                    Assert.IsTrue(originalVertex - 1 == originalNeighbour ||
                        originalVertex + 1 == originalNeighbour);
                }
            }
        }

        /// <summary>
        /// Tests the hilbert search function.
        /// </summary>
        [Test]
        public void SearchHilbertTest4()
        {
            var n = GraphExtensions.DefaultHilbertSteps;

            // build graph.
            var graph = new Graph<Edge>();
            var vertex1 = graph.AddVertex(-90, -180);
            var vertex2 = graph.AddVertex(-90, -60);
            var vertex3 = graph.AddVertex(-30, -60);
            var vertex4 = graph.AddVertex(-30, -180);
            var vertex5 = graph.AddVertex(30, -180);
            var vertex6 = graph.AddVertex(90, -180);
            var vertex7 = graph.AddVertex(90, -60);
            var vertex8 = graph.AddVertex(30, -60);
            var vertex9 = graph.AddVertex(30, 60);
            var vertex10 = graph.AddVertex(90, 60);
            var vertex11 = graph.AddVertex(90, 180);
            var vertex12 = graph.AddVertex(30, 180);
            var vertex13 = graph.AddVertex(-30, 180);
            var vertex14 = graph.AddVertex(-30, 60);
            var vertex15 = graph.AddVertex(-90, 60);
            var vertex16 = graph.AddVertex(-90, 180);

            // search vertex.
            uint vertex;
            int count;

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(-90, -180, n), n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex1, vertex);
            Assert.AreEqual(1, count);

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(-90, -60, n), n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex2, vertex);
            Assert.AreEqual(1, count);

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(-30, -60, n), n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex3, vertex);
            Assert.AreEqual(1, count);

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(-30, -180, n), n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex4, vertex);
            Assert.AreEqual(1, count);

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(30, -180, n), n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex5, vertex);
            Assert.AreEqual(1, count);

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(90, -180, n), n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex6, vertex);
            Assert.AreEqual(1, count);

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(90, -60, n), n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex7, vertex);
            Assert.AreEqual(1, count);

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(30, -60, n), n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex8, vertex);
            Assert.AreEqual(1, count);

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(30, 60, n), n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex9, vertex);
            Assert.AreEqual(1, count);

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(90, 60, n), n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex10, vertex);
            Assert.AreEqual(1, count);

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(90, 180, n), n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex11, vertex);
            Assert.AreEqual(1, count);

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(30, 180, n), n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex12, vertex);
            Assert.AreEqual(1, count);

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(-30, 180, n), n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex13, vertex);
            Assert.AreEqual(1, count);

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(-30, 60, n), n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex14, vertex);
            Assert.AreEqual(1, count);

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(-90, 60, n), n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex15, vertex);
            Assert.AreEqual(1, count);

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(-90, 180, n), n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex16, vertex);
            Assert.AreEqual(1, count);
            
            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(30, 60, n) + 1, n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex9, vertex);
            Assert.AreEqual(0, count);

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(90, -180, n) + 1, n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex6, vertex);
            Assert.AreEqual(0, count);

            Assert.IsTrue(graph.SearchHilbert(HilbertCurve.HilbertDistance(-30, 60, n) + 1, n, 1, 16, out vertex, out count));
            Assert.AreEqual(vertex14, vertex);
            Assert.AreEqual(0, count);
        }

        /// <summary>
        /// Tests the hilbert search function.
        /// </summary>
        [Test]
        public void SearchHilbertTest5()
        {
            var n = GraphExtensions.DefaultHilbertSteps;

            // build graph.
            var graph = new Graph<Edge>();
            var vertex1 = graph.AddVertex(-90, -180);
            var vertex2 = graph.AddVertex(-90, -60);
            var vertex3 = graph.AddVertex(-30, -60);
            var vertex4 = graph.AddVertex(-30, -180);
            var vertex5 = graph.AddVertex(30, -180);
            var vertex6 = graph.AddVertex(90, -180);
            var vertex7 = graph.AddVertex(90, -60);
            var vertex8 = graph.AddVertex(30, -60);
            var vertex9 = graph.AddVertex(30, 60);
            var vertex10 = graph.AddVertex(90, 60);
            var vertex11 = graph.AddVertex(90, 180);
            var vertex12 = graph.AddVertex(30, 180);
            var vertex13 = graph.AddVertex(-30, 180);
            var vertex14 = graph.AddVertex(-30, 60);
            var vertex15 = graph.AddVertex(-90, 60);
            var vertex16 = graph.AddVertex(-90, 180);

            // search.
            var found = graph.SearchHilbert(30, 60, 0.001f);
            Assert.IsNotNull(found);
            Assert.AreEqual(1, found.Count);
            Assert.AreEqual(vertex9, found[0]);

            found = graph.SearchHilbert(30, -180, 0.001f);
            Assert.IsNotNull(found);
            Assert.AreEqual(1, found.Count);
            Assert.AreEqual(vertex5, found[0]);

            found = graph.SearchHilbert(30, 180, 0.001f);
            Assert.IsNotNull(found);
            Assert.AreEqual(1, found.Count);
            Assert.AreEqual(vertex12, found[0]);

            found = graph.SearchHilbert(-30, -60, 0.001f);
            Assert.IsNotNull(found);
            Assert.AreEqual(1, found.Count);
            Assert.AreEqual(vertex3, found[0]);
        }
    }
}