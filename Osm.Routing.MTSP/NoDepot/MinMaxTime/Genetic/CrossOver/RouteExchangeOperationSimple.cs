using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.CrossOver;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.VRP.Core.Routes;
using Tools.Math.VRP.Core.BestPlacement;

namespace Osm.Routing.Core.VRP.NoDepot.MinMaxTime.Genetic.CrossOver
{
    internal class RouteExchangeOperationSimple :
        ICrossOverOperation<Genome, Problem, Fitness>
    {
        public string Name
        {
            get
            {
                return "NotSet";
            }
        }

        public Individual<Genome, Problem, Fitness> CrossOver(
            Solver<Genome, Problem, Fitness> solver,
            Individual<Genome, Problem, Fitness> parent1,
            Individual<Genome, Problem, Fitness> parent2)
        {
            Genome route1 = parent1.Genomes[0];
            Genome route2 = parent2.Genomes[0];
            
            // get the minimum size of both routes.
            int size = route1.Sizes.Length;
            if (route2.Sizes.Length < size)
            {
                size = route2.Sizes.Length;
            }

            // select a customer index.
            int selected_route_idx = Tools.Math.Random.StaticRandomGenerator.Get().Generate(
                route1.Sizes.Length - 1) + 1;

            // select routes from source.
            List<int> selected_from_source = new List<int>();
            for (int idx = 0; idx < selected_route_idx; idx++)
            {
                selected_from_source.Add(idx);
            }

            // select routes from target.
            List<int> selected_from_target = new List<int>();
            for (int idx = route2.Sizes.Length - 1 - (size - selected_from_source.Count) + 1;
                idx <= route2.Sizes.Length - 1; idx++)
            {
                selected_from_target.Add(idx);
            }

            // select a random number of routes.
            List<int> customers = new List<int>();
            List<int> sizes = new List<int>();
            HashSet<int> selected_customers = new HashSet<int>();
            int first_descision = Tools.Math.Random.StaticRandomGenerator.Get().Generate(2);
            if (first_descision == 0)
            {
                // insert the first part.
                foreach (int route_idx in selected_from_source)
                {
                    IRoute route = route1.Route(route_idx);
                    int current_size = 0;
                    foreach (int customer in route)
                    {
                        customers.Add(customer);
                        current_size++;
                        selected_customers.Add(customer);
                    }
                    sizes.Add(current_size);
                }

                // drop one random route.
                int route_to_drop = -1;
                //if (selected_from_target.Count > 1)
                //{
                //    int route_to_drop_idx = Tools.Math.Random.StaticRandomGenerator.Get().Generate(selected_from_target.Count);
                //    route_to_drop = selected_from_target[route_to_drop_idx];
                //}

                // insert the second part.
                foreach (int route_idx in selected_from_target)
                {
                    if (route_idx != route_to_drop)
                    {
                        IRoute route = route2.Route(route_idx);
                        int current_size = 0;
                        foreach (int customer in route)
                        {
                            if (!selected_customers.Contains(customer))
                            {
                                customers.Add(customer);
                                current_size++;
                                selected_customers.Add(customer);
                            }
                        }
                        sizes.Add(current_size);
                    }
                }

            }
            else
            {
                // insert the first part.
                foreach (int route_idx in selected_from_target)
                {
                    IRoute route = route2.Route(route_idx);
                    int current_size = 0;
                    foreach (int customer in route)
                    {
                        customers.Add(customer);
                        current_size++;
                        selected_customers.Add(customer);
                    }
                    sizes.Add(current_size);
                }

                // drop one random route.
                int route_to_drop = -1;
                //if (selected_from_source.Count > 1)
                //{
                //    int route_to_drop_idx = Tools.Math.Random.StaticRandomGenerator.Get().Generate(selected_from_source.Count);
                //    route_to_drop = selected_from_source[route_to_drop_idx];
                //}

                // insert the second part.
                List<int> target_sizes = new List<int>();
                List<int> target_customers = new List<int>();
                foreach (int route_idx in selected_from_source)
                {
                    if (route_idx != route_to_drop)
                    {
                        IRoute route = route1.Route(route_idx);
                        int current_size = 0;
                        foreach (int customer in route)
                        {
                            if (!selected_customers.Contains(customer))
                            {
                                target_customers.Add(customer);
                                current_size++;
                                selected_customers.Add(customer);
                            }
                        }
                        target_sizes.Add(current_size);
                    }
                }
                sizes.InsertRange(0, target_sizes);
                customers.InsertRange(0, target_customers);
            }

            // remove size zeros.
            while (sizes.Remove(0))
            {

            }

            Genome genome = new Genome();
            genome.Sizes = sizes.ToArray();
            genome.Customers = customers.ToArray();

            // insert all non-placed customers in the order of the first route.
            if (genome.Customers.Length != route1.Customers.Length)
            {
                foreach (int customer in route1.Customers)
                {
                    if (!selected_customers.Contains(customer))
                    {
                        // try reinsertion.
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

                        // insert the customer.
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
                    }
                }
            }

            if (!genome.IsValid())
            {
                throw new Exception();
            }
            List<Genome> genomes = new List<Genome>();
            genomes.Add(genome);
            Individual<Genome, Problem, Fitness> individual = new Individual<Genome, Problem, Fitness>();
            individual.Initialize(genomes);
            return individual;
        }
    }
}
