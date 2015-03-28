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
    /// An readonly-index of objects linked to a unique id.
    /// </summary>
    public abstract class IndexReadonly<T>
        where T : struct
    {
        /// <summary>
        /// Tries to get an element with the given id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="element">The element.</param>
        /// <returns>True if an element with the given id was found.</returns>
        public abstract bool TryGet(long id, out T element);

        /// <summary>
        /// Gets the element with the given id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The element.</returns>
        public abstract T Get(long id);
    }
}