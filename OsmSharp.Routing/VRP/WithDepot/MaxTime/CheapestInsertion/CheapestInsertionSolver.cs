// OsmSharp - OpenStreetMap (OSM) SDK
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
using OsmSharp.Units.Time;
using OsmSharp.Math.VRP.Core;
using OsmSharp.Math.VRP.Core.BestPlacement;
using OsmSharp.Math.VRP.Core.Routes;
using OsmSharp.Math.VRP.Core.Routes.ASymmetric;
using OsmSharp.Math.TSP;
using OsmSharp.Routing;
using OsmSharp.Math.VRP.Core.BestPlacement.InsertionCosts;
using OsmSharp.Math.TSP.LocalSearch.HillClimbing3Opt;
using OsmSharp.Routing.VRP.WithDepot.MaxTime.InterImprovements;

namespace OsmSharp.Routing.VRP.WithDepot.MaxTime.CheapestInsertion
{
    /// <summary>
    /// Executes a cheapest insertion procedures with improvements following VND strategy.
    /// </summary>
    public class CheapestInsertionSolverWithImprovements : RouterMaxTime
    {
        /// <summary>
        /// The amount of customers to place before applying local improvements.
        /// </summary>
        private readonly int _k;

        /// <summary>
        /// The percentage bound of space to leave for future improvements.
        /// </summary>
        private readonly float _deltaPercentage;

        /// <summary>
        /// Holds the intra-route improvements;
        /// </summary>
        private readonly List<IImprovement> _intraImprovements;

        /// <summary>
        /// Holds the inter-route improvements.
        /// </summary>
        private readonly List<IInterImprovement> _interImprovements;

        /// <summary>
        /// Flag to configure seed costs.
        /// </summary>
        private readonly bool _useSeedCost;

        /// <summary>
        /// The threshold percentage.
        /// </summary>
        private readonly float _thresholdPercentage;

        /// <summary>
        /// Creates a new best placement min max no depot vrp router.
        /// </summary>
        /// <param name="router"></param>
        /// <param name="max"></param>
        /// <param name="deliveryTime"></param>
        /// <param name="k"></param>
        /// <param name="deltaPercentage"></param>
        /// <param name="useSeedCost"></param>
        /// <param name="thresholdPrecentage"></param>
        public CheapestInsertionSolverWithImprovements(Router router,
            Second max, Second deliveryTime, int k, float deltaPercentage, bool useSeedCost, 
            float thresholdPrecentage)
            : base(max, deliveryTime)
        {
            _k = k;
            _deltaPercentage = deltaPercentage;
            _useSeedCost = useSeedCost;
            _thresholdPercentage = thresholdPrecentage;

            _intraImprovements = new List<IImprovement>();
            //_intra_improvements.Add(
            //    new OsmSharp.Math.TSP.ArbitraryInsertion.ArbitraryInsertionSolver());
            _intraImprovements.Add(
                new HillClimbing3OptSolver(true, true));

            _interImprovements = new List<IInterImprovement>();
            _interImprovements.Add(
                new ExchangeInterImprovement());
            _interImprovements.Add(
                new RelocateImprovement());
            //_inter_improvements.Add(
            //    new TwoOptInterImprovement());
            _interImprovements.Add(
                new RelocateExchangeInterImprovement());
            _interImprovements.Add(
                new CrossExchangeInterImprovement());
        }

        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public override string Name
        {
            get
            {
                return "CI_IMP";
            }
        }

        /// <summary>
        /// Executes a solver procedure.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        internal override MaxTimeSolution Solve(MaxTimeProblem problem)
        {
            // create the solution.
            var solution = new MaxTimeSolution(problem.Size);

            // keep placing customer until none are left.
            var customers = new List<int>(problem.Customers);
            customers.RemoveAt(0);

            double max = problem.Max.Value - (problem.Max.Value * _deltaPercentage);

            // keep a list of cheapest insertions.
            IInsertionCosts costs = new BinaryHeapInsertionCosts();
            double percentage = _thresholdPercentage;
            while (customers.Count > 0)
            {
                //// try and distribute the remaining customers if there are only a few left.
                //if (customers.Count < problem.Size * percentage)
                //{
                //    bool succes = true;
                //    while (succes && customers.Count > 0)
                //    {
                //        succes = false;
                //        CheapestInsertionResult best = new CheapestInsertionResult();
                //        best.Increase = float.MaxValue;
                //        int best_idx = -1;
                //        for (int route_idx = 0; route_idx < solution.Count; route_idx++)
                //        {
                //            IRoute route = solution.Route(route_idx);
                //            CheapestInsertionResult result =
                //                CheapestInsertionHelper.CalculateBestPlacement(problem, route, customers);
                //            if (best.Increase > result.Increase)
                //            {
                //                best = result;
                //                best_idx = route_idx;
                //            }
                //        }

                //        IRoute best_route = solution.Route(best_idx);
                //        double route_time = problem.Time(best_route);
                //        if (route_time + best.Increase < max)
                //        { // insert the customer.
                //            best_route.InsertAfter(best.CustomerBefore, best.Customer);
                //            customers.Remove(best.Customer);

                //            this.Improve(problem, solution, max, best_idx);

                //            succes = true;
                //        }
                //    }
                //}

                // select a customer using some heuristic.
                int customer = this.SelectSeed(problem, problem.MaxTimeCalculator, solution, customers);
                customers.Remove(customer);

                // start a route r.
                IRoute currentRoute = solution.Add(customer);
                solution[solution.Count - 1] = 0;

                while (customers.Count > 0)
                {
                    // calculate the best placement.
                    CheapestInsertionResult result;
                    if (_useSeedCost)
                    { // use the seed cost; the cost to the seed customer.
                        result = CheapestInsertionHelper.CalculateBestPlacement(problem, currentRoute, customers,
                            customer, 0.7);

                        // calculate the 'real' increase.
                        result.Increase = (problem.WeightMatrix[result.CustomerBefore][result.Customer] +
                            problem.WeightMatrix[result.Customer][result.CustomerAfter]) -
                            problem.WeightMatrix[result.CustomerBefore][result.CustomerAfter];
                    }
                    else
                    { // just use cheapest insertion.
                        result = CheapestInsertionHelper.CalculateBestPlacement(problem, currentRoute, customers, costs);
                    }

                    // calculate the new weight.
                    solution[solution.Count - 1] = problem.Time(solution.Route(solution.Count - 1));
                    double potentialWeight = problem.MaxTimeCalculator.CalculateOneRouteIncrease(solution[solution.Count - 1],
                        result.Increase);
                    // cram as many customers into one route as possible.
                    if (potentialWeight < max)
                    {
                        // insert the customer, it is 
                        customers.Remove(result.Customer);
                        currentRoute.InsertAfter(result.CustomerBefore, result.Customer);

                        // free some memory in the costs list.
                        costs.Remove(result.CustomerBefore, result.CustomerAfter);

                        // update the cost of the route.
                        solution[solution.Count - 1] = potentialWeight;

                        // improve if needed.
                        if (((problem.Size - customers.Count) % _k) == 0)
                        { // an improvement is decided.
                            // apply the inter-route improvements.
                            var copy = (solution.Clone() as MaxTimeSolution);

                            int countBefore = solution.Route(solution.Count - 1).Count;

                            solution[solution.Count - 1] = this.ImproveIntraRoute(problem,
                                solution.Route(solution.Count - 1), solution[solution.Count - 1]);
                            if (!solution.IsValid())
                            {
                                throw new Exception();
                            }
                            int countAfter = solution.Route(solution.Count - 1).Count;
                            if (countAfter != countBefore)
                            {
                                throw new Exception();
                            }

                            // also to the inter-improvements.
                            currentRoute = this.Improve(problem, solution, max, solution.Count - 1);
                        }
                    }
                    else
                    {// ok we are done!
                        this.Improve(problem, solution, max, solution.Count - 1);

                        // break the route.
                        break;
                    }
                }
            }

            // remove empty routes.
            for (int routeIdx = solution.Count - 1; routeIdx >= 0; routeIdx--)
            {
                if (solution.Route(routeIdx).IsEmpty)
                {
                    solution.Remove(routeIdx);
                }
            }

            return solution;
        }

        private IRoute Improve(MaxTimeProblem problem,
            MaxTimeSolution solution, double max, int currentRouteIdx)
        {
            // the current route.
            IRoute currentRoute = solution.Route(currentRouteIdx);

            for (int routeIdx = 0; routeIdx < solution.Count; routeIdx++)
            { // apply the intra-route heurstic between the new and all existing routes.
                if (routeIdx != currentRouteIdx &&
                    this.Overlaps(problem, solution.Route(routeIdx), solution.Route(currentRouteIdx)))
                { // only check routes that overlap.
                    if (this.ImproveInterRoute(problem, solution, routeIdx, currentRouteIdx, max))
                    { // an improvement was found, again to the intra operators.
                        if (!solution.IsValid())
                        {
                            throw new Exception();
                        }

                        solution[currentRouteIdx] = this.ImproveIntraRoute(problem, currentRoute, solution[currentRouteIdx]);
                        solution[routeIdx] = this.ImproveIntraRoute(problem, solution.Route(routeIdx), solution[routeIdx]);

                        // recalculate weights.
                        solution[currentRouteIdx] = problem.Time(solution.Route(currentRouteIdx));
                        solution[routeIdx] = problem.Time(solution.Route(routeIdx));
                    }
                }
            }

            return solution.Route(solution.Count - 1);
        }

        #region Seed Selection Heurstic

        /// <summary>
        /// Selects a new seed customer.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="calculator"></param>
        /// <param name="solution"></param>
        /// <param name="customers"></param>
        /// <returns></returns>
        private int SelectSeed(MaxTimeProblem problem, MaxTimeCalculator calculator,
            MaxTimeSolution solution, List<int> customers)
        { // select the customer farthest from the depot.
            int selectedCustomer = -1;
            double maxDistance = double.MinValue;
            foreach (int customerToCheck in customers)
            {
                double distance = problem.WeightMatrix[0][customerToCheck] +
                    problem.WeightMatrix[customerToCheck][0];
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    selectedCustomer = customerToCheck;
                }
            }
            return selectedCustomer;
        }

        #endregion

        #region Improvement Heurstic

        /// <summary>
        /// Apply some improvements within one route.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="currentWeight"></param>
        private double ImproveIntraRoute(IProblemWeights problem, IRoute route, double currentWeight)
        {
            bool improvement = true;
            double newWeight = currentWeight;
            while (improvement)
            { // keep trying while there are still improvements.
                improvement = false;

                // loop over all improvement operations.
                foreach (IImprovement improvementOperation in _intraImprovements)
                { // try the current improvement operations.
                    double difference;
                    if (improvementOperation.Improve(problem, route, out difference))
                    { // there was an improvement.
                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Routing.VRP.WithDepot.MaxTime.CheapestInsertion.CheapestInsertionSolver", System.Diagnostics.TraceEventType.Information,
                            "Intra-improvement found {0} {1}->{2}",
                            improvementOperation.Name, newWeight, newWeight + difference);

                        // check if the route is valid.
                        if (!route.IsValid())
                        {
                            throw new Exception();
                        }

                        // update the weight.
                        newWeight = newWeight + difference;

                        improvement = true;

                        break;
                    }
                }
            }
            return newWeight;
        }

        /// <summary>
        /// Apply some improvements between the given routes and returns the resulting weight.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="solution"></param>
        /// <param name="route1Idx"></param>
        /// <param name="route2Idx"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private bool ImproveInterRoute(MaxTimeProblem problem, MaxTimeSolution solution, 
            int route1Idx, int route2Idx, double max)
        {
            // get the routes.
            IRoute route1 = solution.Route(route1Idx);
            IRoute route2 = solution.Route(route2Idx);

            int countBefore = route1.Count + route2.Count;

            // loop over all improvement operations.
            bool globalImprovement = false;
            foreach (IInterImprovement improvementOperation in _interImprovements)
            { // try the current improvement operations.
                bool improvement = true;
                while (improvement)
                { // keep looping when there is improvement.
                    improvement = false;
                    double totalBefore = problem.Time(solution.Route(route1Idx)) +
                        problem.Time(solution.Route(route2Idx));
                    if (improvementOperation.Improve(problem, solution, route1Idx, route2Idx, max))
                    { // there was an improvement.
                        improvement = true;
                        globalImprovement = true;

                        // check if the route is valid.
                        if (!route1.IsValid())
                        {
                            throw new Exception();
                        }
                        if (!route2.IsValid())
                        {
                            throw new Exception();
                        }

                        int countAfter = route1.Count + route2.Count;
                        if (countBefore != countAfter)
                        {
                            throw new Exception();
                        }

                        double totalAfter = problem.Time(solution.Route(route1Idx)) +
                            problem.Time(solution.Route(route2Idx));
                        if (totalAfter >= totalBefore)
                        {
                            throw new Exception("this is not an improvement!");
                        }
                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Routing.VRP.WithDepot.MaxTime.CheapestInsertion.CheapestInsertionSolver", System.Diagnostics.TraceEventType.Information,
                            "Inter-improvement found {0}<->{1}: {2} ({3}->{4})",
                            route1Idx, route2Idx, improvementOperation.Name, totalBefore, totalAfter);

                        // recalculate weights.
                        solution[route1Idx] = problem.Time(solution.Route(route1Idx));
                        solution[route2Idx] = problem.Time(solution.Route(route2Idx));

                        //break;
                    }
                    else if (!improvementOperation.IsSymmetric &&
                        improvementOperation.Improve(problem, solution, route2Idx, route1Idx, max))
                    { // also do the improvement the other way around when not symmetric.
                        improvement = true;
                        globalImprovement = true;

                        OsmSharp.Logging.Log.TraceEvent("OsmSharp.Routing.VRP.WithDepot.MaxTime.CheapestInsertion.CheapestInsertionSolver", System.Diagnostics.TraceEventType.Information,
                            "Inter-improvement found {0}<->{1}: {2}",
                            route1Idx, route2Idx, improvementOperation.Name);

                        // check if the route is valid.
                        if (!route1.IsValid())
                        {
                            throw new Exception();
                        }
                        if (!route2.IsValid())
                        {
                            throw new Exception();
                        }

                        int countAfter = route1.Count + route2.Count;
                        if (countBefore != countAfter)
                        {
                            throw new Exception();
                        }

                        // recalculate weights.
                        solution[route1Idx] = problem.Time(solution.Route(route1Idx));
                        solution[route2Idx] = problem.Time(solution.Route(route2Idx));

                        //break;
                    }
                }
            }
            return globalImprovement;
        }

        #endregion
    }
}