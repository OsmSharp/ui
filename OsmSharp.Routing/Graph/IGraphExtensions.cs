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

using System;
using System.Collections.Generic;

namespace OsmSharp.Routing.Graph
{
    /// <summary>
    /// Holds extensions for graphs.
    /// </summary>
    public static class IGraphExtensions
    {
        /// <summary>
        /// Holds the default hilbert steps.
        /// </summary>
        private static int DefaultHilbertSteps = 8;

        /// <summary>
        /// Copies all data from the given graph.
        /// </summary>
        /// <typeparam name="TEdgeData"></typeparam>
        /// <param name="copyTo"></param>
        /// <param name="copyFrom"></param>
        public static void CopyFrom<TEdgeData>(this GraphBase<TEdgeData> copyTo, GraphBase<TEdgeData> copyFrom)
            where TEdgeData : struct, IGraphEdgeData
        {
            float latitude, longitude;
            for (uint vertex = 1; vertex <= copyFrom.VertexCount; vertex++)
            {
                copyFrom.GetVertex(vertex, out latitude, out longitude);
                uint newVertex = copyTo.AddVertex(latitude, longitude);
                if (newVertex != vertex)
                {
                    throw new Exception("Graph should be empty when copy new data to it.");
                }
            }

            for (uint vertex = 1; vertex <= copyFrom.VertexCount; vertex++)
            {
                var edges = new List<Edge<TEdgeData>>(copyFrom.GetEdges(vertex));
                foreach (var edge in edges)
                {
                    copyTo.AddEdge(vertex, edge.Neighbour, edge.EdgeData, edge.Intermediates);
                }
            }
        }

        /// <summary>
        /// Sorts the vertices in the given graph based on a hilbert curve using the default step count.
        /// </summary>
        /// <typeparam name="TEdgeData"></typeparam>
        /// <param name="graph"></param>
        /// <param name="n"></param>
        public static void SortHilbert<TEdgeData>(this GraphBase<TEdgeData> graph)
            where TEdgeData : struct, IGraphEdgeData
        {
            graph.SortHilbert(IGraphExtensions.DefaultHilbertSteps);
        }

        /// <summary>
        /// Sorts the vertices in the given graph based on a hilbert curve.
        /// </summary>
        /// <typeparam name="TEdgeData"></typeparam>
        /// <param name="graph"></param>
        /// <param name="n"></param>
        public static void SortHilbert<TEdgeData>(this GraphBase<TEdgeData> graph, int n)
            where TEdgeData : struct, IGraphEdgeData
        {
            if (graph == null) { throw new ArgumentNullException("graph"); }
            if (graph.VertexCount == 1) { return; }

            uint left = 1;
            uint right = graph.VertexCount;

            IGraphExtensions.SortHilbert(graph, n, left, right);
        }

        /// <summary>
        /// Sorts the vertices in the given graph based on a hilbert curve.
        /// </summary>
        /// <typeparam name="TEdgeData"></typeparam>
        /// <param name="graph"></param>
        /// <param name="n"></param>
        private static void SortHilbert<TEdgeData>(GraphBase<TEdgeData> graph, int n, uint left, uint right)
            where TEdgeData : struct, IGraphEdgeData
        {
            if (graph == null) { throw new ArgumentNullException("graph"); }
            if (graph.VertexCount == 1) { return; }

            if (left < right)
            { // left is still to the left.
                uint pivot = IGraphExtensions.SortHilbertPartition(graph, n, left, right);
                if (left <= pivot && pivot <= right)
                {
                    IGraphExtensions.SortHilbert(graph, n, left, pivot - 1);
                    IGraphExtensions.SortHilbert(graph, n, pivot, right);
                }
            }
        }

        /// <summary>
        /// Partions the vertices based on the pivot value.
        /// </summary>
        /// <typeparam name="TEdgeData"></typeparam>
        /// <param name="graph">The graph to sort.</param>
        /// <param name="n">The hilbert accuracy.</param>
        /// <param name="left">The first vertex to consider.</param>
        /// <param name="right">The last vertex to consider.</param>
        /// <return>The new left.</return>
        private static uint SortHilbertPartition<TEdgeData>(GraphBase<TEdgeData> graph, int n, uint left, uint right)
            where TEdgeData : struct, IGraphEdgeData
        { // get the pivot value.
            uint start = left;
            ulong pivotValue = graph.HilbertDistance(n, left);
            left++;

            while (true)
            {
                ulong leftValue = graph.HilbertDistance(n, left);
                while (left <= right && leftValue <= pivotValue)
                { // move the left to the first value bigger than pivot.
                    left++;
                    leftValue = graph.HilbertDistance(n, left);
                }

                ulong rightValue = graph.HilbertDistance(n, right);
                while (left <= right && rightValue > pivotValue)
                { // move the right to the first value smaller than pivot.
                    right--;
                    rightValue = graph.HilbertDistance(n, right);
                }

                if (left > right)
                { // we are done searching, left is to the right of right.
                    // make sure the pivot value is where it is supposed to be.
                    graph.Switch(start, left - 1);
                    return left;
                }

                // swith left<->right.
                graph.Switch(left, right);
            }
        }

        /// <summary>
        /// Returns the hibert distance for n and the given vertex.
        /// </summary>
        /// <typeparam name="TEdgeData"></typeparam>
        /// <param name="graph"></param>
        /// <param name="n"></param>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public static ulong HilbertDistance<TEdgeData>(this GraphBase<TEdgeData> graph, int n, uint vertex)
            where TEdgeData : struct, IGraphEdgeData
        {
            float latitude, longitude;
            graph.GetVertex(vertex, out latitude, out longitude);
            return OsmSharp.Math.Algorithms.HilbertCurve.HilbertDistance(latitude, longitude, n);
        }

        /// <summary>
        /// Switches the data around for the two given vertices.
        /// </summary>
        /// <typeparam name="TEdgeData"></typeparam>
        /// <param name="graph"></param>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        public static void Switch<TEdgeData>(this GraphBase<TEdgeData> graph, uint vertex1, uint vertex2)
            where TEdgeData : struct, IGraphEdgeData
        {
            // get all existing data.
            var edges1 = new List<Edge<TEdgeData>>(graph.GetEdges(vertex1));
            var edges2 = new List<Edge<TEdgeData>>(graph.GetEdges(vertex2));
            float vertex1Latitude, vertex1Longitude;
            graph.GetVertex(vertex1, out vertex1Latitude, out vertex1Longitude);
            float vertex2Latitude, vertex2Longitude;
            graph.GetVertex(vertex2, out vertex2Latitude, out vertex2Longitude);

            // remove all edges.
            graph.RemoveEdges(vertex1);
            graph.RemoveEdges(vertex2);

            // update location.
            graph.SetVertex(vertex1, vertex2Latitude, vertex2Longitude);
            graph.SetVertex(vertex2, vertex1Latitude, vertex1Longitude);

            // add edges again.
            foreach (var edge in edges1)
            {
                // update existing data.
                if (edge.Neighbour == vertex1)
                {
                    edge.Neighbour = vertex2;
                }
                else if (edge.Neighbour == vertex2)
                {
                    edge.Neighbour = vertex1;
                }
                graph.AddEdge(vertex2, edge.Neighbour, edge.EdgeData, edge.Intermediates);
            }
            foreach (var edge in edges2)
            {
                // update existing data.
                if (edge.Neighbour == vertex1)
                {
                    edge.Neighbour = vertex2;
                }
                else if (edge.Neighbour == vertex2)
                {
                    edge.Neighbour = vertex1;
                }
                graph.AddEdge(vertex1, edge.Neighbour, edge.EdgeData, edge.Intermediates);
            }
        }
    }
}