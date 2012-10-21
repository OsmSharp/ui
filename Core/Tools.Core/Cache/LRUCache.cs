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
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Tools.Core.Cache
{
    public class LRUCache<TKey, TValue>
    {
        private ConcurrentDictionary<TKey, CacheEntry<TValue>> _data;

        private long _id;

        private long _last_id;

        public LRUCache(int capacity)
        {
            _id = long.MinValue;
            _last_id = _id;
            _data = new ConcurrentDictionary<TKey, CacheEntry<TValue>>();
            this.Capacity = capacity;
        }

        public int Capacity { get; set; }

        public void Add(TKey key, TValue value)
        {
            _id++;
            CacheEntry<TValue> entry = new CacheEntry<TValue>
            {
                Id = _id,
                Value = value
            };
            _data[key] = entry;

            this.ResizeCache();
        }

        public TValue Get(TKey key)
        {
            CacheEntry<TValue> value;
            _id++;
            if (_data.TryGetValue(key, out value))
            {
                value.Id = _id;
                return value.Value;
            }
            return default(TValue);
        }

        public void Clear()
        {
            _data.Clear();
            _last_id = _id;
            _id = long.MinValue;
        }
        
        public void Remove(TKey id)
        {
            CacheEntry<TValue> entry;
            _data.TryRemove(id, out entry);
        }

        private void ResizeCache()
        {
            if ((_id - _last_id) >= this.Capacity)
            {
                int new_capacity = (this.Capacity * 75) / 100; // keep 75%

                // loop over and remove all with smallest id's.
                long id = _id - new_capacity;
                List<TKey> keys = new List<TKey>(this.Capacity - new_capacity);
                foreach (KeyValuePair<TKey, CacheEntry<TValue>> pair in _data)
                {
                    if (pair.Value.Id < id)
                    {
                        keys.Add(pair.Key);
                    }
                }
                CacheEntry<TValue> entry;
                foreach (TKey key in keys)
                {
                    _data.TryRemove(key, out entry);
                }

                _last_id = id;
            }
        }

        private struct CacheEntry<TValue>
        {
            /// <summary>
            /// The id of the object.
            /// </summary>
            public long Id { get; set; }

            /// <summary>
            /// The object being cached.
            /// </summary>
            public TValue Value { get; set; }
        }
    }
}