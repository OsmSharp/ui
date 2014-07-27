using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Collections
{
    /// <summary>
    /// An array working around the pre .NET 4.5 memory limitations for one object.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class HugeArray<T>
    {
        /// <summary>
        /// Holds the arrays.
        /// </summary>
        private T[][] _arrays;

        /// <summary>
        /// Holds the maximuma array size.
        /// </summary>
        private int _arraySize = 2 ^ 30;

        /// <summary>
        /// Holds the size of this array.
        /// </summary>
        private long _size;

        /// <summary>
        /// Creates a new huge array.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="arraySize"></param>
        public HugeArray(long size, int arraySize)
        {
            _arraySize = arraySize;
            _size = size;

            long arrayCount = (long)System.Math.Ceiling((double)size / _arraySize);
            _arrays = new T[arrayCount][];
            for (int arrayIdx = 0; arrayIdx < arrayCount - 1; arrayIdx++)
            {
                _arrays[arrayIdx] = new T[_arraySize];
            }
            _arrays[arrayCount - 1] = new T[size - ((arrayCount - 1) * _arraySize)];
        }

        /// <summary>
        /// Creates a new huge array.
        /// </summary>
        /// <param name="size"></param>
        public HugeArray(long size)
            : this(size, 2 ^ 30)
        {

        }

        /// <summary>
        /// Gets or sets the element at the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public T this[long idx]
        {
            get
            {
                long arrayIdx = (long)System.Math.Floor(idx / _arraySize);
                long localIdx = idx % _arraySize;
                return _arrays[arrayIdx][localIdx];
            }
            set
            {
                long arrayIdx = (long)System.Math.Floor(idx / _arraySize);
                long localIdx = idx % _arraySize;
                _arrays[arrayIdx][localIdx] = value;
            }
        }

        /// <summary>
        /// Resizes this array.
        /// </summary>
        /// <param name="size"></param>
        public void Resize(long size)
        {
            _size = size;

            long arrayCount = (long)System.Math.Ceiling((double)size / _arraySize);
            if (arrayCount != _arrays.Length)
            {
                Array.Resize<T[]>(ref _arrays, (int)arrayCount);
            }
            for (int arrayIdx = 0; arrayIdx < arrayCount - 1; arrayIdx++)
            {
                if (_arrays[arrayIdx] == null)
                { // there is no array, create it.
                    _arrays[arrayIdx] = new T[_arraySize];
                }
                if (_arrays[arrayIdx].Length != _arraySize)
                { // the size is the same, keep it as it.
                    var localArray = _arrays[arrayIdx];
                    Array.Resize<T>(ref localArray, (int)_arraySize);
                    _arrays[arrayIdx] = localArray;
                }
            }
            var lastArraySize = size - ((arrayCount - 1) * _arraySize);
            if (_arrays[arrayCount - 1] == null)
            { // there is no array, create it.
                _arrays[arrayCount - 1] = new T[lastArraySize];
            }
            if (_arrays[arrayCount - 1].Length != lastArraySize)
            { // the size is the same, keep it as it.
                var localArray = _arrays[arrayCount - 1];
                Array.Resize<T>(ref localArray, (int)lastArraySize);
                _arrays[arrayCount - 1] = localArray;
            }
        }

        /// <summary>
        /// Returns the length of this array.
        /// </summary>
        public long Length
        {
            get
            {
                return _size;
            }
        }
    }
}