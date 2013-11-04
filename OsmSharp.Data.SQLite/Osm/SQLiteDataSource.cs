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
using System.Data.SQLite;
using System.Linq;
using OsmSharp.Math.Geo;
using OsmSharp.Osm;
using OsmSharp.Osm.Data;
using OsmSharp.Collections.Tags;
using System.Data;

namespace OsmSharp.Data.SQLite.Osm
{
    /// <summary>
    /// An SQLite data source.
    /// </summary>
	public class SQLiteDataSource : DataSourceReadOnlyBase, IDisposable
	{
        /// <summary>
        /// Holds the connection string.
        /// </summary>
		private readonly string _connectionString;

        /// <summary>
        /// The unique id for this datasource.
        /// </summary>
		private readonly Guid _id;

        /// <summary>
        /// Creates a new SQLite simple data source.
        /// </summary>
        /// <param name="connectionString"></param>
        public SQLiteDataSource(string connectionString)
		{
			_connectionString = connectionString;
			_id = Guid.NewGuid();
		}

        /// <summary>
        /// Creates a new SQLite simple data source using an existing connection.
        /// </summary>
        /// <param name="connection"></param>
        public SQLiteDataSource(SQLiteConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Holds the connection.
        /// </summary>
		private SQLiteConnection _connection;

        /// <summary>
        /// Creates/get the connection.
        /// </summary>
        /// <returns></returns>
		private SQLiteConnection CreateConnection()
		{
			if (_connection == null)
			{
				_connection = new SQLiteConnection(_connectionString);
				_connection.Open();
			}
			return _connection;
		}

		#region IDataSourceReadOnly Members

        /// <summary>
        /// Returns the boundingbox of this data if any.
        /// </summary>
		public override GeoCoordinateBox BoundingBox
		{
			get
			{
				throw new NotSupportedException();
			}
		}

        /// <summary>
        /// Returns the name of this data source.
        /// </summary>
		public string Name
		{
			get
			{
				return "SQLite Data Source";
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
        /// Returns a value that indicates if the boundingbox is available or not.
        /// </summary>
		public override bool HasBoundinBox
		{
			get
			{
				return false;
			}
		}

        /// <summary>
        /// Returns all the nodes for the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override IList<Node> GetNodes(IList<long> ids)
		{
			IList<Node> returnList = new List<Node>();
			if (ids.Count > 0)
			{
				// initialize connection.
				SQLiteConnection con = CreateConnection();

				//id	latitude	longitude	changeset_id	visible	timestamp	tile	version
                Dictionary<long, Node> nodes = new Dictionary<long, Node>();
				for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
				{
					int start_idx = idx_1000 * 1000;
					int stop_idx = System.Math.Min((idx_1000 + 1) * 1000, ids.Count);
					string ids_string = ConstructIdList(ids, start_idx, stop_idx);
					if (ids_string.Length > 0)
					{
						string sql = "SELECT node.id, node.latitude, node.longitude, node.changeset_id, node.timestamp, node.version, " +
                                                 "node_tags.key, node_tags.value, node.usr, node.usr_id, node.visible " +
												 "FROM node " +
												 "LEFT JOIN node_tags ON node_tags.node_id = node.id " +
												 "WHERE (node.id IN ({0})) ";
						sql = string.Format(sql, ids_string);

						using (SQLiteCommand com = new SQLiteCommand(sql))
						{
							com.Connection = con;
                            using (SQLiteDataReader reader = ExecuteReader(com))
                            {
                                while (reader.Read())
                                {
                                    // load/parse data.
                                    long returned_id = reader.GetInt64(0);

                                    Node node;
                                    if (!nodes.TryGetValue(returned_id, out node))
                                    {
                                        node = new Node();
                                        node.Id = returned_id;
                                        int latitude_int = reader.GetInt32(1);
                                        int longitude_int = reader.GetInt32(2);
                                        node.ChangeSetId = reader.IsDBNull(3) ? null : (long?)reader.GetInt64(3);
                                        node.TimeStamp = reader.IsDBNull(4) ? null : (DateTime?)this.ConvertDateTime(reader.GetInt64(4));
                                        node.Version = reader.IsDBNull(5) ? null : (ulong?)reader.GetInt64(5);
                                        node.Latitude = latitude_int / 10000000.0;
                                        node.Longitude = longitude_int / 10000000.0;
                                        node.UserName = reader.IsDBNull(8) ? null : reader.GetString(8);
                                        node.UserId = reader.IsDBNull(9) ? null : (long?)reader.GetInt64(9);
                                        node.Visible = reader.IsDBNull(10) ? null : (bool?)reader.GetBoolean(10);

                                        nodes.Add(node.Id.Value, node);
                                    }

                                    if (!reader.IsDBNull(6))
                                    {
                                        if (node.Tags == null)
                                        {
                                            node.Tags = new TagsCollection();
                                        }
                                        string key = reader.GetString(6);
                                        string value = reader.GetString(7);
                                        node.Tags.Add(key, value);
                                    }
                                }
                                reader.Close();
                            }
						}
					}
				}

                // TODO: sort the returnlist??
				returnList = nodes.Values.ToList();
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
                SQLiteConnection con = this.CreateConnection();

                // STEP2: Load ways.
                Dictionary<long, Relation> relations = new Dictionary<long, Relation>();
                string sql;
                SQLiteCommand com;
                SQLiteDataReader reader;
                for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
                {
                    int start_idx = idx_1000 * 1000;
                    int stop_idx = System.Math.Min((idx_1000 + 1) * 1000, ids.Count);

                    sql = "SELECT id, changeset_id, visible, timestamp, version, usr, usr_id FROM relation WHERE (id IN ({0})) ";
                    string ids_string = this.ConstructIdList(ids, start_idx, stop_idx);
                    if (ids_string.Length > 0)
                    {
                        sql = string.Format(sql, ids_string);
                        com = new SQLiteCommand(sql);
                        com.Connection = con;
                        reader = ExecuteReader(com);
                        Relation relation;
                        while (reader.Read())
                        {
                            long id = reader.GetInt64(0);
                            long? changeset_id = reader.IsDBNull(1) ? null : (long?)reader.GetInt64(1);
                            bool? visible = reader.IsDBNull(2) ? null : (bool?)reader.GetBoolean(2);
                            DateTime? timestamp = reader.IsDBNull(3) ? null : (DateTime?)this.ConvertDateTime(reader.GetInt64(3));
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
                        com = new SQLiteCommand(sql);
                        com.Connection = con;
                        reader = ExecuteReader(com);
                        while (reader.Read())
                        {
                            long relation_id = reader.GetInt64(0);
                            long member_type = reader.GetInt64(1);
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
                        com = new SQLiteCommand(sql);
                        com.Connection = con;
                        reader = ExecuteReader(com);
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
            switch(member_type)
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
            SQLiteConnection con = CreateConnection();
            SQLiteCommand com;
            SQLiteDataReader reader;

            string sql = "SELECT relation_id FROM relation_members WHERE (member_id = :member_id and member_type = :member_type) ORDER BY sequence_id";
            com = new SQLiteCommand(sql);
            com.Connection = con;
            com.Parameters.Add(new SQLiteParameter(@"member_type", DbType.Int64));
            com.Parameters.Add(new SQLiteParameter(@"member_id", DbType.Int64));
            com.Parameters[0].Value = this.ConvertMemberType(type).Value;
            com.Parameters[1].Value = id;

            HashSet<long> ids = new HashSet<long>();
            reader = ExecuteReader(com);
            while (reader.Read())
            {
                ids.Add(reader.GetInt64(0));
            }
            reader.Close();

            return this.GetRelations(ids.ToList<long>());
        }

        /// <summary>
        /// Returns the ways for the given ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override IList<Way> GetWays(IList<long> ids)
		{
			if (ids.Count > 0)
			{
				SQLiteConnection con = this.CreateConnection();

				// STEP2: Load ways.
				Dictionary<long, Way> ways = new Dictionary<long, Way>();
				string sql;
				SQLiteCommand com;
				SQLiteDataReader reader;
				for (int idx_1000 = 0; idx_1000 <= ids.Count / 1000; idx_1000++)
				{
					int start_idx = idx_1000 * 1000;
					int stop_idx = System.Math.Min((idx_1000 + 1) * 1000, ids.Count);

					sql = "SELECT id, changeset_id, visible, timestamp, version, usr, usr_id FROM way WHERE (id IN ({0})) ";
					string ids_string = this.ConstructIdList(ids, start_idx, stop_idx);
					if (ids_string.Length > 0)
					{
						sql = string.Format(sql, ids_string);
						com = new SQLiteCommand(sql);
						com.Connection = con;
						reader = ExecuteReader(com);
						Way way;
						while (reader.Read())
						{
							long id = reader.GetInt64(0);
							long? changeset_id = reader.IsDBNull(1) ? null : (long?)reader.GetInt64(1);
							bool? visible = reader.IsDBNull(2) ? null : (bool?)reader.GetBoolean(2);
                            DateTime? timestamp = reader.IsDBNull(3) ? null : (DateTime?)this.ConvertDateTime(reader.GetInt64(3));
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

					sql = "SELECT * FROM way_nodes WHERE (way_id IN ({0})) ORDER BY sequence_id";
					string ids_string = this.ConstructIdList(ids, start_idx, stop_idx);
					if (ids_string.Length > 0)
					{
						sql = string.Format(sql, ids_string);
						com = new SQLiteCommand(sql);
						com.Connection = con;
						reader = ExecuteReader(com);
						while (reader.Read())
						{
							long id = reader.GetInt64(0);
							long node_id = reader.GetInt64(1);
							long sequence_id = reader.GetInt64(2);

                            Way way;
                            if (ways.TryGetValue(id, out way))
                            {
                                if(way.Nodes == null)
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
						com = new SQLiteCommand(sql);
						com.Connection = con;
						reader = ExecuteReader(com);
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
				SQLiteConnection con = CreateConnection();
                SQLiteCommand com;
                SQLiteDataReader reader;

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
                        com = new SQLiteCommand(sql);
                        com.Connection = con;
                        reader = ExecuteReader(com);
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
        /// Converts a given unix time to a DateTime object.
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        private DateTime ConvertDateTime(long unixTime)
        {
            return unixTime.FromUnixTime();
        }

        /// <summary>
        /// Executes a reader.
        /// </summary>
        /// <param name="com"></param>
        /// <returns></returns>
		private static SQLiteDataReader ExecuteReader(SQLiteCommand com)
		{
			return com.ExecuteReader();
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
        /// Returns all objects with the given bounding box and valid for the given filter;
        /// </summary>
        /// <param name="box"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public override IList<OsmGeo> Get(GeoCoordinateBox box, OsmSharp.Osm.Filters.Filter filter)
		{
			// initialize connection.
			SQLiteConnection con = this.CreateConnection();
            List<OsmGeo> res = new List<OsmGeo>();

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
            //string sql
            //        = "SELECT node.id, node.latitude, node.longitude, node.changeset_id, node.timestamp, node.version, " +
            //          "node.usr, node.usr_id, node.visible FROM node WHERE  (tile IN ({4})) AND (visible = 1) AND (latitude BETWEEN {0} AND {1} AND longitude BETWEEN {2} AND {3})";
            // remove this nasty BETWEEN operation because it depends on the database (!) what results are returned (including or excluding bounds!!!)
            string sql
                = "SELECT node.id, node.latitude, node.longitude, node.changeset_id, node.timestamp, node.version, " +
                  "node.usr, node.usr_id, node.visible FROM node WHERE  (tile IN ({4})) AND (visible = 1) AND (latitude >= {0} AND latitude < {1} AND longitude >= {2} AND longitude < {3})";
			    sql = string.Format(sql,
							latitude_min.ToString(System.Globalization.CultureInfo.InvariantCulture),
                            latitude_max.ToString(System.Globalization.CultureInfo.InvariantCulture),
                            longitude_min.ToString(System.Globalization.CultureInfo.InvariantCulture),
                            longitude_max.ToString(System.Globalization.CultureInfo.InvariantCulture),
							this.ConstructIdList(boxes));

			// TODO: parameters.
			var com = new SQLiteCommand(sql);
			com.Connection = con;
			SQLiteDataReader reader = ExecuteReader(com);
			Node node = null;
			var nodes = new Dictionary<long, Node>();
            var nodeIds = new List<long>();
			while (reader.Read())
			{
                node = new Node();
                node.Id = reader.GetInt64(0);
                int latitude_int = reader.GetInt32(1);
                int longitude_int = reader.GetInt32(2);
                node.ChangeSetId = reader.IsDBNull(3) ? null : (long?)reader.GetInt64(3);
                node.TimeStamp = reader.IsDBNull(4) ? null : (DateTime?)this.ConvertDateTime(reader.GetInt64(4));
                node.Version = reader.IsDBNull(5) ? null : (ulong?)reader.GetInt64(5);
                node.Latitude = latitude_int / 10000000.0;
                node.Longitude = longitude_int / 10000000.0;
                node.UserName = reader.IsDBNull(6) ? null : reader.GetString(6);
                node.UserId = reader.IsDBNull(7) ? null : (long?)reader.GetInt64(7);
                node.Visible = reader.IsDBNull(8) ? null : (bool?)reader.GetBoolean(8);

                nodeIds.Add(node.Id.Value);
				nodes.Add(node.Id.Value, node);
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
        /// Constructs a list of ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
		private string ConstructIdList(IList<long> ids)
		{
			return this.ConstructIdList(ids, 0, ids.Count);
		}

        /// <summary>
        /// Constructs a list of ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="startIdx"></param>
        /// <param name="endIdx"></param>
        /// <returns></returns>
		private string ConstructIdList(IList<long> ids, int startIdx, int endIdx)
		{
			string return_string = string.Empty;
			if (ids.Count > 0 && ids.Count > startIdx)
			{
				return_string = return_string + ids[startIdx].ToString();
				for (int i = startIdx + 1; i < endIdx; i++)
				{
					return_string = return_string + "," + ids[i].ToString();
				}
			}
			return return_string;
		}

        /// <summary>
        /// Loads all tags for all given nodes.
        /// </summary>
        /// <param name="nodes"></param>
		private void LoadNodeTags(Dictionary<long, Node> nodes)
		{
			if (nodes.Count > 0)
			{
				for (int idx1000 = 0; idx1000 <= nodes.Count / 1000; idx1000++)
				{
					string sql = "SELECT * FROM node_tags WHERE (node_id IN ({0})) ";
					int startIdx = idx1000 * 1000;
					int stopIdx = System.Math.Min((idx1000 + 1) * 1000, nodes.Count);
					string ids = this.ConstructIdList(nodes.Keys.ToList<long>(), startIdx, stopIdx);
					if (ids.Length > 0)
					{
						sql = string.Format(sql, ids);
						SQLiteConnection con = this.CreateConnection();
						var com = new SQLiteCommand(sql);
						com.Connection = con;
						SQLiteDataReader reader = ExecuteReader(com);
						while (reader.Read())
						{
							long returnedId = reader.GetInt64(0);
							string key = reader.GetString(1);
							object val = reader.GetValue(2);
							string value = string.Empty;
							if (val is string)
							{
								value = val as string;
							}

                            Node node;
                            if (nodes.TryGetValue(returnedId, out node))
                            {
                                if (node.Tags == null)
                                {
                                    node.Tags = new TagsCollection();
                                }
                                node.Tags.Add(key, value);
                            }

						}
						reader.Close();
					}
				}
			}
		}

		#endregion

        /// <summary>
        /// Closes this source.
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
        /// Disposes all resources.
        /// </summary>
		public void Dispose()
		{
			_connection.Close();
			_connection.Dispose();
			_connection = null;
		}

		#endregion

        /// <summary>
        /// Represents a cached node.
        /// </summary>
        class CachedNode
        {
            /// <summary>
            /// Creates a new cached node.
            /// </summary>
            /// <param name="node"></param>
            public CachedNode(Node node)
            {
                Node = node;
            }

            /// <summary>
            /// Gets/sets the node.
            /// </summary>
            public Node Node { get; set; }

            /// <summary>
            /// Gets/sets the ways.
            /// </summary>
            public List<Way> Ways { get; set; }
        }
	}
}
