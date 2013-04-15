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
using OsmSharp.Tools.Math.Units.Time;
using OsmSharp.Tools.Math.VRP.Core;
using OsmSharp.Tools.Math.VRP.Core.BestPlacement;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;
using OsmSharp.Tools.Math.TSP;
using OsmSharp.Routing;
using OsmSharp.Tools.Math.VRP.Core.BestPlacement.InsertionCosts;
using OsmSharp.Tools.Math.TSP.LocalSearch.HillClimbing3Opt;
using OsmSharp.Routing.VRP.WithDepot.MaxTime.InterImprovements;

namespace OsmSharp.Routing.VRP.WithDepot.MaxTime.CheapestInsertion
{
    /// <summary>
    /// Executes a cheapest insertion procedures with improvements following VND strategy.
    /// </summary>
    /// <typeparam name="ResolvedType"></typeparam>
    public class CheapestInsertionSolverWithImprovements<ResolvedType> : RouterMaxTime
        where ResolvedType : IRouterPoint
    {
        /// <summary>
        /// The amount of customers to place before applying local improvements.
        /// </summary>
        private int _k;

        /// <summary>
        /// The percentage bound of space to leave for future improvements.
        /// </summary>
        private float _delta_percentage;

        /// <summary>
        /// Holds the intra-route improvements;
        /// </summary>
        private List<IImprovement> _intra_improvements;

        /// <summary>
        /// Holds the inter-route improvements.
        /// </summary>
        private List<IInterImprovement> _inter_improvements;

        /// <summary>
        /// Flag to configure seed costs.
        /// </summary>
        private bool _use_seed_cost;

        /// <summary>
        /// The threshold percentage.
        /// </summary>
        private float _threshold_percentage;

        /// <summary>
        /// Creates a new best placement min max no depot vrp router.
        /// </summary>
        /// <param name="router"></param>
        /// <param name="max"></param>
        /// <param name="delivery_time"></param>
        /// <param name="k"></param>
        /// <param name="delta_percentage"></param>
        /// <param name="use_seed_cost"></param>
        /// <param name="threshold_precentage"></param>
        public CheapestInsertionSolverWithImprovements(IRouter<ResolvedType> router,
            Second max, Second delivery_time, int k, float delta_percentage, bool use_seed_cost, 
            float threshold_precentage)
            : base(max, delivery_time)
        {
            _k = k;
            _delta_percentage = delta_percentage;
            _use_seed_cost = use_seed_cost;
            _threshold_percentage = threshold_precentage;

            _intra_improvements = new List<IImprovement>();
            //_intra_improvements.Add(
            //    new OsmSharp.Tools.Math.TSP.ArbitraryInsertion.ArbitraryInsertionSolver());
            _intra_improvements.Add(
                new HillClimbing3OptSolver(true, true));

            _inter_improvements = new List<IInterImprovement>();
            _inter_improvements.Add(
                new ExchangeInterImprovement());
            _inter_improvements.Add(
                new RelocateImprovement());
            //_inter_improvements.Add(
            //    new TwoOptInterImprovement());
            _inter_improvements.Add(
                new RelocateExchangeInterImprovement());
            _inter_improvements.Add(
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
            MaxTimeSolution solution = new MaxTimeSolution(problem.Size);

            // keep placing customer until none are left.
            List<int> customers = new List<int>(problem.Customers);
            customers.RemoveAt(0);

            double max = problem.Max.Value - (problem.Max.Value * _delta_percentage);

            // keep a list of cheapest insertions.
            IInsertionCosts costs = new BinaryHeapInsertionCosts();
            double percentage = _threshold_percentage;
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
                IRoute current_route = solution.Add(customer);
                solution[solution.Count - 1] = 0;

                while (customers.Count > 0)
                {
                    // calculate the best placement.
                    CheapestInsertionResult result;
                    if (_use_seed_cost)
                    { // use the seed cost; the cost to the seed customer.
                        result = CheapestInsertionHelper.CalculateBestPlacement(problem, current_route, customers,
                            customer, 0.7);

                        // calculate the 'real' increase.
                        result.Increase = (problem.WeightMatrix[result.CustomerBefore][result.Customer] +
                            problem.WeightMatrix[result.Customer][result.CustomerAfter]) -
                            problem.WeightMatrix[result.CustomerBefore][result.CustomerAfter];
                    }
                    else
                    { // just use cheapest insertion.
                        result = CheapestInsertionHelper.CalculateBestPlacement(problem, current_route, customers, costs);
                    }

                    // calculate the new weight.
                    solution[solution.Count - 1] = problem.Time(solution.Route(solution.Count - 1));
                    double potential_weight = problem.MaxTimeCalculator.CalculateOneRouteIncrease(solution[solution.Count - 1],
                        result.Increase);
                    // cram as many customers into one route as possible.
                    if (potential_weight < max)
                    {
                        // insert the customer, it is 
                        customers.Remove(result.Customer);
                        current_route.InsertAfter(result.CustomerBefore, result.Customer);

                        // free some memory in the costs list.
                        costs.Remove(result.CustomerBefore, result.CustomerAfter);

                        // update the cost of the route.
                        solution[solution.Count - 1] = potential_weight;

                        // improve if needed.
                        if (((problem.Size - customers.Count) % _k) == 0)
                        { // an improvement is decided.
                            // apply the inter-route improvements.
                            MaxTimeSolution copy = (solution.Clone() as MaxTimeSolution);

                            int count_before = solution.Route(solution.Count - 1).Count;

                            solution[solution.Count - 1] = this.ImproveIntraRoute(problem,
                                solution.Route(solution.Count - 1), solution[solution.Count - 1]);
                            if (!solution.IsValid())
                            {
                                throw new Exception();
                            }
                            int count_after = solution.Route(solution.Count - 1).Count;
                            if (count_after != count_before)
                            {
                                throw new Exception();
                            }

                            // also to the inter-improvements.
                            current_route = this.Improve(problem, solution, max, solution.Count - 1);
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
            for (int route_idx = solution.Count - 1; route_idx >= 0; route_idx--)
            {
                if (solution.Route(route_idx).IsEmpty)
                {
                    solution.Remove(route_idx);
                }
            }

            return solution;
        }

        private IRoute Improve(MaxTimeProblem problem,
            MaxTimeSolution solution, double max, int current_route_idx)
        {
            // the current route.
            IRoute current_route = solution.Route(current_route_idx);

            for (int route_idx = 0; route_idx < solution.Count; route_idx++)
            { // apply the intra-route heurstic between the new and all existing routes.
                if (route_idx != current_route_idx &&
                    this.Overlaps(problem, solution.Route(route_idx), solution.Route(current_route_idx)))
                { // only check routes that overlap.
                    if (this.ImproveInterRoute(problem, solution, route_idx, current_route_idx, max))
                    { // an improvement was found, again to the intra operators.
                        if (!solution.IsValid())
                        {
                            throw new Exception();
                        }

                        solution[current_route_idx] = this.ImproveIntraRoute(problem, current_route, solution[current_route_idx]);
                        solution[route_idx] = this.ImproveIntraRoute(problem, solution.Route(route_idx), solution[route_idx]);

                        // recalculate weights.
                        solution[current_route_idx] = problem.Time(solution.Route(current_route_idx));
                        solution[route_idx] = problem.Time(solution.Route(route_idx));
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
            int selected_customer = -1;
            double max_distance = double.MinValue;
            foreach (int customer_to_check in customers)
            {
                double distance = problem.WeightMatrix[0][customer_to_check] +
                    problem.WeightMatrix[customer_to_check][0];
                if (distance > max_distance)
                {
                    max_distance = distance;
                    selected_customer = customer_to_check;
                }
            }
            return selected_customer;
        }

        #endregion

        #region Improvement Heurstic

        /// <summary>
        /// Apply some improvements within one route.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <param name="current_weight"></param>
        private double ImproveIntraRoute(IProblemWeights problem, IRoute route, double current_weight)
        {
            bool improvement = true;
            double new_weight = current_weight;
            while (improvement)
            { // keep trying while there are still improvements.
                improvement = false;

                // loop over all improvement operations.
                foreach (IImprovement improvement_operation in _intra_improvements)
                { // try the current improvement operations.
                    double difference;
                    if (improvement_operation.Improve(problem, route, out difference))
                    { // there was an improvement.
                        OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Intra-improvement found {0} {1}->{2}",
                            improvement_operation.Name, new_weight, new_weight + difference);

                        // check if the route is valid.
                        if (!route.IsValid())
                        {
                            throw new Exception();
                        }

                        // update the weight.
                        new_weight = new_weight + difference;

                        improvement = true;

                        break;
                    }
                }
            }
            return new_weight;
        }

        /// <summary>
        /// Apply some improvements between the given routes and returns the resulting weight.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="solution"></param>
        /// <param name="route1_idx"></param>
        /// <param name="route2_idx"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private bool ImproveInterRoute(MaxTimeProblem problem, MaxTimeSolution solution, 
            int route1_idx, int route2_idx, double max)
        {
            // get the routes.
            IRoute route1 = solution.Route(route1_idx);
            IRoute route2 = solution.Route(route2_idx);

            int count_before = route1.Count + route2.Count;

            //// get the weights.
            //double route1_weight = solution[route1_idx];
            //double route2_weight = solution[route2_idx];

            // loop over all improvement operations.
            bool global_improvement = false;
            foreach (IInterImprovement improvement_operation in _inter_improvements)
            { // try the current improvement operations.
                bool improvement = true;
                while (improvement)
                { // keep looping when there is improvement.
                    improvement = false;
                    double total_before = problem.Time(solution.Route(route1_idx)) +
                        problem.Time(solution.Route(route2_idx));
                    if (improvement_operation.Improve(problem, solution, route1_idx, route2_idx, max))
                    { // there was an improvement.
                        improvement = true;
                        global_improvement = true;

                        // check if the route is valid.
                        if (!route1.IsValid())
                        {
                            throw new Exception();
                        }
                        if (!route2.IsValid())
                        {
                            throw new Exception();
                        }

                        int count_after = route1.Count + route2.Count;
                        if (count_before != count_after)
                        {
                            throw new Exception();
                        }

                        double total_after = problem.Time(solution.Route(route1_idx)) +
                            problem.Time(solution.Route(route2_idx));
                        if (total_after >= total_before)
                        {
                            throw new Exception("this is not an improvement!");
                        }

                        OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Inter-improvement found {0}<->{1}: {2} ({3}->{4})",
                            route1_idx, route2_idx, improvement_operation.Name, total_before, total_after);

                        // recalculate weights.
                        solution[route1_idx] = problem.Time(solution.Route(route1_idx));
                        solution[route2_idx] = problem.Time(solution.Route(route2_idx));

                        //break;
                    }
                    else if (!improvement_operation.IsSymmetric &&
                        improvement_operation.Improve(problem, solution, route2_idx, route1_idx, max))
                    { // also do the improvement the other way around when not symmetric.
                        improvement = true;
                        global_improvement = true;

                        OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Inter-improvement found {0}<->{1}: {2}",
                            route1_idx, route2_idx, improvement_operation.Name);

                        // check if the route is valid.
                        if (!route1.IsValid())
                        {
                            throw new Exception();
                        }
                        if (!route2.IsValid())
                        {
                            throw new Exception();
                        }

                        int count_after = route1.Count + route2.Count;
                        if (count_before != count_after)
                        {
                            throw new Exception();
                        }

                        // recalculate weights.
                        solution[route1_idx] = problem.Time(solution.Route(route1_idx));
                        solution[route2_idx] = problem.Time(solution.Route(route2_idx));

                        //break;
                    }
                }
            }
            return global_improvement;
        }

        #endregion
    }
}