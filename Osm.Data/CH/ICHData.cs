using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.CH.Primitives;
using Osm.Data.Core.Sparse;
using Tools.Math.Geo;

namespace Osm.Data.Core.CH
{
    /// <summary>
    /// Interface for a Contraction Hierarchy (CH) data source.
    /// </summary>
    public interface ICHData
    {
        /// <summary>
        /// Returns the vertex with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        CHVertex GetCHVertex(long id);

        /// <summary>
        /// Returns the vertices inside the given box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        IEnumerable<CHVertex> GetCHVertices(GeoCoordinateBox box);

        /// <summary>
        /// Persists the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        void PersistCHVertex(CHVertex vertex);

        /// <summary>
        /// Deletes the given vertex.
        /// </summary>
        /// <param name="id"></param>
        void DeleteCHVertex(long id);
    }
}
