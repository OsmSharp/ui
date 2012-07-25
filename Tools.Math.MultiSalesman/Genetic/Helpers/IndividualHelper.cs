using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic;
using Tools.Math.Random;

namespace Tools.Math.VRP.MultiSalesman.Genetic.Helpers
{
    public class IndividualHelper
    {
        internal static Genome GetSmallest(Individual<Genome, Problem, Fitness> indivdual)
        {
            double weight = double.MaxValue;
            Genome genome = null;
            for (int idx = 0; idx < indivdual.Genomes.Count; idx++)
            {
                double current_weight = indivdual.Fitness.Times[idx];
                if (weight > current_weight)
                {
                    genome = indivdual.Genomes[idx];
                    weight = current_weight;
                }
            }
            return genome;
        }

        internal static Genome GetLargest(Individual<Genome, Problem, Fitness> indivdual)
        {
            double weight = double.MinValue;
            Genome genome = null;
            for (int idx = 0; idx < indivdual.Genomes.Count; idx++)
            {
                double current_weight = indivdual.Fitness.Times[idx];
                if (weight < current_weight)
                {
                    genome = indivdual.Genomes[idx];
                    weight = current_weight;
                }
            }
            return genome;
        }

        internal static Genome SelectRandom(Individual<Genome, Problem, Fitness> indivdual, bool smallest_first)
        {
            double random_time;
            double current_time;
            if (smallest_first)
            {
                double average = indivdual.Fitness.TotalTime / indivdual.Genomes.Count;

                random_time = StaticRandomGenerator.Get().Generate(
                    indivdual.Fitness.TotalTime);
                current_time = 0;
                for (int idx = 0; idx < indivdual.Genomes.Count; idx++)
                {
                    double time = indivdual.Fitness.Times[idx];
                    if (time < average)
                    { // small round: make big
                        time = time + (average - time);
                    }
                    else
                    { // large round: make small
                        time = time - (time - average);
                    }
                    current_time = current_time + time;
                    if (current_time > random_time)
                    {
                        return indivdual.Genomes[idx];
                    }
                }
            }
            else
            {
                random_time = StaticRandomGenerator.Get().Generate(
                    indivdual.Fitness.TotalTime);
                current_time = 0;
                for (int idx = 0; idx < indivdual.Genomes.Count; idx++)
                {
                    current_time = current_time + indivdual.Fitness.Times[idx];
                    if (current_time > random_time)
                    {
                        return indivdual.Genomes[idx];
                    }
                }
            }
            throw new Exception("At least one genome should be found!");
        }

        internal static List<Genome> Except(List<Genome> genomes, Genome except)
        {
            List<Genome> selected = new List<Genome>();
            foreach (Genome selected_genome in genomes)
            {
                if (selected_genome != except)
                {
                    selected.Add(selected_genome);
                }
            }
            return selected;
        }

        internal static void PlaceInGenome(Genome smallest, int city_idx, int city)
        {
            if (smallest.Count == city_idx)
            {
                smallest.Add(city);
            }
            else
            {
                smallest.Insert(city_idx, city);
            }
        }

        internal static bool Overlaps(List<Genome> genomes, Genome genome)
        {
            foreach (Genome source_genome in genomes)
            {
                foreach (int city in genome)
                {
                    if (source_genome.Contains(city))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool Overlaps(List<Genome> genomes, int city)
        {
            foreach (Genome source_genome in genomes)
            {
                if (source_genome.Contains(city))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
