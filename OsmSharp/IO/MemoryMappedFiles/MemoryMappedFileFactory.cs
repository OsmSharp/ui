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
    public class MemoryMappedFileFactory : IDisposable
    {
        /// <summary>
        /// Holds the base-path if any.
        /// </summary>
        private string _path;

        /// <summary>
        /// Holds all files generated using this factory.
        /// </summary>
        private List<IMemoryMappedFile> _files;

        /// <summary>
        /// Creates a new memory mapped file factory.
        /// </summary>
        public MemoryMappedFileFactory()
        {
            _path = null;
            _files = new List<IMemoryMappedFile>();
        }

        /// <summary>
        /// Creates a new memory mapped file factory.
        /// </summary>
        /// <param name="path"></param>
        public MemoryMappedFileFactory(string path)
        {
            _path = path;
            _files = new List<IMemoryMappedFile>();
        }

        /// <summary>
        /// Creates a new memory mapped file based on the settings in this factory.
        /// </summary>
        /// <param name="sizeInBytes">The size in bytes.</param>
        /// <returns></returns>
        public IMemoryMappedFile New(long sizeInBytes)
        {
            IMemoryMappedFile newFile = null;
            if (!string.IsNullOrEmpty(_path))
            {
                newFile =  NativeMemoryMappedFileFactory.CreateFromFile(_path + System.Guid.NewGuid().ToString(), sizeInBytes);
            }
            else
            {
                newFile =  NativeMemoryMappedFileFactory.CreateNew(System.Guid.NewGuid().ToString(), sizeInBytes);
            }
            _files.Add(newFile);
            return newFile;
        }

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