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
using OsmSharp.Osm.Data.SQLServer.SimpleSchema.SchemaTools;
using OsmSharp.Osm.Simple;
using OsmSharp.Osm.Data.Core.Processor;
using System.Data.SqlClient;
using OsmSharp.Tools;

namespace OsmSharp.Osm.Data.SQLServer.SimpleSchema.Processor
{
    /// <summary>
    /// A data processor target for the SqlServer simple schema.
    /// </summary>
    public class SQLServerSimpleSchemaDataProcessorTarget : DataProcessorTarget
    {
        /// <summary>
        /// Holds the connection.
        /// </summary>
        private SqlConnection _connection;

        private DataTable _node_table;

        private DataTable _node_tags_table;

        private DataTable _way_table;

        private DataTable _way_tags_table;

        private DataTable _way_nodes_table;

        private DataTable _relation_table;

        private DataTable _relation_members_table;

        private DataTable _relation_tags_table;

        /// <summary>
        /// Flag that indicates if the schema needs to be created if not present.
        /// </summary>
        private bool _create_and_detect_schema;

        int _batch_nodes = 100000;
        int _batch_ways = 100000;
        int _batch_relations = 5000;

        //int _batch_nodes = 1;
        //int _batch_ways = 1;
        //int _batch_relations = 1;

        private string _connection_string;

        public SQLServerSimpleSchemaDataProcessorTarget(string connection_string)
        {
            _connection_string = connection_string;
            _create_and_detect_schema = false;
        }

        public SQLServerSimpleSchemaDataProcessorTarget(string connection_string, bool create_schema)
        {
            _connection_string = connection_string;
            _create_and_detect_schema = create_schema;
        }


        public override void Initialize()
        {
            _connection = new SqlConnection(_connection_string);
            _connection.Open();

            if (_create_and_detect_schema)
            { // creates or detects the tables.
                SQLServerSimpleSchemaTools.CreateAndDetect(_connection);
            }

            this.CreateNodeTables();
            this.CreateWayTables();
            this.CreateRelationTables();
        }

        #region HelperFunctions

        private void CreateNodeTables()
        {
            // create node bulk objects.
            _node_table = new DataTable();
            _node_table.Columns.Add(new DataColumn("id", typeof(long)));
            _node_table.Columns.Add(new DataColumn("latitude", typeof(int)));
            _node_table.Columns.Add(new DataColumn("longitude", typeof(int)));
            _node_table.Columns.Add(new DataColumn("changeset_id", typeof(long)));
            _node_table.Columns.Add(new DataColumn("visible", typeof(bool)));
            _node_table.Columns.Add(new DataColumn("timestamp", typeof(DateTime)));
            _node_table.Columns.Add(new DataColumn("tile", typeof(long)));
            _node_table.Columns.Add(new DataColumn("version", typeof(int)));
            _node_table.Columns.Add(new DataColumn("usr", typeof(string)));
            _node_table.Columns.Add(new DataColumn("usr_id", typeof(int)));

            // create node_tags bulk objects.
            _node_tags_table = new DataTable();
            _node_tags_table.Columns.Add(new DataColumn("node_id", typeof(long)));
            _node_tags_table.Columns.Add(new DataColumn("key", typeof(string)));
            _node_tags_table.Columns.Add(new DataColumn("value", typeof(string)));
        }

        private void CreateWayTables()
        {
            // create way bulk objects.
            _way_table = new DataTable();
            _way_table.Columns.Add(new DataColumn("id", typeof(long)));
            _way_table.Columns.Add(new DataColumn("changeset_id", typeof(long)));
            _way_table.Columns.Add(new DataColumn("timestamp", typeof(DateTime)));
            _way_table.Columns.Add(new DataColumn("visible", typeof(bool)));
            _way_table.Columns.Add(new DataColumn("version", typeof(int)));
            _way_table.Columns.Add(new DataColumn("usr", typeof(string)));
            _way_table.Columns.Add(new DataColumn("usr_id", typeof(int)));

            // create way_tags bulk objects.
            _way_tags_table = new DataTable();
            _way_tags_table.Columns.Add(new DataColumn("way_id", typeof(long)));
            _way_tags_table.Columns.Add(new DataColumn("key", typeof(string)));
            _way_tags_table.Columns.Add(new DataColumn("value", typeof(string)));

            // create way_nodes bulk objects.
            _way_nodes_table = new DataTable();
            _way_nodes_table.Columns.Add(new DataColumn("way_id", typeof(long)));
            _way_nodes_table.Columns.Add(new DataColumn("node_id", typeof(long)));
            _way_nodes_table.Columns.Add(new DataColumn("sequence_id", typeof(int)));
        }

        private void CreateRelationTables()
        {
            // create relation bulk objects.
            _relation_table = new DataTable();
            _relation_table.Columns.Add(new DataColumn("id", typeof(long)));
            _relation_table.Columns.Add(new DataColumn("changeset_id", typeof(long)));
            _relation_table.Columns.Add(new DataColumn("timestamp", typeof(DateTime)));
            _relation_table.Columns.Add(new DataColumn("visible", typeof(bool)));
            _relation_table.Columns.Add(new DataColumn("version", typeof(int)));
            _relation_table.Columns.Add(new DataColumn("usr", typeof(string)));
            _relation_table.Columns.Add(new DataColumn("usr_id", typeof(int)));

            // create relation_tags bulk objects.
            _relation_tags_table = new DataTable();
            _relation_tags_table.Columns.Add(new DataColumn("relation_id", typeof(long)));
            _relation_tags_table.Columns.Add(new DataColumn("key", typeof(string)));
            _relation_tags_table.Columns.Add(new DataColumn("value", typeof(string)));

            // create relation_members bulk objects.
            _relation_members_table = new DataTable();
            _relation_members_table.Columns.Add(new DataColumn("relation_id", typeof(long)));
            _relation_members_table.Columns.Add(new DataColumn("member_type", typeof(string)));
            _relation_members_table.Columns.Add(new DataColumn("member_id", typeof(long)));
            _relation_members_table.Columns.Add(new DataColumn("member_role", typeof(string)));
            _relation_members_table.Columns.Add(new DataColumn("sequence_id", typeof(int)));
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

                OsmSharp.Tools.Output.OutputStreamHost.WriteLine(
                    "Inserted {0} records into {1}!", table.Rows.Count, table_name);
            }
        }

        #endregion

        public override void ApplyChange(SimpleChangeSet change)
        {
            throw new NotSupportedException();
        }

        public override void AddNode(SimpleNode node)
        {
            DataRow node_row = _node_table.NewRow();

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
            _node_table.Rows.Add(node_row);

            // tags.
            if (node.Tags != null)
            {
                foreach (KeyValuePair<string, string> tag in node.Tags)
                {
                    string key = tag.Key;
                    string value = tag.Value;

                    DataRow tag_row = _node_tags_table.NewRow();
                    tag_row["node_id"] = id;
                    tag_row["key"] = key.Truncate(255);
                    tag_row["value"] = value.Truncate(255);

                    _node_tags_table.Rows.Add(tag_row);
                }
            }

            // bulk insert if needed.
            if (_node_table.Rows.Count >= _batch_nodes)
            {
                this.BulkCopy(_node_table, "node", _batch_nodes);
                this.BulkCopy(_node_tags_table, "node_tags");
                this.CreateNodeTables();
            }
        }

        public override void AddWay(SimpleWay way)
        {
            DataRow way_row = _way_table.NewRow();

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
            _way_table.Rows.Add(way_row);

            // tags.
            if (way.Tags != null)
            {
                foreach (KeyValuePair<string, string> tag in way.Tags)
                {
                    string key = tag.Key;
                    string value = tag.Value;

                    if (key == null || key.Length == 0)
                    {
                        //throw new Exception();
                    }
                    else
                    {
                        DataRow tag_row = _way_tags_table.NewRow();
                        tag_row["way_id"] = id;
                        tag_row["key"] = key.Truncate(255);
                        tag_row["value"] = value.Truncate(255);

                        _way_tags_table.Rows.Add(tag_row);
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

                    DataRow tag_row = _way_nodes_table.NewRow();
                    tag_row["way_id"] = id;
                    tag_row["node_id"] = node_id;
                    tag_row["sequence_id"] = idx;

                    _way_nodes_table.Rows.Add(tag_row);
                }
            }

            // bulk insert if needed.
            if (_way_table.Rows.Count >= _batch_ways)
            {
                this.BulkCopy(_way_table, "way", _batch_ways);
                this.BulkCopy(_way_tags_table, "way_tags");
                this.BulkCopy(_way_nodes_table, "way_nodes");
                this.CreateWayTables();
            }
        }

        public override void AddRelation(SimpleRelation relation)
        {
            DataRow relation_row = _relation_table.NewRow();

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
            _relation_table.Rows.Add(relation_row);

            // tags.
            if (relation.Tags != null)
            {
                foreach (KeyValuePair<string, string> tag in relation.Tags)
                {
                    string key = tag.Key;
                    string value = tag.Value;

                    DataRow tag_row = _relation_tags_table.NewRow();
                    tag_row["relation_id"] = id;
                    tag_row["key"] = key.Truncate(255);
                    tag_row["value"] = value.Truncate(255);

                    _relation_tags_table.Rows.Add(tag_row);
                }
            }

            // member.
            if (relation.Members != null)
            {
                long relation_id = relation.Id.Value;

                for (int idx = 0; idx < relation.Members.Count; idx++)
                {
                    SimpleRelationMember member = relation.Members[idx]; ;

                    DataRow tag_row = _relation_members_table.NewRow();
                    tag_row["relation_id"] = id;
                    tag_row["member_type"] = member.MemberType;
                    tag_row["member_id"] = member.MemberId;
                    tag_row["member_role"] = member.MemberRole;
                    tag_row["sequence_id"] = idx;

                    _relation_members_table.Rows.Add(tag_row);
                }
            }

            // bulk insert if needed.
            if (_relation_table.Rows.Count >= _batch_relations)
            {
                this.BulkCopy(_relation_table, "relation");
                this.BulkCopy(_relation_tags_table, "relation_tags");
                this.BulkCopy(_relation_members_table, "relation_members");
                this.CreateRelationTables();
            }
        }

        public override void Close()
        {
            if (_connection != null)
            {
                if (_node_table.Rows.Count > 0)
                {
                    this.BulkCopy(_node_table, "node");
                    this.BulkCopy(_node_tags_table, "node_tags");
                }

                if (_way_table.Rows.Count > 0)
                {
                    this.BulkCopy(_way_table, "way");
                    this.BulkCopy(_way_tags_table, "way_tags");
                    this.BulkCopy(_way_nodes_table, "way_nodes");
                }

                if (_relation_table.Rows.Count > 0)
                {
                    this.BulkCopy(_relation_table, "relation");
                    this.BulkCopy(_relation_tags_table, "relation_tags");
                    this.BulkCopy(_relation_members_table, "relation_members");
                }

                _connection.Close();
                _connection.Dispose();
            }
            _connection = null;
        }
    }
}
