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
    /// A wrapper around the native memory mapped file.
    /// </summary>
    internal class MemoryMappedFileWrapper : IMemoryMappedFile
    {
        /// <summary>
        /// Holds the mapped file.
        /// </summary>
        private MemoryMappedFile _file;

        /// <summary>
        /// Creates a new memory mapped file wrapper.
        /// </summary>
        /// <param name="file"></param>
        public MemoryMappedFileWrapper(MemoryMappedFile file)
        {
            _file = file;
        }

        /// <summary>
        /// Creates a MemoryMappedViewAccessor that maps to a view of the memory-mapped file, and that has the specified offset and size.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IMemoryMappedViewAccessor CreateViewAccessor(long offset, long size)
        {
            return new MemoryMappedViewAccessorWrapper(_file.CreateViewAccessor(offset, size));
        }

        /// <summary>
        /// Disposes of all native resources associated with this object.
        /// </summary>
        public void Dispose()
        {
            _file.Dispose();
            _file = null;
        }

        /// <summary>
        /// Returns the size of the structure represented by T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public long GetSizeOf<T>() where T : struct
        {
            return System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));
        }
    }
}
