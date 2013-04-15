// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using OsmSharp.Osm.Simple;
using OsmSharp.Osm.Data.Redis.SimpleSchema.Processort.Primitives;
using OsmSharp.Osm.Data.Core.Processor;

namespace OsmSharp.Osm.Data.Redis.SimpleSchema.Processor
{
    /// <summary>
    /// A data processor target using Redis.
    /// </summary>
    public class RedisDataProcessorTarget : DataProcessorTarget
    {
        /// <summary>
        /// Holds the redis client.
        /// </summary>
        private RedisClient _redis_client;

        /// <summary>
        /// Creates a new data processor target.
        /// </summary>
        public RedisDataProcessorTarget()
        {
            _redis_client = new RedisClient();
        }

        /// <summary>
        /// Creates a new data processor target.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public RedisDataProcessorTarget(string host, int port)
        {
            _redis_client = new RedisClient(host, port);
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
            _node_type_client = _redis_client.GetTypedClient<OsmNode>();
            _way_type_client = _redis_client.GetTypedClient<OsmWay>();
            //_relation_type_client = _redis_client.GetTypedClient<SimpleRelation>();
            
             //redis = redisClient.GetTypedClient<OsmNode>()
            _cached_nodes = new Dictionary<long, SimpleNode>();
            _nodes_in_ways = new Dictionary<long, OsmNode>();
        }

        /// <summary>
        /// Apply a changeset to the database.
        /// </summary>
        /// <param name="change"></param>
        public override void ApplyChange(SimpleChangeSet change)
        {
            throw new NotSupportedException("Relation are not supported in the redis target!");
        }

        /// <summary>
        /// Holds the nodes in memory.
        /// </summary>
        private Dictionary<long, SimpleNode> _cached_nodes;

        /// <summary>
        /// Holds the nodes in the ways.
        /// </summary>
        private Dictionary<long, OsmNode> _nodes_in_ways;

        /// <summary>
        /// Adds a node to this database.
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(SimpleNode node)
        {
            _cached_nodes[node.Id.Value] = node;
        }

        /// <summary>
        /// Adds a way to this database.
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(SimpleWay way)
        {
            // converts the way.
            OsmWay new_way = PrimitiveExtensions.ConvertTo(way, way.Nodes);

            // only insert it if it is an highway.
            if (new_way.IsHighway)
            {
                string way_key = new_way.GetRedisKey();
                _way_type_client.SetEntry(way_key, new_way);

                // add all the needed nodes.
                foreach (long id in way.Nodes)
                {
                    SimpleNode node = null;
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
        public override void AddRelation(SimpleRelation relation)
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
                _redis_client.SAdd(new_node.GetOsmHash(), idBytes);

                if ((node_idx % 1000) == 0)
                {
                    Console.WriteLine("Node[{0}]", node_idx);
                }
            }
            _node_type_client.Dispose();
            _way_type_client.Dispose();

            _redis_client.Dispose();
        }
    }
}
