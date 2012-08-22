using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.TSP.Problems;

namespace Tools.Math.TSP.LK.SparseSets
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
                SortedSet<KeyValuePair<float, int>> nearest_customers = new SortedSet<KeyValuePair<float, int>>(
                    new SortedSetComparer());

                // build nearest list.
                float highest_weight = float.MaxValue;
                foreach (int neigbour in customers)
                {
                    if (neigbour != customer)
                    { // do not 
                        // calculate weight
                        float weight = problem.Weight(customer, neigbour);

                        // add to nearest list if needed.
                        if (weight < highest_weight)
                        {
                            nearest_customers.Add(new KeyValuePair<float, int>(weight, neigbour));

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
                foreach (KeyValuePair<float, int> neigbour_pair in nearest_customers)
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

        private class SortedSetComparer : IComparer<KeyValuePair<float, int>>
        {
            public int Compare(KeyValuePair<float, int> x, KeyValuePair<float, int> y)
            {
                return x.Key.CompareTo(y.Key);
            }
        }
    }
}
