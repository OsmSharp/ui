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

using System.Collections.Generic;

namespace OsmSharp.Android.UI.Data.SQLite
{
    /// <summary>
    /// An Android-specific implementation of an SQLiteCommand.
    /// </summary>
    public class SQLiteCommand : OsmSharp.Data.SQLite.SQLiteCommandBase
    {
        /// <summary>
        /// Holds the 'native' or platform-specific command.
        /// </summary>
        private global::SQLite.SQLiteCommand _nativeCommand;

        /// <summary>
        /// Creates a new SQLite command.
        /// </summary>
        /// <param name="nativeCommand"></param>
        internal SQLiteCommand(global::SQLite.SQLiteCommand nativeCommand)
        {
            _nativeCommand = nativeCommand;
        }

        /// <summary>
        /// Adds a parameter with the given name and given value.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="data"></param>
        public override void AddParameterWithValue(string parameter, object data)
        {
            _nativeCommand.Bind(parameter, data);
        }

        /// <summary>
        /// Executes the command and returns a data reader.
        /// </summary>
        /// <returns></returns>
        public override List<T> ExecuteQuery<T>()
        {
            return _nativeCommand.ExecuteQuery<T>();
        }

        /// <summary>
        /// Executes the sql and returns a status.
        /// </summary>
        /// <returns></returns>
        public override int ExecuteNonQuery()
        {
            return _nativeCommand.ExecuteNonQuery();
        }
    }
}