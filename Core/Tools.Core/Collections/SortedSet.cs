// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// Foobar is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// Foobar is distributed in the hope that it will be useful,
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

namespace Tools.Core.Collections
{
    /// <summary>
    /// Represents a strongly typed list of objects that will be sorted using the IComparable interface.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    public class SortedSet<T> : ICollection<T>
        where T : IComparable<T>
    {
        /// <summary>
        /// The list containing the elements.
        /// </summary>
        private List<T> _elements;

        /// <summary>
        /// Creates a new sorted set.
        /// </summary>
        public SortedSet()
        {
            _elements = new List<T>();
        }  

        #region ICollection<T> Members

        /// <summary>
        /// Adds an item to the <see cref="SortedSet{T}"/>.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
                // intialize
                int idx_lower = 0;
                int idx_upper = this.Count;
                bool up = true;

                // loop
                int window_size;
                int idx_to_test;
                while (idx_upper - idx_lower > 0)
                {
                    // (re)calculate the windowsize.
                    window_size = idx_upper - idx_lower;
                    // divide by two
                    // and round the value (Floor function).
                    idx_to_test = (window_size / 2) + idx_lower;

                    // test the element at the given index.
                    up = _elements[idx_to_test].CompareTo(item) < 0;

                    // update the index
                    if (up)
                    {
                        idx_lower = idx_to_test + 1;
                    }
                    else
                    {
                        idx_upper = idx_to_test;
                    }
                }

                // insert
                _elements.Insert(idx_lower, item);
        }

        /// <summary>
        /// Removes all items from the <see cref="SortedSet{T}"/>.
        /// </summary>
        public void Clear()
        {
            _elements.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="SortedSet{T}"/> contains
        ///     a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="SortedSet{T}"/>.</param>
        /// <returns>true if item is found in the <see cref="SortedSet{T}"/>; otherwise,
        ///     false.</returns>
        public bool Contains(T item)
        {
            return _elements.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <see cref="SortedSet{T}"/> to an
        ///     System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements
        ///     copied from <see cref="SortedSet{T}"/>. The System.Array must
        ///     have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int idx = 0; idx < this.Count; idx++)
            {
                array[idx + arrayIndex] = _elements[idx];
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="SortedSet{T}"/>.
        /// </summary>
        public int Count
        {
            get 
            {
                return _elements.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="SortedSet{T}"/>
        //     is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get 
            {
                return false;
            }
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="SortedSet{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="SortedSet{T}"/>.</param>
        /// <returns>true if item was successfully removed from the <see cref="SortedSet{T}"/>;
        ///     otherwise, false. This method also returns false if item is not found in
        ///     the original <see cref="SortedSet{T}"/>.</returns>
        public bool Remove(T item)
        {
            return _elements.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A System.Collections.Generic.IEnumerator<T> that can be used to iterate through
        ///     the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator object that can be used to iterate through
        ///     the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
