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
using OsmSharp.Tools.Math.TSP.Problems;
using OsmSharp.Tools.Math.VRP.Core;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.VRP.Core.BestPlacement;
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;
using OsmSharp.Tools.Math.VRP.Core.BestPlacement.SeedCustomers;
using OsmSharp.Routing;
using OsmSharp.Tools.Math.VRP.Core.BestPlacement.InsertionCosts;

namespace OsmSharp.Routing.VRP.NoDepot.MaxTime.CheapestInsertion
{
    /// <summary>
    /// Solves the max time problem using seeded cheapest insertion.
    /// </summary>
    public class CheapestInsertionSolverWithSeeds : RouterMaxTime
    {
        /// <summary>
        /// Holds the seed selector.
        /// </summary>
        private ISeedCustomerSelector _seed_selector;

        /// <summary>
        /// Holds the number of routes.
        /// </summary>
        private int _k;

        /// <summary>
        /// Creates a new best placement min max no depot vrp router.
        /// </summary>
        /// <param name="max"></param>
        /// <param name="delivery_time"></param>
        /// <param name="k"></param>
        public CheapestInsertionSolverWithSeeds(Second max, Second delivery_time, int k)
            : base(max, delivery_time)
        {
            _seed_selector = new SimpleSeeds();

            _k = k;
        }

        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public override string Name
        {
            get
            {
                return "CI_SEED";
            }
        }

        /// <summary>
        /// Executes a solver procedure.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        internal override MaxTimeSolution Solve(MaxTimeProblem problem)
        {
            MaxTimeCalculator calculator = new MaxTimeCalculator(problem);

            // get the seed customers.
            ICollection<int> seeds = _seed_selector.SelectSeeds(
                problem, _k);
            double[] weights = new double[seeds.Count];

            // start the seed routes.
            List<int> selectable_customers = problem.Customers;
            MaxTimeSolution routes = new MaxTimeSolution(
                problem.Size, true);
            foreach (int seed in seeds)
            {
                routes.Add(seed);
                selectable_customers.Remove(seed);
            }

            if (!routes.IsValid())
            {
                throw new Exception();
            }

            // keep a list of cheapest insertions.
            IInsertionCosts costs = new BinaryHeapInsertionCosts();

            // keep looping until all customers have been placed.
            while (selectable_customers.Count > 0)
            {
                // try and place into every route.
                CheapestInsertionResult best_result = new CheapestInsertionResult();
                best_result.Increase = float.MaxValue;
                int best_route_idx = -1;

                CheapestInsertionResult best_result_above_max = new CheapestInsertionResult();
                best_result_above_max.Increase = float.MaxValue;
                int best_route_above_max_idx = -1;

                for (int route_idx = 0; route_idx < routes.Count; route_idx++)
                {
                    IRoute current_route = routes.Route(route_idx);

                    // choose the next customer.
                    CheapestInsertionResult result =
                        CheapestInsertionHelper.CalculateBestPlacement(problem, current_route, selectable_customers, costs);
                    if (result.Customer == result.CustomerAfter)
                    {
                        throw new Exception();
                    }
                    // get the current weight
                    double weight = weights[route_idx];
                    if (result.Increase < best_result.Increase)
                    {
                        if (weight + result.Increase + calculator.DeliveryTime < problem.Max.Value)
                        { // route will still be inside bounds.
                            best_result = result;
                            best_route_idx = route_idx;
                        }
                        else
                        { // route will become above max.
                            if (result.Increase < best_result_above_max.Increase)
                            {
                                best_result_above_max = result;
                                best_route_above_max_idx = route_idx;
                            }
                        }
                    }
                }

                // do the placement if a placement is found without max violation.
                // else do the placement in the above max route.
                CheapestInsertionResult placement_result = new CheapestInsertionResult();
                placement_result.Increase = double.MaxValue;
                int placement_result_idx = -1;
                if (best_route_idx >= 0)
                { // best placement found.
                    placement_result = best_result;
                    placement_result_idx = best_route_idx;
                }
                else
                { // best placement found but only above max.
                    placement_result = best_result_above_max;
                    placement_result_idx = best_route_above_max_idx;
                }

                // do the actual placement.
                weights[placement_result_idx] = calculator.CalculateOneRouteIncrease(
                    weights[placement_result_idx], placement_result.Increase);
                selectable_customers.Remove(placement_result.Customer);
                //routes.Route(placement_result_idx).InsertAfterAndRemove(
                //    placement_result.CustomerBefore, placement_result.Customer, placement_result.CustomerAfter);
                routes.Route(placement_result_idx).InsertAfter(
                    placement_result.CustomerBefore, placement_result.Customer);

                if (!routes.IsValid())
                {
                    throw new Exception();
                }
            }

            return routes;
        }
    }
}
