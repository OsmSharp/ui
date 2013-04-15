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

namespace OsmSharp.Tools.Math.VRP.Core.Routes
{
    internal class BetweenEnumerable : IEnumerable<int>
    {
        private int _first;

        private int _last;

        private IRoute _route;

        public BetweenEnumerable(IRoute route, int first, int last)
        {
            _route = route;
            _first = first;
            _last = last;
        }

        private class BetweenEnumerator : IEnumerator<int>
        {
            private bool _first_reached = false;

            private bool _last_reached = false;

            private int _first;

            private int _last;

            private IEnumerator<int> _enumerator;

            public BetweenEnumerator(IEnumerator<int> enumerator, int first, int last)
            {
                _enumerator = enumerator;
                _first = first;
                _last = last;
            }

            public int Current
            {
                get
                {
                    return _enumerator.Current;
                }
            }

            public void Dispose()
            {
                _enumerator.Dispose();
                _enumerator = null;
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                if (_last_reached)
                {
                    return false;
                }
                if (!_first_reached)
                {
                    _enumerator.MoveNext();
                    while (_enumerator.Current != _first)
                    {
                        _enumerator.MoveNext();
                    }
                    _first_reached = true;
                    return _enumerator.Current == _first &&
                        _first != _last;
                }
                else if (!_enumerator.MoveNext())
                {
                    _enumerator.Reset();
                    _enumerator.MoveNext();
                }
                if (_last == _enumerator.Current)
                {
                    _last_reached = true;
                }
                return true;
            }

            public void Reset()
            {
                _enumerator.Reset();
                _last_reached = false;
                _first_reached = false;
            }
        }

        public IEnumerator<int> GetEnumerator()
        {
            return new BetweenEnumerator(_route.GetEnumerator(), _first, _last);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
