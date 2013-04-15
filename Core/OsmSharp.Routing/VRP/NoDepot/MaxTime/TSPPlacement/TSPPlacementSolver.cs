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
using OsmSharp.Tools.Math.Units.Time;
using OsmSharp.Routing;

namespace OsmSharp.Routing.VRP.NoDepot.MaxTime.TSPPlacement
{
    /// <summary>
    /// A solver bases on TSP placement.
    /// </summary>
    public class TSPPlacementSolver : RouterMaxTime
    {
        private OsmSharp.Tools.Math.TSP.ISolver _tsp_solver;

        private IRoute _tsp_solution;

        /// <summary>
        /// Creates a new TSP placement solver.
        /// </summary>
        /// <param name="max"></param>
        /// <param name="delivery_time"></param>
        /// <param name="tsp_solver"></param>
        public TSPPlacementSolver(Second max, Second delivery_time,
            OsmSharp.Tools.Math.TSP.ISolver tsp_solver)
            :base(max, delivery_time)
        {
            _tsp_solver = tsp_solver;
        }

        /// <summary>
        /// Creates a new TSP placement solver.
        /// </summary>
        /// <param name="max"></param>
        /// <param name="delivery_time"></param>
        /// <param name="tsp_solution"></param>
        public TSPPlacementSolver(Second max, Second delivery_time,
            IRoute tsp_solution)
            : base(max, delivery_time)
        {
            _tsp_solution = tsp_solution;
        }

        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public override string Name
        {
            get
            {
                return "TSP_PLACE";
            }
        }

        internal override MaxTimeSolution Solve(MaxTimeProblem problem)
        {
            MaxTimeCalculator calculator = new MaxTimeCalculator(
                problem);

            // generate a ATSP solution.
            IRoute tsp_solution = _tsp_solution;
            if (tsp_solution == null)
            {
                tsp_solution = _tsp_solver.Solve(new TSPProblem(problem));
            }

            // generate subtours from this solution.
            MaxTimeSolution solution = new MaxTimeSolution(problem.Size, true);

            // select a random start point.
            int start = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(problem.Size);

            // start the first tour.
            int placed = 0;
            int previous = -1;

            // place the first customer.
            double weight = 0;
            double total_weight = 0;
            IRoute route = solution.Add(start);
            previous = start;

            while (placed < problem.Size)
            {
                // get the next customer from the tsp solution.
                int next = tsp_solution.GetNeigbours(previous)[0];

                // get the weight to the current start.
                double weight_to_next = problem.WeightMatrix[previous][next];
                double weight_to_start = problem.WeightMatrix[next][start];
                total_weight = calculator.CalculateOneRouteIncrease(
                    weight, weight_to_next + weight_to_start);
                weight = calculator.CalculateOneRouteIncrease(
                    weight, weight_to_next);

                if (total_weight > problem.Max.Value)
                { // start a new route.
                    route = solution.Add(next);
                    weight = 0;
                }
                else
                { // just insert the next customer.
                    route.InsertAfter(previous, next);
                    //route.InsertAfterAndRemove(previous, next, -1);
                }

                // set the previous.
                previous = next;
                placed++;
            }

            if (!solution.IsValid())
            {
                throw new Exception();
            }

            StringBuilder builder = new StringBuilder();
            builder.Append("[");
            total_weight = 0;
            for (int idx = 0; idx < solution.Count; idx++)
            {
                //IRoute route = routes.Route(idx);
                route = solution.Route(idx);
                weight = calculator.CalculateOneRoute(route);
                builder.Append(" ");
                builder.Append(weight);
                builder.Append(" ");

                total_weight = total_weight + weight;
            }
            builder.Append("]");
            builder.Append(total_weight);
            builder.Append(": ");
            builder.Append(calculator.Calculate(solution));
            Console.WriteLine(builder.ToString());

            return solution;
        }

        //private int FindNext(MaxTimeProblem problem, MaxTimeCalculator calculator, IRoute tsp, int start)
        //{
        //    int found_customer = start;
        //    int next = -1;
        //    float weight = 0;
        //    int previous = start;
        //    while (next != start)
        //    {
        //        // set the next customer.
        //        next = tsp.GetNeigbours(previous)[0];

        //        // get the weight to the current start.
        //        float weight_to_next = problem.WeightMatrix[previous][next];
        //        float weight_to_start = problem.WeightMatrix[next][start];
        //        float total_weight = calculator.CalculateOneRouteIncrease(
        //            weight, weight_to_next + weight_to_start);
        //        weight = calculator.CalculateOneRouteIncrease(
        //            weight, weight_to_next);

        //        if (total_weight < problem.Max.Value)
        //        {
        //            found_customer = next;
        //        }
        //        if (weight > problem.Max.Value)
        //        {
        //            break;
        //        }
        //    }
        //    return found_customer
        //}
    }
}
