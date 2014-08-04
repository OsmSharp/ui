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
using System.Data;
using Npgsql;
using System.IO;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm.Tiles;

namespace OsmSharp.Data.PostgreSQL.Osm.Streams
{
    /// <summary>
    /// A data processor target for the PostgreSQL schema.
    /// </summary>
    public class PostgreSQLOsmStreamTarget : OsmStreamTarget
    {
        /// <summary>
        /// Holds the connection.
        /// </summary>
        private NpgsqlConnection _connection;

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
        private bool _createAndDetectSchema;

        private Encoding _encoding;

        int _batch_nodes = 100000;
        int _batch_ways = 100000;
        int _batch_relations = 5000;

        private string _connectionString;

        /// <summary>
        /// Creates a new target.
        /// </summary>
        /// <param name="connectionString"></param>
        public PostgreSQLOsmStreamTarget(string connectionString)
            : this(Encoding.UTF8, connectionString) { }

        /// <summary>
        /// Creates a new target.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="createSchema"></param>
        public PostgreSQLOsmStreamTarget(string connectionString, bool createSchema)
            : this(Encoding.UTF8, connectionString, createSchema) { }

        /// <summary>
        /// Creates a new target.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="createSchema"></param>
        public PostgreSQLOsmStreamTarget(NpgsqlConnection connection, bool createSchema)
            : this(Encoding.UTF8, connection, createSchema) { }

        /// <summary>
        /// Creates a new target.
        /// </summary>
        /// <param name="connection"></param>
        public PostgreSQLOsmStreamTarget(NpgsqlConnection connection)
            : this(Encoding.UTF8, connection) { }

        /// <summary>
        /// Creates a new target.
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="connectionString"></param>
        public PostgreSQLOsmStreamTarget(Encoding encoding, string connectionString)
        {
            _encoding = encoding;
            _connectionString = connectionString;
            _createAndDetectSchema = false;
        }

        /// <summary>
        /// Creates a new target.
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="connectionString"></param>
        /// <param name="createSchema"></param>
        public PostgreSQLOsmStreamTarget(Encoding encoding, string connectionString, bool createSchema)
        {
            _encoding = encoding;
            _connectionString = connectionString;
            _createAndDetectSchema = createSchema;
        }

        /// <summary>
        /// Creates a new target.
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="connection"></param>
        /// <param name="createSchema"></param>
        public PostgreSQLOsmStreamTarget(Encoding encoding, NpgsqlConnection connection, bool createSchema)
        {
            _encoding = encoding;
            _connection = connection;
            _createAndDetectSchema = createSchema;
        }

        /// <summary>
        /// Creates a new target.
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="connection"></param>
        public PostgreSQLOsmStreamTarget(Encoding encoding, NpgsqlConnection connection) : this(encoding, connection, true) { }

        /// <summary>
        /// Initializes this target.
        /// </summary>
        public override void Initialize()
        {
            if (_connection == null)
            {
                _connection = new NpgsqlConnection(_connectionString);
            }
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            if (_createAndDetectSchema)
            { // creates or detects the tables.
                PostgreSQLSchemaTools.CreateAndDetect(_connection);
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
            _relationMembersTable.Columns.Add(new DataColumn("member_type", typeof(int)));
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
                    System.IO.Stream target = cin.CopyStream;
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
                            if (value == null || value == DBNull.Value)
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
                                field_data = _encoding.GetBytes(value as string);
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
                            int length = -1; // represents NULL.
                            if (field_data != null)
                            { // the lenght is non-zero.
                                length = field_data.Length;
                            }
                            byte[] length_data = BitConverter.GetBytes(length);
                            this.ReverseEndianness(target, length_data);

                            // write the data.
                            if (field_data != null)
                            {
                                if (reverse)
                                { // write the data in reverse.
                                    this.ReverseEndianness(target, field_data);
                                }
                                else
                                { // write the data in order.
                                    target.Write(field_data, 0, field_data.Length);
                                }
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

                OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.PostgreSQL.Osm.Streams.PostgeSQLOsmStreamTarget", OsmSharp.Logging.TraceEventType.Information,
                    "Inserted {0} records into {1}!", table.Rows.Count, table_name);
            }
        }

        /// <summary>
        /// Writes the given bytes after reversing their order.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="data"></param>
        private void ReverseEndianness(System.IO.Stream stream, byte[] data)
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
        /// Adds a node.
        /// </summary>
        /// <param name="node"></param>
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

            if (!node.Visible.HasValue)
            {
                node_row["visible"] = DBNull.Value;
            }
            else
            {
                node_row["visible"] = node.Visible.Value ? 1 : 0;
            }

            DateTime? timestamp = node.TimeStamp;
            node_row["timestamp"] = timestamp.ConvertToDBValue<DateTime>();

            ulong? version = node.Version;
            node_row["version"] = version.ConvertToDBValue<ulong>();

            // calculate the tile the node belongs to.
            ulong tile = Tile.CreateAroundLocation(new Math.Geo.GeoCoordinate(node.Latitude.Value, node.Longitude.Value), 14).Id;
            node_row["tile"] = tile;

            // set the usr
            node_row["usr"] = node.UserName;
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

        /// <summary>
        /// Adds a way.
        /// </summary>
        /// <param name="way"></param>
        public override void AddWay(Way way)
        {
            DataRow way_row = _wayTable.NewRow();

            // format data and create parameters.
            long? id = way.Id.Value; // id should always contain a value.
            way_row["id"] = id.ConvertToDBValue<long>();

            long? changeset_id = way.ChangeSetId;
            way_row["changeset_id"] = changeset_id.ConvertToDBValue<long>();

            if (!way.Visible.HasValue)
            {
                way_row["visible"] = DBNull.Value;
            }
            else
            {
                way_row["visible"] = way.Visible.Value ? 1 : 0;
            }

            DateTime? timestamp = way.TimeStamp;
            way_row["timestamp"] = timestamp.ConvertToDBValue<DateTime>();

            ulong? version = way.Version;
            way_row["version"] = version.ConvertToDBValue<ulong>();

            // set the usr
            way_row["usr"] = way.UserName;
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

        /// <summary>
        /// Adds a relation.
        /// </summary>
        /// <param name="relation"></param>
        public override void AddRelation(Relation relation)
        {
            DataRow relation_row = _relationTable.NewRow();

            // format data and create parameters.
            long? id = relation.Id.Value; // id should alrelations contain a value.
            relation_row["id"] = id.ConvertToDBValue<long>();

            long? changeset_id = relation.ChangeSetId;
            relation_row["changeset_id"] = changeset_id.ConvertToDBValue<long>();

            if (!relation.Visible.HasValue)
            {
                relation_row["visible"] = DBNull.Value;
            }
            else
            {
                relation_row["visible"] = relation.Visible.Value ? 1 : 0;
            }

            DateTime? timestamp = relation.TimeStamp;
            relation_row["timestamp"] = timestamp.ConvertToDBValue<DateTime>();

            ulong? version = relation.Version;
            relation_row["version"] = version.ConvertToDBValue<ulong>();

            // set the usr
            relation_row["usr"] = relation.UserName;
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

        /// <summary>
        /// Closes this target.
        /// </summary>
        public override void Close()
        {
            if (_connection != null)
            {
                if (!string.IsNullOrWhiteSpace(_connectionString))
                { // the connection was 
                    _connection.Close();
                    _connection.Dispose();
                }
            }
            _connection = null;
        }

        /// <summary>
        /// Flushes all data.
        /// </summary>
        public override void Flush()
        {
            if (_nodeTable.Rows.Count > 0)
            {
                this.BulkCopy(_nodeTable, "node");
                this.BulkCopy(_nodeTagsTable, "node_tags");
                this.CreateNodeTables();
            }

            if (_wayTable.Rows.Count > 0)
            {
                this.BulkCopy(_wayTable, "way");
                this.BulkCopy(_wayTagsTable, "way_tags");
                this.BulkCopy(_wayNodesTable, "way_nodes");
                this.CreateWayTables();
            }

            if (_relationTable.Rows.Count > 0)
            {
                this.BulkCopy(_relationTable, "relation");
                this.BulkCopy(_relationTagsTable, "relation_tags");
                this.BulkCopy(_relationMembersTable, "relation_members");
                this.CreateRelationTables();
            }
        }
    }
}