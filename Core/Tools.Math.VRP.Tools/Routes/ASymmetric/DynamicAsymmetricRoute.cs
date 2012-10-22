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

namespace Tools.Math.VRP.Core.Routes.ASymmetric
{
    public class DynamicAsymmetricRoute : IASymmetricRoute
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
        /// Creates a new dynamic route by creating shallow copy of the array(s) given.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="_next_array"></param>
        /// <param name="is_round"></param>
        private DynamicAsymmetricRoute(int first, int[] next_array, bool is_round)
        {
            _first = first;
            _next_array = next_array.Clone() as int[];
            _is_round = is_round;

            // calculate the last customer.
            this.UpdateLast();
        }

        /// <summary>
        /// Creates a new dynamic assymmetric route using an initial size and customer.
        /// </summary>
        /// <param name="size"></param>
        public DynamicAsymmetricRoute(int size, int customer, bool is_round)
        {
            _is_round = is_round;
            _next_array = new int[size];
            for (int idx = 0; idx < size; idx++)
            {
                _next_array[idx] = -1;
            }
            _first = customer;

            if (_next_array.Length <= customer)
            { // resize the array.
                this.Resize(customer);
            }
            _next_array[customer] = -1;

            // calculate the last customer.
            this.UpdateLast();
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
        /// Returns true if the customer exists in this route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool Contains(int customer)
        {
            if (_next_array.Length > customer)
            {
                return _next_array[customer] >= 0;
            }
            return false;
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

                //// get the to customer.
                //int to = _next_array[from];

                // insert customer.
                _next_array[from] = customer;
                if (to < 0) { }
                else
                {
                    _next_array[customer] = to;
                }

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
        /// Cuts out a part of the route and returns the customers contained.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        public int[] CutAndRemove(int start, int length)
        {
            int[] cut_part = new int[length];
            int position = 0;
            int current_customer = this.First;

            // keep moving next until the start.
            while (position < start - 1)
            {
                position++; // increase the position.
                current_customer = _next_array[current_customer];
            }

            // cut the actual part.
            int start_customer = current_customer;
            while (position < start + length - 1)
            {
                // move next.
                position++; // increase the position.
                current_customer = _next_array[current_customer];

                // set the current customer.
                cut_part[position - start] = current_customer;
            }

            current_customer = _next_array[current_customer];

            // set the next customer.
            _next_array[start_customer] = current_customer;

            return cut_part;
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
        /// Returns the neigbour of the given customer.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public int Next(int customer)
        {
            return _next_array[customer];
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
        /// Creates an exact deep-copy of this route.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new DynamicAsymmetricRoute(_first, _next_array, _is_round);
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

            //private HashSet<int> _customers;

            public Enumerator(int first, int[] next_array)
            {
                _first = first;
                _next_array = next_array;
                //_customers = new HashSet<int>();
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
                    //_customers.Add(_current);
                }
                else
                {
                    _current = _next_array[_current];
                    if (_current == _first)
                    {
                        return false;
                    }
                    //if (_customers.Contains(_current))
                    //{
                    //    throw new Exception("Loop!");
                    //}
                    //_customers.Add(_current);
                }
                return _current >= 0;
            }

            public void Reset()
            {
                _current = -1;
                //_customers.Clear();
            }
        }

        #endregion

        /// <summary>
        /// Returns the size of the route.
        /// </summary>
        public int Count
        {
            get
            {
                return this.Count<int>();
            }
        }

        /// <summary>
        /// Removes a customer from the route.
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public bool Remove(int customer)
        {
            if (customer == _first)
            {
                _first = _next_array[customer];
                _next_array[customer] = -1;
            }
            for (int idx = 0; idx < _next_array.Length; idx++)
            {
                if (_next_array[idx] == customer)
                {
                    _next_array[idx] = _next_array[customer];
                    _next_array[customer] = -1;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the first customer in this route.
        /// </summary>
        public int First
        {
            get 
            {
                return _first; 
            }
        }

        /// <summary>
        /// Returns the last customer in this route.
        /// </summary>
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
            foreach(int possible_customer in this)
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

        #region Static Constructors

        /// <summary>
        /// Creates a dynamic route from an enumerable collection of customers.
        /// </summary>
        /// <param name="customers"></param>
        /// <returns></returns>
        public static DynamicAsymmetricRoute CreateFrom(IEnumerable<int> customers)
        {
            return DynamicAsymmetricRoute.CreateFrom(customers, true);
        }

        /// <summary>
        /// Creates a dynamic route from an enumerable collection of customers.
        /// </summary>
        /// <param name="customers"></param>
        /// <returns></returns>
        public static DynamicAsymmetricRoute CreateFrom(IEnumerable<int> customers, bool is_round)
        {
            DynamicAsymmetricRoute route = null;
            int[] next_array = new int[0];
            int first = -1;
            int previous = -1;
            foreach (int customer in customers)
            {
                // resize the array if needed.
                if (next_array.Length <= customer)
                {
                    Array.Resize<int>(ref next_array, customer + 1);
                }

                // the first customer.
                if (first < 0)
                { // set the first customer.
                    first = customer;
                }
                else
                { // set the next array.
                    next_array[previous] = customer;
                }

                previous = customer;
            }

            next_array[previous] = first;

            // the dynamic route.
            route = new DynamicAsymmetricRoute(first, next_array, is_round);

            return route;
        }

        #endregion

        #region Specific Cut/Paste/Inserts
        
        /// <summary>
        /// Cuts out a part of the route and returns the customers contained.
        /// </summary>
        /// <param name="route"></param>
        /// <param name="start"></param>
        /// <param name="lenght"></param>
        /// <returns></returns>
        public CutResult CutAndRemove(IProblemWeights weights, float weight, int start, int length)
        {
            float weight_difference = 0;
            int[] next_array = _next_array.Clone() as int[];
            List<int> cut_part = new List<int>();
            int position = 0;
            int current_customer = this.First;

            // keep moving next until the start.
            while (position < start - 1)
            {
                position++; // increase the position.
                current_customer = next_array[current_customer];
            }

            // cut the actual part.
            int start_customer = current_customer;
            int previous_customer = current_customer;
            while (position < start + length - 1)
            {
                // move next.
                position++; // increase the position.
                current_customer = next_array[current_customer];

                // set the current customer.
                cut_part.Add(current_customer);

                // add the weigth difference.
                weight_difference = weight_difference -
                    weights.WeightMatrix[previous_customer][current_customer];
                previous_customer = current_customer;
            }

            // move to the next customer.
            current_customer = next_array[current_customer];
            weight_difference = weight_difference -
                weights.WeightMatrix[previous_customer][current_customer];

            // set the next customer.
            next_array[start_customer] = current_customer;
            weight_difference = weight_difference +
                weights.WeightMatrix[start_customer][current_customer];

            // create cut result.
            CutResult result = new CutResult();
            result.Weight = weight + weight_difference;
            result.Route = new DynamicAsymmetricRoute(this.First, next_array, true);
            result.CutPart = cut_part;
            return result;
        }

        public struct CutResult
        {
            public float Weight { get; set; }

            public DynamicAsymmetricRoute Route { get; set; }

            public List<int> CutPart { get; set; }
        }

        #endregion


        public IEnumerable<int> Between(int from, int to)
        {
            return new DynamicAsymmetricBetweenEnumerable(_next_array, from, to);
        }

    }
}
