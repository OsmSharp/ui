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

using OsmSharp.IO.MemoryMappedFiles;

namespace OsmSharp.Collections.Arrays.MemoryMapped
{
    /// <summary>
    /// Represents a memory mapped huge array of uints.
    /// </summary>
    public class MemoryMappedHugeArrayUInt32 : MemoryMappedHugeArray<uint>
    {
        /// <summary>
        /// Creates a memory mapped huge array.
        /// </summary>
        /// <param name="file">The the memory mapped file.</param>
        /// <param name="size">The initial size of the array.</param>
        public MemoryMappedHugeArrayUInt32(MemoryMappedFile file, long size)
            : base(file, 4, size, DefaultFileElementSize)
        {

        }

        /// <summary>
        /// Creates a memory mapped huge array.
        /// </summary>
        /// <param name="file">The the memory mapped file.</param>
        /// <param name="size">The initial size of the array.</param>
        /// <param name="arraySize">The size of an indivdual array block.</param>
        public MemoryMappedHugeArrayUInt32(MemoryMappedFile file, long size, long arraySize)
            : base(file, 4, size, arraySize)
        {

        }

        /// <summary>
        /// Creates a new memory mapped accessor.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="sizeInBytes"></param>
        /// <returns></returns>
        protected override MemoryMappedAccessor<uint> CreateAccessor(MemoryMappedFile file, long sizeInBytes)
        {
            return file.CreateUInt32(sizeInBytes);
        }
    }
}
