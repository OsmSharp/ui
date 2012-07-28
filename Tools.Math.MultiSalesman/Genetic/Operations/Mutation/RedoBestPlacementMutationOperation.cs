using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.Random;
using Tools.Math.VRP.MultiSalesman.Genetic;
using Tools.Math.VRP.MultiSalesman.Genetic.Helpers;

namespace Tools.Math.VRP.MultiSalesman.Solver.Operations.Mutation
{
    internal class RedoBestPlacementMutationOperation : IMutationOperation<List<Genome>, Problem, Fitness>
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

            //Individual<List<Genome>, Problem, Fitness> copy = mutating.Copy();
            //// search for the best customer exchange.
            //int round1_idx = StaticRandomGenerator.Get().Generate(copy.Genomes.Count);
            //int round2_idx = round1_idx;
            //while (round2_idx == round1_idx)
            //{
            //    round2_idx = StaticRandomGenerator.Get().Generate(copy.Genomes.Count);
            //}

            //// list all cities.
            //List<int> cities = new List<int>(
            //    copy.Genomes[round1_idx]);
            //cities.AddRange(
            //    copy.Genomes[round2_idx]);

            //// redo best placement.
            //List<Genome> new_rounds = new List<Genome>();
            //// place one random city in each round.
            //IRandomGenerator random = StaticRandomGenerator.Get();
            //for (int round_idx = 0; round_idx < 2; round_idx++)
            //{
            //    // select a random city to place.
            //    int city_idx = random.Generate(cities.Count);
            //    int city = cities[city_idx];
            //    cities.RemoveAt(city_idx);

            //    // create new genome.
            //    Genome genome = new Genome();
            //    genome.Add(city);
            //    new_rounds.Add(genome);
            //}

            //// best-place the rest.            
            //new_rounds = BestPlacementHelper.DoFast(
            //    solver.Problem,
            //    (solver.FitnessCalculator as FitnessCalculator),
            //    new_rounds,
            //    cities);

            //// replace old genomes
            //copy.Genomes[round1_idx] = new_rounds[0];
            //copy.Genomes[round2_idx] = new_rounds[1];

            //copy.Validate(solver.Problem);

            //// return the copied.
            //return copy;

            throw new NotImplementedException();
        }
    }
}
