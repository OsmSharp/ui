using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.VRP.Core;
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.Routing.VRP.NoDepot.MaxTime.InterImprovements
{
    /// <summary>
    /// Applies inter-improvements by exchanging parts of the route(s).
    /// </summary>
    public class CrossExchangeInterImprovement : IInterImprovement
    {
        /// <summary>
        /// Return the name of this improvement.
        /// </summary>
        public string Name
        {
            get
            {
                return "CROSS";
            }
        }

        /// <summary>
        /// Returns true if this operation is symmetric.
        /// </summary>
        public bool IsSymmetric
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Applies inter-improvements by exchanging parts of the route(s).
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
            int max_window = 50;

            IRoute route1 = solution.Route(route1_idx);
            IRoute route2 = solution.Route(route2_idx);

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

            // build all edge pairs.
            for (int i_idx = 0; i_idx < route1_edges.Count - 2; i_idx++)
            {
                Edge i = route1_edges[i_idx];
                double i_weight = route1_weights[i_idx];
                double weight_before_i = route1_cumul[i_idx];

                int k_idx_max = route1_edges.Count;
                if (k_idx_max > i_idx + 2 + max_window)
                {
                    k_idx_max = i_idx + 2 + max_window;
                }
                for (int k_idx = i_idx + 2; k_idx < k_idx_max; k_idx++)
                {
                    Edge k = route1_edges[k_idx];
                    double k_weight = route1_weights[k_idx];
                    double weight_after_k = route1_cumul[route1_cumul.Length - 1] - route1_cumul[k_idx + 1];
                    double weight_between_route = route1_cumul[k_idx] - route1_cumul[i_idx + 1];

                    EdgePair pair1 = new EdgePair()
                    {
                        First = i,
                        FirstWeight = i_weight,
                        Second = k,
                        SecondWeight = k_weight,
                        Between = new List<int>(route1.Between(i.To, k.From)),
                        WeightTotal = i_weight + k_weight,
                        WeightAfter = weight_after_k,
                        WeightBefore = weight_before_i,
                        WeightBetween = weight_between_route,
                        CustomersBetween = k_idx - i_idx
                    };

                    foreach (EdgePair pair2 in route2_pairs)
                    {
                        double existing_weight = pair1.WeightTotal + pair2.WeightTotal;

                        //double new_weight = 0;

                        // get first route new.
                        double new_weight = problem.WeightMatrix[pair1.First.From][pair2.First.To];
                        //new_weight = first_route1_new;
                        if (new_weight > existing_weight - 0.001)
                        {
                            continue;
                        }

                        double first_route2_new = problem.WeightMatrix[pair2.First.From][pair1.First.To];
                        new_weight = new_weight + first_route2_new;
                        if (new_weight > existing_weight - 0.001)
                        {
                            continue;
                        }

                        double second_route1_new = problem.WeightMatrix[pair1.Second.From][pair2.Second.To];
                        new_weight = new_weight + second_route1_new;
                        if (new_weight > existing_weight - 0.001)
                        {
                            continue;
                        }

                        double second_route2_new = problem.WeightMatrix[pair2.Second.From][pair1.Second.To];
                        new_weight = new_weight + second_route2_new;

                        if (new_weight < existing_weight - 0.001)
                        { // there is a decrease in total weight; check bounds.
                            double route1_weight = pair1.WeightBefore + pair2.WeightBetween + pair1.WeightAfter;
                            double route2_weight = pair2.WeightBefore + pair1.WeightBetween + pair2.WeightAfter;

                            // calculate the maximum.
                            int route1_customers_between = pair1.CustomersBetween;
                            int route2_customers_between = pair1.CustomersBetween;
                            route1_weight = problem.Time(route1_weight, 
                                route1_customers - route1_customers_between + route2_customers_between);
                            route2_weight = problem.Time(route2_weight, 
                                route2_customers - route2_customers_between + route1_customers_between);

                            if (route1_weight < max && route2_weight < max)
                            {
                                MaxTimeSolution solution_copy = solution.Clone() as MaxTimeSolution;

                                List<int> route1_between = pair1.Between;
                                List<int> route2_between = pair2.Between;

                                route1.ReplaceEdgeFrom(pair1.First.From, pair1.Second.To);
                                route2.ReplaceEdgeFrom(pair2.First.From, pair2.Second.To);

                                int previous = pair1.First.From;
                                for (int idx = 0; idx < route2_between.Count; idx++)
                                {
                                    route1.ReplaceEdgeFrom(previous, route2_between[idx]);
                                    previous = route2_between[idx];
                                }
                                route1.ReplaceEdgeFrom(previous, pair1.Second.To);

                                previous = pair2.First.From;
                                for (int idx = 0; idx < route1_between.Count; idx++)
                                {
                                    route2.ReplaceEdgeFrom(previous, route1_between[idx]);
                                    previous = route1_between[idx];
                                }
                                route2.ReplaceEdgeFrom(previous, pair2.Second.To);

                                if (!solution.IsValid())
                                {
                                    throw new Exception();
                                }
                                return true;
                            }
                        }
                    }
                }
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
