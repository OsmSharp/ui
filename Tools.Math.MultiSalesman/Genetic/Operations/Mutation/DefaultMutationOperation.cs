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
    internal class DefaultMutationOperation : IMutationOperation<Genome, Problem, Fitness>
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
            // get from the largest round; place in the smallest round.
            Genome smallest = IndividualHelper.GetSmallest(mutating);
            int smallest_idx = mutating.Genomes.IndexOf(smallest);
            Genome largest = IndividualHelper.GetLargest(mutating);
            int largest_idx = mutating.Genomes.IndexOf(largest);

            // best place one of the largest cities into the smallest.
            if (!mutating.FitnessCalculated)
            {
                mutating.CalculateFitness(
                    solver.Problem,
                    solver.FitnessCalculator);
            }
            Tools.Math.VRP.MultiSalesman.Genetic.Helpers.BestPlacementHelper.BestPlacementResult result = 
                BestPlacementHelper.CalculateBestPlacementInGenome(
                solver.Problem, (solver.FitnessCalculator as FitnessCalculator), smallest, largest);

            // remove from largest/place in smallest after copying.
            Individual<Genome, Problem, Fitness> copy = mutating.Copy();
            largest = copy.Genomes[largest_idx];
            largest.Remove(result.City);
            smallest = copy.Genomes[smallest_idx];
            IndividualHelper.PlaceInGenome(smallest,result.CityIdx,result.City);

            // recalculate fitness.
            //copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);

            return copy;
        }
    }
}
