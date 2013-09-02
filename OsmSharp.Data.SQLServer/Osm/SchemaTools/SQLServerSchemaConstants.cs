// OsmSharp - OpenStreetMap (OSM) SDK
//
// Copyright (C) 2013 Simon Hughes
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

namespace OsmSharp.Data.SQLServer.Osm.SchemaTools
{
    /// <summary>
    /// string length constants that match the SQL DDL schema.
    /// </summary>
    public static class SQLServerSchemaConstants
    {
        /// <summary>
        /// dbo.node.usr
        /// </summary>
        public const int NodeUsr = 100;

        /// <summary>
        /// dbo.node_tags.key
        /// </summary>
        public const int NodeTagsKey = 100;

        /// <summary>
        /// dbo.node_tags.value
        /// </summary>
        public const int NodeTagsValue = 500;

        /// <summary>
        /// dbo.way.usr
        /// </summary>
        public const int WayUsr = 100;

        /// <summary>
        /// dbo.way_tags.key
        /// </summary>
        public const int WayTagsKey = 255;

        /// <summary>
        /// dbo.way_tags.value
        /// </summary>
        public const int WayTagsValue = 500;

        /// <summary>
        /// dbo.relation.usr
        /// </summary>
        public const int RelationUsr = 100;

        /// <summary>
        /// dbo.relation_tags.key
        /// </summary>
        public const int RelationTagsKey = 100;

        /// <summary>
        /// dbo.relation_tags.value
        /// </summary>
        public const int RelationTagsValue = 500;

        /// <summary>
        /// dbo.relation_members.member_type
        /// </summary>
        public const int RelationMemberType = 100;

        /// <summary>
        /// dbo.relation_members.member_role
        /// </summary>
        public const int RelationMemberRole = 100;
    }
}