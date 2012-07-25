using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;

namespace Osm.Routing.Core.VRP.NoDepot.MinMaxTime.Genetic.Mutation
{
    /// <summary>
    /// Mutation operation exchanging a part of some route to a part of another route.
    /// </summary>
    internal class VehicleMutation :
        IMutationOperation<Genome, Problem, Fitness>
    {
        public string Name
        {
            get
            {
                return "VM";
            }
        }

        /// <summary>
        /// Does the mutation.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="mutating"></param>
        /// <returns></returns>
        public Individual<Genome, Problem, Fitness> Mutate(
            Solver<Genome, Problem, Fitness> solver,
            Individual<Genome, Problem, Fitness> mutating)
        {
            Genome genome = mutating.Genomes[0];
            Genome new_genome = null;

            if (genome.Sizes.Length > 1)
            {
                // calculate average.
                int avg = genome.Customers.Length / genome.Sizes.Length;

                int source_route_idx = Tools.Math.Random.StaticRandomGenerator.Get().Generate(genome.Sizes.Length);
                // get a random route.
                //int point_x_source = Tools.Math.Random.StaticRandomGenerator.Get().Generate(genome.Customers.Length);

                //// find the source route.
                //int total = 0;
                //int source_route_idx = -1;
                //for (int idx = 0; idx < genome.Sizes.Length; idx++)
                //{
                //    total = total + genome.Sizes[idx];
                //    if (point_x_source <= total)
                //    {
                //        source_route_idx = idx;
                //        break;
                //    }
                //}

                // get the source count and descide what to do.
                List<int> sizes = new List<int>(genome.Sizes);
                int source_size = genome.Sizes[source_route_idx];
                if (source_size > avg)
                {
                    int size = Tools.Math.Random.StaticRandomGenerator.Get().Generate(source_size / 2);

                    if (size > 0)
                    {
                        sizes[source_route_idx] = sizes[source_route_idx] - size;
                    }
                    sizes.Insert(source_route_idx, size);
                }
                else
                { // route is small; add it to a neighbour route.
                    // select the target route.
                    //int source_or_target = Tools.Math.Random.StaticRandomGenerator.Get().Generate(2);
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

                    if (target_route_idx >= 0 && target_route_idx < sizes.Count)
                    {
                        // merge the two into the target;
                        int size = sizes[source_route_idx];
                        sizes[target_route_idx] = sizes[target_route_idx] + size;
                        sizes.RemoveAt(source_route_idx);
                    }
                }

                // remove all zero's.
                while (sizes.Remove(0))
                {

                }
                new_genome = new Genome();
                new_genome.Sizes = sizes.ToArray();
                new_genome.Customers = genome.Customers.Clone() as int[];
            }
            else
            {
                new_genome = new Genome();
                new_genome.Sizes = genome.Sizes.Clone() as int[];
                new_genome.Customers = genome.Customers.Clone() as int[];
            }

            if (!new_genome.IsValid())
            {
                throw new Exception();
            }

            List<Genome> genomes = new List<Genome>();
            genomes.Add(new_genome);
            Individual<Genome, Problem, Fitness> individual = new Individual<Genome, Problem, Fitness>();
            individual.Initialize(genomes);
            return individual;
        }
    }
}
