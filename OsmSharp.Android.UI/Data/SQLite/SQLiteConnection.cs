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

using OsmSharp.Data.SQLite;

namespace OsmSharp.Android.UI.Data.SQLite
{
    /// <summary>
    /// An Android-specific implementation of an SQLiteConnection.
    /// </summary>
    public class SQLiteConnection : OsmSharp.Data.SQLite.SQLiteConnectionBase
    {
        /// <summary>
        /// Holds the 'native' or platform-specific connection.
        /// </summary>
        private global::SQLite.SQLiteConnection _nativeConnection;

        /// <summary>
        /// Creates a new SQLite connection.
        /// </summary>
        /// <param name="databasePath"></param>
        public SQLiteConnection(string databasePath)
        {
            _nativeConnection = new global::SQLite.SQLiteConnection(databasePath);
        }

        /// <summary>
        /// Creates an SQLite command.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public override SQLiteCommandBase CreateCommand(string sql)
        {
            return new SQLiteCommand(_nativeConnection.CreateCommand(sql));
        }
    }
}