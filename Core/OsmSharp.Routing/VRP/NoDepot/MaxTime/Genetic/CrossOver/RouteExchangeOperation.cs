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
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;
using OsmSharp.Tools.Math.AI.Genetic.Operations.Mutations;
using OsmSharp.Tools.Math.AI.Genetic.Operations.CrossOver;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Tools.Math.VRP.Core.BestPlacement;
using OsmSharp.Tools.Collections;

namespace OsmSharp.Routing.VRP.NoDepot.MaxTime.Genetic.CrossOver
{
    internal class RouteExchangeOperation :
        ICrossOverOperation<MaxTimeSolution, MaxTimeProblem, Fitness>
    {
        public Individual<MaxTimeSolution, MaxTimeProblem, Fitness> CrossOver(
            Solver<MaxTimeSolution, MaxTimeProblem, Fitness> solver,
            Individual<MaxTimeSolution, MaxTimeProblem, Fitness> parent1,
            Individual<MaxTimeSolution, MaxTimeProblem, Fitness> parent2)
        {
            MaxTimeCalculator calculator = new MaxTimeCalculator(solver.Problem);

            MaxTimeSolution route1 = parent1.Genomes;
            MaxTimeSolution route2 = parent2.Genomes;

            // get the minimum size of both routes.
            int size = route1.Count;
            if (route2.Count < size)
            {
                size = route2.Count;
            }

            // select a random number of routes.
            HashSet<int> selected_first = new HashSet<int>();
            HashSet<int> selected_second = new HashSet<int>();
            List<IRoute> selected_routes = new List<IRoute>();
            bool first = true;
            while (selected_routes.Count < size)
            {
                // select route.
                int selected_route = -1;
                if (first)
                {
                    selected_route = this.ChooseNextFrom(selected_routes, route1, selected_first);

                    selected_first.Add(selected_route);
                    selected_routes.Add(route1.Route(selected_route));
                }
                else
                {
                    selected_route = this.ChooseNextFrom(selected_routes, route2, selected_second);

                    selected_second.Add(selected_route);
                    selected_routes.Add(route2.Route(selected_route));
                }

                first = !first;
            }

            // generate the new customer genome.
            MaxTimeSolution solution = new MaxTimeSolution(route1.Size, true);

            int previous = -1;
            foreach (IRoute route in selected_routes)
            {
                IRoute current_route = null;
                foreach (int customer in route)
                {
                    MaxTimeSolution copy = (solution.Clone() as MaxTimeSolution);
                    string solution_string = solution.ToString();
                    if (!solution.Contains(customer))
                    {
                        if (current_route == null)
                        { // add the route.
                            current_route = solution.Add(customer);

                            // safe the previous customer.
                            previous = customer;

                            if (!solution.IsValid())
                            {
                                throw new Exception();
                            }
                        }
                        else
                        { // add the customer.
                            string current_route_string = current_route.ToString();
                            //current_route.InsertAfterAndRemove(previous, customer, current_route.First);
                            current_route.InsertAfter(previous, customer);
                            //current_route.InsertAfter(customer, current_route.First);

                            if (!solution.IsValid())
                            {
                                throw new Exception();
                            }

                            // safe the previous customer.
                            previous = customer;
                        }
                    }
                }
            }

            if (!solution.IsValid())
            {
                throw new Exception();
            }

            this.FillRoutes(calculator, route1, solution, solver.Problem);

            if (!solution.IsValid())
            {
                throw new Exception();
            }

            return new Individual<MaxTimeSolution, MaxTimeProblem, Fitness>(solution);
        }

        public string Name
        {
            get
            {
                return "NotSet";
            }
        }

        private int ChooseNextFrom(List<IRoute> selected_routes, MaxTimeSolution solution, HashSet<int> selected)
        {
            int selected_route = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(solution.Count);
            while (selected.Contains(selected_route))
            {
                selected_route = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(solution.Count);
            }
            return selected_route;
        }

        //private int ChooseNextFrom(List<IRoute> selected_routes, MaxTimeSolution solution, HashSet<int> selected)
        //{
        //    int min_overlap = int.MaxValue;
        //    int idx = -1;
        //    for (int route_idx = 0; route_idx < solution.Count; route_idx++)
        //    {
        //        if (!selected.Contains(route_idx))
        //        {
        //            int overlap = this.CalculateOverlap(selected_routes, solution.Route(route_idx));
        //            if (overlap == 0)
        //            {
        //                return route_idx;
        //            }
        //            else if (overlap < min_overlap)
        //            {
        //                min_overlap = overlap;
        //                idx = route_idx;
        //            }
        //        }
        //    }
        //    return idx;
        //}

        private int CalculateOverlap(List<IRoute> selected_routes, IRoute route)
        {
            HashSet<int> not_found_customers = new HashSet<int>(route);
            int count = not_found_customers.Count;
            foreach (IRoute selected_route in selected_routes)
            {
                HashSet<int> old_not_found_customers = new HashSet<int>(not_found_customers);
                foreach (int customer in not_found_customers)
                {
                    if (route.Contains(customer))
                    {
                        old_not_found_customers.Remove(customer);
                    }
                }
                not_found_customers = old_not_found_customers;
            }
            return count - not_found_customers.Count;
        }

        private void FillRoutes(MaxTimeCalculator calculator, MaxTimeSolution route1, MaxTimeSolution solution,
            MaxTimeProblem problem)
        {
            double[] weights = new double[solution.Count];
            
            // insert all non-placed customers in the order of the first route.
            HashSet<int> unplaced = new HashSet<int>();
            for (int route_idx = 0; route_idx < route1.Count; route_idx++)
            {
                IRoute route1_route = route1.Route(route_idx);
                foreach (int customer in route1_route)
                {
                    if (!solution.Contains(customer))
                    {
                        unplaced.Add(customer);
                    }
                }
            }

            for (int idx = 0; idx < solution.Count; idx++)
            {
                IRoute route = solution.Route(idx);
                weights[idx] = calculator.CalculateOneRoute(route);
            }

            // insert all non-placed customers in the order of the first route.
            //for (int route_idx = 0; route_idx < route1.Count; route_idx++)
            //{
            //    IRoute route1_route = route1.Route(route_idx);
            //    foreach (int customer in route1_route)
            //    {
            //        if (!solution.Contains(customer))
            //        {
            while (unplaced.Count > 0)
            {
                int customer = unplaced.First<int>();

                // try reinsertion.
                CheapestInsertionResult result = new CheapestInsertionResult();
                result.Increase = double.MaxValue;
                int target_idx = -1;

                CheapestInsertionResult unlimited_result = new CheapestInsertionResult();
                unlimited_result.Increase = double.MaxValue;
                int unlimited_target_idx = -1;
                for (int idx = 0; idx < solution.Count; idx++)
                {
                    IRoute route = solution.Route(idx);

                    CheapestInsertionResult current_result =
                        CheapestInsertionHelper.CalculateBestPlacement(problem.Weights, route, customer);
                    if (current_result.Increase < result.Increase)
                    {
                        if (weights[idx] + current_result.Increase < problem.Max.Value)
                        {
                            target_idx = idx;
                            result = current_result;

                            if (result.Increase <= 0)
                            {
                                break;
                            }
                        }
                    }
                    if (current_result.Increase < unlimited_result.Increase)
                    {
                        unlimited_target_idx = idx;
                        unlimited_result = current_result;
                    }
                }

                if (target_idx < 0)
                {
                    result = unlimited_result;
                    target_idx = unlimited_target_idx;
                }

                // get the target route and insert.
                IRoute target_route = solution.Route(target_idx);
                weights[target_idx] = weights[target_idx] + result.Increase;
                //target_route.InsertAfterAndRemove(result.CustomerBefore, result.Customer, result.CustomerAfter);
                target_route.InsertAfter(result.CustomerBefore, result.Customer);
                unplaced.Remove(result.Customer);

                ////solution.ToString();
                //if (!solution.IsValid())
                //{
                //    throw new Exception();
                //}
            }
            //    }
            //}
        }
    }
}
