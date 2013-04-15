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

namespace OsmSharp.Tools.Math.VRP.Core.Routes.Symmetric
{
    /// <summary>
    /// An implementation for a symmetric route with varying customer counts.
    /// </summary>
    public class DynamicSymmetricRoute : ISymmetricRoute
    {
        IList<int> _customers = null;

        /// <summary>
        /// Creates a new dynamic symmetric route.
        /// </summary>
        /// <param name="customer"></param>
        public DynamicSymmetricRoute(int customer)
        {
            _customers = new List<int>();
            _customers.Add(customer);
        }

        /// <summary>
        /// Creates a new dynamic symmetric route.
        /// </summary>
        /// <param name="customers"></param>
        public DynamicSymmetricRoute(IList<int> customers)
        {
            _customers = new List<int>(customers);
        }

        /// <summary>
        /// Returns true as the route is empty.
        /// </summary>
        public bool IsEmpty
        {
            get 
            { 
                return _customers.Count == 0; 
            }
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
        /// Returns true if the customers are contained in the given order.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool Contains(int from, int to)
        {
            for (int idx = 0; idx < _customers.Count - 1; idx++)
            {
                if (_customers[idx] == from && _customers[idx + 1] == to)
                {
                    return true;
                }
                else if (_customers[idx] == to && _customers[idx + 1] == from)
                {
                    return true;
                }
            }
            if (this.IsRound)
            {
                if (_customers[0] == from && _customers[_customers.Count - 1] == to)
                {
                    return true;
                }
                else if (_customers[_customers.Count - 1] == to && _customers[0] == from)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Inserts a new edge.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="customer"></param>
        /// <param name="to"></param>
        public void Insert(int from, int customer, int to)
        {
            for (int idx = 0; idx < _customers.Count - 1; idx++)
            {
                if (_customers[idx] == from && _customers[idx + 1] == to)
                {
                    _customers.Insert(idx + 1, customer);
                    return;
                }
                else if (_customers[idx] == to && _customers[idx + 1] == from)
                {
                    _customers.Insert(idx + 1, customer);
                    return;
                }
            }
            if (this.IsRound)
            {
                if (_customers[0] == from && _customers[_customers.Count - 1] == to)
                {
                    _customers.Add(customer);
                    return;
                }
                else if (_customers[_customers.Count - 1] == to && _customers[0] == from)
                {
                    _customers.Add(customer);
                    return;
                }
            }
            throw new ArgumentOutOfRangeException(string.Format("No edge found from {0} to {1} to insert customer {2}",
                from, to, customer));
        }

        /// <summary>
        /// Returns the neighbours.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int[] GetNeigbours(int customer)
        {
            int before = -1;
            int after = -1;
            int idx = _customers.IndexOf(customer);

            if (idx > 0)
            {
                before = _customers[idx - 1];
            }
            else if(this.IsRound)
            {
                before = _customers[_customers.Count - 1];
            }
            if (idx < _customers.Count - 1)
            {
                after = _customers[idx + 1];
            }
            else if (this.IsRound)
            {
                after = _customers[0];
            }
            int[] neighbours = null;
            if (after >= 0 && before >= 0)
            {
                neighbours = new int[2];
                neighbours[0] = before;
                neighbours[1] = after;
            }
            else if (after >= 0)
            {
                neighbours = new int[1];
                neighbours[0] = after;
            }
            else if (before >= 0)
            {
                neighbours = new int[1];
                neighbours[0] = before;
            }
            return neighbours;
        }

        /// <summary>
        /// Returns true if this route is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Clones this route.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new DynamicSymmetricRoute(_customers);
        }
        
        /// <summary>
        /// Returns the count of the customers.
        /// </summary>
        public int Count
        {
            get
            {
                return this.Count<int>();
            }
        }

        /// <summary>
        /// Removes the customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool Remove(int customer)
        {
            for (int idx = 0; idx < _customers.Count - 1; idx++)
            {
                if (_customers[idx] == customer)
                {
                    _customers[idx] = _customers[customer];
                    _customers[customer] = -1;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns an enumerator for the route.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<int> GetEnumerator()
        {
            return _customers.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator for the route.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Returns the first symmetric route.
        /// </summary>
        public int First
        {
            get { throw new NotImplementedException(); }
        }
        
        /// <summary>
        /// Returns the last symmetric route.
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
        /// Returns a enumerator between two customers.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public IEnumerable<int> Between(int from, int to)
        {
            return new BetweenEnumerable(this, from, to);
        }

        /// <summary>
        /// Returns true if the customer is in this route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool Contains(int customer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the route is empty.
        /// </summary>
        bool IRoute.IsEmpty
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns true if the route is round.
        /// </summary>
        bool IRoute.IsRound
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns the count of this route.
        /// </summary>
        int IRoute.Count
        {
            get { throw new NotImplementedException(); }
        }
        
        /// <summary>
        /// Returns the first customer.
        /// </summary>
        int IRoute.First
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns the last customer.
        /// </summary>
        int IRoute.Last
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns true if the two customers are in this route.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        bool IRoute.Contains(int from, int to)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the customer is in this route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        bool IRoute.Contains(int customer)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Removes the customers.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        bool IRoute.Remove(int customer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Replaces an edge.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="customer"></param>
        void IRoute.ReplaceEdgeFrom(int from, int customer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Insert a customer after.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="customer"></param>
        void IRoute.InsertAfter(int from, int customer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the neigbours.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        int[] IRoute.GetNeigbours(int customer)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Returns the index of a given customers.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        int IRoute.GetIndexOf(int customer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true if the route is valid.
        /// </summary>
        /// <returns></returns>
        bool IRoute.IsValid()
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Returns an enumeration between two customers.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        IEnumerable<int> IRoute.Between(int from, int to)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Returns an enumeration.
        /// </summary>
        /// <returns></returns>
        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an enumerator of all edges.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Edge> Edges()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserts a new first customer.
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
        /// Clears all customers.
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
