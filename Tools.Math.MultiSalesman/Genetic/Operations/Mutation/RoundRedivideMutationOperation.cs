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
    internal class RoundRedivideMutationOperation : IMutationOperation<Genome, Problem, Fitness>
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

            if (copy.Genomes.Count <= 1)
            {
                return copy;
            }

            if (!copy.FitnessCalculated)
            {
                copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            }
            // randomly select a small genome.
            Genome smallest1 = IndividualHelper.SelectRandom(copy, true);
            copy.Genomes.Remove(smallest1);

            // use best placement.
            copy = BestPlacementHelper.DoFast(
                solver.Problem,
                solver.FitnessCalculator as FitnessCalculator,
                copy as Individual,
                smallest1);

            copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);

            return copy;
        }
    }
}
