using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;

namespace OsmSharp.Osm.Data.PostgreSQL.SimpleSchema.SchemaTools
{
    /// <summary>
    /// Tools for creation/detection of the simple schema in PostgreSQL.
    /// </summary>
    internal static class PostgreSQLSimpleSchemaTools
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
                                                    "    changeset_id  bigint NOT NULL, " +
                                                    "    visible       boolean NOT NULL, " +
                                                    "    timestamp     timestamp NOT NULL, " +
                                                    "    tile          bigint NOT NULL, " +
                                                    "    version       integer NOT NULL, " +
                                                    "    usr           varchar(510), " +
                                                    "    usr_id        integer, " +
                                                    "    CONSTRAINT    pk_node PRIMARY KEY (id) " +
                                                    " );";

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
        /// SQL to create the way table.
        /// </summary>
        private const string TABLE_WAY_CREATION =   "CREATE TABLE way " +
                                                    "( " +
                                                    "	id            bigint NOT NULL, " +
                                                    "	changeset_id  bigint NOT NULL, " +
                                                    "	timestamp     timestamp NOT NULL, " +
                                                    "	visible       boolean NOT NULL, " +
                                                    "	version       integer NOT NULL, " +
                                                    "	usr           varchar(510), " +
                                                    "	usr_id        integer, " +
                                                    "   CONSTRAINT    pk_way PRIMARY KEY (id) " +
                                                    "); ";

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
        /// SQL to create way tags table.
        /// </summary>
        private const string TABLE_WAY_TAGS_CREATION =  "CREATE TABLE way_tags " +
                                                        "( " +
                                                        "	way_id  bigint NOT NULL, " +
                                                        "	key     varchar(510) NOT NULL, " +
                                                        "	value   varchar(510) " +
                                                        ");";

        /// <summary>
        /// SQL to create relation table.
        /// </summary>
        private const string TABLE_RELATION_CREATION =  "CREATE TABLE relation " +
                                                        "( " +
                                                        "	id            bigint NOT NULL, " +
                                                        "	changeset_id  bigint NOT NULL, " +
                                                        "	timestamp     timestamp NOT NULL, " +
                                                        "	visible       boolean NOT NULL, " +
                                                        "	version       integer NOT NULL, " +
                                                        "	usr           varchar(510), " +
                                                        "	usr_id        integer, " +
                                                        "   CONSTRAINT    pk_relation PRIMARY KEY (id) " +
                                                        ");";

        /// <summary>
        /// SQL to create relation members table.
        /// </summary>
        private const string TABLE_RELATION_MEMBERS_CREATION =  "CREATE TABLE relation_members " +
                                                                "( " +
                                                                "	relation_id  bigint NOT NULL, " +
                                                                "	member_type  varchar(20) NOT NULL, " +
                                                                "	member_id    bigint NOT NULL, " +
                                                                "	member_role  varchar(510), " +
                                                                "	sequence_id  integer NOT NULL " +
                                                                ");";

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
            return PostgreSQLSimpleSchemaTools.DetectTable(connection, "node");
        }

        /// <summary>
        /// Creates the nodes table.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static void CreateNodeTable(NpgsqlConnection connection)
        {
            PostgreSQLSimpleSchemaTools.ExecuteScript(connection, TABLE_NODE_CREATION);
        }

        /// <summary>
        /// Returns true if the node tags table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectNodeTagsTable(NpgsqlConnection connection)
        {
            return PostgreSQLSimpleSchemaTools.DetectTable(connection, "node_tags");
        }

        /// <summary>
        /// Creates the node tags table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateNodeTagsTable(NpgsqlConnection connection)
        {
            PostgreSQLSimpleSchemaTools.ExecuteScript(connection, TABLE_NODE_TAGS_CREATION);
        }

        /// <summary>
        /// Returns true if the ways table exists.
        /// </summary>
        /// <param name="connection"></param>
        public static bool DetectWayTable(NpgsqlConnection connection)
        {
            return PostgreSQLSimpleSchemaTools.DetectTable(connection, "way");
        }

        /// <summary>
        /// Creates the way table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateWayTable(NpgsqlConnection connection)
        {
            PostgreSQLSimpleSchemaTools.ExecuteScript(connection, TABLE_WAY_CREATION);
        }

        /// <summary>
        /// Returns true if the way nodes table exists.
        /// </summary>
        /// <param name="connection"></param>
        public static bool DetectWayNodesTable(NpgsqlConnection connection)
        {
            return PostgreSQLSimpleSchemaTools.DetectTable(connection, "way_nodes");
        }

        /// <summary>
        /// Creates the way nodes table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateWayNodesTable(NpgsqlConnection connection)
        {
            PostgreSQLSimpleSchemaTools.ExecuteScript(connection, TABLE_WAY_NODES_CREATION);
        }

        /// <summary>
        /// Creates the way tags table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateWayTagsTable(NpgsqlConnection connection)
        {
            PostgreSQLSimpleSchemaTools.ExecuteScript(connection, TABLE_WAY_TAGS_CREATION);
        }

        /// <summary>
        /// Returns true if the way tags table exists.
        /// </summary>
        /// <param name="connection"></param>
        public static bool DetectWayTagsTable(NpgsqlConnection connection)
        {
            return PostgreSQLSimpleSchemaTools.DetectTable(connection, "way_tags");
        }

        /// <summary>
        /// Returns true if the relation table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectRelationTable(NpgsqlConnection connection)
        {
            return PostgreSQLSimpleSchemaTools.DetectTable(connection, "relation");
        }

        /// <summary>
        /// Creates the relation table.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateRelationTable(NpgsqlConnection connection)
        {
            PostgreSQLSimpleSchemaTools.ExecuteScript(connection, TABLE_RELATION_CREATION);
        }

        /// <summary>
        /// Returns true if the relation members table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectRelationMembersTable(NpgsqlConnection connection)
        {
            return PostgreSQLSimpleSchemaTools.DetectTable(connection, "relation_members");
        }

        /// <summary>
        /// Creates the relation members table.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static void CreateRelationMembersTable(NpgsqlConnection connection)
        {
            PostgreSQLSimpleSchemaTools.ExecuteScript(connection, TABLE_RELATION_MEMBERS_CREATION);
        }

        /// <summary>
        /// Returns true if the relation tags table exists.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static bool DetectRelationTagsTable(NpgsqlConnection connection)
        {
            return PostgreSQLSimpleSchemaTools.DetectTable(connection, "relation_tags");
        }

        /// <summary>
        /// Creates relation tags.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateRelationTagsTable(NpgsqlConnection connection)
        {
            PostgreSQLSimpleSchemaTools.ExecuteScript(connection, TABLE_RELATION_TAGS_CREATION);
        }

        /// <summary>
        /// Creates the entire schema but also detects existing tables.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateAndDetect(NpgsqlConnection connection)
        {
            if (!PostgreSQLSimpleSchemaTools.DetectNodeTable(connection))
            {
                PostgreSQLSimpleSchemaTools.CreateNodeTable(connection);
            }
            if (!PostgreSQLSimpleSchemaTools.DetectNodeTagsTable(connection))
            {
                PostgreSQLSimpleSchemaTools.CreateNodeTagsTable(connection);
            }

            if(!PostgreSQLSimpleSchemaTools.DetectWayTable(connection))
            {
                PostgreSQLSimpleSchemaTools.CreateWayTable(connection);
            }
            if(!PostgreSQLSimpleSchemaTools.DetectWayTagsTable(connection))
            {
                PostgreSQLSimpleSchemaTools.CreateWayTagsTable(connection);
            }
            if(!PostgreSQLSimpleSchemaTools.DetectWayNodesTable(connection))
            {
                PostgreSQLSimpleSchemaTools.CreateWayNodesTable(connection);
            }

            if(!PostgreSQLSimpleSchemaTools.DetectRelationTable(connection))
            {
                PostgreSQLSimpleSchemaTools.CreateRelationTable(connection);
            }
            if(!PostgreSQLSimpleSchemaTools.DetectRelationTagsTable(connection))
            {
                PostgreSQLSimpleSchemaTools.CreateRelationTagsTable(connection);
            }
            if(!PostgreSQLSimpleSchemaTools.DetectRelationMembersTable(connection))
            {
                PostgreSQLSimpleSchemaTools.CreateRelationMembersTable(connection);
            }
        }
    }
}