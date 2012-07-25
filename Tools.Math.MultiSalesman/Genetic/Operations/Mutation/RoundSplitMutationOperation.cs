using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.Random;
using Tools.Math.VRP.MultiSalesman.Genetic;
using Tools.Math.VRP.MultiSalesman.Genetic.Helpers;

namespace Tools.Math.VRP.MultiSalesman.Solver.Operations.Mutation
{
    internal class RoundSplitMutationOperation : IMutationOperation<Genome, Problem, Fitness>
    {
        public string Name
        {
            get
            {
                return "NotSet";
            }
        }

        public Individual<Genome, Problem, Fitness> Mutate(
            Solver<Genome, Problem, Fitness> solver, Individual<Genome, Problem, Fitness> mutating)
        {
            Individual<Genome, Problem, Fitness> copy = mutating.Copy();
            if (!copy.FitnessCalculated)
            {
                copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            }

            // randomly select a small genome.
            Genome big = IndividualHelper.SelectRandom(copy, false);

            // make sure there are at least two.
            if (big.Count < 3)
            {
                return copy;
            }

            // remove the old round.
            copy.Genomes.Remove(big);

            // create new rounds.
            Genome new_round1 = new Genome();
            Genome new_round2 = new Genome();
            List<int> cities = big;

            // place two random.
            int city_idx = StaticRandomGenerator.Get().Generate(cities.Count);
            int city = cities[city_idx];
            cities.RemoveAt(city_idx);
            new_round1.Add(city);
            city_idx = StaticRandomGenerator.Get().Generate(cities.Count);
            city = cities[city_idx];
            cities.RemoveAt(city_idx);
            new_round2.Add(city);

            // use best placement.
            List<Genome> genomes = new List<Genome>();
            genomes.Add(new_round1);
            genomes.Add(new_round2);

            BestPlacementHelper.DoFast(
                solver.Problem,
                solver.FitnessCalculator as FitnessCalculator,
                genomes,
                cities);

            // add the new rounds.
            copy.Genomes.AddRange(genomes);

            copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);

            return copy;
        }
    }
}
