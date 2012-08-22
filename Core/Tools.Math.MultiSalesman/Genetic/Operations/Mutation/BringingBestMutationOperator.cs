using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.Random;
using Tools.Math.VRP.MultiSalesman.Genetic.Helpers;
using Tools.Math.VRP.MultiSalesman.Genetic;

namespace Tools.Math.VRP.MultiSalesman.Solver.Operations.Mutation
{
    internal class BringingBestMutationOperator : IMutationOperation<List<Genome>, Problem, Fitness>
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

            //// get from the largest round; place in the smallest round.
            //int smallest_idx = StaticRandomGenerator.Get().Generate(mutating.Genomes.Count);
            //Genome smallest = mutating.Genomes[smallest_idx];

            //// build a list of the rest of the cities.
            //List<int> cities = new List<int>();
            //for (int city_to_place = 0; city_to_place < solver.Problem.Cities; city_to_place++)
            //{
            //    if (!smallest.Contains(city_to_place))
            //    {
            //        cities.Add(city_to_place);
            //    }
            //}

            //// best place one of the largest cities into the smallest.
            //Tools.Math.VRP.MultiSalesman.Genetic.Helpers.BestPlacementHelper.BestPlacementResult result =
            //    BestPlacementHelper.CalculateBestPlacementInGenome(
            //    solver.Problem, (solver.FitnessCalculator as FitnessCalculator), smallest, cities);

            //// remove from largest/place in smallest after copying.
            //Individual<List<Genome>, Problem, Fitness> copy = mutating.Copy();
            //bool empty = false;
            //smallest = copy.Genomes[smallest_idx];
            //foreach (Genome genome in copy.Genomes)
            //{
            //    if (genome.Remove(result.City))
            //    {
            //        if (genome.Count == 0)
            //        {
            //            genome.Add(result.City);
            //            empty = true;
            //        }
            //        break;
            //    }
            //}
            //if (!empty)
            //{
            //    IndividualHelper.PlaceInGenome(smallest,
            //        result.CityIdx, result.City);
            //}

            //// recalculate fitness.
            ////copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);

            //copy.Validate(solver.Problem);

            //return copy;

            throw new NotImplementedException();
        }
    }
}
