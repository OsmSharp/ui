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
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Math.AI.Genetic.Operations.Mutations;
using OsmSharp.Tools.Math.Random;
using OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic;
using OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic.Helpers;

namespace OsmSharp.Tools.Math.VRP.MultiSalesman.Solver.Operations.Mutation
{
    internal class RoundReduceMultipleMutationOperation : IMutationOperation<List<Genome>, Problem, Fitness>
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
            //// randomly select a big genome.
            //Genome big1 = IndividualHelper.SelectRandom(copy, false);
            //double average = copy.Fitness.TotalTime / copy.Genomes.Count;
            //List<Genome> big_ones = new List<Genome>();
            //for(int idx=0;idx<copy.Genomes.Count;idx++)
            //{
            //    if (copy.Fitness.Times[idx] > average
            //        && copy.Genomes[idx] != big1)
            //    {
            //        big_ones.Add(copy.Genomes[idx]);
            //    }
            //}

            //int round_idx = -1;
            //if (big_ones.Count == 0)
            //{
            //    copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            //    return copy;
            //}
            //else if (big_ones.Count == 1)
            //{
            //    round_idx = 0;
            //}
            //else
            //{

            //    // select a big one nearest to the orignal big one.
            //    double lowest_weight = double.MaxValue;
            //    for (int big1_idx = 0; big1_idx < big1.Count; big1_idx++)
            //    {
            //        for (int big_round_idx = 0; big_round_idx < big_ones.Count; big_round_idx++)
            //        {
            //            Genome big_temp = big_ones[big_round_idx];
            //            for (int big_temp_idx = 0; big_temp_idx < big_temp.Count; big_temp_idx++)
            //            {
            //                double weight = solver.Problem.Weight(
            //                    big1[big1_idx], big_temp[big_temp_idx]);
            //                if (weight < lowest_weight)
            //                {
            //                    lowest_weight = weight;
            //                    round_idx = big_round_idx;
            //                }
            //            }
            //        }
            //    }
            //}
            //Genome big2 = big_ones[round_idx];

            //copy.Genomes.Remove(big1);
            //copy.Genomes.Remove(big2);

            //// redivide into three others.
            //List<int> cities_to_place = big1;
            //big1.AddRange(big2);
            //Genome round1 = new Genome();
            //int random_idx = StaticRandomGenerator.Get().Generate(
            //    cities_to_place.Count);
            //round1.Add(cities_to_place[random_idx]);
            //cities_to_place.RemoveAt(random_idx);
            //Genome round2 = new Genome();
            //random_idx = StaticRandomGenerator.Get().Generate(
            //    cities_to_place.Count);
            //round2.Add(cities_to_place[random_idx]);
            //cities_to_place.RemoveAt(random_idx);
            //Genome round3 = new Genome();
            //random_idx = StaticRandomGenerator.Get().Generate(
            //    cities_to_place.Count);
            //round3.Add(cities_to_place[random_idx]);
            //cities_to_place.RemoveAt(random_idx);

            //List<Genome> targets = new List<Genome>();
            //targets.Add(round1);
            //targets.Add(round2);
            //targets.Add(round3);

            //// use best placement.
            //BestPlacementHelper.DoFast(
            //    solver.Problem,
            //    solver.FitnessCalculator as FitnessCalculator,
            //    targets,
            //    cities_to_place);

            //copy.Genomes.AddRange(
            //    targets);
            //copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);

            //return copy;
            throw new NotImplementedException();
        }
    }
}
