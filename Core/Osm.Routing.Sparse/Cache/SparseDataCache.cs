using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Core.Cache;
using Tools.Math.Geo;
using Osm.Data.Core.Sparse;
using Osm.Data.Core.Sparse.Primitives;

namespace Osm.Routing.Sparse.Cache
{
    /// <summary>
    /// A cache for sparse data.
    /// </summary>
    public class SparseDataCache : ISparseData
    {
        /// <summary>
        /// The data to fill the cache and to persist to.
        /// </summary>
        private ISparseData _data;

        /// <summary>
        /// Creates a new sparse cache.
        /// </summary>
        /// <param name="capacity"></param>
        /// <param name="data"></param>
        public SparseDataCache(int capacity, ISparseData data)
        {
            _data = data;

            _sparse_vertex_cache = new LRUCache<long, SparseVertex>(capacity);
            _sparse_simple_vertex_cache = new LRUCache<long, SparseSimpleVertex>(capacity);
            _simple_vertex_cache = new LRUCache<long, SimpleVertex>(capacity);
            _simple_arc_cache = new LRUCache<long, SimpleArc>(capacity);
        }

        #region Sparse Vertex

        /// <summary>
        /// The cache holding all sparse vertices.
        /// </summary>
        private LRUCache<long, SparseVertex> _sparse_vertex_cache;

        /// <summary>
        /// Returns a sparse vertex if any.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SparseVertex GetSparseVertex(long id)
        {
            SparseVertex vertex = _sparse_vertex_cache.Get(id);
            if (vertex == null)
            {
                vertex = _data.GetSparseVertex(id);
                if (vertex != null)
                {
                    _sparse_vertex_cache.Add(id, vertex);
                }
            }
            return vertex;
        }

        /// <summary>
        /// Returns all vertices for the given ids.
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
        /// Persist the sparse vertex.
        /// </summary>
        /// <param name="vertex"></param>
        public void PersistSparseVertex(SparseVertex vertex)
        {
            _data.PersistSparseVertex(vertex);
        }

        /// <summary>
        /// Delete the sparse vertex.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteSparseVertex(long id)
        {
            _data.DeleteSparseVertex(id);

            _sparse_vertex_cache.Remove(id);
        }

        #endregion

        #region Sparse Simple Vertex

        /// <summary>
        /// The cache holding all sparse simple vertices.
        /// </summary>
        private LRUCache<long, SparseSimpleVertex> _sparse_simple_vertex_cache;

        /// <summary>
        /// Perists a sparse simple vertex.
        /// </summary>
        /// <param name="vertex"></param>
        public void PersistSparseSimpleVertex(SparseSimpleVertex vertex)
        {
            _data.PersistSparseSimpleVertex(vertex);
        }

        /// <summary>
        /// Returns a sparse simple vertex if any.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SparseSimpleVertex GetSparseSimpleVertex(long id)
        {
            SparseSimpleVertex vertex = _sparse_simple_vertex_cache.Get(id);
            if (vertex == null)
            {
                vertex = _data.GetSparseSimpleVertex(id);
                if (vertex != null)
                {
                    _sparse_simple_vertex_cache.Add(id, vertex);
                }
            }
            return vertex;
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
        /// Returns all simple sparse vertices in a given box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public List<SparseSimpleVertex> GetSparseSimpleVertices(GeoCoordinateBox box)
        {
            List<SparseSimpleVertex> vertices = _data.GetSparseSimpleVertices(box);

            foreach (SparseSimpleVertex vertex in vertices)
            {
                _sparse_simple_vertex_cache.Add(vertex.Id, vertex);
            }

            return vertices;
        }

        /// <summary>
        /// Deletes a sparse simple vertex.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteSparseSimpleVertex(long id)
        {
            _data.DeleteSparseSimpleVertex(id);

            _sparse_simple_vertex_cache.Remove(id);
        }

        #endregion

        #region Simple Vertex

        /// <summary>
        /// The cache holding all simple vertices.
        /// </summary>
        private LRUCache<long, SimpleVertex> _simple_vertex_cache;

        /// <summary>
        /// Perists a simple vertex.
        /// </summary>
        /// <param name="vertex"></param>
        public void PersistSimpleVertex(SimpleVertex vertex)
        {
            _data.PersistSimpleVertex(vertex);
        }

        /// <summary>
        /// Returns a simple vertex if any.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SimpleVertex GetSimpleVertex(long id)
        {
            SimpleVertex vertex = _simple_vertex_cache.Get(id);
            if (vertex == null)
            {
                vertex = _data.GetSimpleVertex(id);
                if (vertex != null)
                {
                    _simple_vertex_cache.Add(id, vertex);
                }
            }
            return vertex;
        }

        /// <summary>
        /// Returns all vertices with the given ids.
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
        /// Deletes a sparse simple vertex.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteSimpleVertex(long id)
        {
            _data.DeleteSimpleVertex(id);
        }

        #endregion

        #region Simple Arc

        /// <summary>
        /// The cache holding all simple arcs.
        /// </summary>
        private LRUCache<long, SimpleArc> _simple_arc_cache;

        /// <summary>
        /// Persists a simple arc.
        /// </summary>
        /// <param name="arc"></param>
        public void PersistSimpleArc(SimpleArc arc)
        {
            _data.PersistSimpleArc(arc);
        }

        /// <summary>
        /// Returns a simple arc.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SimpleArc GetSimpleArc(long id)
        {
            SimpleArc arc = _simple_arc_cache.Get(id);
            if (arc == null)
            {
                arc = _data.GetSimpleArc(id);
                if (arc != null)
                {
                    _simple_arc_cache.Add(id, arc);
                }
            }
            return arc;
        }

        /// <summary>
        /// Returns a list of simple arcs.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<SimpleArc> GetSimpleArcs(IList<long> ids)
        {
            List<SimpleArc> arcs = new List<SimpleArc>();
            foreach (long id in ids)
            {
                SimpleArc arc = this.GetSimpleArc(id);
                arcs.Add(arc);
            }
            return arcs;
        }

        /// <summary>
        /// Deletes a simple arc.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteSimpleArc(long id)
        {
            _data.DeleteSimpleArc(id);

            _simple_arc_cache.Remove(id);
        }

        #endregion
    }
}
