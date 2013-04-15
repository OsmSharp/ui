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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using OsmSharp.Osm.Simple;
using OsmSharp.Osm.Data.Core.Processor;

namespace OsmSharp.Osm.Data.Oracle.Raw.Processor
{
    /// <summary>
    /// A data processor source starting from an oracle database.
    /// </summary>
    public class OracleSimpleDataProcessorSource : DataProcessorSource
    {
        /// <summary>
        /// Holds the connection.
        /// </summary>
        private OracleConnection _connection;

        private SimpleOsmGeoType _current_type;

        private SimpleOsmGeo _current;

        private string _connection_string;

        /// <summary>
        /// Creates a new oracle simple source.
        /// </summary>
        /// <param name="connection_string"></param>
        public OracleSimpleDataProcessorSource(string connection_string)
        {
            _connection_string = connection_string;
        }

        /// <summary>
        /// Initialiers this oracle source.
        /// </summary>
        public override void Initialize()
        {
            _connection = new OracleConnection(_connection_string);
            _connection.Open();

            _current = null;
            _current_type = SimpleOsmGeoType.Node;

            _node_reader = null;
            _node_tag_reader = null;
        }

        private OracleDataReader _node_reader;
        private OracleDataReader _node_tag_reader;

        private OracleDataReader _way_reader;
        private OracleDataReader _way_tag_reader;
        private OracleDataReader _way_node_reader;

        private OracleDataReader _relation_reader;
        private OracleDataReader _relation_tag_reader;
        private OracleDataReader _relation_member_reader;

        /// <summary>
        /// Move to the next object.
        /// </summary>
        /// <returns></returns>
        public override bool MoveNext()
        {
            bool next = false;
            switch (_current_type)
            {
                case SimpleOsmGeoType.Node:
                    if (this.MoveNextNode())
                    {
                        return true;
                    }
                    return this.MoveNext();
                case SimpleOsmGeoType.Way:
                    if (this.MoveNextWay())
                    {
                        return true;
                    }
                    return this.MoveNext();
                case SimpleOsmGeoType.Relation:
                    return this.MoveNextRelation();
            }
            return next;
        }

        private bool MoveNextRelation()
        {
            if (_relation_reader == null)
            {
                OracleCommand relation_command = new OracleCommand("select * from relation order by id");
                relation_command.Connection = _connection;
                _relation_reader = relation_command.ExecuteReader();
                if (!_relation_reader.Read())
                {
                    _relation_reader.Close();
                }
                OracleCommand relation_tag_command = new OracleCommand("select * from relation_tags order by relation_id");
                relation_tag_command.Connection = _connection;
                _relation_tag_reader = relation_tag_command.ExecuteReader();
                if (!_relation_tag_reader.IsClosed && !_relation_tag_reader.Read())
                {
                    _relation_tag_reader.Close();
                }
                OracleCommand relation_node_command = new OracleCommand("select * from relation_members order by relation_id,sequence_id");
                relation_node_command.Connection = _connection;
                _relation_member_reader = relation_node_command.ExecuteReader();
                if (!_relation_member_reader.IsClosed && !_relation_member_reader.Read())
                {
                    _relation_member_reader.Close();
                }
            }

            // read next relation.
            if (!_relation_reader.IsClosed)
            {
                // load/parse data.
                long id = _relation_reader.GetInt64(0);
                long changeset_id = _relation_reader.GetInt64(1);
                DateTime timestamp = _relation_reader.GetDateTime(2);
                bool visible = _relation_reader.GetInt64(3) == 1;
                long version = _relation_reader.GetInt64(4);
                string user = _relation_reader.GetString(5);
                long uid = _relation_reader.GetInt64(6);
                SimpleRelation relation = new SimpleRelation();
                relation.Id = id;
                relation.ChangeSetId = changeset_id;
                relation.TimeStamp = timestamp;
                relation.UserId = null;
                relation.UserName = null;
                relation.Version = (ulong)version;
                relation.Visible = visible;
                relation.UserName = user;
                relation.UserId = uid;

                if (!_relation_tag_reader.IsClosed)
                {
                    long returned_id = _relation_tag_reader.GetInt64(0);
                    while (returned_id == relation.Id.Value)
                    {
                        if (relation.Tags == null)
                        {
                            relation.Tags = new Dictionary<string, string>();
                        }
                        string key = _relation_tag_reader.GetString(1);
                        string value = _relation_tag_reader.GetString(2);

                        relation.Tags.Add(key, value);

                        if (!_relation_tag_reader.Read())
                        {
                            _relation_tag_reader.Close();
                            returned_id = -1;
                        }
                        else
                        {
                            returned_id = _relation_tag_reader.GetInt64(0);
                        }
                    }
                }
                if (!_relation_member_reader.IsClosed)
                {
                    long returned_id = _relation_member_reader.GetInt64(0);
                    while (returned_id == relation.Id.Value)
                    {
                        if (relation.Members == null)
                        {
                            relation.Members = new List<SimpleRelationMember>();
                        }
                        string member_type = _relation_member_reader.GetString(1);
                        long member_id = _relation_member_reader.GetInt64(2);
                        object member_role = _relation_member_reader.GetValue(3);

                        SimpleRelationMember member = new SimpleRelationMember();
                        member.MemberId = member_id;
                        if (member_role != DBNull.Value)
                        {
                            member.MemberRole = member_role as string;
                        }
                        switch (member_type)
                        {
                            case "Node":
                                member.MemberType = SimpleRelationMemberType.Node;
                                break;
                            case "Way":
                                member.MemberType = SimpleRelationMemberType.Way;
                                break;
                            case "Relation":
                                member.MemberType = SimpleRelationMemberType.Relation;
                                break;
                        }

                        relation.Members.Add(member);

                        if (!_relation_member_reader.Read())
                        {
                            _relation_member_reader.Close();
                            returned_id = -1;
                        }
                        else
                        {
                            returned_id = _relation_member_reader.GetInt64(0);
                        }
                    }
                }

                // set the current variable!
                _current = relation;

                // advance the reader(s).
                if (!_relation_reader.Read())
                {
                    _relation_reader.Close();
                }
                if (!_relation_tag_reader.IsClosed && !_relation_tag_reader.Read())
                {
                    _relation_tag_reader.Close();
                }
                if (!_relation_member_reader.IsClosed && !_relation_member_reader.Read())
                {
                    _relation_member_reader.Close();
                }
                return true;
            }
            else
            {
                _relation_reader.Close();
                _relation_reader.Dispose();
                _relation_reader = null;

                _relation_tag_reader.Close();
                _relation_tag_reader.Dispose();
                _relation_tag_reader = null;

                _current_type = SimpleOsmGeoType.Relation;

                return false;
            }
        }

        private bool MoveNextNode()
        {
            if (_node_reader == null)
            {
                OracleCommand node_command = new OracleCommand("select * from node order by id");
                node_command.Connection = _connection;
                _node_reader = node_command.ExecuteReader();
                if (!_node_reader.Read())
                {
                    _node_reader.Close();
                }
                OracleCommand node_tag_command = new OracleCommand("select * from node_tags order by node_id");
                node_tag_command.Connection = _connection;
                _node_tag_reader = node_tag_command.ExecuteReader();
                if (!_node_tag_reader.Read())
                {
                    _node_tag_reader.Close();
                }
            }

            // read next node.
            if (!_node_reader.IsClosed)
            {
                // load/parse data.
                long id = _node_reader.GetInt64(0);
                long latitude_int = _node_reader.GetInt64(1);
                long longitude_int = _node_reader.GetInt64(2);
                long changeset_id = _node_reader.GetInt64(3);
                bool visible = _node_reader.GetInt64(4) == 1;
                DateTime timestamp = _node_reader.GetDateTime(5);
                long tile = _node_reader.GetInt64(6);
                long version = _node_reader.GetInt64(7);
                string user = _node_reader.GetString(8);
                long uid = _node_reader.GetInt64(9);
                SimpleNode node = new SimpleNode();
                node.Id = id;
                node.Latitude = latitude_int;
                node.Longitude = longitude_int;
                node.ChangeSetId = changeset_id;
                node.TimeStamp = timestamp;
                node.UserId = null;
                node.UserName = null;
                node.Version  = (ulong)version;
                node.Visible = visible;
                node.UserName = user;
                node.UserId = uid;

                if (!_node_tag_reader.IsClosed)
                {
                    long returned_id = _node_tag_reader.GetInt64(0);
                    while (returned_id == node.Id.Value)
                    {
                        if (node.Tags == null)
                        {
                            node.Tags = new Dictionary<string, string>();
                        }
                        string key = _node_tag_reader.GetString(1);
                        string value = _node_tag_reader.GetString(2);

                        node.Tags.Add(key, value);

                        if (!_node_tag_reader.Read())
                        {
                            _node_tag_reader.Close();
                        }
                        returned_id = _node_tag_reader.GetInt64(0);
                    }
                }

                // set the current variable!
                _current = node;

                // advance the reader(s).
                if (!_node_reader.Read())
                {
                    _node_reader.Close();
                }
                if (!_node_tag_reader.IsClosed && !_node_tag_reader.Read())
                {
                    _node_tag_reader.Close();
                }
                return true;
            }
            else
            {
                _node_reader.Close();
                _node_reader.Dispose();
                _node_reader = null;

                _node_tag_reader.Close();
                _node_tag_reader.Dispose();
                _node_tag_reader = null;

                _current_type = SimpleOsmGeoType.Way;

                return false;
            }
        }

        private bool MoveNextWay()
        {
            if (_way_reader == null)
            {
                OracleCommand way_command = new OracleCommand("select * from way order by id");
                way_command.Connection = _connection;
                _way_reader = way_command.ExecuteReader();
                if (!_way_reader.Read())
                {
                    _way_reader.Close();
                }
                OracleCommand way_tag_command = new OracleCommand("select * from way_tags order by way_id");
                way_tag_command.Connection = _connection;
                _way_tag_reader = way_tag_command.ExecuteReader();
                if (!_way_tag_reader.IsClosed && !_way_tag_reader.Read())
                {
                    _way_tag_reader.Close();
                }
                OracleCommand way_node_command = new OracleCommand("select * from way_nodes order by way_id,sequence_id");
                way_node_command.Connection = _connection;
                _way_node_reader = way_node_command.ExecuteReader();
                if (!_way_node_reader.IsClosed && !_way_node_reader.Read())
                {
                    _way_node_reader.Close();
                }
            }

            // read next way.
            if (!_way_reader.IsClosed)
            {
                // load/parse data.
                long id = _way_reader.GetInt64(0);
                long changeset_id = _way_reader.GetInt64(1);
                DateTime timestamp = _way_reader.GetDateTime(2);
                bool visible = _way_reader.GetInt64(3) == 1;
                long version = _way_reader.GetInt64(4);
                string user = _way_reader.GetString(5);
                long uid = _way_reader.GetInt64(6);

                SimpleWay way = new SimpleWay();
                way.Id = id;
                way.ChangeSetId = changeset_id;
                way.TimeStamp = timestamp;
                way.UserId = uid;
                way.UserName = user;
                way.Version = (ulong)version;
                way.Visible = visible;

                if (!_way_tag_reader.IsClosed)
                {
                    long returned_id = _way_tag_reader.GetInt64(_way_tag_reader.GetOrdinal("way_id"));
                    while (returned_id == way.Id.Value)
                    {
                        if (way.Tags == null)
                        {
                            way.Tags = new Dictionary<string, string>();
                        }
                        string key = _way_tag_reader.GetString(1);
                        string value = _way_tag_reader.GetString(2);

                        way.Tags.Add(key, value);

                        if (!_way_tag_reader.Read())
                        {
                            _way_tag_reader.Close();
                            returned_id = -1;
                        }
                        else
                        {
                            returned_id = _way_tag_reader.GetInt64(0);
                        }
                    }
                }
                if (!_way_node_reader.IsClosed)
                {
                    long returned_id = _way_node_reader.GetInt64(_way_node_reader.GetOrdinal("way_id"));
                    while (returned_id == way.Id.Value)
                    {
                        if (way.Nodes == null)
                        {
                            way.Nodes = new List<long>();
                        }
                        long node_id = _way_node_reader.GetInt64(1);

                        way.Nodes.Add(node_id);

                        if (!_way_node_reader.Read())
                        {
                            _way_node_reader.Close();
                            returned_id = -1;
                        }
                        else
                        {
                            returned_id = _way_node_reader.GetInt64(0);
                        }
                    }
                }

                // set the current variable!
                _current = way;

                // advance the reader(s).
                if (!_way_reader.Read())
                {
                    _way_reader.Close();
                }
                if (!_way_tag_reader.IsClosed && !_way_tag_reader.Read())
                {
                    _way_tag_reader.Close();
                }
                if (!_way_node_reader.IsClosed && !_way_node_reader.Read())
                {
                    _way_node_reader.Close();
                }
                return true;
            }
            else
            {
                _way_reader.Close();
                _way_reader.Dispose();
                _way_reader = null;

                _way_tag_reader.Close();
                _way_tag_reader.Dispose();
                _way_tag_reader = null;

                _current_type = SimpleOsmGeoType.Relation;

                return false;
            }
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override SimpleOsmGeo Current()
        {
            return _current;
        }

        /// <summary>
        /// Resets the current source.
        /// </summary>
        public override void Reset()
        {
            _current = null;
            _current_type = SimpleOsmGeoType.Node;

            if (_node_reader != null)
            {
                _node_reader.Close();
                _node_reader.Dispose();
                _node_reader = null;
            }
            if (_node_tag_reader != null)
            {
                _node_tag_reader.Close();
                _node_tag_reader.Dispose();
                _node_tag_reader = null;
            }
        }

        /// <summary>
        /// Returns true if this source can be reset.
        /// </summary>
        public override bool CanReset
        {
            get { return true; }
        }
    }
}
