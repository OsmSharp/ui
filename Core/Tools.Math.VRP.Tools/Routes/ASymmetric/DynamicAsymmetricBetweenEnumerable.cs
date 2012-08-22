using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.VRP.Core.Routes.ASymmetric
{
    public class DynamicAsymmetricBetweenEnumerable : IEnumerable<int>
    {
        private int _first;

        private int _last;

        private int[] _next_array;

        public DynamicAsymmetricBetweenEnumerable(int[] next_array, int first, int last)
        {
            _next_array = next_array;
            _first = first;
            _last = last;
        }

        private class BetweenEnumerator : IEnumerator<int>
        {
            private int _current = -1;
            
            private int _first;

            private int _last;

            private int[] _next_array;

            public BetweenEnumerator(int[] next_array, int first, int last)
            {
                _next_array = next_array;
                _first = first;
                _last = last;
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
                return true;
            }

            public void Reset()
            {
                _current = -1;
            }
        }

        public IEnumerator<int> GetEnumerator()
        {
            return new BetweenEnumerator(_next_array, _first, _last);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
