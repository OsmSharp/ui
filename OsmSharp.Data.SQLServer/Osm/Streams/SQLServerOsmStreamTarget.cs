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
using OsmSharp.Osm.Simple;
using OsmSharp.Collections.Tags;

namespace OsmSharp.Data.SQLServer.Osm.Streams
{
    /// <summary>
    /// A data processor target for the SqlServer simple schema.
    /// </summary>
    public class SQLServerOsmStreamTarget : OsmStreamTarget
    {
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
        private bool _createAndSetectSchema;

        int _batch_nodes = 100000;
        int _batch_ways = 100000;
        int _batch_relations = 5000;

        //int _batch_nodes = 1;
        //int _batch_ways = 1;
        //int _batch_relations = 1;

        private string _connectionString;

        public SQLServerOsmStreamTarget(string connectionString)
        {
            _connectionString = connectionString;
            _createAndSetectSchema = false;
        }

        public SQLServerOsmStreamTarget(string connectionString, bool createSchema)
        {
            _connectionString = connectionString;
            _createAndSetectSchema = createSchema;
        }


        public override void Initialize()
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();

            if (_createAndSetectSchema)
            { // creates or detects the tables.
                OsmSharp.Data.SQLServer.Osm.SchemaTools.SQLServerSchemaTools.CreateAndDetect(_connection);
            }

            this.CreateNodeTables();
            this.CreateWayTables();
            this.CreateRelationTables();
        }

        #region HelperFunctions

        private void CreateNodeTables()
        {
            // create node bulk objects.
            _nodeTable = new DataTable();
            _nodeTable.Columns.Add(new DataColumn("id", typeof(long)));
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
            _nodeTagsTable.Columns.Add(new DataColumn("node_id", typeof(long)));
            _nodeTagsTable.Columns.Add(new DataColumn("key", typeof(string)));
            _nodeTagsTable.Columns.Add(new DataColumn("value", typeof(string)));
        }

        private void CreateWayTables()
        {
            // create way bulk objects.
            _wayTable = new DataTable();
            _wayTable.Columns.Add(new DataColumn("id", typeof(long)));
            _wayTable.Columns.Add(new DataColumn("changeset_id", typeof(long)));
            _wayTable.Columns.Add(new DataColumn("timestamp", typeof(DateTime)));
            _wayTable.Columns.Add(new DataColumn("visible", typeof(bool)));
            _wayTable.Columns.Add(new DataColumn("version", typeof(int)));
            _wayTable.Columns.Add(new DataColumn("usr", typeof(string)));
            _wayTable.Columns.Add(new DataColumn("usr_id", typeof(int)));

            // create way_tags bulk objects.
            _wayTagsTable = new DataTable();
            _wayTagsTable.Columns.Add(new DataColumn("way_id", typeof(long)));
            _wayTagsTable.Columns.Add(new DataColumn("key", typeof(string)));
            _wayTagsTable.Columns.Add(new DataColumn("value", typeof(string)));

            // create way_nodes bulk objects.
            _wayNodesTable = new DataTable();
            _wayNodesTable.Columns.Add(new DataColumn("way_id", typeof(long)));
            _wayNodesTable.Columns.Add(new DataColumn("node_id", typeof(long)));
            _wayNodesTable.Columns.Add(new DataColumn("sequence_id", typeof(int)));
        }

        private void CreateRelationTables()
        {
            // create relation bulk objects.
            _relationTable = new DataTable();
            _relationTable.Columns.Add(new DataColumn("id", typeof(long)));
            _relationTable.Columns.Add(new DataColumn("changeset_id", typeof(long)));
            _relationTable.Columns.Add(new DataColumn("timestamp", typeof(DateTime)));
            _relationTable.Columns.Add(new DataColumn("visible", typeof(bool)));
            _relationTable.Columns.Add(new DataColumn("version", typeof(int)));
            _relationTable.Columns.Add(new DataColumn("usr", typeof(string)));
            _relationTable.Columns.Add(new DataColumn("usr_id", typeof(int)));

            // create relation_tags bulk objects.
            _relationTagsTable = new DataTable();
            _relationTagsTable.Columns.Add(new DataColumn("relation_id", typeof(long)));
            _relationTagsTable.Columns.Add(new DataColumn("key", typeof(string)));
            _relationTagsTable.Columns.Add(new DataColumn("value", typeof(string)));

            // create relation_members bulk objects.
            _relationMembersTable = new DataTable();
            _relationMembersTable.Columns.Add(new DataColumn("relation_id", typeof(long)));
            _relationMembersTable.Columns.Add(new DataColumn("member_type", typeof(string)));
            _relationMembersTable.Columns.Add(new DataColumn("member_id", typeof(long)));
            _relationMembersTable.Columns.Add(new DataColumn("member_role", typeof(string)));
            _relationMembersTable.Columns.Add(new DataColumn("sequence_id", typeof(int)));
        }

        /// <summary>
        /// Does the actual bulk copy.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="table_name"></param>
        private void BulkCopy(DataTable table, string table_name)
        {
            this.BulkCopy(table, table_name, table.Rows.Count + 1);
        }

        /// <summary>
        /// Does the actual bulk inserts.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="table_name"></param>
        /// <param name="batch_size"></param>
        private void BulkCopy(DataTable table, string table_name, int batch_size)
        {
            if (table != null && table.Rows.Count > 0)
            {

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_connection))
                {
                    bulkCopy.DestinationTableName = table_name;
                    bulkCopy.BatchSize = batch_size;
                    bulkCopy.WriteToServer(table);
                }

                OsmSharp.IO.Output.OutputStreamHost.WriteLine(
                    "Inserted {0} records into {1}!", table.Rows.Count, table_name);
            }
        }

        #endregion

        public override void AddNode(Node node)
        {
            DataRow node_row = _nodeTable.NewRow();

            // format data and create parameters.
            long? id = node.Id;
            node_row["id"] = id.ConvertToDBValue<long>();

            int? latitude = (int)(node.Latitude * 10000000); // latitude should always contain a value.
            node_row["latitude"] = latitude.ConvertToDBValue<int>();

            int? longitude = (int)(node.Longitude * 10000000); // longitude should always containt a value.
            node_row["longitude"] = longitude.ConvertToDBValue<int>();

            long? changeset_id = node.ChangeSetId;
            node_row["changeset_id"] = changeset_id.ConvertToDBValue<long>();

            bool? visible = node.Visible;
            int visible_int = 1;
            if (!visible.HasValue || !visible.Value)
            {
                visible_int = 0;
            }
            node_row["visible"] = visible_int;

            DateTime? timestamp = node.TimeStamp;
            node_row["timestamp"] = timestamp.ConvertToDBValue<DateTime>();

            ulong? version = node.Version;
            node_row["version"] = version.ConvertToDBValue<ulong>();

            // calculate the tile the node belongs to.
            long tile = TileCalculations.xy2tile(TileCalculations.lon2x(node.Longitude.Value), TileCalculations.lat2y(node.Latitude.Value));
            node_row["tile"] = tile;

            // set the usr
            node_row["usr"] = node.UserName.ToStringEmptyWhenNull();
            node_row["usr_id"] = node.UserId.ConvertToDBValue<long>();

            // add the node and it's tags.
            _nodeTable.Rows.Add(node_row);

            // tags.
            if (node.Tags != null)
            {
                foreach (Tag tag in node.Tags)
                {
                    string key = tag.Key;
                    string value = tag.Value;

                    DataRow tag_row = _nodeTagsTable.NewRow();
                    tag_row["node_id"] = id;
                    tag_row["key"] = key.Truncate(255);
                    tag_row["value"] = value.Truncate(255);

                    _nodeTagsTable.Rows.Add(tag_row);
                }
            }

            // bulk insert if needed.
            if (_nodeTable.Rows.Count >= _batch_nodes)
            {
                this.BulkCopy(_nodeTable, "node", _batch_nodes);
                this.BulkCopy(_nodeTagsTable, "node_tags");
                this.CreateNodeTables();
            }
        }

        public override void AddWay(Way way)
        {
            DataRow way_row = _wayTable.NewRow();

            // format data and create parameters.
            long? id = way.Id.Value; // id should always contain a value.
            way_row["id"] = id.ConvertToDBValue<long>();

            long? changeset_id = way.ChangeSetId;
            way_row["changeset_id"] = changeset_id.ConvertToDBValue<long>();

            bool? visible = way.Visible;
            int visible_int = 1;
            if (!visible.HasValue || !visible.Value)
            {
                visible_int = 0;
            }
            way_row["visible"] = visible_int;

            DateTime? timestamp = way.TimeStamp;
            way_row["timestamp"] = timestamp.ConvertToDBValue<DateTime>();

            ulong? version = way.Version;
            way_row["version"] = version.ConvertToDBValue<ulong>();

            // set the usr
            way_row["usr"] = way.UserName.ToStringEmptyWhenNull();
            way_row["usr_id"] = way.UserId.ConvertToDBValue<long>();

            // add the way and it's tags.
            _wayTable.Rows.Add(way_row);

            // tags.
            if (way.Tags != null)
            {
                foreach (Tag tag in way.Tags)
                {
                    string key = tag.Key;
                    string value = tag.Value;

                    if (key == null || key.Length == 0)
                    {
                        //throw new Exception();
                    }
                    else
                    {
                        DataRow tag_row = _wayTagsTable.NewRow();
                        tag_row["way_id"] = id;
                        tag_row["key"] = key.Truncate(255);
                        tag_row["value"] = value.Truncate(255);

                        _wayTagsTable.Rows.Add(tag_row);
                    }
                }
            }

            // insert way nodes.
            if (way.Nodes != null)
            {
                long way_id = way.Id.Value;
                for (int idx = 0; idx < way.Nodes.Count; idx++)
                {
                    long node_id = way.Nodes[idx];

                    DataRow tag_row = _wayNodesTable.NewRow();
                    tag_row["way_id"] = id;
                    tag_row["node_id"] = node_id;
                    tag_row["sequence_id"] = idx;

                    _wayNodesTable.Rows.Add(tag_row);
                }
            }

            // bulk insert if needed.
            if (_wayTable.Rows.Count >= _batch_ways)
            {
                this.BulkCopy(_wayTable, "way", _batch_ways);
                this.BulkCopy(_wayTagsTable, "way_tags");
                this.BulkCopy(_wayNodesTable, "way_nodes");
                this.CreateWayTables();
            }
        }

        public override void AddRelation(Relation relation)
        {
            DataRow relation_row = _relationTable.NewRow();

            // format data and create parameters.
            long? id = relation.Id.Value; // id should alrelations contain a value.
            relation_row["id"] = id.ConvertToDBValue<long>();

            long? changeset_id = relation.ChangeSetId;
            relation_row["changeset_id"] = changeset_id.ConvertToDBValue<long>();

            bool? visible = relation.Visible;
            int visible_int = 1;
            if (!visible.HasValue || !visible.Value)
            {
                visible_int = 0;
            }
            relation_row["visible"] = visible_int;

            DateTime? timestamp = relation.TimeStamp;
            relation_row["timestamp"] = timestamp.ConvertToDBValue<DateTime>();

            ulong? version = relation.Version;
            relation_row["version"] = version.ConvertToDBValue<ulong>();

            // set the usr
            relation_row["usr"] = relation.UserName.ToStringEmptyWhenNull();
            relation_row["usr_id"] = relation.UserId.ConvertToDBValue<long>();


            // add the node and it's tags.
            _relationTable.Rows.Add(relation_row);

            // tags.
            if (relation.Tags != null)
            {
                foreach (Tag tag in relation.Tags)
                {
                    string key = tag.Key;
                    string value = tag.Value;

                    DataRow tag_row = _relationTagsTable.NewRow();
                    tag_row["relation_id"] = id;
                    tag_row["key"] = key.Truncate(255);
                    tag_row["value"] = value.Truncate(255);

                    _relationTagsTable.Rows.Add(tag_row);
                }
            }

            // member.
            if (relation.Members != null)
            {
                long relation_id = relation.Id.Value;

                for (int idx = 0; idx < relation.Members.Count; idx++)
                {
                    RelationMember member = relation.Members[idx]; ;

                    DataRow tag_row = _relationMembersTable.NewRow();
                    tag_row["relation_id"] = id;
                    tag_row["member_type"] = member.MemberType;
                    tag_row["member_id"] = member.MemberId;
                    tag_row["member_role"] = member.MemberRole;
                    tag_row["sequence_id"] = idx;

                    _relationMembersTable.Rows.Add(tag_row);
                }
            }

            // bulk insert if needed.
            if (_relationTable.Rows.Count >= _batch_relations)
            {
                this.BulkCopy(_relationTable, "relation");
                this.BulkCopy(_relationTagsTable, "relation_tags");
                this.BulkCopy(_relationMembersTable, "relation_members");
                this.CreateRelationTables();
            }
        }

        public override void Close()
        {
            if (_connection != null)
            {
                if (_nodeTable.Rows.Count > 0)
                {
                    this.BulkCopy(_nodeTable, "node");
                    this.BulkCopy(_nodeTagsTable, "node_tags");
                }

                if (_wayTable.Rows.Count > 0)
                {
                    this.BulkCopy(_wayTable, "way");
                    this.BulkCopy(_wayTagsTable, "way_tags");
                    this.BulkCopy(_wayNodesTable, "way_nodes");
                }

                if (_relationTable.Rows.Count > 0)
                {
                    this.BulkCopy(_relationTable, "relation");
                    this.BulkCopy(_relationTagsTable, "relation_tags");
                    this.BulkCopy(_relationMembersTable, "relation_members");
                }

                _connection.Close();
                _connection.Dispose();
            }
            _connection = null;
        }
    }
}
