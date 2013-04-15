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

namespace OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric
{
    /// <summary>
    /// Asymmetric cycles.
    /// </summary>
    public class AsymmetricCycles
    {
        private int[] _next_array;

        private Dictionary<int, int> _cycles;

        /// <summary>
        /// Creates a new collection of asymmetric cylces.
        /// </summary>
        /// <param name="length"></param>
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

        /// <summary>
        /// Returns the nextarray.
        /// </summary>
        public int[] NextArray
        {
            get
            {
                return _next_array;
            }
        }

        /// <summary>
        /// Returns the next customer.
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public int this[int from]
        {
            get
            {
                return _next_array[from];
            }
        }

        /// <summary>
        /// Adds a new edge.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public void AddEdge(int from, int to)
        {
            _cycles = null;
            // set the next to.
            _next_array[from] = to;
        }

        /// <summary>
        /// Returns cycles.
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
            bool[] done = new bool[_next_array.Length];
            //HashSet<int> to_do = new HashSet<int>(_next_array);
            for(int idx = 0; idx < _next_array.Length; idx++)
            {
                if (!done[idx])
                {
                    this.CheckForCycle(done, idx);
                }
            }
        }

        private void CheckForCycle(bool[] done, int customer)
        {
            int start = customer;
            int count = 1;
            while (_next_array[customer] >= 0)
            {
                done[customer] = true;

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

        /// <summary>
        /// Clones this asymmetric cycles.
        /// </summary>
        /// <returns></returns>
        public AsymmetricCycles Clone()
        {

            return new AsymmetricCycles(_next_array.Clone() as int[], null);
        }

        /// <summary>
        /// Returns the length.
        /// </summary>
        public int Length
        {
            get
            {
                return _next_array.Length;
            }
        }
    }
}
