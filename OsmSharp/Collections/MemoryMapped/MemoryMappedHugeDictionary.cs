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

using OsmSharp.Collections.Arrays.MemoryMapped;
using OsmSharp.Collections.Indexes.MemoryMapped;
using OsmSharp.IO.MemoryMappedFiles;
using System;
using System.Collections.Generic;

namespace OsmSharp.Collections.MemoryMapped
{
    /// <summary>
    /// A memory-mapped dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <remarks>Warning: This dictionary can only grow, once a key is added it can be removed but it will exist in the memory-mapped file.</remarks>
    public class MemoryMappedHugeDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDisposable
    {
        /// <summary>
        /// Holds the memory-mapped file.
        /// </summary>
        private MemoryMappedFile _file;

        /// <summary>
        /// The index holding all values.
        /// </summary>
        private MemoryMappedIndex<ValueStruct> _values;

        /// <summary>
        /// The index holding all keys.
        /// </summary>
        private MemoryMappedIndex<KeyStruct> _keys;

        /// <summary>
        /// The linked list of key-value pair id's.
        /// </summary>
        /// <remarks>Contains triplets: (keyId, valueId, nextId)</remarks>
        private MemoryMappedHugeArrayInt64 _pairs;

        /// <summary>
        /// The index of hashes
        /// </summary>
        /// <remarks>Contains a references to a first pair: (pairId) and the pairs for a linked-list.</remarks>
        private MemoryMappedHugeArrayInt64 _hashes;

        /// <summary>
        /// Holds the size.
        /// </summary>
        private int _size;

        /// <summary>
        /// Holds the next pair id.
        /// </summary>
        private long _nextPairId;

        /// <summary>
        /// Creates a new dictionary.
        /// </summary>
        /// <param name="file">A memory mapped file.</param>
        /// <param name="readKeyFrom"></param>
        /// <param name="writeToKey"></param>
        /// <param name="readValueFrom"></param>
        /// <param name="writeValueTo"></param>
        public MemoryMappedHugeDictionary(MemoryMappedFile file, 
            MemoryMappedFile.ReadFromDelegate<TKey> readKeyFrom, MemoryMappedFile.WriteToDelegate<TKey> writeToKey,
            MemoryMappedFile.ReadFromDelegate<TValue> readValueFrom, MemoryMappedFile.WriteToDelegate<TValue> writeValueTo)
        {
            _file = file;
            _size = 0;
            _nextPairId = 0;

            _hashes = new MemoryMappedHugeArrayInt64(_file, 16);
            for (int idx = 0; idx < _hashes.Length; idx++)
            {
                _hashes[idx] = -1;
            }
            _pairs = new MemoryMappedHugeArrayInt64(_file, 16 * 4);
            _keys = new MemoryMappedIndex<KeyStruct>(_file,
                (stream, position) => 
                {
                    return new KeyStruct() 
                    {
                        Key = readKeyFrom.Invoke(stream, position)
                    };
                },
                (stream, position, structure) => 
                {
                    return writeToKey.Invoke(stream, position, structure.Key);
                });
            _values = new MemoryMappedIndex<ValueStruct>(_file, 
                (stream, position) => 
                {
                    return new ValueStruct() 
                    {
                        Value = readValueFrom.Invoke(stream, position)
                    };
                },
                (stream, position, structure) => 
                {
                    return writeValueTo.Invoke(stream, position, structure.Value);
                });
        }

        /// <summary>
        /// Adds to 
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void Add(long hash, TKey key, TValue value)
        {
            // set next pair id.
            var nextPairId = _nextPairId;
            _nextPairId = _nextPairId + 3;

            // find the last entry.
            var pairId = _hashes[hash];
            if(pairId < 0)
            { // no key yet at this hash.
                _hashes[hash] = nextPairId;
            }
            else
            { // search for the first id with an empty next.
                var lastPairId = pairId;
                while (pairId >= 0)
                {
                    lastPairId = pairId;
                    pairId = _pairs[pairId + 2];
                }

                // set last entry.
                _pairs[lastPairId + 2] = nextPairId;
            }

            // add pair.
            _pairs[nextPairId + 0] = _keys.Add(new KeyStruct() { Key = key });
            _pairs[nextPairId + 1] = _values.Add(new ValueStruct() { Value = value });
            _pairs[nextPairId + 2] = -1; // no next just yet.
            _size++;
            return;
        }

        /// <summary>
        /// Sets to 
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void Set(long hash, TKey key, TValue value)
        {            
            // try an find the key.
            var pairId = _hashes[hash];
            while (pairId >= 0)
            {
                var keyId = _pairs[pairId];
                var currentKey = _keys.Get(keyId).Key;
                if (currentKey.Equals(key))
                { // key was found.
                    _pairs[pairId + 1] = _values.Add(new ValueStruct() { Value = value });
                    return;
                }
                pairId = _pairs[pairId + 2];
            }
            this.Add(hash, key, value);
        }

        /// <summary>
        /// Gets a the given key.
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        /// <param name="keyId"></param>
        /// <param name="valueId">
        /// <param name="pairId"></param></param>
        /// <returns></returns>
        private bool Get(long hash, TKey key, out long keyId, out long valueId, out long pairId)
        {
            // try an find the key.
            pairId = _hashes[hash];
            while (pairId >= 0)
            {
                keyId = _pairs[pairId];
                var currentKey = _keys.Get(keyId).Key;
                if (currentKey.Equals(key))
                { // key was found.
                    valueId = _pairs[pairId + 1];
                    return true;
                }
                pairId = _pairs[pairId + 2];
            }
            keyId = -1;
            valueId = -1;
            pairId = -1;
            return false;
        }

        /// <summary>
        /// Removes the given key.
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="key"></param>
        private bool Remove(long hash, TKey key)
        {
            // find the last entry.
            var pairId = _hashes[hash];
            if(pairId >= 0)
            { // search for the first id with an empty next.
                if(_pairs[pairId + 2] < 0)
                { // no next one in the first entry, remove everything.
                    var potentialKey = _keys.Get(_pairs[pairId + 0]);
                    if (potentialKey.Equals(key))
                    { // key was found.
                        _hashes[hash] = -1;
                        _size--;
                        return true;
                    }
                }
                else
                { // override if key.
                    var lastPairId = pairId;
                    pairId = _pairs[pairId + 2];
                    while(pairId >= 0)
                    {
                        var potentialKey = _keys.Get(_pairs[pairId + 0]);
                        lastPairId = pairId;
                        pairId = _pairs[pairId + 2];
                        if (potentialKey.Equals(key))
                        { // key was found.
                            _pairs[lastPairId + 2] = pairId;
                            _size--;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Calculates hash.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private long Hash(TKey key)
        {
            return (key.GetHashCode() % (_hashes.Length / 2)) + (_hashes.Length / 2);
        }

        private struct ValueStruct
        {
            public TValue Value { get; set; }
        }

        private struct KeyStruct
        {
            public TKey Key { get; set; }
        }


        public void Add(TKey key, TValue value)
        {
            long keyId, valueId, pairId;
            var hash = this.Hash(key);
            if (this.Get(hash, key, out keyId, out valueId, out pairId))
            {
                throw new ArgumentException("Duplicate key");
            }
            this.Add(hash, key, value);
        }

        public bool ContainsKey(TKey key)
        {
            long keyId, valueId, pairId;
            return this.Get(this.Hash(key), key, out keyId, out valueId, out pairId);
        }

        public ICollection<TKey> Keys
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(TKey key)
        {
            return this.Remove(this.Hash(key), key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            long keyId, valueId, pairId;
            var hash = this.Hash(key);
            if (this.Get(hash, key, out keyId, out valueId, out pairId))
            { // key was found.
                value = _values.Get(valueId).Value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        public ICollection<TValue> Values
        {
            get { throw new NotImplementedException(); }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if(!this.TryGetValue(key, out value))
                {
                    throw new KeyNotFoundException();
                }
                return value;
            }
            set
            {
                var hash = this.Hash(key);
                this.Set(hash, key, value);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            for (int idx = 0; idx < _hashes.Length; idx++)
            {
                _hashes[idx] = -1;
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            if(this.TryGetValue(item.Key, out value))
            {
                return value.Equals(item.Value);
            }
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _size; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return this.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disposes of all native resources associated with this index.
        /// </summary>
        public void Dispose()
        {
            _hashes.Dispose();
            _pairs.Dispose();
            _keys.Dispose();
            _values.Dispose();
        }
    }
}