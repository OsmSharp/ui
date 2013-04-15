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
using OsmSharp.Tools.Math.TSP.Problems;

namespace OsmSharp.Tools.Math.TSP.LK.SparseSets
{
    internal static class SparseSetHelper
    {
        internal static SparseSet CreateNonSparseSet(IProblem problem, IList<int> customers)
        {            
            // initialize sparse set.
            SparseSet set = new SparseSet();

            // loop over all customers
            foreach (int customer in customers)
            {
                // loop over all customers again.
                foreach (int neighbour in customers)
                {
                    if (neighbour != customer)
                    { // do not 
                        set.Add(new Edge()
                        {
                            From = customer,
                            To = neighbour,
                            Weight = problem.Weight(customer, neighbour)
                        });
                    }
                }
            }
            return set;
        }

        internal static SparseSet CreateNearestNeighourSet(IProblem problem, IList<int> customers, int n)
        {
            // initialize sparse set.
            SparseSet set = new SparseSet();

            // loop over all customers and add their n-nearest neigbours.
            foreach (int customer in customers)
            {
                // initialize nearest list.
                SortedSet<KeyValuePair<double, int>> nearest_customers = new SortedSet<KeyValuePair<double, int>>(
                    new SortedSetComparer());

                // build nearest list.
                double highest_weight = double.MaxValue;
                foreach (int neigbour in customers)
                {
                    if (neigbour != customer)
                    { // do not 
                        // calculate weight
                        double weight = problem.Weight(customer, neigbour);

                        // add to nearest list if needed.
                        if (weight < highest_weight)
                        {
                            nearest_customers.Add(new KeyValuePair<double, int>(weight, neigbour));

                            // remove highest weight.
                            if (nearest_customers.Count > n)
                            {
                                nearest_customers.Remove(nearest_customers.Max);
                            }

                            // set highest weight again.
                            if (nearest_customers.Count == n)
                            {
                                highest_weight = nearest_customers.Max.Key;
                            }
                        }
                    }
                }

                // add nearest list to sparseset.
                foreach (KeyValuePair<double, int> neigbour_pair in nearest_customers)
                {
                    set.Add(new Edge()
                    {
                        From = customer,
                        To = neigbour_pair.Value,
                        Weight = neigbour_pair.Key
                    });
                }
            }
            return set;
        }

        private class SortedSetComparer : IComparer<KeyValuePair<double, int>>
        {
            public int Compare(KeyValuePair<double, int> x, KeyValuePair<double, int> y)
            {
                return x.Key.CompareTo(y.Key);
            }
        }
    }
}
