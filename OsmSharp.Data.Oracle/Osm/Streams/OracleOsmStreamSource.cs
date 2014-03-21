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

using Oracle.ManagedDataAccess.Client;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm.Streams;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Data.Oracle.Osm.Streams
{
    /// <summary>
    /// An OSM stream source reading data from an Oracle database.
    /// </summary>
    public class OracleOsmStreamSource : OsmStreamSource
    {
        /// <summary>
        /// Holds the connection.
        /// </summary>
        private OracleConnection _connection;

        /// <summary>
        /// Holds the current type.
        /// </summary>
        private OsmGeoType _currentType;

        /// <summary>
        /// Holds the current OsmGeo-object.
        /// </summary>
        private OsmGeo _current;

        /// <summary>
        /// Holds the connection string.
        /// </summary>
        private string _connectionString;

        /// <summary>
        /// Creates a new oracle simple source.
        /// </summary>
        /// <param name="connectionString"></param>
        public OracleOsmStreamSource(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Initialiers this oracle source.
        /// </summary>
        public override void Initialize()
        {
            _connection = new OracleConnection(_connectionString);
            _connection.Open();

            _current = null;
            _currentType = OsmGeoType.Node;

            _nodeReader = null;
            _nodeTagReader = null;
        }

        private OracleDataReader _nodeReader;
        private OracleDataReader _nodeTagReader;

        private OracleDataReader _wayReader;
        private OracleDataReader _wayTagReader;
        private OracleDataReader _wayNodeReader;

        private OracleDataReader _relationReader;
        private OracleDataReader _relationTagReader;
        private OracleDataReader _relationMemberReader;

        /// <summary>
        /// Move to the next item in the stream.
        /// </summary>
        /// <param name="ignoreNodes">Makes this source skip all nodes.</param>
        /// <param name="ignoreWays">Makes this source skip all ways.</param>
        /// <param name="ignoreRelations">Makes this source skip all relations.</param>
        /// <returns></returns>
        public override bool MoveNext(bool ignoreNodes, bool ignoreWays, bool ignoreRelations)
        {
            bool next = false;
            switch (_currentType)
            {
                case OsmGeoType.Node:
                    while (this.DoMoveNextNode())
                    {
                        if(!ignoreNodes)
                        {
                            return true;
                        }
                    }
                    return this.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
                case OsmGeoType.Way:
                    if (this.DoMoveNextWay())
                    {
                        if (!ignoreWays)
                        {
                            return true;
                        }
                    }
                    return this.MoveNext(ignoreNodes, ignoreWays, ignoreRelations);
                case OsmGeoType.Relation:
                    if(ignoreRelations)
                    {
                        return false;
                    }
                    return this.DoMoveNextRelation();
            }
            return next;
        }

        /// <summary>
        /// Returns true if this source is sorted.
        /// </summary>
        public override bool IsSorted
        {
            get
            {
                return true;
            }
        }

        private bool DoMoveNextRelation()
        {
            if (_relationReader == null)
            {
                OracleCommand relation_command = new OracleCommand("select * from relation order by id");
                relation_command.Connection = _connection;
                _relationReader = relation_command.ExecuteReader();
                if (!_relationReader.Read())
                {
                    _relationReader.Close();
                }
                OracleCommand relation_tag_command = new OracleCommand("select * from relation_tags order by relation_id");
                relation_tag_command.Connection = _connection;
                _relationTagReader = relation_tag_command.ExecuteReader();
                if (!_relationTagReader.IsClosed && !_relationTagReader.Read())
                {
                    _relationTagReader.Close();
                }
                OracleCommand relation_node_command = new OracleCommand("select * from relation_members order by relation_id,sequence_id");
                relation_node_command.Connection = _connection;
                _relationMemberReader = relation_node_command.ExecuteReader();
                if (!_relationMemberReader.IsClosed && !_relationMemberReader.Read())
                {
                    _relationMemberReader.Close();
                }
            }

            // read next relation.
            if (!_relationReader.IsClosed)
            {
                // load/parse data.
                long id = _relationReader.GetInt64(0);
                long changeset_id = _relationReader.GetInt64(1);
                DateTime timestamp = _relationReader.GetDateTime(2);
                bool visible = _relationReader.GetInt64(3) == 1;
                long version = _relationReader.GetInt64(4);
                string user = _relationReader.GetString(5);
                long uid = _relationReader.GetInt64(6);
                Relation relation = new Relation();
                relation.Id = id;
                relation.ChangeSetId = changeset_id;
                relation.TimeStamp = timestamp;
                relation.UserId = null;
                relation.UserName = null;
                relation.Version = (ulong)version;
                relation.Visible = visible;
                relation.UserName = user;
                relation.UserId = uid;

                if (!_relationTagReader.IsClosed)
                {
                    long returned_id = _relationTagReader.GetInt64(0);
                    while (returned_id == relation.Id.Value)
                    {
                        if (relation.Tags == null)
                        {
                            relation.Tags = new TagsCollection();
                        }
                        string key = _relationTagReader.GetString(1);
                        string value = _relationTagReader.GetString(2);

                        relation.Tags.Add(key, value);

                        if (!_relationTagReader.Read())
                        {
                            _relationTagReader.Close();
                            returned_id = -1;
                        }
                        else
                        {
                            returned_id = _relationTagReader.GetInt64(0);
                        }
                    }
                }
                if (!_relationMemberReader.IsClosed)
                {
                    long returned_id = _relationMemberReader.GetInt64(0);
                    while (returned_id == relation.Id.Value)
                    {
                        if (relation.Members == null)
                        {
                            relation.Members = new List<RelationMember>();
                        }
                        string member_type = _relationMemberReader.GetString(1);
                        long member_id = _relationMemberReader.GetInt64(2);
                        object member_role = _relationMemberReader.GetValue(3);

                        RelationMember member = new RelationMember();
                        member.MemberId = member_id;
                        if (member_role != DBNull.Value)
                        {
                            member.MemberRole = member_role as string;
                        }
                        switch (member_type)
                        {
                            case "Node":
                                member.MemberType = OsmGeoType.Node;
                                break;
                            case "Way":
                                member.MemberType = OsmGeoType.Way;
                                break;
                            case "Relation":
                                member.MemberType = OsmGeoType.Relation;
                                break;
                        }

                        relation.Members.Add(member);

                        if (!_relationMemberReader.Read())
                        {
                            _relationMemberReader.Close();
                            returned_id = -1;
                        }
                        else
                        {
                            returned_id = _relationMemberReader.GetInt64(0);
                        }
                    }
                }

                // set the current variable!
                _current = relation;

                // advance the reader(s).
                if (!_relationReader.Read())
                {
                    _relationReader.Close();
                }
                if (!_relationTagReader.IsClosed && !_relationTagReader.Read())
                {
                    _relationTagReader.Close();
                }
                if (!_relationMemberReader.IsClosed && !_relationMemberReader.Read())
                {
                    _relationMemberReader.Close();
                }
                return true;
            }
            else
            {
                _relationReader.Close();
                _relationReader.Dispose();
                _relationReader = null;

                _relationTagReader.Close();
                _relationTagReader.Dispose();
                _relationTagReader = null;

                _currentType = OsmGeoType.Relation;

                return false;
            }
        }

        private bool DoMoveNextNode()
        {
            if (_nodeReader == null)
            {
                OracleCommand node_command = new OracleCommand("select * from node order by id");
                node_command.Connection = _connection;
                _nodeReader = node_command.ExecuteReader();
                if (!_nodeReader.Read())
                {
                    _nodeReader.Close();
                }
                OracleCommand node_tag_command = new OracleCommand("select * from node_tags order by node_id");
                node_tag_command.Connection = _connection;
                _nodeTagReader = node_tag_command.ExecuteReader();
                if (!_nodeTagReader.Read())
                {
                    _nodeTagReader.Close();
                }
            }

            // read next node.
            if (!_nodeReader.IsClosed)
            {
                // load/parse data.
                long id = _nodeReader.GetInt64(0);
                long latitude_int = _nodeReader.GetInt64(1);
                long longitude_int = _nodeReader.GetInt64(2);
                long changeset_id = _nodeReader.GetInt64(3);
                bool visible = _nodeReader.GetInt64(4) == 1;
                DateTime timestamp = _nodeReader.GetDateTime(5);
                long tile = _nodeReader.GetInt64(6);
                long version = _nodeReader.GetInt64(7);
                string user = _nodeReader.GetString(8);
                long uid = _nodeReader.GetInt64(9);
                Node node = new Node();
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

                if (!_nodeTagReader.IsClosed)
                {
                    long returned_id = _nodeTagReader.GetInt64(0);
                    while (returned_id == node.Id.Value)
                    {
                        if (node.Tags == null)
                        {
                            node.Tags = new TagsCollection();
                        }
                        string key = _nodeTagReader.GetString(1);
                        string value = _nodeTagReader.GetString(2);

                        node.Tags.Add(key, value);

                        if (!_nodeTagReader.Read())
                        {
                            _nodeTagReader.Close();
                        }
                        returned_id = _nodeTagReader.GetInt64(0);
                    }
                }

                // set the current variable!
                _current = node;

                // advance the reader(s).
                if (!_nodeReader.Read())
                {
                    _nodeReader.Close();
                }
                if (!_nodeTagReader.IsClosed && !_nodeTagReader.Read())
                {
                    _nodeTagReader.Close();
                }
                return true;
            }
            else
            {
                _nodeReader.Close();
                _nodeReader.Dispose();
                _nodeReader = null;

                _nodeTagReader.Close();
                _nodeTagReader.Dispose();
                _nodeTagReader = null;

                _currentType = OsmGeoType.Way;

                return false;
            }
        }

        private bool DoMoveNextWay()
        {
            if (_wayReader == null)
            {
                OracleCommand way_command = new OracleCommand("select * from way order by id");
                way_command.Connection = _connection;
                _wayReader = way_command.ExecuteReader();
                if (!_wayReader.Read())
                {
                    _wayReader.Close();
                }
                OracleCommand way_tag_command = new OracleCommand("select * from way_tags order by way_id");
                way_tag_command.Connection = _connection;
                _wayTagReader = way_tag_command.ExecuteReader();
                if (!_wayTagReader.IsClosed && !_wayTagReader.Read())
                {
                    _wayTagReader.Close();
                }
                OracleCommand way_node_command = new OracleCommand("select * from way_nodes order by way_id,sequence_id");
                way_node_command.Connection = _connection;
                _wayNodeReader = way_node_command.ExecuteReader();
                if (!_wayNodeReader.IsClosed && !_wayNodeReader.Read())
                {
                    _wayNodeReader.Close();
                }
            }

            // read next way.
            if (!_wayReader.IsClosed)
            {
                // load/parse data.
                long id = _wayReader.GetInt64(0);
                long changeset_id = _wayReader.GetInt64(1);
                DateTime timestamp = _wayReader.GetDateTime(2);
                bool visible = _wayReader.GetInt64(3) == 1;
                long version = _wayReader.GetInt64(4);
                string user = _wayReader.GetString(5);
                long uid = _wayReader.GetInt64(6);

                Way way = new Way();
                way.Id = id;
                way.ChangeSetId = changeset_id;
                way.TimeStamp = timestamp;
                way.UserId = uid;
                way.UserName = user;
                way.Version = (ulong)version;
                way.Visible = visible;

                if (!_wayTagReader.IsClosed)
                {
                    long returned_id = _wayTagReader.GetInt64(_wayTagReader.GetOrdinal("way_id"));
                    while (returned_id == way.Id.Value)
                    {
                        if (way.Tags == null)
                        {
                            way.Tags = new TagsCollection();
                        }
                        string key = _wayTagReader.GetString(1);
                        string value = _wayTagReader.GetString(2);

                        way.Tags.Add(key, value);

                        if (!_wayTagReader.Read())
                        {
                            _wayTagReader.Close();
                            returned_id = -1;
                        }
                        else
                        {
                            returned_id = _wayTagReader.GetInt64(0);
                        }
                    }
                }
                if (!_wayNodeReader.IsClosed)
                {
                    long returned_id = _wayNodeReader.GetInt64(_wayNodeReader.GetOrdinal("way_id"));
                    while (returned_id == way.Id.Value)
                    {
                        if (way.Nodes == null)
                        {
                            way.Nodes = new List<long>();
                        }
                        long node_id = _wayNodeReader.GetInt64(1);

                        way.Nodes.Add(node_id);

                        if (!_wayNodeReader.Read())
                        {
                            _wayNodeReader.Close();
                            returned_id = -1;
                        }
                        else
                        {
                            returned_id = _wayNodeReader.GetInt64(0);
                        }
                    }
                }

                // set the current variable!
                _current = way;

                // advance the reader(s).
                if (!_wayReader.Read())
                {
                    _wayReader.Close();
                }
                if (!_wayTagReader.IsClosed && !_wayTagReader.Read())
                {
                    _wayTagReader.Close();
                }
                if (!_wayNodeReader.IsClosed && !_wayNodeReader.Read())
                {
                    _wayNodeReader.Close();
                }
                return true;
            }
            else
            {
                _wayReader.Close();
                _wayReader.Dispose();
                _wayReader = null;

                _wayTagReader.Close();
                _wayTagReader.Dispose();
                _wayTagReader = null;

                _currentType = OsmGeoType.Relation;

                return false;
            }
        }

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override OsmGeo Current()
        {
            return _current;
        }

        /// <summary>
        /// Resets the current source.
        /// </summary>
        public override void Reset()
        {
            _current = null;
            _currentType = OsmGeoType.Node;

            if (_nodeReader != null)
            {
                _nodeReader.Close();
                _nodeReader.Dispose();
                _nodeReader = null;
            }
            if (_nodeTagReader != null)
            {
                _nodeTagReader.Close();
                _nodeTagReader.Dispose();
                _nodeTagReader = null;
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
