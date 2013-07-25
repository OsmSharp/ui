// OsmSharp - OpenStreetMap tools & library.
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
using OsmSharp.Osm.Data.Streams;
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
        /// <param name="connectionString">Connection to the database</param>
        public SQLServerOsmStreamTarget(SqlConnection connection)
        {
            _connection = connection;
            _createAndDetectSchema = false;
        }

        /// <summary>
        /// Create a SQLServerSimpleSchemaDataProcessorTarget
        /// </summary>
        /// <param name="connectionString">Connection to the database</param>
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
            _relationMembersTable.Columns.Add(new DataColumn("member_type", typeof(string)));
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
                    OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.SQLServer.Osm.Streams.SQLServerOsmStreamTarget", System.Diagnostics.TraceEventType.Information,
                        "Inserting {0} records into {1}.", table.Rows.Count, tableName);
                    bulkCopy.WriteToServer(table);
                }
                catch (Exception e)
                {
                    OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.SQLServer.Osm.Streams.SQLServerOsmStreamTarget", System.Diagnostics.TraceEventType.Error,
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
            long id = node.Id.Value;
            nodeRow["id"] = id;

            var latitude = (int)(node.Latitude.Value * 10000000); // latitude should always contain a value.
            nodeRow["latitude"] = latitude;

            var longitude = (int)(node.Longitude.Value * 10000000); // longitude should always containt a value.
            nodeRow["longitude"] = longitude;

            long? changesetID = node.ChangeSetId;
            nodeRow["changeset_id"] = changesetID.ConvertToDBValue();

            bool? visible = node.Visible;
            int visibleInt = 1;
            if (!visible.HasValue || !visible.Value)
            {
                visibleInt = 0;
            }
            nodeRow["visible"] = visibleInt;

            DateTime? timestamp = node.TimeStamp;
            nodeRow["timestamp"] = timestamp.ConvertToDBValue();

            ulong? version = node.Version;
            nodeRow["version"] = version.ConvertToDBValue();

            // calculate the tile the node belongs to.
            nodeRow["tile"] = Tile.CreateAroundLocation(node.Latitude.Value, node.Longitude.Value, TileDefaultsForRouting.Zoom).Id;

            // list the usr
            nodeRow["usr"] = node.UserName.ToStringEmptyWhenNull().Truncate(SQLServerSchemaConstants.NodeUsr);
            nodeRow["usr_id"] = node.UserId.ConvertToDBValue();

            // add the node and it's tags.
            _nodeTable.Rows.Add(nodeRow);

            // tags.
            if (node.Tags != null)
            {
                foreach (Tag tag in node.Tags)
                {
                    DataRow tagRow = _nodeTagsTable.NewRow();
                    tagRow["key"] = tag.Key.Trim().Truncate(SQLServerSchemaConstants.NodeTagsKey);
                    tagRow["node_id"] = id;
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
            long id = way.Id.Value; // id should always contain a value.
            wayRow["id"] = id;

            long? changesetID = way.ChangeSetId;
            wayRow["changeset_id"] = changesetID.ConvertToDBValue();

            bool? visible = way.Visible;
            int visibleInt = 1;
            if (!visible.HasValue || !visible.Value)
            {
                visibleInt = 0;
            }
            wayRow["visible"] = visibleInt;

            DateTime? timestamp = way.TimeStamp;
            wayRow["timestamp"] = timestamp.ConvertToDBValue();

            ulong? version = way.Version;
            wayRow["version"] = version.ConvertToDBValue();

            // list the usr
            wayRow["usr"] = way.UserName.ToStringEmptyWhenNull().Truncate(SQLServerSchemaConstants.WayUsr);
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
                        tagRow["way_id"] = id;
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
                    long nodeID = way.Nodes[idx];
                    DataRow tagRow = _wayNodesTable.NewRow();
                    tagRow["way_id"] = id;
                    tagRow["node_id"] = nodeID;
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
            long id = relation.Id.Value; // id should alrelations contain a value.
            relationRow["id"] = id;

            long? changesetID = relation.ChangeSetId;
            relationRow["changeset_id"] = changesetID.ConvertToDBValue();

            bool? visible = relation.Visible;
            int visibleInt = 1;
            if (!visible.HasValue || !visible.Value)
            {
                visibleInt = 0;
            }
            relationRow["visible"] = visibleInt;

            DateTime? timestamp = relation.TimeStamp;
            relationRow["timestamp"] = timestamp.ConvertToDBValue();

            ulong? version = relation.Version;
            relationRow["version"] = version.ConvertToDBValue();

            // list the usr
            relationRow["usr"] = relation.UserName.ToStringEmptyWhenNull().Truncate(SQLServerSchemaConstants.RelationUsr);
            relationRow["usr_id"] = relation.UserId.ConvertToDBValue();

            // add the node and it's tags.
            _relationTable.Rows.Add(relationRow);

            // tags.
            if (relation.Tags != null)
            {
                foreach (Tag tag in relation.Tags)
                {
                    DataRow tagRow = _relationTagsTable.NewRow();
                    tagRow["relation_id"] = id;
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
                    tagRow["relation_id"] = id;
                    tagRow["member_type"] = member.MemberType;
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
        /// Flushes all data to the db.
        /// </summary>
        public override void Flush()
        {
            OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.SQLServer.Osm.Streams.SQLServerOsmStreamTarget", System.Diagnostics.TraceEventType.Information,
                "Flushing remaining data");
            BulkCopy(_nodeTable, "node");
            BulkCopy(_nodeTagsTable, "node_tags");

            BulkCopy(_wayTable, "way");
            BulkCopy(_wayTagsTable, "way_tags");
            BulkCopy(_wayNodesTable, "way_nodes");

            BulkCopy(_relationTable, "relation");
            BulkCopy(_relationTagsTable, "relation_tags");
            BulkCopy(_relationMembersTable, "relation_members");
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
                    OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.SQLServer.Osm.Streams.SQLServerOsmStreamTarget", System.Diagnostics.TraceEventType.Information,
                        "Database connection closed");
                }
            }
            _connection = null;
        }
    }
}