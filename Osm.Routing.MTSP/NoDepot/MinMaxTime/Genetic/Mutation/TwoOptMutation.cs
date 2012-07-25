using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.VRP.Core.BestPlacement;

namespace Osm.Routing.Core.VRP.NoDepot.MinMaxTime.Genetic.Mutation
{
    internal class TwoOptMutation :
        IMutationOperation<Genome, Problem, Fitness>
    {
        public string Name
        {
            get
            {
                return "2Opt";
            }
        }

        public Individual<Genome, Problem, Fitness> Mutate(Solver<Genome, Problem, Fitness> solver,
            Individual<Genome, Problem, Fitness> mutating)
        {
            List<Genome> genomes = new List<Genome>();
            Genome genome = mutating.Genomes[0];

            int source_route_idx = Tools.Math.Random.StaticRandomGenerator.Get().Generate(genome.Sizes.Length);

            // select the target route.
            int source_or_target = Tools.Math.Random.StaticRandomGenerator.Get().Generate(2);
            //int target_route_idx = -1;
            //if (source_or_target > 0)
            //{ // take -1.
            //    target_route_idx = source_route_idx - 1;
            //}
            //else
            //{
            //    target_route_idx = source_route_idx + 1;
            //}
            int target_route_idx = -1;
            do
            {
                target_route_idx = Tools.Math.Random.StaticRandomGenerator.Get().Generate(genome.Sizes.Length);
            } while (source_route_idx == target_route_idx);

            if (target_route_idx >= 0 && target_route_idx < genome.Sizes.Length)
            {
                // select two points.
                int source_x = Tools.Math.Random.StaticRandomGenerator.Get().Generate(genome.Sizes[source_route_idx]);
                int source_size = (genome.Sizes[source_route_idx] - source_x);
                int source_x_absolute = genome.StartOf(source_route_idx) + source_x;
                int source_x_absolute_end = source_x_absolute + source_size;

                int target_x = Tools.Math.Random.StaticRandomGenerator.Get().Generate(genome.Sizes[target_route_idx]);
                int target_size = (genome.Sizes[target_route_idx] - target_x);
                int target_x_absolute = genome.StartOf(target_route_idx) + target_x;
                int target_x_absolute_end = target_x_absolute + (genome.Sizes[target_route_idx] - target_x);

                // get the customers.
                List<int> source = new List<int>();
                for (int idx = source_x_absolute; idx < source_x_absolute_end; idx++)
                {
                    source.Add(genome.Customers[idx]);
                }
                List<int> target = new List<int>();
                for (int idx = target_x_absolute; idx < target_x_absolute_end; idx++)
                {
                    target.Add(genome.Customers[idx]);
                }

                // re-insert them and make the switch.
                int[] customers = new int[genome.Customers.Length];
                int new_idx = 0;
                for (int idx = 0; idx < genome.Customers.Length; idx++)
                {
                    if (idx == target_x_absolute)
                    {
                        foreach (int source_customer in source)
                        {
                            customers[new_idx] = source_customer;

                            new_idx++;
                        }
                    }
                    else if (idx > target_x_absolute && idx < target_x_absolute_end)
                    {
                        // do nothing.
                    }
                    else if (idx == source_x_absolute)
                    {
                        foreach (int target_customer in target)
                        {
                            customers[new_idx] = target_customer;

                            new_idx++;
                        }
                    }
                    else if (idx > source_x_absolute && idx < source_x_absolute_end)
                    {
                        // do nothing.
                    }
                    else
                    {
                        customers[new_idx] = genome.Customers[idx];

                        new_idx++;
                    }
                }

                // create new sizes.
                int[] sizes = genome.Sizes.Clone() as int[];
                sizes[source_route_idx] = sizes[source_route_idx] + (target_size - source_size);
                sizes[target_route_idx] = sizes[target_route_idx] + (source_size - target_size);

                // create new genome.
                Genome new_genome = new Genome();
                new_genome.Sizes = sizes;
                new_genome.Customers = customers;

                if (!new_genome.IsValid())
                {
                    throw new Exception();
                }

                genomes.Add(new_genome);
            }
            else
            { // just keep the old one.
                genomes.Add(genome);
            }
            Individual<Genome, Problem, Fitness> individual = new Individual<Genome, Problem, Fitness>();
            individual.Initialize(genomes);
            return individual;
        }
    }
}
