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

using System.Data.SQLite;
using OsmSharp.Osm.Data.SQLite.Raw.Processor;
using OsmSharp.Osm.Simple;
using OsmSharp.Osm.Data.Core.Processor;
using System;
using System.Collections.Generic;
using System.Data;
using OsmSharp.Tools;

namespace OsmSharp.Osm.Data.SQLite.SimpleSchema.Processor
{
	/// <summary>
	/// Data processor target for SQLite.
	/// </summary>
	public class SQLiteDataProcessorTarget : DataProcessorTarget
	{
	    private const int BatchNodes = 500000;
	    private const int BatchWays = 100000;
	    private const int BatchRelations = 50000;
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
		private int _nodecount = 0;
		private int _waycount = 0;
		private int _relationcount = 0;

        /// <summary>
        /// Creates a new SQLite target.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
		public SQLiteDataProcessorTarget(string connectionString)
		{
			_connectionString = connectionString;
		}

        /// <summary>
        /// Initializes this target.
        /// </summary>
		public override void Initialize()
		{
			_connection = new SQLiteConnection(_connectionString);
			_connection.Open();

			using (SQLiteCommand sqliteCmd = _connection.CreateCommand())
			{
				sqliteCmd.CommandText =
					@"CREATE TABLE IF NOT EXISTS [node] ([id] INTEGER  NOT NULL PRIMARY KEY,[latitude] INTEGER  NULL,[longitude] INTEGER NULL,[changeset_id] INTEGER NULL,[visible] INTEGER NULL,[timestamp] datetime NULL,[tile] INTEGER NULL,[version] INTEGER NULL,[usr] varchar(100) NULL,[usr_id] INTEGER NULL); " +
					@"CREATE TABLE IF NOT EXISTS [node_tags] ([node_id] INTEGER  NOT NULL,[key] varchar(100) NOT NULL,[value] varchar(500) NULL, PRIMARY KEY ([node_id],[key])); " +
					@"CREATE TABLE IF NOT EXISTS [way] ([id] INTEGER  NOT NULL PRIMARY KEY,[changeset_id] INTEGER NULL,[visible] INTEGER NULL,[timestamp] datetime NULL,[version] INTEGER NULL,[usr] varchar(100) NULL,[usr_id] INTEGER NULL); " +
					@"CREATE TABLE IF NOT EXISTS [way_tags] ([way_id] INTEGER  NOT NULL,[key] varchar(100) NOT NULL,[value] varchar(500) NULL, PRIMARY KEY ([way_id],[key])); " +
					@"CREATE TABLE IF NOT EXISTS [way_nodes] ([way_id] INTEGER  NOT NULL,[node_id] INTEGER  NOT NULL,[sequence_id] INTEGER  NOT NULL, PRIMARY KEY ([way_id],[node_id])); " +
					@"CREATE TABLE IF NOT EXISTS [relation] ([id] INTEGER  NOT NULL PRIMARY KEY,[changeset_id] INTEGER NULL,[visible] INTEGER NULL,[timestamp] datetime NULL,[version] INTEGER NULL,[usr] varchar(100) NULL,[usr_id] INTEGER NULL); " +
					@"CREATE TABLE IF NOT EXISTS [relation_tags] ([relation_id] INTEGER NOT NULL,[key] varchar(100) NOT NULL,[value] varchar(500) NULL, PRIMARY KEY ([relation_id],[key])); " +
					@"CREATE TABLE IF NOT EXISTS [relation_members] ([relation_id] INTEGER NOT NULL,[member_type] varchar(100) NULL,[member_id] INTEGER  NOT NULL,[member_role] varchar(100) NULL,[sequence_id] INTEGER  NOT NULL, PRIMARY KEY ([relation_id],[member_id])); " +
					@"CREATE INDEX IF NOT EXISTS [IDX_NODE_TILE] ON [node]([tile]  ASC); " +
					@"CREATE INDEX [IDX_WAY_NODES_NODE] ON [way_nodes]([node_id]  ASC); " +
					@"CREATE INDEX [IDX_WAY_NODES_WAY_SEQUENCE] ON [way_nodes]([way_id]  ASC,[sequence_id]  ASC); " +
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
			_insertNodeCmd.Parameters.Add(new SQLiteParameter(@"timestamp", DbType.DateTime));
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
			_insertWayCmd.Parameters.Add(new SQLiteParameter(@"timestamp", DbType.DateTime));
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
			_insertRelationCmd.Parameters.Add(new SQLiteParameter(@"timestamp", DbType.DateTime));
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
			_insertRelationMembersCmd.Parameters.Add(new SQLiteParameter(@"member_type", DbType.String));
			_insertRelationMembersCmd.Parameters.Add(new SQLiteParameter(@"member_id", DbType.Int64));
			_insertRelationMembersCmd.Parameters.Add(new SQLiteParameter(@"member_role", DbType.String));
			_insertRelationMembersCmd.Parameters.Add(new SQLiteParameter(@"sequence_id", DbType.Int64));
		}

        /// <summary>
        /// Applies changesets to this target.
        /// </summary>
        /// <param name="change">The change to apply.</param>[Obsolete("This class has become obsolete use OsmSharp.Routing.Graph.DynamicGraphRouterDataSource instead!")]
    
		public override void ApplyChange(SimpleChangeSet change)
		{
			throw new NotSupportedException();
		}

	    /// <summary>
	    /// Adds a node to this target.
	    /// </summary>
	    /// <param name="node"></param>
	    public override void AddNode(SimpleNode node)
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
	            (node.Latitude.HasValue ? (int) (node.Latitude.GetValueOrDefault()*10000000.0) : (int?) null)
	                .ConvertToDBValue();
	        _insertNodeCmd.Parameters[3].Value = node.ChangeSetId.ConvertToDBValue();
	        _insertNodeCmd.Parameters[4].Value = node.Visible.HasValue && node.Visible.Value ? 1 : 0;
	        _insertNodeCmd.Parameters[5].Value = node.TimeStamp.ConvertToDBValue();
	        _insertNodeCmd.Parameters[6].Value = TileCalculations.xy2tile((uint) TileCalculations.lon2x(node.Longitude.Value),
	                                                                      (uint) TileCalculations.lat2y(node.Latitude.Value));
	        _insertNodeCmd.Parameters[7].Value = node.Version.ConvertToDBValue();
	        _insertNodeCmd.Parameters[8].Value = node.UserName.ToStringEmptyWhenNull();
	        _insertNodeCmd.Parameters[9].Value = node.UserId.ConvertToDBValue();
	        _insertNodeCmd.ExecuteNonQuery();

            // insert the tags.
	        if (node.Tags != null)
	        {
	            foreach (KeyValuePair<string, string> keyValuePair in node.Tags)
	            {
	                _insertNodeTagsCmd.Parameters[0].Value = node.Id;
	                _insertNodeTagsCmd.Parameters[1].Value = keyValuePair.Key;
	                _insertNodeTagsCmd.Parameters[2].Value = keyValuePair.Value;
	                _insertNodeTagsCmd.ExecuteNonQuery();
	            }
	        }

            // commit nodes in batch.
	        _nodecount++;
	        if (_nodecount < BatchNodes)
	            return;

	        _insertNodeCmd.Transaction.Commit();
	        _insertNodeTagsCmd.Transaction = 
                _insertNodeCmd.Transaction = _connection.BeginTransaction();
	    }

	    /// <summary>
        /// Adds a way to this target.
        /// </summary>
        /// <param name="way"></param>
		public override void AddWay(SimpleWay way)
		{
			long? id = way.Id;
			bool? visible = way.Visible;
			_insertWayCmd.Parameters[0].Value = id.ConvertToDBValue();
			_insertWayCmd.Parameters[1].Value = way.ChangeSetId.ConvertToDBValue();
			_insertWayCmd.Parameters[2].Value = visible.HasValue && visible.Value ? 1 : 0;
			_insertWayCmd.Parameters[3].Value = way.TimeStamp.ConvertToDBValue();
			_insertWayCmd.Parameters[4].Value = way.Version.ConvertToDBValue();
			_insertWayCmd.Parameters[5].Value = way.UserName.ToStringEmptyWhenNull();
			_insertWayCmd.Parameters[6].Value = way.UserId.ConvertToDBValue();
			_insertWayCmd.ExecuteNonQuery();
			if (way.Tags != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in way.Tags)
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
				for (int index = 0; index < way.Nodes.Count; ++index)
				{
					_insertWayNodesCmd.Parameters[0].Value = id;
					_insertWayNodesCmd.Parameters[1].Value = way.Nodes[index];
					_insertWayNodesCmd.Parameters[2].Value = index;
					_insertWayNodesCmd.ExecuteNonQuery();
				}
			}
			_waycount++;
			if (_waycount < BatchWays)
				return;

			_insertWayCmd.Transaction.Commit();
			_insertWayTagsCmd.Transaction = _insertWayNodesCmd.Transaction = _insertWayCmd.Transaction = _connection.BeginTransaction();
		}

        /// <summary>
        /// Adds a relation to this target.
        /// </summary>
        /// <param name="relation"></param>
		public override void AddRelation(SimpleRelation relation)
		{
			long? id = relation.Id;
			bool? visible = relation.Visible;
			_insertRelationCmd.Parameters[0].Value = id.ConvertToDBValue();
			_insertRelationCmd.Parameters[1].Value = relation.ChangeSetId.ConvertToDBValue();
			_insertRelationCmd.Parameters[2].Value = visible.HasValue && visible.Value ? 1 : 0;
			_insertRelationCmd.Parameters[3].Value = relation.TimeStamp.ConvertToDBValue();
			_insertRelationCmd.Parameters[4].Value = relation.Version.ConvertToDBValue();
			_insertRelationCmd.Parameters[5].Value = relation.UserName.ToStringEmptyWhenNull();
			_insertRelationCmd.Parameters[6].Value = relation.UserId.ConvertToDBValue();
			_insertRelationCmd.ExecuteNonQuery();
			if (relation.Tags != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in relation.Tags)
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
					SimpleRelationMember simpleRelationMember = relation.Members[index];
					_insertRelationMembersCmd.Parameters[0].Value = id;
					_insertRelationMembersCmd.Parameters[1].Value = simpleRelationMember.MemberType;
					_insertRelationMembersCmd.Parameters[2].Value = simpleRelationMember.MemberId;
					_insertRelationMembersCmd.Parameters[3].Value = simpleRelationMember.MemberRole;
					_insertRelationMembersCmd.Parameters[4].Value = index;
					_insertRelationMembersCmd.ExecuteNonQuery();
				}
			}
			_relationcount++;
			if (_relationcount < BatchRelations)
				return;
			_insertRelationCmd.Transaction.Commit();
			_insertRelationTagsCmd.Transaction = _insertRelationMembersCmd.Transaction = _insertRelationCmd.Transaction = _connection.BeginTransaction();
		}

        /// <summary>
        /// Closes this target.
        /// </summary>
		public override void Close()
		{
			if (_connection != null)
			{
				_insertNodeCmd.Transaction.Commit();
				_insertWayCmd.Transaction.Commit();
				_insertRelationCmd.Transaction.Commit();
				_connection.Close();
				_connection.Dispose();
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
