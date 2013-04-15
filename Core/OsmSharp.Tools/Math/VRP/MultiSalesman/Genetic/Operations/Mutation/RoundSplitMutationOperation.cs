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
    internal class RoundSplitMutationOperation : IMutationOperation<List<Genome>, Problem, Fitness>
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
            throw new NotImplementedException("Not re-implemented after refactoring GA");
            //Individual<List<Genome>, Problem, Fitness> copy = mutating.Copy();
            //if (!copy.FitnessCalculated)
            //{
            //    copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            //}

            //// randomly select a small genome.
            //Genome big = IndividualHelper.SelectRandom(copy, false);

            //// make sure there are at least two.
            //if (big.Count < 3)
            //{
            //    return copy;
            //}

            //// remove the old round.
            //copy.Genomes.Remove(big);

            //// create new rounds.
            //Genome new_round1 = new Genome();
            //Genome new_round2 = new Genome();
            //List<int> cities = big;

            //// place two random.
            //int city_idx = StaticRandomGenerator.Get().Generate(cities.Count);
            //int city = cities[city_idx];
            //cities.RemoveAt(city_idx);
            //new_round1.Add(city);
            //city_idx = StaticRandomGenerator.Get().Generate(cities.Count);
            //city = cities[city_idx];
            //cities.RemoveAt(city_idx);
            //new_round2.Add(city);

            //// use best placement.
            //List<Genome> genomes = new List<Genome>();
            //genomes.Add(new_round1);
            //genomes.Add(new_round2);

            //BestPlacementHelper.DoFast(
            //    solver.Problem,
            //    solver.FitnessCalculator as FitnessCalculator,
            //    genomes,
            //    cities);

            //// add the new rounds.
            //copy.Genomes.AddRange(genomes);

            //copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);

            //return copy;
        }
    }
}
