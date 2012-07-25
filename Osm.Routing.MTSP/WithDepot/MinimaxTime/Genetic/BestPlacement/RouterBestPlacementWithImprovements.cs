//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Osm.Routing.Core.Resolving;
//using Tools.Math.Units.Time;
//using Tools.Math.VRP.Core;
//using Tools.Math.VRP.Core.BestPlacement;
//using Tools.Math.VRP.Core.Routes;
//using Tools.Math.VRP.Core.Routes.ASymmetric;

//namespace Osm.Routing.Core.VRP.NoDepot.MinMaxTime.BestPlacement
//{
//    public class RouterBestPlacementWithImprovements<ResolvedType> : RouterMinMaxTime<ResolvedType>
//        where ResolvedType : IResolvedPoint
//    {
//        /// <summary>
//        /// The amount of customers to place before applying local improvements.
//        /// </summary>
//        private int _k;

//        /// <summary>
//        /// The percentage bound of space to leave for future improvements.
//        /// </summary>
//        private float _delta;

//        /// <summary>
//        /// Creates a new best placement min max no depot vrp router.
//        /// </summary>
//        /// <param name="router"></param>
//        /// <param name="min"></param>
//        /// <param name="max"></param>
//        public RouterBestPlacementWithImprovements(IRouter<ResolvedType> router,
//            Second min, Second max, int k, float delta)
//            :base(router, min, max)
//        {
//            _k = k;
//            _delta = delta;
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
//            int count = 0;
//            while (enumerator.MoveNext())
//            {
//                // the current customer.
//                int customer = enumerator.Current;
//                count++;

//                // create a new route if needed.
//                current_route = new DynamicAsymmetricRoute(0, customer, true);
//                weight = 0;
//                customers.Remove(customer);

//                while (current_route != null)
//                {
//                    if (count >= _k)
//                    { // the count has reached the threshold.
//                        count = 0;

//                        if (current_route != null)
//                        { // the current route can be improved.
//                            // do the improvements.
//                            float old_weight = this.CalculateWeight(problem, current_route);
//                            this.ImproveInterRoute(problem, current_route);
//                            weight = this.CalculateWeight(problem, current_route);
//                        }
//                    }
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
//                            count++;
//                            weight = potential_weight;
//                            routes.Add(current_route);

//                            current_route = null; // new route.
//                        }
//                        else if (potential_weight > (max.Value - (max.Value * _delta)))
//                        { // the weight is within delta% of the max limit.
//                            routes.Add(current_route);

//                            current_route = null; // new route.
//                        }
//                        else
//                        {
//                            customers.Remove(result.Customer);
//                            current_route.Insert(result.CustomerBefore, result.Customer, result.CustomerAfter);
//                            count++;

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

//        private float CalculateWeight(
//            IProblemWeights problem, IRoute route)
//        {
//            float weight = 0;
//            int first = -1;
//            int previous = -1;
//            foreach (int customer in route)
//            {
//                if (first <= 0)
//                {
//                    first = customer;
//                }
//                if (previous >= 0)
//                {
//                    weight = weight + problem.Weight(previous, customer);
//                }
//                previous = customer;
//            }
//            weight = weight + problem.Weight(previous, first);
//            return weight;
//        }

//        /// <summary>
//        /// Apply a selection of inter-route improvement techniques.
//        /// </summary>
//        /// <param name="problem"></param>
//        /// <param name="route"></param>
//        private void ImproveInterRoute(
//            IProblemWeights problem,
//            IRoute route)
//        {
//            Tools.Math.VRP.Core.BestPlacement.LocalSearch.RePlace1OptHelper.CalculateRePlaceOptHelper(
//                problem, route);
//        }
//    }
//}
