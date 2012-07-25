using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.VRP.MultiSalesman.Genetic;
using Tools.Math.VRP.MultiSalesman.Genetic.Helpers;

namespace Tools.Math.VRP.MultiSalesman.Solver.Operations.Mutation
{
    internal class RedivideFromLargeMutationOperation : IMutationOperation<Genome, Problem, Fitness>
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

            if (mutating.Genomes.Count > 2)
            {
                // get from the largest round; place in the smallest round.
                if (!copy.FitnessCalculated)
                {
                    copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);
                }
                Genome biggest = IndividualHelper.GetLargest(copy);
                int biggest_idx = copy.Genomes.IndexOf(biggest);

                // build a list of the other genomes.
                List<Genome> other_genomes = new List<Genome>();
                foreach (Genome other in copy.Genomes)
                {
                    if (other != biggest)
                    {
                        other_genomes.Add(other);
                    }
                }

                // remove customers until the target time is reached.
                double weight = copy.Fitness.Times[biggest_idx];
                while (weight > solver.Problem.TargetTime.Value)
                {
                    // remove the city from the biggest.
                    if (biggest.Count > 1)
                    {
                        // best place one of the largest cities into the smallest.
                        Tools.Math.VRP.MultiSalesman.Genetic.Helpers.BestPlacementHelper.BestPlacementResult result =
                            BestPlacementHelper.CalculateBestPlacementInGenomes(
                            solver.Problem, (solver.FitnessCalculator as FitnessCalculator), other_genomes, biggest[biggest.Count - 1]);

                        biggest.RemoveAt(biggest.Count - 1);

                        // place the result.
                        IndividualHelper.PlaceInGenome(other_genomes[result.RoundIdx],
                            result.CityIdx,
                            result.City);

                        // recalculate fitness
                        copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);

                        // reset weight.
                        weight = copy.Fitness.Times[biggest_idx];
                    }
                    else
                    {
                        break;
                    }
                }

            }
            return copy;
        }
    }
}
