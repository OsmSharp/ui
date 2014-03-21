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
using System.Data.SQLite;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm;
using OsmSharp.Osm.Streams;

namespace OsmSharp.Data.SQLite.Osm.Streams
{
    /// <summary>
    /// An SQLite data processor source.
    /// </summary>
    public class SQLiteOsmStreamSource : OsmStreamSource
    {
        /// <summary>
        /// Holds the connection.
        /// </summary>
        private SQLiteConnection _connection;

        /// <summary>
        /// Holds the current type.
        /// </summary>
        private OsmGeoType _currentType;

        /// <summary>
        /// Holds the current object.
        /// </summary>
        private OsmGeo _current;

        /// <summary>
        /// Holds the connection string.
        /// </summary>
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new SQLite data processor source.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SQLiteOsmStreamSource(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Initializes this source.
        /// </summary>
        public override void Initialize()
        {
            _connection = new SQLiteConnection(_connectionString);
            _connection.Open();

            _current = null;
            _currentType = OsmGeoType.Node;

            _nodeReader = null;
        }

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
                        if (!ignoreNodes)
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
                    if (ignoreRelations)
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

        #region MoveNext fuctions

        private SQLiteDataReader _nodeReader;
        private SQLiteDataReader _wayReader;
        private SQLiteDataReader _wayTagReader;
        private SQLiteDataReader _wayNodeReader;
        private SQLiteDataReader _relationReader;
        private SQLiteDataReader _relationTagReader;
        private SQLiteDataReader _relationMemberReader;

        private bool DoMoveNextRelation()
        {
            if (_relationReader == null)
            {
                var relationCommand = new SQLiteCommand("select * from relation order by id", _connection);
                _relationReader = relationCommand.ExecuteReader();
                if (!_relationReader.Read())
                {
                    _relationReader.Close();
                }
                var relationTagCommand = new SQLiteCommand("select * from relation_tags order by relation_id", _connection);
                _relationTagReader = relationTagCommand.ExecuteReader();
                if (!_relationTagReader.IsClosed && !_relationTagReader.Read())
                {
                    _relationTagReader.Close();
                }
                var relationNodeCommand = new SQLiteCommand("select * from relation_members order by relation_id,sequence_id", _connection);
                _relationMemberReader = relationNodeCommand.ExecuteReader();
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
                long changesetId = _relationReader.GetInt64(1);
				bool visible = _relationReader.GetInt64(2) == 1;
				DateTime timestamp = _relationReader.IsDBNull(3)? DateTime.MinValue : _relationReader.GetDateTime(3);
                long version = _relationReader.GetInt64(4);
                string user = _relationReader.GetString(5);
                long uid = _relationReader.GetInt64(6);
                var relation = new Relation
                                   {
                                       Id = id,
                                       ChangeSetId = changesetId,
                                       TimeStamp = timestamp,
                                       UserId = null,
                                       UserName = null,
                                       Version = (ulong) version,
                                       Visible = visible
                                   };
                relation.UserName = user;
                relation.UserId = uid;

                if (!_relationTagReader.IsClosed)
                {
                    long returnedId = _relationTagReader.GetInt64(0);
                    while (returnedId == relation.Id.Value)
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
                            returnedId = -1;
                        }
                        else
                        {
                            returnedId = _relationTagReader.GetInt64(0);
                        }
                    }
                }
                if (!_relationMemberReader.IsClosed)
                {
                    long returnedId = _relationMemberReader.GetInt64(0);
                    while (returnedId == relation.Id.Value)
                    {
                        if (relation.Members == null)
                        {
                            relation.Members = new List<RelationMember>();
                        }
                        string memberType = _relationMemberReader.GetString(1);
                        long memberId = _relationMemberReader.GetInt64(2);
                        object memberRole = _relationMemberReader.GetValue(3);

                        var member = new RelationMember();
                        member.MemberId = memberId;
                        if (memberRole != DBNull.Value)
                        {
                            member.MemberRole = memberRole as string;
                        }
                        switch (memberType)
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
                            returnedId = -1;
                        }
                        else
                        {
                            returnedId = _relationMemberReader.GetInt64(0);
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
        		SQLiteCommand node_command = new SQLiteCommand("select * from node left join node_tags on node_tags.node_id = node.id order by node.id");
        		node_command.Connection = _connection;
        		_nodeReader = node_command.ExecuteReader();
        		if (!_nodeReader.Read())
        			_nodeReader.Close();
        	}

        	// read next node.
        	if (!_nodeReader.IsClosed)
        	{
        		// load/parse data.
        		Node node = new Node();
        		node.Id = _nodeReader.GetInt64(0);
        		node.Latitude = _nodeReader.GetInt64(1)/10000000.0;
        		node.Longitude = _nodeReader.GetInt64(2)/10000000.0;
        		node.ChangeSetId = _nodeReader.GetInt64(3);
        		node.TimeStamp = _nodeReader.GetDateTime(5);
        		node.Version = (ulong) _nodeReader.GetInt64(7);
        		node.Visible = _nodeReader.GetInt64(4) == 1;
        		//node.UserName = _node_reader.GetString(8);
        		//node.UserId = _node_reader.IsDBNull(9) ? -1 : _node_reader.GetInt64(9);

						//Has tags?
        		if (!_nodeReader.IsDBNull(10))
        		{
							//if (node.Tags == null)
								//node.Tags = new Dictionary<string, string>();
							
							long currentnode = node.Id.Value;
							while(currentnode == node.Id.Value){
								//string key = _node_reader.GetString(11);
								//string value = _node_reader.GetString(12);
								//node.Tags.Add(key, value);
								if (!_nodeReader.Read())
								{
									_nodeReader.Close();
									break;
								}
								currentnode = _nodeReader.GetInt64(0);
							}
        		}else if (!_nodeReader.Read())
        			_nodeReader.Close();
						// set the current variable!
        		_current = node;
        		return true;
        	}
        	_nodeReader.Close();
        	_nodeReader.Dispose();
        	_nodeReader = null;
        	_currentType = OsmGeoType.Way;
        	return false;
        }

    	private bool DoMoveNextWay()
        {
					if (_wayReader == null)
					{
						SQLiteCommand way_command = new SQLiteCommand("select * from way where id > 26478817 order by id");
						way_command.Connection = _connection;
						_wayReader = way_command.ExecuteReader();
						if (!_wayReader.Read())
						{
							_wayReader.Close();
						}
						SQLiteCommand way_tag_command = new SQLiteCommand("select * from way_tags where way_id > 26478817 order by way_id");
						way_tag_command.Connection = _connection;
						_wayTagReader = way_tag_command.ExecuteReader();
						if (!_wayTagReader.IsClosed && !_wayTagReader.Read())
						{
							_wayTagReader.Close();
						}
						SQLiteCommand way_node_command = new SQLiteCommand("select * from way_nodes where way_id > 26478817 order by way_id,sequence_id");
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

                Way way = new Way();
								way.Id = _wayReader.GetInt64(0);
								way.ChangeSetId = _wayReader.GetInt64(1);
								way.TimeStamp = _wayReader.IsDBNull(3) ? DateTime.MinValue : _wayReader.GetDateTime(3);
								//way.UserId = _way_reader.GetInt64(6);
								//way.UserName = _way_reader.GetString(5);
								way.Version = (ulong)_wayReader.GetInt64(4);
								way.Visible = _wayReader.GetInt64(2) == 1;

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

        #endregion

        /// <summary>
        /// Returns the current object.
        /// </summary>
        /// <returns></returns>
        public override OsmGeo Current()
        {
            return _current;
        }

        /// <summary>
        /// Resets this source.
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
        }

        /// <summary>
        /// Returns a value that indicates if this source can be reset or not.
        /// </summary>
        public override bool CanReset
        {
            get { return true; }
        }
    }
}
