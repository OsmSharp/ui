using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.Units.Time;
using Tools.Math.VRP.Core;
using Tools.Math.VRP.Core.BestPlacement;
using Tools.Math.VRP.Core.Routes;
using Tools.Math.VRP.Core.Routes.ASymmetric;
using Osm.Core;
using Tools.Math.TSP;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime.BestPlacement
{
    public class RouterBestPlacementWithImprovements<ResolvedType> : RouterMaxTime<ResolvedType>
        where ResolvedType : ILocationObject
    {
        /// <summary>
        /// The amount of customers to place before applying local improvements.
        /// </summary>
        private int _k;

        /// <summary>
        /// The percentage bound of space to leave for future improvements.
        /// </summary>
        private float _delta;

        /// <summary>
        /// Holds the local improvements;
        /// </summary>
        private List<IImprovement> _inter_improvements;

        /// <summary>
        /// Creates a new best placement min max no depot vrp router.
        /// </summary>
        /// <param name="router"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public RouterBestPlacementWithImprovements(IRouter<ResolvedType> router,
            Second max, Second delivery_time, int k, float delta)
            :base(router, max, delivery_time)
        {
            _k = k;
            _delta = delta;

            _inter_improvements = new List<IImprovement>();
            _inter_improvements.Add(
                new Tools.Math.TSP.ArbitraryInsertion.ArbitraryInsertionSolver());
        }

        ///// <summary>
        ///// Uses a best placement algorithm to generate routes in a deterministic way.
        ///// </summary>
        ///// <param name="problem"></param>
        ///// <param name="customers"></param>
        ///// <returns></returns>
        //public override int[][] DoCalculation(
        //    MaxTimeProblem problem,
        //    ICollection<int> customers,
        //    Second max)
        //{
        //    MaxTimeCalculator calculator = new MaxTimeCalculator(problem);

        //    MaxTimeSolution routes = this.DoCalculationInternal(
        //        problem, customers, max);

        //    StringBuilder builder = new StringBuilder();
        //    builder.Append("[");
        //    float total_weight = 0;
        //    for (int idx = 0; idx < routes.Count; idx++)
        //    {
        //        //IRoute route = routes.Route(idx);
        //        IRoute route = routes.Route(idx);
        //        float weight = calculator.CalculateOneRoute(route);
        //        builder.Append(" ");
        //        builder.Append(weight);
        //        builder.Append(" ");

        //        total_weight = total_weight + weight;
        //    }
        //    builder.Append("]");
        //    builder.Append(total_weight);
        //    builder.Append(": ");
        //    builder.Append(calculator.Calculate(routes));
        //    Console.WriteLine(builder.ToString());

        //    // convert output.
        //    int[][] solution = new int[routes.Count][];
        //    for (int idx = 0; idx < routes.Count; idx++)
        //    {
        //        IRoute current = routes.Route(idx);
        //        //IRoute current = routes[idx];
        //        List<int> route = new List<int>(current);
        //        if (current.IsRound)
        //        {
        //            route.Add(route[0]);
        //        }
        //        solution[idx] = route.ToArray();
        //    }
        //    return solution;
        //}



        /// <summary>
        /// Executes a solver procedure.
        /// </summary>
        /// <param name="problem"></param>
        /// <returns></returns>
        internal override MaxTimeSolution Solve(MaxTimeProblem problem)
        {
            throw new NotImplementedException();
        }

        public MaxTimeSolution DoCalculationInternal(
            MaxTimeProblem problem,
            ICollection<int> customers_to_place,
            Second max)
        {
            MaxTimeCalculator calculator = new MaxTimeCalculator(problem);

            //List<IRoute> routes = new List<IRoute>();
            double improvement_probalitity = 0.25;

            // create the solution.
            MaxTimeSolution solution = new MaxTimeSolution(problem.Size, true);

            CheapestInsertionHelper helper = new CheapestInsertionHelper();

            // keep placing customer until none are left.
            List<int> customers = new List<int>(customers_to_place);
            while (customers.Count > 0)
            {
                // select a random customer.
                float weight = 0;
                int customer_idx = Tools.Math.Random.StaticRandomGenerator.Get().Generate(customers.Count);
                int customer = customers[customer_idx];
                customers.RemoveAt(customer_idx);

                // use best placement to generate a route.
                //IRoute current_route = new DynamicAsymmetricRoute(0, customer, true);
                IRoute current_route = solution.Add(customer);
                //Console.WriteLine("Starting new route with {0}", customer);
                while (customers.Count > 0)
                {
                    // calculate the best placement.
                    CheapestInsertionResult result =
                        CheapestInsertionHelper.CalculateBestPlacement(problem, current_route, customers);

                    // calculate the new weight.
                    float potential_weight = calculator.CalculateOneRouteIncrease(weight, result.Increase);
                    // cram as many customers into one route as possible.
                    if (potential_weight < max.Value - (max.Value * _delta))
                    {
                        customers.Remove(result.Customer);
                        current_route.Insert(result.CustomerBefore, result.Customer, result.CustomerAfter);
                        weight = potential_weight;

                        // improve if needed.
                        if (improvement_probalitity > Tools.Math.Random.StaticRandomGenerator.Get().Generate(1))
                        { // an improvement is descided.
                            weight = this.ImproveInterRoute(problem, current_route, weight);
                        }
                    }
                    else
                    {// ok we are done!
                        break;
                    }
                }
                //routes.Add(current_route);
            }

            return solution;
        }

        /// <summary>
        /// Apply some improvements within one route.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="routes"></param>
        private float ImproveInterRoute(
            IProblemWeights problem, IRoute route, float current_weight)
        {
            bool improvement = true;
            float new_weight = current_weight;
            while (improvement)
            {
                improvement = false;

                foreach (IImprovement improvement_operation in _inter_improvements)
                {
                    //IRoute copy = (route.Clone() as IRoute);
                    float difference;
                    if (improvement_operation.Improve(problem, route, out difference))
                    {
                        new_weight = current_weight + difference;

                        if (!route.IsValid())
                        {
                            throw new Exception();
                        }
                        break;
                    }
                }
            }
            return new_weight;
        }

        /// <summary>
        /// Apply some improvements between routes.
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="route"></param>
        /// <returns></returns>
        private void ImproveIntraRoute(
            IProblemWeights problem, List<IRoute> routes)
        {
            //// improve the route.
            //Tools.Math.TSP.RandomizedArbitraryInsertionSolver solver = 
            //    new Tools.Math.TSP.RandomizedArbitraryInsertionSolver(route);
            //route = solver.Solve(new Tools.Math.TSP.Problems.MatrixProblem(problem.WeightMatrix, false));

            //weight = this.CalculateWeight(problem, route);
            //return route;
        }

        private float CalculateWeight(
            IProblemWeights problem, IRoute route)
        {
            float weight = 0;
            int first = -1;
            int previous = -1;
            foreach (int customer in route)
            {
                if (first <= 0)
                {
                    first = customer;
                }
                if (previous >= 0)
                {
                    weight = weight + problem.Weight(previous, customer);
                }
                previous = customer;
            }
            weight = weight + problem.Weight(previous, first) + route.Count * 20;
            return weight;
        }
    }
}
