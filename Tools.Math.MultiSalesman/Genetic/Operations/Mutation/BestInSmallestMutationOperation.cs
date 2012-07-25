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
    internal class BestInSmallestMutationOperation : IMutationOperation<Genome, Problem, Fitness>
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
            if (mutating.Genomes.Count < 2)
            {
                return mutating;
            }

            // get from the largest round; place in the smallest round.
            if (!mutating.FitnessCalculated)
            {
                mutating.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            }
            Genome smallest = IndividualHelper.GetSmallest(mutating);
            int smallest_idx = mutating.Genomes.IndexOf(smallest);
            
            // build a list of the rest of the cities.
            List<int> cities = new List<int>();
            for (int city_to_place = 0; city_to_place < solver.Problem.Cities; city_to_place++)
            {
                if (!smallest.Contains(city_to_place))
                {
                    cities.Add(city_to_place);
                }
            }

            // best place one of the largest cities into the smallest.
            Tools.Math.VRP.MultiSalesman.Genetic.Helpers.BestPlacementHelper.BestPlacementResult result =
                BestPlacementHelper.CalculateBestPlacementInGenome(
                solver.Problem, (solver.FitnessCalculator as FitnessCalculator), smallest, cities);

            // remove from largest/place in smallest after copying.
            Individual<Genome, Problem, Fitness> copy = mutating.Copy();
            smallest = copy.Genomes[smallest_idx];
            foreach (Genome genome in copy.Genomes)
            {
                if (genome.Remove(result.City))
                {
                    if (genome.Count == 0)
                    {
                        genome.Add(result.City);
                    }
                    else
                    {
                        IndividualHelper.PlaceInGenome(smallest, result.CityIdx, result.City);
                    }
                    break;
                }
            }

            // recalculate fitness.
            //copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);

            copy.Validate(solver.Problem);

            return copy;
        }
    }
}
