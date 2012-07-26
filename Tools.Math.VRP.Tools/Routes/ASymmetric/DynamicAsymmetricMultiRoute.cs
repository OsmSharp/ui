using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools.Math.VRP.Core.Routes.ASymmetric
{
    /// <summary>
    /// An asymetric dynamically sizeable mutliple routes object.
    /// </summary>
    public class DynamicAsymmetricMultiRoute : IMultiRoute, IEquatable<DynamicAsymmetricMultiRoute>
    {
        /// <summary>
        /// The is round flag for every route.
        /// </summary>
        private bool _is_round;

        /// <summary>
        /// The next-array containing all route information.
        /// </summary>
        private int[] _next_array;

        /// <summary>
        /// The first customer for every route.
        /// </summary>
        private int[] _first;

        /// <summary>
        /// Creates a new dynamic route by creating shallow copy of the array(s) given.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="_next_array"></param>
        /// <param name="is_round"></param>
        private DynamicAsymmetricMultiRoute(int[] first, int[] next_array, bool is_round)
        {
            _first = first.Clone() as int[];
            _next_array = next_array.Clone() as int[];
            _is_round = is_round;
        }

        /// <summary>
        /// Creates a new dynamic assymmetric route using an initial size and customer.
        /// </summary>
        /// <param name="size"></param>
        public DynamicAsymmetricMultiRoute(int size, bool is_round)
        {
            _is_round = is_round;

            _next_array = new int[size];
            for (int idx = 0; idx < size; idx++)
            {
                _next_array[idx] = -1;
            }
            _first = new int[0];
        }

        /// <summary>
        /// Adds a new route intialized with the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public IRoute Add(int customer)
        {
            // add one element to the first array.
            int route_idx = _first.Length;
            Array.Resize<int>(ref _first, _first.Length + 1);

            // set the initial customer.
            _first[route_idx] = customer;

            // resize the array if needed.
            if (_next_array.Length <= customer)
            { // resize the array.
                this.Resize(customer);
            }
            _next_array[customer] = customer;

            // return the new route.
            return this.Route(route_idx);
        }

        /// <summary>
        /// Adds a new route by copying the given one.
        /// </summary>
        /// <param name="route"></param>
        public IRoute Add(IRoute route)
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
                    Array.Resize<int>(ref _first, _first.Length + 1);

                    // set the initial customer.
                    _first[route_idx] = customer;

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
            return this.Route(route_idx);
        }

        /// <summary>
        /// Returns the route at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public IRoute Route(int idx)
        {
            return new MultiRoutePart(_first[idx], _next_array, _is_round);
        }

        /// <summary>
        /// Removes the given route.
        /// </summary>
        /// <param name="route_idx"></param>
        /// <returns></returns>
        public bool Remove(int route_idx)
        {
            int start = _first[route_idx];

            List<int> firsts = new List<int>(_first);
            firsts.RemoveAt(route_idx);
            _first = firsts.ToArray();

            int customer = start;
            do
            {
                int customer_next = _next_array[customer];
                _next_array[customer] = -1;

                customer = customer_next;
            }
            while (customer >= 0 && start != customer);

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

        /// <summary>
        /// Returns true if this route is valid.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            HashSet<int> unique_customers = new HashSet<int>(_next_array);
            unique_customers.Remove(-1);
            int count = 0;
            foreach (int customer in _next_array)
            {
                if (customer >= 0)
                {
                    count++;
                }
            }
            if (unique_customers.Count != count)
            {
                return false;
            }
            return true;
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

            /// <summary>
            /// The next-array.
            /// </summary>
            private int[] _next_array;

            /// <summary>
            /// The first customer.
            /// </summary>
            private int _first;

            /// <summary>
            /// The last customer.
            /// </summary>
            private int _last;

            /// <summary>
            /// Creates a new dynamic route.
            /// </summary>
            /// <param name="first"></param>
            /// <param name="_next_array"></param>
            /// <param name="is_round"></param>
            internal MultiRoutePart(int first, int[] next_array, bool is_round)
            {
                _first = first;
                _next_array = next_array;
                _is_round = is_round;

                // calculate the last customer.
                this.UpdateLast();
            }

            /// <summary>
            /// Updates and sets the last customer.
            /// </summary>
            private void UpdateLast()
            {
                _last = _first;
                while (_next_array[_last] >= 0 && _next_array[_last] != _first)
                {
                    _last = _next_array[_last];
                }
            }

            /// <summary>
            /// Returns true if the route is empty.
            /// </summary>
            public bool IsEmpty
            {
                get
                {
                    return false;
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
                if (_next_array.Length > from)
                { // customers should exist.
                    if (_next_array[from] == to)
                    { // edge found.
                        return true;
                    }
                }
                return false; // array too small.
            }

            /// <summary>
            /// Inserts a customer right after from and before to.
            /// </summary>
            /// <param name="from"></param>
            /// <param name="customer"></param>
            /// <param name="to"></param>
            public void Insert(int from, int customer, int to)
            {
                if (_next_array.Length > from)
                { // customers should exist.
                    // resize the array if needed.
                    if (_next_array.Length <= customer)
                    { // resize the array.
                        this.Resize(customer);
                    }

                    //// get the next customer.
                    //int to = _next_array[from];

                    // insert customer.
                    _next_array[from] = customer;
                    _next_array[customer] = to;

                    // update last.
                    if (_first == to)
                    {
                        _last = customer;
                    }
                    return;
                }
                throw new ArgumentOutOfRangeException("Customer(s) do not exist in this route!");
            }

            /// <summary>
            /// Resizes the array.
            /// </summary>
            /// <param name="customer"></param>
            private void Resize(int customer)
            { // THIS EXPENSIZE! TRY TO ESTIMATE CORRECT SIZE WHEN CREATING ROUTE!
                int old_size = _next_array.Length;
                Array.Resize<int>(ref _next_array, customer + 1);
                for (int new_customer = old_size; new_customer < _next_array.Length; new_customer++)
                { // initialize with -1.
                    _next_array[new_customer] = -1;
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
                neighbour[0] = _next_array[customer];
                return neighbour;
            }

            /// <summary>
            /// Returns true if this route is valid.
            /// </summary>
            /// <returns></returns>
            public bool IsValid()
            {
                HashSet<int> unique_customers = new HashSet<int>(_next_array);
                int count = 0;
                foreach (int customer in _next_array)
                {
                    if (customer >= 0)
                    {
                        count++;
                    }
                }
                if (unique_customers.Count == count)
                {
                    throw new Exception("Unique customer count not correct!");
                }
                return true;
            }

            #region Enumerators

            public IEnumerator<int> GetEnumerator()
            {
                return new Enumerator(_first, _next_array);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new Enumerator(_first, _next_array);
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

            #endregion

            public int Count
            {
                get
                {
                    return this.Count<int>();
                }
            }

            public bool Remove(int customer)
            {
                for (int idx = 0; idx < _next_array.Length - 1; idx++)
                {
                    if (_next_array[idx] == customer)
                    {
                        _next_array[idx] = _next_array[customer];
                        _next_array[customer] = -1;

                        if (customer == _last)
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


            public IEnumerable<int> Between(int from, int to)
            {
                throw new NotImplementedException();
            }
        }

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

        bool IEquatable<DynamicAsymmetricMultiRoute>.Equals(DynamicAsymmetricMultiRoute other)
        {
            return this.Equals(other);
        }

        public bool RemoveCustomer(int customer)
        {
            bool removed = false;

            for (int idx = 0; idx < _first.Length; idx++)
            {
                if (_first[idx] == customer)
                {
                    int next = _next_array[customer];
                    if (next >= 0)
                    {
                        _first[idx] = next;
                    }
                    else
                    { // remove from array
                        List<int> first = new List<int>(_first);
                        first.RemoveAt(idx);
                        _first = first.ToArray();
                    }
                    removed = true;
                }
            }
            for (int idx = 0; idx < _next_array.Length; idx++)
            {
                if (_next_array[idx] == customer)
                {
                    _next_array[idx] = _next_array[customer];
                    _next_array[customer] = -1;
                    removed = true;
                }
            }

            return removed;
        }


        public object Clone()
        {
            return new DynamicAsymmetricMultiRoute(_first, _next_array, _is_round);
        }
    }
}
