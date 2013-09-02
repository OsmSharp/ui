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

namespace OsmSharp.Data.Oracle
{
    /// <summary>
    /// Extensions for the oracle command.
    /// </summary>
    public static class OracleCommandExtension
    {
        /// <summary>
        /// Adds a list of parameters to the given oracle command.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="command"></param>
        /// <param name="name"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static OracleCommand AddParameterCollection<TValue>(this OracleCommand command, string name, IEnumerable<TValue> collection)
        {
            var oraParams = new List<OracleParameter>();
            var counter = 0;
            var collectionParams = new StringBuilder(":");
            foreach (var obj in collection)
            {
                var param = name + counter;
                collectionParams.Append(param);
                collectionParams.Append(", :");
                command.Parameters.Add(param, obj);
                counter++;
            }
            collectionParams.Remove(collectionParams.Length - 3, 3);
            command.CommandText = command.CommandText.Replace(":" + name, collectionParams.ToString());
            command.Parameters.AddRange(oraParams.ToArray());
            return command;
        }
    }
}