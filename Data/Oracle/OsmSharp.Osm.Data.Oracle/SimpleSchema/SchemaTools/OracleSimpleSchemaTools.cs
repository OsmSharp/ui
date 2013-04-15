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
using Oracle.DataAccess.Client;

namespace OsmSharp.Osm.Data.PostgreSQL.SimpleSchema.SchemaTools
{
    /// <summary>
    /// Tools for creation/detection of the simple schema in Oracle.
    /// </summary>
    internal static class OracleSimpleSchemaTools
    {
        /// <summary>
        /// SQL to detect the existence of a table.
        /// </summary>
        private const string TABLE_DETECTION_SQL = "SELECT COUNT (relname) as a FROM pg_class WHERE relname = :table_name";

        /// <summary>
        /// SQL to create the nodes table.
        /// </summary>
        private const string TABLE_NODE_CREATION = "CREATE TABLE node " +
                                                    "(  " +
                                                    "    id            NUMBER(11) NOT NULL, " +
                                                    "    latitude      NUMBER(11) NOT NULL, " +
                                                    "    longitude     NUMBER(11) NOT NULL, " +
                                                    "    changeset_id  NUMBER(11) NOT NULL, " +
                                                    "    visible       NUMBER(1) NOT NULL, " +
                                                    "    timestamp     DATE NOT NULL, " +
                                                    "    tile          NUMBER(11) NOT NULL, " +
                                                    "    version       NUMBER(11) NOT NULL, " +
                                                    "    usr           VARCHAR2(255), " +
                                                    "    usr_id        NUMBER(11) " +
                                                    " );";

        /// <summary>
        /// SQL to create the node tags table.
        /// </summary>
        private const string TABLE_NODE_TAGS_CREATION = "CREATE TABLE node_tags " +
                                                        "( " +
                                                        "	node_id  NUMBER(11) NOT NULL, " +
                                                        "	key      VARCHAR2(255) NOT NULL, " +
                                                        "	value    VARCHAR2(255) " +
                                                        ");   ";

        /// <summary>
        /// SQL to create the way table.
        /// </summary>
        private const string TABLE_WAY_CREATION = "CREATE TABLE way " +
                                                    "( " +
                                                    "	id            NUMBER(11) NOT NULL, " +
                                                    "	changeset_id  NUMBER(11) NOT NULL, " +
                                                    "	timestamp     DATE NOT NULL, " +
                                                    "	visible       NUMBER(1) NOT NULL, " +
                                                    "	version       NUMBER(11) NOT NULL, " +
                                                    "	usr           VARCHAR2(255), " +
                                                    "	usr_id        NUMBER(11) " +
                                                    "); ";

        /// <summary>
        /// SQL to create way nodes table.
        /// </summary>
        private const string TABLE_WAY_NODES_CREATION = "CREATE TABLE way_nodes " +
                                                        "( " +
                                                        "    way_id       NUMBER(11) NOT NULL, " +
                                                        "    node_id      NUMBER(11) NOT NULL, " +
                                                        "    sequence_id  NUMBER(11) NOT NULL " +
                                                        ");";

        /// <summary>
        /// SQL to create way tags table.
        /// </summary>
        private const string TABLE_WAY_TAGS_CREATION = "CREATE TABLE way_tags " +
                                                        "( " +
                                                        "	way_id  NUMBER(11) NOT NULL, " +
                                                        "	key     VARCHAR2(255) NOT NULL, " +
                                                        "	value   VARCHAR2(255) " +
                                                        ");";

        /// <summary>
        /// SQL to create relation table.
        /// </summary>
        private const string TABLE_RELATION_CREATION = "CREATE TABLE relation " +
                                                        "( " +
                                                        "	id            NUMBER(11) NOT NULL, " +
                                                        "	changeset_id  NUMBER(11) NOT NULL, " +
                                                        "	timestamp     DATE NOT NULL, " +
                                                        "	visible       NUMBER(1) NOT NULL, " +
                                                        "	version       NUMBER(11) NOT NULL, " +
                                                        "	usr           VARCHAR2(255), " +
                                                        "	usr_id        NUMBER(11) " +
                                                        ");";

        /// <summary>
        /// SQL to create relation members table.
        /// </summary>
        private const string TABLE_RELATION_MEMBERS_CREATION = "CREATE TABLE relation_members " +
                                                                "( " +
                                                                "	relation_id  NUMBER(11) NOT NULL, " +
                                                                "	member_type  VARCHAR2(20) NOT NULL, " +
                                                                "	member_id    NUMBER(11) NOT NULL, " +
                                                                "	member_role  VARCHAR2(255), " +
                                                                "	sequence_id  NUMBER(11) NOT NULL " +
                                                                ");";

        /// <summary>
        /// SQL to create relation tags table.
        /// </summary>
        private const string TABLE_RELATION_TAGS_CREATION = "CREATE TABLE relation_tags " +
                                                            "( " +
                                                            "	relation_id  NUMBER(11) NOT NULL, " +
                                                            "	key          VARCHAR2(255) NOT NULL, " +
                                                            "	value        VARCHAR2(255) " +
                                                            ");";

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
            long count = (long)command.ExecuteScalar();
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
            OracleCommand command = new OracleCommand(TABLE_NODE_CREATION);
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
            return OracleSimpleSchemaTools.DetectTable(connection, "node");
        }

        /// <summary>
        /// Creates the nodes table.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static void CreateNodeTable(OracleConnection connection)
        {
            OracleSimpleSchemaTools.ExecuteScript(connection, TABLE_NODE_CREATION);
        }

        /// <summary>
        /// Returns true if the node tags table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectNodeTagsTable(OracleConnection connection)
        {
            return OracleSimpleSchemaTools.DetectTable(connection, "node_tags");
        }

        /// <summary>
        /// Creates the node tags table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateNodeTagsTable(OracleConnection connection)
        {
            OracleSimpleSchemaTools.ExecuteScript(connection, TABLE_NODE_TAGS_CREATION);
        }

        /// <summary>
        /// Returns true if the ways table exists.
        /// </summary>
        /// <param name="connection"></param>
        public static bool DetectWayTable(OracleConnection connection)
        {
            return OracleSimpleSchemaTools.DetectTable(connection, "way");
        }

        /// <summary>
        /// Creates the way table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateWayTable(OracleConnection connection)
        {
            OracleSimpleSchemaTools.ExecuteScript(connection, TABLE_WAY_CREATION);
        }

        /// <summary>
        /// Returns true if the way nodes table exists.
        /// </summary>
        /// <param name="connection"></param>
        public static bool DetectWayNodesTable(OracleConnection connection)
        {
            return OracleSimpleSchemaTools.DetectTable(connection, "way_nodes");
        }

        /// <summary>
        /// Creates the way nodes table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateWayNodesTable(OracleConnection connection)
        {
            OracleSimpleSchemaTools.ExecuteScript(connection, TABLE_WAY_NODES_CREATION);
        }

        /// <summary>
        /// Creates the way tags table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateWayTagsTable(OracleConnection connection)
        {
            OracleSimpleSchemaTools.ExecuteScript(connection, TABLE_WAY_TAGS_CREATION);
        }

        /// <summary>
        /// Returns true if the way tags table exists.
        /// </summary>
        /// <param name="connection"></param>
        public static bool DetectWayTagsTable(OracleConnection connection)
        {
            return OracleSimpleSchemaTools.DetectTable(connection, "way_tags");
        }

        /// <summary>
        /// Returns true if the relation table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectRelationTable(OracleConnection connection)
        {
            return OracleSimpleSchemaTools.DetectTable(connection, "relation");
        }

        /// <summary>
        /// Creates the relation table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateRelationTable(OracleConnection connection)
        {
            OracleSimpleSchemaTools.ExecuteScript(connection, TABLE_RELATION_CREATION);
        }

        /// <summary>
        /// Returns true if the relation members table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectRelationMembersTable(OracleConnection connection)
        {
            return OracleSimpleSchemaTools.DetectTable(connection, "relation_members");
        }

        /// <summary>
        /// Creates the relation members table.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static void CreateRelationMembersTable(OracleConnection connection)
        {
            OracleSimpleSchemaTools.ExecuteScript(connection, TABLE_RELATION_MEMBERS_CREATION);
        }

        /// <summary>
        /// Returns true if the relation tags table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectRelationTagsTable(OracleConnection connection)
        {
            return OracleSimpleSchemaTools.DetectTable(connection, "relation_tags");
        }

        /// <summary>
        /// Creates relation tags.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateRelationTagsTable(OracleConnection connection)
        {
            OracleSimpleSchemaTools.ExecuteScript(connection, TABLE_RELATION_TAGS_CREATION);
        }

        /// <summary>
        /// Creates the entire schema but also detects existing tables.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateAndDetect(OracleConnection connection)
        {
            if (!OracleSimpleSchemaTools.DetectNodeTable(connection))
            {
                OracleSimpleSchemaTools.CreateNodeTable(connection);
            }
            if (!OracleSimpleSchemaTools.DetectNodeTagsTable(connection))
            {
                OracleSimpleSchemaTools.CreateNodeTagsTable(connection);
            }

            if (!OracleSimpleSchemaTools.DetectWayTable(connection))
            {
                OracleSimpleSchemaTools.CreateWayTable(connection);
            }
            if (!OracleSimpleSchemaTools.DetectWayTagsTable(connection))
            {
                OracleSimpleSchemaTools.CreateWayTagsTable(connection);
            }
            if (!OracleSimpleSchemaTools.DetectWayNodesTable(connection))
            {
                OracleSimpleSchemaTools.CreateWayNodesTable(connection);
            }

            if (!OracleSimpleSchemaTools.DetectRelationTable(connection))
            {
                OracleSimpleSchemaTools.CreateRelationTable(connection);
            }
            if (!OracleSimpleSchemaTools.DetectRelationTagsTable(connection))
            {
                OracleSimpleSchemaTools.CreateRelationTagsTable(connection);
            }
            if (!OracleSimpleSchemaTools.DetectRelationMembersTable(connection))
            {
                OracleSimpleSchemaTools.CreateRelationMembersTable(connection);
            }
        }
    }
}