//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Osm.Routing.HH;
//using ServiceStack.Redis;
//using Osm.Data.Redis;
//using Osm.Routing.Graphs;
//using Osm.Routing.Raw.Graphs;
//using Osm.Routing.HH.Primitives;

//namespace Osm.Data.Redis.HH
//{
//    /// <summary>
//    /// Class used to accept and process highway hierarchy data.
//    /// </summary>
//    public class HighwayHierachy : IHighwayHierarchy
//    {
//        /// <summary>
//        /// Holds the redis client.
//        /// </summary>
//        private RedisClient _client;

//        /// <summary>
//        /// Holds the simple source from redis.
//        /// </summary>
//        private RedisSimpleSource _source;

//        /// <summary>
//        /// Creates a new highway hierarchy redis class.
//        /// </summary>
//        public HighwayHierachy()
//        {
//            _client = new RedisClient();
//            _source = new RedisSimpleSource(_client);
//        }

//        /// <summary>
//        /// Clears all the data in this target.
//        /// </summary>
//        public void ClearTarget()
//        {
//            // flushes all data from the target.
//            _client.FlushDb();
//        }

//        /// <summary>
//        /// The datasource from redis.
//        /// </summary>
//        public Osm.Data.IDataSourceReadOnly Data
//        {
//            get 
//            {
//                return _source;
//            }
//        }

//        /// <summary>
//        /// Returns an enumerable to return all highway edges for a given level.
//        /// </summary>
//        /// <param name="level"></param>
//        /// <returns></returns>
//        public IEnumerable<GraphVertex> GetHighwayNodes(int level)
//        {
//            throw new NotImplementedException();
//        }

//        public void AddEdge(int level, HighwayEdge edges)
//        {
//            throw new NotImplementedException();
//        }

//        public GraphVertex GetUncontracted(int level)
//        {
//            throw new NotImplementedException();
//        }

//        public void MarkUncontracted(GraphVertex vertex)
//        {
//            throw new NotImplementedException();
//        }

//        public void ContractVertex(GraphVertex vertex, HashSet<HighwayEdge> shortcuts)
//        {
//            throw new NotImplementedException();
//        }

//        public HashSet<HighwayEdge> GetNeigbours(int level, GraphVertex vertex, HashSet<GraphVertex> exceptions)
//        {
//            throw new NotImplementedException();
//        }

//        public HashSet<HighwayEdge> GetNeigboursReversed(int level, GraphVertex vertex, HashSet<GraphVertex> exceptions)
//        {
//            throw new NotImplementedException();
//        }

//        public bool ContainsVertex(int level, GraphVertex vertex)
//        {
//            throw new NotImplementedException();
//        }

//        public void AddCore(int level, GraphVertex vertex)
//        {
//            throw new NotImplementedException();
//        }

//        public void StartCore(int _level)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
