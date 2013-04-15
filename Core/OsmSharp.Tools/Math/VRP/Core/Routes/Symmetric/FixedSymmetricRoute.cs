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

namespace OsmSharp.Tools.Math.VRP.Core.Routes.Symmetric
{
    /// <summary>
    /// A fast implementation of a route with a steady customer count.
    /// </summary>
    public class FixedSymmetricRoute : ISymmetricRoute, IEnumerable<int>
    {
        /// <summary>
        /// Contains the list of customers in this symmetric route.
        /// </summary>
        private int[][] _customers;

        /// <summary>
        /// Creates a new fixed symmetric route.
        /// </summary>
        /// <param name="enumerable"></param>
        public FixedSymmetricRoute(IEnumerable<int> enumerable)
        {
            List<int> customers = new List<int>(enumerable);

            _customers = new int[customers.Count][];
            _customers[customers[0]] = new int[2];
            _customers[customers[0]][0] = customers[customers.Count - 1];
            _customers[customers[0]][1] = customers[1];

            for (int idx = 1; idx < customers.Count - 1; idx++)
            {
                _customers[customers[idx]] = new int[2];
                _customers[customers[idx]][0] = customers[idx - 1];
                _customers[customers[idx]][1] = customers[idx + 1];
            }

            _customers[customers[customers.Count - 1]] = new int[2];
            _customers[customers[customers.Count - 1]][0] = customers[customers.Count - 2];
            _customers[customers[customers.Count - 1]][1] = customers[0];
        }

        private FixedSymmetricRoute(int[][] customers)
        {
            _customers = new int[customers.Length][];
            for (int idx = 0; idx < customers.Length; idx++)
            {
                _customers[idx] = customers[idx].Clone() as int[];
            }
        }

        /// <summary>
        /// Returns true if the route is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return _customers.Length == 0;
            }
        }

        /// <summary>
        /// Returns true if the route contains the two customers next to eachother.
        /// </summary>
        /// <param name="customer1"></param>
        /// <param name="customer2"></param>
        /// <returns></returns>
        public bool Contains(int customer1, int customer2)
        {
            return _customers[customer1][0] == customer2
                || _customers[customer1][1] == customer1;
        }

        /// <summary>
        /// Returns the neigbours of the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int[] GetNeigbours(int customer)
        {
            return _customers[customer];
        }

        /// <summary>
        /// Removes two customers.
        /// </summary>
        /// <param name="customer1"></param>
        /// <param name="customer2"></param>
        public void Remove(int customer1, int customer2)
        {
            if (_customers[customer1][0] == customer2)
            {
                _customers[customer1][0] = -1;
            }
            else if (_customers[customer1][1] == customer2)
            {
                _customers[customer1][1] = -1;
            }
            if (_customers[customer2][0] == customer1)
            {
                _customers[customer2][0] = -1;
            }
            else if (_customers[customer2][1] == customer1)
            {
                _customers[customer2][1] = -1;
            }
        }

        /// <summary>
        /// Adds a new edge.
        /// </summary>
        /// <param name="customer1"></param>
        /// <param name="customer2"></param>
        public void Add(int customer1, int customer2)
        {
            if (_customers[customer1][0] == -1)
            {
                _customers[customer1][0] = customer2;
            }
            else if (_customers[customer1][1] == -1)
            {
                _customers[customer1][1] = customer2;
            }
            if (_customers[customer2][0] == -1)
            {
                _customers[customer2][0] = customer1;
            }
            else if (_customers[customer2][1] == -1)
            {
                _customers[customer2][1] = customer1;
            }
        }
        
        /// <summary>
        /// Clones this route.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new FixedSymmetricRoute(_customers);
        }

        /// <summary>
        /// Returns true if valid.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            // always begin at zero.
            int current = 0;        
 
            // get one of the neigbours of current.
            int next = _customers[current][0];
            int cnt = 1;
            while (next > 0)
            {
                // get the neigbour that is not the current one.
                int neighbour = _customers[next][0];
                if (neighbour == current)
                {
                    neighbour = _customers[next][1]; 
                    if (neighbour == current)
                    {
                        return false;
                    }
                }
                current = next;
                next = neighbour;
                if (cnt > _customers.Length)
                {
                    return false;
                }
                cnt++;
            }
            return _customers.Length == cnt;
        }
        
        /// <summary>
        /// Returns true if is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsValidNew()
        {
            // always begin at zero.
            int current = 0;

            // get one of the neigbours of current.
            int next = _customers[current][0];
            int cnt = 1;
            HashSet<int> unique = new HashSet<int>();
            unique.Add(next);
            if (next < 0)
            {
                return false;
            }
            while (next != 0)
            {
                // get the neigbour that is not the current one.
                int neighbour = _customers[next][0];
                if (neighbour == current)
                {
                    neighbour = _customers[next][1];
                    if (neighbour == current)
                    {
                        return false;
                    }
                }
                current = next;
                next = neighbour;
                if (cnt > _customers.Length)
                {
                    return false;
                }
                if (next < 0)
                {
                    return false;
                }
                unique.Add(next);
                cnt++;
            }
            return _customers.Length == cnt &&
                unique.Count == cnt;
        }

        /// <summary>
        /// Returns true if the route is a round.
        /// </summary>
        public bool IsRound
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns a description of the route.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            // always begin at zero.
            int current = 0;
            builder.Append(current);

            // get one of the neigbours of current.
            int next = _customers[current][0];
            builder.Append("->");
            builder.Append(next);
            while (next != 0)
            {
                // get the neigbour that is not the current one.
                int neighbour = _customers[next][0];
                if (neighbour == current)
                {
                    neighbour = _customers[next][1];
                    if (neighbour == current)
                    {
                        return "Invalid Route!";
                    }
                }
                current = next;
                next = neighbour;
                builder.Append("->");
                builder.Append(next);
            }
            return builder.ToString();
        }

        #region IEnumerable<int>

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<int> GetEnumerator()
        {
            return new SymmetricRouteEnumerator(_customers);
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        private class SymmetricRouteEnumerator : IEnumerator<int>
        {
            private int[][] _customers;

            /// <summary>
            /// Creates a new enumerator.
            /// </summary>
            /// <param name="customers"></param>
            public SymmetricRouteEnumerator(int[][] customers)
            {
                _customers = customers;
            }

            int _next = -1;
            int _current = -1;
            
            /// <summary>
            /// Returns the current.
            /// </summary>
            public int Current
            {
                get
                {
                    return _current;
                }
            }

            /// <summary>
            /// Disposes this object.
            /// </summary>
            public void Dispose()
            {

            }

            /// <summary>
            /// Returns the current.
            /// </summary>
            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return _current;
                }
            }

            /// <summary>
            /// Move to the next customer.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if (_next < 0)
                {
                    // always begin at zero.
                    _current = 0;

                    // get one of the neigbours of current.
                    _next = _customers[_current][0];
                    return true;
                }
                else
                {
                    // get the neigbour that is not the current one.
                    int neighbour = _customers[_next][0];
                    if (neighbour == _current)
                    {
                        neighbour = _customers[_next][1];
                        if (neighbour == _current)
                        {
                            return false;
                        }
                    }
                    _current = _next;
                    _next = neighbour;

                    return _current != 0;
                }
            }

            /// <summary>
            /// Resets this route.
            /// </summary>
            public void Reset()
            {
                _current = -1;
                _next = -1;
            }
        }

        /// <summary>
        /// Inserts a new customer.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="customer"></param>
        /// <param name="to"></param>
        public void Insert(int from, int customer, int to)
        {
            this.Add(from, customer);
            this.Add(customer, to);
        }

        /// <summary>
        /// Returns the customer count.
        /// </summary>
        public int Count
        {
            get 
            { 
                return _customers.Length; 
            }
        }

        /// <summary>
        /// Removes a customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool Remove(int customer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the first customer.
        /// </summary>
        public int First
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns the last customer.
        /// </summary>
        public int Last
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns the index of the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int GetIndexOf(int customer)
        {
            int idx = 0;
            foreach (int possible_customer in this)
            {
                if (possible_customer == customer)
                {
                    return idx;
                }
                idx++;
            }
            return -1; // customer not found!
        }

        /// <summary>
        /// Enumerates between.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public IEnumerable<int> Between(int from, int to)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if this customer is contained in the given route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool Contains(int customer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Replaces an edge.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="customer"></param>
        public void ReplaceEdgeFrom(int from, int customer)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Inserts a new edge.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="customer"></param>
        public void InsertAfter(int from, int customer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Enumerates all edges.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Edge> Edges()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserts a new first customers.
        /// </summary>
        /// <param name="first"></param>
        public void InsertFirst(int first)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Replaces the first customer.
        /// </summary>
        /// <param name="first"></param>
        public void ReplaceFirst(int first)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Clears all customers in this route.
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
