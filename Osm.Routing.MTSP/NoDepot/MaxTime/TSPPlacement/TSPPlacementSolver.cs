using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Core;
using Tools.Math.VRP.Core.Routes;
using Tools.Math.Units.Time;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime.TSPPlacement
{
    public class TSPPlacementSolver<ResolvedType> : RouterMaxTime<ResolvedType>
        where ResolvedType : ILocationObject
    {
        private Tools.Math.TSP.ISolver _tsp_solver;

        private IRoute _tsp_solution;

        public TSPPlacementSolver(IRouter<ResolvedType> router, Second max, Second delivery_time,
            Tools.Math.TSP.ISolver tsp_solver)
            :base(router, max, delivery_time)
        {
            _tsp_solver = tsp_solver;
        }

        public TSPPlacementSolver(IRouter<ResolvedType> router, Second max, Second delivery_time,
            IRoute tsp_solution)
            : base(router, max, delivery_time)
        {
            _tsp_solution = tsp_solution;
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
            int start = Tools.Math.Random.StaticRandomGenerator.Get().Generate(problem.Size);

            // start the first tour.
            int placed = 0;
            int previous = -1;

            // place the first customer.
            float weight = 0;
            float total_weight = 0;
            IRoute route = solution.Add(start);
            previous = start;

            while (placed < problem.Size)
            {
                // get the next customer from the tsp solution.
                int next = tsp_solution.GetNeigbours(previous)[0];

                // get the weight to the current start.
                float weight_to_next = problem.WeightMatrix[previous][next];
                float weight_to_start = problem.WeightMatrix[next][start];
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
                    route.Insert(previous, next, -1);
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
