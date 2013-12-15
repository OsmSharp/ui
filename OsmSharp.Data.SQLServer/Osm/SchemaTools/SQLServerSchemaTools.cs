// OsmSharp - OpenStreetMap (OSM) SDK
//
// Copyright (C) 2013 Abelshausen Ben
//                    Alexander Sinitsyn
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
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OsmSharp.Data.SQLServer.Osm.SchemaTools
{
    /// <summary>
    /// Tools for creation/detection of the simple schema in PostgreSQL.
    /// </summary>
    public static class SQLServerSchemaTools
    {
        /// <summary>
        /// Creates/detects the simple schema. Does not create the indexes and constraints. That is done after the data is loaded
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateAndDetect(SqlConnection connection)
        {
            //check if Simple Schema table exists
            const string sql = "select object_id('dbo.node', 'U')";
            object res;
            using (var cmd = new SqlCommand(sql, connection))
            {
                res = cmd.ExecuteScalar();
            }
            //if table exists, we are OK
            if (!DBNull.Value.Equals(res))
                return;

            OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.SQLServer.Osm.SchemaTools.SQLServerSchemaTools", OsmSharp.Logging.TraceEventType.Information,
                "Creating database schema");
            ExecuteSQL(connection, "SchemaDDL.sql");
        }

        /// <summary>
        /// Removes the simple schema.
        /// </summary>
        public static void Remove(SqlConnection connection)
        {
            OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.SQLServer.Osm.SchemaTools.SQLServerSchemaTools", OsmSharp.Logging.TraceEventType.Information, 
                "Removing database schema");
            ExecuteSQL(connection, "SchemaDROP.sql");
        }

        /// <summary>
        /// Removes non-routing data from the database. For the UK, this resulted in 34.4 million records being deleted.
        /// </summary>
        public static void RemoveNonRoutingData(SqlConnection connection)
        {
            OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.SQLServer.Osm.SchemaTools.SQLServerSchemaTools", OsmSharp.Logging.TraceEventType.Information, 
                "Removing non-routing data");
            ExecuteSQL(connection, "SchemaDeleteNonRouting.sql");
        }

        /// <summary>
        /// Add indexes, removed duplicates and adds constraints
        /// </summary>
        /// <param name="connection"></param>
        public static void AddConstraints(SqlConnection connection)
        {
            OsmSharp.Logging.Log.TraceEvent("OsmSharp.Data.SQLServer.Osm.SchemaTools.SQLServerSchemaTools", OsmSharp.Logging.TraceEventType.Information, 
                "Adding database constraints");
            ExecuteSQL(connection, "SchemaConstraints.sql");
        }

        private static void ExecuteSQL(SqlConnection connection, string resourceFilename)
        {
            foreach (string resource in Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(resource => resource.EndsWith(resourceFilename)))
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            using (var cmd = new SqlCommand("", connection))
                            {
                                cmd.CommandTimeout = 1800;  // 30 minutes. Adding constraints can be time consuming
                                cmd.CommandText = reader.ReadToEnd();
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                break;
            }
        }
    }
}