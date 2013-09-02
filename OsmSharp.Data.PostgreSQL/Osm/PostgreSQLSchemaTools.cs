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
using Npgsql;

namespace OsmSharp.Data.PostgreSQL.Osm
{
    /// <summary>
    /// Tools for creation/detection of the simple schema in PostgreSQL.
    /// </summary>
    public static class PostgreSQLSchemaTools
    {
        /// <summary>
        /// SQL to detect the existence of a table.
        /// </summary>
        private const string TABLE_DETECTION_SQL = "SELECT COUNT (relname) as a FROM pg_class WHERE relname = :table_name";

        /// <summary>
        /// SQL to create the nodes table.
        /// </summary>
        private const string TABLE_NODE_CREATION =  "CREATE TABLE node " +
                                                    "(  " +
                                                    "    id            bigint NOT NULL, " +
                                                    "    latitude      integer NOT NULL, " +
                                                    "    longitude     integer NOT NULL, " +
                                                    "    changeset_id  bigint, " +
                                                    "    visible       boolean, " +
                                                    "    timestamp     timestamp, " +
                                                    "    tile          bigint NOT NULL, " +
                                                    "    version       integer, " +
                                                    "    usr           varchar(510), " +
                                                    "    usr_id        integer, " +
                                                    "    CONSTRAINT    pk_node PRIMARY KEY (id) " +
                                                    " );";

        /// <summary>
        /// SQL to drop the nodes table.
        /// </summary>
        private const string TABLE_NODE_DROP = "DROP TABLE node;";

        /// <summary>
        /// SQL to create the node tags table.
        /// </summary>
        private const string TABLE_NODE_TAGS_CREATION = "CREATE TABLE node_tags " +
                                                        "( " +
                                                        "	node_id  bigint NOT NULL, " +
                                                        "	key      varchar(510) NOT NULL, " +
                                                        "	value    varchar(510) " +
                                                        ");   ";

        /// <summary>
        /// SQL to drop the node_tags table.
        /// </summary>
        private const string TABLE_NODE_TAGS_DROP = "DROP TABLE node_tags;";

        /// <summary>
        /// SQL to create the way table.
        /// </summary>
        private const string TABLE_WAY_CREATION =   "CREATE TABLE way " +
                                                    "( " +
                                                    "	id            bigint NOT NULL, " +
                                                    "	changeset_id  bigint, " +
                                                    "	timestamp     timestamp, " +
                                                    "	visible       boolean, " +
                                                    "	version       integer, " +
                                                    "	usr           varchar(510), " +
                                                    "	usr_id        integer, " +
                                                    "   CONSTRAINT    pk_way PRIMARY KEY (id) " +
                                                    "); ";

        /// <summary>
        /// SQL to drop the way table.
        /// </summary>
        private const string TABLE_WAY_DROP = "DROP TABLE way;";

        /// <summary>
        /// SQL to create way nodes table.
        /// </summary>
        private const string TABLE_WAY_NODES_CREATION = "CREATE TABLE way_nodes " +
                                                        "( " +
                                                        "    way_id       bigint NOT NULL, " +
                                                        "    node_id      bigint NOT NULL, " +
                                                        "    sequence_id  integer NOT NULL " +
                                                        "); " +
                                                        "CREATE INDEX way_nodes_node_idx " +
                                                        " ON way_nodes " +
                                                        " USING btree " +
                                                        " (node_id); " +
                                                        "CREATE INDEX way_nodes_way_idx " +
                                                        " ON way_nodes " +
                                                        " USING btree " +
                                                        " (way_id);";

        /// <summary>
        /// SQL to drop the way_nodes table.
        /// </summary>
        private const string TABLE_WAY_NODES_DROP = "DROP TABLE way_nodes;";

        /// <summary>
        /// SQL to create way tags table.
        /// </summary>
        private const string TABLE_WAY_TAGS_CREATION =  "CREATE TABLE way_tags " +
                                                        "( " +
                                                        "	way_id  bigint NOT NULL, " +
                                                        "	key     varchar(510) NOT NULL, " +
                                                        "	value   varchar(510) " +
                                                        ");";

        /// <summary>
        /// SQL to drop the way_tags table.
        /// </summary>
        private const string TABLE_WAY_TAGS_DROP = "DROP TABLE way_tags;";

        /// <summary>
        /// SQL to create relation table.
        /// </summary>
        private const string TABLE_RELATION_CREATION =  "CREATE TABLE relation " +
                                                        "( " +
                                                        "	id            bigint NOT NULL, " +
                                                        "	changeset_id  bigint, " +
                                                        "	timestamp     timestamp, " +
                                                        "	visible       boolean, " +
                                                        "	version       integer, " +
                                                        "	usr           varchar(510), " +
                                                        "	usr_id        integer, " +
                                                        "   CONSTRAINT    pk_relation PRIMARY KEY (id) " +
                                                        ");";

        /// <summary>
        /// SQL to drop the relation table.
        /// </summary>
        private const string TABLE_RELATION_DROP = "DROP TABLE relation;";

        /// <summary>
        /// SQL to create relation members table.
        /// </summary>
        private const string TABLE_RELATION_MEMBERS_CREATION =  "CREATE TABLE relation_members " +
                                                                "( " +
                                                                "	relation_id  bigint NOT NULL, " +
                                                                "	member_type  integer NOT NULL, " +
                                                                "	member_id    bigint NOT NULL, " +
                                                                "	member_role  varchar(510), " +
                                                                "	sequence_id  integer NOT NULL " +
                                                                ");";

        /// <summary>
        /// SQL to drop the relation_members table.
        /// </summary>
        private const string TABLE_RELATION_MEMBERS_DROP = "DROP TABLE relation_members;";

        /// <summary>
        /// SQL to create relation tags table.
        /// </summary>
        private const string TABLE_RELATION_TAGS_CREATION = "CREATE TABLE relation_tags " +
                                                            "( " +
                                                            "	relation_id  bigint NOT NULL, " +
                                                            "	key          varchar(510) NOT NULL, " +
                                                            "	value        varchar(510) " +
                                                            ");";

        /// <summary>
        /// SQL to drop the relation_tags table.
        /// </summary>
        private const string TABLE_RELATION_TAGS_DROP = "DROP TABLE relation_tags;";

        /// <summary>
        /// Returns true if the table with the given name exists in the database connected to.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static bool DetectTable(NpgsqlConnection connection, string name)
        {
            var command = new NpgsqlCommand(TABLE_DETECTION_SQL);
            command.Parameters.Add(new NpgsqlParameter("table_name", name)); // use lower case lettering everywhere!
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
        private static void ExecuteScript(NpgsqlConnection connection, string sql)
        {
            NpgsqlCommand command = new NpgsqlCommand(sql);
            command.Connection = connection;
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Returns true if the nodes table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectNodeTable(NpgsqlConnection connection)
        {
            return PostgreSQLSchemaTools.DetectTable(connection, "node");
        }

        /// <summary>
        /// Creates the nodes table.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static void CreateNodeTable(NpgsqlConnection connection)
        {
            PostgreSQLSchemaTools.ExecuteScript(connection, TABLE_NODE_CREATION);
        }

        /// <summary>
        /// Drops the nodes table.
        /// </summary>
        /// <param name="connection"></param>
        public static void DropNodeTable(NpgsqlConnection connection)
        {
            PostgreSQLSchemaTools.ExecuteScript(connection, TABLE_NODE_DROP);
        }

        /// <summary>
        /// Returns true if the node tags table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectNodeTagsTable(NpgsqlConnection connection)
        {
            return PostgreSQLSchemaTools.DetectTable(connection, "node_tags");
        }

        /// <summary>
        /// Creates the node tags table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateNodeTagsTable(NpgsqlConnection connection)
        {
            PostgreSQLSchemaTools.ExecuteScript(connection, TABLE_NODE_TAGS_CREATION);
        }

        /// <summary>
        /// Drops the node tags table.
        /// </summary>
        /// <param name="connection"></param>
        public static void DropNodeTagsTable(NpgsqlConnection connection)
        {
            PostgreSQLSchemaTools.ExecuteScript(connection, TABLE_NODE_TAGS_DROP);
        }

        /// <summary>
        /// Returns true if the ways table exists.
        /// </summary>
        /// <param name="connection"></param>
        public static bool DetectWayTable(NpgsqlConnection connection)
        {
            return PostgreSQLSchemaTools.DetectTable(connection, "way");
        }

        /// <summary>
        /// Creates the way table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateWayTable(NpgsqlConnection connection)
        {
            PostgreSQLSchemaTools.ExecuteScript(connection, TABLE_WAY_CREATION);
        }

        /// <summary>
        /// Drops the way table.
        /// </summary>
        /// <param name="connection"></param>
        public static void DropWayTable(NpgsqlConnection connection)
        {
            PostgreSQLSchemaTools.ExecuteScript(connection, TABLE_WAY_DROP);
        }

        /// <summary>
        /// Returns true if the way nodes table exists.
        /// </summary>
        /// <param name="connection"></param>
        public static bool DetectWayNodesTable(NpgsqlConnection connection)
        {
            return PostgreSQLSchemaTools.DetectTable(connection, "way_nodes");
        }

        /// <summary>
        /// Creates the way nodes table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateWayNodesTable(NpgsqlConnection connection)
        {
            PostgreSQLSchemaTools.ExecuteScript(connection, TABLE_WAY_NODES_CREATION);
        }

        /// <summary>
        /// Drops the way nodes table.
        /// </summary>
        /// <param name="connection"></param>
        public static void DropWayNodesTable(NpgsqlConnection connection)
        {
            PostgreSQLSchemaTools.ExecuteScript(connection, TABLE_WAY_NODES_DROP);
        }

        /// <summary>
        /// Creates the way tags table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateWayTagsTable(NpgsqlConnection connection)
        {
            PostgreSQLSchemaTools.ExecuteScript(connection, TABLE_WAY_TAGS_CREATION);
        }

        /// <summary>
        /// Returns true if the way tags table exists.
        /// </summary>
        /// <param name="connection"></param>
        public static bool DetectWayTagsTable(NpgsqlConnection connection)
        {
            return PostgreSQLSchemaTools.DetectTable(connection, "way_tags");
        }

        /// <summary>
        /// Drops the way tags table.
        /// </summary>
        /// <param name="connection"></param>
        public static void DropWayTagsTable(NpgsqlConnection connection)
        {
            PostgreSQLSchemaTools.ExecuteScript(connection, TABLE_WAY_TAGS_DROP);
        }

        /// <summary>
        /// Returns true if the relation table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectRelationTable(NpgsqlConnection connection)
        {
            return PostgreSQLSchemaTools.DetectTable(connection, "relation");
        }

        /// <summary>
        /// Creates the relation table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateRelationTable(NpgsqlConnection connection)
        {
            PostgreSQLSchemaTools.ExecuteScript(connection, TABLE_RELATION_CREATION);
        }

        /// <summary>
        /// Drops the relation table.
        /// </summary>
        /// <param name="connection"></param>
        public static void DropRelationTable(NpgsqlConnection connection)
        {
            PostgreSQLSchemaTools.ExecuteScript(connection, TABLE_RELATION_DROP);
        }

        /// <summary>
        /// Returns true if the relation members table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectRelationMembersTable(NpgsqlConnection connection)
        {
            return PostgreSQLSchemaTools.DetectTable(connection, "relation_members");
        }

        /// <summary>
        /// Creates the relation members table.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static void CreateRelationMembersTable(NpgsqlConnection connection)
        {
            PostgreSQLSchemaTools.ExecuteScript(connection, TABLE_RELATION_MEMBERS_CREATION);
        }

        /// <summary>
        /// Drops the relation members table.
        /// </summary>
        /// <param name="connection"></param>
        public static void DropRelationMembersTable(NpgsqlConnection connection)
        {
            PostgreSQLSchemaTools.ExecuteScript(connection, TABLE_RELATION_MEMBERS_DROP);
        }

        /// <summary>
        /// Returns true if the relation tags table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectRelationTagsTable(NpgsqlConnection connection)
        {
            return PostgreSQLSchemaTools.DetectTable(connection, "relation_tags");
        }

        /// <summary>
        /// Creates relation tags.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateRelationTagsTable(NpgsqlConnection connection)
        {
            PostgreSQLSchemaTools.ExecuteScript(connection, TABLE_RELATION_TAGS_CREATION);
        }

        /// <summary>
        /// Drops the relation tags table.
        /// </summary>
        /// <param name="connection"></param>
        public static void DropRelationTagsTable(NpgsqlConnection connection)
        {
            PostgreSQLSchemaTools.ExecuteScript(connection, TABLE_RELATION_TAGS_DROP);
        }

        /// <summary>
        /// Creates the entire schema but also detects existing tables.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateAndDetect(NpgsqlConnection connection)
        {
            if (!PostgreSQLSchemaTools.DetectNodeTable(connection))
            {
                PostgreSQLSchemaTools.CreateNodeTable(connection);
            }
            if (!PostgreSQLSchemaTools.DetectNodeTagsTable(connection))
            {
                PostgreSQLSchemaTools.CreateNodeTagsTable(connection);
            }

            if (!PostgreSQLSchemaTools.DetectWayTable(connection))
            {
                PostgreSQLSchemaTools.CreateWayTable(connection);
            }
            if (!PostgreSQLSchemaTools.DetectWayTagsTable(connection))
            {
                PostgreSQLSchemaTools.CreateWayTagsTable(connection);
            }
            if (!PostgreSQLSchemaTools.DetectWayNodesTable(connection))
            {
                PostgreSQLSchemaTools.CreateWayNodesTable(connection);
            }

            if (!PostgreSQLSchemaTools.DetectRelationTable(connection))
            {
                PostgreSQLSchemaTools.CreateRelationTable(connection);
            }
            if (!PostgreSQLSchemaTools.DetectRelationTagsTable(connection))
            {
                PostgreSQLSchemaTools.CreateRelationTagsTable(connection);
            }
            if (!PostgreSQLSchemaTools.DetectRelationMembersTable(connection))
            {
                PostgreSQLSchemaTools.CreateRelationMembersTable(connection);
            }
        }

        /// <summary>
        /// Drops the entire schema.
        /// </summary>
        /// <param name="connection"></param>
        public static void Drop(NpgsqlConnection connection)
        {
            if (PostgreSQLSchemaTools.DetectRelationMembersTable(connection))
            {
                PostgreSQLSchemaTools.DropRelationMembersTable(connection);
            }
            if (PostgreSQLSchemaTools.DetectRelationTagsTable(connection))
            {
                PostgreSQLSchemaTools.DropRelationTagsTable(connection);
            }
            if (PostgreSQLSchemaTools.DetectRelationTable(connection))
            {
                PostgreSQLSchemaTools.DropRelationTable(connection);
            }
            if (PostgreSQLSchemaTools.DetectWayNodesTable(connection))
            {

                PostgreSQLSchemaTools.DropWayNodesTable(connection);
            }
            if (PostgreSQLSchemaTools.DetectWayTagsTable(connection))
            {
                PostgreSQLSchemaTools.DropWayTagsTable(connection);
            }
            if (PostgreSQLSchemaTools.DetectWayTable(connection))
            {
                PostgreSQLSchemaTools.DropWayTable(connection);
            }
            if (PostgreSQLSchemaTools.DetectNodeTagsTable(connection))
            {
                PostgreSQLSchemaTools.DropNodeTagsTable(connection);
            }
            if (PostgreSQLSchemaTools.DetectNodeTable(connection))
            {
                PostgreSQLSchemaTools.DropNodeTable(connection);
            }
        }
    }
}