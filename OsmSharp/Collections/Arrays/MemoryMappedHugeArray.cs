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
        /// Holds the path to store the memory mapped files.
        /// </summary>
        private string _path;

        /// <summary>
        /// Holds the list of files for each range.
        /// </summary>
        private List<IMemoryMappedFile> _files;

        /// <summary>
        /// Holds the list of accessors for each file.
        /// </summary>
        private List<IMemoryMappedViewAccessor> _accessors;

        /// <summary>
        /// Holds the maximum array size.
        /// </summary>
        private int _fileElementSize = (int)System.Math.Pow(2, 20);

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
        /// <param name="size">The size of the array.</param>
        public MemoryMappedHugeArray(long size)
            : this(size, (int)System.Math.Pow(2, 20))
        {

        }

        /// <summary>
        /// Creates a memory mapped huge array.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="arraySize"></param>
        public MemoryMappedHugeArray(long size, int arraySize)
            : this(null, size, arraySize)
        {

        }

        /// <summary>
        /// Creates a memory mapped huge array.
        /// </summary>
        /// <param name="path">The path to the memory mapped files.</param>
        /// <param name="size">The size of the array.</param>
        public MemoryMappedHugeArray(string path, long size)
            : this(path, size, (int)System.Math.Pow(2, 20))
        {

        }

        /// <summary>
        /// Creates a memory mapped huge array.
        /// </summary>
        /// <param name="path">The path to the memory mapped files.</param>
        /// <param name="size">The size of the array.</param>
        /// <param name="arraySize">The size of an indivdual array block.</param>
        public MemoryMappedHugeArray(string path, long size, int arraySize)
        {
            _path = path;
            _length = size;
            _fileElementSize = arraySize;
            _elementSize = NativeMemoryMappedFileFactory.GetSize(typeof(T));
            _fileSizeBytes = arraySize * _elementSize;

            var arrayCount = (int)System.Math.Ceiling((double)size / _fileElementSize);
            _files = new List<IMemoryMappedFile>(arrayCount);
            _accessors = new List<IMemoryMappedViewAccessor>(arrayCount);
            for (int arrayIdx = 0; arrayIdx < arrayCount; arrayIdx++)
            {
                var file = this.CreateNew();
                _files.Add(file);
                _accessors.Add(file.CreateViewAccessor(0, _fileSizeBytes));
            }
        }

        /// <summary>
        /// Helper method to create a new memory mapped file.
        /// </summary>
        /// <returns></returns>
        private IMemoryMappedFile CreateNew()
        {
            if(string.IsNullOrEmpty(_path))
            {
                return NativeMemoryMappedFileFactory.CreateFromFile(_path + System.Guid.NewGuid().ToString(), _fileSizeBytes);
            }
            return NativeMemoryMappedFileFactory.CreateNew(System.Guid.NewGuid().ToString(), _fileSizeBytes);
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
            _files = new List<IMemoryMappedFile>(arrayCount);
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
                    var file = this.CreateNew();
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
                _accessors[(int)arrayIdx].Read<T>(localPosition, out structure);
                return structure;
            }
            set
            {
                long arrayIdx = (long)System.Math.Floor(idx / _fileElementSize);
                long localIdx = idx % _fileElementSize;
                long localPosition = localIdx * _elementSize;
                _accessors[(int)arrayIdx].Write<T>(localPosition, ref value);
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
