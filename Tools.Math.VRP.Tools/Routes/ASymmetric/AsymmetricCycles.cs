using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.VRP.Core.Routes.ASymmetric
{
    public class AsymmetricCycles
    {
        private int[] _next_array;

        private Dictionary<int, int> _cycles;

        public AsymmetricCycles(int length)
        {
            _next_array = new int[length];
            _cycles = null;

            for (int idx = 0; idx < length; idx++)
            {
                _next_array[idx] = -1;
            }
        }

        private AsymmetricCycles(int[] next_array,
            Dictionary<int, int> cycles)
        {
            _next_array = next_array;
            _cycles = cycles;
        }

        public int this[int from]
        {
            get
            {
                return _next_array[from];
            }
        }

        public void AddEdge(int from, int to)
        {
            _cycles = null;
            // set the next to.
            _next_array[from] = to;
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
            HashSet<int> to_do = new HashSet<int>(_next_array);
            while (to_do.Count > 0)
            {
                this.CheckForCycle(to_do, to_do.First());
            }
        }

        private void CheckForCycle(HashSet<int> to_do, int customer)
        {
            int start = customer;
            int count = 1;
            while (_next_array[customer] >= 0)
            {
                to_do.Remove(customer);

                if (_next_array[customer] == start)
                {
                    _cycles.Add(start, count);
                    break;
                }

                count++;
                customer = _next_array[customer];

                if (count > _next_array.Length)
                {
                    break;
                }
            }
        }

        public AsymmetricCycles Clone()
        {

            return new AsymmetricCycles(_next_array.Clone() as int[], null);
        }

        public int Length
        {
            get
            {
                return _next_array.Length;
            }
        }
    }
}
