using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.VRP.MultiSalesman.Genetic.Helpers;
using Tools.Math.VRP.MultiSalesman.Genetic;

namespace Tools.Math.VRP.MultiSalesman.Solver.Operations.Mutation
{
    internal class RedivideToSmallMutationOperation : IMutationOperation<List<Genome>, Problem, Fitness>
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
            //Individual<List<Genome>, Problem, Fitness> copy = mutating.Copy();

            //if (mutating.Genomes.Count > 2)
            //{

            //    // get from the largest round; place in the smallest round.
            //    if (!copy.FitnessCalculated)
            //    {
            //        copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            //    }
            //    Genome smallest = IndividualHelper.GetSmallest(copy);
            //    int smallest_idx = copy.Genomes.IndexOf(smallest);

            //    // build a list of the rest of the cities.
            //    List<int> cities = new List<int>();
            //    for (int city_to_place = 0; city_to_place < solver.Problem.Cities; city_to_place++)
            //    {
            //        if (!smallest.Contains(city_to_place))
            //        {
            //            cities.Add(city_to_place);
            //        }
            //    }

            //    // remove customers until the target time is reached.
            //    double weight = copy.Fitness.Times[smallest_idx];
            //    while (weight < solver.Problem.TargetTime.Value)
            //    {
            //        // best place one of the largest cities into the smallest.
            //        Tools.Math.VRP.MultiSalesman.Genetic.Helpers.BestPlacementHelper.BestPlacementResult result =
            //            BestPlacementHelper.CalculateBestPlacementInGenome(
            //            solver.Problem, (solver.FitnessCalculator as FitnessCalculator), smallest, cities);

            //        // remove from largest/place in smallest after copying.
            //        bool empty = false;
            //        foreach (Genome genome in copy.Genomes)
            //        {
            //            if (genome.Remove(result.City))
            //            {
            //                if (genome.Count == 0)
            //                {
            //                    genome.Add(result.City);
            //                    empty = true;
            //                }
            //                break;
            //            }
            //        }
            //        if (!empty)
            //        {
            //            IndividualHelper.PlaceInGenome(smallest, result.CityIdx, result.City);

            //            // recalculate fitness
            //            copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);

            //            // reset weight.
            //            weight = copy.Fitness.Times[smallest_idx];
            //        }


            //        cities.Remove(result.City);
            //    }
            //}
            //return copy;

            throw new NotImplementedException("Not re-implemented after refactoring GA");
        }
    }
}
