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
using System.IO;

namespace OsmSharp.iOS.UI
{
    /// <summary>
    /// An iOS-specific implementation of an SQLiteConnection.
    /// </summary>
    public class SQLiteConnection : OsmSharp.Data.SQLite.SQLiteConnectionBase
    {
        /// <summary>
        /// Holds the 'native' or platform-specific connection.
        /// </summary>
        private OsmSharp.iOS.SQLite.SQLiteConnection _nativeConnection;

        /// <summary>
        /// Creates a new SQLite connection.
        /// </summary>
        /// <param name="databasePath"></param>
        public SQLiteConnection(string databasePath)
        {
            _nativeConnection = new OsmSharp.iOS.SQLite.SQLiteConnection(databasePath);
        }

        /// <summary>
        /// Creates a SQLiteCommand given the command text (SQL) with arguments. Place a '?'
        /// in the command text for each of the arguments and then executes that command.
        /// It returns each row of the result using the mapping automatically generated for
        /// the given type.
        /// </summary>
        /// <param name="query">
        /// The fully escaped SQL.
        /// </param>
        /// <param name="args">
        /// Arguments to substitute for the occurences of '?' in the query.
        /// </param>
        /// <returns>
        /// An enumerable with one result for each row returned by the query.
        /// </returns>
        public override System.Collections.Generic.List<T> Query<T>(string query, params object[] args)
        {
            return _nativeConnection.Query<T>(query, args);
        }

        /// <summary>
        /// Creates a new SQLite connection from a Stream by copying the data in the stream to a local path and open that file.
        /// </summary>
        /// <param name="stream">The stream containing the database data.</param>
        /// <param name="dbName">A name for the temporary file to use.</param>
        /// <returns></returns>
        public static SQLiteConnection CreateFrom(Stream stream, string dbName)
        {
            var destinationPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName + ".db3");
            using (var destination = System.IO.File.Create(destinationPath))
            {
                stream.CopyTo(destination);
            }
            return new SQLiteConnection(destinationPath);
        }

        /// <summary>
        /// Diposes of all resources associated with this connection.
        /// </summary>
        public override void Dispose()
        {
            _nativeConnection.Dispose();
        }

        /// <summary>
        /// Closes this connection.
        /// </summary>
        public override void Close()
        {
            _nativeConnection.Close();
        }
    }
}

