//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Osm.Routing.Core.Resolving;
//using Tools.Math.Units.Time;
//using Osm.Routing.Core.Route;
//using Tools.Math.TSP.Problems;
//using Tools.Math.VRP.Core;
//using Tools.Math.VRP.Core.Routes;
//using Tools.Math.VRP.Core.BestPlacement;
//using Tools.Math.VRP.Core.Routes.ASymmetric;

//namespace Osm.Routing.Core.VRP.NoDepot.MaxTime.BestPlacement
//{
//    public class RouterBestPlacement<ResolvedType> : RouterMaxTime<ResolvedType>
//        where ResolvedType : IResolvedPoint
//    {
//        /// <summary>
//        /// Creates a new best placement min max no depot vrp router.
//        /// </summary>
//        /// <param name="router"></param>
//        /// <param name="min"></param>
//        /// <param name="max"></param>
//        public RouterBestPlacement(IRouter<ResolvedType> router,
//            Second min, Second max)
//            :base(router, min, max)
//        {

//        }

//        /// <summary>
//        /// Uses a best placement algorithm to generate routes in a deterministic way.
//        /// </summary>
//        /// <param name="problem"></param>
//        /// <param name="customers"></param>
//        /// <returns></returns>
//        protected override int[][] DoCalculation(
//            IProblemWeights problem,
//            ICollection<int> customers,
//            Second min,
//            Second max)
//        {
//            List<IRoute> routes = new List<IRoute>();
//            IRoute current_route = null;
//            IEnumerator<int> enumerator = customers.GetEnumerator();
//            float weight = 0;
//            while (enumerator.MoveNext())
//            {
//                // the current customer.
//                int customer = enumerator.Current;

//                // create a new route if needed.
//                current_route = new DynamicAsymmetricRoute(0, customer, true);
//                weight = 0;
//                customers.Remove(customer);

//                while (current_route != null)
//                {
//                    if (customers.Count > 0)
//                    {
//                        // choose the next customer.
//                        BestPlacementResult result =
//                            BestPlacementHelper.CalculateBestPlacement(problem, current_route, customers);
//                        float potential_weight = result.Increase + weight;
//                        if (potential_weight > min.Value && potential_weight < min.Value)
//                        {
//                            customers.Remove(result.Customer);
//                            current_route.Insert(result.CustomerBefore, result.Customer, result.CustomerAfter);
//                            weight = potential_weight;
//                            routes.Add(current_route);

//                            current_route = null; // new route.
//                        }
//                        else if (potential_weight > max.Value)
//                        {
//                            routes.Add(current_route);

//                            current_route = null; // new route.
//                        }
//                        else
//                        {
//                            customers.Remove(result.Customer);
//                            current_route.Insert(result.CustomerBefore, result.Customer, result.CustomerAfter);
//                            weight = potential_weight;
//                        }
//                    }
//                    else
//                    {
//                        routes.Add(current_route);
//                        current_route = null;
//                    }
//                }

//                // get a new enumerator.
//                enumerator = customers.GetEnumerator();
//            }

//            // convert output.
//            int[][] solution = new int[routes.Count][];
//            for (int idx = 0; idx < routes.Count; idx++)
//            {
//                List<int> route = new List<int>(routes[idx]);
//                if (routes[idx].IsRound)
//                {
//                    route.Add(route[0]);
//                }
//                solution[idx] = route.ToArray();

//            }
//            return solution;
//        }
//    }
//}
