// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using OsmSharp;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm;
using OsmSharp.Collections.Tags;

namespace OsmSharp.Data.SQLite.Osm.Streams
{
	/// <summary>
	/// Data processor target for SQLite.
	/// </summary>
	public class SQLiteOsmStreamTarget : OsmStreamTarget
	{
        //private const int BatchNodes = 500000;
        //private const int BatchWays = 100000;
        //private const int BatchRelations = 50000;
	    private SQLiteConnection _connection;
		private readonly string _connectionString;
		private SQLiteCommand _insertNodeCmd;
		private SQLiteCommand _insertNodeTagsCmd;
		private SQLiteCommand _insertWayCmd;
		private SQLiteCommand _insertWayTagsCmd;
		private SQLiteCommand _insertWayNodesCmd;
		private SQLiteCommand _insertRelationCmd;
		private SQLiteCommand _insertRelationTagsCmd;
		private SQLiteCommand _insertRelationMembersCmd;
        //private int _nodecount = 0;
        //private int _waycount = 0;
        //private int _relationcount = 0;

        /// <summary>
        /// Creates a new SQLite target.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public SQLiteOsmStreamTarget(SQLiteConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Creates a new SQLite target.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SQLiteOsmStreamTarget(string connectionString)
		{
			_connectionString = connectionString;
		}

        /// <summary>
        /// Initializes this target.
        /// </summary>
		public override void Initialize()
		{
            if (_connection == null)
            {
                _connection = new SQLiteConnection(_connectionString);
            }
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

			using (SQLiteCommand sqliteCmd = _connection.CreateCommand())
			{
				sqliteCmd.CommandText =
                    @"CREATE TABLE IF NOT EXISTS [node] ([id] INTEGER  NOT NULL PRIMARY KEY,[latitude] INTEGER  NULL,[longitude] INTEGER NULL,[changeset_id] INTEGER NULL,[visible] INTEGER NULL,[timestamp] INTEGER NULL,[tile] INTEGER NULL,[version] INTEGER NULL,[usr] varchar(100) NULL,[usr_id] INTEGER NULL); " +
					@"CREATE TABLE IF NOT EXISTS [node_tags] ([node_id] INTEGER  NOT NULL,[key] varchar(100) NOT NULL,[value] varchar(500) NULL, PRIMARY KEY ([node_id],[key])); " +
                    @"CREATE TABLE IF NOT EXISTS [way] ([id] INTEGER  NOT NULL PRIMARY KEY,[changeset_id] INTEGER NULL,[visible] INTEGER NULL,[timestamp] INTEGER NULL,[version] INTEGER NULL,[usr] varchar(100) NULL,[usr_id] INTEGER NULL); " +
					@"CREATE TABLE IF NOT EXISTS [way_tags] ([way_id] INTEGER  NOT NULL,[key] varchar(100) NOT NULL,[value] varchar(500) NULL, PRIMARY KEY ([way_id],[key])); " +
                    @"CREATE TABLE IF NOT EXISTS [way_nodes] ([way_id] INTEGER  NOT NULL,[node_id] INTEGER  NOT NULL,[sequence_id] INTEGER  NOT NULL, PRIMARY KEY ([way_id],[node_id],[sequence_id])); " +
                    @"CREATE TABLE IF NOT EXISTS [relation] ([id] INTEGER  NOT NULL PRIMARY KEY,[changeset_id] INTEGER NULL,[visible] INTEGER NULL,[timestamp] INTEGER NULL,[version] INTEGER NULL,[usr] varchar(100) NULL,[usr_id] INTEGER NULL); " +
					@"CREATE TABLE IF NOT EXISTS [relation_tags] ([relation_id] INTEGER NOT NULL,[key] varchar(100) NOT NULL,[value] varchar(500) NULL, PRIMARY KEY ([relation_id],[key])); " +
                    @"CREATE TABLE IF NOT EXISTS [relation_members] ([relation_id] INTEGER NOT NULL,[member_type]INTEGER NOT NULL,[member_id] INTEGER  NOT NULL,[member_role] varchar(100) NULL,[sequence_id] INTEGER  NOT NULL); " +
					@"CREATE INDEX IF NOT EXISTS [IDX_NODE_TILE] ON [node]([tile]  ASC); " +
                    @"CREATE INDEX IF NOT EXISTS [IDX_WAY_NODES_NODE] ON [way_nodes]([node_id]  ASC); " +
                    @"CREATE INDEX IF NOT EXISTS [IDX_WAY_NODES_WAY_SEQUENCE] ON [way_nodes]([way_id]  ASC,[sequence_id]  ASC); " +
					@"";
				sqliteCmd.ExecuteNonQuery();
			}

			_insertNodeCmd = _connection.CreateCommand();
			_insertNodeCmd.Transaction = _connection.BeginTransaction();
			_insertNodeCmd.CommandText = @"INSERT OR REPLACE INTO node (id,latitude,longitude,changeset_id,visible,timestamp,tile,version,usr,usr_id) VALUES (:id,:latitude,:longitude,:changeset_id,:visible,:timestamp,:tile,:version,:usr,:usr_id);";
			_insertNodeCmd.Parameters.Add(new SQLiteParameter(@"id", DbType.Int64));
			_insertNodeCmd.Parameters.Add(new SQLiteParameter(@"latitude", DbType.Int64));
			_insertNodeCmd.Parameters.Add(new SQLiteParameter(@"longitude", DbType.Int64));
			_insertNodeCmd.Parameters.Add(new SQLiteParameter(@"changeset_id", DbType.Int64));
			_insertNodeCmd.Parameters.Add(new SQLiteParameter(@"visible", DbType.Int64));
            _insertNodeCmd.Parameters.Add(new SQLiteParameter(@"timestamp", DbType.Int64)); // date stored as Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
			_insertNodeCmd.Parameters.Add(new SQLiteParameter(@"tile", DbType.Int64));
			_insertNodeCmd.Parameters.Add(new SQLiteParameter(@"version", DbType.Int64));
			_insertNodeCmd.Parameters.Add(new SQLiteParameter(@"usr", DbType.String));
			_insertNodeCmd.Parameters.Add(new SQLiteParameter(@"usr_id", DbType.Int64));

			_insertNodeTagsCmd = _connection.CreateCommand();
			_insertNodeTagsCmd.Transaction = _insertNodeCmd.Transaction;
			_insertNodeTagsCmd.CommandText = @"INSERT OR REPLACE INTO node_tags (node_id,key,value) VALUES (:node_id,:key,:value);";
			_insertNodeTagsCmd.Parameters.Add(new SQLiteParameter(@"node_id", DbType.Int64));
			_insertNodeTagsCmd.Parameters.Add(new SQLiteParameter(@"key", DbType.String));
			_insertNodeTagsCmd.Parameters.Add(new SQLiteParameter(@"value", DbType.String));

			_insertWayCmd = _connection.CreateCommand();
			_insertWayCmd.Transaction = _connection.BeginTransaction();
			_insertWayCmd.CommandText = @"INSERT OR REPLACE INTO way (id,changeset_id,visible,timestamp,version,usr,usr_id) VALUES (:id,:changeset_id,:visible,:timestamp,:version,:usr,:usr_id);";
			_insertWayCmd.Parameters.Add(new SQLiteParameter(@"id", DbType.Int64));
			_insertWayCmd.Parameters.Add(new SQLiteParameter(@"changeset_id", DbType.Int64));
			_insertWayCmd.Parameters.Add(new SQLiteParameter(@"visible", DbType.Int64));
            _insertWayCmd.Parameters.Add(new SQLiteParameter(@"timestamp", DbType.Int64)); // date stored as Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
			_insertWayCmd.Parameters.Add(new SQLiteParameter(@"version", DbType.Int64));
			_insertWayCmd.Parameters.Add(new SQLiteParameter(@"usr", DbType.String));
			_insertWayCmd.Parameters.Add(new SQLiteParameter(@"usr_id", DbType.Int64));

			_insertWayTagsCmd = _connection.CreateCommand();
			_insertWayTagsCmd.Transaction = _insertWayCmd.Transaction;
			_insertWayTagsCmd.CommandText = @"INSERT OR REPLACE INTO way_tags (way_id,key,value) VALUES (:way_id,:key,:value);";
			_insertWayTagsCmd.Parameters.Add(new SQLiteParameter(@"way_id", DbType.Int64));
			_insertWayTagsCmd.Parameters.Add(new SQLiteParameter(@"key", DbType.String));
			_insertWayTagsCmd.Parameters.Add(new SQLiteParameter(@"value", DbType.String));

			_insertWayNodesCmd = _connection.CreateCommand();
			_insertWayNodesCmd.Transaction = _insertWayCmd.Transaction;
			_insertWayNodesCmd.CommandText = @"INSERT OR REPLACE INTO way_nodes (way_id,node_id,sequence_id) VALUES (:way_id,:node_id,:sequence_id);";
			_insertWayNodesCmd.Parameters.Add(new SQLiteParameter(@"way_id", DbType.Int64));
			_insertWayNodesCmd.Parameters.Add(new SQLiteParameter(@"node_id", DbType.Int64));
			_insertWayNodesCmd.Parameters.Add(new SQLiteParameter(@"sequence_id", DbType.Int64));

			_insertRelationCmd = _connection.CreateCommand();
			_insertRelationCmd.Transaction = _connection.BeginTransaction();
			_insertRelationCmd.CommandText = @"INSERT OR REPLACE INTO relation (id,changeset_id,visible,timestamp,version,usr,usr_id) VALUES (:id,:changeset_id,:visible,:timestamp,:version,:usr,:usr_id);";
			_insertRelationCmd.Parameters.Add(new SQLiteParameter(@"id", DbType.Int64));
			_insertRelationCmd.Parameters.Add(new SQLiteParameter(@"changeset_id", DbType.Int64));
			_insertRelationCmd.Parameters.Add(new SQLiteParameter(@"visible", DbType.Int64));
            _insertRelationCmd.Parameters.Add(new SQLiteParameter(@"timestamp", DbType.Int64)); // date stored as Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
			_insertRelationCmd.Parameters.Add(new SQLiteParameter(@"version", DbType.Int64));
			_insertRelationCmd.Parameters.Add(new SQLiteParameter(@"usr", DbType.String));
			_insertRelationCmd.Parameters.Add(new SQLiteParameter(@"usr_id", DbType.Int64));

			_insertRelationTagsCmd = _connection.CreateCommand();
			_insertRelationTagsCmd.Transaction = _insertRelationCmd.Transaction;
			_insertRelationTagsCmd.CommandText = @"INSERT OR REPLACE INTO relation_tags (relation_id,key,value) VALUES (:relation_id,:key,:value);";
			_insertRelationTagsCmd.Parameters.Add(new SQLiteParameter(@"relation_id", DbType.Int64));
			_insertRelationTagsCmd.Parameters.Add(new SQLiteParameter(@"key", DbType.String));
			_insertRelationTagsCmd.Parameters.Add(new SQLiteParameter(@"value", DbType.String));

			_insertRelationMembersCmd = _connection.CreateCommand();
			_insertRelationMembersCmd.Transaction = _insertRelationCmd.Transaction;
			_insertRelationMembersCmd.CommandText = @"INSERT OR REPLACE INTO relation_members (relation_id,member_type,member_id,member_role,sequence_id) VALUES (:relation_id,:member_type,:member_id,:member_role,:sequence_id);";
			_insertRelationMembersCmd.Parameters.Add(new SQLiteParameter(@"relation_id", DbType.Int64));
            _insertRelationMembersCmd.Parameters.Add(new SQLiteParameter(@"member_type", DbType.Int64));
			_insertRelationMembersCmd.Parameters.Add(new SQLiteParameter(@"member_id", DbType.Int64));
            _insertRelationMembersCmd.Parameters.Add(new SQLiteParameter(@"member_role", DbType.String));
			_insertRelationMembersCmd.Parameters.Add(new SQLiteParameter(@"sequence_id", DbType.Int64));
		}

	    /// <summary>
	    /// Adds a node to this target.
	    /// </summary>
	    /// <param name="node"></param>
	    public override void AddNode(Node node)
	    {
	        if (!node.Latitude.HasValue || !node.Longitude.HasValue)
	        {
	            // cannot insert nodes without lat/lon.
	            throw new ArgumentOutOfRangeException("node", "Cannot insert nodes without lat/lon.");
	        }

            // insert the node.
	        _insertNodeCmd.Parameters[0].Value = node.Id;
	        _insertNodeCmd.Parameters[1].Value =
	            (node.Latitude.HasValue ? (int) (node.Latitude.GetValueOrDefault()*10000000.0) : (int?) null)
	                .ConvertToDBValue();
	        _insertNodeCmd.Parameters[2].Value =
                (node.Longitude.HasValue ? (int)(node.Longitude.GetValueOrDefault() * 10000000.0) : (int?)null)
	                .ConvertToDBValue();
	        _insertNodeCmd.Parameters[3].Value = node.ChangeSetId.ConvertToDBValue();
            _insertNodeCmd.Parameters[4].Value = node.Visible.ConvertToDBValue();
	        _insertNodeCmd.Parameters[5].Value = this.ConvertDateTime(node.TimeStamp);
	        _insertNodeCmd.Parameters[6].Value = TileCalculations.xy2tile((uint) TileCalculations.lon2x(node.Longitude.Value),
	                                                                      (uint) TileCalculations.lat2y(node.Latitude.Value));
	        _insertNodeCmd.Parameters[7].Value = node.Version.ConvertToDBValue();
	        _insertNodeCmd.Parameters[8].Value = node.UserName;
	        _insertNodeCmd.Parameters[9].Value = node.UserId.ConvertToDBValue();
	        _insertNodeCmd.ExecuteNonQuery();

            // insert the tags.
	        if (node.Tags != null)
	        {
	            foreach (Tag keyValuePair in node.Tags)
	            {
	                _insertNodeTagsCmd.Parameters[0].Value = node.Id;
	                _insertNodeTagsCmd.Parameters[1].Value = keyValuePair.Key;
	                _insertNodeTagsCmd.Parameters[2].Value = keyValuePair.Value;
	                _insertNodeTagsCmd.ExecuteNonQuery();
	            }
	        }

            //// commit nodes in batch.
            //_nodecount++;
            //if (_nodecount < BatchNodes)
            //    return;

            //_insertNodeCmd.Transaction.Commit();
            //_insertNodeTagsCmd.Transaction = 
            //    _insertNodeCmd.Transaction = _connection.BeginTransaction();
	    }

	    /// <summary>
        /// Adds a way to this target.
        /// </summary>
        /// <param name="way"></param>
		public override void AddWay(Way way)
		{
			long? id = way.Id;
			bool? visible = way.Visible;
			_insertWayCmd.Parameters[0].Value = id.ConvertToDBValue();
			_insertWayCmd.Parameters[1].Value = way.ChangeSetId.ConvertToDBValue();
            _insertWayCmd.Parameters[2].Value = visible.ConvertToDBValue();
			_insertWayCmd.Parameters[3].Value = this.ConvertDateTime(way.TimeStamp);
			_insertWayCmd.Parameters[4].Value = way.Version.ConvertToDBValue();
			_insertWayCmd.Parameters[5].Value = way.UserName;
			_insertWayCmd.Parameters[6].Value = way.UserId.ConvertToDBValue();
			_insertWayCmd.ExecuteNonQuery();
			if (way.Tags != null)
			{
				foreach (Tag keyValuePair in way.Tags)
				{
					var key = keyValuePair.Key;
					if (!string.IsNullOrEmpty(key))
					{
						_insertWayTagsCmd.Parameters[0].Value = id;
						_insertWayTagsCmd.Parameters[1].Value = key;
						_insertWayTagsCmd.Parameters[2].Value = keyValuePair.Value;
						_insertWayTagsCmd.ExecuteNonQuery();
					}
				}
			}
			if (way.Nodes != null)
			{
				for (int index = 0; index < way.Nodes.Count; index++)
				{
					_insertWayNodesCmd.Parameters[0].Value = id;
					_insertWayNodesCmd.Parameters[1].Value = way.Nodes[index];
					_insertWayNodesCmd.Parameters[2].Value = index;
					_insertWayNodesCmd.ExecuteNonQuery();
				}
			}
            //_waycount++;
            //if (_waycount < BatchWays)
            //    return;

            //_insertWayCmd.Transaction.Commit();
            //_insertWayTagsCmd.Transaction = _insertWayNodesCmd.Transaction = _insertWayCmd.Transaction = _connection.BeginTransaction();
		}

        /// <summary>
        /// Adds a relation to this target.
        /// </summary>
        /// <param name="relation"></param>
		public override void AddRelation(Relation relation)
		{
			long? id = relation.Id;
			bool? visible = relation.Visible;
			_insertRelationCmd.Parameters[0].Value = id.ConvertToDBValue();
			_insertRelationCmd.Parameters[1].Value = relation.ChangeSetId.ConvertToDBValue();
            _insertRelationCmd.Parameters[2].Value = visible.ConvertToDBValue();
            _insertRelationCmd.Parameters[3].Value = this.ConvertDateTime(relation.TimeStamp);
			_insertRelationCmd.Parameters[4].Value = relation.Version.ConvertToDBValue();
			_insertRelationCmd.Parameters[5].Value = relation.UserName;
			_insertRelationCmd.Parameters[6].Value = relation.UserId.ConvertToDBValue();
			_insertRelationCmd.ExecuteNonQuery();
			if (relation.Tags != null)
			{
				foreach (Tag keyValuePair in relation.Tags)
				{
					_insertRelationTagsCmd.Parameters[0].Value = id;
					_insertRelationTagsCmd.Parameters[1].Value = keyValuePair.Key;
					_insertRelationTagsCmd.Parameters[2].Value = keyValuePair.Value;
					_insertRelationTagsCmd.ExecuteNonQuery();
				}
			}
			if (relation.Members != null)
			{
				for (int index = 0; index < relation.Members.Count; ++index)
				{
					RelationMember simpleRelationMember = relation.Members[index];
					_insertRelationMembersCmd.Parameters[0].Value = id;
					_insertRelationMembersCmd.Parameters[1].Value = this.ConvertMemberType(simpleRelationMember.MemberType);
					_insertRelationMembersCmd.Parameters[2].Value = simpleRelationMember.MemberId;
					_insertRelationMembersCmd.Parameters[3].Value = simpleRelationMember.MemberRole;
					_insertRelationMembersCmd.Parameters[4].Value = index;
					_insertRelationMembersCmd.ExecuteNonQuery();
				}
			}
            //_relationcount++;

            //_insertRelationCmd.Transaction.Commit();
            //_insertRelationTagsCmd.Transaction = _insertRelationMembersCmd.Transaction = _insertRelationCmd.Transaction = _connection.BeginTransaction();
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
        /// Converts the given datetime object to Unix Time, the number of seconds since 1970-01-01 00:00:00 UTC.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private object ConvertDateTime(DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                return dateTime.Value.ToUnixTime();
            }
            return null;
        }

        /// <summary>
        /// Closes this target.
        /// </summary>
		public override void Close()
		{
			if (_connection != null)
			{
                //_insertNodeCmd.Transaction.Commit();
                //_insertWayCmd.Transaction.Commit();
                //_insertRelationCmd.Transaction.Commit();
                if (!string.IsNullOrWhiteSpace(_connectionString))
                { // the connection was created here, it needs to be destroyed here.
                    _connection.Close();
                    _connection.Dispose();
                }
			}
			_connection = (SQLiteConnection)null;
		}
	}

    /// <summary>
    /// Common database extensions.
    /// </summary>
	public static class Extensions
	{
        /// <summary>
        /// Converts the given value into a proper db value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nullable"></param>
        /// <returns></returns>
		public static object ConvertToDBValue<T>(this T? nullable) where T : struct
		{
			return nullable.HasValue ? (object) nullable.Value : DBNull.Value;
		}
	}
}
