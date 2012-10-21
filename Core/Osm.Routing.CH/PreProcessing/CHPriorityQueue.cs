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

namespace Osm.Routing.CH.PreProcessing
{
    public class CHPriorityQueue : IEnumerable<uint>
    {
        private Dictionary<uint, float> _weights;

        private SortedList<float, HashSet<uint>> _sorted_weights;

        public CHPriorityQueue()
        {
            _weights = new Dictionary<uint, float>();
            _sorted_weights = new SortedList<float, HashSet<uint>>();
        }

        public int Count
        {
            get
            {
                return _weights.Count;
            }
        }

        public bool Contains(uint id)
        {
            return _weights.ContainsKey(id);
        }

        public void Enqueue(uint id, float weight)
        {
            HashSet<uint> queue;
            float old_weight;
            if (_weights.TryGetValue(id, out old_weight))
            {
                _weights.Remove(id);
                if (old_weight != weight)
                {
                    if (_sorted_weights.TryGetValue(old_weight, out queue))
                    {
                        queue.Remove(id);
                        if (queue.Count == 0)
                        {
                            _sorted_weights.Remove(old_weight);
                        }
                    }
                }
            }
            if (!_sorted_weights.TryGetValue(weight, out queue))
            {
                queue = new HashSet<uint>();
                _sorted_weights.Add(weight, queue);
            }
            queue.Add(id);
            _weights.Add(id, weight);
        }

        public uint Pop()
        {
            float weight = _sorted_weights.Keys[0];
            HashSet<uint> first_set = _sorted_weights[weight];
            uint vertex_id = first_set.First();

            // remove the vertex.
            first_set.Remove(vertex_id);
            if (first_set.Count == 0)
            {
                _sorted_weights.Remove(weight);
            }
            _weights.Remove(vertex_id);

            return vertex_id;
        }

        public void Remove(uint vertex_id)
        {
            // remove the vertex.
            float weight = _weights[vertex_id];
            HashSet<uint> first_set = _sorted_weights[weight];
            first_set.Remove(vertex_id);
            if (first_set.Count == 0)
            {
                _sorted_weights.Remove(weight);
            }
            _weights.Remove(vertex_id);

        }

        public uint Peek()
        {
            float weight = _sorted_weights.Keys[0];
            HashSet<uint> first_set = _sorted_weights[weight];
            uint vertex_id = first_set.First();

            return vertex_id;
        }

        public HashSet<uint> PeekAll()
        {
            float weight = _sorted_weights.Keys[0];
            return _sorted_weights[weight];
        }

        public IEnumerable<float> Weights
        {
            get
            {
                return _sorted_weights.Keys;
            }
        }

        public HashSet<uint> PeekAtWeight(float weight)
        {
            return _sorted_weights[weight];
        }

        public float Weight(uint current_id)
        {
            float weight;
            _weights.TryGetValue(current_id, out weight);
            return weight;
        }

        #region IEnumerator<uint> Implementation

        public IEnumerator<uint> GetEnumerator()
        {
            return _weights.Keys.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _weights.Keys.GetEnumerator();
        }

        #endregion
    }
}
