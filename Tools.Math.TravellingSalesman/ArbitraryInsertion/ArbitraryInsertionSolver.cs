using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.TSP.Problems;
using Tools.Math.VRP.Core;
using Tools.Math.VRP.Core.BestPlacement;
using Tools.Math.VRP.Core.Routes;
using Tools.Math.VRP.Core.Routes.Symmetric;
using Tools.Math.VRP.Core.Routes.ASymmetric;

namespace Tools.Math.TSP.ArbitraryInsertion
{
    /// <summary>
    /// Implements a best-placement solver.
    /// </summary>
    public class ArbitraryInsertionSolver : ISolver
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
        public ArbitraryInsertionSolver()
        {
            _stopped = false;
        }

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        /// <param name="problem"></param>
        public ArbitraryInsertionSolver(IProblem problem, IList<int> customers)
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
                return "AI";
            }
        }

        /// <summary>
        /// Returns a solution found using best-placement.
        /// </summary>
        /// <returns></returns>
        public IRoute Solve(IProblem problem)
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
                    int idx = Tools.Math.Random.StaticRandomGenerator.Get().Generate(customers_to_place.Count);
                    customers.Add(customers_to_place[idx]);
                    customers_to_place.RemoveAt(idx);
                }
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
                int customer = customers[0];
                customers.RemoveAt(0);

                // calculate placement.
                BestPlacementResult result =
                    BestPlacementHelper.CalculateBestPlacement(problem, route, customer);

                // place the customer.
                if (result.CustomerAfter >= 0 && result.CustomerBefore >= 0)
                {
                    route.Insert(result.CustomerBefore, result.Customer, result.CustomerAfter);
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
