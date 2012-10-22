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
using Osm.Core;
using Tools.Math.Geo;
using Osm.Data.Core.Sparse;
using Osm.Data.Core.Sparse.Primitives;

namespace Osm.Routing.Sparse.Memory
{
    /// <summary>
    /// Holds sparse data in memory.
    /// </summary>
    public class MemorySparseData : ISparseData
    {
        /// <summary>
        /// Creates a new in-memory sparse data set.
        /// </summary>
        public MemorySparseData()
        {
            // sparse vertices.
            _sparse_vertices = new Dictionary<long, SparseVertex>();

            // sparse simple vertices.
            _sparse_simple_vertices_per_box = new Dictionary<string, HashSet<SparseSimpleVertex>>();
            _sparse_simple_vertices = new Dictionary<long, SparseSimpleVertex>();

            // simple vertices.
            _simple_vertices = new Dictionary<long, SimpleVertex>();
            _simple_arcs = new Dictionary<long, SimpleArc>();
        }

        #region Sparse Vertex

        /// <summary>
        /// Holds all sparse vertices.
        /// </summary>
        private Dictionary<long, SparseVertex> _sparse_vertices;

        /// <summary>
        /// Returns a sparse vertex.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SparseVertex GetSparseVertex(long id)
        {
            SparseVertex vertex = null;
            _sparse_vertices.TryGetValue(id, out vertex);
            return vertex;
        }

        /// <summary>
        /// Returns all sparse vertices for the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<SparseVertex> GetSparseVertices(IList<long> ids)
        {
            List<SparseVertex> vertices = new List<SparseVertex>();
            foreach (long id in ids)
            {
                SparseVertex vertex = this.GetSparseVertex(id);
                vertices.Add(vertex);
            }
            return vertices;
        }

        /// <summary>
        /// Perists a sparse vertex.
        /// </summary>
        /// <param name="vertex"></param>
        public void PersistSparseVertex(SparseVertex vertex)
        {
            _sparse_vertices[vertex.Id] = vertex;
        }
        
        /// <summary>
        /// Deletes a sparse vertex.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteSparseVertex(long id)
        {
            _sparse_vertices.Remove(id);
        }

        #endregion

        #region Sparse Simple Vertex

        /// <summary>
        /// Holds all sparse simple vertices per box.
        /// </summary>
        private Dictionary<string, HashSet<SparseSimpleVertex>> _sparse_simple_vertices_per_box;

        /// <summary>
        /// Holds all sparse simple vertices.
        /// </summary>
        private Dictionary<long, SparseSimpleVertex> _sparse_simple_vertices;

        /// <summary>
        /// Persists a sparse simple vertex.
        /// </summary>
        /// <param name="simple"></param>
        public void PersistSparseSimpleVertex(SparseSimpleVertex simple)
        {
            // first remove the old.
            SparseSimpleVertex old = this.GetSparseSimpleVertex(simple.Id);
            if (old != null)
            {
                string old_box = OsmHash.GetOsmHashAsString(simple.Latitude, simple.Longitude);
                _sparse_simple_vertices_per_box[old_box].Remove(old);
            }

            // add to new box.
            string new_box = OsmHash.GetOsmHashAsString(simple.Latitude, simple.Longitude);
            HashSet<SparseSimpleVertex> simple_vertices = null;
            if (!_sparse_simple_vertices_per_box.TryGetValue(new_box, out simple_vertices))
            {
                simple_vertices = new HashSet<SparseSimpleVertex>();
                _sparse_simple_vertices_per_box.Add(new_box, simple_vertices);
            }
            simple_vertices.Add(simple);

            _sparse_simple_vertices[simple.Id] = simple;
        }

        /// <summary>
        /// Returns all sparse simple vertices inside a given bounding box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public List<SparseSimpleVertex> GetSparseSimpleVertices(Tools.Math.Geo.GeoCoordinateBox box)
        {            
            // TODO: improve this to allow loading of bigger bb's. 
            uint x_min = OsmHash.lon2x(box.MinLon);
            uint x_max = OsmHash.lon2x(box.MaxLon);
            uint y_min = OsmHash.lat2y(box.MinLat);
            uint y_max = OsmHash.lat2y(box.MaxLat);

            IList<long> boxes = new List<long>();

            List<OsmBase> result = new List<OsmBase>();
            HashSet<long> way_ids = new HashSet<long>();

            HashSet<SparseSimpleVertex> vertexes_in_box;
            List<SparseSimpleVertex> vertices = new List<SparseSimpleVertex>();

            for (uint x = x_min; x <= x_max; x++)
            {
                for (uint y = y_min; y <= y_max; y++)
                {
                    if(_sparse_simple_vertices_per_box.TryGetValue(OsmHash.GetOsmHashAsString(x, y), out vertexes_in_box))
                    {
                        if (x == x_min || x == x_max || y == y_min || y == y_max)
                        {
                            foreach (SparseSimpleVertex vertex in vertexes_in_box)
                            {
                                GeoCoordinate coordinate = new GeoCoordinate(vertex.Latitude,
                                    vertex.Longitude);
                                if (box.IsInside(coordinate))
                                {
                                    vertices.Add(vertex);
                                }
                            }
                        }
                        else
                        {
                            vertices.AddRange(vertexes_in_box);
                        }
                    }
                }
            }
            return vertices;
        }

        /// <summary>
        /// Returns the sparse simple vertex.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SparseSimpleVertex GetSparseSimpleVertex(long id)
        {
            SparseSimpleVertex simple = null;
            _sparse_simple_vertices.TryGetValue(id, out simple);
            return simple;
        }

        /// <summary>
        /// Returns all vertices with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<SparseSimpleVertex> GetSparseSimpleVertices(IList<long> ids)
        {
            List<SparseSimpleVertex> vertices = new List<SparseSimpleVertex>();
            foreach (long id in ids)
            {
                SparseSimpleVertex vertex = this.GetSparseSimpleVertex(id);
                vertices.Add(vertex);
            }
            return vertices;
        }

        /// <summary>
        /// Deletes a sparse simple vertex.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteSparseSimpleVertex(long id)
        {
            SparseSimpleVertex vertex = this.GetSparseSimpleVertex(id);
            if (vertex != null)
            {
                _sparse_simple_vertices.Remove(id);

                string old_box = OsmHash.GetOsmHashAsString(vertex.Latitude, vertex.Longitude);
                _sparse_simple_vertices_per_box[old_box].Remove(vertex);
            }
        }

        #endregion

        #region Simple

        /// <summary>
        /// Keeps all simple vertices.
        /// </summary>
        private Dictionary<long, SimpleVertex> _simple_vertices;

        /// <summary>
        /// Keeps all simple arcs.
        /// </summary>
        private Dictionary<long, SimpleArc> _simple_arcs;

        /// <summary>
        /// Persist a simple vertex.
        /// </summary>
        /// <param name="vertex"></param>
        public void PersistSimpleVertex(SimpleVertex vertex)
        {
            _simple_vertices[vertex.Id] = vertex;
        }

        /// <summary>
        /// Returns a simple vertex.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SimpleVertex GetSimpleVertex(long id)
        {
            SimpleVertex simple = null;
            _simple_vertices.TryGetValue(id, out simple);
            return simple;
        }

        /// <summary>
        /// Returns a list of vertices.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<SimpleVertex> GetSimpleVertices(IList<long> ids)
        {
            List<SimpleVertex> vertices = new List<SimpleVertex>();
            foreach (long id in ids)
            {
                SimpleVertex vertex = this.GetSimpleVertex(id);
                vertices.Add(vertex);
            }
            return vertices;
        }

        /// <summary>
        /// Deletes a simple vertex.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteSimpleVertex(long id)
        {
            _simple_vertices.Remove(id);
        }

        /// <summary>
        /// Persist a simple vertex.
        /// </summary>
        /// <param name="vertex"></param>
        public void PersistSimpleArc(SimpleArc arc)
        {
            _simple_arcs[arc.Id] = arc;
        }

        /// <summary>
        /// Returns a simple vertex.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SimpleArc GetSimpleArc(long id)
        {
            SimpleArc simple = null;
            _simple_arcs.TryGetValue(id, out simple);
            return simple;
        }

        /// <summary>
        /// Returns a list of vertices.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<SimpleArc> GetSimpleArcs(IList<long> ids)
        {
            List<SimpleArc> vertices = new List<SimpleArc>();
            foreach (long id in ids)
            {
                SimpleArc vertex = this.GetSimpleArc(id);
                vertices.Add(vertex);
            }
            return vertices;
        }

        /// <summary>
        /// Deletes a simple vertex.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteSimpleArc(long id)
        {
            _simple_arcs.Remove(id);
        }

        #endregion
    }
}
