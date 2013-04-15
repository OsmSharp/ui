using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Tools.Math.VRP.Core.Routes
{
    /// <summary>
    /// Enumerates all edges in an IRoute.
    /// </summary>
    internal class EdgeEnumerable : IEnumerable<Edge>
    {
        private IRoute _route;

        /// <summary>
        /// Creates a new edge enumerable.
        /// </summary>
        /// <param name="route"></param>
        public EdgeEnumerable(IRoute route)
        {
            _route = route;
        }

        private class EdgeEnumerator : IEnumerator<Edge>
        {
            private Edge _current;

            private int _first;

            private IEnumerator<int> _enumerator;

            public EdgeEnumerator(IEnumerator<int> enumerator, int first)
            {
                _current = new Edge(-1, -1);
                _enumerator = enumerator;
                _first = first;
            }

            public Edge Current
            {
                get
                {
                    return _current;
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
                if (_current.From == -1 && _current.To == -1)
                {
                    if (_enumerator.MoveNext())
                    {
                        _current.From = _enumerator.Current;
                    }
                    else
                    {
                        return false;
                    }

                    if (_enumerator.MoveNext())
                    {
                        _current.To = _enumerator.Current;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (_current.To != _first && _current.From >= 0 && _current.To >= 0)
                {
                    if (_enumerator.MoveNext())
                    {
                        _current.From = _current.To;
                        _current.To = _enumerator.Current;
                    }
                    else if (_first >= 0 && _current.To != _first)
                    {
                        _current.From = _current.To;
                        _current.To = _first;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }

            public void Reset()
            {
                _enumerator.Reset();
                _current = new Edge(-1, -1);
            }
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Edge> GetEnumerator()
        {
            if (_route.IsRound)
            {
                return new EdgeEnumerator(_route.GetEnumerator(), _route.First);
            }
            return new EdgeEnumerator(_route.GetEnumerator(), -1);
        }

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    /// <summary>
    /// Represents an edge.
    /// </summary>
    public struct Edge
    {
        /// <summary>
        /// Creates a new edge.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public Edge(int from, int to)
            :this()
        {
            this.From = from;
            this.To = to;
        }

        /// <summary>
        /// Returns the from customer.
        /// </summary>
        public int From { get; set; }

        /// <summary>
        /// Returns the to customer.
        /// </summary>
        public int To { get; set; }

        /// <summary>
        /// Returns a description of this edge.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} -> {1}", this.From, this.To);
        }

        /// <summary>
        /// Returns a hashcode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.From.GetHashCode() ^
                this.To.GetHashCode();
        }

        /// <summary>
        /// Returns true if the other object is equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Edge)
            {
                return ((Edge)obj).From == this.From &&
                    ((Edge)obj).To == this.To;
            }
            return false;
        }
    }
}
