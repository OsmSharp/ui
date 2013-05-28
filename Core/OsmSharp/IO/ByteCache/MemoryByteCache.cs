using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.IO.ByteCache
{
    /// <summary>
    /// An in-memory cache of byte arrays.
    /// </summary>
    public class MemoryByteCache : IByteCache
    {
        /// <summary>
        /// Holds the next id.
        /// </summary>
        private uint _nextId = 0;

        /// <summary>
        /// Holds the data.
        /// </summary>
        private Dictionary<uint, byte[]> _data = 
            new Dictionary<uint, byte[]>(); 

        /// <summary>
        /// Returns the size of this cache.
        /// </summary>
        public int Size
        {
            get { return _data.Count; }
        }

        /// <summary>
        /// Adds a new array.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public uint Add(byte[] data)
        {
            while (_data.ContainsKey(_nextId))
            { // no check for full cache but cache can still get HUGE.
                // this is to prevent long-running processes from stopping.
                if (_nextId == uint.MaxValue)
                {
                    _nextId = 0;
                }
                else
                {
                    _nextId++;
                }
            }
            _data[_nextId] = data;
            return _nextId;
        }

        /// <summary>
        /// Gets the data associated with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public byte[] Get(uint id)
        {
            byte[] data;
            if (_data.TryGetValue(id, out data))
            {
                return data;
            }
            throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Removes all data associated with the given id.
        /// </summary>
        /// <param name="id"></param>
        public void Remove(uint id)
        {
            _data.Remove(id);
        }

        /// <summary>
        /// Disposes of all resources used in this cache.
        /// </summary>
        public void Dispose()
        {
            _data.Clear();
            _data = null;
        }
    }
}
