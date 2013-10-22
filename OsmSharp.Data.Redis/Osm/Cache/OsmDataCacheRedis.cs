// OsmSharp - OpenStreetMap (OSM) SDK
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
using OsmSharp.Osm.Cache;
using ServiceStack.Redis;
using OsmSharp.Osm;
using OsmSharp.Data.Redis.Osm.Streams;
using ServiceStack.Redis.Generic;
using OsmSharp.Data.Redis.Osm.Primitives;

namespace OsmSharp.Data.Redis.Osm.Cache
{
    /// <summary>
    /// An implementation of the OsmDataCache with a Redis database as backend.
    /// </summary>
    public class OsmDataCacheRedis : OsmDataCache, IDisposable
    {
        private RedisClient _client;
        private IRedisTypedClient<RedisNode> _clientNode = null;
        private IRedisTypedClient<RedisWay> _clientWay = null;
        private IRedisTypedClient<RedisRelation> _clientRelation = null;

        /// <summary>
        /// Creates a new osm data cache for simple OSM objects kept in memory.
        /// </summary>
        public OsmDataCacheRedis()
        {
            _client = new RedisClient();
            _clientNode = _client.As<RedisNode>();
            _clientWay = _client.As<RedisWay>();
            _clientRelation = _client.As<RedisRelation>();
        }

        /// <summary>
        /// Creates a new osm data cache for simple OSM objects kept in memory.
        /// </summary>
        /// <param name="redisClient"></param>
        public OsmDataCacheRedis(RedisClient redisClient)
        {
            _client = redisClient;
            _clientNode = _client.As<RedisNode>();
            _clientWay = _client.As<RedisWay>();
            _clientRelation = _client.As<RedisRelation>(); ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(Node node)
        {
            if (node == null) throw new ArgumentNullException("node");
            if (node.Id == null) throw new Exception("node.Id is null");

            this.Store(node);
        }

        /// <summary>
        /// Removes the node with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool RemoveNode(long id)
        {
            if (this.Exist(id, OsmGeoType.Node))
            {
                this.Delete(id, OsmGeoType.Node);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public override bool TryGetNode(long id, out Node node)
        {
            if (this.Exist(id, OsmGeoType.Node))
            {
                node = this.Read(id, OsmGeoType.Node) as Node;
                return true;
            }
            node = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(Way way)
        {
            if (way == null) throw new ArgumentNullException("way");
            if (way.Id == null) throw new Exception("way.Id is null");

            this.Store(way);
        }

        /// <summary>
        /// Removes the way with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool RemoveWay(long id)
        {
            if (this.Exist(id, OsmGeoType.Way))
            {
                this.Delete(id, OsmGeoType.Way);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="way"></param>
        /// <returns></returns>
        public override bool TryGetWay(long id, out Way way)
        {
            if (this.Exist(id, OsmGeoType.Way))
            {
                way = this.Read(id, OsmGeoType.Way) as Way;
                return true;
            }
            way = null;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(Relation relation)
        {
            if (relation == null) throw new ArgumentNullException("relation");
            if (relation.Id == null) throw new Exception("relation.Id is null");

            this.Store(relation);
        }

        /// <summary>
        /// Removes the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool RemoveRelation(long id)
        {
            if (this.Exist(id, OsmGeoType.Relation))
            {
                this.Delete(id, OsmGeoType.Relation);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        public override bool TryGetRelation(long id, out Relation relation)
        {
            if(this.Exist(id, OsmGeoType.Relation))
            { // exists.
                relation = this.Read(id, OsmGeoType.Relation) as Relation;
                return true;
            }
            relation = null;
            return false;
        }

        /// <summary>
        /// Stores an osmGeo object to disk.
        /// </summary>
        /// <param name="osmGeo"></param>
        private void Store(OsmGeo osmGeo)
        {
            switch (osmGeo.Type)
            {
                case OsmGeoType.Node:
                    Node node = osmGeo as Node;
                    string nodeKey = node.GetRedisKey();
                    _clientNode.SetEntry(nodeKey, PrimitiveExtensions.ConvertTo(node));
                    break;
                case OsmGeoType.Way:
                    Way way = osmGeo as Way;
                    string wayKey = way.GetRedisKey();
                    _clientWay.SetEntry(wayKey, PrimitiveExtensions.ConvertTo(way));
                    break;
                case OsmGeoType.Relation:
                    Relation relation = osmGeo as Relation;
                    string relationKey = relation.GetRedisKey();
                    _clientRelation.SetEntry(relationKey, PrimitiveExtensions.ConvertTo(relation));
                    break;
            }
        }

        /// <summary>
        /// Reads an osmGeo object from disk.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        private OsmGeo Read(long id, OsmGeoType type)
        {
            switch (type)
            {
                case OsmGeoType.Node:
                    string nodeKey = PrimitiveExtensions.BuildNodeRedisKey(id);
                    RedisNode redisNode = _clientNode.GetValue(nodeKey);
                    Node node = null;
                    if (redisNode != null)
                    {
                        node = PrimitiveExtensions.ConvertFrom(redisNode);
                    }
                    return node;
                case OsmGeoType.Way:
                    string wayKey = PrimitiveExtensions.BuildWayRedisKey(id);
                    RedisWay redisWay = _clientWay.GetValue(wayKey);
                    Way way = null;
                    if (redisWay != null)
                    {
                        way = PrimitiveExtensions.ConvertFrom(redisWay);
                    }
                    return way;
                case OsmGeoType.Relation:
                    string relationKey = PrimitiveExtensions.BuildRelationRedisKey(id);
                    RedisRelation redisRelation = _clientRelation.GetValue(relationKey);
                    Relation relation = null;
                    if (redisRelation != null)
                    {
                        relation = PrimitiveExtensions.ConvertFrom(redisRelation);
                    }
                    return relation;
            }
            throw new ArgumentOutOfRangeException("type");
        }

        /// <summary>
        /// Deletes an osmGeo object from disk.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        private void Delete(long id, OsmGeoType type)
        {
            switch (type)
            {
                case OsmGeoType.Node:
                    string nodeKey = PrimitiveExtensions.BuildNodeRedisKey(id);
                    _client.Del(nodeKey);
                    //_clientNode.DeleteById(nodeKey);
                    break;
                case OsmGeoType.Way:
                    string wayKey = PrimitiveExtensions.BuildWayRedisKey(id);
                    //_clientWay.DeleteById(wayKey);
                    _client.Del(wayKey);
                    break;
                case OsmGeoType.Relation:
                    string relationKey = PrimitiveExtensions.BuildRelationRedisKey(id);
                    //_clientRelation.DeleteById(relationKey);
                    _client.Del(relationKey);
                    break;
            }
        }

        /// <summary>
        /// Returns true if the given object exists.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool Exist(long id, OsmGeoType type)
        {
            switch (type)
            {
                case OsmGeoType.Node:
                    string nodeKey = PrimitiveExtensions.BuildNodeRedisKey(id);
                    return _clientNode.ContainsKey(nodeKey);
                case OsmGeoType.Way:
                    string wayKey = PrimitiveExtensions.BuildWayRedisKey(id);
                    return _clientWay.ContainsKey(wayKey);
                case OsmGeoType.Relation:
                    string relationKey = PrimitiveExtensions.BuildRelationRedisKey(id);
                    return _clientRelation.ContainsKey(relationKey);
            }
            throw new ArgumentOutOfRangeException("type");
        }

        /// <summary>
        /// Makes sure the cache directory is deleted after using it.
        /// </summary>
        public void Dispose()
        {
            _clientNode.Dispose();
            _clientWay.Dispose();
            _clientRelation.Dispose();

            _client.Dispose();
        }

        /// <summary>
        /// Clears all data from this cache.
        /// </summary>
        public override void Clear()
        {
            _client.FlushDb();
        }
    }
}
