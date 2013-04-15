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
using System.Collections.ObjectModel;
using System.Diagnostics;
using OsmSharp.Tools.Collections;

namespace OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric
{
    /// <summary>
    /// An asymetric dynamically sizeable mutliple routes object.
    /// </summary>
    public class DynamicAsymmetricMultiRoute : IMultiRoute, IEquatable<DynamicAsymmetricMultiRoute>
    {
        /// <summary>
        /// The is round flag for every route.
        /// </summary>
        protected bool _is_round;

        /// <summary>
        /// The next-array containing all route information.
        /// </summary>
        protected int[] _next_array;

        /// <summary>
        /// The first customer for every route.
        /// </summary>
        private MultiRoutePart[] _first;

        /// <summary>
        /// Creates a new dynamic route by creating shallow copy of the array(s) given.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="next_array"></param>
        /// <param name="is_round"></param>
        protected DynamicAsymmetricMultiRoute(int[] first, int[] next_array, bool is_round)
        {
            _next_array = next_array.Clone() as int[];
            _is_round = is_round;

            _first = new MultiRoutePart[first.Length];
            for (int idx = 0; idx < first.Length; idx++)
            { // create the multi route parts.
                _first[idx] = new MultiRoutePart(this, first[idx], is_round);
            }
        }

        /// <summary>
        /// Creates a new dynamic route by creating shallow copy of the array(s) given.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="next_array"></param>
        /// <param name="is_round"></param>
        private DynamicAsymmetricMultiRoute(IEnumerable<MultiRoutePart> first, int[] next_array, bool is_round)
        {
            _next_array = next_array.Clone() as int[];
            _is_round = is_round;

            _first = new MultiRoutePart[first.Count<MultiRoutePart>()];
            int idx = 0;
            foreach (MultiRoutePart part in first)
            {
                _first[idx] = new MultiRoutePart(this, part.First, is_round);
                idx++;
            }
        }

        /// <summary>
        /// Creates a new dynamic assymmetric route using an initial size and customer.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="is_round"></param>
        public DynamicAsymmetricMultiRoute(int size, bool is_round)
        {
            _is_round = is_round;

            _next_array = new int[size];
            for (int idx = 0; idx < size; idx++)
            {
                _next_array[idx] = -1;
            }
            _first = new MultiRoutePart[0];
        }

        /// <summary>
        /// Adds a new empty route.
        /// </summary>
        /// <returns></returns>
        public virtual IRoute Add()
        {
            // add one element to the first array.
            int route_idx = _first.Length;
            Array.Resize<MultiRoutePart>(ref _first, _first.Length + 1);

            // create and set an empty route.
            _first[route_idx] = new MultiRoutePart(this, _is_round);

            // return the new route.
            _sizes = null;
            return this.Route(route_idx);
        }

        /// <summary>
        /// Adds a new route intialized with the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public virtual IRoute Add(int customer)
        {
            // add one element to the first array.
            int route_idx = _first.Length;
            Array.Resize<MultiRoutePart>(ref _first, _first.Length + 1);

            // set the initial customer.
            _first[route_idx] = new MultiRoutePart(this, customer, _is_round);

            // resize the array if needed.
            if (_next_array.Length <= customer)
            { // resize the array.
                this.Resize(customer);
            }
            _next_array[customer] = -1;

            // return the new route.
            _sizes = null;
            return this.Route(route_idx);
        }

        /// <summary>
        /// Adds a new route by copying the given one.
        /// </summary>
        /// <param name="route"></param>
        public virtual IRoute Add(IRoute route)
        {
            bool first = true;
            int previous = -1;
            int route_idx = -1;
            foreach (int customer in route)
            {
                if (first)
                {
                    // add one element to the first array.
                    route_idx = _first.Length;
                    Array.Resize<MultiRoutePart>(ref _first, _first.Length + 1);

                    // set the initial customer.
                    _first[route_idx] = new MultiRoutePart(this, customer, _is_round);

                    // resize the array if needed.
                    if (_next_array.Length <= customer)
                    { // resize the array.
                        this.Resize(customer);
                    }
                    _next_array[customer] = -1;
                    first = false;
                }
                else
                {
                    _next_array[previous] = customer;
                }

                // set the previous customer.
                previous = customer;
            }

            // return the new route.
            if (route_idx < 0)
            {
                return null;
            }

            _sizes = null;
            return this.Route(route_idx);
        }

        /// <summary>
        /// Returns the route at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public virtual IRoute Route(int idx)
        {
            return _first[idx];
        }

        /// <summary>
        /// Removes the given route.
        /// </summary>
        /// <param name="route_idx"></param>
        /// <returns></returns>
        public bool Remove(int route_idx)
        {
            int start = _first[route_idx].First;

            List<MultiRoutePart> firsts = new List<MultiRoutePart>(_first);
            firsts.RemoveAt(route_idx);
            _first = firsts.ToArray();

            //if (start >= 0)
            //{
            //    int customer = start;
            //    do
            //    {
            //        int customer_next = _next_array[customer];
            //        _next_array[customer] = -1;

            //        customer = customer_next;
            //    }
            //    while (customer >= 0 && start != customer);
            //}

            //if (!this.IsValid())
            //{
            //    throw new Exception();
            //}
            return true;
        }

        /// <summary>
        /// Resizes the array.
        /// </summary>
        /// <param name="customer"></param>
        private void Resize(int customer)
        { // THIS IS EXPENSIZE! TRY TO ESTIMATE CORRECT SIZE WHEN CREATING ROUTE!
            int old_size = _next_array.Length;
            Array.Resize<int>(ref _next_array, customer + 1);
            for (int new_customer = old_size; new_customer < _next_array.Length; new_customer++)
            { // initialize with -1.
                _next_array[new_customer] = -1;
            }
        }

        /// <summary>
        /// Returns the number of routes.
        /// </summary>
        public int Count
        {
            get
            {
                return _first.Length;
            }
        }

        /// <summary>
        /// Returns the size.
        /// </summary>
        public int Size
        {
            get
            {
                return _next_array.Length;
            }
        }

        #region Sizes

        /// <summary>
        /// Holds the sizes of all routes.
        /// </summary>
        private int[] _sizes;

        /// <summary>
        /// Resets all sizes.
        /// </summary>
        private void ResetSizes()
        {
            _sizes = null;
        }

        /// <summary>
        /// Recalculate all sizes.
        /// </summary>
        private void RecalculateSizes()
        {
            _sizes = new int[_first.Length];
            for (int idx = 0; idx < _first.Length; idx++)
            {
                _sizes[idx] = this.Route(idx).Count;
            }
        }

        /// <summary>
        /// Returns all sizes.
        /// </summary>
        public ReadOnlyCollection<int> Sizes
        {
            get
            {
                if (_sizes == null)
                {
                    this.RecalculateSizes();
                }
                return new ReadOnlyCollection<int>(_sizes);
            }
        }

        /// <summary>
        /// Returns the count of all customers.
        /// </summary>
        public int CountCustomers
        {
            get
            {
                int total = 0;
                foreach (int count in _sizes)
                {
                    total = total + count;
                }
                return total;
            }
        }

        #endregion

        /// <summary>
        /// Returns the customer after the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int Next(int customer)
        {
            int next = _next_array[customer];
            if (next < 0)
            {
                for (int idx = 0; idx < this.Count; idx++)
                {
                    IRoute route = this.Route(idx);
                    if (route.Contains(customer) && 
                        route.GetNeigbours(customer)[0] == route.First)
                    {
                        return route.First;
                    }
                }
            }
            return next;
        }

        /// <summary>
        /// Returns true if this route is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            HashSet<int> unique_customers = new HashSet<int>();
            int count = 0;
            foreach (int customer in _next_array)
            {
                if (customer >= 0)
                {
                    count++;

                    if (unique_customers.Contains(customer))
                    {
                        return false;
                    }
                    unique_customers.Add(customer);
                }
            }
            if (unique_customers.Count != count)
            {
                return false;
            }
            foreach (MultiRoutePart first in _first)
            {
                if (unique_customers.Contains(first.First))
                { // first customer cannot be referenced by another customer!
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if the other multi route is equal in content.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(DynamicAsymmetricMultiRoute other)
        {
            if (((object)this).Equals((object)other))
            {
                return true;
            }
            if (this.Count == other.Count)
            {
                if (this._next_array.Length != other._next_array.Length)
                {
                    return false;
                }

                // compare the initial customers.
                for (int route_idx = 0; route_idx < this.Count; route_idx++)
                {
                    if (this._first[route_idx] != other._first[route_idx])
                    {
                        return false;
                    }
                }

                // compare the next array.
                for (int idx = 0; idx < this._next_array.Length; idx++)
                {
                    if (this._next_array[idx] != other._next_array[idx])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if both multiroutes are equal in content.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool IEquatable<DynamicAsymmetricMultiRoute>.Equals(DynamicAsymmetricMultiRoute other)
        {
            return this.Equals(other);
        }

        /// <summary>
        /// Removes the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool RemoveCustomer(int customer)
        {
            foreach (MultiRoutePart part in _first)
            {
                if (part.Remove(customer))
                {
                    return true;
                }
            }

            if (!this.IsValid())
            {
                throw new Exception();
            }
            return false;
        }

        /// <summary>
        /// Creates a deep copy of this multi route.
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return new DynamicAsymmetricMultiRoute(_first, _next_array, _is_round);
        }

        /// <summary>
        /// Returns true if the from-to is contained in this multi route.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool Contains(int from, int to)
        {
            return _next_array[from] == to;
        }

        /// <summary>
        /// Returns true if the given customer is contained in this multi route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool Contains(int customer)
        {
            if (_next_array[customer] >= 0)
            {
                return true;
            }
            foreach (IRoute route in _first)
            {
                if (route != null &&
                    route.First == customer)
                {
                    return true;
                }
            }
            foreach (int current in _next_array)
            {
                if (customer == current)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// A class exposing only the information about one route.
        /// </summary>
        private class MultiRoutePart : IASymmetricRoute
        {
            /// <summary>
            /// The is round flag.
            /// </summary>
            private bool _is_round;

            ///// <summary>
            ///// The next-array.
            ///// </summary>
            //private int[] _next_array;

            /// <summary>
            /// The first customer.
            /// </summary>
            private int _first;

            /// <summary>
            /// The last customer.
            /// </summary>
            private int _last;

            /// <summary>
            /// Holds the parent route.
            /// </summary>
            private DynamicAsymmetricMultiRoute _parent;
            
            /// <summary>
            /// Creates a new dynamic route.
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="is_round"></param>
            internal MultiRoutePart(DynamicAsymmetricMultiRoute parent, bool is_round)
            {
                _parent = parent;
                _first = -1;
                _last = -1;
                _is_round = is_round;
            }

            /// <summary>
            /// Creates a new dynamic route.
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="first"></param>
            /// <param name="is_round"></param>
            internal MultiRoutePart(DynamicAsymmetricMultiRoute parent, 
                int first, bool is_round)
            {
                _parent = parent;

                _first = first;
                //_next_array = next_array;
                _is_round = is_round;

                if (_is_round)
                {
                    _last = first;
                }
                else
                {
                    _last = -1;
                }
            }

            /// <summary>
            /// Updates and sets the last customer.
            /// </summary>
            private void UpdateLast()
            {
                _last = _first;
                if (!this.IsRound)
                {
                    while (_parent._next_array[_last] >= 0 && _parent._next_array[_last] != _first)
                    {
                        _last = _parent._next_array[_last];
                    }
                }
            }

            /// <summary>
            /// Returns true if the route is empty.
            /// </summary>
            public bool IsEmpty
            {
                get
                {
                    return _first < 0;
                }
            }

            /// <summary>
            /// Returns true if there is a route from the last customer back to the first.
            /// </summary>
            public bool IsRound
            {
                get
                {
                    return _is_round;
                }
            }

            /// <summary>
            /// Returns true if there exists an edge from the given customer to another.
            /// </summary>
            /// <param name="from"></param>
            /// <param name="to"></param>
            /// <returns></returns>
            public bool Contains(int from, int to)
            {
                if (_parent._next_array.Length > from)
                { // customers should exist.
                    if (this.Contains(from))
                    {
                        if (_parent._next_array[from] == to)
                        { // edge found.
                            return true;
                        }
                        else if(this.IsRound && to == _first)
                        {
                            return true;
                        }
                    }
                }
                return false; // array too small.
            }

            /// <summary>
            /// Inserts a customer right after from and before to.
            /// </summary>
            /// <param name="from"></param>
            /// <param name="customer"></param>
            public void ReplaceEdgeFrom(int from, int customer)
            {
                //if (customer < 0)
                //{ // a new customer cannot be negative!
                //    throw new ArgumentOutOfRangeException("Cannot add customers with a negative index!");
                //}
                if (this.IsEmpty)
                { // add the given customer as the first one.
                    _first = customer;
                    if (this.IsRound)
                    { // first is last when round.
                        _last = _first;
                    }

                    // resize the array if needed.
                    if (_parent._next_array.Length <= customer)
                    { // resize the array.
                        this.Resize(customer);
                    }
                }
                else
                { // there are already existing customers.
                    if (from < 0)
                    { // a new customer cannot be negative!
                        throw new ArgumentOutOfRangeException("Cannot add a customer after a customer with a negative index!");
                    }

                    if (this.IsRound && customer == _first)
                    { // the next customer is actually the first customer.
                        // set the next customer of the from customer to -1.
                        customer = -1;
                    }

                    if (_parent._next_array.Length > from)
                    { // customers should exist.
                        // resize the array if needed.
                        if (_parent._next_array.Length <= customer)
                        { // resize the array.
                            this.Resize(customer);
                        }

                        // insert customer.
                        _parent._next_array[from] = customer;
                        return;
                    }
                    throw new ArgumentOutOfRangeException("Customer(s) do not exist in this route!");
                }
            }

            /// <summary>
            /// Inserts a customer right after from and before to.
            /// </summary>
            /// <param name="from"></param>
            /// <param name="customer"></param>
            public void InsertAfter(int from, int customer)
            {
                if (customer < 0)
                { // a new customer cannot be negative!
                    throw new ArgumentOutOfRangeException("Cannot add customers with a negative index!");
                }
                if (this.IsEmpty)
                { // add the given customer as the first one.
                    _first = customer;
                    if (this.IsRound)
                    { // first is last when round.
                        _last = _first;
                    }

                    // resize the array if needed.
                    if (_parent._next_array.Length <= customer)
                    { // resize the array.
                        this.Resize(customer);
                    }
                }
                else
                { // there are already existing customers.
                    if (from < 0)
                    { // a new customer cannot be negative!
                        throw new ArgumentOutOfRangeException("Cannot add a customer after a customer with a negative index!");
                    }

                    if (_parent._next_array.Length > from)
                    { // customers should exist.
                        // resize the array if needed.
                        if (_parent._next_array.Length <= customer)
                        { // resize the array.
                            this.Resize(customer);
                        }

                        // get the to customer if needed.
                        int to = _parent._next_array[from];

                        // insert customer.
                        _parent._next_array[from] = customer;
                        if (to < 0) { }
                        else
                        {
                            if (to != _first)
                            {
                                _parent._next_array[customer] = to;
                            }
                        }

                        // update last.
                        if (_first == to && !this.IsRound)
                        {
                            _last = customer;
                        }
                        return;
                    }
                    throw new ArgumentOutOfRangeException("Customer(s) do not exist in this route!");
                }


                if (!this.IsValid())
                {
                    throw new Exception();
                }
            }

            /// <summary>
            /// Resizes the array.
            /// </summary>
            /// <param name="customer"></param>
            private void Resize(int customer)
            { // THIS EXPENSIZE! TRY TO ESTIMATE CORRECT SIZE WHEN CREATING ROUTE!
                int old_size = _parent._next_array.Length;
                Array.Resize<int>(ref _parent._next_array, customer + 1);
                for (int new_customer = old_size; new_customer < _parent._next_array.Length; new_customer++)
                { // initialize with -1.
                    _parent._next_array[new_customer] = -1;
                }
            }

            /// <summary>
            /// Returns the neigbour(s) of the given customer.
            /// </summary>
            /// <param name="customer"></param>
            /// <returns></returns>
            public int[] GetNeigbours(int customer)
            {
                int[] neighbour = new int[1];
                neighbour[0] = _parent._next_array[customer];
                if (neighbour[0] < 0 && this.IsRound)
                {
                    neighbour[0] = this.First;
                }
                return neighbour;
            }

            /// <summary>
            /// Returns true if this route is valid.
            /// </summary>
            /// <returns></returns>
            public bool IsValid()
            {
                if (_first < 0)
                { // empty route is always valid.
                    return true;
                }
                HashSet<int> unique_customers = new HashSet<int>(_parent._next_array);
                int count = 0;
                foreach (int customer in _parent._next_array)
                {
                    if (customer >= 0)
                    {
                        count++;
                    }
                }
                unique_customers.Remove(-1);
                if (unique_customers.Count != count)
                {
                    throw new Exception("Unique customer count not correct!");
                }

                return _parent.IsValid();
                //return true;
            }

            #region Enumerators

            public IEnumerator<int> GetEnumerator()
            {
                return new Enumerator(_first, _parent._next_array);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new Enumerator(_first, _parent._next_array);
            }

            private class Enumerator : IEnumerator<int>
            {
                private int _first;

                private int[] _next_array;

                private int _current = -1;

                public Enumerator(int first, int[] next_array)
                {
                    _first = first;
                    _next_array = next_array;
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
                    _next_array = null;
                }

                object System.Collections.IEnumerator.Current
                {
                    get
                    {
                        return this.Current;
                    }
                }

                public bool MoveNext()
                {
                    if (_current == -1)
                    {
                        _current = _first;
                    }
                    else
                    {
                        _current = _next_array[_current];
                        if (_current == _first)
                        {
                            return false;
                        }
                    }
                    return _current >= 0;
                }

                public void Reset()
                {
                    _current = -1;
                }
            }

            public IEnumerable<int> Between(int from, int to)
            {
                return new DynamicAsymmetricBetweenEnumerable(_parent._next_array, from, to, _first);
            }

            public IEnumerable<Edge> Edges()
            {
                return new EdgeEnumerable(this);
            }

            #endregion

            /// <summary>
            /// Returns the number of customers in this route.
            /// </summary>
            public int Count
            {
                get
                {
                    return this.Count<int>();
                }
            }

            /// <summary>
            /// Removes a customer from this route.
            /// </summary>
            /// <param name="customer"></param>
            /// <returns></returns>
            public bool Remove(int customer)
            {
                // special handling of the first customer.
                if (this.First == customer)
                {
                    _first = _parent._next_array[customer];
                    _parent._next_array[customer] = -1;

                    if (this.IsRound)
                    { // if this route is a round; changing the first changes the last.
                        _last = _first;
                    }
                }
                
                // customer is not the first one (anymore)
                for (int idx = 0; idx < _parent._next_array.Length; idx++)
                {
                    if (_parent._next_array[idx] == customer)
                    {
                        _parent._next_array[idx] = _parent._next_array[customer]; // bypass the existing customer.
                        _parent._next_array[customer] = -1; // there is no next customer anymore.

                        if (customer == this.Last)
                        {
                            this.UpdateLast();
                        }
                        return true;
                    }
                }
                return false;
            }


            public int First
            {
                get
                {
                    return _first;
                }
            }

            public int Last
            {
                get
                {
                    if (_last < 0)
                    {
                        // calculate the last customer.
                        this.UpdateLast();
                    }
                    return _last;
                }
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

            public bool Contains(int customer)
            {
                foreach (int contained in this)
                {
                    if (contained == customer)
                    {
                        return true;
                    }
                }
                return false;
            }

            public override string ToString()
            {
                int previous = -1;
                StringBuilder result = new StringBuilder();
                foreach (int customer in this)
                {
                    if (previous < 0)
                    {
                        result.Append(customer);
                    }
                    else
                    {
                        result.Append("->");
                        result.Append(customer);
                    }
                    previous = customer;
                }
                return result.ToString();
            }

            public object Clone()
            {
                throw new NotSupportedException("Cannot clone a route that's part of a multi-route.");
            }

            public int Next(int customer)
            {
                throw new NotImplementedException();
            }

            public void InsertFirst(int first)
            {
                if (!this.IsEmpty)
                { // only replace the next customer of the current first customer if 
                    // there already is one.
                    _parent._next_array[first] = _first;
                }
                _first = first;

                // resize the array if needed.
                if (_parent._next_array.Length <= first)
                { // resize the array.
                    this.Resize(first);
                }
            }

            public void ReplaceFirst(int first)
            {
                //int next_first = _parent._next_array[_first];
                ////_parent._next_array[_first] = -1;
                //_parent._next_array[first] = next_first;
                _first = first;

                // resize the array if needed.
                if (_parent._next_array.Length <= first)
                { // resize the array.
                    this.Resize(first);
                }
            }

            public void Clear()
            {
                _first = -1;
            }
        }
    }
}
