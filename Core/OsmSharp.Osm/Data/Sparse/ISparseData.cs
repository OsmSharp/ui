//// OsmSharp - OpenStreetMap tools & library.
//// Copyright (C) 2012 Abelshausen Ben
//// 
//// This file is part of OsmSharp.
//// 
//// OsmSharp is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 2 of the License, or
//// (at your option) any later version.
//// 
//// OsmSharp is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License
//// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OsmSharp.Tools.Math.Geo;
//using OsmSharp.Osm.Data.Core.Sparse.Primitives;

//namespace OsmSharp.Osm.Data.Core.Sparse
//{
//    /// <summary>
//    /// A sparse data interface.
//    /// </summary>
//    public interface ISparseData
//    {
//        #region Sparse Vertex

//        /// <summary>
//        /// Returns the vertex for the given id.
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        SparseVertex GetSparseVertex(long id);

//        /// <summary>
//        /// Returns all the sparse vertices with the given ids and null when there are none.
//        /// </summary>
//        /// <param name="ids"></param>
//        /// <returns></returns>
//        List<SparseVertex> GetSparseVertices(IList<long> ids);

//        /// <summary>
//        /// Adds/updates the vertex.
//        /// </summary>
//        /// <param name="vertex"></param>
//        void PersistSparseVertex(SparseVertex vertex);

//        /// <summary>
//        /// Deletes the vertex with the given id.
//        /// </summary>
//        /// <param name="id"></param>
//        void DeleteSparseVertex(long id);

//        #endregion

//        #region Sparse Simple Vertex

//        /// <summary>
//        /// Persists a vertex as a bypassed vertex.
//        /// </summary>
//        /// <param name="vertex_id"></param>
//        /// <param name="coordinate"></param>
//        void PersistSparseSimpleVertex(SparseSimpleVertex vertex);

//        /// <summary>
//        /// Returns the simple vertex.
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        SparseSimpleVertex GetSparseSimpleVertex(long id);


//        /// <summary>
//        /// Returns all the simple vertices in the given box.
//        /// </summary>
//        /// <param name="box"></param>
//        /// <returns></returns>
//        List<SparseSimpleVertex> GetSparseSimpleVertices(GeoCoordinateBox box);

//        /// <summary>
//        /// Returns all the simple vertices with the given ids.
//        /// </summary>
//        /// <param name="ids"></param>
//        /// <returns></returns>
//        List<SparseSimpleVertex> GetSparseSimpleVertices(IList<long> ids);

//        /// <summary>
//        /// Deletes the simple vertex with the given id.
//        /// </summary>
//        /// <param name="id"></param>
//        void DeleteSparseSimpleVertex(long id);

//        #endregion

//        #region Simple

//        /// <summary>
//        /// Persists a vertex as a bypassed vertex.
//        /// </summary>
//        /// <param name="vertex_id"></param>
//        /// <param name="coordinate"></param>
//        void PersistSimpleVertex(SimpleVertex vertex);

//        /// <summary>
//        /// Returns the simple vertex.
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        SimpleVertex GetSimpleVertex(long id);

//        /// <summary>
//        /// Returns all the simple vertices with the given ids.
//        /// </summary>
//        /// <param name="ids"></param>
//        /// <returns></returns>
//        List<SimpleVertex> GetSimpleVertices(IList<long> ids);

//        /// <summary>
//        /// Deletes the simple vertex with the given id.
//        /// </summary>
//        /// <param name="id"></param>
//        void DeleteSimpleVertex(long id);

//        /// <summary>
//        /// Persists a arc as a bypassed arc.
//        /// </summary>
//        /// <param name="arc_id"></param>
//        /// <param name="coordinate"></param>
//        void PersistSimpleArc(SimpleArc arc);

//        /// <summary>
//        /// Returns the simple arc.
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        SimpleArc GetSimpleArc(long id);

//        /// <summary>
//        /// Returns all the simple arcs with the given ids.
//        /// </summary>
//        /// <param name="ids"></param>
//        /// <returns></returns>
//        List<SimpleArc> GetSimpleArcs(IList<long> ids);

//        /// <summary>
//        /// Deletes the simple arc with the given id.
//        /// </summary>
//        /// <param name="id"></param>
//        void DeleteSimpleArc(long id);

//        #endregion
//    }
//}
