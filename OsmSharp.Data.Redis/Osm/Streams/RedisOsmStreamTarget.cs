// OsmSharp - OpenStreetMap tools & library.
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
using OsmSharp.Osm.Data.Streams;
using OsmSharp.Data.Redis.Osm.Streams.Primitives;
using OsmSharp.Osm.Simple;

namespace OsmSharp.Data.Redis.Osm.Streams
{
    /// <summary>
    /// A data processor target using Redis.
    /// </summary>
    public class RedisDataProcessorTarget : OsmStreamTarget
    {
        /// <summary>
        /// Holds the redis client.
        /// </summary>
        private RedisClient _redisClient;

        /// <summary>
        /// Creates a new data processor target.
        /// </summary>
        public RedisDataProcessorTarget()
        {
            _redisClient = new RedisClient();
        }

        /// <summary>
        /// Creates a new data processor target.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public RedisDataProcessorTarget(string host, int port)
        {
            _redisClient = new RedisClient(host, port);
        }

        /// <summary>
        /// The redist node clients.
        /// </summary>
        private IRedisTypedClient<OsmNode> _node_type_client;

        /// <summary>
        /// The redist node clients.
        /// </summary>
        private IRedisTypedClient<OsmWay> _way_type_client;

        ///// <summary>
        ///// The redist node clients.
        ///// </summary>
        //private IRedisTypedClient<SimpleRelation> _relation_type_client;

        /// <summary>
        /// Initializes this data processor.
        /// </summary>
        public override void Initialize()
        {
            _node_type_client = _redisClient.As<OsmNode>();
            _way_type_client = _redisClient.As<OsmWay>();
            //_relation_type_client = _redis_client.GetTypedClient<SimpleRelation>();
            
             //redis = redisClient.GetTypedClient<OsmNode>()
            _cached_nodes = new Dictionary<long, Node>();
            _nodes_in_ways = new Dictionary<long, OsmNode>();
        }

        /// <summary>
        /// Holds the nodes in memory.
        /// </summary>
        private Dictionary<long, Node> _cached_nodes;

        /// <summary>
        /// Holds the nodes in the ways.
        /// </summary>
        private Dictionary<long, OsmNode> _nodes_in_ways;

        /// <summary>
        /// Adds a node to this database.
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(Node node)
        {
            _cached_nodes[node.Id.Value] = node;
        }

        /// <summary>
        /// Adds a way to this database.
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(Way way)
        {
            // converts the way.
            OsmWay newWay = PrimitiveExtensions.ConvertTo(way, way.Nodes);

            // only insert it if it is an highway.
            if (newWay.IsHighway)
            {
                string way_key = newWay.GetRedisKey();
                _way_type_client.SetEntry(way_key, newWay);

                // add all the needed nodes.
                foreach (long id in way.Nodes)
                {
                    Node node = null;
                    OsmNode new_node = null;
                    if (_cached_nodes.TryGetValue(id, out node)
                        && !_nodes_in_ways.TryGetValue(id, out new_node))
                    {
                        // convert to the new node.
                        new_node = PrimitiveExtensions.ConvertTo(node);
                        _nodes_in_ways.Add(id, new_node);
                    }
                    if (new_node != null)
                    {
                        new_node.Ways.Add(way.Id.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a relation to this database.
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(Relation relation)
        {
            //string relation_key = relation.GetRedisKey();
            //_relation_type_client.SetEntry(relation_key, relation);
        }

        /// <summary>
        /// Closes the redis target.
        /// </summary>
        public override void Close()
        {
            long node_idx = 0;
            foreach (OsmNode new_node in _nodes_in_ways.Values)
            {
                node_idx++;
                string node_key = new_node.GetRedisKey();
                _node_type_client.SetEntry(node_key, new_node);

                var idBytes = BitConverter.GetBytes(new_node.Id);
                _redisClient.SAdd(new_node.GetOsmHash(), idBytes);

                if ((node_idx % 1000) == 0)
                {
                    Console.WriteLine("Node[{0}]", node_idx);
                }
            }
            _node_type_client.Dispose();
            _way_type_client.Dispose();

            _redisClient.Dispose();
        }
    }
}
