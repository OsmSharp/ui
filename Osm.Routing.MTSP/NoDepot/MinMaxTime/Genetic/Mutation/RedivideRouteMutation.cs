using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic;
using Tools.Math.VRP.Core.BestPlacement;
using Tools.Math.VRP.Core.Routes;
using Tools.Math.AI.Genetic.Solvers;

namespace Osm.Routing.Core.VRP.NoDepot.MinMaxTime.Genetic.Mutation
{
    /// <summary>
    /// Mutation operation exchanging a part of some route to a part of another route.
    /// </summary>
    internal class RedivideRouteMutation :
        IMutationOperation<List<Genome>, Problem, Fitness>
    {
        public string Name
        {
            get
            {
                return "RED";
            }
        }

        /// <summary>
        /// Does the mutation.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="mutating"></param>
        /// <returns></returns>
        public Individual<List<Genome>, Problem, Fitness> Mutate(
            Solver<List<Genome>, Problem, Fitness> solver,
            Individual<List<Genome>, Problem, Fitness> mutating)
        {
            // get the route information.
            Genome multi_route = mutating.Genomes[0];
            Genome genome = null;

            if (multi_route.Sizes.Length > 1)
            { 
                // select a route.
                int source_route_idx = Tools.Math.Random.StaticRandomGenerator.Get().Generate(multi_route.Sizes.Length);

                // create a copy of the old genome without the source route.
                int[] sizes = new int[multi_route.Sizes.Length - 1];
                int start_idx = multi_route.StartOf(source_route_idx);

                int new_idx = 0;
                for (int idx = 0; idx < multi_route.Sizes.Length; idx++)
                {
                    if (idx != source_route_idx)
                    {
                        sizes[new_idx] = multi_route.Sizes[idx];
                        new_idx++;
                    }
                }
                List<int> customers = new List<int>();
                for (int idx = 0; idx < multi_route.Customers.Length; idx++)
                {
                    if (idx >= start_idx && idx < start_idx + multi_route.Sizes[source_route_idx])
                    {
                        // this is the route that has been removed; do nothing.
                    }
                    else
                    {
                        customers.Add(multi_route.Customers[idx]);
                    }
                }

                genome = new Genome();
                genome.Sizes = sizes;
                genome.Customers = customers.ToArray();

                // index customers.
                List<int> source_route_customers = new List<int>(multi_route.Route(source_route_idx));

                // execute best placement.
                while (source_route_customers.Count > 0)
                {
                    // get the top customer.
                    int customer = source_route_customers[0];

                    // find a best placement.
                    BestPlacementResult result = new BestPlacementResult();
                    result.Increase = float.MaxValue;
                    int target_idx = -1;
                    for (int idx = 0; idx < genome.Sizes.Length; idx++)
                    {
                        IRoute route = genome.Route(idx);

                        if (genome.Sizes[idx] > 0)
                        {
                            BestPlacementResult current_result =
                                BestPlacementHelper.CalculateBestPlacement(solver.Problem, route, customer);
                            if (current_result.Increase < result.Increase)
                            {
                                target_idx = idx;
                                result = current_result;
                            }
                        }
                    }

                    for (int idx = 0; idx < customers.Count; idx++)
                    {
                        if (customers[idx] == result.CustomerBefore)
                        {
                            if (customers.Count - 1 == idx)
                            {
                                customers.Add(customer);
                            }
                            else
                            {
                                customers.Insert(idx + 1, customer);
                            }
                            break;
                        }
                    }

                    // set the mutated.
                    genome.Sizes[target_idx] = genome.Sizes[target_idx] + 1;
                    genome.Customers = customers.ToArray();

                    source_route_customers.RemoveAt(0);
                }
            }
            else
            {
                genome = multi_route;
            }

            List<Genome> genomes = new List<Genome>();
            if (!genome.IsValid())
            {
                throw new Exception();
            }
            genomes.Add(genome);
            Individual<List<Genome>, Problem, Fitness> individual = new Individual<List<Genome>, Problem, Fitness>(genomes);
            //individual.Initialize();
            return individual;
        }
    }
}
