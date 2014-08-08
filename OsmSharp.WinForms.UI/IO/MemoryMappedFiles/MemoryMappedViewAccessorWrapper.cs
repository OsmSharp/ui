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

using OsmSharp.IO.MemoryMappedFiles;
using System.IO.MemoryMappedFiles;

namespace OsmSharp.WinForms.UI.IO.MemoryMappedFiles
{
    /// <summary>
    /// A wrapper around the MemoryMappedViewAccessor
    /// </summary>
    internal class MemoryMappedViewAccessorWrapper : IMemoryMappedViewAccessor
    {
        /// <summary>
        /// Holds the accessor.
        /// </summary>
        private MemoryMappedViewAccessor _accessor;

        /// <summary>
        /// Creates a new memory mapped view accessor wrapper.
        /// </summary>
        /// <param name="accessor"></param>
        public MemoryMappedViewAccessorWrapper(MemoryMappedViewAccessor accessor)
        {
            _accessor = accessor;
        }

        /// <summary>
        /// Determines whether the accessor is readable.
        /// </summary>
        public bool CanRead
        {
            get { return _accessor.CanRead; }
        }

        /// <summary>
        /// Determines whether the accessory is writable.
        /// </summary>
        public bool CanWrite
        {
            get { return _accessor.CanWrite; }
        }

        /// <summary>
        /// Gets the capacity of the accessor.
        /// </summary>
        public long Capacity
        {
            get { return _accessor.Capacity; }
        }

        /// <summary>
        /// Reads a structure of type T from the accessor into a provided reference.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <param name="structure"></param>
        public void Read<T>(long position, out T structure) where T : struct
        {
            _accessor.Read<T>(position, out structure);
        }

        /// <summary>
        /// Reads structures of type T from the accessor into an array of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int ReadArray<T>(long position, T[] array, int offset, int count) where T : struct
        {
            return _accessor.ReadArray<T>(position, array, offset, count);
        }

        /// <summary>
        /// Writes a structure into the accessor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <param name="structure"></param>
        public void Write<T>(long position, ref T structure) where T : struct
        {
            _accessor.Write<T>(position, ref structure);
        }

        /// <summary>
        /// Writes structures from an array of type T into the accessor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <param name="array"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void WriteArray<T>(long position, T[] array, int offset, int count) where T : struct
        {
            _accessor.WriteArray<T>(position, array, offset, count);
        }

        public void Dispose()
        {
            _accessor.Dispose();
            _accessor = null;
        }
    }
}
