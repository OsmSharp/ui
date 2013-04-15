using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.Units.Time;
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.Routing.VRP.NoDepot.MaxTime.SavingsHeuristics
{
    /// <summary>
    /// A heuristic solver based on a construction heuristic inspired on the Clarke-Wright savings heuristic.
    /// </summary>
    public class SavingsHeuristicSolver : RouterMaxTime
    {
        /// <summary>
        /// Creates a solver based on a construction heuristic.
        /// </summary>
        /// <param name="max"></param>
        /// <param name="delivery_time"></param>
        public SavingsHeuristicSolver(Second max, Second delivery_time)
            :base(max, delivery_time)
        {

        }

        /// <summary>
        /// Returns the name of this solver.
        /// </summary>
        public override string Name
        {
            get 
            { 
                return "SAVINGS"; 
            }
        }

        /// <summary>
        /// Calculates a solution.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        internal override MaxTimeSolution Solve(MaxTimeProblem problem)
        {
            // create the calculator.
            MaxTimeCalculator calculator = new MaxTimeCalculator(problem);

            // create the solution.
            MaxTimeSolution solution = new MaxTimeSolution(problem.Size, true);

            double max = problem.Max.Value;

            // keep placing customer until none are left.
            List<int> customers = new List<int>(problem.Customers);

            // create n routes.
            for (int customer = 0; customer < customers.Count; customer++)
            {
                solution.Add(customer);
                solution[solution.Count - 1] = calculator.CalculateOneRouteIncrease(
                    0, 0);
            }

            // creates a result.
            MergeResult result = new MergeResult();
            result.Weight = double.MaxValue;

            // loop over all route pairs and merge the smallest merge.
            while (result != null)
            { // keep looping until there is no result anymore.
                result = new MergeResult();
                result.Weight = double.MaxValue;

                for (int route1_idx = 1; route1_idx < solution.Count; route1_idx++)
                { // keep looping over all routes.
                    for (int route2_idx = 0; route2_idx < solution.Count; route2_idx++)
                    { // keep looping over all routes.
                        if (route1_idx == route2_idx)
                        { // only consider different routes.
                            break;
                        }

                        // calculate the merge result.
                        MergeResult current_result = this.TryMerge(problem, solution,
                            route1_idx, route2_idx, problem.Max.Value);

                        // evaluate the current result.
                        if (current_result != null && current_result.Weight < result.Weight)
                        { // current result is best.
                            result = current_result;
                        }
                    }
                }

                // evaluate the result.
                if (result.Weight < double.MaxValue)
                { // there is a result; apply it!
                    IRoute source = solution.Route(result.RouteSourceId);
                    IRoute target = solution.Route(result.RouteTargetId);

                    //string source_string = source.ToString();
                    //string target_string = target.ToString();

                    if (target.Count > 1 && target.First == target.GetNeigbours(result.CustomerTargetSource)[0])
                    {
                        //throw new Exception();
                    }

                    // create an enumeration of all customers of source in the correct order.
                    IEnumerable<int> source_between = new List<int>(
                        source.Between(result.CustomerSourceSource, result.CustomerSourceTarget));

                    // insert after the complete source.
                    int previous = result.CustomerTargetSource;
                    int next = target.GetNeigbours(result.CustomerTargetSource)[0];
                    foreach (int source_customer in source_between)
                    {
                        // insert.
                        target.ReplaceEdgeFrom(previous, source_customer);

                        previous = source_customer; // update previous.
                    }
                    target.ReplaceEdgeFrom(previous, next);

                    // remove the source route.
                    solution.Remove(result.RouteSourceId);
                    solution.RemoveWeight(result.RouteTargetId);

                    // calculate the weight of the new route.
                    solution[result.RouteTargetId] = solution[result.RouteTargetId] + result.Weight + 
                        solution[result.RouteSourceId];
                    
                    if (!solution.IsValid())
                    {
                        throw new Exception();
                    }
                }
                else
                { // set the result null.
                    result = null;
                }
            }

            return solution;
        }

        /// <summary>
        /// Try and merge route2 into route1.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="solution"></param>
        /// <param name="route1_idx"></param>
        /// <param name="route2_idx"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private MergeResult TryMerge(MaxTimeProblem problem, MaxTimeSolution solution, 
            int route1_idx, int route2_idx, double max)
        {
            // get the route weights.
            double route1_weight = solution[route1_idx];
            double route2_weight = solution[route2_idx];

            // creates a result.
            MergeResult result = new MergeResult();
            result.Weight = double.MaxValue;

            // get the two routes.
            IRoute route1 = solution.Route(route1_idx);
            IRoute route2 = solution.Route(route2_idx);

            // just first do the case where both routes are of zero length.
            if (route1.Count == 1 && route2.Count == 1)
            { // calculate the increase when joining the two points.
                foreach (int customer1 in route1)
                {
                    foreach (int customer2 in route2)
                    {
                        double difference = problem.WeightMatrix[customer1][customer2] +
                                problem.WeightMatrix[customer2][customer1];
                        double new_route_weight = route1_weight + difference + route2_weight;
                        if (new_route_weight < max)
                        {
                            result.Weight = difference;
                            result.RouteSourceId = route2_idx;
                            result.RouteTargetId = route1_idx;
                            result.CustomerSourceSource = customer2;
                            result.CustomerSourceTarget = customer2;
                            result.CustomerTargetSource = customer1;

                            return result;
                        }
                    }
                }
            }

            foreach (Edge route1_edge in route1.Edges())
            { // loop over all route1 edges.
                // calculate weight of the current edge.
                double route1_edge_weight = problem.WeightMatrix[route1_edge.From][route1_edge.To];
                double route1_edge_without = route1_weight - route1_edge_weight;

                if (route2.Count == 1)
                { // there is only one customer.
                    foreach (int customer2 in route2)
                    {
                        //// calculate weight of the current edge.
                        //double route2_edge_weight = problem.WeightMatrix[route2_edge.From][route2_edge.To];
                        //double route2_edge_without = route2_weight - route2_edge_weight;

                        double new_edges_weight = problem.WeightMatrix[route1_edge.From][customer2] +
                            problem.WeightMatrix[customer2][route1_edge.To];

                        double difference = problem.WeightDifferenceAfterMerge(solution,
                            new_edges_weight - (route1_edge_weight));

                        // check if the max bound is not violated.
                        double new_route_weight = route1_edge_without + difference + route2_weight; // the customer remain the same.
                        if (new_route_weight < max)
                        {
                            // the difference is smaller than the current result.
                            if (difference < result.Weight)
                            {
                                result.Weight = difference;
                                result.RouteSourceId = route2_idx;
                                result.RouteTargetId = route1_idx;
                                result.CustomerSourceSource = customer2;
                                result.CustomerSourceTarget = customer2;
                                result.CustomerTargetSource = route1_edge.From;
                            }
                        }
                    }
                }
                else
                { // there is at least one edge.
                    foreach (Edge route2_edge in route2.Edges())
                    { // loop over all route2 edges.
                        // calculate weight of the current edge.
                        double route2_edge_weight = problem.WeightMatrix[route2_edge.From][route2_edge.To];
                        double route2_edge_without = route2_weight - route2_edge_weight;

                        double new_edges_weight = problem.WeightMatrix[route1_edge.From][route2_edge.To] +
                            problem.WeightMatrix[route2_edge.From][route1_edge.To];

                        double difference = problem.WeightDifferenceAfterMerge(solution,
                            new_edges_weight - (route1_edge_weight + route2_edge_weight));

                        // check if the max bound is not violated.
                        double new_route_weight = route1_edge_weight + route2_edge_without + new_edges_weight; // the customer remain the same.
                        if (new_route_weight < max)
                        {
                            // the difference is smaller than the current result.
                            if (difference < result.Weight)
                            {
                                result.Weight = difference;
                                result.RouteSourceId = route2_idx;
                                result.RouteTargetId = route1_idx;
                                result.CustomerSourceSource = route2_edge.To;
                                result.CustomerSourceTarget = route2_edge.From;
                                result.CustomerTargetSource = route1_edge.From;
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Represents a merge result.
        /// </summary>
        private class MergeResult
        {
            /// <summary>
            /// The decrease in weight.
            /// </summary>
            public double Weight { get; set; }

            /// <summary>
            /// The target route.
            /// </summary>
            public int RouteTargetId { get; set; }

            /// <summary>
            /// The source route.
            /// </summary>
            public int RouteSourceId { get; set; }

            /// <summary>
            /// The customer source in the target route.
            /// </summary>
            public int CustomerTargetSource { get; set; }

            /// <summary>
            /// The customer source in the source route.
            /// </summary>
            public int CustomerSourceSource { get; set; }

            /// <summary>
            /// The customer target in the source route.
            /// </summary>
            public int CustomerSourceTarget { get; set; }
        }
    }
}
