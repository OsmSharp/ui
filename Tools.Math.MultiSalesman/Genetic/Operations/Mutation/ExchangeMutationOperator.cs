using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.Random;
using Tools.Math.VRP.MultiSalesman.Genetic;

namespace Tools.Math.VRP.MultiSalesman.Solver.Operations.Mutation
{
    internal class ExchangeMutationOperator : IMutationOperation<List<Genome>, Problem, Fitness>
    {
        public string Name
        {
            get
            {
                return "NotSet";
            }
        }

        public Individual<List<Genome>, Problem, Fitness> Mutate(
            Solver<List<Genome>, Problem, Fitness> solver, Individual<List<Genome>, Problem, Fitness> mutating)
        {
            //if (mutating.Genomes.Count < 2)
            //{
            //    return mutating;
            //}

            //// search for the best customer exchange.
            //int round1_idx = StaticRandomGenerator.Get().Generate(mutating.Genomes.Count);
            //int round2_idx = round1_idx;
            //while (round2_idx == round1_idx)
            //{
            //    round2_idx = StaticRandomGenerator.Get().Generate(mutating.Genomes.Count);
            //}

            //// go over all customers and keep the best individual (even the original)
            //int round1_count = mutating.Genomes[round1_idx].Count;
            //int round2_count = mutating.Genomes[round2_idx].Count;
            //Individual<List<Genome>, Problem, Fitness> selected = mutating;
            //if (!selected.FitnessCalculated)
            //{
            //    selected.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            //}
            //for (int city1_idx = 0; city1_idx < round1_count; city1_idx++)
            //{
            //    for (int city2_idx = 0; city2_idx < round2_count; city2_idx++)
            //    {
            //        Individual<List<Genome>, Problem, Fitness> copy = mutating.Copy();

            //        Genome round1 = copy.Genomes[round1_idx];
            //        Genome round2 = copy.Genomes[round2_idx];

            //        int city1 = round1[city1_idx];
            //        int city2 = round2[city2_idx];
            //        round1[city1_idx] = city2;
            //        round2[city2_idx] = city1;

            //        // calculate fitness
            //        copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            //        if (copy.Fitness < selected.Fitness)
            //        {
            //            selected = copy;
            //        }
            //    }
            //}

            //return selected;

            throw new NotImplementedException();
        }
    }
}
