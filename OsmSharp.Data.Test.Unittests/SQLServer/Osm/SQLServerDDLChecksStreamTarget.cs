// OsmSharp - OpenStreetMap (OSM) SDK
//
// Copyright (C) 2013 Abelshausen Ben
//                    Alexander Sinitsyn
//                    Simon Hughes
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

using System.Data.SqlClient;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm;
using OsmSharp.Osm.Streams;

namespace OsmSharp.Data.Test.Unittests.SQLServer
{
    /// <summary>
    /// Helper stream target class for SQLServer.
    /// </summary>
    internal class SQLServerDDLChecksStreamTarget : OsmStreamTarget
    {
        private SqlConnection _connection;
        private readonly string _connectionString;

        public int NodeUsr;
        public int NodeTagsKey;
        public int NodeTagsValue;

        public int WayUsr;
        public int WayTagsKey;
        public int WayTagsValue;

        public int RelationUsr;
        public int RelationTagsKey;
        public int RelationTagsValue;
        public int RelationMemberRole;

        public SQLServerDDLChecksStreamTarget(string connectionString)
        {
            _connectionString = connectionString;
        }
        public override void Initialize()
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
        }

        public override void AddNode(Node node)
        {
            if (!node.Id.HasValue)
                return;

            MaxStringLength(node.UserName, ref NodeUsr);

            if (node.Tags == null)
                return;

            foreach (Tag tag in node.Tags)
            {
                MaxStringLength(tag.Key, ref NodeTagsKey);
                MaxStringLength(tag.Value, ref NodeTagsValue);
            }
        }

        public override void AddWay(Way way)
        {
            if (!way.Id.HasValue)
                return;

            MaxStringLength(way.UserName, ref WayUsr);

            if (way.Tags == null)
                return;

            foreach (Tag tag in way.Tags)
            {
                MaxStringLength(tag.Key, ref WayTagsKey);
                MaxStringLength(tag.Value, ref WayTagsValue);
            }
        }

        public override void AddRelation(Relation relation)
        {
            if (!relation.Id.HasValue)
                return;

            MaxStringLength(relation.UserName, ref RelationUsr);

            if (relation.Tags != null)
            {
                foreach (Tag tag in relation.Tags)
                {
                    MaxStringLength(tag.Key, ref RelationTagsKey);
                    MaxStringLength(tag.Value, ref RelationTagsValue);
                }
            }

            if (relation.Members != null)
            {
                foreach (RelationMember member in relation.Members)
                {
                    MaxStringLength(member.MemberRole, ref RelationMemberRole);
                }
            }
        }

        private static void MaxStringLength(string value, ref int length)
        {
            if (value == null)
                return;

            int len = value.Length;
            if (length < len)
                length = len;
        }

        public override void Close()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
            _connection = null;
        }
    }
}
