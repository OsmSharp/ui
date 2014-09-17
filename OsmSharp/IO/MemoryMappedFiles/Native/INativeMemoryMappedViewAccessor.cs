// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2014 Abelshausen Ben
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

namespace OsmSharp.IO.MemoryMappedFiles
{
    /// <summary>
    /// Abstract representation of a memory-mapped accessor: Provides random access to unmanaged blocks of memory from managed code.
    /// </summary>
    public interface INativeMemoryMappedViewAccessor : IDisposable
    {
        /// <summary>
        /// Determines whether the accessor is readable.
        /// </summary>
        bool CanRead { get; }

        /// <summary>
        /// Determines whether the accessory is writable.
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        /// Gets the capacity of the accessor.
        /// </summary>
        long Capacity { get; }

        /// <summary>
        /// Reads a structure of type T from the accessor into a provided reference.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <param name="structure"></param>
        void Read<T>(long position, out T structure) where T : struct;

        /// <summary>
        /// Reads structures of type T from the accessor into an array of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        int ReadArray<T>(long position, T[] array, int offset, int count) where T : struct;

        /// <summary>
        /// Writes a structure into the accessor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <param name="structure"></param>
        void Write<T>(long position, ref T structure) where T : struct;

        /// <summary>
        /// Writes structures from an array of type T into the accessor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        void WriteArray<T>(long position, T[] array, int offset, int count) where T : struct;
    }
}