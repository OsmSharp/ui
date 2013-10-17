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

namespace OsmSharp.UnitTests
{
    /// <summary>
    /// Keeps objects in memory between tests to speed-up testing by not re-creating the same conditions each time.
    /// </summary>
    public static class StaticDictionary
    {
        /// <summary>
        /// The dictionary with the objects.
        /// </summary>
        private static Dictionary<string, object> _dic = new Dictionary<string,object>();

        /// <summary>
        /// Adds a new object of a given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Add<T>(string key, T value)
            where T : class
        {
            _dic[key] = value;
        }

        /// <summary>
        /// Gets an object from the dictionary.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
            where T : class
        {
            object value = null;
            if (_dic.TryGetValue(key, out value))
            { // the value was in the dictionary.
                if (value is T)
                {
                    return value as T;
                }
                else if(value != null)
                { // there is a non-null value of the wrong type.
                    throw new Exception(string.Format("Requested object of type {0} with key {1} but object with type {2} was found!",
                        typeof(T).ToString(), key, value.GetType().ToString()));
                }
            }
            return null;
        }
    }
}
