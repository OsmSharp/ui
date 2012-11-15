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
    public class CheapestInsertionSolver : ISolver
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
        /// <param name="problem"></param>
        public CheapestInsertionSolver()
        {
            _stopped = false;
        }

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        /// <param name="problem"></param>
        public CheapestInsertionSolver(IProblem problem, IList<int> customers)
        {
            _stopped = false;
            _customers = customers;
        }

        /// <summary>
        /// Retuns the name of this solver.
        /// </summary>
        public string Name
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
        public IRoute Solve(IProblem problem)
        {
            // build the customer list to place.
            List<int> customers = new List<int>();
            for (int customer = 0; customer < problem.Size; customer++)
            {
                customers.Add(customer);
            }

            // initialize the route based on the problem definition.
            IRoute route = null;

            bool is_round;
            int first;

            if (problem.First.HasValue)
            { // the first customer is set.
                // test if the last customer is the same.
                if (problem.Last == problem.First)
                { // the route is a round.
                    is_round = true;
                    first = problem.First.Value;
                }
                else
                { // the route is not a round.
                    is_round = false;
                    first = problem.First.Value;
                }
            }
            else
            { // the first and last customer can be choosen randomly.
                is_round = false;
                first = _customers[0];
            }

            if (problem.Symmetric)
            { // create a symmetric route that is dynamic and can accept new customers.
                if (is_round)
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
                route = new DynamicAsymmetricRoute(customers.Count, first, is_round);
            }

            // remove the first customer.
            customers.Remove(first);

            // insert the rest of the customers.
            while (customers.Count > 0 && !_stopped)
            { // keep placing customer 0 until all customers are placed.
                // calculate placement.
                CheapestInsertionResult result =
                    CheapestInsertionHelper.CalculateBestPlacement(problem, route, customers);

                // place the customer.
                if (result.CustomerAfter >= 0 && result.CustomerBefore >= 0)
                {
                    route.Insert(result.CustomerBefore, result.Customer, result.CustomerAfter);

                    customers.Remove(result.Customer);
                }
            }

            return route;
        }

        /// <summary>
        /// Stops executiong.
        /// </summary>
        public void Stop()
        {
            _stopped = true;
        }

        #region Intermidiate Results

        /// <summary>
        /// Raised when an intermidiate result is available.
        /// </summary>
        public event SolverDelegates.IntermidiateDelegate IntermidiateResult;

        /// <summary>
        /// Returns true when the event has to be raised.
        /// </summary>
        /// <returns></returns>
        protected bool CanRaiseIntermidiateResult()
        {
            return this.IntermidiateResult != null;
        }

        /// <summary>
        /// Raises the intermidiate results event.
        /// </summary>
        /// <param name="result"></param>
        protected void RaiseIntermidiateResult(int[] result, float weight)
        {
            if (IntermidiateResult != null)
            {
                this.IntermidiateResult(result, weight);
            }
        }

        #endregion
    }
}
