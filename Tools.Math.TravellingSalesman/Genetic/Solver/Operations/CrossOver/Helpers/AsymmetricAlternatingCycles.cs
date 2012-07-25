using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.TSP.Genetic.Solver.Operations.CrossOver.Helpers
{
    public class AsymmetricAlternatingCycles
    {
        private KeyValuePair<int, int>[] _next_array;

        private Dictionary<int, int> _cycles;

        public AsymmetricAlternatingCycles(int length)
        {
            _next_array = new KeyValuePair<int, int>[length];
            //_cycles = new Dictionary<int,int>();
            _cycles = null;

            for (int idx = 0; idx < length; idx++)
            {
                _next_array[idx] = new KeyValuePair<int,int>(-1, -1);
            }
        }

        public void AddEdge(int from_a, int to, int from_b)
        {
            _cycles = null;
            _next_array[from_a] = new KeyValuePair<int, int>(to, from_b);
        }

        public Dictionary<int, int> Cycles
        {
            get
            {
                if (_cycles == null)
                {
                    this.CalculateCycles();
                }
                return _cycles;
            }
        }

        private void CalculateCycles()
        {
            _cycles = new Dictionary<int, int>();
            HashSet<int> to_do = new HashSet<int>();
            foreach (KeyValuePair<int, int> pair in _next_array)
            {
                if (pair.Key >= 0)
                {
                    to_do.Add(pair.Key);
                }
            }
            while (to_do.Count > 0)
            {
                this.CheckForCycle(to_do, to_do.First());
            }
        }

        private void CheckForCycle(HashSet<int> to_do, int customer)
        {
            int start = customer;
            int count = 1;
            to_do.Remove(customer);
            while (_next_array[customer].Value >= 0)
            {

                if (_next_array[customer].Value == start)
                {
                    _cycles.Add(start, count);
                    break;
                }

                count++;
                customer = _next_array[customer].Value;
                to_do.Remove(customer);
            }
        }

        internal KeyValuePair<int, int> Next(int start)
        {
            return _next_array[start];
        }
    }
}
