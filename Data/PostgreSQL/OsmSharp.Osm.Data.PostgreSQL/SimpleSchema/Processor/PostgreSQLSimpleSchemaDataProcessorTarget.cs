// OsmSharp - OpenStreetMap tools & library.
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
using System.Data;
using OsmSharp.Tools;
using OsmSharp.Osm.Simple;
using OsmSharp.Osm.Data.Core.Processor;
using Npgsql;
using System.IO;
using OsmSharp.Osm.Data.PostgreSQL.SimpleSchema.SchemaTools;

namespace OsmSharp.Osm.Data.PostgreSQL.SimpleSchema.Processor
{
    /// <summary>
    /// A data processor target for the PostgreSQL simple schema.
    /// </summary>
    public class PostgreSQLSimpleSchemaDataProcessorTarget : DataProcessorTarget
    {
        /// <summary>
        /// Holds the connection.
        /// </summary>
        private NpgsqlConnection _connection;

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

        /// <summary>
        /// Creates a new target.
        /// </summary>
        /// <param name="connection_string"></param>
        public PostgreSQLSimpleSchemaDataProcessorTarget(string connection_string)
        {
            _connection_string = connection_string;
            _create_and_detect_schema = false;
        }

        /// <summary>
        /// Creates a new target.
        /// </summary>
        /// <param name="connection_string"></param>
        /// <param name="create_schema"></param>
        public PostgreSQLSimpleSchemaDataProcessorTarget(string connection_string, bool create_schema)
        {
            _connection_string = connection_string;
            _create_and_detect_schema = create_schema;
        }

        /// <summary>
        /// Initializes this target.
        /// </summary>
        public override void Initialize()
        {
            _connection = new NpgsqlConnection(_connection_string);
            _connection.Open();

            if (_create_and_detect_schema)
            { // creates or detects the tables.
                PostgreSQLSimpleSchemaTools.CreateAndDetect(_connection);
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
                // the copy command.
                NpgsqlCommand command = new NpgsqlCommand(string.Format(
                    "COPY {0} FROM STDIN WITH BINARY", table_name), _connection);

                // the copy in stream.
                // TODO: convert this to binary mode for speed and
                // to make sure the char ` can also be included in tags!
                NpgsqlCopyIn cin = new NpgsqlCopyIn(command, _connection);

                // copy line-by-line.
                cin.Start();
                try
                {
                    Stream target = cin.CopyStream;
                    //Stream target = new FileInfo(@"C:\Users\ben.abelshausen\Desktop\node_osmsharp.copy").OpenWrite();

                    // write header.
                    List<byte> header = new List<byte>();
                    header.AddRange(System.Text.Encoding.ASCII.GetBytes("PGCOPY\n"));
                    header.Add((byte)255);
                    header.AddRange(System.Text.Encoding.ASCII.GetBytes("\r\n\0"));

                    header.Add((byte)0); // start of Flags field
                    header.Add((byte)0);
                    header.Add((byte)0);
                    header.Add((byte)0);
                    header.Add((byte)0); // start of Flags field
                    header.Add((byte)0);
                    header.Add((byte)0);
                    header.Add((byte)0);
                    target.Write(header.ToArray(), 0, header.Count);

                    for (int row_idx = 0; row_idx < table.Rows.Count; row_idx++)
                    { // for each row generate the binary data.
                        // write the 16-bit integer count of the number of fields
                        byte[] field_count_data = BitConverter.GetBytes((short)table.Columns.Count);
                        this.ReverseEndianness(target, field_count_data);
                        //target.Write(field_count_data, 0, field_count_data.Length);

                        for (int column_idx = 0; column_idx < table.Columns.Count; column_idx++)
                        {
                            // serialize the data.
                            byte[] field_data = null;
                            object value = table.Rows[row_idx][column_idx];
                            bool reverse = false;
                            if (value == null)
                            {
                                // do nothing: just leave the field_data null.
                            }
                            else if (value is long)
                            { // convert the long data into bytes postgres can understand.
                                field_data = BitConverter.GetBytes((long)value);
                                reverse = true;
                            }
                            else if (value is int)
                            { // convert the int data into bytes postgres can understand.
                                field_data = BitConverter.GetBytes((int)value);
                                reverse = true;
                            }
                            else if (value is double)
                            { // convert the double data into bytes postgres can understand.
                                field_data = BitConverter.GetBytes((double)value);
                                reverse = true;
                            }
                            else if (value is float)
                            { // convert the float data into bytes postgres can understand.
                                field_data = BitConverter.GetBytes((float)value);
                                reverse = true;
                            }
                            else if (value is decimal)
                            { // convert the decimal data into bytes postgres can understand.
                                field_data = BitConverter.GetBytes((double)value);
                                reverse = true;
                            }
                            else if (value is DateTime)
                            { // convert the string data into bytes postgres can understand.
                                long microseconds = (long)((DateTime)value - (new DateTime(2000, 01, 01))).TotalSeconds 
                                    * 1000000;
                                //field_data = System.Text.Encoding.ASCII.GetBytes(((DateTime)value).ToString(
                                //    System.Globalization.CultureInfo.InvariantCulture));
                                field_data = BitConverter.GetBytes(microseconds);
                                reverse = true;
                            }
                            else if (value is string)
                            { // convert the string data into bytes postgres can understand.
                                field_data = System.Text.Encoding.ASCII.GetBytes(value as string);
                            }
                            else if (value is bool)
                            { // convert the bool data into bytes postgres can understand.
                                field_data = new byte[1];
                                if ((bool)value)
                                {
                                    field_data[0] = (byte)1;
                                }
                                else
                                {
                                    field_data[0] = (byte)0;
                                }
                            }
                            else
                            { // the type of the value is unsupported!
                                throw new InvalidDataException(string.Format("Data type not supported: {0}!",
                                    value.GetType()));
                            }

                            // write the length of the field.
                            int length = field_data.Length;
                            byte[] length_data = BitConverter.GetBytes(length);
                            this.ReverseEndianness(target, length_data);
                            //target.Write(length_data, 0, length_data.Length);

                            // write the data.
                            if (reverse)
                            { // write the data in reverse.
                                this.ReverseEndianness(target, field_data);
                            }
                            else
                            { // write the data in order.
                                target.Write(field_data, 0, field_data.Length);
                            }
                        }

                        if (row_idx % 100 == 0)
                        { // flush the data once in a while.
                            target.Flush();
                        }
                    }

                    // write the file trailer: a 16-bit integer word containing -1
                    byte[] trailer = BitConverter.GetBytes((short)-1);
                    target.Write(trailer, 0, trailer.Length);

                    // flush the stream data and close.
                    target.Flush();
                    target.Close();
                }
                catch (Exception ex)
                {
                    cin.Cancel(ex.Message);
                }
                finally
                {
                    cin.End();
                }

                OsmSharp.Tools.Output.OutputStreamHost.WriteLine(
                    "Inserted {0} records into {1}!", table.Rows.Count, table_name);
            }
        }

        /// <summary>
        /// Writes the given bytes after reversing their order.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        private void ReverseEndianness(Stream stream, byte[] data)
        {
            byte[] data_reverse = new byte[data.Length];
            for (int idx = data.Length - 1; idx >= 0; idx--)
            {
                data_reverse[data.Length - 1 - idx] = data[idx];
            }
            stream.Write(data_reverse, 0, data_reverse.Length);
        }

        #endregion

        /// <summary>
        /// Applies the given change.
        /// </summary>
        /// <param name="change"></param>
        public override void ApplyChange(SimpleChangeSet change)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Adds a node.
        /// </summary>
        /// <param name="node"></param>
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

        /// <summary>
        /// Adds a way.
        /// </summary>
        /// <param name="way"></param>
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

        /// <summary>
        /// Adds a relation.
        /// </summary>
        /// <param name="relation"></param>
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

        /// <summary>
        /// Closes this target.
        /// </summary>
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