using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Osm.Routing.CH.PreProcessing
{
    public class CHPriorityQueue : IEnumerable<long>
    {
        private Dictionary<long, double> _weights;

        private SortedList<double, List<long>> _sorted_weights;

        public CHPriorityQueue()
        {
            _weights = new Dictionary<long, double>();
            _sorted_weights = new SortedList<double, List<long>>();
        }

        public int Count
        {
            get
            {
                return _weights.Count;
            }
        }

        public bool Contains(long id)
        {
            return _weights.ContainsKey(id);
        }

        public void Enqueue(long id, double weight)
        {
            List<long> queue;
            double old_weight;
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
                queue = new List<long>();
                _sorted_weights.Add(weight, queue);
            }
            queue.Add(id);
            _weights.Add(id, weight);
        }

        public long Pop()
        {
            double weight = _sorted_weights.Keys[0];
            List<long> first_set = _sorted_weights[weight];
            long vertex_id = first_set[0];

            // remove the vertex.
            first_set.Remove(vertex_id);
            if (first_set.Count == 0)
            {
                _sorted_weights.Remove(weight);
            }
            _weights.Remove(vertex_id);

            return vertex_id;
        }

        public void Remove(long vertex_id)
        {
            // remove the vertex.
            double weight = _weights[vertex_id];
            List<long> first_set = _sorted_weights[weight];
            first_set.Remove(vertex_id);
            if (first_set.Count == 0)
            {
                _sorted_weights.Remove(weight);
            }
            _weights.Remove(vertex_id);

        }

        public long Peek()
        {
            double weight = _sorted_weights.Keys[0];
            List<long> first_set = _sorted_weights[weight];
            long vertex_id = first_set[0];

            return vertex_id;
        }

        public List<long> PeekAll()
        {
            double weight = _sorted_weights.Keys[0];
            return _sorted_weights[weight];
        }

        #region IEnumerator<long> Implementation

        public IEnumerator<long> GetEnumerator()
        {
            return _weights.Keys.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _weights.Keys.GetEnumerator();
        }

        #endregion

        internal double Weight(long current_id)
        {
            double weight;
            _weights.TryGetValue(current_id, out weight);
            return weight;
        }
    }
}
