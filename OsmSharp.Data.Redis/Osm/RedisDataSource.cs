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
using OsmSharp.Osm.Data;
using OsmSharp.Math.Geo;
using OsmSharp.Osm;
using OsmSharp.Osm.Tiles;
using OsmSharp.Data.Redis.Osm.Primitives;

namespace OsmSharp.Data.Redis.Osm
{
    /// <summary>
    /// A datasource for OSM-objects for redis.
    /// </summary>
    public class RedisDataSource : DataSourceReadOnlyBase, IDisposable
    {
        /// <summary>
        /// Holds the id of this datasource.
        /// </summary>
        private Guid _id;

        private RedisClient _client;
        private IRedisTypedClient<RedisNode> _clientNode = null;
        private IRedisTypedClient<RedisWay> _clientWay = null;
        private IRedisTypedClient<RedisRelation> _clientRelation = null;

        /// <summary>
        /// Creates a new datasource.
        /// </summary>
        public RedisDataSource()
        {
            _id = Guid.NewGuid();

            _client = new RedisClient();
            _clientNode = _client.As<RedisNode>();
            _clientWay = _client.As<RedisWay>();
            _clientRelation = _client.As<RedisRelation>();
        }

        /// <summary>
        /// Creates a new datasource.
        /// </summary>
        /// <param name="client"></param>
        public RedisDataSource(RedisClient client)
        {
            _id = Guid.NewGuid();

            _client = client;
            _clientNode = _client.As<RedisNode>();
            _clientWay = _client.As<RedisWay>();
            _clientRelation = _client.As<RedisRelation>();
        }

        /// <summary>
        /// Returns the boundingbox if any.
        /// </summary>
        public override GeoCoordinateBox BoundingBox
        {
            get 
            { 
                throw new NotSupportedException(); 
            }
        }

        /// <summary>
        /// Returns the name of this datasource.
        /// </summary>
        public string Name
        {
            get 
            {
                return "Redis Data Source";
            }
        }

        /// <summary>
        /// Returns the id of this datasource.
        /// </summary>
        public override Guid Id
        {
            get 
            { 
                return _id; 
            }
        }

        /// <summary>
        /// Returns true if there is a boundingbox available.
        /// </summary>
        public override bool HasBoundinBox
        {
            get 
            { 
                return false; 
            }
        }

        private class NodeEnumerable : IEnumerable<Node>
        {
            private RedisClient _client;

            public NodeEnumerable(RedisClient client)
            {
                _client = client;
            }
            
            public IEnumerator<Node> GetEnumerator()
            {
                byte[][] keys =_client.Keys("hash:*");
                throw new NotImplementedException();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        private class NodeEnumerator : IEnumerator<Node>
        {
            public Node Current
            {
                get { throw new NotImplementedException(); }
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            object System.Collections.IEnumerator.Current
            {
                get { throw new NotImplementedException(); }
            }

            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns the node for the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Node GetNode(long id)
        {
            string nodeKey = PrimitiveExtensions.BuildNodeRedisKey(id);
            RedisNode redisNode = _clientNode.GetValue(nodeKey);
            Node node = null;
            if (redisNode != null)
            {
                node = PrimitiveExtensions.ConvertFrom(redisNode);
            }
            return node;
        }

        /// <summary>
        /// Returns all the nodes with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override IList<Node> GetNodes(IList<long> ids)
        {
            List<string> keys = new List<string>();
            foreach (int id in ids)
            {
                string nodeKey = PrimitiveExtensions.BuildNodeRedisKey(id);
                keys.Add(nodeKey);
            }
            List<RedisNode> redisNodes = _clientNode.GetValues(keys);
            List<Node> nodes = new List<Node>();
            foreach (RedisNode redisNode in redisNodes)
            {
                Node node = PrimitiveExtensions.ConvertFrom(redisNode);
                nodes.Add(node);
            }
            return nodes;
        }

        /// <summary>
        /// Returns the relation with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Relation GetRelation(long id)
        {
            string relationKey = PrimitiveExtensions.BuildRelationRedisKey(id);
            RedisRelation redisRelation = _clientRelation.GetValue(relationKey);
            Relation relation = null;
            if (redisRelation != null)
            {
                relation = PrimitiveExtensions.ConvertFrom(redisRelation);
            }
            return relation;
        }

        /// <summary>
        /// Returns all relations for the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override IList<Relation> GetRelations(IList<long> ids)
        {
            List<string> keys = new List<string>();
            foreach (int id in ids)
            {
                string relationKey = PrimitiveExtensions.BuildRelationRedisKey(id);
                keys.Add(relationKey);
            }
            List<RedisRelation> redisRelations = _clientRelation.GetValues(keys);
            List<Relation> relations = new List<Relation>();
            foreach (RedisRelation redisRelation in redisRelations)
            {
                Relation relation = PrimitiveExtensions.ConvertFrom(redisRelation);
                relations.Add(relation);
            }
            return relations;
        }

        /// <summary>
        /// Returns all relations containing the given object.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public override IList<Relation> GetRelationsFor(OsmGeoType type, long id)
        {
            string listKey = PrimitiveExtensions.BuildMemberRelationListRedisKey(type, id);

            HashSet<string> relationIdStrings = _client.GetAllItemsFromSet(listKey);
            List<long> relationIds = new List<long>();
            if (relationIdStrings != null)
            {
                foreach (string relationIdString in relationIdStrings)
                {
                    relationIds.Add(long.Parse(relationIdString, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture));
                }
            }

            return this.GetRelations(relationIds);
        }

        /// <summary>
        /// Returns the way for the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Way GetWay(long id)
        {
            string wayKey = PrimitiveExtensions.BuildWayRedisKey(id);
            RedisWay redisWay = _clientWay.GetValue(wayKey);
            Way way = null;
            if (redisWay != null)
            {
                way = PrimitiveExtensions.ConvertFrom(redisWay);
            }
            return way;
        }

        /// <summary>
        /// Returns all ways for the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override IList<Way> GetWays(IList<long> ids)
        {
            List<string> keys = new List<string>();
            foreach (int id in ids)
            {
                string wayKey = PrimitiveExtensions.BuildWayRedisKey(id);
                keys.Add(wayKey);
            }
            List<RedisWay> redisWays = _clientWay.GetValues(keys);
            List<Way> ways = new List<Way>();
            foreach (RedisWay redisWay in redisWays)
            {
                Way way = PrimitiveExtensions.ConvertFrom(redisWay);
                ways.Add(way);
            }
            return ways;
        }

        /// <summary>
        /// Returns all ways containing the given node.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override IList<Way> GetWaysFor(long id)
        {
            string listKey = PrimitiveExtensions.BuildNodeWayListRedisKey(id);

            HashSet<string> wayIdStrings = _client.GetAllItemsFromSet(listKey);
            List<long> wayIds = new List<long>();
            if (wayIdStrings != null)
            {
                foreach (string wayIdString in wayIdStrings)
                {
                    wayIds.Add(long.Parse(wayIdString, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture));
                }
            }

            return this.GetWays(wayIds);
        }

        /// <summary>
        /// Returns all ways containing one or more of the given nodes.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Way> GetWaysFor(List<long> ids)
        {
            HashSet<long> wayIds = new HashSet<long>();
            foreach (long id in ids)
            {
                string listKey = PrimitiveExtensions.BuildNodeWayListRedisKey(id);

                HashSet<string> wayIdStrings = _client.GetAllItemsFromSet(listKey);
                if (wayIdStrings != null)
                {
                    foreach (string wayIdString in wayIdStrings)
                    {
                        wayIds.Add(long.Parse(wayIdString, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture));
                    }
                }
            }

            return this.GetWays(wayIds.ToList());
        }

        /// <summary>
        /// Returns all objects in the given bounding box and that pass the given filter.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override IList<OsmGeo> Get(GeoCoordinateBox box, OsmSharp.Osm.Filters.Filter filter)
        {
            List<OsmGeo> res = new List<OsmGeo>();

            // create a range or tiles around the given bounding box.
            TileRange range = TileRange.CreateAroundBoundingBox(box, 14);

            // build all redis keys for the given boxes.
            var hashKeys = new List<string>();
            foreach (Tile tile in range)
            {
                hashKeys.Add(tile.Id.ToString());
            }

            byte[][] box_members = _client.SUnion(hashKeys.ToArray());
            HashSet<string> nodeKeys = new HashSet<string>();
            foreach (byte[] box_member in box_members)
            {
                long node_id = BitConverter.ToInt64(box_member, 0);
                string node_key = PrimitiveExtensions.BuildNodeRedisKey(node_id);
                nodeKeys.Add(node_key);
            }

            List<RedisNode> redisNodes = _clientNode.GetValues(new List<string>(nodeKeys));
            var nodeIds = new List<long>();
            foreach (RedisNode redisNode in redisNodes)
            {
                // test if the node is in the given bb. 
                GeoCoordinate coordinate = new GeoCoordinate(redisNode.Latitude.Value, redisNode.Longitude.Value);
                if (box.Contains(coordinate))
                {
                    res.Add(PrimitiveExtensions.ConvertFrom(redisNode));
                    nodeIds.Add(redisNode.Id.Value);
                }
            }

            // load all ways that contain the nodes that have been found.
            res.AddRange(this.GetWaysFor(nodeIds));

            // get relations containing any of the nodes or ways in the current results-list.
            List<Relation> relations = new List<Relation>();
            HashSet<long> relationIds = new HashSet<long>();
            foreach (OsmGeo osmGeo in res)
            {
                IList<Relation> relationsFor = this.GetRelationsFor(osmGeo);
                foreach (Relation relation in relationsFor)
                {
                    if (!relationIds.Contains(relation.Id.Value))
                    {
                        relations.Add(relation);
                        relationIds.Add(relation.Id.Value);
                    }
                }
            }

            // recursively add all relations containing other relations as a member.
            do
            {
                res.AddRange(relations); // add previous relations-list.
                List<Relation> newRelations = new List<Relation>();
                foreach (OsmGeo osmGeo in relations)
                {
                    IList<Relation> relationsFor = this.GetRelationsFor(osmGeo);
                    foreach (Relation relation in relationsFor)
                    {
                        if (!relationIds.Contains(relation.Id.Value))
                        {
                            newRelations.Add(relation);
                            relationIds.Add(relation.Id.Value);
                        }
                    }
                }
                relations = newRelations;
            } while (relations.Count > 0);

            if (filter != null)
            {
                List<OsmGeo> filtered = new List<OsmGeo>();
                foreach (OsmGeo geo in res)
                {
                    if (filter.Evaluate(geo))
                    {
                        filtered.Add(geo);
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Disposes of all resource associated with this datasource.
        /// </summary>
        public void Dispose()
        {
            _clientNode.Dispose();
            _clientWay.Dispose();
            _clientRelation.Dispose();

            _client.Dispose();
        }
    }
}
