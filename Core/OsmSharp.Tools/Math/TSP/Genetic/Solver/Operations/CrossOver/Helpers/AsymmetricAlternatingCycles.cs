// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
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
using OsmSharp.Tools.Collections;

namespace OsmSharp.Tools.Math.TSP.Genetic.Solver.Operations.CrossOver.Helpers
{
    /// <summary>
    /// A collection of asymmetric alternating cycles.
    /// </summary>
    public class AsymmetricAlternatingCycles
    {
        private KeyValuePair<int, int>[] _next_array;

        private Dictionary<int, int> _cycles;

        /// <summary>
        /// Create a new collection of asymmetric alternating cycles.
        /// </summary>
        /// <param name="length"></param>
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

        /// <summary>
        /// Adds an edge.
        /// </summary>
        /// <param name="from_a"></param>
        /// <param name="to"></param>
        /// <param name="from_b"></param>
        public void AddEdge(int from_a, int to, int from_b)
        {
            _cycles = null;
            _next_array[from_a] = new KeyValuePair<int, int>(to, from_b);
        }

        /// <summary>
        /// Returns the cycles.
        /// </summary>
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
