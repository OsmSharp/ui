// OsmSharp - OpenStreetMap tools & library.
//
// Copyright (C) 2013 Abelshausen Ben
//                    Alexander Sinitsin
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
using System.Reflection;
using System.IO;

namespace OsmSharp.Osm.Data.SQLServer.SimpleSchema.SchemaTools
{
    /// <summary>
    /// Tools for creation/detection of the simple schema in PostgreSQL.
    /// </summary>
    internal static class SQLServerSimpleSchemaTools
    {
        /// <summary>
        /// Creates/detects the simple schema.
        /// </summary>
        /// <param name="connection"></param>
        public static void CreateAndDetect(SqlConnection connection)
        {
            //check if Simple Schema table exists
            string sql = "select object_id('dbo.node', 'U')";
            object res = null;
            using (SqlCommand cmd = new SqlCommand(sql, connection))
            {
                res = cmd.ExecuteScalar();
            }
            //if table exists, we are OK
            if (!DBNull.Value.Equals(res))
                return;
            //in other case let's run script to recreate tables
            //obtain DDL script from resources
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("OsmSharp.Osm.Data.SQLServer.SchemaTools." + "SimpleSchemaDDL.sql"))
            using (StreamReader reader = new StreamReader(stream))
            using (SqlCommand cmd = new SqlCommand("", connection))
            {
                cmd.CommandText = reader.ReadToEnd();
                res = cmd.ExecuteNonQuery();
            }
            
        }
    }
}