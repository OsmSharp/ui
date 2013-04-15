//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using OsmSharp.Tools.Math.VRP.Core;
//using OsmSharp.Tools.Math.VRP.Core.Routes;

//namespace OsmSharp.Routing.VRP.NoDepot.MaxTime.InterImprovements
//{
//    /// <summary>
//    /// Applies the inter-improvements by exchanging the last part of the routes.
//    /// </summary>
//    public class TwoOptInterImprovement : IInterImprovement
//    {
//        /// <summary>
//        /// Return the name of this improvement.
//        /// </summary>
//        public string Name
//        {
//            get
//            {
//                return "2_OPT";
//            }
//        }

//        /// <summary>
//        /// Applies the inter-improvements by exchanging the last part of the routes.
//        /// </summary>
//        /// <param name="problem"></param>
//        /// <param name="route1"></param>
//        /// <param name="route2"></param>
//        /// <param name="difference"></param>
//        /// <returns></returns>
//        public bool Improve(MaxTimeProblem problem, MaxTimeSolution solution,
//            int route1_idx, int route2_idx, double max)
//        {
//            IRoute route1 = solution.Route(route1_idx);
//            IRoute route2 = solution.Route(route2_idx);

//            // pre-calculate all edge-weights and store cumulatively.
//            double[] route1_cumul_weights = problem.MaxTimeCalculator.CalculateCumulWeights(route1);
//            double[] route2_cumul_weights = problem.MaxTimeCalculator.CalculateCumulWeights(route2);

//            // try and exchange the last part of the route.
//            int route1_customer_idx = 0;
//            foreach (int customer1 in route1)
//            {
//                // get the next customer
//                int customer1_next = route1.GetNeigbours(customer1)[0];

//                // don't consider changing the last edge: this would imply changing the entire route.
//                if (customer1_next == route1.First)
//                { // the last customer has been considered.
//                    break;
//                }

//                // calculate all the weights possible about route1.
//                double route1_before = route1_cumul_weights[route1_customer_idx];
//                double route1_after = route1_cumul_weights[route1_cumul_weights.Length - 1] - route1_cumul_weights[route1_customer_idx + 1];
//                double route1_lost_edge = route1_cumul_weights[route1_customer_idx + 1] - route1_cumul_weights[route1_customer_idx];

//                // loop over all customers of route2.
//                int route2_customer_idx = 0;
//                foreach (int customer2 in route2)
//                {
//                    // get the next customer
//                    int customer2_next = route2.GetNeigbours(customer2)[0];

//                    // don't consider changing the last edge: this would imply changing the entire route.
//                    if (customer2_next == route2.First)
//                    { // the last customer has been considered.
//                        break;
//                    }

//                    // calculate all the weights possible about route2.
//                    double route2_before = route2_cumul_weights[route2_customer_idx];
//                    double route2_after = route2_cumul_weights[route2_cumul_weights.Length - 1] - route2_cumul_weights[route2_customer_idx + 1];
//                    double route2_lost_edge = route2_cumul_weights[route2_customer_idx + 1] - route2_cumul_weights[route2_customer_idx];

//                    // calculate the weights of the new edges.
//                    double route1_new_edge = problem.MaxTimeCalculator.CalculateOneRouteIncrease(0,
//                        problem.WeightMatrix[customer1][customer2_next]);
//                    double route2_new_edge = problem.MaxTimeCalculator.CalculateOneRouteIncrease(0,
//                        problem.WeightMatrix[customer2][customer1_next]);

//                    // see if there is a decrease in total weight.
//                    if (((route1_new_edge + route2_new_edge) - (route2_lost_edge + route1_lost_edge)) > 0.001)
//                    { // yes there is a descrease!
//                        // and if the routes are still inside the max bounds.
//                        double weight_route1 = route1_before + route1_new_edge + route2_after;
//                        double weight_route2 = route2_before + route2_new_edge + route1_after;

//                        // test bounds!
//                        if (weight_route1 < max && weight_route2 < max)
//                        { // yes! there is an improvement!
//                            if (customer1 == customer2_next || customer2 == customer1_next)
//                            {
//                                throw new Exception();
//                            }
//                            route1.ReplaceEdgeFrom(customer1, customer2_next);
//                            route2.ReplaceEdgeFrom(customer2, customer1_next);

//                            // check if the route is valid.
//                            if (!route1.IsValid())
//                            {
//                                throw new Exception();
//                            }
//                            if (!route2.IsValid())
//                            {
//                                throw new Exception();
//                            }

//                            return true;
//                        }
//                    }
//                    route2_customer_idx++;
//                }

//                route1_customer_idx++;
//            }
//            return false;
//        }
//    }
//}
