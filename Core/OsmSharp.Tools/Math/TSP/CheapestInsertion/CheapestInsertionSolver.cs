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
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.VRP.Core.Routes.Symmetric;
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;
using OsmSharp.Tools.Math.VRP.Core.BestPlacement;

namespace OsmSharp.Tools.Math.TSP.CheapestInsertion
{
    /// <summary>
    /// Cheapest insertion solver.
    /// </summary>
    public class CheapestInsertionSolver : SolverBase
    {
        /// <summary>
        /// Keeps the stopped flag.
        /// </summary>
        private bool _stopped = false;

        /// <summary>
        /// Keeps an orginal list of customers.
        /// </summary>
        private IList<int> _customers;

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        public CheapestInsertionSolver()
        {
            _stopped = false;
        }

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="customers"></param>
        public CheapestInsertionSolver(IProblem problem, IList<int> customers)
        {
            _stopped = false;
            _customers = customers;
        }

        /// <summary>
        /// Retuns the name of this solver.
        /// </summary>
        public override string Name
        {
            get
            {
                return "CI";
            }
        }

        /// <summary>
        /// Returns a solution found using best-placement.
        /// </summary>
        /// <returns></returns>
        protected override IRoute DoSolve(IProblem problem)
        {
            // build the customer list to place.
            List<int> customers = null;
            if (_customers != null)
            { // copy the list of the given customers and keep this order.
                customers = new List<int>(_customers);
            }
            else
            { // generate some random route.
                customers = new List<int>();
                List<int> customers_to_place = new List<int>();
                for (int customer = 0; customer < problem.Size; customer++)
                {
                    customers_to_place.Add(customer);
                }
                while (customers_to_place.Count > 0)
                {
                    int idx = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(customers_to_place.Count);
                    customers.Add(customers_to_place[idx]);
                    customers_to_place.RemoveAt(idx);
                }
            }

            // initialize the route based on the problem definition.
            IRoute route = null;
            double weight = double.MaxValue;
            if (problem.Symmetric)
            { // create a symmetric route that is dynamic and can accept new customers.
                if (problem.First.HasValue && problem.Last.HasValue && problem.First == problem.Last)
                { // route is a round.
                    route = new DynamicSymmetricRoute(problem.First.Value);
                }
                else
                { // not a round.
                    throw new NotImplementedException("No symmetric routes implemented that are not rounds!");
                }
            }
            else
            { // create a asymmetric route that is dynamic and can accept new customers.
                if (problem.First.HasValue)
                { // the first customer is set.
                    // test if the last customer is the same.
                    if (!problem.Last.HasValue ||
                        problem.Last == problem.First)
                    { // the route is a round.
                        route = new DynamicAsymmetricRoute(customers.Count, problem.First.Value, true);

                        // remove the first customer.
                        customers.Remove(problem.First.Value);
                    }
                    else
                    { // the route is not a round.
                        route = new DynamicAsymmetricRoute(customers.Count, problem.First.Value, false);
                        route.InsertAfter(problem.First.Value, problem.Last.Value);

                        // remove the first customer.
                        customers.Remove(problem.First.Value);
                        customers.Remove(problem.Last.Value);
                    }
                }
                else
                { // the first and last customer can be choosen randomly.
                    // find two customers close together.
                    int from = -1;
                    int to = -1;
                    for (int x = 0; x < customers.Count; x++)
                    {
                        for (int y = 0; y < customers.Count; y++)
                        {
                            if (x != y)
                            { // only different customers.
                                double current_weight = problem.WeightMatrix[x][y];
                                if (current_weight < weight)
                                { // the current weight is better.
                                    from = x;
                                    to = y;
                                    weight = current_weight;

                                    if (weight == 0)
                                    { // no edge with less weight is going to be found.
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    route = new DynamicAsymmetricRoute(customers.Count, from, false);
                    route.InsertAfter(from, to);

                    // remove the first customer.
                    customers.Remove(from);
                    customers.Remove(to);
                }
            }

            // insert the rest of the customers.
            while (customers.Count > 0 && !_stopped)
            { // keep placing customer 0 until all customers are placed.
                // calculate placement.
                CheapestInsertionResult result =
                    CheapestInsertionHelper.CalculateBestPlacement(problem, route, customers);

                // place the customer.
                if (result.CustomerAfter >= 0 && result.CustomerBefore >= 0)
                {
                    //route.InsertAfterAndRemove(result.CustomerBefore, result.Customer, result.CustomerAfter);
                    route.InsertAfter(result.CustomerBefore, result.Customer);
                    customers.Remove(result.Customer);
                }
            }

            return route;
        }

        /// <summary>
        /// Stops executiong.
        /// </summary>
        public override void Stop()
        {
            _stopped = true;
        }
    }
}
