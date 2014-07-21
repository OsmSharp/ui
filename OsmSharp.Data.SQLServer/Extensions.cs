// OsmSharp - OpenStreetMap (OSM) SDK
//
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

namespace OsmSharp.Data.SQLServer
{
    /// <summary>
    /// Contains some extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts a nullable to a db value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="nullable"></param>
        /// <returns></returns>
        public static object ConvertToDBValue<T>(this Nullable<T> nullable) where T : struct
        {
            if (nullable.HasValue)
            {
                return nullable.Value;
            }
            else
            {
                return DBNull.Value;
            }
        }
    }
}
