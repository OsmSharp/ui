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
    internal class RoundMergeMutationOperation : IMutationOperation<List<Genome>, Problem, Fitness>
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

            //if (!copy.FitnessCalculated)
            //{
            //    copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            //}
            //// randomly select a small genome.
            //Genome smallest1 = IndividualHelper.SelectRandom(copy, true);
            //Genome smallest2 = IndividualHelper.SelectRandom(copy, true);

            //// make sure the two are different.
            //if (smallest1 == smallest2)
            //{
            //    return copy;
            //}

            //// remove the old rounds.
            //copy.Genomes.Remove(smallest1);
            //copy.Genomes.Remove(smallest2);

            //// create new round.
            //Genome new_round = new Genome();
            //List<int> cities = smallest1;
            //cities.AddRange(smallest2);

            //// place one random.
            //int city_idx = StaticRandomGenerator.Get().Generate(cities.Count);
            //int city = cities[city_idx];
            //cities.RemoveAt(city_idx);
            //new_round.Add(city);

            //// use best placement.
            //BestPlacementHelper.DoFast(
            //    solver.Problem,
            //    solver.FitnessCalculator as FitnessCalculator,
            //    new_round,
            //    cities);

            //// add the new round
            //copy.Genomes.Add(new_round);

            //copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);

            //return copy;

            throw new NotImplementedException("Not re-implemented after refactoring GA");
        }
    }
}
