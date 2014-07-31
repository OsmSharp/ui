using System;
using System.Collections.Generic;
using System.Linq;
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

using System.Text;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.CH.PreProcessing;
using OsmSharp.Collections.LongIndex.LongIndex;

namespace OsmSharp.Routing.CH.Serialization.Sorted
{
    /// <summary>
    /// Enumerates all vertices in a CH graph in a depth-first manner starting with the vertex at the highest level.
    /// </summary>
    public class CHDepthFirstEnumerator : IEnumerator<CHDepthFirstVertex>, IEnumerable<CHDepthFirstVertex>
    {
        /// <summary>
        /// Holds the current position.
        /// </summary>
        private Position _current = null;

        /// <summary>
        /// Holds the graph to be enumerated.
        /// </summary>
        private IDynamicGraph<CHEdgeData> _graph;

        /// <summary>
        /// Holds the index of visited vertices.
        /// </summary>
        private LongIndex _index;

        /// <summary>
        /// Holds the current count of enumerated vertices.
        /// </summary>
        private uint _currentCount = 0;

        /// <summary>
        /// Creates depth-first enumerator.
        /// </summary>
        /// <param name="graph"></param>
        public CHDepthFirstEnumerator(IDynamicGraph<CHEdgeData> graph)
        {
            _graph = graph;
            _index = new LongIndex();
        }

        /// <summary>
        /// Moves to the next item.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (_current == null)
            {
                _current = this.FindHighest();
                _index.Add(_current.Vertex);
                _currentCount = 1;
                return _current != null;
            }

            // search for the next arc.
            KeyValuePair<uint, CHEdgeData>[] edges = _graph.GetEdges(_current.Vertex);
            int arcIdx = _current.ArcIdx;
            arcIdx++;
            while (arcIdx < edges.Length)
            { // check if it is 'lower'.
                if (edges[arcIdx].Value.ToLower && 
                    !_index.Contains(edges[arcIdx].Key))
                { // yes the arc is 'lower' take it!
                    _current.ArcIdx = arcIdx; // move the arcIdx.

                    Position newPosition = new Position();
                    newPosition.Parent = _current;
                    newPosition.ArcIdx = -1;
                    newPosition.Vertex = edges[arcIdx].Key;
                    newPosition.Depth = _current.Depth + 1;
                    _current = newPosition;

                    _index.Add(_current.Vertex);
                    _currentCount++;
                    return true;
                }
                arcIdx++; // move to the next arc.
            }

            // move to parent.
            if (_current.Parent != null)
            { // set parent as current and move next.
                _current = _current.Parent;
                return this.MoveNext();
            }

            // also enumerate all the other 'islands' of vertices unconnected to the current vertices.
            return this.MoveToNextIsland();
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        public CHDepthFirstVertex Current
        {
            get
            {
                return new CHDepthFirstVertex() { VertexId = _current.Vertex, Depth = _current.Depth };
            }
        }

        /// <summary>
        /// Resets this enumerator.
        /// </summary>
        public void Reset()
        {
            _current = null;
            _index = new LongIndex();
        }

        /// <summary>
        /// Returns the highest vertex.
        /// </summary>
        /// <returns></returns>
        private Position FindHighest()
        {
            if (_graph.VertexCount > 1)
            {
                uint highest = 1;
                KeyValuePair<uint, CHEdgeData>[] arcs = _graph.GetArcsHigher(highest);
                while (arcs.Length > 0)
                {
                    highest = arcs[0].Key;
                    arcs = _graph.GetArcsHigher(highest);
                }
                return new Position() { ArcIdx = -1, Vertex = highest, Parent = null, Depth = 0 };
            }
            return null;
        }

        /// <summary>
        /// Moves to the next island.
        /// </summary>
        /// <returns></returns>
        private bool MoveToNextIsland()
        {
            if (_currentCount < _graph.VertexCount)
            {
                for (uint vertex = 1; vertex < _graph.VertexCount + 1; vertex++)
                {
                    if (!_index.Contains(vertex))
                    { // vertex was not enumerated, it is part of an 'island'.
                        KeyValuePair<uint, CHEdgeData>[] arcs = _graph.GetArcsHigher(vertex);
                        while (arcs.Length > 0)
                        {
                            vertex = arcs[0].Key;
                            arcs = _graph.GetArcsHigher(vertex);
                        }
                        _index.Add(vertex);
                        _currentCount++;
                        _current = new Position() { ArcIdx = -1, Vertex = vertex, Parent = null, Depth = 0 };
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Represents the current position.
        /// </summary>
        private class Position
        {
            /// <summary>
            /// Holds the parent-position.
            /// </summary>
            public Position Parent { get; set; }

            /// <summary>
            /// Holds the current vertex.
            /// </summary>
            public uint Vertex { get; set; }

            /// <summary>
            /// Holds the current arc idx.
            /// </summary>
            public int ArcIdx { get; set; }

            /// <summary>
            /// Holds the current depth.
            /// </summary>
            public uint Depth { get; set; }
        }

        /// <summary>
        /// Disposes of all resources associated with this enumerator.
        /// </summary>
        public void Dispose()
        {
            _graph = null;
            _current = null;
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        /// <summary>
        /// Returns a depth-first enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CHDepthFirstVertex> GetEnumerator()
        {
            return new CHDepthFirstEnumerator(_graph);
        }

        /// <summary>
        /// Returns a depth-first enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new CHDepthFirstEnumerator(_graph);
        }
    }

    /// <summary>
    /// Represents a vertex and a depth in the global CH graph.
    /// </summary>
    public class CHDepthFirstVertex
    {
        /// <summary>
        /// Holds the vertex.
        /// </summary>
        public uint VertexId { get; set; }

        /// <summary>
        /// The depth of the vertex.
        /// </summary>
        public uint Depth { get; set; }

        /// <summary>
        /// Returns a description for this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}@{1}",
                this.VertexId, this.Depth);
        }
    }
}
