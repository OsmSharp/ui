using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace OsmSharp.Tools.Collections
{
#if WINDOWS_PHONE
    /// <summary>
    /// A HashSet implementation for Windows Phone based just using the Dictionary key collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HashSet<T> : ICollection<T>
    {
        /// <summary>
        /// Holds the actual hashset data.
        /// </summary>
        private Dictionary<T, int> _keys;

        /// <summary>
        /// Creates a new hashset.
        /// </summary>
        public HashSet()
        {
            _keys = new Dictionary<T, int>();
        }

        /// <summary>
        /// Creates a new hashset.
        /// </summary>
        public HashSet(IEnumerable<T> initial_items)
        {
            _keys = new Dictionary<T, int>();

            foreach (T item in initial_items)
            {
                _keys[item] = 0;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _keys.Keys.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Adds a new item to this hashset.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            _keys[item] = 0;
        }

        /// <summary>
        /// Clears this hashset.
        /// </summary>
        public void Clear()
        {
            _keys.Clear();
        }

        /// <summary>
        /// Returns true if the item exists in this hashset.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return _keys.ContainsKey(item);
        }

        /// <summary>
        /// Copies the elements of the collection to an array, starting at a particular index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (T item in this)
            {
                array[arrayIndex] = item;
                arrayIndex++;
            }
        }

        /// <summary>
        /// Removes an item from this hashset.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            return _keys.Remove(item);
        }

        /// <summary>
        /// Returns the number of items in this hashset.
        /// </summary>
        public int Count
        {
            get
            {
                return _keys.Count;
            }
        }

        /// <summary>
        /// Returns true if the collection is readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
    }
#endif
}
