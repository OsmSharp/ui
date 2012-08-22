using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.Sparse;
using Osm.Data.Core.CH.Primitives;
using Osm.Data.Core.Sparse.Primitives;
using Osm.Core;
using Tools.Math.Geo;

namespace Osm.Data.Core.CH.Memory
{
    /// <summary>
    /// Holds ch data in memory.
    /// </summary>
    public class MemoryCHData : ICHData
    {
        /// <summary>
        /// Creates a new in-memory sparse data set.
        /// </summary>
        public MemoryCHData()
        {
            // sparse vertices.
            _ch_vertices = new Dictionary<long, CHVertex>();
        }

        #region CH Vertex

        /// <summary>
        /// Holds all sparse vertices.
        /// </summary>
        private Dictionary<long, CHVertex> _ch_vertices;

        /// <summary>
        /// Returns the vertex with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CHVertex GetCHVertex(long id)
        {
            CHVertex vertex = null;
            _ch_vertices.TryGetValue(id, out vertex);
            return vertex;
        }

        /// <summary>
        /// Returns all vertices inside the given box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public IEnumerable<CHVertex> GetCHVertices(GeoCoordinateBox box)
        {
            HashSet<CHVertex> vertices_in_box = new HashSet<CHVertex>();
            foreach (CHVertex vertex in _ch_vertices.Values)
            {
                if (box.IsInside(vertex.Location))
                {
                    vertices_in_box.Add(vertex);
                }
            }
            return vertices_in_box;
        }

        /// <summary>
        /// Persists a given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        public void PersistCHVertex(CHVertex vertex)
        {
            _ch_vertices[vertex.Id] = vertex;
        }

        /// <summary>
        /// Deletes the vertex with the given id.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteCHVertex(long id)
        {
            _ch_vertices.Remove(id);
        }

        #endregion
    }
}
