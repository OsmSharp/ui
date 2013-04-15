// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Tools.Math.AI.Genetic.Operations.CrossOver;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Math.Random;
using OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic.Helpers;


namespace OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic.Operations.CrossOver
{
    internal class RandomSelectionCrossOverOperation : ICrossOverOperation<List<Genome>, Problem, Fitness>
    {

        public string Name
        {
            get
            {
                return "NotSet";
            }
        }

        public Individual<List<Genome>, Problem, Fitness> CrossOver(
            Solver<List<Genome>, Problem, Fitness> solver, Individual<List<Genome>, Problem, Fitness> parent1, Individual<List<Genome>, Problem, Fitness> parent2)
        {
            //List<Genome> genomes = null;

            //Individual new_individual = new Individual();

            //List<Genome> parent1_genomes = parent1.Copy().Genomes;
            //List<Genome> parent2_genomes = parent2.Copy().Genomes;

            //genomes = new List<Genome>();
            //while (parent1_genomes.Count > 0 || parent2_genomes.Count > 0)
            //{
            //    // try from parent 1
            //    Genome genome = null;
            //    if (parent1_genomes.Count > 0)
            //    {
            //        genome = parent1_genomes[StaticRandomGenerator.Get().Generate(parent1_genomes.Count)];
            //        if (!IndividualHelper.Overlaps(genomes, genome))
            //        {
            //            genomes.Add(genome);
            //        }
            //        parent1_genomes.Remove(genome);
            //    }
            //    if (parent2_genomes.Count > 0)
            //    {
            //        genome = parent2_genomes[StaticRandomGenerator.Get().Generate(parent2_genomes.Count)];
            //        if (!IndividualHelper.Overlaps(genomes, genome))
            //        {
            //            genomes.Add(genome);
            //        }
            //        parent2_genomes.Remove(genome);
            //    }
            //}

            //// list the rest of the cities and divide them into new routes.
            //List<int> rest = new List<int>();
            //for (int city_to_place = 0; city_to_place < solver.Problem.Cities; city_to_place++)
            //{
            //    if (!IndividualHelper.Overlaps(genomes, city_to_place))
            //    {
            //        rest.Add(city_to_place);
            //    }
            //}

            //// create the new individual.
            //new_individual.Initialize(genomes);
            //new_individual.CalculateFitness(
            //    solver.Problem,
            //    solver.FitnessCalculator,
            //    false);

            //// TODO: place the rest of the cities in existing rounds.
            //bool place_in_existing_succes = true;
            //while (place_in_existing_succes && rest.Count > 0)
            //{
            //    place_in_existing_succes = false;
            //    OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic.Helpers.BestPlacementHelper.BestPlacementResult best_result = null;
            //    for (int round_idx = 0; round_idx < genomes.Count; round_idx++)
            //    {
            //        if (new_individual.Fitness.LargestRoundCategories[round_idx] == 0)
            //        {
            //            Individual<List<Genome>, Problem, Fitness> copy = new_individual.Copy();

            //            OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic.Helpers.BestPlacementHelper.BestPlacementResult result =
            //                BestPlacementHelper.CalculateBestPlacementInGenome(
            //                    solver.Problem,
            //                    solver.FitnessCalculator as FitnessCalculator,
            //                    new_individual.Genomes[round_idx],
            //                    rest);

            //            // do the placement.
            //            IndividualHelper.PlaceInGenome(
            //                copy.Genomes[round_idx],
            //                result.CityIdx,
            //                result.City);

            //            // re-calculate fitness
            //            copy.CalculateFitness(solver.Problem, solver.FitnessCalculator,false);

            //            if (copy.Fitness.LargestRoundCategories[round_idx] == 0)
            //            {
            //                place_in_existing_succes = true;

            //                if (best_result == null
            //                    || best_result.Fitness > copy.Fitness)
            //                { // current placement is better.
            //                    result.RoundIdx = round_idx;
            //                    result.Fitness = copy.Fitness;

            //                    best_result = result;
            //                }
            //            }
            //        }
            //    }

            //    if(place_in_existing_succes)
            //    {
            //        IndividualHelper.PlaceInGenome(
            //            new_individual.Genomes[best_result.RoundIdx],
            //            best_result.CityIdx,
            //            best_result.City);

            //        rest.Remove(best_result.City);
            //    }
            //}

            //bool new_round = true;
            //Genome current_round = null;
            //double previous_time = 0;
            //while (rest.Count > 0)
            //{
            //    // create a new round if needed.
            //    int city;
            //    int city_idx;

            //    // force placement if less than half of the average round size remains.
            //    int size = solver.Problem.Cities / new_individual.Genomes.Count;
            //    if (new_round && rest.Count < size / 2)
            //    {
            //        new_individual = BestPlacementHelper.DoFast(
            //            solver.Problem,
            //            solver.FitnessCalculator as FitnessCalculator,
            //            new_individual,
            //            rest);

            //        rest.Clear();

            //        break;
            //    }

            //    if (new_round)
            //    {
            //        new_round = false;
            //        current_round = new Genome();
            //        genomes.Add(current_round);

            //        // select a random city to place.
            //        city_idx = StaticRandomGenerator.Get().Generate(rest.Count);
            //        city = rest[city_idx];
            //        rest.RemoveAt(city_idx);
            //        current_round.Add(city);

            //        previous_time = solver.Problem.TargetTime.Value;
            //    }

            //    if (rest.Count > 0)
            //    {
            //        // find the best city to place next.
            //        // calculate the best position to place the next city.
            //        BestPlacementHelper.BestPlacementResult new_position_to_place =
            //            BestPlacementHelper.CalculateBestPlacementInGenome(
            //                solver.Problem,
            //                solver.FitnessCalculator as FitnessCalculator,
            //                current_round,
            //                rest);

            //        city = new_position_to_place.City;

            //        // remove the node from the source list.
            //        rest.Remove(city);

            //        // place the node.
            //        current_round.Insert(new_position_to_place.CityIdx, new_position_to_place.City);

            //        // calculate the time.
            //        double time = (solver.FitnessCalculator as FitnessCalculator).CalculateTime(
            //            solver.Problem, current_round);

            //        if (solver.Problem.TargetTime.Value < time)
            //        { // time limit has been reached.
            //            double diff_average = time - solver.Problem.TargetTime.Value;
            //            double diff_previous = solver.Problem.TargetTime.Value - previous_time;

            //            if (diff_average > diff_previous)
            //            { // remove the last added city.
            //                current_round.Remove(city);
            //                rest.Add(city);
            //            }
            //            else
            //            { // keep the last city.

            //            }

            //            // keep the generated round.
            //            new_round = true;
            //        }
            //        previous_time = time;
            //    }
            //}

            //new_individual.CalculateFitness(
            //    solver.Problem,
            //    solver.FitnessCalculator);

            //return new_individual;
            throw new NotImplementedException("Not re-implemented after refactoring GA");
        }
    }
}
