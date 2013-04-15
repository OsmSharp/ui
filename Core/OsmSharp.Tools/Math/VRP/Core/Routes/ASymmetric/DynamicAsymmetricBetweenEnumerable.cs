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
    /// A dynamic asymmetric between enumerable.
    /// </summary>
    public class DynamicAsymmetricBetweenEnumerable : IEnumerable<int>
    {
        private int _first;

        private int _last;

        private int _first_route;

        private int[] _next_array;

        /// <summary>
        /// Creates a new between enumerable.
        /// </summary>
        /// <param name="next_array"></param>
        /// <param name="first"></param>
        /// <param name="last"></param>
        /// <param name="first_route"></param>
        public DynamicAsymmetricBetweenEnumerable(int[] next_array, int first, int last, int first_route)
        {
            _next_array = next_array;
            _first = first;
            _last = last;

            _first_route = first_route;
        }

        private class BetweenEnumerator : IEnumerator<int>
        {
            private int _current = -1;
            
            private int _first;

            private int _last;

            private int _first_route;

            private int[] _next_array;

            public BetweenEnumerator(int[] next_array, int first, int last, int first_route)
            {
                _next_array = next_array;
                _first = first;
                _last = last;

                _first_route = first_route;
            }

            public int Current
            {
                get
                {
                    return _current;
                }
            }

            public void Dispose()
            {

            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                if (_current == _last)
                {
                    return false;
                }
                if (_current == -1)
                {
                    _current = _first;
                    return true;
                }
                _current = _next_array[_current];
                if (_current == -1)
                {
                    _current = _first_route;
                }
                return true;
            }

            public void Reset()
            {
                _current = -1;
            }
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<int> GetEnumerator()
        {
            return new BetweenEnumerator(_next_array, _first, _last, _first_route);
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
