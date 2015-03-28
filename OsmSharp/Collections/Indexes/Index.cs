// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

namespace OsmSharp.Collections.Indexes
{
    /// <summary>
    /// An index of objects linked to a unique id.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Index<T> : IndexReadonly<T>
        where T : struct
    {
        /// <summary>
        /// Adds a new element to this index.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public abstract long Add(T element);
    }
}