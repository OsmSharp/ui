﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Units.Time;
using Osm.Routing.Core.Route;
using Tools.Math.TSP.Problems;
using Tools.Math.VRP.Core;
using Tools.Math.VRP.Core.Routes;
using Tools.Math.VRP.Core.BestPlacement;
using Tools.Math.VRP.Core.Routes.ASymmetric;
using Osm.Core;
using Tools.Math.VRP.Core.BestPlacement.SeedCustomers;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime.BestPlacement
{
    public class RouterBestPlacementWithSeeds<ResolvedType> : RouterMaxTime<ResolvedType>
        where ResolvedType : ILocationObject
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
        /// <param name="router"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public RouterBestPlacementWithSeeds(IRouter<ResolvedType> router, Second max, Second delivery_time, int k)
            : base(router, max, delivery_time)
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
            float[] weights = new float[seeds.Count];

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
                        CheapestInsertionHelper.CalculateBestPlacement(problem, current_route, selectable_customers);
                    if (result.Customer == result.CustomerAfter)
                    {
                        throw new Exception();
                    }
                    // get the current weight
                    float weight = weights[route_idx];
                    if (weight + result.Increase + calculator.DeliveryTime < problem.Max.Value)
                    { // route will still be inside bounds.
                        if (result.Increase < best_result.Increase)
                        {
                            best_result = result;
                            best_route_idx = route_idx;
                        }
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

                // do the placement if a placement is found without max violation.
                // else do the placement in the above max route.
                CheapestInsertionResult placement_result = new CheapestInsertionResult();
                placement_result.Increase = float.MaxValue;
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
                routes.Route(placement_result_idx).Insert(placement_result.CustomerBefore, placement_result.Customer, placement_result.CustomerAfter);

                if (!routes.IsValid())
                {
                    throw new Exception();
                }
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("[");
            float total_weight = 0;
            for (int idx = 0; idx < routes.Count; idx++)
            {
                //IRoute route = routes.Route(idx);
                IRoute route = routes.Route(idx);
                float weight = calculator.CalculateOneRoute(route);
                builder.Append(" ");
                builder.Append(weight);
                builder.Append(" ");

                total_weight = total_weight + weight;
            }
            builder.Append("]");
            builder.Append(total_weight);
            builder.Append(": ");
            builder.Append(calculator.Calculate(routes));
            Console.WriteLine(builder.ToString());

            return routes;
        }
    }
}