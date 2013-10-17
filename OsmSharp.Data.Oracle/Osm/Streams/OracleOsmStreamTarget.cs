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
using OsmSharp.Osm.Streams;
using Oracle.ManagedDataAccess.Client;
using OsmSharp.Osm;
using OsmSharp.Data.Oracle.Osm;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm.Tiles;
using OsmSharp.Math.Geo;

namespace OsmSharp.Data.Oracle.Osm.Streams
{
    /// <summary>
    /// An OSM stream target loading data into an oracle database.
    /// </summary>
    public class OracleOsmStreamTarget : OsmStreamTarget
    {
        /// <summary>
        /// Holds the oracle connection.
        /// </summary>
        private OracleConnection _connection;

        /// <summary>
        /// Holds the nodes-table.
        /// </summary>
        private object[] _nodeTable;

        /// <summary>
        /// Holds the nodes-table index keeping track of data present.
        /// </summary>
        private int _nodeTableLength = 0;

        /// <summary>
        /// Holds the nodes-tags-table.
        /// </summary>
        private object[] _nodeTagsTable;

        /// <summary>
        /// Holds the nodes-tags-table index keeping track of data present.
        /// </summary>
        private int _nodeTagsTableLength = 0;

        /// <summary>
        /// Holds the way-table.
        /// </summary>
        private object[] _wayTable;

        /// <summary>
        /// Holds the way-table index keeping track of data present.
        /// </summary>
        private int _wayTableLength = 0;

        /// <summary>
        /// Holds the way-tags-table.
        /// </summary>
        private object[] _wayTagsTable;

        /// <summary>
        /// Holds the way-tags-table index keeping track of data present.
        /// </summary>
        private int _wayTagsTableLength = 0;

        /// <summary>
        /// Holds the way-nodes-table.
        /// </summary>
        private object[] _wayNodesTable;

        /// <summary>
        /// Holds the way-nodes-table index keeping track of data present.
        /// </summary>
        private int _wayNodesTableLength = 0;

        /// <summary>
        /// Holds the relation table.
        /// </summary>
        private object[] _relationTable;

        /// <summary>
        /// Holds the relation-table index keeping track of data present.
        /// </summary>
        private int _relationTableLength = 0;

        /// <summary>
        /// Holds the relation-members-table.
        /// </summary>
        private object[] _relationMembersTable;

        /// <summary>
        /// Holds the relation-members-table index keeping track of data present.
        /// </summary>
        private int _relationMembersTableLength = 0;

        /// <summary>
        /// Holds the relation-tags-table.
        /// </summary>
        private object[] _relationTagsTable;

        /// <summary>
        /// Holds the relation-tags-table index keeping track of data present.
        /// </summary>
        private int _relationTagsTableLength = 0;

        /// <summary>
        /// Holds the batch count for nodes.
        /// </summary>
        private int _batchSizeNodes = 500000;

        /// <summary>
        /// Holds the batch count for ways.
        /// </summary>
        private int _batchSizeWays = 100000;

        /// <summary>
        /// Holds the batch count for relations.
        /// </summary>
        private int _batchSizeRelations = 50000;

        /// <summary>
        /// Holds the connection string.
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// Flag that indicates if the schema needs to be created if not present.
        /// </summary>
        private readonly bool _createAndDetectSchema;

        /// <summary>
        /// Creates a new oracle simple data processor target.
        /// </summary>
        /// <param name="connectionString"></param>
        public OracleOsmStreamTarget(string connectionString)
        {
            _connectionString = connectionString;
            _createAndDetectSchema = false;
        }

        /// <summary>
        /// Creates a new oracle simple data processor target.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="createAndDetectSchema"></param>
        public OracleOsmStreamTarget(string connectionString, bool createAndDetectSchema)
        {
            _connectionString = connectionString;
            _createAndDetectSchema = createAndDetectSchema;
        }

        /// <summary>
        /// Creates a new oracle simple data processor target.
        /// </summary>
        /// <param name="connection"></param>
        public OracleOsmStreamTarget(OracleConnection connection)
        {
            _connection = connection;
            _createAndDetectSchema = false;
        }

        /// <summary>
        /// Creates a new oracle simple data processor target.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="createAndDetectSchema"></param>
        public OracleOsmStreamTarget(OracleConnection connection, bool createAndDetectSchema)
        {
            _connection = connection;
            _createAndDetectSchema = createAndDetectSchema;
        }

        /// <summary>
        /// Sets the batch sizes.
        /// </summary>
        /// <param name="batchSizeNodes"></param>
        /// <param name="batchSizeWays"></param>
        /// <param name="batchSizeRelations"></param>
        public void SetBatchSizes(int batchSizeNodes, int batchSizeWays, int batchSizeRelations)
        {
            if (batchSizeNodes < 0) throw new ArgumentOutOfRangeException("batchSizeNodes");
            if (batchSizeWays < 0) throw new ArgumentOutOfRangeException("batchSizeWays");
            if (batchSizeRelations < 0) throw new ArgumentOutOfRangeException("batchSizeRelations");

            _batchSizeNodes = batchSizeNodes;
            _batchSizeWays = batchSizeWays;
            _batchSizeRelations = batchSizeRelations;
        }

        /// <summary>
        /// Initializes this target.
        /// </summary>
        public override void Initialize()
        {
            if (_connection == null)
            { // create connection if needed.
                _connection = new OracleConnection(_connectionString);
                _connection.Open();
            }

            if (_createAndDetectSchema)
            {
                // creates or detects the tables.
                OracleSchemaTools.CreateAndDetect(_connection);
            }

            this.CreateNodeBuffers();
            this.CreateWayBuffers();
            this.CreateRelationBuffers();
        }

        #region HelperFunctions

        /// <summary>
        /// Creates the node table buffers.
        /// </summary>
        private void CreateNodeBuffers()
        {
            // create node bulk objects.
            _nodeTable = new object[10];
            _nodeTable[0] = new long[_batchSizeNodes]; // id
            _nodeTable[1] = new long[_batchSizeNodes]; // latitude
            _nodeTable[2] = new long[_batchSizeNodes]; // longitude
            _nodeTable[3] = new long?[_batchSizeNodes]; // changeset_id
            _nodeTable[4] = new int?[_batchSizeNodes]; // visible
            _nodeTable[5] = new DateTime?[_batchSizeNodes]; // timestamp
            _nodeTable[6] = new long[_batchSizeNodes]; // tile
            _nodeTable[7] = new long?[_batchSizeNodes]; // version
            _nodeTable[8] = new string[_batchSizeNodes]; // usr
            _nodeTable[9] = new long?[_batchSizeNodes]; // usr_id

            // create node_tags bulk objects.
            _nodeTagsTable = new object[3];
            _nodeTagsTable[0] = new long[_batchSizeNodes]; // node_id
            _nodeTagsTable[1] = new string[_batchSizeNodes]; // key
            _nodeTagsTable[2] = new string[_batchSizeNodes]; // value
        }

        /// <summary>
        /// Creates the way table buffers.
        /// </summary>
        private void CreateWayBuffers()
        {
            // create way bulk objects.
            _wayTable = new object[7];
            _wayTable[0] = new long[_batchSizeWays]; // id
            _wayTable[1] = new long?[_batchSizeWays]; // changeset_id
            _wayTable[2] = new DateTime?[_batchSizeWays]; // timestamp
            _wayTable[3] = new int?[_batchSizeWays]; // visible
            _wayTable[4] = new long?[_batchSizeWays]; // version
            _wayTable[5] = new string[_batchSizeWays]; // usr
            _wayTable[6] = new long?[_batchSizeWays]; // usr_id

            // create way_tags bulk objects.
            _wayTagsTable = new object[3];
            _wayTagsTable[0] = new long[_batchSizeWays]; // node_id
            _wayTagsTable[1] = new string[_batchSizeWays]; // key
            _wayTagsTable[2] = new string[_batchSizeWays]; // value

            // create way_nodes bulk objects.
            _wayNodesTable = new object[3];
            _wayNodesTable[0] = new long[_batchSizeWays]; // way_id
            _wayNodesTable[1] = new long[_batchSizeWays]; // node_id
            _wayNodesTable[2] = new long[_batchSizeWays]; // sequence_id
        }

        /// <summary>
        /// Creates the relation table buffers.
        /// </summary>
        private void CreateRelationBuffers()
        {
            // create relation bulk objects.
            _relationTable = new object[7];
            _relationTable[0] = new long[_batchSizeWays]; // id
            _relationTable[1] = new long?[_batchSizeWays]; // changeset_id
            _relationTable[2] = new DateTime?[_batchSizeWays]; // timestamp
            _relationTable[3] = new int?[_batchSizeWays]; // visible
            _relationTable[4] = new long?[_batchSizeWays]; // version
            _relationTable[5] = new string[_batchSizeWays]; // usr
            _relationTable[6] = new long?[_batchSizeWays]; // usr_id

            // create relation_tags bulk objects.
            _relationTagsTable = new object[3];
            _relationTagsTable[0] = new long[_batchSizeRelations]; // node_id
            _relationTagsTable[1] = new string[_batchSizeRelations]; // key
            _relationTagsTable[2] = new string[_batchSizeRelations]; // value

            // create relation_members bulk objects.
            _relationMembersTable = new object[5];
            _relationMembersTable[0] = new long[_batchSizeRelations]; // relation_id
            _relationMembersTable[1] = new long[_batchSizeRelations]; // member_type
            _relationMembersTable[2] = new long[_batchSizeRelations]; // member_id
            _relationMembersTable[3] = new string[_batchSizeRelations]; // member_role
            _relationMembersTable[4] = new long[_batchSizeRelations]; // sequence_id
        }

        #endregion

        /// <summary>
        /// Adds a node.
        /// </summary>
        /// <param name="node"></param>
        public override void AddNode(Node node)
        {
            this.BulkCopyNode(node);

            // calculate the tileId
            Tile tile = Tile.CreateAroundLocation(node.Latitude.Value, node.Longitude.Value, 14);

            // add a new row to the buffer.
            (_nodeTable[0] as long[])[_nodeTableLength] = node.Id.Value;
            (_nodeTable[1] as long[])[_nodeTableLength] = (int)(node.Latitude * 10000000);
            (_nodeTable[2] as long[])[_nodeTableLength] = (int)(node.Longitude * 10000000);
            (_nodeTable[3] as long?[])[_nodeTableLength] = node.ChangeSetId;
            (_nodeTable[4] as int?[])[_nodeTableLength] = node.Visible.HasValue ? (int?)(node.Visible.Value ? 1 : 0) : null;
            (_nodeTable[5] as DateTime?[])[_nodeTableLength] = node.TimeStamp;
            (_nodeTable[6] as long[])[_nodeTableLength] = (long)tile.Id;
            (_nodeTable[7] as long?[])[_nodeTableLength] = node.Version.HasValue ? (long?)(long)node.Version.Value : null;
            (_nodeTable[8] as string[])[_nodeTableLength] = node.UserName;
            (_nodeTable[9] as long?[])[_nodeTableLength] = node.UserId.HasValue ? (long?)(long)node.UserId.Value : null;
            _nodeTableLength++;

            // add the tag rows to the buffer.
            if (node.Tags != null)
            {
                foreach (Tag tag in node.Tags)
                {
                    (_nodeTagsTable[0] as long[])[_nodeTagsTableLength] = node.Id.Value;
                    (_nodeTagsTable[1] as string[])[_nodeTagsTableLength] = tag.Key;
                    (_nodeTagsTable[2] as string[])[_nodeTagsTableLength] = tag.Value;
                    _nodeTagsTableLength++;
                }
            }
        }

        /// <summary>
        /// Bulk copies the existing nodes if the given node would fill up the buffer.
        /// </summary>
        /// <param name="node"></param>
        private void BulkCopyNode(Node node)
        {
            if (_nodeTableLength + 1 == _batchSizeNodes || 
                (node.Tags != null && node.Tags.Count + _nodeTagsTableLength >= _batchSizeNodes))
            { // oeps this node will overflow the buffers, bulk copy nodes.
                this.BulkCopyNode();
            }
        }

        /// <summary>
        /// Bulk copies the existing nodes.
        /// </summary>
        private void BulkCopyNode()
        {
            if (_nodeTableLength > 0)
            { // ok, bulk-insert the nodes.
                var insertCommand = new OracleCommand("INSERT INTO node (id, latitude, longitude, changeset_id, visible, timestamp, tile, version, usr, usr_id) " +
                    "values (:id, :latitude, :longitude, :changeset_id, :visible, :timestamp, :tile, :version, :usr, :usr_id)");
                insertCommand.Connection = _connection;
                insertCommand.ArrayBindCount = _nodeTableLength;

                insertCommand.Parameters.Add(new OracleParameter("id", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("latitude", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("longitude", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("changeset_id", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("visible", OracleDbType.Int16));
                insertCommand.Parameters.Add(new OracleParameter("timestamp", OracleDbType.Date));
                insertCommand.Parameters.Add(new OracleParameter("tile", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("version", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("usr", OracleDbType.Varchar2));
                insertCommand.Parameters.Add(new OracleParameter("usr_id", OracleDbType.Int64));

                insertCommand.Parameters["id"].Value = _nodeTable[0];
                insertCommand.Parameters["latitude"].Value = _nodeTable[1];
                insertCommand.Parameters["longitude"].Value = _nodeTable[2];
                insertCommand.Parameters["changeset_id"].Value = _nodeTable[3];
                insertCommand.Parameters["visible"].Value = _nodeTable[4];
                insertCommand.Parameters["timestamp"].Value = _nodeTable[5];
                insertCommand.Parameters["tile"].Value = _nodeTable[6];
                insertCommand.Parameters["version"].Value = _nodeTable[7];
                insertCommand.Parameters["usr"].Value = _nodeTable[8];
                insertCommand.Parameters["usr_id"].Value = _nodeTable[9];

                insertCommand.ExecuteNonQuery();
                _nodeTableLength = 0;
            }

            if (_nodeTagsTableLength > 0)
            {
                var insertCommand = new OracleCommand("INSERT INTO node_tags (node_id, key, value) " +
                    "values (:node_id, :key, :value)");
                insertCommand.Connection = _connection;
                insertCommand.ArrayBindCount = _nodeTagsTableLength;

                insertCommand.Parameters.Add(new OracleParameter("node_id", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("key", OracleDbType.Varchar2));
                insertCommand.Parameters.Add(new OracleParameter("value", OracleDbType.Varchar2));

                insertCommand.Parameters["node_id"].Value = _nodeTagsTable[0];
                insertCommand.Parameters["key"].Value = _nodeTagsTable[1];
                insertCommand.Parameters["value"].Value = _nodeTagsTable[2];

                insertCommand.ExecuteNonQuery();
                _nodeTagsTableLength = 0;
            }
        }

        /// <summary>
        /// Adds a way.
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(Way way)
        {
            this.BulkCopyWay(way);

            // add a new row to the buffer.
            (_wayTable[0] as long[])[_wayTableLength] = way.Id.Value;
            (_wayTable[1] as long?[])[_wayTableLength] = way.ChangeSetId;
            (_wayTable[2] as DateTime?[])[_wayTableLength] = way.TimeStamp;
            (_wayTable[3] as int?[])[_wayTableLength] = way.Visible.HasValue ? (int?)(way.Visible.Value ? 1 : 0) : null;
            (_wayTable[4] as long?[])[_wayTableLength] = way.Version.HasValue ? (long?)(long)way.Version.Value : null;
            (_wayTable[5] as string[])[_wayTableLength] = way.UserName;
            (_wayTable[6] as long?[])[_wayTableLength] = way.UserId;
            _wayTableLength++;

            // add the tag rows to the buffer.
            if (way.Tags != null)
            {
                foreach (Tag tag in way.Tags)
                {
                    (_wayTagsTable[0] as long[])[_wayTagsTableLength] = way.Id.Value;
                    (_wayTagsTable[1] as string[])[_wayTagsTableLength] = tag.Key;
                    (_wayTagsTable[2] as string[])[_wayTagsTableLength] = tag.Value;
                    _wayTagsTableLength++;
                }
            }

            // add the node rows to the buffer.
            if (way.Nodes != null)
            {
                for (int sequenceId = 0; sequenceId < way.Nodes.Count; sequenceId++)
                {
                    (_wayNodesTable[0] as long[])[_wayNodesTableLength] = way.Id.Value; // way_id
                    (_wayNodesTable[1] as long[])[_wayNodesTableLength] = way.Nodes[sequenceId]; // node_id
                    (_wayNodesTable[2] as long[])[_wayNodesTableLength] = sequenceId; // sequence_id
                    _wayNodesTableLength++;
                }
            }
        }

        /// <summary>
        /// Bulk copies the existing ways if the given way would fill up the buffer.
        /// </summary>
        /// <param name="way"></param>
        private void BulkCopyWay(Way way)
        {
            if (_wayTableLength + 1 == _batchSizeWays ||
                (way.Tags != null && way.Tags.Count + _wayTagsTableLength >= _batchSizeWays) ||
                (way.Nodes != null && way.Nodes.Count + _wayNodesTableLength > _batchSizeWays))
            { // oeps this node will overflow the buffers, bulk copy nodes.
                this.BulkCopyWay();
            }
        }

        /// <summary>
        /// Bulk copies the existing ways.
        /// </summary>
        private void BulkCopyWay()
        {
            if (_wayTableLength > 0)
            { // ok, bulk-insert the nodes.
                var insertCommand = new OracleCommand("INSERT INTO way (id, changeset_id, timestamp, visible, version, usr, usr_id) " +
                    "values (:id, :changeset_id, :timestamp, :visible, :version, :usr, :usr_id)");
                insertCommand.Connection = _connection;
                insertCommand.ArrayBindCount = _wayTableLength;

                insertCommand.Parameters.Add(new OracleParameter("id", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("changeset_id", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("timestamp", OracleDbType.Date));
                insertCommand.Parameters.Add(new OracleParameter("visible", OracleDbType.Int16));
                insertCommand.Parameters.Add(new OracleParameter("version", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("usr", OracleDbType.Varchar2));
                insertCommand.Parameters.Add(new OracleParameter("usr_id", OracleDbType.Int64));

                insertCommand.Parameters["id"].Value = _wayTable[0];
                insertCommand.Parameters["changeset_id"].Value = _wayTable[1];
                insertCommand.Parameters["timestamp"].Value = _wayTable[2];
                insertCommand.Parameters["visible"].Value = _wayTable[3];
                insertCommand.Parameters["version"].Value = _wayTable[4];
                insertCommand.Parameters["usr"].Value = _wayTable[5];
                insertCommand.Parameters["usr_id"].Value = _wayTable[6];

                insertCommand.ExecuteNonQuery();
                _wayTableLength = 0;
            }

            if (_wayTagsTableLength > 0)
            {
                var insertCommand = new OracleCommand("INSERT INTO way_tags (way_id, key, value) " +
                    "values (:way_id, :key, :value)");
                insertCommand.Connection = _connection;
                insertCommand.ArrayBindCount = _wayTagsTableLength;

                insertCommand.Parameters.Add(new OracleParameter("way_id", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("key", OracleDbType.Varchar2));
                insertCommand.Parameters.Add(new OracleParameter("value", OracleDbType.Varchar2));

                insertCommand.Parameters["way_id"].Value = _wayTagsTable[0];
                insertCommand.Parameters["key"].Value = _wayTagsTable[1];
                insertCommand.Parameters["value"].Value = _wayTagsTable[2];

                insertCommand.ExecuteNonQuery();
                _wayTagsTableLength = 0;
            }

            if (_wayNodesTableLength > 0)
            {
                var insertCommand = new OracleCommand("INSERT INTO way_nodes (way_id, node_id, sequence_id) " +
                    "values (:way_id, :node_id, :sequence_id)");
                insertCommand.Connection = _connection;
                insertCommand.ArrayBindCount = _wayNodesTableLength;

                insertCommand.Parameters.Add(new OracleParameter("way_id", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("node_id", OracleDbType.Varchar2));
                insertCommand.Parameters.Add(new OracleParameter("sequence_id", OracleDbType.Varchar2));

                insertCommand.Parameters["way_id"].Value = _wayNodesTable[0];
                insertCommand.Parameters["node_id"].Value = _wayNodesTable[1];
                insertCommand.Parameters["sequence_id"].Value = _wayNodesTable[2];

                insertCommand.ExecuteNonQuery();
                _wayNodesTableLength = 0;
            }
        }

        /// <summary>
        /// Adds a relation.
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(Relation relation)
        {
            this.BulkCopyRelation(relation);

            // add a new row to the buffer.
            (_relationTable[0] as long[])[_relationTableLength] = relation.Id.Value;
            (_relationTable[1] as long?[])[_relationTableLength] = relation.ChangeSetId;
            (_relationTable[2] as DateTime?[])[_relationTableLength] = relation.TimeStamp;
            (_relationTable[3] as int?[])[_relationTableLength] = relation.Visible.HasValue ? (int?)(relation.Visible.Value ? 1 : 0) : null;
            (_relationTable[4] as long?[])[_relationTableLength] = relation.Version.HasValue ? (long?)(long)relation.Version.Value : null;
            (_relationTable[5] as string[])[_relationTableLength] = relation.UserName;
            (_relationTable[6] as long?[])[_relationTableLength] = relation.UserId;
            _relationTableLength++;

            // add the tag rows to the buffer.
            if (relation.Tags != null)
            {
                foreach (Tag tag in relation.Tags)
                {
                    (_relationTagsTable[0] as long[])[_relationTagsTableLength] = relation.Id.Value;
                    (_relationTagsTable[1] as string[])[_relationTagsTableLength] = tag.Key;
                    (_relationTagsTable[2] as string[])[_relationTagsTableLength] = tag.Value;
                    _relationTagsTableLength++;
                }
            }

            // add the node rows to the buffer.
            if (relation.Members != null)
            {
                for (int sequenceId = 0; sequenceId < relation.Members.Count; sequenceId++)
                {
                    (_relationMembersTable[0] as long[])[_relationMembersTableLength] = relation.Id.Value; // relation_id
                    (_relationMembersTable[1] as long[])[_relationMembersTableLength] = this.ConvertMemberType(relation.Members[sequenceId].MemberType.Value).Value; // 
                    (_relationMembersTable[2] as long[])[_relationMembersTableLength] = relation.Members[sequenceId].MemberId.Value; // 
                    (_relationMembersTable[3] as string[])[_relationMembersTableLength] = relation.Members[sequenceId].MemberRole; // 
                    (_relationMembersTable[4] as long[])[_relationMembersTableLength] = sequenceId; // 
                    _relationMembersTableLength++;
                }
            }
        }

        /// <summary>
        /// Bulk copies the existing relations if the given relation would fill up the buffer.
        /// </summary>
        /// <param name="relation"></param>
        private void BulkCopyRelation(Relation relation)
        {
            if (_relationTableLength + 1 == _batchSizeRelations ||
                (relation.Tags != null && relation.Tags.Count + _relationTagsTableLength >= _batchSizeRelations) ||
                (relation.Members != null && relation.Members.Count + _relationMembersTableLength > _batchSizeRelations))
            { // oeps this node will overflow the buffers, bulk copy nodes.
                this.BulkCopyRelation();
            }
        }

        /// <summary>
        /// Bulk copies the existing relations.
        /// </summary>
        private void BulkCopyRelation()
        {
            if (_relationTableLength > 0)
            { // ok, bulk-insert the nodes.
                var insertCommand = new OracleCommand("INSERT INTO relation (id, changeset_id, timestamp, visible, version, usr, usr_id) " +
                    "values (:id, :changeset_id, :timestamp, :visible, :version, :usr, :usr_id)");
                insertCommand.Connection = _connection;
                insertCommand.ArrayBindCount = _relationTableLength;

                insertCommand.Parameters.Add(new OracleParameter("id", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("changeset_id", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("timestamp", OracleDbType.Date));
                insertCommand.Parameters.Add(new OracleParameter("visible", OracleDbType.Int16));
                insertCommand.Parameters.Add(new OracleParameter("version", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("usr", OracleDbType.Varchar2));
                insertCommand.Parameters.Add(new OracleParameter("usr_id", OracleDbType.Int64));

                insertCommand.Parameters["id"].Value = _relationTable[0];
                insertCommand.Parameters["changeset_id"].Value = _relationTable[1];
                insertCommand.Parameters["timestamp"].Value = _relationTable[2];
                insertCommand.Parameters["visible"].Value = _relationTable[3];
                insertCommand.Parameters["version"].Value = _relationTable[4];
                insertCommand.Parameters["usr"].Value = _relationTable[5];
                insertCommand.Parameters["usr_id"].Value = _relationTable[6];

                insertCommand.ExecuteNonQuery();
                _relationTableLength = 0;
            }

            if (_relationTagsTableLength > 0)
            {
                var insertCommand = new OracleCommand("INSERT INTO relation_tags (relation_id, key, value) " +
                    "values (:relation_id, :key, :value)");
                insertCommand.Connection = _connection;
                insertCommand.ArrayBindCount = _relationTagsTableLength;

                insertCommand.Parameters.Add(new OracleParameter("relation_id", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("key", OracleDbType.Varchar2));
                insertCommand.Parameters.Add(new OracleParameter("value", OracleDbType.Varchar2));

                insertCommand.Parameters["relation_id"].Value = _relationTagsTable[0];
                insertCommand.Parameters["key"].Value = _relationTagsTable[1];
                insertCommand.Parameters["value"].Value = _relationTagsTable[2];

                insertCommand.ExecuteNonQuery();
                _relationTagsTableLength = 0;
            }

            if (_relationMembersTableLength > 0)
            {
                var insertCommand = new OracleCommand("INSERT INTO relation_members (relation_id, member_type, member_id, member_role, sequence_id) " +
                    "values (:relation_id, :member_type, :member_id, :member_role, :sequence_id)");
                insertCommand.Connection = _connection;
                insertCommand.ArrayBindCount = _relationMembersTableLength;

                insertCommand.Parameters.Add(new OracleParameter("relation_id", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("member_type", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("member_id", OracleDbType.Int64));
                insertCommand.Parameters.Add(new OracleParameter("member_role", OracleDbType.Varchar2));
                insertCommand.Parameters.Add(new OracleParameter("sequence_id", OracleDbType.Int64));

                insertCommand.Parameters["relation_id"].Value = _relationMembersTable[0];
                insertCommand.Parameters["member_type"].Value = _relationMembersTable[1];
                insertCommand.Parameters["member_id"].Value = _relationMembersTable[2];
                insertCommand.Parameters["member_role"].Value = _relationMembersTable[3];
                insertCommand.Parameters["sequence_id"].Value = _relationMembersTable[4];

                insertCommand.ExecuteNonQuery();
                _relationMembersTableLength = 0;
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
        /// Flushes this target.
        /// </summary>
        public override void Flush()
        {
            this.BulkCopyNode();
            this.BulkCopyWay();
            this.BulkCopyRelation();
        }

        /// <summary>
        /// Closes this target.
        /// </summary>
        public override void Close()
        {
            if (_connection != null)
            {
                if (!string.IsNullOrWhiteSpace(_connectionString))
                {
                    _connection.Close();
                    _connection.Dispose();
                }
            }
            _connection = null;
        }
    }
}