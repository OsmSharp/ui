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
using System.Data;
using System.Data.SqlClient;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm;
using OsmSharp.Collections.Tags;
using OsmSharp.Data.SQLServer.Osm.SchemaTools;
using OsmSharp.Osm.Tiles;

namespace OsmSharp.Data.SQLServer.Osm.Streams
{
    /// <summary>
    /// A data processor target for the SqlServer simple schema.
    /// </summary>
    public class SQLServerOsmStreamTarget : OsmStreamTarget{
        
        /// <summary>
        /// Holds the connection.
        /// </summary>
        private SqlConnection _connection;

        private DataTable _nodeTable;
        private DataTable _nodeTagsTable;
        private DataTable _wayTable;
        private DataTable _wayTagsTable;
        private DataTable _wayNodesTable;
        private DataTable _relationTable;
        private DataTable _relationMembersTable;
        private DataTable _relationTagsTable;

        /// <summary>
        /// Flag that indicates if the schema needs to be created if not present.
        /// </summary>
        private readonly bool _createAndDetectSchema;

        private const int BatchNodes     = 100000;
        private const int BatchWays      = 10000;
        private const int BatchRelations = 100000;

        //int _batch_nodes = 1;
        //int _batch_ways = 1;
        //int _batch_relations = 1;

        private readonly string _connectionString;

        /// <summary>
        /// Create a SQLServerSimpleSchemaDataProcessorTarget. Schema will not be created.
        /// </summary>
        /// <param name="connectionString">Connection string to the database</param>
        public SQLServerOsmStreamTarget(string connectionString)
        {
            _connectionString = connectionString;
            _createAndDetectSchema = false;
        }

        /// <summary>
        /// Create a SQLServerSimpleSchemaDataProcessorTarget
        /// </summary>
        /// <param name="connectionString">Connection string to the database</param>
        /// <param name="createSchema">If true, will drop and re-create the schema</param>
        public SQLServerOsmStreamTarget(string connectionString, bool createSchema)
        {
            _connectionString = connectionString;
            _createAndDetectSchema = createSchema;
        }

        /// <summary>
        /// Create a SQLServerSimpleSchemaDataProcessorTarget
        /// </summary>
        /// <param name="connection">Connection to the database</param>
        public SQLServerOsmStreamTarget(SqlConnection connection)
        {
            _connection = connection;
            _createAndDetectSchema = false;
        }

        /// <summary>
        /// Create a SQLServerSimpleSchemaDataProcessorTarget
        /// </summary>
        /// <param name="connection">Connection to the database</param>
        /// <param name="createSchema">If true, will drop and re-create the schema</param>
        public SQLServerOsmStreamTarget(SqlConnection connection, bool createSchema)
        {
            _connection = connection;
            _createAndDetectSchema = createSchema;
        }

        /// <summary>
        /// Initializes the target.
        /// </summary>
        public override void Initialize()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }

            if (_createAndDetectSchema)
            { 
                // creates or detects the tables.
                SQLServerSchemaTools.CreateAndDetect(_connection);
            }

            CreateNodeTables();
            CreateWayTables();
            CreateRelationTables();
        }

        #region HelperFunctions

        private void CreateNodeTables()
        {
            // create node bulk objects.
            _nodeTable = new DataTable();
            _nodeTable.Columns.Add(new DataColumn("id", typeof (long)));
            _nodeTable.Columns.Add(new DataColumn("latitude", typeof(int)));
            _nodeTable.Columns.Add(new DataColumn("longitude", typeof(int)));
            _nodeTable.Columns.Add(new DataColumn("changeset_id", typeof(long)));
            _nodeTable.Columns.Add(new DataColumn("visible", typeof(bool)));
            _nodeTable.Columns.Add(new DataColumn("timestamp", typeof(DateTime)));
            _nodeTable.Columns.Add(new DataColumn("tile", typeof(long)));
            _nodeTable.Columns.Add(new DataColumn("version", typeof(int)));
            _nodeTable.Columns.Add(new DataColumn("usr", typeof(string)));
            _nodeTable.Columns.Add(new DataColumn("usr_id", typeof(int)));

            // create node_tags bulk objects.
            _nodeTagsTable = new DataTable();
            _nodeTagsTable.Columns.Add(new DataColumn("node_id", typeof (long)));
            _nodeTagsTable.Columns.Add(new DataColumn("key", typeof (string)));
            _nodeTagsTable.Columns.Add(new DataColumn("value", typeof(string)));
        }

        private void CreateWayTables()
        {
            // create way bulk objects.
            _wayTable = new DataTable();
            _wayTable.Columns.Add(new DataColumn("id", typeof (long)));
            _wayTable.Columns.Add(new DataColumn("changeset_id", typeof(long)));
            _wayTable.Columns.Add(new DataColumn("timestamp", typeof(DateTime)));
            _wayTable.Columns.Add(new DataColumn("visible", typeof(bool)));
            _wayTable.Columns.Add(new DataColumn("version", typeof(int)));
            _wayTable.Columns.Add(new DataColumn("usr", typeof(string)));
            _wayTable.Columns.Add(new DataColumn("usr_id", typeof(int)));

            // create way_tags bulk objects.
            _wayTagsTable = new DataTable();
            _wayTagsTable.Columns.Add(new DataColumn("way_id", typeof (long)));
            _wayTagsTable.Columns.Add(new DataColumn("key", typeof (string)));
            _wayTagsTable.Columns.Add(new DataColumn("value", typeof(string)));

            // create way_nodes bulk objects.
            _wayNodesTable = new DataTable();
            _wayNodesTable.Columns.Add(new DataColumn("way_id", typeof (long)));
            _wayNodesTable.Columns.Add(new DataColumn("node_id", typeof (long)));
            _wayNodesTable.Columns.Add(new DataColumn("sequence_id", typeof (int)));
        }

        private void CreateRelationTables()
        {
            // create relation bulk objects.
            _relationTable = new DataTable();
            _relationTable.Columns.Add(new DataColumn("id", typeof (long)));
            _relationTable.Columns.Add(new DataColumn("changeset_id", typeof(long)));
            _relationTable.Columns.Add(new DataColumn("timestamp", typeof(DateTime)));
            _relationTable.Columns.Add(new DataColumn("visible", typeof(bool)));
            _relationTable.Columns.Add(new DataColumn("version", typeof(int)));
            _relationTable.Columns.Add(new DataColumn("usr", typeof(string)));
            _relationTable.Columns.Add(new DataColumn("usr_id", typeof(int)));

            // create relation_tags bulk objects.
            _relationTagsTable = new DataTable();
            _relationTagsTable.Columns.Add(new DataColumn("relation_id", typeof (long)));
            _relationTagsTable.Columns.Add(new DataColumn("key", typeof(string)));
            _relationTagsTable.Columns.Add(new DataColumn("value", typeof(string)));

            // create relation_members bulk objects.
            _relationMembersTable = new DataTable();
            _relationMembersTable.Columns.Add(new DataColumn("relation_id", typeof (long)));
            _relationMembersTable.Columns.Add(new DataColumn("member_type", typeof(short)));
            _relationMembersTable.Columns.Add(new DataColumn("member_id", typeof(long)));
            _relationMembersTable.Columns.Add(new DataColumn("member_role", typeof(string)));
            _relationMembersTable.Columns.Add(new DataColumn("sequence_id", typeof (int)));
        }

        /// <summary>
        /// Does the actual bulk copy.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableName"></param>
        private void BulkCopy(DataTable table, string tableName)
        {
            BulkCopy(table, tableName, table.Rows.Count + 1);
        }

        /// <summary>
        /// Does the actual bulk inserts.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="tableName"></param>
        /// <param name="batchSize"></param>
        private void BulkCopy(DataTable table, string tableName, int batchSize)
        {
            if (table == null || table.Rows.Count < 1)
                return;

            using (var bulkCopy = new SqlBulkCopy(_connection))
            {
                bulkCopy.BulkCopyTimeout = 1200; // 20 minutes. Use a long time as the database can expand, or can be busy
                bulkCopy.DestinationTableName = tableName;
                bulkCopy.BatchSize = batchSize;

                try
                {
                    OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.SQLServer.Osm.Streams.SQLServerOsmStreamTarget", OsmSharp.Logging.TraceEventType.Information,
                        "Inserting {0} records into {1}.", table.Rows.Count, tableName);
                    bulkCopy.WriteToServer(table);
                }
                catch (Exception e)
                {
                    OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.SQLServer.Osm.Streams.SQLServerOsmStreamTarget", OsmSharp.Logging.TraceEventType.Error,
                        e.ToString());
                }
            }
        }

        #endregion

        /// <summary>
        /// Adds a node to the target.
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(Node node)
        {
            if (!node.Id.HasValue || !node.Latitude.HasValue || !node.Longitude.HasValue)
                return;

            DataRow nodeRow = _nodeTable.NewRow();
            // format data and create parameters.
            nodeRow["id"] = node.Id.Value;
            var latitude = (int)(node.Latitude.Value * 10000000); // latitude should always contain a value.
            nodeRow["latitude"] = latitude;
            var longitude = (int)(node.Longitude.Value * 10000000); // longitude should always containt a value.
            nodeRow["longitude"] = longitude;
            nodeRow["changeset_id"] = node.ChangeSetId.ConvertToDBValue();
            nodeRow["visible"] = node.Visible.HasValue ? (object)(node.Visible.Value ? 1 : 0) : DBNull.Value;
            nodeRow["timestamp"] = node.TimeStamp.ConvertToDBValue();
            nodeRow["version"] = node.Version.ConvertToDBValue();
            nodeRow["tile"] = Tile.CreateAroundLocation(node.Latitude.Value, node.Longitude.Value, TileDefaultsForRouting.Zoom).Id;
            nodeRow["usr"] = node.UserName == null ? DBNull.Value : (object)node.UserName.Truncate(SQLServerSchemaConstants.RelationUsr);
            nodeRow["usr_id"] = node.UserId.ConvertToDBValue();
            _nodeTable.Rows.Add(nodeRow);

            // tags.
            if (node.Tags != null)
            {
                foreach (Tag tag in node.Tags)
                {
                    DataRow tagRow = _nodeTagsTable.NewRow();
                    tagRow["key"] = tag.Key.Trim().Truncate(SQLServerSchemaConstants.NodeTagsKey);
                    tagRow["node_id"] = node.Id.Value;
                    tagRow["value"] = tag.Value.Trim().Truncate(SQLServerSchemaConstants.NodeTagsValue);

                    _nodeTagsTable.Rows.Add(tagRow);
                }
            }

            // bulk insert if needed.
            if (_nodeTable.Rows.Count >= BatchNodes)
            {
                BulkCopy(_nodeTable, "node");
                BulkCopy(_nodeTagsTable, "node_tags");
                CreateNodeTables();
            }
        }

        /// <summary>
        /// Adds a way to the target.
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(Way way)
        {
            if (!way.Id.HasValue)
                return; // id should always contain a value.

            DataRow wayRow = _wayTable.NewRow();

            // format data and create parameters.
            wayRow["id"] = way.Id.Value;
            wayRow["changeset_id"] = way.ChangeSetId.ConvertToDBValue();
            wayRow["visible"] = way.Visible.HasValue ? (object)(way.Visible.Value ? 1 : 0) : DBNull.Value;
            wayRow["timestamp"] = way.TimeStamp.ConvertToDBValue();
            wayRow["version"] = way.Version.ConvertToDBValue();
            wayRow["usr"] = way.UserName == null ? DBNull.Value : (object)way.UserName.Truncate(SQLServerSchemaConstants.RelationUsr);
            wayRow["usr_id"] = way.UserId.ConvertToDBValue();

            // add the way and it's tags.
            _wayTable.Rows.Add(wayRow);

            // tags.
            if (way.Tags != null)
            {
                foreach (Tag tag in way.Tags)
                {
                    string key = tag.Key.Truncate(SQLServerSchemaConstants.WayTagsKey);
                    if (string.IsNullOrEmpty(key))
                    {
                        //throw new Exception();
                    }
                    else
                    {
                        DataRow tagRow = _wayTagsTable.NewRow();
                        tagRow["way_id"] = way.Id.Value;
                        tagRow["key"] = key;
                        tagRow["value"] = tag.Value.Truncate(SQLServerSchemaConstants.WayTagsValue);

                        _wayTagsTable.Rows.Add(tagRow);
                    }
                }
            }

            // insert way nodes.
            if (way.Nodes != null)
            {
                for (int idx = 0; idx < way.Nodes.Count; idx++)
                {
                    DataRow tagRow = _wayNodesTable.NewRow();
                    tagRow["way_id"] = way.Id.Value;
                    tagRow["node_id"] = way.Nodes[idx];
                    tagRow["sequence_id"] = idx;

                    _wayNodesTable.Rows.Add(tagRow);
                }
            }

            // bulk insert if needed.
            if (_wayTable.Rows.Count >= BatchWays)
            {
                BulkCopy(_wayTable, "way");
                BulkCopy(_wayTagsTable, "way_tags");
                BulkCopy(_wayNodesTable, "way_nodes");
                CreateWayTables();
            }
        }

        /// <summary>
        /// Adds a relation to the target.
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(Relation relation)
        {
            if (!relation.Id.HasValue)
                return;

            DataRow relationRow = _relationTable.NewRow();

            // format data and create parameters.
            relationRow["id"] = relation.Id.Value;
            relationRow["changeset_id"] = relation.ChangeSetId.ConvertToDBValue();
            relationRow["visible"] = relation.Visible.HasValue ? (object)(relation.Visible.Value ? 1 : 0) : DBNull.Value;
            relationRow["timestamp"] = relation.TimeStamp.ConvertToDBValue();
            relationRow["version"] = relation.Version.ConvertToDBValue();
            relationRow["usr"] = relation.UserName == null ? DBNull.Value : (object)relation.UserName.Truncate(SQLServerSchemaConstants.RelationUsr);
            relationRow["usr_id"] = relation.UserId.ConvertToDBValue();

            // add the node and it's tags.
            _relationTable.Rows.Add(relationRow);

            // tags.
            if (relation.Tags != null)
            {
                foreach (Tag tag in relation.Tags)
                {
                    DataRow tagRow = _relationTagsTable.NewRow();
                    tagRow["relation_id"] = relation.Id.Value;
                    tagRow["key"] = tag.Key.Truncate(SQLServerSchemaConstants.RelationTagsKey);
                    tagRow["value"] = tag.Value.Truncate(SQLServerSchemaConstants.RelationTagsValue);

                    _relationTagsTable.Rows.Add(tagRow);
                }
            }

            // member.
            if (relation.Members != null)
            {
                for (int idx = 0; idx < relation.Members.Count; idx++)
                {
                    RelationMember member = relation.Members[idx];

                    DataRow tagRow = _relationMembersTable.NewRow();
                    tagRow["relation_id"] = relation.Id.Value;
                    tagRow["member_type"] = this.ConvertMemberType(relation.Members[idx].MemberType.Value).Value;
                    tagRow["member_id"] = member.MemberId;
                    tagRow["member_role"] = member.MemberRole.Truncate(SQLServerSchemaConstants.RelationMemberRole);
                    tagRow["sequence_id"] = idx;

                    _relationMembersTable.Rows.Add(tagRow);
                }
            }

            // bulk insert if needed.
            if (_relationTable.Rows.Count >= BatchRelations)
            {
                BulkCopy(_relationTable, "relation");
                BulkCopy(_relationTagsTable, "relation_tags");
                BulkCopy(_relationMembersTable, "relation_members");
                CreateRelationTables();
            }
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
        /// Flushes all data to the db.
        /// </summary>
        public override void Flush()
        {
            OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.SQLServer.Osm.Streams.SQLServerOsmStreamTarget", OsmSharp.Logging.TraceEventType.Information,
                "Flushing remaining data");
            BulkCopy(_nodeTable, "node");
            BulkCopy(_nodeTagsTable, "node_tags");
            _nodeTable.Clear();
            _nodeTagsTable.Clear();

            BulkCopy(_wayTable, "way");
            BulkCopy(_wayTagsTable, "way_tags");
            BulkCopy(_wayNodesTable, "way_nodes");
            _wayTable.Clear();
            _wayTagsTable.Clear();
            _wayNodesTable.Clear();

            BulkCopy(_relationTable, "relation");
            BulkCopy(_relationTagsTable, "relation_tags");
            BulkCopy(_relationMembersTable, "relation_members");
            _relationTable.Clear();
            _relationTagsTable.Clear();
            _relationMembersTable.Clear();
        }

        /// <summary>
        /// Closes the current target.
        /// </summary>
        public override void Close()
        {
            if(_connection != null)
            {
                if(_createAndDetectSchema)
                {
                    // Adds constraints
                    SQLServerSchemaTools.AddConstraints(_connection);
                }

                if (!string.IsNullOrWhiteSpace(_connectionString))
                {
                    _connection.Close();
                    _connection.Dispose();
                    OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.SQLServer.Osm.Streams.SQLServerOsmStreamTarget", OsmSharp.Logging.TraceEventType.Information,
                        "Database connection closed");
                }
            }
            _connection = null;
        }
    }
}