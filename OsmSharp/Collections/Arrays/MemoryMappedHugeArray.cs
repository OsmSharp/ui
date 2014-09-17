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
using System;
using System.Collections.Generic;

namespace OsmSharp.Collections.Arrays
{
    /// <summary>
    /// Represents a memory mapped huge array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MemoryMappedHugeArray<T> : IHugeArray<T>
        where T : struct
    {
        /// <summary>
        /// Holds the length of this array.
        /// </summary>
        private long _length;

        /// <summary>
        /// Holds the factory to create the memory mapped files.
        /// </summary>
        private MemoryMappedFileFactory<T> _factory;

        /// <summary>
        /// Holds the list of files for each range.
        /// </summary>
        private List<IMemoryMappedFile<T>> _files;

        /// <summary>
        /// Holds the list of accessors for each file.
        /// </summary>
        private List<IMemoryMappedViewAccessor<T>> _accessors;

        /// <summary>
        /// Holds the default file element size.
        /// </summary>
        private static long DefaultFileElementSize = (long)1024 * (long)1024 * (long)128;

        /// <summary>
        /// Holds the file element size.
        /// </summary>
        private long _fileElementSize = DefaultFileElementSize;

        /// <summary>
        /// Holds the element size.
        /// </summary>
        private int _elementSize;

        /// <summary>
        /// Holds the maximum array size in bytes.
        /// </summary>
        private long _fileSizeBytes;

        /// <summary>
        /// Creates a memory mapped huge array.
        /// </summary>
        /// <param name="factory">The factory to create the memory mapped files.</param>
        /// <param name="size">The size of the array.</param>
        public MemoryMappedHugeArray(MemoryMappedFileFactory<T> factory, long size)
            : this(factory, size, DefaultFileElementSize)
        {

        }

        /// <summary>
        /// Creates a memory mapped huge array.
        /// </summary>
        /// <param name="factory">The factory to create the memory mapped files.</param>
        /// <param name="size">The size of the array.</param>
        /// <param name="arraySize">The size of an indivdual array block.</param>
        public MemoryMappedHugeArray(MemoryMappedFileFactory<T> factory, long size, long arraySize)
        {
            _factory = factory;
            _length = size;
            _fileElementSize = arraySize;
            _elementSize = factory.SizeOf();
            _fileSizeBytes = arraySize * _elementSize;

            var arrayCount = (int)System.Math.Ceiling((double)size / _fileElementSize);
            _files = new List<IMemoryMappedFile<T>>(arrayCount);
            _accessors = new List<IMemoryMappedViewAccessor<T>>(arrayCount);
            for (int arrayIdx = 0; arrayIdx < arrayCount; arrayIdx++)
            {
                var file = _factory.New(_fileSizeBytes);
                _files.Add(file);
                _accessors.Add(file.CreateViewAccessor(0, _fileSizeBytes));
            }
        }

        /// <summary>
        /// Returns the length of this array.
        /// </summary>
        public long Length
        {
            get { return _length; }
        }

        /// <summary>
        /// Resizes this array.
        /// </summary>
        /// <param name="size"></param>
        public void Resize(long size)
        {
            _length = size;

            var arrayCount = (int)System.Math.Ceiling((double)size / _fileElementSize);
            _files = new List<IMemoryMappedFile<T>>(arrayCount);
            if (arrayCount < _files.Count)
            { // decrease files/accessors.
                for (int arrayIdx = (int)arrayCount; arrayIdx < _files.Count; arrayIdx++)
                {
                    _accessors[arrayIdx].Dispose();
                    _accessors[arrayIdx] = null;
                    _files[arrayIdx].Dispose();
                    _files[arrayIdx] = null;
                }
                _files.RemoveRange((int)arrayCount, (int)(_files.Count - arrayCount));
            }
            else
            { // increase files/accessors.
                for (int arrayIdx = _files.Count; arrayIdx < arrayCount; arrayIdx++)
                {
                    var file = _factory.New(_fileSizeBytes);
                    _files.Add(file);
                    _accessors.Add(file.CreateViewAccessor(0, _fileSizeBytes));
                }
            }
        }

        /// <summary>
        /// Returns the element at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public T this[long idx]
        {
            get
            {
                long arrayIdx = (long)System.Math.Floor(idx / _fileElementSize);
                long localIdx = idx % _fileElementSize;
                long localPosition = localIdx * _elementSize;
                T structure;
                _accessors[(int)arrayIdx].Read(localPosition, out structure);
                return structure;
            }
            set
            {
                long arrayIdx = (long)System.Math.Floor(idx / _fileElementSize);
                long localIdx = idx % _fileElementSize;
                long localPosition = localIdx * _elementSize;
                _accessors[(int)arrayIdx].Write(localPosition, ref value);
            }
        }

        /// <summary>
        /// Diposes of all native resource associated withh this array.
        /// </summary>
        public void Dispose()
        {
            foreach (var accessor in _accessors)
            {
                accessor.Dispose();
            }
            _accessors.Clear();
            foreach(var file in _files)
            {
                file.Dispose();
            }
            _files.Clear();
        }
    }
}
