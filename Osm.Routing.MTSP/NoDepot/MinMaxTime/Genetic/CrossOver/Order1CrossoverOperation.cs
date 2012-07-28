using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.CrossOver;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;

namespace Osm.Routing.Core.VRP.NoDepot.MinMaxTime.Genetic.CrossOver
{
    internal class Order1CrossoverOperation :
        ICrossOverOperation<List<Genome>, Problem, Fitness>
    {
        public Individual<List<Genome>, Problem, Fitness> CrossOver(
            Solver<List<Genome>, Problem, Fitness> solver,
            Individual<List<Genome>, Problem, Fitness> parent1,
            Individual<List<Genome>, Problem, Fitness> parent2)
        {
            int i = Tools.Math.Random.StaticRandomGenerator.Get().Generate(parent1.Genomes[0].Customers.Length);
            int j = Tools.Math.Random.StaticRandomGenerator.Get().Generate(parent1.Genomes[0].Customers.Length);

            while (i == j)
            {
                j = Tools.Math.Random.StaticRandomGenerator.Get().Generate(parent1.Genomes[0].Customers.Length);
            }

            if (i > j)
            {
                int k = j;
                j = i;
                i = k;
            }

            int[] offspring = new int[parent1.Genomes[0].Customers.Length];
            HashSet<int> offspring_place = new HashSet<int>();
            for (int idx = i; idx <= j; idx++)
            {
                offspring[idx] = parent1.Genomes[0].Customers[idx];
                offspring_place.Add(offspring[idx]);
            }

            int placement_idx = j + 1;
            for (int idx = j + 1; idx < parent1.Genomes[0].Customers.Length; idx++)
            {
                int customer = parent2.Genomes[0].Customers[idx];
                if (!offspring_place.Contains(customer))
                {
                    offspring[placement_idx] = customer;
                    placement_idx++;
                }
            }
            if (placement_idx == parent1.Genomes[0].Customers.Length)
            {
                placement_idx = 0;

            }
            if (placement_idx != i)
            {
                for (int idx = 0; idx < parent1.Genomes[0].Customers.Length; idx++)
                {
                    int customer = parent2.Genomes[0].Customers[idx]; ;
                    if (!offspring_place.Contains(customer))
                    {
                        offspring[placement_idx] = customer;
                        placement_idx++;
                        if (placement_idx == parent1.Genomes[0].Customers.Length)
                        {
                            placement_idx = 0;
                        }
                        if (placement_idx == i)
                        {
                            break;
                        }
                    }
                }
            }

            Genome genome = new Genome();
            genome.Sizes = parent1.Genomes[0].Sizes.Clone() as int[];
            genome.Customers = offspring;

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
                return "O1";
            }
        }
    }
}
