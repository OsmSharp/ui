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
using OsmSharp.Tools.Math.VRP.Core;
using OsmSharp.Tools.Math.VRP.Core.BestPlacement;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.VRP.Core.Routes.Symmetric;
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;

namespace OsmSharp.Tools.Math.TSP.ArbitraryInsertion
{
    /// <summary>
    /// Implements a best-placement solver.
    /// </summary>
    public class ArbitraryInsertionSolver : SolverBase, IImprovement
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
        public ArbitraryInsertionSolver()
        {
            _stopped = false;
        }

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        /// <param name="customers"></param>
        public ArbitraryInsertionSolver(IList<int> customers)
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
                return "AI";
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

                        // find the customer that is farthest away and add it.
                        int to = -1;
                        weight = double.MinValue;
                        for (int x = 0; x < customers.Count; x++)
                        {
                            if (x != problem.First.Value)
                            { // only different customers.
                                double current_weight = problem.WeightMatrix[x][problem.First.Value] +
                                    problem.WeightMatrix[problem.First.Value][x];
                                if (current_weight > weight)
                                { // the current weight is better.
                                    to = x;
                                    weight = current_weight;
                                }
                            }
                        }
                        route.InsertAfter(problem.First.Value, to);
                        customers.Remove(to);
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
                int customer = customers[0];
                customers.RemoveAt(0);

                // insert the customer at the best place.
                double difference;
                ArbitraryInsertionSolver.InsertOne(problem, route, customer, out difference);
            }

            return route;
        }

        /// <summary>
        /// Tries to improve the existing route using CI and return true if succesful.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="difference"></param>
        /// <returns></returns>
        public bool Improve(IProblemWeights problem, IRoute route, out double difference)
        {
            bool improvement = false;

            difference = 0;
            if (route.Count > 3)
            {
                // loop over all customers and try cheapest insertion.
                for (int customer = 0; customer < problem.Size; customer++)
                {
                    string route_string = route.ToString();
                    //IRoute previous = route.Clone() as IRoute;
                    if (route.Contains(customer))
                    {
                        // remove customer and keep position.
                        int next = route.GetNeigbours(customer)[0];
                        route.Remove(customer);

                        // insert again.
                        ArbitraryInsertionSolver.InsertOne(problem, route, customer, out difference);

                        if (!route.IsValid())
                        {
                            throw new Exception();
                        }
                        if (route.GetNeigbours(customer)[0] != next
                            && difference < 0)
                        { // another customer was found as the best, improvement is succesful.
                            improvement = true;
                            break;
                        }
                    }
                }
            }
            return improvement;
        }

        /// <summary>
        /// Re-inserts a customer in the route.
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="route"></param>
        /// <param name="customer"></param>
        /// <param name="difference"></param>
        /// <returns></returns>
        public static bool InsertOne(IProblemWeights weights, IRoute route, int customer,
            out double difference)
        {
            // calculate placement.
            CheapestInsertionResult result =
                CheapestInsertionHelper.CalculateBestPlacement(weights, route, customer);

            // place the customer.
            if (result.CustomerAfter >= 0 && result.CustomerBefore >= 0)
            {
                //route.InsertAfterAndRemove(result.CustomerBefore, result.Customer, result.CustomerAfter);
                route.InsertAfter(result.CustomerBefore, result.Customer);
                difference = 
                    -(weights.WeightMatrix[result.CustomerBefore][result.CustomerAfter]) +
                     (weights.WeightMatrix[result.CustomerBefore][result.Customer]) +
                     (weights.WeightMatrix[result.Customer][result.CustomerAfter]);
                return true;
            }
            difference = 0;
            return false;
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
