using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Generation;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Osm.Routing.Core.VRP.NoDepot.MaxTime.BestPlacement;
using Tools.Math.VRP.Core.BestPlacement;
using Tools.Math.VRP.Core;
using Tools.Math.VRP.Core.Routes.ASymmetric;
using Tools.Math.VRP.Core.Routes;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime.Genetic.Generation
{
    /// <summary>
    /// Best-placement generator based on a random first customer for each route.
    /// </summary>
    internal class RandomBestPlacement :
        IGenerationOperation<MaxTimeSolution, MaxTimeProblem, Fitness>
    {
        public string Name
        {
            get
            {
                return "NotSet";
            }
        }

        /// <summary>
        /// Generates individuals based on a random first customer for each route.
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        public Individual<MaxTimeSolution, MaxTimeProblem, Fitness> Generate(
            Solver<MaxTimeSolution, MaxTimeProblem, Fitness> solver)
        {
            MaxTimeProblem problem = solver.Problem;

            // create the solution.
            MaxTimeSolution solution = new MaxTimeSolution(problem.Size, true);

            CheapestInsertionHelper helper = new CheapestInsertionHelper();

            // keep placing customer until none are left.
            List<int> customers = new List<int>(problem.Customers);
            while (customers.Count > 0)
            {
                // select a random customer.
                float weight = 0;
                int customer_idx = Tools.Math.Random.StaticRandomGenerator.Get().Generate(customers.Count);
                int customer = customers[customer_idx];
                customers.RemoveAt(customer_idx);

                // use best placement to generate a route.
                IRoute current_route = solution.Add(customer);
                //Console.WriteLine("Starting new route with {0}", customer);
                while (customers.Count > 0)
                {
                    // calculate the best placement.
                    CheapestInsertionResult result =
                        CheapestInsertionHelper.CalculateBestPlacement(problem, current_route, customers);

                    // calculate the new weight.
                    float potential_weight = result.Increase + weight + 20;
                    // cram as many customers into one route as possible.
                    if (potential_weight < problem.Max.Value)
                    {
                        customers.Remove(result.Customer);
                        current_route.Insert(result.CustomerBefore, result.Customer, result.CustomerAfter);
                        weight = potential_weight;
                    }
                    else
                    {// ok we are done!
                        break;
                    }
                }
            }

            return new Individual<MaxTimeSolution, MaxTimeProblem, Fitness>(solution);
        }
    }
}
