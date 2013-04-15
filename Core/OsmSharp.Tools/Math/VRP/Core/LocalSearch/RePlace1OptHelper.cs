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
using OsmSharp.Tools.Collections;

namespace OsmSharp.Tools.Math.VRP.Core.LocalSearch
{
    /// <summary>
    /// Responsible for an local search strategy doing best-placement.
    /// </summary>
    public class RePlace1OptHelper
    {
        /// <summary>
        /// Applies a local search strategy.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        public static void CalculateRePlaceOptHelper(
            IProblemWeights problem, IRoute route)
        {
            //bool improvement = true;
            //while (improvement)
            //{
            //// reset improvement flag.
            //improvement = false;

            // try re-placement of one customer.
            int before = -1;
            int after = -1;
            int found_customer = -1;

            // loop over all customers and try to place it better.
            HashSet<int> customers_to_place = new HashSet<int>(route);
            foreach (int customer_to_place in customers_to_place)
            {
                // find the best place.
                double current_cost = -1;
                double other_cost = double.MaxValue;
                int previous_before = -1;
                int previous = -1;
                foreach (int customer in route)
                {
                    if (previous >= 0 && previous_before >= 0)
                    {
                        if (previous == customer_to_place)
                        {
                            current_cost = problem.Weight(previous_before, previous) +
                                problem.Weight(previous, customer);
                        }
                        else if (previous_before != customer_to_place &&
                            customer != customer_to_place)
                        { // calculate the cost.
                            double cost = problem.Weight(previous_before, customer_to_place) +
                                problem.Weight(customer_to_place, previous);
                            if (cost < other_cost)
                            {
                                other_cost = cost;

                                before = previous_before;
                                after = previous;
                                found_customer = customer;
                            }
                        }
                    }

                    previous_before = previous;
                    previous = customer;
                }

                // determine if the cost is better.
                if (current_cost > other_cost)
                { // the current cost is better.
                    route.Remove(found_customer);
                    //route.InsertAfterAndRemove(before, found_customer, after);
                    route.InsertAfter(before, found_customer);
                    break;
                }
                else
                { // current cost is not better.                        
                    before = -1;
                    after = -1;
                    found_customer = -1;
                }
            }

            //if (found_customer >= 0)
            //{ // a found customer.
            //    improvement = true;
            //}
            //}
        }
    }
}
