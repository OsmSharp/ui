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
using OsmSharp.Osm;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.Filters;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using OsmSharp.Osm.Factory;
using OsmSharp.Osm.Data.Redis.SimpleSchema.Processort.Primitives;

namespace OsmSharp.Osm.Data.Redis.SimpleSchema
{
    public class RedisSimpleSource : IDataSourceReadOnly, IDisposable
    {
        private Guid _id;

        private RedisClient _client;
        private IRedisTypedClient<OsmNode> _client_node = null;
        private IRedisTypedClient<OsmWay> _client_way = null;
        //private IRedisTypedClient<SimpleRelation> _client_relation = null;

        public RedisSimpleSource()
        {
            _id = Guid.NewGuid();

            _client = new RedisClient();
            _client_node = _client.GetTypedClient<OsmNode>();
            _client_way = _client.GetTypedClient<OsmWay>();
            //_client_relation = _client.GetTypedClient<SimpleRelation>();
        }

        public RedisSimpleSource(RedisClient client)
        {
            _id = Guid.NewGuid();

            _client = client;
            _client_node = _client.GetTypedClient<OsmNode>();
            _client_way = _client.GetTypedClient<OsmWay>();
            //_client_relation = _client.GetTypedClient<SimpleRelation>();
        }

        public OsmSharp.Tools.Math.Geo.GeoCoordinateBox BoundingBox
        {
            get 
            { 
                throw new NotSupportedException(); 
            }
        }

        public string Name
        {
            get 
            {
                return "Redis Simple Source";
            }
        }

        public Guid Id
        {
            get 
            { 
                return _id; 
            }
        }

        public bool HasBoundinBox
        {
            get 
            { 
                return false; 
            }
        }

        public bool IsReadOnly
        {
            get 
            { 
                return true;
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


        public Node GetNode(long id)
        {
            string node_key = OsmNode.BuildRedisKey(id);
            OsmNode simple_node = _client_node.GetValue(node_key);
            Node node = null;
            if (simple_node != null)
            {
                node = this.ConvertTo(simple_node);
            }
            return node;
        }

        public IList<Node> GetNodes(IList<long> ids)
        {
            List<string> keys = new List<string>();
            foreach (int id in ids)
            {
                string node_key = OsmNode.BuildRedisKey(id);
                keys.Add(node_key);
            }
            List<OsmNode> simple_nodes = _client_node.GetValues(keys);
            List<Node> nodes = new List<Node>();
            foreach (OsmNode simple_node in simple_nodes)
            {
                Node node = this.ConvertTo(simple_node);

                nodes.Add(node);
            }
            return nodes;
        }

        private Node ConvertTo(OsmNode simple_node)
        {
            Node node = OsmBaseFactory.CreateNode(simple_node.Id);
            node.Coordinate = new GeoCoordinate(simple_node.Latitude, simple_node.Longitude);
            node.ChangeSetId = -1;//simple_node.ChangeSetId;
            foreach (OsmTag tag in simple_node.Tags)
            {
                node.Tags.Add(tag.Key, tag.Value);
            }
            node.TimeStamp = null;
            node.User = string.Empty;
            node.UserId = -1;
            node.Version = -1; //simple_node.Version;
            node.Visible = true;

            return node;
        } 

        public Relation GetRelation(long id)
        {
            return null;
        }

        public IList<Relation> GetRelations(IList<long> ids)
        {
            List<Relation> relations = new List<Relation>();
            return relations;
        }

        public IList<Relation> GetRelationsFor(OsmBase obj)
        {
            return new List<Relation>();
        }

        public Way GetWay(long id)
        {
            string way_key = OsmWay.BuildRedisKey(id);
            OsmWay simple_way = _client_way.GetValue(way_key);
            Way way = null;
            if (simple_way != null)
            {
                IList<Node> nodes = this.GetNodes(simple_way.Nds);
                Dictionary<long, Node> nodes_dic = new Dictionary<long, Node>();
                foreach (Node node in nodes)
                {
                    nodes_dic[node.Id] = node;
                }
                way = this.ConvertTo(simple_way, nodes_dic);
            }
            return way;
        }

        public IList<Way> GetWays(IList<long> ids)
        {
            List<string> keys = new List<string>();
            foreach (int id in ids)
            {
                string way_key = OsmWay.BuildRedisKey(id);
                keys.Add(way_key);
            }
            List<OsmWay> simple_ways = _client_way.GetValues(keys);
            List<Way> ways = new List<Way>();
            HashSet<long> node_ids = new HashSet<long>();
            foreach (OsmWay simple_way in simple_ways)
            {
                foreach (long node_id in simple_way.Nds)
                {
                    node_ids.Add(node_id);
                }
            }
            IList<Node> nodes = this.GetNodes(new List<long>(node_ids));
            Dictionary<long, Node> nodes_dic = new Dictionary<long, Node>();
            foreach (Node node in nodes)
            {
                nodes_dic[node.Id] = node;
            }
            foreach (OsmWay simple_way in simple_ways)
            {
                Way way = this.ConvertTo(simple_way, nodes_dic);
                ways.Add(way);
            }
            return ways;
        }

        private Way ConvertTo(OsmWay simple_way, Dictionary<long, Node> nodes_dic)
        {
            Way way = OsmBaseFactory.CreateWay(simple_way.Id);
            way.ChangeSetId = -1; // simple_way.ChangeSetId;
            foreach (OsmTag tag in simple_way.Tags)
            {
                way.Tags.Add(tag.Key, tag.Value);
            }
            foreach (long node_id in simple_way.Nds)
            {
                Node node = null;
                if (nodes_dic.TryGetValue(node_id, out node))
                {
                    way.Nodes.Add(node);
                }
            }
            way.TimeStamp = null; // simple_way.TimeStamp;
            way.User = string.Empty; // simple_way.UserName;
            way.UserId = -1;// simple_way.UserId;
            way.Version = -1; // (simple_way.Version != null && simple_way.Version.HasValue) ? (long)simple_way.Version.Value : 0;
            way.Visible = true;// (simple_way.Visible == true) ? true : false;

            return way;
        }

        public IList<Way> GetWaysFor(Node node)
        {
            if (node != null)
            {
                string node_key = OsmNode.BuildRedisKey(node.Id);
                OsmNode new_node = _client_node.GetValue(node_key);

                return this.GetWays(new_node.Ways);
            }
            return new List<Way>();
        }

        public IList<OsmGeo> Get(GeoCoordinateBox box, Filter filter)
        {
            // TODO: improve this to allow loading of bigger bb's. 
            uint x_min = lon2x(box.MinLon);
            uint x_max = lon2x(box.MaxLon);
            uint y_min = lat2y(box.MinLat);
            uint y_max = lat2y(box.MaxLat);

            IList<long> boxes = new List<long>();

            List<OsmGeo> result = new List<OsmGeo>();
            HashSet<long> way_ids = new HashSet<long>();

            var hash_keys = new List<string>();
            for (uint x = x_min; x <= x_max; x++)
                for (uint y = y_min; y <= y_max; y++)
                    hash_keys.Add(OsmNode.BuildOsmHashRedisKey(x, y));

            byte[][] box_members = _client.SUnion(hash_keys.ToArray());
            HashSet<string> node_keys = new HashSet<string>();
            foreach (byte[] box_member in box_members)
            {
                long node_id = BitConverter.ToInt64(box_member, 0);
                string node_key = OsmNode.BuildRedisKey(node_id);
                node_keys.Add(node_key);
            }

            List<OsmNode> simple_nodes = _client_node.GetValues(new List<string>(node_keys));
            foreach (OsmNode simple_node in simple_nodes)
            {
                // test if the node is in the given bb. 
                GeoCoordinate coordinate = new GeoCoordinate(simple_node.Latitude, simple_node.Longitude);
                if (box.IsInside(coordinate))
                {
                    result.Add(this.ConvertTo(simple_node));

                    foreach (long way_id in simple_node.Ways)
                    {
                        way_ids.Add(way_id);
                    }
                }
            }

            // get all ways. 
            result.AddRange(this.GetWays(new List<long>(way_ids)));

            return result;
        }
            
        #region Tile Calculations

        uint xy2tile(uint x, uint y)
        {
            uint tile = 0;
            int i;

            for (i = 15; i >= 0; i--)
            {
                tile = (tile << 1) | ((x >> i) & 1);
                tile = (tile << 1) | ((y >> i) & 1);
            }

            return tile;
        }

        uint lon2x(double lon)
        {
            return (uint)Math.Floor(((lon + 180.0) * 65536.0 / 360.0));
        }

        uint lat2y(double lat)
        {
            return (uint)Math.Floor(((lat + 90.0) * 65536.0 / 180.0));
        }

        #endregion

        public void Dispose()
        {
            _client_node.Dispose();
            _client_way.Dispose();

            _client.Dispose();
        }
    }
}
