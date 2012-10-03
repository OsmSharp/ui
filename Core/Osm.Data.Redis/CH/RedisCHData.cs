//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Osm.Data.Core.CH;
//using ServiceStack.Redis;
//using ServiceStack.Redis.Generic;
//using Osm.Data.Core.CH.Primitives;
//using Osm.Core;
//using Tools.Math.Geo;

//namespace Osm.Data.Redis.CH
//{
//    /// <summary>
//    /// A CH data source based on redis.
//    /// </summary>
//    public class RedisCHData : ICHData
//    {
//        /// <summary>
//        /// The redis client.
//        /// </summary>
//        private RedisClient _client;

//        /// <summary>
//        /// The CHVertex client.
//        /// </summary>
//        private IRedisTypedClient<CHVertex> _client_ch_vertex = null;

//            /// <summary>
//        /// Persists a CH vertex.
//        /// </summary>
//        /// <param name="simple"></param>
//        public void PersistCHVertex(CHVertex simple)
//        {
//            var idBytes = BitConverter.GetBytes(simple.Id);

//            // returns a bypassed vertex.
//            CHVertex bypassed = _client_ch_vertex.GetValue(simple.BuildRedisKey());
//            if (bypassed != null)
//            {
//                _client.SRem(OsmHash.GetOsmHashAsString(bypassed.Latitude, bypassed.Longitude), idBytes);
//            }

//            // create the new bypassed.
//            _client_ch_vertex.SetEntry(simple.Id.ToString(), simple);
//            _client.SAdd(OsmHash.GetOsmHashAsString(simple.Latitude, simple.Longitude), idBytes);
//        }

//        /// <summary>
//        /// Returns all CH vertices inside the given box.
//        /// </summary>
//        /// <param name="box"></param>
//        /// <returns></returns>
//        public IEnumerable<CHVertex> GetCHVertices(GeoCoordinateBox box)
//        {
//            // TODO: improve this to allow loading of bigger bb's. 
//            uint x_min = OsmHash.lon2x(box.MinLon);
//            uint x_max = OsmHash.lon2x(box.MaxLon);
//            uint y_min = OsmHash.lat2y(box.MinLat);
//            uint y_max = OsmHash.lat2y(box.MaxLat);

//            IList<long> boxes = new List<long>();

//            List<CHVertex> result = new List<CHVertex>();
//            HashSet<long> way_ids = new HashSet<long>();

//            var hash_keys = new List<string>();
//            for (uint x = x_min; x <= x_max; x++)
//                for (uint y = y_min; y <= y_max; y++)
//                    hash_keys.Add(OsmHash.GetOsmHashAsString(x, y));

//            byte[][] box_members = _client.SUnion(hash_keys.ToArray());
//            HashSet<string> vertex_keys = new HashSet<string>();
//            foreach (byte[] box_member in box_members)
//            {
//                long node_id = BitConverter.ToInt64(box_member, 0);
//                vertex_keys.Add(node_id.ToString());
//            }

//            List<CHVertex> simple_nodes = _client_ch_vertex.GetValues(new List<string>(vertex_keys));
//            foreach (CHVertex simple_node in simple_nodes)
//            {
//                // test if the node is in the given bb. 
//                GeoCoordinate coordinate = new GeoCoordinate(simple_node.Latitude, simple_node.Longitude);
//                if (box.IsInside(coordinate))
//                {
//                    result.Add(simple_node);
//                }
//            }
//            return result;
//        }

//        /// <summary>
//        /// Returns the vertex with the given id.
//        /// </summary>
//        /// <param name="id"></param>
//        /// <returns></returns>
//        public CHVertex GetCHVertex(long id)
//        {
//            return _client_ch_vertex.GetValue(
//                CHPrimitiveExtensions.BuildRedisKeySparseSimpleVertex(id));
//        }

//        /// <summary>
//        /// Returns all vertices with the given ids.
//        /// </summary>
//        /// <param name="ids"></param>
//        /// <returns></returns>
//        public List<CHVertex> GetCHVertices(IList<long> ids)
//        {
//            List<CHVertex> vertices = new List<CHVertex>();
//            foreach (long id in ids)
//            {
//                vertices.Add(this.GetCHVertex(id));
//            }
//            return vertices;
//        }

//                public IEnumerable<CHVertex> GetCHVerticesNoLevel()
//                {
//                    throw new NotImplementedException();
//                }

//        /// <summary>
//        /// Deletes a CH vertex.
//        /// </summary>
//        /// <param name="id"></param>
//        public void DeleteCHVertex(long id)
//        {
//            _client_ch_vertex.DeleteById(
//                CHPrimitiveExtensions.BuildRedisKeySparseSimpleVertex(id));
//        }

//        public void DeleteNeighbours(long vertexid)
//        {
//            throw new NotImplementedException();
//        }

//        public void DeleteNeighbour(CHVertex vertex, CHVertexNeighbour neighbour, bool forward)
//        {
//            throw new NotImplementedException();
//        }

//        public void PersistCHVertexNeighbour(CHVertex vertex, CHVertexNeighbour arc, bool forward)
//        {
//            throw new NotImplementedException();
//        }

//        public IEnumerable<CHVertex> GetCHVertices()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
