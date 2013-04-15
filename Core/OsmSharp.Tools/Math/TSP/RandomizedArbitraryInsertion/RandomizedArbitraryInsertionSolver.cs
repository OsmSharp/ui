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
using OsmSharp.Tools.Math.TSP.Problems;
using OsmSharp.Tools.Math.VRP.Core.Routes.ASymmetric;
using OsmSharp.Tools.Math.VRP.Core.BestPlacement;
using OsmSharp.Tools.Math.TSP;

namespace OsmSharp.Tools.Math.TSP
{
    /// <summary>
    /// Solver that uses RAI to solve instances of the TSP.
    /// </summary>
    public class RandomizedArbitraryInsertionSolver : SolverBase
    {
        ///// <summary>
        ///// Boolean to stop execution.
        ///// </summary>
        //private bool _stopped = false;

        /// <summary>
        /// The route this solver was initialized with.
        /// </summary>
        private IRoute _route;

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        public RandomizedArbitraryInsertionSolver() { }

        /// <summary>
        /// Creates a new solver.
        /// </summary>
        /// <param name="route"></param>
        public RandomizedArbitraryInsertionSolver(IRoute route)
        {
            _route = route;
        }

        /// <summary>
        /// Retuns the name of this solver.
        /// </summary>
        public override string Name
        {
            get
            {
                return "RAI";
            }
        }

        /// <summary>
        /// Solves the problem.
        /// </summary>
        /// <returns></returns>
        protected override IRoute DoSolve(IProblem problem)
        {
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
                first = -1;
            }

            // convert the problem to another problem with a virtual depot if needed.
            IProblem converted_problem;
            if (!is_round && first < 0)
            { // the end points can both vary.
                converted_problem = problem.AddVirtualDepot();
            }
            else
            { // the problem does not need to be converted.
                converted_problem = problem;
            }

            // do the RAI.
            IRoute route = RandomizedArbitraryInsertionSolver.DoSolve(this, converted_problem, _route);

            // convert the route again if needed.
            if (!is_round && first < 0)
            { // the end points could both vary so a virtual one was added. Remove it again here.
                List<int> route_list = new List<int>(route);
                List<int> new_route_list = new List<int>();
                for (int idx = 0; idx < route_list.Count; idx++)
                { // remove customer zero.
                    if (route_list[idx] > 0)
                    {
                        new_route_list.Add(route_list[idx] - 1);
                    }
                }
                route = DynamicAsymmetricRoute.CreateFrom(new_route_list, false);
            }
            return route;
        }

        
        /// <summary>
        /// Executes the RAI.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="initial_route"></param>
        /// <returns></returns>
        public static IRoute DoSolve(IProblem problem, IRoute initial_route)
        {
            return RandomizedArbitraryInsertionSolver.DoSolve(null, problem, initial_route);
        }

        /// <summary>
        /// Executes the RAI.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="solver"></param>
        /// <param name="initial_route"></param>
        /// <returns></returns>
        private static IRoute DoSolve(RandomizedArbitraryInsertionSolver solver, IProblem problem, IRoute initial_route)
        {
            // initialize a route using best-placement.
            DynamicAsymmetricRoute route = null;
            if (initial_route == null)
            {
                OsmSharp.Tools.Math.TSP.ArbitraryInsertion.ArbitraryInsertionSolver ai_solver =
                    new OsmSharp.Tools.Math.TSP.ArbitraryInsertion.ArbitraryInsertionSolver();
                initial_route = ai_solver.Solve(problem);
            }

            // get/create the dynamic route.
            if (initial_route is DynamicAsymmetricRoute)
            { // initial route is already the correct type.
                route = initial_route as DynamicAsymmetricRoute;
            }
            else
            { // convert the initial route to a route of the correct type.
                route = DynamicAsymmetricRoute.CreateFrom(initial_route);
            }

            // do the Arbitrary Insertion.
            double weight = route.CalculateWeight(problem);

            int try_count = 0;
            while (try_count < (route.Count * route.Count))
            { // keep trying for a given number of times.

                int factor = 2;

                // cut out a part.
                int i = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate((route.Count / factor) - 1) + 1;
                int j = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate((route.Count / factor) - 1) + 1;

                while (i == j)
                {
                    j = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate((route.Count / factor) - 1) + 1;
                }

                if (i > j)
                {
                    int k = j;
                    j = i;
                    i = k;
                }

                // cut out the i->j part.
                int length = j - i;
                if (length > 0)
                {
                    // cut and remove.
                    DynamicAsymmetricRoute.CutResult cut_result = route.CutAndRemove(
                        problem, weight, i, length);

                    // calculate the weight that was removed.
                    double new_weight = cut_result.Weight;
                    List<int> cut_part = cut_result.CutPart;
                    DynamicAsymmetricRoute cut_route = cut_result.Route;

                    // use best placement to re-insert.
                    int c = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(cut_part.Count);
                    while (cut_part.Count > 0)
                    { // loop until it's empty.
                        int customer = cut_part[c];
                        cut_part.RemoveAt(c);

                        // calculate the best placement.
                        CheapestInsertionResult result = CheapestInsertionHelper.CalculateBestPlacement(
                            problem, cut_route, customer);
                        //cut_route.InsertAfterAndRemove(result.CustomerBefore, result.Customer, result.CustomerAfter);
                        cut_route.InsertAfter(result.CustomerBefore, result.Customer);
                        new_weight = new_weight + result.Increase;

                        // choose next random customer.
                        c = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(cut_part.Count);
                    }

                    // descide to keep new route or not.
                    if (weight > new_weight)
                    {
                        route = cut_route;
                        weight = new_weight;

                        //if (this.CanRaiseIntermidiateResult())
                        //{
                        //    this.RaiseIntermidiateResult(route.ToArray<int>(), weight);
                        //}
                    }
                }

                // increase the try count.
                try_count++;
            }

            if (solver != null && solver.CanRaiseIntermidiateResult())
            {
                solver.RaiseIntermidiateResult(route.ToArray<int>());
            }

            return route;
        }

        /// <summary>
        /// Stops the solver.
        /// </summary>
        public override void Stop()
        {
            //_stopped = true;
        }
    }
}
