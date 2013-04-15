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
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.VRP.Core.BestPlacement.InsertionCosts;

namespace OsmSharp.Tools.Math.VRP.Core.BestPlacement
{
    /// <summary>
    /// Implements some generic functions for best-placement.
    /// </summary>
    public class CheapestInsertionHelper
    {
        /// <summary>
        /// Returns the customer that least increases the length of the given route.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="customers"></param>
        /// <returns></returns>
        public static CheapestInsertionResult CalculateBestPlacement(
            IProblemWeights problem,
            IRoute route,
            ICollection<int> customers)
        {
            IInsertionCosts costs = new BinaryHeapInsertionCosts();

            return CheapestInsertionHelper.CalculateBestPlacement(problem, route, customers, costs);
        }

        /// <summary>
        /// Returns the customer that least increases the length of the given route.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="customers"></param>
        /// <param name="costs"></param>
        /// <returns></returns>
        public static CheapestInsertionResult CalculateBestPlacement(
            IProblemWeights problem,
            IRoute route,
            ICollection<int> customers,
            IInsertionCosts costs)
        {  
            // initialize the best placement result.
            CheapestInsertionResult best = new CheapestInsertionResult();
            best.Increase = float.MaxValue;

            // loop over all customers in the route.
            if (route.Count > 0)
            { // there have to be at least two customers.
                IEnumerator<int> route_enumerator = route.GetEnumerator();
                if (!route_enumerator.MoveNext())
                { // this route is empty
                    throw new ArgumentException("Route needs to be initialized with at least two customers!");
                }
                int customer_before = route_enumerator.Current;
                int customer_after = -1;
                while (route_enumerator.MoveNext())
                { // keep moving!
                    customer_after = route_enumerator.Current;
                    InsertionCost cost = costs.PopCheapest(customer_before, customer_after);
                    bool found = false;
                    while(!found)
                    { // test if the costs are empty.
                        if (cost == null)
                        { // re-initialize the costs with all customers under consideration.
                            foreach (int customer in customers)
                            {
                                costs.Add(customer_before, customer_after, customer, 
                                    (float)problem.WeightMatrix[customer_before][customer] +
                                    (float)problem.WeightMatrix[customer][customer_after] -
                                    (float)problem.WeightMatrix[customer_before][customer_after]);
                            }

                            // pop the cheapest again!
                            cost = costs.PopCheapest(customer_before, customer_after);
                        }
                        else if (customers.Contains(cost.Customer))
                        { // the customer is found!
                            found = true;
                        }
                        else
                        { // pop the cheapest again!
                            cost = costs.PopCheapest(customer_before, customer_after);
                        }
                    }

                    if (cost.Cost < best.Increase)
                    { // the costs is better than the current cost!
                        best = new CheapestInsertionResult()
                            {
                                Customer = cost.Customer,
                                CustomerAfter = customer_after,
                                CustomerBefore = customer_before,
                                Increase = cost.Cost
                            };
                        if (best.Increase == 0)
                        { // immidiately return if smaller than epsilon.
                            return best;
                        }
                    }

                    // set the after to the before.
                    customer_before = route_enumerator.Current;
                }

                // if the round is a round try first-last.
                if (route.IsRound)
                { // the route is a round!
                    customer_after = route.First;
                    InsertionCost cost = costs.PopCheapest(customer_before, customer_after);
                    bool found = false;
                    while (!found)
                    { // test if the costs are empty.
                        if (cost == null)
                        { // re-initialize the costs with all customers under consideration.
                            foreach (int customer in customers)
                            {
                                costs.Add(customer_before, customer_after, customer,
                                    (float)problem.WeightMatrix[customer_before][customer] +
                                    (float)problem.WeightMatrix[customer][customer_after] -
                                    (float)problem.WeightMatrix[customer_before][customer_after]);
                            }

                            // pop the cheapest again!
                            cost = costs.PopCheapest(customer_before, customer_after);
                        }
                        else if (customers.Contains(cost.Customer))
                        { // the customer is found!
                            found = true;
                        }
                        else
                        { // pop the cheapest again!
                            cost = costs.PopCheapest(customer_before, customer_after);
                        }
                    }

                    if (cost.Cost < best.Increase)
                    { // the costs is better than the current cost!
                        best = new CheapestInsertionResult()
                        {
                            Customer = cost.Customer,
                            CustomerAfter = customer_after,
                            CustomerBefore = customer_before,
                            Increase = cost.Cost
                        };
                        if (best.Increase == 0)
                        { // immidiately return if smaller than epsilon.
                            return best;
                        }
                    }
                }
            }
            else
            { // route needs to be initialized.
                throw new ArgumentException("Route needs to be initialized with at least two customers!");
            }

            // return result.
            return best;
        }

        /// <summary>
        /// Returns the customer that least increases the length of the given route.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="customers"></param>
        /// <param name="seed_customer"></param>
        /// <param name="seed_customer_ratio"></param>
        /// <returns></returns>
        public static CheapestInsertionResult CalculateBestPlacement(
            IProblemWeights problem,
            IRoute route,
            ICollection<int> customers,
            int seed_customer,
            double seed_customer_ratio)
        {
            IInsertionCosts costs = new BinaryHeapInsertionCosts();

            // initialize the best placement result.
            CheapestInsertionResult best = new CheapestInsertionResult();
            best.Increase = float.MaxValue;

            // loop over all customers in the route.
            if (route.Count > 0)
            { // there have to be at least two customers.
                IEnumerator<int> route_enumerator = route.GetEnumerator();
                if (!route_enumerator.MoveNext())
                { // this route is empty
                    throw new ArgumentException("Route needs to be initialized with at least two customers!");
                }
                int customer_before = route_enumerator.Current;
                int customer_after = -1;
                while (route_enumerator.MoveNext())
                { // keep moving!
                    customer_after = route_enumerator.Current;
                    InsertionCost cost = costs.PopCheapest(customer_before, customer_after);
                    bool found = false;
                    while (!found)
                    { // test if the costs are empty.
                        if (cost == null)
                        { // re-initialize the costs with all customers under consideration.
                            foreach (int customer in customers)
                            {
                                float cost_quantity = (float)problem.WeightMatrix[customer_before][customer] +
                                    (float)problem.WeightMatrix[customer][customer_after] -
                                    (float)problem.WeightMatrix[customer_before][customer_after];
                                float seed_cost = (float)problem.WeightMatrix[seed_customer][customer] +
                                    (float)problem.WeightMatrix[customer][seed_customer];
                                costs.Add(customer_before, customer_after, customer,
                                    (float)(cost_quantity + (seed_customer_ratio * seed_cost)));
                            }

                            // pop the cheapest again!
                            cost = costs.PopCheapest(customer_before, customer_after);
                        }
                        else if (customers.Contains(cost.Customer))
                        { // the customer is found!
                            found = true;
                        }
                        else
                        { // pop the cheapest again!
                            cost = costs.PopCheapest(customer_before, customer_after);
                        }
                    }

                    if (cost.Cost < best.Increase)
                    { // the costs is better than the current cost!
                        best = new CheapestInsertionResult()
                        {
                            Customer = cost.Customer,
                            CustomerAfter = customer_after,
                            CustomerBefore = customer_before,
                            Increase = cost.Cost
                        };
                        if (best.Increase == 0)
                        { // immidiately return if smaller than epsilon.
                            return best;
                        }
                    }

                    // set the after to the before.
                    customer_before = route_enumerator.Current;
                }

                // if the round is a round try first-last.
                if (route.IsRound)
                { // the route is a round!
                    customer_after = route.First;
                    InsertionCost cost = costs.PopCheapest(customer_before, customer_after);
                    bool found = false;
                    while (!found)
                    { // test if the costs are empty.
                        if (cost == null)
                        { // re-initialize the costs with all customers under consideration.
                            foreach (int customer in customers)
                            {
                                float cost_quantity = (float)problem.WeightMatrix[customer_before][customer] +
                                    (float)problem.WeightMatrix[customer][customer_after] -
                                    (float)problem.WeightMatrix[customer_before][customer_after];
                                float seed_cost = (float)problem.WeightMatrix[seed_customer][customer] +
                                    (float)problem.WeightMatrix[customer][seed_customer];
                                costs.Add(customer_before, customer_after, customer,
                                    (float)(cost_quantity + (seed_customer_ratio * seed_cost)));
                            }

                            // pop the cheapest again!
                            cost = costs.PopCheapest(customer_before, customer_after);
                        }
                        else if (customers.Contains(cost.Customer))
                        { // the customer is found!
                            found = true;
                        }
                        else
                        { // pop the cheapest again!
                            cost = costs.PopCheapest(customer_before, customer_after);
                        }
                    }

                    if (cost.Cost < best.Increase)
                    { // the costs is better than the current cost!
                        best = new CheapestInsertionResult()
                        {
                            Customer = cost.Customer,
                            CustomerAfter = customer_after,
                            CustomerBefore = customer_before,
                            Increase = cost.Cost
                        };
                        if (best.Increase == 0)
                        { // immidiately return if smaller than epsilon.
                            return best;
                        }
                    }
                }
            }
            else
            { // route needs to be initialized.
                throw new ArgumentException("Route needs to be initialized with at least two customers!");
            }

            // return result.
            return best;
        }

        /// <summary>
        /// Searches for the best place to insert the given customer.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static CheapestInsertionResult CalculateBestPlacement(
            IProblemWeights problem,
            IRoute route,
            int customer)
        {  // initialize the best placement result.
            double[][] weights = problem.WeightMatrix;

            CheapestInsertionResult result
                = new CheapestInsertionResult();
            result.Customer = customer;
            result.CustomerAfter = -1;
            result.CustomerBefore = -1;
            result.Increase = double.MaxValue;

            double difference = double.MaxValue;
            if (!route.IsEmpty)
            {
                double new_weight = double.MaxValue;
                double old_weight = 0;

                int previous = -1;
                int first = -1;
                foreach (int current in route)
                {
                    if (previous >= 0)
                    { // the previous customer exists.
                        // only if the previous is known.

                        // calculate the old weights.
                        //old_weight = problem.Weight(previous, current);
                        old_weight = weights[previous][current];

                        // calculate the new weights.
                        //new_weight = problem.Weight(previous, customer);
                        new_weight = weights[previous][customer];

                        //new_weight = new_weight +
                        //    problem.Weight(customer, current);
                        new_weight = new_weight +
                            weights[customer][current];

                        // calculate the difference.
                        difference = new_weight - old_weight;
                        if (result.Increase > difference)
                        {
                            result.CustomerAfter = current;
                            result.CustomerBefore = previous;
                            result.Increase = difference;

                            // if the difference is equal to or smaller than epsilon we have search enough.
                            if (difference == 0)
                            {
                                // result is the best we will be able to get.
                                return result;
                            }
                        }
                    }
                    else
                    { // store the first city for later.
                        first = current;
                    }

                    // go to the next loop.
                    previous = current;
                }

                // set the pervious to the last.
                //previous = route.Last;

                // test last-to-first if the route is a round.
                if (route.IsRound)
                {
                    // calculate the new weights.
                    //new_weight = problem.Weight(previous, customer)
                    //        + (problem.Weight(customer, first));
                    new_weight = weights[previous][customer]
                        + weights[customer][first];

                    // calculate the old weights.
                    //old_weight = problem.Weight(previous, first);
                    old_weight = weights[previous][first];

                    // calculate the difference.
                    difference = new_weight - old_weight;
                    if (result.Increase > difference)
                    {
                        result.CustomerAfter = first;
                        result.CustomerBefore = previous;
                        result.Increase = difference;
                    }
                }
            }
            else
            { // route needs to be initialized.
                throw new ArgumentException("Route needs to be initialized with at least one customer!");
            }

            // return result.
            return result;
        }

        /// <summary>
        /// Searches for the best place to insert the given two customers abstracting the distance between them.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static CheapestInsertionResult CalculateBestPlacement(
            IProblemWeights problem,
            IRoute route,
            int from,
            int to)
        {  // initialize the best placement result.
            CheapestInsertionResult result
                = new CheapestInsertionResult();
            result.Customer = -1; // this property is useless here!
            result.CustomerAfter = -1;
            result.CustomerBefore = -1;
            result.Increase = double.MaxValue;

            if (!route.IsEmpty)
            {
                double new_weight = double.MaxValue;
                double old_weight = 0;

                int previous = -1;
                int first = -1;
                foreach (int current in route)
                {
                    if (previous >= 0)
                    { // only if the previous is known.
                        // calculate the new weights.
                        new_weight = problem.Weight(previous, from)
                                + (problem.Weight(to, current));

                        // calculate the old weights.
                        old_weight = problem.Weight(previous, current);

                        // calculate the difference.
                        double difference = new_weight - old_weight;
                        if (result.Increase > difference)
                        {
                            result.CustomerAfter = current;
                            result.CustomerBefore = previous;
                            result.Increase = difference;
                        }
                    }
                    else
                    { // store the first city for later.
                        first = current;
                    }

                    // go to the next loop.
                    previous = current;
                }

                // test last-to-first if the route is a round.
                if (route.IsRound)
                {
                    // calculate the new weights.
                    new_weight = problem.Weight(previous, from)
                            + (problem.Weight(to, first));

                    // calculate the old weights.
                    old_weight = problem.Weight(previous, first);

                    // calculate the difference.
                    double difference = new_weight - old_weight;
                    if (result.Increase > difference)
                    {
                        result.CustomerBefore = previous;
                        result.CustomerAfter = first;
                        result.Increase = difference;
                    }
                }
            }
            else
            { // route needs to be initialized.
                throw new ArgumentException("Route needs to be initialized with at least one customer!");
            }

            // return result.
            return result;
        }
    }
}
