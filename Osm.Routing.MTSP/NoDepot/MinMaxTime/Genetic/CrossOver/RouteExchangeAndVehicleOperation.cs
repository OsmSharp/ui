using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.VRP.Core.Routes.ASymmetric;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic.Operations.CrossOver;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.VRP.Core.Routes;

namespace Osm.Routing.Core.VRP.NoDepot.MinMaxTime.Genetic.CrossOver
{
    internal class RouteExchangeAndVehicleOperation :
        ICrossOverOperation<List<Genome>, Problem, Fitness>
    {
        public Individual<List<Genome>, Problem, Fitness> CrossOver(
            Solver<List<Genome>, Problem, Fitness> solver,
            Individual<List<Genome>, Problem, Fitness> parent1,
            Individual<List<Genome>, Problem, Fitness> parent2)
        {
            Genome route1 = parent1.Genomes[0];
            Genome route2 = parent2.Genomes[0];

            // generate a random cross-over point.
            int x_point = Tools.Math.Random.StaticRandomGenerator.Get().Generate(route1.Customers.Length);

            // decide for first or second part to be kept.
            int x_point_descision = Tools.Math.Random.StaticRandomGenerator.Get().Generate(route1.Customers.Length);
            bool keep_first = x_point_descision < x_point;

            // copy everything that needs to be kept from route1.
            int[] customers = new int[route1.Customers.Length];
            HashSet<int> selected = new HashSet<int>();
            if (keep_first)
            {
                for (int idx = 0; idx < x_point; idx++)
                {
                    customers[idx] = route1.Customers[idx];
                    selected.Add(route1.Customers[idx]);
                }

                // copy all remaining customers in the same order as in the orginal array.
                int idx_new = x_point;
                for (int idx = 0; idx < route2.Customers.Length; idx++)
                {
                    int customer = route2.Customers[idx];
                    if (!selected.Contains(customer))
                    {
                        customers[idx_new] = customer;

                        idx_new++;
                    }
                }
            }
            else
            {
                for (int idx = x_point; idx < route1.Customers.Length; idx++)
                {
                    customers[idx] = route1.Customers[idx];
                    selected.Add(route1.Customers[idx]);
                }                
                
                // copy all remaining customers in the same order as in the orginal array.
                int idx_new = 0;
                for (int idx = 0; idx < route2.Customers.Length; idx++)
                {
                    int customer = route2.Customers[idx];
                    if (!selected.Contains(customer))
                    {
                        customers[idx_new] = customer;

                        idx_new++;
                    }
                }
            }

            // split the sizes.
            int size_x_point = Tools.Math.Random.StaticRandomGenerator.Get().Generate(route1.Sizes.Length);
            int[] sizes = new int[route1.Sizes.Length];
            int sizes_idx = 0;
            for (int idx = size_x_point; idx < route1.Sizes.Length; idx++)
            {
                sizes[sizes_idx] = route1.Sizes[idx];
                sizes_idx++;
            }
            for (int idx = 0; idx < size_x_point; idx++)
            {
                sizes[sizes_idx] = route1.Sizes[idx];
                sizes_idx++;
            }

            // create new genome.
            Genome genome = new Genome();
            genome.Sizes = sizes;
            genome.Customers = customers;

            if (!genome.IsValid())
            {
                throw new Exception();
            }
            List<Genome> genomes = new List<Genome>();
            genomes.Add(genome);
            Individual<List<Genome>, Problem, Fitness> individual = new Individual<List<Genome>, Problem, Fitness>(genomes);
            //individual.Initialize();
            return individual;
        }


        public string Name
        {
            get
            {
                return "REX";
            }
        }
    }
}
