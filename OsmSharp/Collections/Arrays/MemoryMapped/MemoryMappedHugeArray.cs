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
using System.Collections.Generic;

namespace OsmSharp.Collections.Arrays.MemoryMapped
{
    /// <summary>
    /// Represents a memory mapped huge array.
    /// </summary>
    public abstract class MemoryMappedHugeArray<T> : IHugeArray<T>
        where T : struct
    {
        /// <summary>
        /// Holds the length of this array.
        /// </summary>
        private long _length;

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
        public static long DefaultFileElementSize = (long)1024 * (long)1024;

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
        /// <param name="file">The the memory mapped file.</param>
        /// <param name="elementSize">The element size.</param>
        /// <param name="size">The initial size of the array.</param>
        /// <param name="arraySize">The size of an indivdual array block.</param>
        public MemoryMappedHugeArray(MemoryMappedFile file, int elementSize, long size, long arraySize)
        {
            _file = file;
            _length = size;
            _fileElementSize = arraySize;
            _elementSize = elementSize;
            _fileSizeBytes = arraySize * _elementSize;

            var arrayCount = (int)System.Math.Ceiling((double)size / _fileElementSize);
            _accessors = new List<MemoryMappedAccessor<T>>(arrayCount);
            for (int arrayIdx = 0; arrayIdx < arrayCount; arrayIdx++)
            {
                _accessors.Add(this.CreateAccessor(_file, _fileSizeBytes));
            }
        }

        /// <summary>
        /// Creates a new memory mapped accessor.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="sizeInBytes"></param>
        /// <returns></returns>
        protected abstract MemoryMappedAccessor<T> CreateAccessor(MemoryMappedFile file, long sizeInBytes);

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
            _accessors = new List<MemoryMappedAccessor<T>>(arrayCount);
            if (arrayCount < _accessors.Count)
            { // decrease files/accessors.
                for (int arrayIdx = (int)arrayCount; arrayIdx < _accessors.Count; arrayIdx++)
                {
                    _accessors[arrayIdx].Dispose();
                    _accessors[arrayIdx] = null;
                }
                _accessors.RemoveRange((int)arrayCount, (int)(_accessors.Count - arrayCount));
            }
            else
            { // increase files/accessors.
                for (int arrayIdx = _accessors.Count; arrayIdx < arrayCount; arrayIdx++)
                {
                    _accessors.Add(this.CreateAccessor(_file, _fileSizeBytes));
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
        /// Diposes of all native resource associated with this array.
        /// </summary>
        public void Dispose()
        {
            // disposing the file will also dispose of all undisposed accessors, and accessor cannot exist without a file.
            _file.Dispose();
        }
    }
}
