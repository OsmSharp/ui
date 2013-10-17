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
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm;
using OsmSharp.Data.Redis.Osm.Primitives;

namespace OsmSharp.Data.Redis.Osm.Streams
{
    /// <summary>
    /// A data processor target using Redis.
    /// </summary>
    public class RedisOsmStreamTarget : OsmStreamTarget
    {
        /// <summary>
        /// Holds the redis client.
        /// </summary>
        private RedisClient _redisClient;

        /// <summary>
        /// Creates a new data processor target.
        /// </summary>
        public RedisOsmStreamTarget()
        {
            _redisClient = new RedisClient();
        }

        /// <summary>
        /// Creates a new data processor target.
        /// </summary>
        /// <param name="host"></param>
        public RedisOsmStreamTarget(string host)
        {
            _redisClient = new RedisClient(host);
        }

        /// <summary>
        /// Creates a new data processor target.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public RedisOsmStreamTarget(string host, int port)
        {
            _redisClient = new RedisClient(host, port);
        }

        /// <summary>
        /// Creates a new data processor target.
        /// </summary>
        /// <param name="redisClient"></param>
        public RedisOsmStreamTarget(RedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary>
        /// The redist node clients.
        /// </summary>
        private IRedisTypedClient<RedisNode> _nodeTypeClient;

        /// <summary>
        /// The redist node clients.
        /// </summary>
        private IRedisTypedClient<RedisWay> _wayTypeClient;

        /// <summary>
        /// The redist node clients.
        /// </summary>
        private IRedisTypedClient<RedisRelation> _relationTypeClient;

        /// <summary>
        /// Initializes this data processor.
        /// </summary>
        public override void Initialize()
        {
            _nodeTypeClient = _redisClient.As<RedisNode>();
            _wayTypeClient = _redisClient.As<RedisWay>();
            _relationTypeClient = _redisClient.As<RedisRelation>();
        }

        /// <summary>
        /// Adds a node to this database.
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(Node node)
        {
            // save the node in the current redis key.
            string nodeKey = node.GetRedisKey();
            _nodeTypeClient.SetEntry(nodeKey, PrimitiveExtensions.ConvertTo(node));

            // save the node in the correct osmhash location.
            var idBytes = BitConverter.GetBytes(node.Id.Value);
            _redisClient.SAdd(node.GetOsmHash(), idBytes);
        }

        /// <summary>
        /// Adds a way to this database.
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(Way way)
        {
            // save the way in the current redis key.
            string wayKey = way.GetRedisKey();
            _wayTypeClient.SetEntry(wayKey, PrimitiveExtensions.ConvertTo(way));

            // save the way-node relation.
            if (way.Nodes != null)
            {
                foreach (long nodeId in way.Nodes)
                {
                    _redisClient.AddItemToSet(PrimitiveExtensions.BuildNodeWayListRedisKey(nodeId),
                        way.Id.Value.ToString());
                }
            }
        }

        /// <summary>
        /// Adds a relation to this database.
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(Relation relation)
        {
            // save the relation in the current redis key.
            string relationKey = relation.GetRedisKey();
            _relationTypeClient.SetEntry(relationKey, PrimitiveExtensions.ConvertTo(relation));

            // save the relation-member relation.
            if (relation.Members != null)
            {
                foreach (var member in relation.Members)
                {
                    _redisClient.AddItemToSet(PrimitiveExtensions.BuildMemberRelationListRedisKey(member),
                        relation.Id.Value.ToString());
                }
            }
        }

        /// <summary>
        /// Closes the redis target.
        /// </summary>
        public override void Close()
        {
            _nodeTypeClient.Dispose();
            _wayTypeClient.Dispose();
            _relationTypeClient.Dispose();

            _redisClient.Dispose();
        }
    }
}