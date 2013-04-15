// OsmSharp - OpenStreetMap tools & library.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.Geo.Simple;

namespace OsmSharp.Routing.Graph.DynamicGraph.Memory
{
    /// <summary>
    /// An implementation of an in-memory dynamic graph.
    /// </summary>
    public class MemoryDynamicGraph<EdgeData> : IDynamicGraph<EdgeData>
        where EdgeData : IDynamicGraphEdgeData
    {
        /// <summary>
        /// Holds the next id.
        /// </summary>
        private uint _next_id;

        /// <summary>
        /// Holds all graph data.
        /// </summary>
        private KeyValuePair<uint, EdgeData>[][] _vertices;

        /// <summary>
        /// Holds the coordinates of the vertices.
        /// </summary>
        private GeoCoordinateSimple[] _coordinates;
        
        /// <summary>
        /// Creates a new in-memory graph.
        /// </summary>
        public MemoryDynamicGraph()
        {
            _next_id = 1;
            _vertices = new KeyValuePair<uint, EdgeData>[1000][];
            _coordinates = new GeoCoordinateSimple[1000];
        }

        /// <summary>
        /// Increases the memory allocation for this dynamic graph.
        /// </summary>
        private void IncreaseSize()
        {
            Array.Resize<GeoCoordinateSimple>(ref _coordinates, _coordinates.Length + 1000);
            Array.Resize<KeyValuePair<uint, EdgeData>[]>(ref _vertices, _vertices.Length + 1000);
        }

        /// <summary>
        /// Adds a new vertex to this graph.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="neighbours_estimate"></param>
        /// <returns></returns>
        public uint AddVertex(float latitude, float longitude, byte neighbours_estimate)
        {
            return this.AddVertex(latitude, longitude);
        }

        /// <summary>
        /// Adds a new vertex.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public uint AddVertex(float latitude, float longitude)
        {
            // make sure vertices array is large enough.
            if (_next_id >= _vertices.Length)
            {
                this.IncreaseSize();
            }

            // create vertex.
            uint new_id = _next_id;
            _vertices[new_id] = null;
            _coordinates[new_id] = new GeoCoordinateSimple()
            {
                Latitude = latitude,
                Longitude = longitude
            };
            _next_id++; // increase for next vertex.
            return new_id; 
        }

        /// <summary>
        /// Returns the information in the current vertex.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public bool GetVertex(uint id, out float latitude, out float longitude)
        {
            if (_vertices.Length > id)
            {
                latitude = _coordinates[id].Latitude;
                longitude = _coordinates[id].Longitude;
                return true;
            }
            latitude = float.MaxValue;
            longitude = float.MaxValue;
            return false;
        }

        /// <summary>
        /// Returns an enumerable of all vertices.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<uint> GetVertices()
        {
            if (_next_id > 1)
            {
                return OsmSharp.Tools.Range.UInt32(1, (uint)_next_id - 1, 1U);
            }
            return new List<uint>();
        }

        /// <summary>
        /// Adds and arc to an existing vertex.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="data"></param>
        /// <param name="comparer"></param>
        public void AddArc(uint from, uint to, EdgeData data, IDynamicGraphEdgeComparer<EdgeData> comparer)
        {
            if (_vertices.Length > from)
            {
                KeyValuePair<uint, EdgeData>[] arcs =
                    _vertices[from];
                int idx = -1;
                if (arcs != null)
                { // check for an existing edge first.
                    for (int arc_idx = 0; arc_idx < arcs.Length; arc_idx++)
                    {
                        if (arcs[arc_idx].Key == to &&
                            arcs[arc_idx].Value.Weight > data.Weight &&
                            (comparer != null && comparer.Overlaps(arcs[arc_idx].Value, data)))
                        { // an arc was found that represents the same directional information.
                            arcs[arc_idx] = new KeyValuePair<uint, EdgeData>(
                                to, data);
                            return;
                        }
                    }
                    
                    // if here: there did not exist an edge yet!
                    idx = arcs.Length;
                    Array.Resize<KeyValuePair<uint, EdgeData>>(ref arcs, arcs.Length + 1);
                    _vertices[from] = arcs;
                }
                else
                { // create an arcs array.
                    arcs = new KeyValuePair<uint, EdgeData>[1];
                    idx = 0;
                    _vertices[from] = arcs;
                }

                // set the arc.
                arcs[idx] = new KeyValuePair<uint, EdgeData>(
                    to, data);

                return;
            }
            throw new ArgumentOutOfRangeException("from");
        }

        /// <summary>
        /// Removes all arcs starting at from ending at to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void DeleteArc(uint from, uint to)
        {
            if (_vertices.Length > from)
            {
                KeyValuePair<uint, EdgeData>[] arcs =
                    _vertices[from];
                if (arcs != null && arcs.Length > 0)
                {
                    List<KeyValuePair<uint, EdgeData>> arcs_list =
                        new List<KeyValuePair<uint, EdgeData>>(arcs);
                    foreach (KeyValuePair<uint, EdgeData> arc in arcs)
                    {
                        if (arc.Key == to)
                        {
                            arcs_list.Remove(arc);
                        }
                    }
                    _vertices[from] = arcs_list.ToArray();
                }
                return;
            }
            throw new ArgumentOutOfRangeException("from");
        }

        /// <summary>
        /// Returns all arcs starting at the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public KeyValuePair<uint, EdgeData>[] GetArcs(uint vertex)
        {
            if (_vertices.Length > vertex)
            {
                if (_vertices[vertex] == null)
                {
                    return new KeyValuePair<uint, EdgeData>[0];
                }
                return _vertices[vertex];
            }
            return new KeyValuePair<uint, EdgeData>[0]; // return empty data if the vertex does not exist!
        }

        /// <summary>
        /// Returns true if the given vertex has neighbour as a neighbour.
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="neighbour"></param>
        /// <returns></returns>
        public bool HasNeighbour(uint vertex, uint neighbour)
        {
            if (_vertices.Length > vertex)
            {
                if (_vertices[vertex] == null)
                {
                    return false;
                }
                foreach(KeyValuePair<uint, EdgeData> arc in  _vertices[vertex])
                {
                    if (arc.Key == neighbour)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        ///// <summary>
        ///// Represents a simple vertex.
        ///// </summary>
        //private struct Vertex
        //{
        //    /// <summary>
        //    /// Holds an array of edges starting at this vertex.
        //    /// </summary>
        //    public KeyValuePair<uint, EdgeData>[] Arcs { get; set; }
        //}

        /// <summary>
        /// Returns the number of vertices in this graph.
        /// </summary>
        public uint VertexCount
        {
            get { return _next_id - 1; }
        }
    }
}
