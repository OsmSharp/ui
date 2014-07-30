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
        /// Holds the memory mapped file.
        /// </summary>
        private IMemoryMappedFile _file;

        /// <summary>
        /// Holds the list of accessors for each range.
        /// </summary>
        private List<IMemoryMappedViewAccessor> _accessors;

        /// <summary>
        /// Holds the maximum array size.
        /// </summary>
        private int _arraySize = (int)System.Math.Pow(2, 20);

        /// <summary>
        /// Holds the size of only element in bytes.
        /// </summary>
        private long _elementSize = -1;

        /// <summary>
        /// Represents a memory mapped huge array.
        /// </summary>
        /// <param name="file">The memory mapped file to use.</param>
        /// <param name="size">The size of the array.</param>
        public MemoryMappedHugeArray(IMemoryMappedFile file, long size)
            : this(file, size, (int)System.Math.Pow(2, 20))
        {

        }

        /// <summary>
        /// Represents a memory mapped huge array.
        /// </summary>
        /// <param name="file">The memory mapped file to use.</param>
        /// <param name="size">The size of the array.</param>
        /// <param name="arraySize">The size of an indivdual array block.</param>
        public MemoryMappedHugeArray(IMemoryMappedFile file, long size, int arraySize)
        {
            _file = file;
            _length = size;
            _arraySize = arraySize;

            _elementSize = _file.GetSizeOf<T>();

            long arrayCount = (long)System.Math.Ceiling((double)size / _arraySize);
            _accessors = new List<IMemoryMappedViewAccessor>((int)arrayCount);
            long offset = 0;
            long accessorSize = _arraySize * _elementSize;
            for (int arrayIdx = 0; arrayIdx < arrayCount - 1; arrayIdx++)
            {
                _accessors.Add(_file.CreateViewAccessor(offset, accessorSize));
                offset = offset + accessorSize;
            }
            accessorSize = (size - ((arrayCount - 1) * _arraySize)) * _elementSize;
            _accessors.Add(_file.CreateViewAccessor(offset, accessorSize));
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

            long arrayCount = (long)System.Math.Ceiling((double)size / _arraySize);
            long accessorSize = _arraySize * _elementSize;
            if (arrayCount < _accessors.Count)
            {
                for(int arrayIdx = (int)arrayCount; arrayIdx < _accessors.Count; arrayIdx++)
                {
                    _accessors[arrayIdx].Dispose();
                    _accessors[arrayIdx] = null;
                }
                _accessors.RemoveRange((int)arrayCount, (int)(_accessors.Count - arrayCount));
            }
            long offset = accessorSize * 0;
            for (int arrayIdx = 0; arrayIdx < arrayCount - 1; arrayIdx++)
            {
                while(arrayIdx >= _accessors.Count)
                {
                    _accessors.Add(null);
                }
                offset = accessorSize * arrayIdx;
                if (_accessors[arrayIdx] == null)
                { // there is no array, create it.
                    _accessors[arrayIdx] = _file.CreateViewAccessor(offset, accessorSize);
                }
                if (_accessors[arrayIdx].Capacity != accessorSize)
                { // the size is not the same, replace it.
                    var localArray = _accessors[arrayIdx];
                    localArray.Dispose();
                    localArray = _file.CreateViewAccessor(offset, accessorSize);
                    _accessors[arrayIdx] = localArray;
                }
            }
            offset = (arrayCount - 1) * accessorSize;
            accessorSize = (size - ((arrayCount - 1) * _arraySize)) * _elementSize;
            while (arrayCount - 1 >= _accessors.Count)
            {
                _accessors.Add(null);
            }
            if (_accessors[(int)arrayCount - 1] == null)
            { // there is no array, create it.
                _accessors[(int)arrayCount - 1] = _file.CreateViewAccessor(offset, accessorSize);
            }
            if (_accessors[(int)arrayCount - 1].Capacity != accessorSize)
            { // the size is not the same, replace it.
                var localArray = _accessors[(int)arrayCount - 1];
                localArray.Dispose();
                localArray = _file.CreateViewAccessor(offset, accessorSize);
                _accessors[(int)arrayCount - 1] = localArray;
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
                long arrayIdx = (long)System.Math.Floor(idx / _arraySize);
                long localIdx = idx % _arraySize;
                long localPosition = localIdx * _elementSize;
                T structure;
                _accessors[(int)arrayIdx].Read<T>(localPosition, out structure);
                return structure;
            }
            set
            {
                long arrayIdx = (long)System.Math.Floor(idx / _arraySize);
                long localIdx = idx % _arraySize;
                long localPosition = localIdx * _elementSize;
                _accessors[(int)arrayIdx].Write<T>(localPosition, ref value);
            }
        }
    }
}
