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
using OsmSharp.Tools.Math.VRP.Core.BestPlacement;

namespace OsmSharp.Routing.VRP.WithDepot.MaxTime.InterImprovements
{
    /// <summary>
    /// Tries to relocate customers using the source the same as the CrossExchange but places customers using cheapest insertion.
    /// </summary>
    public class RelocateExchangeInterImprovement : IInterImprovement
    {
        /// <summary>
        /// Returns the name of this improvment.
        /// </summary>
        public string Name
        {
            get
            {
                return "CROSS_REL";
            }
        }

        /// <summary>
        /// Returns true if this operation is symmetric.
        /// </summary>
        public bool IsSymmetric
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to relocate customers using the source the same as the CrossExchange but places customers using cheapest insertion.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="solution"></param>
        /// <param name="route1_idx"></param>
        /// <param name="route2_idx"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public bool Improve(MaxTimeProblem problem, MaxTimeSolution solution,
            int route1_idx, int route2_idx, double max)
        {
            int max_window = 5;

            IRoute route1 = solution.Route(route1_idx);
            IRoute route2 = solution.Route(route2_idx);

            double total_before = problem.Time(solution.Route(route1_idx)) +
                problem.Time(solution.Route(route2_idx));

            int route1_customers = route1.Count;
            int route2_customers = route2.Count;

            double[] route1_cumul = problem.TimeCumul(route1);
            double[] route2_cumul = problem.TimeCumul(route2);

            // build all edge weights.
            List<Edge> route1_edges = new List<Edge>(route1.Edges());
            List<Edge> route2_edges = new List<Edge>(route2.Edges());
            double[] route1_weights = new double[route1_edges.Count];
            for (int idx = 0; idx < route1_edges.Count; idx++)
            {
                Edge edge = route1_edges[idx];
                route1_weights[idx] = problem.WeightMatrix[edge.From][edge.To];
            }
            double[] route2_weights = new double[route2_edges.Count];
            for (int idx = 0; idx < route2_edges.Count; idx++)
            {
                Edge edge = route2_edges[idx];
                route2_weights[idx] = problem.WeightMatrix[edge.From][edge.To];
            }

            List<EdgePair> route2_pairs = new List<EdgePair>();
            for (int i_idx = 0; i_idx < route2_edges.Count - 2; i_idx++)
            {
                Edge i = route2_edges[i_idx];
                double i_weight = route2_weights[i_idx];
                double weight_before_i = route2_cumul[i_idx];

                int k_idx_max = route2_edges.Count;
                if (k_idx_max > i_idx + 2 + max_window)
                {
                    k_idx_max = i_idx + 2 + max_window;
                }
                for (int k_idx = i_idx + 2; k_idx < k_idx_max; k_idx++)
                {
                    Edge k = route2_edges[k_idx];
                    double k_weight = route2_weights[k_idx];
                    double weight_after_k = route2_cumul[route2_cumul.Length - 1] - route2_cumul[k_idx + 1];
                    double weight_between_route = route2_cumul[k_idx] - route2_cumul[i_idx + 1];

                    route2_pairs.Add(new EdgePair()
                    {
                        First = i,
                        FirstWeight = i_weight,
                        Second = k,
                        SecondWeight = k_weight,
                        Between = new List<int>(route2.Between(i.To, k.From)),
                        WeightTotal = i_weight + k_weight,
                        WeightAfter = weight_after_k,
                        WeightBefore = weight_before_i,
                        WeightBetween = weight_between_route,
                        CustomersBetween = k_idx - i_idx
                    });
                }
            }

            // try to relocate all and find best pair.
            double route1_weight = route1_cumul[route1_cumul.Length - 1];
            EdgePair best = null;
            CheapestInsertionResult best_result = new CheapestInsertionResult();
            double best_extra = double.MaxValue;
            foreach (EdgePair pair2 in route2_pairs)
            {
                // calculate cheapest insertion.
                CheapestInsertionResult result =
                    CheapestInsertionHelper.CalculateBestPlacement(problem, route1,
                    pair2.First.To, pair2.Second.From);

                double extra_route2 = problem.WeightMatrix[pair2.First.From][pair2.Second.To];

                // check if the result has a net-decrease.
                if (result.Increase + extra_route2 < pair2.WeightTotal - 0.01)
                { // there is a net decrease.
                    // calculate the real increase.
                    double new_weight = problem.Time(route1_weight + result.Increase + pair2.WeightBetween, route1_customers +
                        pair2.CustomersBetween);

                    // check the max.
                    if (new_weight < max && new_weight < best_extra)
                    { // the route is smaller than max.
                        best_extra = new_weight;
                        best_result = result;
                        best = pair2;
                    }
                }
            }

            if (best != null)
            {
                if (route2.Last == best.Second.To)
                {
                    //throw new Exception();
                }
                route2.ReplaceEdgeFrom(best.First.From, best.Second.To);

                int previous = best_result.CustomerBefore;
                foreach (int customer in best.Between)
                {
                    route1.ReplaceEdgeFrom(previous, customer);

                    previous = customer;
                }
                route1.ReplaceEdgeFrom(previous, best_result.CustomerAfter);

                // check validity.
                if (!route2.IsValid())
                {
                    throw new Exception();
                }
                if (!route1.IsValid())
                {
                    throw new Exception();
                }

                if (route1.Count + route2.Count != route1_customers + route2_customers)
                {
                    throw new Exception();
                }

                double total_after = problem.Time(solution.Route(route1_idx)) +
                    problem.Time(solution.Route(route2_idx));
                if (total_after >= total_before)
                {
                    throw new Exception("this is not an improvement!");
                }

                return true;
            }
            return false;
        }

        private class EdgePair
        {
            public Edge First { get; set; }

            public double FirstWeight { get; set; }

            public Edge Second { get; set; }

            public double SecondWeight { get; set; }

            public List<int> Between { get; set; }

            public double WeightTotal { get; set; }

            public double WeightBefore { get; set; }

            public double WeightAfter { get; set; }

            public double WeightBetween { get; set; }

            public int CustomersBetween { get; set; }
        }
    }
}
