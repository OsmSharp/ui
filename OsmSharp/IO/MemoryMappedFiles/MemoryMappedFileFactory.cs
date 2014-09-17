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

namespace OsmSharp.IO.MemoryMappedFiles
{
    /// <summary>
    /// Represents a memory mapped file factory.
    /// </summary>
    public abstract class MemoryMappedFileFactory<T> : IDisposable
        where T : struct
    {
        /// <summary>
        /// Holds all files generated using this factory.
        /// </summary>
        private List<IDisposable> _files;

        /// <summary>
        /// Creates a new memory mapped file factory.
        /// </summary>
        public MemoryMappedFileFactory()
        {
            _files = new List<IDisposable>();
        }

        /// <summary>
        /// Returns the size of the given struct on disk.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public abstract int SizeOf();

        /// <summary>
        /// Creates a new memory mapped file based on the settings in this factory.
        /// </summary>
        /// <param name="sizeInBytes">The size in bytes.</param>
        /// <returns></returns>
        public IMemoryMappedFile<T> New(long sizeInBytes)
        {
            var newFile = this.DoCreateNew(sizeInBytes);
            _files.Add(newFile);
            return newFile;
        }

        /// <summary>
        /// Creates a new memory mapped file based on the settings in this factory.
        /// </summary>
        /// <param name="sizeInBytes">The size in bytes.</param>
        /// <returns></returns>
        public abstract IMemoryMappedFile<T> DoCreateNew(long sizeInBytes);

        /// <summary>
        /// Disposes of all resources associated with this files.
        /// </summary>
        public void Dispose()
        {
            foreach (var file in _files)
            {
                file.Dispose();
            }
            _files.Clear();
        }
    }
}