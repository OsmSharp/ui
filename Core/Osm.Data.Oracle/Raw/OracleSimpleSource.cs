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
using System.Data.OracleClient;
using Osm.Core;
using Osm.Core.Factory;
using Tools.Math.Geo;
using Osm.Core.Filters;

namespace Osm.Data.Oracle.Raw
{
    public class OracleSimpleSource : IDataSourceReadOnly, IDisposable
    {
        private string _connection_string;

        private Guid _id;

        public OracleSimpleSource(string connection_string)
        {
            _connection_string = connection_string;
            _id = Guid.NewGuid();
        }

        private OracleConnection _connection;

        private OracleConnection CreateConnection()
        {
            if (_connection == null)
            {
                _connection = new OracleConnection(_connection_string);
                _connection.Open();
            }
            return _connection;
        }

        #region IDataSourceReadOnly Members

        public GeoCoordinateBox BoundingBox
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
                return "Oracle API Data Source";
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
                return false;
            }
        }

        public Node GetNode(long id)
        {
            IList<Node> nodes = this.GetNodes(new List<long>(new long[] { id }));
            if (nodes.Count > 0)
            {
                return nodes[0];
            }
            return null;
        }

        public IList<Node> GetNodes(IList<long> ids)
        {
            IList<Node> return_list = new List<Node>();
            if (ids.Count > 0)
            {
                // initialize connection.
                OracleConnection con = this.CreateConnection();
                // STEP 1: query nodes table.
                //id	latitude	longitude	changeset_id	visible	timestamp	tile	version

                Dictionary<long, Node> nodes = new Dictionary<long, Node>();
                for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
                {
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = Math.Min((idx_1000 + 1) * 1000, ids.Count);
                    string sql
                        = "SELECT * FROM node WHERE (id IN ({0})) ";
                    ;
                    string ids_string = this.ConstructIdList(ids,start_idx,stop_idx);
                    if (ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);

                        OracleCommand com = new OracleCommand(sql);
                        com.Connection = con;
                        OracleDataReader reader = com.ExecuteReader();
                        Node node = null;
                        List<long> node_ids = new List<long>();
                        while (reader.Read())
                        {
                            // load/parse data.
                            long returned_id = reader.GetInt64(0);
                            int latitude_int = reader.GetInt32(1);
                            int longitude_int = reader.GetInt32(2);
                            long changeset_id = reader.GetInt64(3);
                            bool visible = reader.GetInt32(4) == 1;
                            DateTime timestamp = reader.GetDateTime(5);
                            long tile = reader.GetInt64(6);
                            long version = reader.GetInt64(7);

                            if (!nodes.ContainsKey(returned_id))
                            {
                                // create node.
                                node = OsmBaseFactory.CreateNode(returned_id);
                                node.Version = version;
                                //node.UserId = user_id;
                                node.TimeStamp = timestamp;
                                node.ChangeSetId = changeset_id;
                                node.Coordinate = new GeoCoordinate(
                                    ((double)latitude_int) / 10000000.0, ((double)longitude_int) / 10000000.0);

                                nodes.Add(node.Id, node);
                                node_ids.Add(node.Id);
                            }
                        }
                        reader.Close();
                    }
                }

                // STEP2: Load all node tags.
                this.LoadNodeTags(nodes);

                return_list = nodes.Values.ToList<Node>();
            }
            return return_list;
        }

        public Relation GetRelation(long id)
        {
            // TODO: implement this
            return null;
        }

        public IList<Relation> GetRelations(IList<long> ids)
        {
            // TODO: implement this
            return new List<Relation>();
        }

        public IList<Relation> GetRelationsFor(Osm.Core.OsmBase obj)
        {
            // TODO: implement this
            return new List<Relation>();
        }

        public Way GetWay(long id)
        {
            IList<Way> ways = this.GetWays(new List<long>(new long[] { id }));
            if (ways.Count > 0)
            {
                return ways[0];
            }
            return null;
        }

        public IList<Way> GetWays(IList<long> ids)
        {
            return this.GetWays(ids, null);
        }

        private IList<Way> GetWays(IList<long> ids,Dictionary<long,Node> nodes)
        {
            if (ids.Count > 0)
            {
                OracleConnection con = this.CreateConnection();

                // STEP2: Load ways.
                Dictionary<long, Way> ways = new Dictionary<long, Way>();
                string sql;
                OracleCommand com;
                OracleDataReader reader;
                for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
                {
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = Math.Min((idx_1000 + 1) * 1000, ids.Count);

                    sql = "SELECT * FROM way WHERE (id IN ({0})) ";
                    string ids_string = this.ConstructIdList(ids,start_idx,stop_idx);
                    if(ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);
                        com = new OracleCommand(sql);
                        com.Connection = con;
                        reader = com.ExecuteReader();
                        Way way;
                        while (reader.Read())
                        {
                            long id = reader.GetInt64(0);
                            long changeset_id = reader.GetInt64(1);
                            DateTime timestamp = reader.GetDateTime(2);
                            bool visible = reader.GetInt64(3) == 1;
                            long version = reader.GetInt64(4);

                            // create way.
                            way = OsmBaseFactory.CreateWay(id);
                            way.Version = version;
                            //node.UserId = user_id;
                            way.TimeStamp = timestamp;
                            way.ChangeSetId = changeset_id;

                            ways.Add(way.Id, way);
                        }
                        reader.Close();
                    }
                }

                //STEP3: Load all node-way relations
                List<long> missing_node_ids = new List<long>();
                for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
                {
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = Math.Min((idx_1000 + 1) * 1000, ids.Count);

                    sql = "SELECT * FROM way_nodes WHERE (way_id IN ({0})) ORDER BY sequence_id";
                    string ids_string = this.ConstructIdList(ids, start_idx, stop_idx);
                    if (ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);
                        com = new OracleCommand(sql);
                        com.Connection = con;
                        reader = com.ExecuteReader();
                        while (reader.Read())
                        {
                            long id = reader.GetInt64(0);
                            long node_id = reader.GetInt64(1);
                            long sequence_id = reader.GetInt64(2);

                            if (nodes == null || !nodes.ContainsKey(node_id))
                            {
                                missing_node_ids.Add(node_id);
                            }
                        }
                        reader.Close();
                    }
                }

                //STEP4: Load all missing nodes.
                IList<Node> missing_nodes = this.GetNodes(missing_node_ids);
                Dictionary<long, Node> way_nodes = new Dictionary<long, Node>();
                if (nodes != null)
                {
                    way_nodes = new Dictionary<long, Node>(nodes);
                }
                foreach (Node node in missing_nodes)
                {
                    way_nodes.Add(node.Id, node);
                }

                //STEP5: assign nodes to way.
                for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
                {
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = Math.Min((idx_1000 + 1) * 1000, ids.Count);

                    sql = "SELECT * FROM way_nodes WHERE (way_id IN ({0})) ORDER BY sequence_id";
                    string ids_string = this.ConstructIdList(ids, start_idx, stop_idx);
                    if (ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);
                        com = new OracleCommand(sql);
                        com.Connection = con;
                        reader = com.ExecuteReader();
                        while (reader.Read())
                        {
                            long id = reader.GetInt64(0);
                            long node_id = reader.GetInt64(1);
                            long sequence_id = reader.GetInt64(2);

                            Node way_node;
                            if (way_nodes.TryGetValue(node_id, out way_node))
                            {
                                Way way;
                                if (ways.TryGetValue(id, out way))
                                {
                                    way.Nodes.Add(way_node);
                                }
                            }
                        }
                        reader.Close();
                    }
                }


                //STEP4: Load all tags.
                for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
                {
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = Math.Min((idx_1000 + 1) * 1000, ids.Count);

                    sql = "SELECT * FROM way_tags WHERE (way_id IN ({0})) ";
                    string ids_string = this.ConstructIdList(ids, start_idx, stop_idx);
                    if (ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);
                        com = new OracleCommand(sql);
                        com.Connection = con;
                        reader = com.ExecuteReader();
                        while (reader.Read())
                        {
                            long id = reader.GetInt64(0);
                            string key = reader.GetString(1);
                            object value_object = reader[2];
                            string value = string.Empty;
                            if (value_object != null && value_object != DBNull.Value)
                            {
                                value = (string)value_object;
                            }

                            Way way;
                            if (ways.TryGetValue(id, out way))
                            {
                                way.Tags.Add(key, value);
                            }
                        }
                        reader.Close();
                    }
                }

                return ways.Values.ToList<Way>();
            }
            return new List<Way>();
        }

        public IList<Way> GetWaysFor(Node node)
        {
            Dictionary<long,Node> nodes = new Dictionary<long,Node>();
            nodes.Add(node.Id,node);
            return this.GetWaysForNodes(nodes);
        }

        public IList<Way> GetWaysForNodes(Dictionary<long,Node> nodes)
        {
            if (nodes.Count > 0)
            {
                OracleConnection con = this.CreateConnection();

                List<long> way_ids = new List<long>();
                for (int idx_1000 = 0; idx_1000 <= nodes.Count / 1000; idx_1000++)
                {
                    // STEP1: Load ways that exist for the given nodes.
                    string sql = "SELECT * FROM way_nodes WHERE (node_id IN ({0})) ";
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = Math.Min((idx_1000 + 1) * 1000, nodes.Count);
                    string ids_string = this.ConstructIdList(nodes.Keys.ToList<long>(), start_idx, stop_idx);
                    if (ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);
                        OracleCommand com = new OracleCommand(sql);
                        com.Connection = con;
                        OracleDataReader reader = com.ExecuteReader();

                        while (reader.Read())
                        {
                            long id = reader.GetInt64(0);
                            if (!way_ids.Contains(id))
                            {
                                way_ids.Add(id);
                            }
                        }
                        reader.Close();
                    }
                }

                return this.GetWays(way_ids, nodes);
            }
            return new List<Way>();
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

        public IList<OsmGeo> Get(GeoCoordinateBox box, Filter filter)
        {
            // initialize connection.
            OracleConnection con = this.CreateConnection();
            List<OsmGeo> base_list = new List<OsmGeo>();

            // calculate bounding box parameters to query db.
            long latitude_min = (long)(box.MinLat * 10000000.0);
            long longitude_min = (long)(box.MinLon * 10000000.0);
            long latitude_max = (long)(box.MaxLat * 10000000.0);
            long longitude_max = (long)(box.MaxLon * 10000000.0);

            // TODO: improve this to allow loading of bigger bb's.
            uint x_min = lon2x(box.MinLon);
            uint x_max = lon2x(box.MaxLon);
            uint y_min = lat2y(box.MinLat);
            uint y_max = lat2y(box.MaxLat);

            IList<long> boxes = new List<long>();

            for (uint x = x_min; x <= x_max; x++)
            {
                for (uint y = y_min; y <= y_max; y++)
                {
                    boxes.Add(this.xy2tile(x, y));
                }
            }

            // STEP 1: query nodes table.
            //id	latitude	longitude	changeset_id	visible	timestamp	tile	version
            string sql
                = "SELECT * FROM node WHERE (visible = 1) AND  (tile IN ({4})) AND (latitude BETWEEN {0} AND {1} AND longitude BETWEEN {2} AND {3})";
            sql = string.Format(sql,
                    latitude_min.ToString(),
                    latitude_max.ToString(),
                    longitude_min.ToString(),
                    longitude_max.ToString(),
                    this.ConstructIdList(boxes));

            // TODO: parameters.
            OracleCommand com = new OracleCommand(sql);
            com.Connection = con;
            OracleDataReader reader = com.ExecuteReader();
            Node node = null;
            Dictionary<long, Node> nodes = new Dictionary<long, Node>();
            List<long> node_ids = new List<long>();
            while (reader.Read())
            {
                // load/parse data.
                long returned_id = reader.GetInt64(0);
                int latitude_int = reader.GetInt32(1);
                int longitude_int = reader.GetInt32(2);
                long changeset_id = reader.GetInt64(3);
                bool visible = reader.GetInt64(4)==1;
                DateTime timestamp = reader.GetDateTime(5);
                long tile = reader.GetInt64(6);
                long version = reader.GetInt64(7);

                // create node.
                node = OsmBaseFactory.CreateNode(returned_id);
                node.Version = version;
                //node.UserId = user_id;
                node.TimeStamp = timestamp;
                node.ChangeSetId = changeset_id;
                node.Coordinate = new GeoCoordinate(
                    ((double)latitude_int) / 10000000.0, ((double)longitude_int) / 10000000.0);

                nodes.Add(node.Id,node);
                node_ids.Add(node.Id);
            }
            reader.Close();

            // STEP2: Load all node tags.
            this.LoadNodeTags(nodes);            

            // STEP3: Load all ways for the given nodes.
            IList<Way> ways = this.GetWaysForNodes(nodes);



            // Add all objects to the base list.
            foreach (Node node_result in nodes.Values.ToList<Node>())
            {
                base_list.Add(node_result);
            }
            foreach (Way way in ways)
            {
                base_list.Add(way);
            }
            return base_list;
        }

        private string ConstructIdList(IList<long> ids)
        {
            return this.ConstructIdList(ids, 0, ids.Count);
        }

        private string ConstructIdList(IList<long> ids,int start_idx,int end_idx)
        {
            string return_string = string.Empty;
            if (ids.Count > 0 && ids.Count > start_idx)
            {
                return_string = return_string + ids[start_idx].ToString();
                for (int i = start_idx + 1; i < end_idx; i++)
                {
                    return_string = return_string + "," + ids[i].ToString();
                }
            }
            return return_string;
        }

        private void LoadNodeTags(Dictionary<long,Node> nodes)
        {
            if (nodes.Count > 0)
            {
                for (int idx_1000 = 0; idx_1000 <= nodes.Count / 1000; idx_1000++)
                {
                    string sql = "SELECT * FROM node_tags WHERE (node_id IN ({0})) ";
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = Math.Min((idx_1000 + 1) * 1000,nodes.Count);
                    string ids = this.ConstructIdList(nodes.Keys.ToList<long>(), start_idx,stop_idx);
                    if(ids.Length > 0)
                    {
                        sql = string.Format(sql, ids);
                        OracleConnection con = this.CreateConnection();
                        OracleCommand com = new OracleCommand(sql);
                        com.Connection = con;
                        OracleDataReader reader = com.ExecuteReader();
                        while (reader.Read())
                        {
                            long returned_id = reader.GetInt64(0);
                            string key = reader.GetString(1);
                            object val = reader.GetValue(2);
                            string value = string.Empty;
                            if (val is string)
                            {
                                value = val as string;
                            }

                            nodes[returned_id].Tags.Add(key, value);

                        }
                        reader.Close();
                    }
                }
            }
        }

        #endregion

        public void Close()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection = null;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
            _connection = null;
        }

        #endregion
    }
}
