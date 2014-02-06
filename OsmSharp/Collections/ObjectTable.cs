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
using System.Linq;
using System.Text;

namespace OsmSharp.Collections
{
    /// <summary>
    /// An object table containing and index of object to reduce memory usage by preventing duplicates.
    /// </summary>
    public class ObjectTable<Type>
    {
        /// <summary>
        /// The array containing all strings.
        /// </summary>
        private Type[] _strings;

        /// <summary>
        /// A dictionary containing the index of each string.
        /// </summary>
        private Dictionary<Type, uint> _reverseIndex;

        /// <summary>
        /// Holds a custom comparer.
        /// </summary>
        private IEqualityComparer<Type> _comparer;

        /// <summary>
        /// Holds the initial capacity and is also used as an allocation step.
        /// </summary>
        private int _initCapacity;

        /// <summary>
        /// Holds the next idx.
        /// </summary>
        private uint _nextIdx = 0;

        /// <summary>
        /// Creates a new string table.
        /// </summary>
        /// <param name="reverseIndex">The reverse index is enable if true.</param>
        public ObjectTable(bool reverseIndex)
            :this(reverseIndex, 1000)
        {

        }

        /// <summary>
        /// Creates a new string table.
        /// </summary>
        /// <param name="reverseIndex">The reverse index is enable if true.</param>
        /// <param name="comparer">A custom comparer.</param>
        public ObjectTable(bool reverseIndex, IEqualityComparer<Type> comparer)
            :this(reverseIndex, 1000, comparer)
        {

        }

        /// <summary>
        /// Creates a new string table.
        /// </summary>
        /// <param name="reverseIndex">The reverse index is enable if true.</param>
        /// <param name="initCapacity">The initial capacity estimate.</param>
        public ObjectTable(bool reverseIndex, int initCapacity)
            : this(reverseIndex, initCapacity, null)
        {

        }

        /// <summary>
        /// Creates a new string table.
        /// </summary>
        /// <param name="reverseIndex">The reverse index is enable if true.</param>
        /// <param name="initCapacity">The initial capacity estimate.</param>
        /// <param name="comparer">A custom comparer.</param>
        public ObjectTable(bool reverseIndex, int initCapacity, IEqualityComparer<Type> comparer)
        {
            _strings = new Type[initCapacity];
            _initCapacity = initCapacity;
            _comparer = comparer;
            if (reverseIndex)
            { // build a reverse index.
                this.BuildReverseIndex();
            }
        }

        /// <summary>
        /// Clears all data from this object table.
        /// </summary>
        public void Clear()
        {
            _strings = new Type[_initCapacity];
            _nextIdx = 0;
            if (_reverseIndex != null)
            {
                _reverseIndex.Clear();
            }
        }

        #region Reverse Index

        /// <summary>
        /// Builds the reverse index.
        /// </summary>
        public void BuildReverseIndex()
        {
            if (_comparer != null)
            { // there is a custom comparer.
                _reverseIndex = new Dictionary<Type, uint>(_comparer);
            }
            else
            { // no custom comparer.
                _reverseIndex = new Dictionary<Type, uint>();
            }
            for(uint idx = 0; idx < _nextIdx; idx++)
            {
                Type value = _strings[idx];
                if (value != null)
                {
                    _reverseIndex[value] = idx;
                }
            }
        }

        /// <summary>
        /// Drops the reverse index.
        /// </summary>
        public void DropReverseIndex()
        {
            _reverseIndex = null;
        }

        #endregion

        #region Table

        private uint AddString(Type value)
        {
            uint value_int = _nextIdx;

            if (_strings.Length <= _nextIdx)
            { // the string table is not big enough anymore.
                Array.Resize<Type>(ref _strings, _strings.Length + _initCapacity);
            }
            _strings[_nextIdx] = value;

            if (_reverseIndex != null)
            {
                _reverseIndex[value] = _nextIdx;
            }

            _nextIdx++;
            return value_int;
        }

        #endregion

        /// <summary>
        /// Returns the highest id in this object table.
        /// </summary>
        public uint Count
        {
            get
            {
                return _nextIdx;
            }
        }

        /// <summary>
        /// Returns an index for the given string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public uint Add(Type value)
        {
            uint valueInt;
            if (_reverseIndex != null)
            { // add string based on the reverse index, is faster.
                if (!_reverseIndex.TryGetValue(value, out valueInt))
                { // string was not found.
                    valueInt = this.AddString(value);
                }
            }
            else
            {
                int idx = Array.IndexOf<Type>(_strings, value); // this is O(n), a lot worse compared to the best-case O(1).
                if (idx < 0)
                { // string was not found.
                    valueInt = this.AddString(value);
                }
                else
                { // string was found.
                    valueInt = (uint)idx;
                }
            }
            return valueInt;
        }

        /// <summary>
        /// Returns a string given it's encoded index.
        /// </summary>
        /// <param name="valueIdx"></param>
        /// <returns></returns>
        public Type Get(uint valueIdx)
        {
            return _strings[valueIdx];
        }

        /// <summary>
        /// Returns a copy of all data in this object table.
        /// </summary>
        /// <returns></returns>
        public Type[] ToArray()
        {
            Type[] copy = new Type[_nextIdx];
            for (int idx = 0; idx < _nextIdx; idx++)
            {
                copy[idx] = _strings[idx];
            }
            return copy;
        }
    }
}