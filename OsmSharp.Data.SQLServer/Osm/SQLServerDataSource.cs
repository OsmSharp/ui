// OsmSharp - OpenStreetMap (OSM) SDK
//
// Copyright (C) 2013 Abelshausen Ben
//                    Alexander Sinitsyn
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
using System.Data.SqlClient;
using OsmSharp.Osm.Data;
using OsmSharp.Data.SQLServer.Osm.SchemaTools;
using OsmSharp.Math.Geo;
using OsmSharp.Osm;
using OsmSharp.Osm.Filters;
using OsmSharp.Collections.Tags;
using System.Data;
using OsmSharp.Osm.Tiles;

namespace OsmSharp.Data.SQLServer.Osm
{
    /// <summary>
    /// Allows a version of the OsmSharp simple schema to be queried in SQLServer.
    /// 
    /// http://www.osmsharp.com/wiki/simpleschema
    /// </summary>
    public class SQLServerDataSource : DataSourceReadOnlyBase, IDisposable
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
        public SQLServerDataSource(string connectionString)
        {
            _connectionString = connectionString;
            _id = Guid.NewGuid();
            _createAndDetectSchema = false;
        }

        /// <summary>
        /// Creates a new simple schema datasource.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public SQLServerDataSource(SqlConnection connection)
        {
            _connection = connection;
            _id = Guid.NewGuid();
            _createAndDetectSchema = false;
        }

        /// <summary>
        /// Creates a new simple schema datasource.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="createSchema">Creates all the needed tables if true.</param>
        public SQLServerDataSource(SqlConnection connection, bool createSchema)
        {
            _connection = connection;
            _id = Guid.NewGuid();
            _createAndDetectSchema = createSchema;
        }

        /// <summary>
        /// Creates a new simple schema datasource.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="createSchema">Creates all the needed tables if true.</param>
        public SQLServerDataSource(string connectionString, bool createSchema)
        {
            _connectionString = connectionString;
            _id = Guid.NewGuid();
            _createAndDetectSchema = createSchema;
        }

        /// <summary>
        /// Holds the connection to the SQLServer db.
        /// </summary>
        private SqlConnection _connection;

        /// <summary>
        /// Creates a new/gets the existing connection.
        /// </summary>
        /// <returns></returns>
        private SqlConnection CreateConnection()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();

                if (_createAndDetectSchema)
                { // creates or detects the tables.
                    SQLServerSchemaTools.CreateAndDetect(_connection);
                }
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
                return "SQLServer Schema Source";
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
        /// Returns all the nodes with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override IList<Node> GetNodes(IList<long> ids)
        {
            IList<Node> return_list = new List<Node>();
            if (ids.Count > 0)
            {
                // initialize connection.
                SqlConnection con = this.CreateConnection();
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
                        
                        SqlCommand com = new SqlCommand(sql);
                        com.Connection = con;
                        SqlDataReader reader = com.ExecuteReader();
                        Node node = null;
                        List<long> node_ids = new List<long>();
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
                            ulong? version = reader.IsDBNull(7) ? null : (ulong?)(ulong)reader.GetInt32(7);
                            string usr = reader.IsDBNull(8) ? null : reader.GetString(8);
                            long? usr_id = reader.IsDBNull(9) ? null : (long?)reader.GetInt32(9);

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
        /// Returns all the relations with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override IList<Relation> GetRelations(IList<long> ids)
        {
            if (ids.Count > 0)
            {
                SqlConnection con = this.CreateConnection();

                // STEP2: Load ways.
                Dictionary<long, Relation> relations = new Dictionary<long, Relation>();
                string sql;
                SqlCommand com;
                SqlDataReader reader;
                for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
                {
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = System.Math.Min((idx_1000 + 1) * 1000, ids.Count);

                    sql = "SELECT id, changeset_id, visible, timestamp, version, usr, usr_id FROM relation WHERE (id IN ({0})) ";
                    string ids_string = this.ConstructIdList(ids, start_idx, stop_idx);
                    if (ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);
                        com = new SqlCommand(sql);
                        com.Connection = con;
                        reader = com.ExecuteReader();
                        Relation relation;
                        while (reader.Read())
                        {
                            long id = reader.GetInt64(0);
                            long? changeset_id = reader.IsDBNull(1) ? null : (long?)reader.GetInt64(1);
                            bool? visible = reader.IsDBNull(2) ? null : (bool?)(reader.GetValue(2));
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
                        com = new SqlCommand(sql);
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
                        com = new SqlCommand(sql);
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
        /// Returns all relations that contain the given object.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public override IList<Relation> GetRelationsFor(OsmGeoType type, long id)
        {
            SqlConnection con = CreateConnection();
            SqlCommand com;
            SqlDataReader reader;

            string sql = "SELECT relation_id FROM relation_members WHERE (member_id = @member_id and member_type = @member_type) ORDER BY sequence_id";
            com = new SqlCommand(sql);
            com.Connection = con;
            com.Parameters.Add(new SqlParameter("member_id", SqlDbType.BigInt));
            com.Parameters.Add(new SqlParameter("member_type", SqlDbType.Int));
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
        /// Returns all ways with the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override IList<Way> GetWays(IList<long> ids)
        {
            if (ids.Count > 0)
            {
                SqlConnection con = this.CreateConnection();

                // STEP2: Load ways.
                Dictionary<long, Way> ways = new Dictionary<long, Way>();
                string sql;
                SqlCommand com;
                SqlDataReader reader;
                for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
                {
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = System.Math.Min((idx_1000 + 1) * 1000, ids.Count);

                    sql = "SELECT id, changeset_id, timestamp, visible, version, usr, usr_id FROM way WHERE (id IN ({0})) ";
                    string ids_string = this.ConstructIdList(ids,start_idx,stop_idx);
                    if(ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);
                        com = new SqlCommand(sql);
                        com.Connection = con;
                        reader = com.ExecuteReader();
                        Way way;
                        while (reader.Read())
                        {
                            long id = reader.GetInt64(0);
                            long? changeset_id = reader.IsDBNull(1) ? null : (long?)reader.GetInt64(1);
                            DateTime? timestamp = reader.IsDBNull(2) ? null : (DateTime?)reader.GetDateTime(2);
                            bool? visible = reader.IsDBNull(3) ? null : (bool?)(reader.GetValue(3));
                            ulong? version = reader.IsDBNull(4) ? null : (ulong?)reader.GetInt32(4);
                            string user = reader.IsDBNull(5) ? null : reader.GetString(5);
                            long? user_id = reader.IsDBNull(6) ? null : (long?)reader.GetInt32(6);

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

                    sql = "SELECT * FROM way_nodes WHERE (way_id IN ({0})) ORDER BY sequence_id";
                    string ids_string = this.ConstructIdList(ids, start_idx, stop_idx);
                    if (ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);
                        com = new SqlCommand(sql);
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
                        com = new SqlCommand(sql);
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
            List<long> ids = new List<long>();
            ids.Add(id);
            return this.GetWaysFor(ids);
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
                SqlConnection con = this.CreateConnection();

                List<long> way_ids = new List<long>();
                for (int idx_100 = 0; idx_100 <= ids.Count / 100; idx_100++)
                {
                    // STEP1: Load ways that exist for the given nodes.
                    string sql = "SELECT * FROM way_nodes WHERE (node_id IN ({0})) ";
                    int start_idx = idx_100 * 100;
                    int stop_idx = System.Math.Min((idx_100 + 1) * 100, ids.Count);
                    string ids_string = this.ConstructIdList(ids, start_idx, stop_idx);
                    if (ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);
                        SqlCommand com = new SqlCommand(sql);
                        com.Connection = con;
                        SqlDataReader reader = com.ExecuteReader();

                        while (reader.Read())
                        {
                            long id = reader.GetInt64(0);
                            if (!way_ids.Contains(id))
                            {
                                way_ids.Add(id);
                            }
                        }
                        reader.Close();
                        com.Dispose();
                    }
                }

                return this.GetWays(way_ids);
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
        public override IList<OsmGeo> Get(GeoCoordinateBox box, Filter filter)
        {
            // initialize connection.
            SqlConnection con = this.CreateConnection();
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
            var com = new SqlCommand(sql);
            com.Connection = con;
            SqlDataReader reader = com.ExecuteReader();
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
                bool? visible = reader.IsDBNull(4) ? null : (bool?)(reader.GetValue(4));
                DateTime? timestamp = reader.IsDBNull(5) ? null : (DateTime?)reader.GetDateTime(5);
                long tile = reader.GetInt64(6);
                ulong? version = reader.IsDBNull(7) ? null : (ulong?)(ulong)reader.GetInt32(7);
                string usr = reader.IsDBNull(8) ? null : reader.GetString(8);
                long? usr_id = reader.IsDBNull(9) ? null : (long?)reader.GetInt32(9);

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
                        SqlConnection con = this.CreateConnection();
                        SqlCommand com = new SqlCommand(sql);
                        com.Connection = con;
                        SqlDataReader reader = com.ExecuteReader();
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
        /// Closes this datasource.
        /// </summary>
        public void Close()
        {
            if (_connection != null)
            {
                if (!string.IsNullOrWhiteSpace(_connectionString))
                { // only close connection if it was created here!
                    _connection.Close();
                    _connection.Dispose();
                    _connection = null;
                }
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
