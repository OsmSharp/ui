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
using Oracle.ManagedDataAccess.Client;
using OsmSharp.Math.Geo;
using OsmSharp.Osm;
using OsmSharp.Osm.Data;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm.Tiles;
using OsmSharp.Osm.Filters;

namespace OsmSharp.Data.Oracle.Osm
{
    /// <summary>
    /// An OSM data source implementation that can read imported data from and Oracle database.
    /// </summary>
    public class OracleDataSource : DataSourceReadOnlyBase, IDisposable
    {
        /// <summary>
        /// The connection string.
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// The connection id.
        /// </summary>
        private Guid _id;

        /// <summary>
        /// Creates a new oracle data source.
        /// </summary>
        /// <param name="connectionString"></param>
        public OracleDataSource(string connectionString)
        {
            _connectionString = connectionString;
            _id = Guid.NewGuid();
        }

        /// <summary>
        /// Creates a new oracle data source.
        /// </summary>
        /// <param name="connection"></param>
        public OracleDataSource(OracleConnection connection)
        {
            _connection = connection;
            _id = Guid.NewGuid();
        }

        /// <summary>
        /// Holds the connection.
        /// </summary>
        private OracleConnection _connection;

        /// <summary>
        /// Gets/creates the connection.
        /// </summary>
        /// <returns></returns>
        private OracleConnection CreateConnection()
        {
            if (_connection == null)
            {
                _connection = new OracleConnection(_connectionString);
                _connection.Open();
            }
            return _connection;
        }

        #region IDataSourceReadOnly Members

        /// <summary>
        /// Returns the bounding box.
        /// </summary>
        public override GeoCoordinateBox BoundingBox
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// The name of this source.
        /// </summary>
        public string Name
        {
            get
            {
                return "Oracle Data Source";
            }
        }

        /// <summary>
        /// Returns the id of this data source.
        /// </summary>
        public override Guid Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// Returns true if there is a bounding box.
        /// </summary>
        public override bool HasBoundinBox
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if this data source is readonly.
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns all nodes with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override IList<Node> GetNodes(IList<long> ids)
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
                    int stop_idx = System.Math.Min((idx_1000 + 1) * 1000, ids.Count);
                    string sql
                        = "SELECT id, latitude, longitude, changeset_id, visible, timestamp, tile, version, usr, usr_id FROM node WHERE (id IN ({0})) ";
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
                            long? changeset_id = reader.IsDBNull(3) ? null : (long?)reader.GetInt64(3);
                            bool? visible = reader.IsDBNull(4) ? null : (bool?)(reader.GetInt32(4) == 1);
                            DateTime? timestamp = reader.IsDBNull(5) ? null : (DateTime?)reader.GetDateTime(5);
                            long tile = reader.GetInt64(6);
                            ulong? version = reader.IsDBNull(7) ? null : (ulong?)(ulong)reader.GetInt64(7);
                            string usr = reader.IsDBNull(8) ? null : reader.GetString(8);
                            long? usr_id = reader.IsDBNull(9) ? null : (long?)reader.GetInt64(9);

                            if (!nodes.ContainsKey(returned_id))
                            {
                                // create node.
                                node = new Node();
                                node.Id = returned_id;
                                node.Version = version;
                                node.UserId = usr_id;
                                node.TimeStamp = timestamp;
                                node.ChangeSetId = changeset_id;
                                node.Visible = visible;
                                node.Latitude = ((double)latitude_int) / 10000000.0;
                                node.Longitude = ((double)longitude_int) / 10000000.0;
                                node.UserName = usr;

                                nodes.Add(node.Id.Value, node);
                                node_ids.Add(node.Id.Value);
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
        
        /// <summary>
        /// Returns all ways containing the given nodes.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override IList<Way> GetWays(IList<long> ids)
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
                    int stop_idx = System.Math.Min((idx_1000 + 1) * 1000, ids.Count);

                    sql = "SELECT id, changeset_id, timestamp, visible, version, usr, usr_id FROM way WHERE (id IN ({0})) ";
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
                            long? changeset_id = reader.IsDBNull(1) ? null : (long?)reader.GetInt64(1);
                            DateTime? timestamp = reader.IsDBNull(2) ? null : (DateTime?)reader.GetDateTime(2);
                            bool? visible = reader.IsDBNull(3) ? null : (bool?)(reader.GetInt16(3) == 1);
                            ulong? version = reader.IsDBNull(4) ? null : (ulong?)reader.GetInt64(4);
                            string user = reader.IsDBNull(5) ? null : reader.GetString(5);
                            long? user_id = reader.IsDBNull(6) ? null : (long?)reader.GetInt64(6);

                            // create way.
                            way = new Way();
                            way.Id = id;
                            way.Version = version;
                            way.UserName = user;
                            way.UserId = user_id;
                            way.Visible = visible;
                            way.TimeStamp = timestamp;
                            way.ChangeSetId = changeset_id;

                            ways.Add(way.Id.Value, way);
                        }
                        reader.Close();
                    }
                }

                //STEP3: Load all node-way relations
                List<long> missing_node_ids = new List<long>();
                for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
                {
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = System.Math.Min((idx_1000 + 1) * 1000, ids.Count);

                    sql = "SELECT way_id, node_id, sequence_id FROM way_nodes WHERE (way_id IN ({0})) ORDER BY sequence_id";
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

                            Way way;
                            if (ways.TryGetValue(id, out way))
                            {
                                if (way.Nodes == null)
                                {
                                    way.Nodes = new List<long>();
                                }
                                way.Nodes.Add(node_id);
                            }
                        }
                        reader.Close();
                    }
                }

                //STEP4: Load all tags.
                for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
                {
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = System.Math.Min((idx_1000 + 1) * 1000, ids.Count);

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
                            string key = reader.IsDBNull(1) ? null : reader.GetString(1);
                            string val = reader.IsDBNull(2) ? null : reader.GetString(2);

                            Way way;
                            if (ways.TryGetValue(id, out way))
                            {
                                if (way.Tags == null)
                                {
                                    way.Tags = new TagsCollection();
                                }
                                way.Tags.Add(key, val);
                            }
                        }
                        reader.Close();
                    }
                }

                return ways.Values.ToList<Way>();
            }
            return new List<Way>();
        }

        /// <summary>
        /// Returns all the ways that contain the given node.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override IList<Way> GetWaysFor(long id)
        {
            List<long> ids = new List<long>();
            ids.Add(id);
            return this.GetWaysFor(ids);
        }

        /// <summary>
        /// Returns all the ways that contain any of the given nodes.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Way> GetWaysFor(List<long> ids)
        {
            if (ids.Count > 0)
            {
                OracleConnection con = CreateConnection();
                OracleCommand com;
                OracleDataReader reader;

                HashSet<long> wayIds = new HashSet<long>();
                for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
                {
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = System.Math.Min((idx_1000 + 1) * 1000, ids.Count);

                    string sql = "SELECT * FROM way_nodes WHERE (node_id IN ({0})) ORDER BY sequence_id";
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
                            wayIds.Add(id);
                        }
                        reader.Close();
                    }
                }

                return this.GetWays(wayIds.ToList<long>());
            }

            return new List<Way>();
        }

        /// <summary>
        /// Returns the relations for the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override IList<Relation> GetRelations(IList<long> ids)
        {
            if (ids.Count > 0)
            {
                OracleConnection con = this.CreateConnection();

                // STEP2: Load ways.
                Dictionary<long, Relation> relations = new Dictionary<long, Relation>();
                string sql;
                OracleCommand com;
                OracleDataReader reader;
                for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
                {
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = System.Math.Min((idx_1000 + 1) * 1000, ids.Count);

                    sql = "SELECT id, changeset_id, visible, timestamp, version, usr, usr_id FROM relation WHERE (id IN ({0})) ";
                    string ids_string = this.ConstructIdList(ids, start_idx, stop_idx);
                    if (ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);
                        com = new OracleCommand(sql);
                        com.Connection = con;
                        reader = com.ExecuteReader();
                        Relation relation;
                        while (reader.Read())
                        {
                            long id = reader.GetInt64(0);
                            long? changeset_id = reader.IsDBNull(1) ? null : (long?)reader.GetInt64(1);
                            bool? visible = reader.IsDBNull(2) ? null : (bool?)(reader.GetInt16(2) == 1);
                            DateTime? timestamp = reader.IsDBNull(3) ? null : (DateTime?)reader.GetDateTime(3);
                            ulong? version = reader.IsDBNull(4) ? null : (ulong?)reader.GetInt64(4);
                            string user = reader.IsDBNull(5) ? null : reader.GetString(5);
                            long? user_id = reader.IsDBNull(6) ? null : (long?)reader.GetInt64(6);

                            // create way.
                            relation = new Relation();
                            relation.Id = id;
                            relation.Version = version;
                            relation.UserName = user;
                            relation.UserId = user_id;
                            relation.Visible = visible;
                            relation.TimeStamp = timestamp;
                            relation.ChangeSetId = changeset_id;

                            relations.Add(relation.Id.Value, relation);
                        }
                        reader.Close();
                    }
                }

                //STEP3: Load all relation-member relations
                List<long> missing_node_ids = new List<long>();
                for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
                {
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = System.Math.Min((idx_1000 + 1) * 1000, ids.Count);

                    sql = "SELECT relation_id, member_type, member_id, member_role, sequence_id FROM relation_members WHERE (relation_id IN ({0})) ORDER BY sequence_id";
                    string ids_string = this.ConstructIdList(ids, start_idx, stop_idx);
                    if (ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);
                        com = new OracleCommand(sql);
                        com.Connection = con;
                        reader = com.ExecuteReader();
                        while (reader.Read())
                        {
                            long relation_id = reader.GetInt64(0);
                            long member_type = reader.GetInt16(1);
                            long? member_id = reader.IsDBNull(2) ? null : (long?)reader.GetInt64(2);
                            string member_role = reader.IsDBNull(3) ? null : reader.GetString(3);

                            Relation relation;
                            if (relations.TryGetValue(relation_id, out relation))
                            {
                                if (relation.Members == null)
                                {
                                    relation.Members = new List<RelationMember>();
                                }
                                RelationMember member = new RelationMember();
                                member.MemberId = member_id;
                                member.MemberRole = member_role;
                                member.MemberType = this.ConvertMemberType(member_type);

                                relation.Members.Add(member);
                            }
                        }
                        reader.Close();
                    }
                }

                //STEP4: Load all tags.
                for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
                {
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = System.Math.Min((idx_1000 + 1) * 1000, ids.Count);

                    sql = "SELECT * FROM relation_tags WHERE (relation_id IN ({0})) ";
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

                            Relation relation;
                            if (relations.TryGetValue(id, out relation))
                            {
                                if (relation.Tags == null)
                                {
                                    relation.Tags = new TagsCollection();
                                }
                                relation.Tags.Add(key, value);
                            }
                        }
                        reader.Close();
                    }
                }

                return relations.Values.ToList<Relation>();
            }
            return new List<Relation>();
        }

        /// <summary>
        /// Converts the member type id to the relationmembertype enum.
        /// </summary>
        /// <param name="member_type"></param>
        /// <returns></returns>
        private OsmGeoType? ConvertMemberType(long member_type)
        {
            switch (member_type)
            {
                case (long)OsmGeoType.Node:
                    return OsmGeoType.Node;
                case (long)OsmGeoType.Way:
                    return OsmGeoType.Way;
                case (long)OsmGeoType.Relation:
                    return OsmGeoType.Relation;
            }
            throw new ArgumentOutOfRangeException("Invalid member type.");
        }

        /// <summary>
        /// Converts the member type to long.
        /// </summary>
        /// <param name="memberType"></param>
        /// <returns></returns>
        private long? ConvertMemberType(OsmGeoType? memberType)
        {
            if (memberType.HasValue)
            {
                return (long)memberType.Value;
            }
            return null;
        }

        /// <summary>
        /// Returns all relations that contain the given object.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public override IList<Relation> GetRelationsFor(OsmGeoType type, long id)
        {
            OracleConnection con = CreateConnection();
            OracleCommand com;
            OracleDataReader reader;

            string sql = "SELECT relation_id FROM relation_members WHERE (member_id = :member_id and member_type = :member_type) ORDER BY sequence_id";
            com = new OracleCommand(sql);
            com.Connection = con;
            com.Parameters.Add(new OracleParameter("member_id", OracleDbType.Int64));
            com.Parameters.Add(new OracleParameter("member_type", OracleDbType.Int64));
            com.Parameters[0].Value = id;
            com.Parameters[1].Value = this.ConvertMemberType(type).Value;

            HashSet<long> ids = new HashSet<long>();
            reader = com.ExecuteReader();
            while (reader.Read())
            {
                ids.Add(reader.GetInt64(0));
            }
            reader.Close();

            return this.GetRelations(ids.ToList<long>());
        }

        /// <summary>
        /// Returns all objects with the given bounding box and valid for the given filter;
        /// </summary>
        /// <param name="box"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override IList<OsmGeo> Get(GeoCoordinateBox box, Filter filter)
        {
            // initialize connection.
            OracleConnection con = this.CreateConnection();
            List<OsmGeo> res = new List<OsmGeo>();

            // calculate bounding box parameters to query db.
            long latitudeMin = (long)(box.MinLat * 10000000.0);
            long longitudeMin = (long)(box.MinLon * 10000000.0);
            long latitudeMax = (long)(box.MaxLat * 10000000.0);
            long longitudeMax = (long)(box.MaxLon * 10000000.0);

            IList<long> boxes = new List<long>();

            TileRange tileRange = TileRange.CreateAroundBoundingBox(box, 14);
            foreach (Tile tile in tileRange)
            {
                boxes.Add((long)tile.Id);
            }

            // STEP 1: query nodes table.
            //id	latitude	longitude	changeset_id	visible	timestamp	tile	version
            //string sql
            //        = "SELECT node.id, node.latitude, node.longitude, node.changeset_id, node.timestamp, node.version, " +
            //          "node.usr, node.usr_id, node.visible FROM node WHERE  (tile IN ({4})) AND (visible = 1) AND (latitude BETWEEN {0} AND {1} AND longitude BETWEEN {2} AND {3})";
            // remove this nasty BETWEEN operation because it depends on the database (!) what results are returned (including or excluding bounds!!!)
            string sql
                = "SELECT id, latitude, longitude, changeset_id, visible, timestamp, tile, version, usr, usr_id " +
                  "FROM node WHERE  (tile IN ({4})) AND (visible = 1) AND (latitude >= {0} AND latitude < {1} AND longitude >= {2} AND longitude < {3})";
            sql = string.Format(sql,
                        latitudeMin.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        latitudeMax.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        longitudeMin.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        longitudeMax.ToString(System.Globalization.CultureInfo.InvariantCulture),
                        this.ConstructIdList(boxes));

            // TODO: parameters.
            var com = new OracleCommand(sql);
            com.Connection = con;
            OracleDataReader reader = com.ExecuteReader();
            Node node = null;
            var nodes = new Dictionary<long, Node>();
            var nodeIds = new List<long>();
            while (reader.Read())
            {
                // load/parse data.
                long returned_id = reader.GetInt64(0);
                int latitude_int = reader.GetInt32(1);
                int longitude_int = reader.GetInt32(2);
                long? changeset_id = reader.IsDBNull(3) ? null : (long?)reader.GetInt64(3);
                bool? visible = reader.IsDBNull(4) ? null : (bool?)(reader.GetInt32(4) == 1);
                DateTime? timestamp = reader.IsDBNull(5) ? null : (DateTime?)reader.GetDateTime(5);
                long tile = reader.GetInt64(6);
                ulong? version = reader.IsDBNull(7) ? null : (ulong?)(ulong)reader.GetInt64(7);
                string usr = reader.IsDBNull(8) ? null : reader.GetString(8);
                long? usr_id = reader.IsDBNull(9) ? null : (long?)reader.GetInt64(9);

                if (!nodes.ContainsKey(returned_id))
                {
                    // create node.
                    node = new Node();
                    node.Id = returned_id;
                    node.Version = version;
                    node.UserId = usr_id;
                    node.TimeStamp = timestamp;
                    node.ChangeSetId = changeset_id;
                    node.Visible = visible;
                    node.Latitude = ((double)latitude_int) / 10000000.0;
                    node.Longitude = ((double)longitude_int) / 10000000.0;
                    node.UserName = usr;

                    nodeIds.Add(node.Id.Value);
                    nodes.Add(node.Id.Value, node);
                }
            }
            reader.Close();

            // STEP2: Load all node tags.
            this.LoadNodeTags(nodes);
            res.AddRange(nodes.Values);

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
                    int stop_idx = System.Math.Min((idx_1000 + 1) * 1000, nodes.Count);
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
                            string key = reader.IsDBNull(1) ? null : reader.GetString(1);
                            string val = reader.IsDBNull(2) ? null : reader.GetString(2);

                            Node node;
                            if (nodes.TryGetValue(returned_id, out node))
                            {
                                if (node.Tags == null)
                                {
                                    node.Tags = new TagsCollection();
                                }
                                node.Tags.Add(key, val);
                            }
                        }
                        reader.Close();
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Closes the source.
        /// </summary>
        public void Close()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection = null;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Releases all resources.
        /// </summary>
        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
            _connection = null;
        }

        #endregion
    }
}
