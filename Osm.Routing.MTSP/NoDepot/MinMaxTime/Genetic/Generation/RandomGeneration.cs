using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Generation;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.VRP.Core.Routes.ASymmetric;
using Tools.Math.VRP.Core.Routes;
using Tools.Math.VRP.Core.BestPlacement;

namespace Osm.Routing.Core.VRP.NoDepot.MinMaxTime.Genetic.Generation
{
    /// <summary>
    /// Best-placement generator based on a random first customer for each route.
    /// </summary>
    internal class RandomGeneration :
        IGenerationOperation<List<Genome>, Problem, Fitness>
    {
        public string Name
        {
            get
            {
                return "RAN";
            }
        }

        /// <summary>
        /// Generates individuals based on a random first customer for each route.
        /// </summary>
        /// <param name="solver"></param>
        /// <returns></returns>
        public Individual<List<Genome>, Problem, Fitness> Generate(
            Solver<List<Genome>, Problem, Fitness> solver)
        {
            Problem problem = solver.Problem;

            DynamicAsymmetricMultiRoute multi_route = new DynamicAsymmetricMultiRoute(problem.Size, true);

            // create the problem for the genetic algorithm.
            List<int> customers = new List<int>();
            for (int customer = 0; customer < problem.Size; customer++)
            {
                customers.Add(customer);
            }
            //BestPlacementHelper helper = new BestPlacementHelper();

            // keep placing customer until none are left.
            while (customers.Count > 0)
            {
                // select a random customer.
                float weight = 0;
                int customer_idx = Tools.Math.Random.StaticRandomGenerator.Get().Generate(customers.Count);
                int customer = customers[customer_idx];
                customers.RemoveAt(customer_idx);

                // use best placement to generate a route.
                IRoute current_route = multi_route.Add(customer);
                //Console.WriteLine("Starting new route with {0}", customer);
                while (customers.Count > 0)
                {
                    DynamicAsymmetricMultiRoute backup_clone = multi_route.Clone() as DynamicAsymmetricMultiRoute;

                    // calculate the best placement.
                    int customer_to_place = customers[Tools.Math.Random.StaticRandomGenerator.Get().Generate(customers.Count)];
                    BestPlacementResult result = BestPlacementHelper.CalculateBestPlacement(problem, current_route, customer_to_place);

                    // calculate the new weight.
                    float potential_weight = result.Increase + weight;
                    // cram as many customers into one route as possible.
                    if (potential_weight > problem.Min.Value)
                    { // ok we are done!
                        break;
                    }
                    else
                    {
                        //Console.WriteLine("Inserting {0} between {1} and {2}!", result.Customer, result.CustomerBefore, result.CustomerAfter);

                        customers.Remove(result.Customer);
                        current_route.Insert(result.CustomerBefore, result.Customer, result.CustomerAfter);
                        weight = potential_weight;
                    }


                    if (!multi_route.IsValid())
                    {
                        throw new Exception();
                    }
                }
            }

            if (!multi_route.IsValid())
            {
                throw new Exception();
            }

            List<Genome> genomes = new List<Genome>();
            genomes.Add(Genome.CreateFrom(multi_route));
            Individual<List<Genome>, Problem, Fitness> individual = new Individual<List<Genome>, Problem, Fitness>(genomes);
            //individual.Initialize();
            return individual;
        }
    }
}
