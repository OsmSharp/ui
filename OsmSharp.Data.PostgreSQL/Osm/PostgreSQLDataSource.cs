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
using Npgsql;
using OsmSharp.Math.Geo;
using OsmSharp.Osm;
using OsmSharp.Osm.Data;
using OsmSharp.Collections.Tags;
using System.Data;
using OsmSharp.Osm.Tiles;

namespace OsmSharp.Data.PostgreSQL.Osm
{
    /// <summary>
    /// Allows a version of the OsmSharp simple schema to be queried in PostgreSQL.
    /// 
    /// http://www.osmsharp.com/wiki/simpleschema
    /// </summary>
    public class PostgreSQLDataSource : DataSourceReadOnlyBase, IDisposable
    {
        /// <summary>
        /// Holds the connection string.
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// The id of this datasource.
        /// </summary>
        private Guid _id;

        /// <summary>
        /// Flag that indicates if the schema needs to be created if not present.
        /// </summary>
        private bool _createAndDetectSchema;

        /// <summary>
        /// Creates a new simple schema datasource.
        /// </summary>
        /// <param name="connectionString"></param>
        public PostgreSQLDataSource(string connectionString)
        {
            _connectionString = connectionString;
            _id = Guid.NewGuid();
            _createAndDetectSchema = false;
        }

        /// <summary>
        /// Creates a new simple schema datasource.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="createSchema">Creates all the needed tables if true.</param>
        public PostgreSQLDataSource(string connectionString, bool createSchema)
        {
            _connectionString = connectionString;
            _id = Guid.NewGuid();
            _createAndDetectSchema = createSchema;
        }

        /// <summary>
        /// Creates a datasource.
        /// </summary>
        /// <param name="connection"></param>
        public PostgreSQLDataSource(NpgsqlConnection connection)
        {
            _connection = connection;
            _id = Guid.NewGuid();
            _createAndDetectSchema = false;
        }

        /// <summary>
        /// Holds the connection to the PostgreSQL db.
        /// </summary>
        private NpgsqlConnection _connection;

        /// <summary>
        /// Creates a new/gets the existing connection.
        /// </summary>
        /// <returns></returns>
        private NpgsqlConnection CreateConnection()
        {
            if (_connection == null)
            {
                _connection = new NpgsqlConnection(_connectionString);
                _connection.Open();

                if (_createAndDetectSchema)
                { // creates or detects the tables.
                    PostgreSQLSchemaTools.CreateAndDetect(_connection);
                }
            }
            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }
            return _connection;
        }

        #region IDataSourceReadOnly Members

        /// <summary>
        /// Not supported.
        /// </summary>
        public override GeoCoordinateBox BoundingBox
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Returns the name.
        /// </summary>
        public string Name
        {
            get
            {
                return "PostgreSQL Schema Source";
            }
        }

        /// <summary>
        /// Returns the id.
        /// </summary>
        public override Guid Id
        {
            get
            {
                return _id;
            }
        }

        /// <summary>
        /// Returns false; database sources have no bounding box.
        /// </summary>
        public override bool HasBoundinBox
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Return true; source is readonly.
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns all the nodes with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override IList<Node> GetNodes(IList<long> ids)
        {
            IList<Node> returnList = new List<Node>();
            if (ids.Count > 0)
            {
                // initialize connection.
                NpgsqlConnection con = this.CreateConnection();
                // STEP 1: query nodes table.
                // id, latitude, longitude, changeset_id, visible, timestamp, tile, version, usr, usr_id

                Dictionary<long, Node> nodes = new Dictionary<long, Node>();
                for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
                {
                    int startIdx = idx_1000 * 1000;
                    int stopIdx = System.Math.Min((idx_1000 + 1) * 1000, ids.Count);
                    string sql = "SELECT id, latitude, longitude, changeset_id, visible, timestamp, tile, version, usr, usr_id FROM node WHERE (id IN ({0})) ";
                    string ids_string = this.ConstructIdList(ids,startIdx,stopIdx);
                    if (ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);
                        
                        NpgsqlCommand com = new NpgsqlCommand(sql);
                        com.Connection = con;
                        NpgsqlDataReader reader = com.ExecuteReader();
                        Node node = null;
                        List<long> nodeIds = new List<long>();
                        while (reader.Read())
                        {
                            // load/parse data.
                            long returned_id = reader.GetInt64(0);
                            int latitude_int = reader.GetInt32(1);
                            int longitude_int = reader.GetInt32(2);
                            long? changeset_id = reader.IsDBNull(3) ? null : (long?)reader.GetInt64(3);
                            bool? visible = reader.IsDBNull(4) ? null : (bool?)reader.GetBoolean(4);
                            DateTime? timestamp = reader.IsDBNull(5) ? null : (DateTime?)reader.GetDateTime(5);
                            long tile = reader.GetInt64(6);
                            ulong? version = reader.IsDBNull(7) ? null : (ulong?)reader.GetInt32(7);
                            string usr = reader.IsDBNull(8) ? null : reader.GetString(8);
                            long? usr_id = reader.IsDBNull(9) ? null : (long?)reader.GetInt32(9);

                            if (!nodes.ContainsKey(returned_id))
                            {
                                // create node.
                                node = new Node();
                                node.Id = returned_id;
                                node.Version = version;
                                node.UserId = usr_id;
                                node.UserName = usr;
                                node.TimeStamp = timestamp;
                                node.ChangeSetId = changeset_id;
                                node.Latitude = ((double)latitude_int) / 10000000.0;
                                node.Longitude = ((double)longitude_int) / 10000000.0;
                                node.Visible = visible;

                                nodes.Add(node.Id.Value, node);
                                nodeIds.Add(node.Id.Value);
                            }
                        }
                        reader.Close();
                    }
                }

                // STEP2: Load all node tags.
                this.LoadNodeTags(nodes);

                returnList = nodes.Values.ToList<Node>();
            }
            return returnList;
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
                NpgsqlConnection con = this.CreateConnection();

                // STEP2: Load ways.
                Dictionary<long, Relation> relations = new Dictionary<long, Relation>();
                string sql;
                NpgsqlCommand com;
                NpgsqlDataReader reader;
                for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
                {
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = System.Math.Min((idx_1000 + 1) * 1000, ids.Count);

                    sql = "SELECT id, changeset_id, visible, timestamp, version, usr, usr_id FROM relation WHERE (id IN ({0})) ";
                    string ids_string = this.ConstructIdList(ids, start_idx, stop_idx);
                    if (ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);
                        com = new NpgsqlCommand(sql);
                        com.Connection = con;
                        reader = com.ExecuteReader();
                        Relation relation;
                        while (reader.Read())
                        {
                            long id = reader.GetInt64(0);
                            long? changeset_id = reader.IsDBNull(1) ? null : (long?)reader.GetInt64(1);
                            bool? visible = reader.IsDBNull(2) ? null : (bool?)reader.GetBoolean(2);
                            DateTime? timestamp = reader.IsDBNull(3) ? null : (DateTime?)reader.GetDateTime(3);
                            ulong? version = reader.IsDBNull(4) ? null : (ulong?)reader.GetInt32(4);
                            string user = reader.IsDBNull(5) ? null : reader.GetString(5);
                            long? user_id = reader.IsDBNull(6) ? null : (long?)reader.GetInt32(6);

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
                        com = new NpgsqlCommand(sql);
                        com.Connection = con;
                        reader = com.ExecuteReader();
                        while (reader.Read())
                        {
                            long relation_id = reader.GetInt64(0);
                            long member_type = reader.GetInt32(1);
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
                        com = new NpgsqlCommand(sql);
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
        /// Returns all relations for the given objects.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public override IList<Relation> GetRelationsFor(OsmGeoType type, long id)
        {
            NpgsqlConnection con = CreateConnection();
            NpgsqlCommand com;
            NpgsqlDataReader reader;

            string sql = "SELECT relation_id FROM relation_members WHERE (member_id = :member_id and member_type = :member_type) ORDER BY sequence_id";
            com = new NpgsqlCommand(sql);
            com.Connection = con;
            com.Parameters.Add(new NpgsqlParameter(@"member_type", DbType.Int64));
            com.Parameters.Add(new NpgsqlParameter(@"member_id", DbType.Int64));
            com.Parameters[0].Value = this.ConvertMemberType(type).Value;
            com.Parameters[1].Value = id;

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
        /// Returns all ways but use the existing nodes to fill the Nodes-lists.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override IList<Way> GetWays(IList<long> ids)
        {
            if (ids.Count > 0)
            {
                NpgsqlConnection con = this.CreateConnection();

                // STEP2: Load ways.
                Dictionary<long, Way> ways = new Dictionary<long, Way>();
                string sql;
                NpgsqlCommand com;
                NpgsqlDataReader reader;
                for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
                {
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = System.Math.Min((idx_1000 + 1) * 1000, ids.Count);

                    sql = "SELECT id, changeset_id, visible, timestamp, version, usr, usr_id FROM way WHERE (id IN ({0})) ";
                    string ids_string = this.ConstructIdList(ids,start_idx,stop_idx);
                    if(ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);
                        com = new NpgsqlCommand(sql);
                        com.Connection = con;
                        reader = com.ExecuteReader();
                        Way way;
                        while (reader.Read())
                        {
                            // load/parse data.
                            // id, changeset_id, visible, timestamp, tile, version, usr, usr_id
                            long returned_id = reader.GetInt64(0);
                            long? changeset_id = reader.IsDBNull(1) ? null : (long?)reader.GetInt64(1);
                            bool? visible = reader.IsDBNull(2) ? null : (bool?)reader.GetBoolean(2);
                            DateTime? timestamp = reader.IsDBNull(3) ? null : (DateTime?)reader.GetDateTime(3);
                            ulong? version = reader.IsDBNull(4) ? null : (ulong?)reader.GetInt32(4);
                            string usr = reader.IsDBNull(5) ? null : reader.GetString(5);
                            long? usr_id = reader.IsDBNull(6) ? null : (long?)reader.GetInt32(6);

                            // create way.
                            way = new Way();
                            way.Id = returned_id;
                            way.Version = version;
                            way.UserId = usr_id;
                            way.UserName = usr;
                            way.TimeStamp = timestamp;
                            way.ChangeSetId = changeset_id;
                            way.Visible = visible;

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

                    sql = "SELECT * FROM way_nodes WHERE (way_id IN ({0})) ORDER BY sequence_id";
                    string ids_string = this.ConstructIdList(ids, start_idx, stop_idx);
                    if (ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);
                        com = new NpgsqlCommand(sql);
                        com.Connection = con;
                        reader = com.ExecuteReader();
                        while (reader.Read())
                        {
                            long id = reader.GetInt64(0);
                            long node_id = reader.GetInt64(1);
                            long sequence_id = reader.GetInt32(2);

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
                        com = new NpgsqlCommand(sql);
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
                                if (way.Tags == null)
                                {
                                    way.Tags = new TagsCollection();
                                }
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

        /// <summary>
        /// Returns all ways using the given node.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override IList<Way> GetWaysFor(long id)
        {
            List<long> nodes = new List<long>();
            nodes.Add(id);
            return this.GetWaysFor(nodes);
        }

        /// <summary>
        /// Returns all ways using any of the given nodes.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Way> GetWaysFor(List<long> ids)
        {
            if (ids.Count > 0)
            {
                NpgsqlConnection con = this.CreateConnection();

                List<long> wayIds = new List<long>();
                for (int idx_100 = 0; idx_100 <= ids.Count / 100; idx_100++)
                {
                    // STEP1: Load ways that exist for the given nodes.
                    string sql = "SELECT way_id FROM way_nodes WHERE (node_id IN ({0})) ";
                    int start_idx = idx_100 * 100;
                    int stop_idx = System.Math.Min((idx_100 + 1) * 100, ids.Count);
                    string ids_string = this.ConstructIdList(ids, start_idx, stop_idx);
                    if (ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);
                        NpgsqlCommand com = new NpgsqlCommand(sql);
                        com.Connection = con;
                        NpgsqlDataReader reader = com.ExecuteReader();

                        while (reader.Read())
                        {
                            long id = reader.GetInt64(0);
                            if (!wayIds.Contains(id))
                            {
                                wayIds.Add(id);
                            }
                        }
                        reader.Close();
                        com.Dispose();
                    }
                }

                return this.GetWays(wayIds);
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
            return (uint)System.Math.Floor(((lon + 180.0) * 65536.0 / 360.0));
        }

        uint lat2y(double lat)
        {
            return (uint)System.Math.Floor(((lat + 90.0) * 65536.0 / 180.0));
        }

        #endregion

        /// <summary>
        /// Returns all data within the given bounding box and filtered by the given filter.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override IList<OsmGeo> Get(GeoCoordinateBox box, OsmSharp.Osm.Filters.Filter filter)
        {
            // initialize connection.
            NpgsqlConnection con = this.CreateConnection();
            List<OsmGeo> res = new List<OsmGeo>();

            // calculate bounding box parameters to query db.
            long latitude_min = (long)(box.MinLat * 10000000.0);
            long longitude_min = (long)(box.MinLon * 10000000.0);
            long latitude_max = (long)(box.MaxLat * 10000000.0);
            long longitude_max = (long)(box.MaxLon * 10000000.0);

            // calculate bounding box parameters to query db.
            TileRange range = TileRange.CreateAroundBoundingBox(box, 14);

            IList<long> boxes = new List<long>();

            foreach (Tile tile in range)
            {
                boxes.Add((long)tile.Id);
            }

            // STEP 1: query nodes table.
            //id	latitude	longitude	changeset_id	visible	timestamp	tile	version
            string sql
                = "SELECT id, latitude, longitude, changeset_id, visible, timestamp, tile, version, usr, usr_id FROM node WHERE (visible = true) AND  (tile IN ({4})) AND (latitude >= {0} AND latitude < {1} AND longitude >= {2} AND longitude < {3})";
            sql = string.Format(sql,
                    latitude_min.ToString(),
                    latitude_max.ToString(),
                    longitude_min.ToString(),
                    longitude_max.ToString(),
                    this.ConstructIdList(boxes));

            // TODO: parameters.
            NpgsqlCommand com = new NpgsqlCommand(sql);
            com.Connection = con;
            NpgsqlDataReader reader = com.ExecuteReader();
            Node node = null;
            Dictionary<long, Node> nodes = new Dictionary<long, Node>();
            List<long> nodeIds = new List<long>();
            while (reader.Read())
            {
                // load/parse data.
                long returned_id = reader.GetInt64(0);
                int latitude_int = reader.GetInt32(1);
                int longitude_int = reader.GetInt32(2);
                long? changeset_id = reader.IsDBNull(3) ? null : (long?)reader.GetInt64(3);
                bool? visible = reader.IsDBNull(4) ? null : (bool?)reader.GetBoolean(4);
                DateTime? timestamp = reader.IsDBNull(5) ? null : (DateTime?)reader.GetDateTime(5);
                long tile = reader.GetInt64(6);
                ulong? version = reader.IsDBNull(7) ? null : (ulong?)reader.GetInt32(7);
                string usr = reader.IsDBNull(8) ? null : reader.GetString(8);
                long? usr_id = reader.IsDBNull(9) ? null : (long?)reader.GetInt32(9);

                if (!nodes.ContainsKey(returned_id))
                {
                    // create node.
                    node = new Node();
                    node.Id = returned_id;
                    node.Version = version;
                    node.UserId = usr_id;
                    node.UserName = usr;
                    node.TimeStamp = timestamp;
                    node.ChangeSetId = changeset_id;
                    node.Latitude = ((double)latitude_int) / 10000000.0;
                    node.Longitude = ((double)longitude_int) / 10000000.0;
                    node.Visible = visible;

                    nodes.Add(node.Id.Value, node);
                    nodeIds.Add(node.Id.Value);
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

        /// <summary>
        /// Constructs an id list for SQL.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private string ConstructIdList(IList<long> ids)
        {
            return this.ConstructIdList(ids, 0, ids.Count);
        }

        /// <summary>
        /// Constructs an id list for SQL for only the specified section of ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="start_idx"></param>
        /// <param name="end_idx"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Loads all tags for the given nodes.
        /// </summary>
        /// <param name="nodes"></param>
        private void LoadNodeTags(Dictionary<long,Node> nodes)
        {
            if (nodes.Count > 0)
            {
                for (int idx_1000 = 0; idx_1000 <= nodes.Count / 1000; idx_1000++)
                {
                    string sql = "SELECT * FROM node_tags WHERE (node_id IN ({0})) ";
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = System.Math.Min((idx_1000 + 1) * 1000,nodes.Count);
                    string ids = this.ConstructIdList(nodes.Keys.ToList<long>(), start_idx,stop_idx);
                    if(ids.Length > 0)
                    {
                        sql = string.Format(sql, ids);
                        NpgsqlConnection con = this.CreateConnection();
                        NpgsqlCommand com = new NpgsqlCommand(sql);
                        com.Connection = con;
                        NpgsqlDataReader reader = com.ExecuteReader();
                        while (reader.Read())
                        {
                            long returned_id = reader.GetInt64(0);
                            string key = reader.GetString(1);
                            object val = reader.GetValue(2);
                            string value = string.Empty;
                            Node node;
                            if(nodes.TryGetValue(returned_id, out node))
                            {
                                if (val is string)
                                {
                                    value = val as string;
                                }
                                if(node.Tags == null)
                                {
                                    node.Tags = new TagsCollection();
                                }

                                nodes[returned_id].Tags.Add(key, value);
                            }

                        }
                        reader.Close();
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Closes this datasource.
        /// </summary>
        public void Close()
        {
            if (_connection != null && !string.IsNullOrWhiteSpace(_connectionString))
            { // connection exists and was created here; close it here!
                _connection.Close();
                _connection = null;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Diposes the resources used in this datasource.
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
