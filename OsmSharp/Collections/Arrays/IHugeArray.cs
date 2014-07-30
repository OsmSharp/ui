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

namespace OsmSharp.Collections.Arrays
{
    /// <summary>
    /// An abstract representation of a huge array.
    /// </summary>
    public interface IHugeArray<T>
    {
        /// <summary>
        /// Returns the length of this array.
        /// </summary>
        long Length { get; }
        
        /// <summary>
        /// Resizes this array.
        /// </summary>
        /// <param name="size"></param>
        void Resize(long size);

        /// <summary>
        /// Returns the element at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        T this[long idx] { get; set; }
    }
}