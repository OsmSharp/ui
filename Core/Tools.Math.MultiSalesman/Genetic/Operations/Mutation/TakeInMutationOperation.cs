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
    internal class TakeInMutationOperation : IMutationOperation<List<Genome>, Problem, Fitness>
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

            //if (!copy.FitnessCalculated)
            //{
            //    copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            //}
            //// randomly select a small genome.
            //Genome smallest = IndividualHelper.SelectRandom(copy, true);

            //// build a list of the rest of the cities.
            //SortedList<double, int> cities = new SortedList<double, int>();
            //int city_count = solver.Problem.Cities / 50 + 1;
            //for (int city_to_place = 0; city_to_place < solver.Problem.Cities; city_to_place++)
            //{
            //    if (!smallest.Contains(city_to_place))
            //    {
            //        double weight = double.MaxValue;
            //        foreach (int customer in smallest)
            //        {
            //            double current_weight = solver.Problem.Weight(
            //                city_to_place, customer);
            //            if (current_weight < weight)
            //            {
            //                weight = current_weight;
            //            }
            //        }

            //        cities[weight] = city_to_place;

            //        if (cities.Count > city_count)
            //        {
            //            cities.RemoveAt(cities.Count - 1);
            //        }
            //    }
            //}

            //// best place one of the largest cities into the smallest.
            //Tools.Math.VRP.MultiSalesman.Genetic.Helpers.BestPlacementHelper.BestPlacementResult result =
            //    BestPlacementHelper.CalculateBestPlacementInGenome(
            //    solver.Problem, (solver.FitnessCalculator as FitnessCalculator), smallest, new List<int>(cities.Values));
            //// remove from largest/place in smallest after copying.
            //bool empty = false;
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
            //copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);

            //return copy;
            throw new NotImplementedException();
        }
    }
}
