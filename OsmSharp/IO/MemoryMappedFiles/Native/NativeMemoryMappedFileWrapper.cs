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

namespace OsmSharp.IO.MemoryMappedFiles.Native
{
    /// <summary>
    /// A native memory mapped file wrapper.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NativeMemoryMappedFileWrapper<T> : IMemoryMappedFile<T>
        where T : struct
    {
        /// <summary>
        /// Holds the native memory mapped file.
        /// </summary>
        private INativeMemoryMappedFile _nativeFile;

        /// <summary>
        /// Creates a new native memory mapped file wrapper.
        /// </summary>
        /// <param name="nativeFile"></param>
        public NativeMemoryMappedFileWrapper(INativeMemoryMappedFile nativeFile)
        {
            _nativeFile = nativeFile;
        }

        /// <summary>
        /// Creates a new view accessor.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IMemoryMappedViewAccessor<T> CreateViewAccessor(long offset, long size)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the size of the given struct on disk.
        /// </summary>
        /// <returns></returns>
        public long GetSizeOf()
        {
            return _nativeFile.GetSizeOf<T>();
        }

        /// <summary>
        /// Disposes the resources associated with this wrapper.
        /// </summary>
        public void Dispose()
        {
            _nativeFile.Dispose();
        }
    }
}
