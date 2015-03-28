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
using System;
using System.Collections.Generic;

namespace OsmSharp.Collections.Indexes.MemoryMapped
{
    /// <summary>
    /// A memory-mapped implementation of an index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class MemoryMappedIndex<T> : Index<T>
        where T : struct
    {
        /// <summary>
        /// Holds the file to create the memory mapped accessors.
        /// </summary>
        private MemoryMappedFile _file;

        /// <summary>
        /// Holds the list of accessors, one for each range.
        /// </summary>
        private List<MemoryMappedAccessor<T>> _accessors;

        /// <summary>
        /// Holds the default file element size.
        /// </summary>
        public static long DefaultFileSize = (long)1024 * (long)1024 * (long)64;

        /// <summary>
        /// Holds the read-from delegate.
        /// </summary>
        private MemoryMappedFile.ReadFromDelegate<T> _readFrom;

        /// <summary>
        /// Holds the read-to delegate.
        /// </summary>
        private MemoryMappedFile.WriteToDelegate<T> _writeTo;

        /// <summary>
        /// Creates a new memory-mapped index.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="readFrom"></param>
        /// <param name="writeTo"></param>
        public MemoryMappedIndex(MemoryMappedFile file, MemoryMappedFile.ReadFromDelegate<T> readFrom,
            MemoryMappedFile.WriteToDelegate<T> writeTo)
        {
            if (file == null) { throw new ArgumentNullException("file"); }
            if (readFrom == null) { throw new ArgumentNullException("readFrom"); }
            if (writeTo == null) { throw new ArgumentNullException("writeTo"); }

            _file = file;
            _readFrom = readFrom;
            _writeTo = writeTo;
            _position = 0;

            _accessors = new List<MemoryMappedAccessor<T>>();
            _accessors.Add(this.CreateAccessor(_file, DefaultFileSize));
        }

        /// <summary>
        /// Holds the current position.
        /// </summary>
        private long _position;

        /// <summary>
        /// Creates a variable-sized accessor.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="sizeInBytes"></param>
        /// <returns></returns>
        private MemoryMappedAccessor<T> CreateAccessor(MemoryMappedFile file, long sizeInBytes)
        {
            return file.CreateVariable<T>(sizeInBytes, _readFrom, _writeTo);
        }

        /// <summary>
        /// Adds a new element to this index.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>The id of the object.</returns>
        public override long Add(T element)
        {
            var id = _position;
            var accessorId = (int)System.Math.Floor(_position / DefaultFileSize);
            if (accessorId == _accessors.Count)
            { // add new accessor.
                _accessors.Add(this.CreateAccessor(_file, DefaultFileSize));
            }
            var accessor = _accessors[accessorId];
            var size = accessor.Write(_position, ref element);
            if(size < 0)
            { // writing failed.
                // add new accessor.
                _accessors.Add(this.CreateAccessor(_file, DefaultFileSize));
                accessorId++;
                accessor = _accessors[accessorId];
                size = accessor.Write(_position, ref element);
            }
            _position = _position + size;
            return id;
        }

        /// <summary>
        /// Tries to get an element with the given id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="element">The element.</param>
        /// <returns>True if an element with the given id was found.</returns>
        public override bool TryGet(long id, out T element)
        {
            var accessorId = (int)System.Math.Floor(id / DefaultFileSize);
            if (accessorId >= _accessors.Count)
            {
                element = default(T);
                return false;
            }
            var localPosition = (id - (DefaultFileSize * accessorId));
            _accessors[accessorId].Read(localPosition, out element);
            return true;
        }

        /// <summary>
        /// Gets the element with the given id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The element.</returns>
        public override T Get(long id)
        {
            T element;
            if(!this.TryGet(id, out element))
            {
                throw new KeyNotFoundException();
            }
            return element;
        }
    }
}
