// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using OsmSharp.Routing;
using OsmSharp.Routing.Graph.DynamicGraph;

namespace OsmSharp.Routing.Router
{
    /// <summary>
    /// An implementation of an in-memory dynamic graph.
    /// </summary>
    internal class RouterResolvedGraph
    {
        /// <summary>
        /// Holds all graph data.
        /// </summary>
        private Dictionary<long, RouterResolvedGraphVertex> _vertices;

        /// <summary>
        /// Creates a new in-memory graph.
        /// </summary>
        public RouterResolvedGraph()
        {
            _vertices = new Dictionary<long, RouterResolvedGraphVertex>();
        }

        /// <summary>
        /// Adds a new vertex.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public void AddVertex(long id, float latitude, float longitude)
        {
            RouterResolvedGraphVertex vertex = new RouterResolvedGraphVertex();
            vertex.Id = id;
            vertex.Latitude = latitude;
            vertex.Longitude = longitude;

            // create vertex.
            _vertices.Add(id, vertex);
        }

        /// <summary>
        /// Returns the information in the current vertex.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public bool GetVertex(long id, out float latitude, out float longitude)
        {
            RouterResolvedGraphVertex vertex;
            if (_vertices.TryGetValue(id, out vertex))
            {
                latitude = vertex.Latitude;
                longitude = vertex.Longitude;
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
        public IEnumerable<long> GetVertices()
        {
            return _vertices.Keys;
        }

        /// <summary>
        /// Adds and arc to an existing vertex.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="data"></param>
        public void AddArc(long from, long to, RouterResolvedGraphEdge data)
        {
            RouterResolvedGraphVertex vertex;
            if (_vertices.TryGetValue(from, out vertex))
            {
                KeyValuePair<long, RouterResolvedGraphEdge>[] arcs =
                    vertex.Arcs;
                int idx = -1;
                if (arcs == null)
                {
                    arcs = new KeyValuePair<long, RouterResolvedGraphEdge>[1];
                    idx = 0;
                    vertex.Arcs = arcs;
                }
                else
                {
                    idx = arcs.Length;
                    Array.Resize<KeyValuePair<long, RouterResolvedGraphEdge>>(ref arcs, arcs.Length + 1);
                    vertex.Arcs = arcs;
                }
                arcs[idx] = new KeyValuePair<long, RouterResolvedGraphEdge>(
                    to, data);
                _vertices[from] = vertex;
                return;
            }
            throw new ArgumentOutOfRangeException("from");
        }

        /// <summary>
        /// Removes all arcs starting at from ending at to.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void DeleteArc(long from, long to)
        {
            RouterResolvedGraphVertex vertex;
            if (_vertices.TryGetValue(from, out vertex))
            {
                KeyValuePair<long, RouterResolvedGraphEdge>[] arcs =
                    _vertices[from].Arcs;
                if (arcs != null && arcs.Length > 0)
                {
                    List<KeyValuePair<long, RouterResolvedGraphEdge>> arcs_list =
                        new List<KeyValuePair<long, RouterResolvedGraphEdge>>(arcs);
                    foreach (KeyValuePair<long, RouterResolvedGraphEdge> arc in arcs)
                    {
                        if (arc.Key == to)
                        {
                            arcs_list.Remove(arc);
                        }
                    }
                    vertex.Arcs = arcs_list.ToArray();
                }
                _vertices[from] = vertex;
                return;
            }
            throw new ArgumentOutOfRangeException("from");
        }

        /// <summary>
        /// Returns all arcs starting at the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        /// <returns></returns>
        public KeyValuePair<long, RouterResolvedGraphEdge>[] GetArcs(long vertex)
        {
            if (_vertices.ContainsKey(vertex))
            {
                if (_vertices[vertex].Arcs == null)
                {
                    return new KeyValuePair<long, RouterResolvedGraphEdge>[0];
                }
                return _vertices[vertex].Arcs;
            }
            return new KeyValuePair<long, RouterResolvedGraphEdge>[0]; // return empty data if the vertex does not exist!
        }

        /// <summary>
        /// Represents a simple vertex.
        /// </summary>
        private struct RouterResolvedGraphVertex
        {
            /// <summary>
            /// The id of this vertex.
            /// </summary>
            public long Id { get; set; }

            /// <summary>
            /// Holds the latitude.
            /// </summary>
            public float Latitude { get; set; }

            /// <summary>
            /// Holds longitude.
            /// </summary>
            public float Longitude { get; set; }

            /// <summary>
            /// Holds an array of edges starting at this vertex.
            /// </summary>
            public KeyValuePair<long, RouterResolvedGraphEdge>[] Arcs { get; set; }
        }

        /// <summary>
        /// Represents a resolved edge.
        /// </summary>
        internal class RouterResolvedGraphEdge : IDynamicGraphEdgeData
        {
            ///// <summary>
            ///// Returns true if the edge can be followed only in the foward direction.
            ///// </summary>
            //public bool Forward
            //{
            //    get;
            //    set;
            //}

            ///// <summary>
            ///// Returns true if the edge can be followed only in the backward direction.
            ///// </summary>
            //public bool Backward
            //{
            //    get;
            //    set;
            //}

            /// <summary>
            /// Returns the weight of this edge.
            /// </summary>
            public double Weight
            {
                get;
                set;
            }

            /// <summary>
            /// Returns the tags identifier.
            /// </summary>
            public uint Tags
            {
                get;
                set;
            }

            /// <summary>
            /// These edge can always be resolved on.
            /// </summary>
            public bool IsVirtual
            {
                get
                {
                    return false;
                }
            }
        }
    }
}
