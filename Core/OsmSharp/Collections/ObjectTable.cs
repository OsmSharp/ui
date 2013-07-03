// OsmSharp - OpenStreetMap tools & library.
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
        private Dictionary<Type, uint> _reverse_index;

        /// <summary>
        /// Holds the initial capacity and is also used as an allocation step.
        /// </summary>
        private int _init_capacity;

        /// <summary>
        /// Holds the next idx.
        /// </summary>
        private uint _next_idx = 0;

        /// <summary>
        /// Creates a new string table.
        /// </summary>
        /// <param name="reverse_index">The reverse index is enable if true.</param>
        public ObjectTable(bool reverse_index)
            :this(reverse_index, 1000)
        {

        }

        /// <summary>
        /// Creates a new string table.
        /// </summary>
        /// <param name="reverse_index">The reverse index is enable if true.</param>
        /// <param name="init_capacity"></param>
        public ObjectTable(bool reverse_index, int init_capacity)
        {
            _strings = new Type[init_capacity];
            _init_capacity = init_capacity;

            if (reverse_index)
            {
                this.BuildReverseIndex();
            }
        }

        #region Reverse Index

        /// <summary>
        /// Builds the reverse index.
        /// </summary>
        public void BuildReverseIndex()
        {
            _reverse_index = new Dictionary<Type, uint>();
            for(uint idx = 0; idx < _strings.Length; idx++)
            {
                Type value = _strings[idx];
                if (value != null)
                {
                    _reverse_index[value] = idx;
                }
            }
        }

        /// <summary>
        /// Drops the reverse index.
        /// </summary>
        public void DropReverseIndex()
        {
            _reverse_index = null;
        }

        #endregion

        #region Table

        private uint AddString(Type value)
        {
            uint value_int = _next_idx;

            if (_strings.Length <= _next_idx)
            { // the string table is not big enough anymore.
                Array.Resize<Type>(ref _strings, _strings.Length + _init_capacity);
            }
            _strings[_next_idx] = value;

            if (_reverse_index != null)
            {
                _reverse_index[value] = _next_idx;
            }

            _next_idx++;
            return value_int;
        }

        #endregion

        /// <summary>
        /// Returns an index for the given string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public uint Add(Type value)
        {
            uint value_int;
            if (_reverse_index != null)
            { // add string based on the reverse index, is faster.
                if (!_reverse_index.TryGetValue(value, out value_int))
                { // string was not found.
                    value_int = this.AddString(value);
                }
            }
            else
            {
                int idx = Array.IndexOf<Type>(_strings, value); // this is O(n), a lot worse compared to the best-case O(1).
                if (idx < 0)
                { // string was not found.
                    value_int = this.AddString(value);
                }
                else
                { // string was found.
                    value_int = (uint)idx;
                }
            }
            return value_int;
        }

        /// <summary>
        /// Returns a string given it's encoded index.
        /// </summary>
        /// <param name="value_idx"></param>
        /// <returns></returns>
        public Type Get(uint value_idx)
        {
            return _strings[value_idx];
        }
    }
}