// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using Tools.Math.Geo;
using Osm.Core;
using Osm.Data.Core.Sparse;
using Osm.Data.Core.Sparse.Primitives;

namespace Osm.Data.Redis.Sparse
{
    /// <summary>
    /// Redis sparse-data provider.
    /// </summary>
    public class RedisSparseData : ISparseData
    {
        /// <summary>
        /// The redis client.
        /// </summary>
        private RedisClient _client;

        /// <summary>
        /// The SparseVertex client.
        /// </summary>
        private IRedisTypedClient<SparseVertex> _client_sparse_vertex = null;

        /// <summary>
        /// The SparseSimpleVertex client.
        /// </summary>
        private IRedisTypedClient<SparseSimpleVertex> _client_sparse_simple_vertex = null;

        /// <summary>
        /// The SimpleVertex client.
        /// </summary>
        private IRedisTypedClient<SimpleVertex> _client_simple_vertex = null;

        /// <summary>
        /// The SimpleArc client.
        /// </summary>
        private IRedisTypedClient<SimpleArc> _client_simple_arc = null;

        /// <summary>
        /// Creates a new sparse-data provider.
        /// </summary>
        public RedisSparseData()
        {
            _client = new RedisClient();
            _client_sparse_vertex = _client.GetTypedClient<SparseVertex>();
            _client_sparse_simple_vertex = _client.GetTypedClient<SparseSimpleVertex>();
            _client_simple_vertex = _client.GetTypedClient<SimpleVertex>();
            _client_simple_arc = _client.GetTypedClient<SimpleArc>();
        }

        /// <summary>
        /// Creates a new sparse-data provider.
        /// </summary>
        /// <param name="client"></param>
        public RedisSparseData(RedisClient client)
        {
            _client = client;
            _client_sparse_vertex = _client.GetTypedClient<SparseVertex>();
            _client_sparse_simple_vertex = _client.GetTypedClient<SparseSimpleVertex>();
            _client_simple_vertex = _client.GetTypedClient<SimpleVertex>();
            _client_simple_arc = _client.GetTypedClient<SimpleArc>();
        }

        #region Sparse Vertex

        /// <summary>
        /// Returns the sparse vertex for the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SparseVertex GetSparseVertex(long id)
        { // returns the vertex.
            SparseVertex sparse_vertex = _client_sparse_vertex.GetValue(PrimitiveExtensions.BuildRedisKeySparseVertex(id));

            if (sparse_vertex != null && sparse_vertex.Neighbours == null)
            {
                sparse_vertex = null;
            }
            return sparse_vertex;
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
        /// Persists the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        public void PersistSparseVertex(SparseVertex vertex)
        { // sets the vertex.
            _client_sparse_vertex.SetEntry(vertex.BuildRedisKey(), vertex);
        }

        /// <summary>
        /// Deletes the vertex with the given id.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteSparseVertex(long id)
        {
            _client_sparse_vertex.DeleteById(PrimitiveExtensions.BuildRedisKeySparseVertex(id));
        }

        #endregion

        #region Sparse Simple Vertex

        /// <summary>
        /// Persists a simple vertex.
        /// </summary>
        /// <param name="vertex_id"></param>
        /// <param name="coordinate"></param>
        /// <param name="neighbour1"></param>
        /// <param name="neighbour2"></param>
        public void PersistSparseSimpleVertex(SparseSimpleVertex simple)
        {
            var idBytes = BitConverter.GetBytes(simple.Id);

            // returns a bypassed vertex.
            SparseSimpleVertex bypassed = _client_sparse_simple_vertex.GetValue(simple.BuildRedisKey());
            if (bypassed != null)
            {
                _client.SRem(OsmHash.GetOsmHashAsString(bypassed.Latitude, bypassed.Longitude), idBytes);
            }

            // create the new bypassed.
            _client_sparse_simple_vertex.SetEntry(simple.Id.ToString(), simple);
            _client.SAdd(OsmHash.GetOsmHashAsString(simple.Latitude, simple.Longitude), idBytes);
        }


        public List<SparseSimpleVertex> GetSparseSimpleVertices(GeoCoordinateBox box)
        {
            // TODO: improve this to allow loading of bigger bb's. 
            uint x_min = OsmHash.lon2x(box.MinLon);
            uint x_max = OsmHash.lon2x(box.MaxLon);
            uint y_min = OsmHash.lat2y(box.MinLat);
            uint y_max = OsmHash.lat2y(box.MaxLat);

            IList<long> boxes = new List<long>();

            List<SparseSimpleVertex> result = new List<SparseSimpleVertex>();
            HashSet<long> way_ids = new HashSet<long>();

            var hash_keys = new List<string>();
            for (uint x = x_min; x <= x_max; x++)
                for (uint y = y_min; y <= y_max; y++)
                    hash_keys.Add(OsmHash.GetOsmHashAsString(x, y));

            byte[][] box_members = _client.SUnion(hash_keys.ToArray());
            HashSet<string> vertex_keys = new HashSet<string>();
            foreach (byte[] box_member in box_members)
            {
                long node_id = BitConverter.ToInt64(box_member, 0);
                vertex_keys.Add(node_id.ToString());
            }

            List<SparseSimpleVertex> simple_nodes = _client_sparse_simple_vertex.GetValues(new List<string>(vertex_keys));
            foreach (SparseSimpleVertex simple_node in simple_nodes)
            {
                // test if the node is in the given bb. 
                GeoCoordinate coordinate = new GeoCoordinate(simple_node.Latitude, simple_node.Longitude);
                if (box.IsInside(coordinate))
                {
                    result.Add(simple_node);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns the vertex with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SparseSimpleVertex GetSparseSimpleVertex(long id)
        {
            return _client_sparse_simple_vertex.GetValue(PrimitiveExtensions.BuildRedisKeySparseSimpleVertex(id));
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
            _client_sparse_simple_vertex.DeleteById(
                PrimitiveExtensions.BuildRedisKeySparseSimpleVertex(id));
        }

        #endregion

        #region Simple Vertex

        /// <summary>
        /// Returns the simple vertex for the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SimpleVertex GetSimpleVertex(long id)
        { // returns the vertex.
            SimpleVertex simple_vertex = _client_simple_vertex.GetValue(
                PrimitiveExtensions.BuildRedisKeySimpleVertex(id));

            return simple_vertex;
        }

        /// <summary>
        /// Returns all vertices for the given ids.
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
        /// Persists the given vertex.
        /// </summary>
        /// <param name="vertex"></param>
        public void PersistSimpleVertex(SimpleVertex vertex)
        { // sets the vertex.
            _client_simple_vertex.SetEntry(vertex.BuildRedisKey(), vertex);
        }

        /// <summary>
        /// Deletes the vertex with the given id.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteSimpleVertex(long id)
        {
            _client_simple_vertex.DeleteById(
                PrimitiveExtensions.BuildRedisKeySimpleVertex(id));
        }

        #endregion

        #region Simple Arc

        /// <summary>
        /// Returns the simple arc for the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SimpleArc GetSimpleArc(long id)
        { // returns the arc.
            SimpleArc simple_arc = _client_simple_arc.GetValue(
                PrimitiveExtensions.BuildRedisKeySimpleArc(id));

            return simple_arc;
        }

        /// <summary>
        /// Returns all vertices for the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<SimpleArc> GetSimpleArcs(IList<long> ids)
        {
            List<SimpleArc> vertices = new List<SimpleArc>();
            foreach (long id in ids)
            {
                SimpleArc arc = this.GetSimpleArc(id);
                vertices.Add(arc);
            }
            return vertices;
        }

        /// <summary>
        /// Persists the given arc.
        /// </summary>
        /// <param name="arc"></param>
        public void PersistSimpleArc(SimpleArc arc)
        { // sets the arc.
            _client_simple_arc.SetEntry(arc.BuildRedisKey(), arc);
        }

        /// <summary>
        /// Deletes the arc with the given id.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteSimpleArc(long id)
        {
            _client_simple_arc.DeleteById(
                PrimitiveExtensions.BuildRedisKeySimpleArc(id));
        }

        #endregion

    }
}
