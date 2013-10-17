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
using Oracle.ManagedDataAccess.Client;

namespace OsmSharp.Data.Oracle.Osm
{
    /// <summary>
    /// Tools for creation/detection of the schema in Oracle.
    /// </summary>
    public static class OracleSchemaTools
    {
        /// <summary>
        /// SQL to detect the existence of a table.
        /// </summary>
        private const string TABLE_DETECTION_SQL = "select count(tname) from tab where tname = upper(:table_name)";

        /// <summary>
        /// SQL to create the nodes table.
        /// </summary>
        private const string TABLE_NODE_CREATION = "CREATE TABLE node " +
                                                    "(  " +
                                                    "    id            NUMBER(11) NOT NULL, " +
                                                    "    latitude      NUMBER(11) NOT NULL, " +
                                                    "    longitude     NUMBER(11) NOT NULL, " +
                                                    "    changeset_id  NUMBER(11) NULL, " +
                                                    "    visible       NUMBER(1) NULL, " +
                                                    "    timestamp     DATE NULL, " +
                                                    "    tile          NUMBER(11) NOT NULL, " +
                                                    "    version       NUMBER(11) NULL, " +
                                                    "    usr           VARCHAR2(255), " +
                                                    "    usr_id        NUMBER(11) " +
                                                    ")";

        /// <summary>
        /// SQL to drop the nodes table.
        /// </summary>
        private const string TABLE_NODE_DROP = "DROP TABLE node";

        /// <summary>
        /// SQL to create the node tags table.
        /// </summary>
        private const string TABLE_NODE_TAGS_CREATION = "CREATE TABLE node_tags " +
                                                        "( " +
                                                        "	node_id  NUMBER(11) NOT NULL, " +
                                                        "	key      VARCHAR2(255) NOT NULL, " +
                                                        "	value    VARCHAR2(255) " +
                                                        ")";

        /// <summary>
        /// SQL to drop the node_tags table.
        /// </summary>
        private const string TABLE_NODE_TAGS_DROP = "DROP TABLE node_tags";

        /// <summary>
        /// SQL to create the way table.
        /// </summary>
        private const string TABLE_WAY_CREATION = "CREATE TABLE way " +
                                                    "( " +
                                                    "	id            NUMBER(11) NOT NULL, " +
                                                    "	changeset_id  NUMBER(11) NULL, " +
                                                    "	timestamp     DATE NULL, " +
                                                    "	visible       NUMBER(1) NULL, " +
                                                    "	version       NUMBER(11) NULL, " +
                                                    "	usr           VARCHAR2(255), " +
                                                    "	usr_id        NUMBER(11) " +
                                                    ")";

        /// <summary>
        /// SQL to drop the ways table.
        /// </summary>
        private const string TABLE_WAY_DROP = "DROP TABLE way";

        /// <summary>
        /// SQL to create way nodes table.
        /// </summary>
        private const string TABLE_WAY_NODES_CREATION = "CREATE TABLE way_nodes " +
                                                        "( " +
                                                        "    way_id       NUMBER(11) NOT NULL, " +
                                                        "    node_id      NUMBER(11) NOT NULL, " +
                                                        "    sequence_id  NUMBER(11) NOT NULL " +
                                                        ")";

        /// <summary>
        /// SQL to drop the ways table.
        /// </summary>
        private const string TABLE_WAY_NODES_DROP = "DROP TABLE way_nodes";

        /// <summary>
        /// SQL to create way tags table.
        /// </summary>
        private const string TABLE_WAY_TAGS_CREATION = "CREATE TABLE way_tags " +
                                                        "( " +
                                                        "	way_id  NUMBER(11) NOT NULL, " +
                                                        "	key     VARCHAR2(255) NOT NULL, " +
                                                        "	value   VARCHAR2(255) " +
                                                        ")";

        /// <summary>
        /// SQL to drop the ways table.
        /// </summary>
        private const string TABLE_WAY_TAGS_DROP = "DROP TABLE way_tags";

        /// <summary>
        /// SQL to create relation table.
        /// </summary>
        private const string TABLE_RELATION_CREATION = "CREATE TABLE relation " +
                                                        "( " +
                                                        "	id            NUMBER(11) NOT NULL, " +
                                                        "	changeset_id  NUMBER(11) NULL, " +
                                                        "	timestamp     DATE NULL, " +
                                                        "	visible       NUMBER(1) NULL, " +
                                                        "	version       NUMBER(11) NULL, " +
                                                        "	usr           VARCHAR2(255), " +
                                                        "	usr_id        NUMBER(11) " +
                                                        ")";

        /// <summary>
        /// SQL to drop the relation table.
        /// </summary>
        private const string TABLE_RELATION_DROP = "DROP TABLE relation";

        /// <summary>
        /// SQL to create relation members table.
        /// </summary>
        private const string TABLE_RELATION_MEMBERS_CREATION = "CREATE TABLE relation_members " +
                                                                "( " +
                                                                "	relation_id  NUMBER(11) NOT NULL, " +
                                                                "	member_type  NUMBER(1) NOT NULL, " +
                                                                "	member_id    NUMBER(11) NOT NULL, " +
                                                                "	member_role  VARCHAR2(255), " +
                                                                "	sequence_id  NUMBER(11) NOT NULL " +
                                                                ")";

        /// <summary>
        /// SQL to drop the relation_members table.
        /// </summary>
        private const string TABLE_RELATION_MEMBERS_DROP = "DROP TABLE relation_members";

        /// <summary>
        /// SQL to create relation tags table.
        /// </summary>
        private const string TABLE_RELATION_TAGS_CREATION = "CREATE TABLE relation_tags " +
                                                            "( " +
                                                            "	relation_id  NUMBER(11) NOT NULL, " +
                                                            "	key          VARCHAR2(255) NOT NULL, " +
                                                            "	value        VARCHAR2(255) " +
                                                            ")";

        /// <summary>
        /// SQL to drop the relation_tags table.
        /// </summary>
        private const string TABLE_RELATION_TAGS_DROP = "DROP TABLE relation_tags";

        /// <summary>
        /// Returns true if the table with the given name exists in the database connected to.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool DetectTable(OracleConnection connection, string name)
        {
            OracleCommand command = new OracleCommand(TABLE_DETECTION_SQL);
            command.Parameters.Add("table_name", name); // use lower case lettering everywhere!
            command.Connection = connection;

            // execute the query.
            decimal count = (decimal)command.ExecuteScalar();
            command.Dispose();
            return count == 1;
        }

        /// <summary>
        /// Executes the given script on the database connected to.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        private static void ExecuteScript(OracleConnection connection, string sql)
        {
            OracleCommand command = new OracleCommand(sql);
            command.Connection = connection;
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Returns true if the nodes table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectNodeTable(OracleConnection connection)
        {
            return OracleSchemaTools.DetectTable(connection, "node");
        }

        /// <summary>
        /// Creates the nodes table.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static void CreateNodeTable(OracleConnection connection)
        {
            OracleSchemaTools.ExecuteScript(connection, TABLE_NODE_CREATION);
        }

        /// <summary>
        /// Drop the nodes table.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static void DropNodeTable(OracleConnection connection)
        {
            OracleSchemaTools.ExecuteScript(connection, TABLE_NODE_DROP);
        }

        /// <summary>
        /// Returns true if the node tags table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectNodeTagsTable(OracleConnection connection)
        {
            return OracleSchemaTools.DetectTable(connection, "node_tags");
        }

        /// <summary>
        /// Creates the node tags table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateNodeTagsTable(OracleConnection connection)
        {
            OracleSchemaTools.ExecuteScript(connection, TABLE_NODE_TAGS_CREATION);
        }

        /// <summary>
        /// Drops the node tags table.
        /// </summary>
        /// <param name="connection"></param>
        public static void DropNodeTagsTable(OracleConnection connection)
        {
            OracleSchemaTools.ExecuteScript(connection, TABLE_NODE_TAGS_DROP);
        }

        /// <summary>
        /// Returns true if the ways table exists.
        /// </summary>
        /// <param name="connection"></param>
        public static bool DetectWayTable(OracleConnection connection)
        {
            return OracleSchemaTools.DetectTable(connection, "way");
        }

        /// <summary>
        /// Creates the way table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateWayTable(OracleConnection connection)
        {
            OracleSchemaTools.ExecuteScript(connection, TABLE_WAY_CREATION);
        }

        /// <summary>
        /// Drops the way table.
        /// </summary>
        /// <param name="connection"></param>
        public static void DropWayTable(OracleConnection connection)
        {
            OracleSchemaTools.ExecuteScript(connection, TABLE_WAY_DROP);
        }

        /// <summary>
        /// Returns true if the way nodes table exists.
        /// </summary>
        /// <param name="connection"></param>
        public static bool DetectWayNodesTable(OracleConnection connection)
        {
            return OracleSchemaTools.DetectTable(connection, "way_nodes");
        }

        /// <summary>
        /// Creates the way nodes table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateWayNodesTable(OracleConnection connection)
        {
            OracleSchemaTools.ExecuteScript(connection, TABLE_WAY_NODES_CREATION);
        }

        /// <summary>
        /// Drops the way nodes table.
        /// </summary>
        /// <param name="connection"></param>
        public static void DropWayNodesTable(OracleConnection connection)
        {
            OracleSchemaTools.ExecuteScript(connection, TABLE_WAY_NODES_DROP);
        }

        /// <summary>
        /// Creates the way tags table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateWayTagsTable(OracleConnection connection)
        {
            OracleSchemaTools.ExecuteScript(connection, TABLE_WAY_TAGS_CREATION);
        }

        /// <summary>
        /// Drops the way tags table.
        /// </summary>
        /// <param name="connection"></param>
        public static void DropWayTagsTable(OracleConnection connection)
        {
            OracleSchemaTools.ExecuteScript(connection, TABLE_WAY_TAGS_DROP);
        }

        /// <summary>
        /// Returns true if the way tags table exists.
        /// </summary>
        /// <param name="connection"></param>
        public static bool DetectWayTagsTable(OracleConnection connection)
        {
            return OracleSchemaTools.DetectTable(connection, "way_tags");
        }

        /// <summary>
        /// Returns true if the relation table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectRelationTable(OracleConnection connection)
        {
            return OracleSchemaTools.DetectTable(connection, "relation");
        }

        /// <summary>
        /// Creates the relation table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateRelationTable(OracleConnection connection)
        {
            OracleSchemaTools.ExecuteScript(connection, TABLE_RELATION_CREATION);
        }

        /// <summary>
        /// Drops the relation table.
        /// </summary>
        /// <param name="connection"></param>
        public static void DropRelationTable(OracleConnection connection)
        {
            OracleSchemaTools.ExecuteScript(connection, TABLE_RELATION_DROP);
        }

        /// <summary>
        /// Returns true if the relation members table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectRelationMembersTable(OracleConnection connection)
        {
            return OracleSchemaTools.DetectTable(connection, "relation_members");
        }

        /// <summary>
        /// Creates the relation members table.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static void CreateRelationMembersTable(OracleConnection connection)
        {
            OracleSchemaTools.ExecuteScript(connection, TABLE_RELATION_MEMBERS_CREATION);
        }

        /// <summary>
        /// Drops the relation members table.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static void DropRelationMembersTable(OracleConnection connection)
        {
            OracleSchemaTools.ExecuteScript(connection, TABLE_RELATION_MEMBERS_DROP);
        }

        /// <summary>
        /// Returns true if the relation tags table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectRelationTagsTable(OracleConnection connection)
        {
            return OracleSchemaTools.DetectTable(connection, "relation_tags");
        }

        /// <summary>
        /// Creates relation tags.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateRelationTagsTable(OracleConnection connection)
        {
            OracleSchemaTools.ExecuteScript(connection, TABLE_RELATION_TAGS_CREATION);
        }

        /// <summary>
        /// Drops relation tags.
        /// </summary>
        /// <param name="connection"></param>
        public static void DropRelationTagsTable(OracleConnection connection)
        {
            OracleSchemaTools.ExecuteScript(connection, TABLE_RELATION_TAGS_DROP);
        }

        /// <summary>
        /// Creates the entire schema but also detects existing tables.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateAndDetect(OracleConnection connection)
        {
            if (!OracleSchemaTools.DetectNodeTable(connection))
            {
                OracleSchemaTools.CreateNodeTable(connection);
            }
            if (!OracleSchemaTools.DetectNodeTagsTable(connection))
            {
                OracleSchemaTools.CreateNodeTagsTable(connection);
            }

            if (!OracleSchemaTools.DetectWayTable(connection))
            {
                OracleSchemaTools.CreateWayTable(connection);
            }
            if (!OracleSchemaTools.DetectWayTagsTable(connection))
            {
                OracleSchemaTools.CreateWayTagsTable(connection);
            }
            if (!OracleSchemaTools.DetectWayNodesTable(connection))
            {
                OracleSchemaTools.CreateWayNodesTable(connection);
            }

            if (!OracleSchemaTools.DetectRelationTable(connection))
            {
                OracleSchemaTools.CreateRelationTable(connection);
            }
            if (!OracleSchemaTools.DetectRelationTagsTable(connection))
            {
                OracleSchemaTools.CreateRelationTagsTable(connection);
            }
            if (!OracleSchemaTools.DetectRelationMembersTable(connection))
            {
                OracleSchemaTools.CreateRelationMembersTable(connection);
            }
        }

        /// <summary>
        /// Drops objects in this schema.
        /// </summary>
        /// <param name="connection"></param>
        public static void Drop(OracleConnection connection)
        {
            if (OracleSchemaTools.DetectNodeTable(connection))
            {
                OracleSchemaTools.DropNodeTable(connection);
            }
            if (OracleSchemaTools.DetectNodeTagsTable(connection))
            {
                OracleSchemaTools.DropNodeTagsTable(connection);
            }

            if (OracleSchemaTools.DetectWayTable(connection))
            {
                OracleSchemaTools.DropWayTable(connection);
            }
            if (OracleSchemaTools.DetectWayTagsTable(connection))
            {
                OracleSchemaTools.DropWayTagsTable(connection);
            }
            if (OracleSchemaTools.DetectWayNodesTable(connection))
            {
                OracleSchemaTools.DropWayNodesTable(connection);
            }

            if (OracleSchemaTools.DetectRelationTable(connection))
            {
                OracleSchemaTools.DropRelationTable(connection);
            }
            if (OracleSchemaTools.DetectRelationTagsTable(connection))
            {
                OracleSchemaTools.DropRelationTagsTable(connection);
            }
            if (OracleSchemaTools.DetectRelationMembersTable(connection))
            {
                OracleSchemaTools.DropRelationMembersTable(connection);
            }
        }
    }
}
