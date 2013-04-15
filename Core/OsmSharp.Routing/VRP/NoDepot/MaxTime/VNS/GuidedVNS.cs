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
using OsmSharp.Tools.Math.VRP.Core;
using OsmSharp.Tools.Math.Units.Time;
using OsmSharp.Tools.Math.VRP.Core.Routes;
using OsmSharp.Routing;
using OsmSharp.Routing.VRP.NoDepot.MaxTime.CheapestInsertion;
using OsmSharp.Tools.Math.TSP;
using OsmSharp.Routing.VRP.NoDepot.MaxTime.InterImprovements;
using OsmSharp.Tools.Math.TSP.LocalSearch.HillClimbing3Opt;
using OsmSharp.Tools.Math.TSP.ArbitraryInsertion;

namespace OsmSharp.Routing.VRP.NoDepot.MaxTime.VNS
{
    /// <summary>
    /// Uses a Variable Neighbourhood Search technique.
    /// </summary>
    public class GuidedVNS : RouterMaxTime
    {
        /// <summary>
        /// Holds the lambda value.
        /// </summary>
        private float _lambda;

        /// <summary>
        /// Holds the sigma value.
        /// </summary>
        private float _sigma;

        /// <summary>
        /// The threshold percentage.
        /// </summary>
        private float _threshold_percentage;

        /// <summary>
        /// Creates a new Guided Variable Neighbourhood Search solver.
        /// </summary>
        /// <param name="router"></param>
        /// <param name="max"></param>
        /// <param name="delivery_time"></param>
        /// <param name="threshold_precentage"></param>
        /// <param name="lambda"></param>
        /// <param name="sigma"></param>
        public GuidedVNS(IRouter<RouterPoint> router, Second max, Second delivery_time, 
            float threshold_precentage, float lambda, float sigma)
            : base(max, delivery_time)
        {
            _threshold_percentage = threshold_precentage;
            _lambda = lambda;
            _sigma = sigma;

            _intra_improvements = new List<IImprovement>();
            //_intra_improvements.Add(
            //    new ArbitraryInsertionSolver());
            _intra_improvements.Add(
                new HillClimbing3OptSolver(true, true));

            _inter_improvements = new List<IInterImprovement>();
            _inter_improvements.Add(
                new RelocateImprovement());
            _inter_improvements.Add(
                new ExchangeInterImprovement());
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
                return "VNS";
            }
        }

        /// <summary>
        /// Executes a solver procedure.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        internal override MaxTimeSolution Solve(MaxTimeProblem problem)
        {
            float lambda = _lambda;

            CheapestInsertionSolverWithImprovements vrp_router =
                new CheapestInsertionSolverWithImprovements(problem.Max.Value, problem.DeliveryTime.Value, 10, 0.10f, true, _threshold_percentage, true, 0.75f);
            MaxTimeSolution original_solution = vrp_router.Solve(
                problem);

            for (int round_x = 0; round_x < original_solution.Count; round_x++)
            { // keep looping on rounds.
                for (int round_y = 0; round_y < round_x; round_y++)
                { // keep looping on rounds with a smaller index not equal to the current round.
                    if (round_x != round_y)
                    { // routes are different.
                        if (this.Overlaps(problem, original_solution.Route(round_x), original_solution.Route(round_y)))
                        { // only check routes that overlap.
                            double tau = double.MinValue;
                            Dictionary<Edge, int> penalizations = new Dictionary<Edge, int>();

                            //bool improvement = true;
                            //while (improvement)
                            //{ // keep looping until no more improvement is found.
                            //    improvement = false;

                            while (true)
                            { // keep trying to improve until the tau limit is exceeded.
                                // calculate the route sizes before.
                                double route1_actual_before = problem.Time(original_solution.Route(round_x));
                                double route2_actual_before = problem.Time(original_solution.Route(round_y));

                                // copy orignal solution.
                                MaxTimeSolution solution = (original_solution.Clone() as MaxTimeSolution);

                                // apply penalties.
                                foreach (KeyValuePair<Edge, int> penalty in penalizations)
                                {
                                    problem.Penalize(penalty.Key, (double)penalty.Value * lambda);
                                }

                                // apply the inter-route improvements.
                                int count_before = solution.Route(round_x).Count + solution.Route(round_y).Count;
                                int count_after;
                                if (this.ImproveInterRoute(problem, solution, round_x, round_y, problem.Max.Value))
                                { // the improve inter route succeeded.
                                    if (!solution.IsValid())
                                    {
                                        throw new Exception();
                                    }
                                    //improvement_inter = true;

                                    count_after = solution.Route(round_x).Count + solution.Route(round_y).Count;
                                    if (count_before != count_after)
                                    {
                                        throw new Exception();
                                    }

                                    // apply the intra-route improvements.
                                    solution[round_x] = this.ImproveIntraRoute(problem, solution.Route(round_x), solution[round_x]);
                                    solution[round_y] = this.ImproveIntraRoute(problem, solution.Route(round_y), solution[round_y]);

                                    // recalculate weights.
                                    solution[round_x] = problem.Time(solution.Route(round_x));
                                    solution[round_y] = problem.Time(solution.Route(round_y));
                                }

                                // check customer counts.
                                count_after = solution.Route(round_x).Count + solution.Route(round_y).Count;
                                if (count_before != count_after)
                                {
                                    throw new Exception();
                                }

                                // undo the penalizations.
                                problem.ResetPenalizations();

                                // check against the orginal objective function.
                                double route1_actual_after = problem.Time(solution.Route(round_x));
                                double route2_actual_after = problem.Time(solution.Route(round_y));
                                if (route1_actual_after + route2_actual_after < route1_actual_before + route2_actual_before - 0.001 &&
                                    route1_actual_after < problem.Max.Value && route2_actual_after < problem.Max.Value)
                                { // there is improvement!
                                    original_solution = solution;

                                    //improvement = true;

                                    OsmSharp.Tools.Output.OutputStreamHost.WriteLine("IMPROVEMENT: {0}->{1}",
                                        route1_actual_before + route2_actual_before, route1_actual_after + route2_actual_after);
                                }

                                // select arc to be penalized.
                                IRoute route1 = original_solution.Route(round_x);
                                IRoute route2 = original_solution.Route(round_y);
                                double u = double.MinValue;
                                Edge penalizing_edge = new Edge();
                                double total_p = 0;
                                foreach (Edge edge in route1.Edges())
                                {
                                    int edge_p;
                                    if (!penalizations.TryGetValue(edge, out edge_p))
                                    {
                                        edge_p = 0;
                                    }
                                    total_p = total_p + edge_p;
                                    double edge_u = ((lambda * (double)edge_p) + problem.WeightMatrix[edge.From][edge.To]) /
                                        ((double)edge_p + 1.0);
                                    if (u <= edge_u)
                                    {
                                        penalizing_edge = edge;
                                        u = edge_u;
                                    }
                                }
                                foreach (Edge edge in route2.Edges())
                                {
                                    int edge_p;
                                    if (!penalizations.TryGetValue(edge, out edge_p))
                                    {
                                        edge_p = 0;
                                    }
                                    total_p = total_p + edge_p;
                                    double edge_u = ((lambda * (double)edge_p) + problem.WeightMatrix[edge.From][edge.To]) /
                                        ((double)edge_p + 1.0);
                                    if (u <= edge_u)
                                    {
                                        penalizing_edge = edge;
                                        u = edge_u;
                                    }
                                }

                                // actually penalize the edge.
                                int p;
                                if (!penalizations.TryGetValue(penalizing_edge, out p))
                                {
                                    p = 1;
                                }
                                else
                                {
                                    p++;
                                }
                                penalizations[penalizing_edge] = p;

                                // evaluate or set tau.
                                if (tau > double.MinValue)
                                { // evaluate if penalizations should end.
                                    if (tau <= lambda * total_p)
                                    { // the penalization should end!
                                        break;
                                    }
                                }
                                else
                                { // first edge being penalized.
                                    tau = lambda * problem.WeightMatrix[penalizing_edge.From][penalizing_edge.To] / 10;
                                }
                            }
                        }
                    }
                }
            }
            return original_solution;
        }

        #region Improvement Heurstics

        /// <summary>
        /// Holds the intra-route improvements;
        /// </summary>
        private List<IImprovement> _intra_improvements;

        /// <summary>
        /// Holds the inter-route improvements.
        /// </summary>
        private List<IInterImprovement> _inter_improvements;

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

        ///// <summary>
        ///// Apply some improvements between the given routes and returns the resulting weight.
        ///// </summary>
        ///// <param name="problem"></param>
        ///// <param name="route"></param>
        ///// <returns></returns>
        //private bool ImproveInterRoute(MaxTimeProblem problem, MaxTimeSolution solution, int route1_idx, int route2_idx, double max)
        //{
        //    // get the routes.
        //    IRoute route1 = solution.Route(route1_idx);
        //    IRoute route2 = solution.Route(route2_idx);

        //    int count_before = route1.Count + route2.Count;

        //    // get the weights.
        //    double route1_weight = solution[route1_idx];
        //    double route2_weight = solution[route2_idx];

        //    // loop over all improvement operations.
        //    bool global_improvement = false;
        //    foreach (IInterImprovement improvement_operation in _inter_improvements)
        //    { // try the current improvement operations.
        //        bool improvement = true;
        //        while (improvement)
        //        { // keep looping when there is improvement.
        //            improvement = false;
        //            if (improvement_operation.Improve(problem, solution, route1_idx, route2_idx, max))
        //            { // there was an improvement.
        //                improvement = true;
        //                global_improvement = true;

        //                OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Inter-improvement found {0}<->{1}: {2}",
        //                    route1_idx, route2_idx, improvement_operation.Name);

        //                // check if the route is valid.
        //                if (!route1.IsValid())
        //                {
        //                    throw new Exception();
        //                }
        //                if (!route2.IsValid())
        //                {
        //                    throw new Exception();
        //                }

        //                int count_after = route1.Count + route2.Count;
        //                if (count_before != count_after)
        //                {
        //                    throw new Exception();
        //                }

        //                // recalculate weights.
        //                solution[route1_idx] = problem.Time(solution.Route(route1_idx));
        //                solution[route2_idx] = problem.Time(solution.Route(route2_idx));

        //                //break;
        //            }
        //        }
        //    }
        //    return global_improvement;
        //}



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
                            route2_idx, route1_idx, improvement_operation.Name, total_before, total_after);

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
