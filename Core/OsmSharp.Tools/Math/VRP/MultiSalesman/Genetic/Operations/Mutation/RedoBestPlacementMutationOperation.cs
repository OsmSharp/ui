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
using OsmSharp.Tools.Math.AI.Genetic.Operations.Mutations;
using OsmSharp.Tools.Math.AI.Genetic;
using OsmSharp.Tools.Math.AI.Genetic.Solvers;
using OsmSharp.Tools.Math.Random;
using OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic;
using OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic.Helpers;

namespace OsmSharp.Tools.Math.VRP.MultiSalesman.Solver.Operations.Mutation
{
    internal class RedoBestPlacementMutationOperation : IMutationOperation<List<Genome>, Problem, Fitness>
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
            //// search for the best customer exchange.
            //int round1_idx = StaticRandomGenerator.Get().Generate(copy.Genomes.Count);
            //int round2_idx = round1_idx;
            //while (round2_idx == round1_idx)
            //{
            //    round2_idx = StaticRandomGenerator.Get().Generate(copy.Genomes.Count);
            //}

            //// list all cities.
            //List<int> cities = new List<int>(
            //    copy.Genomes[round1_idx]);
            //cities.AddRange(
            //    copy.Genomes[round2_idx]);

            //// redo best placement.
            //List<Genome> new_rounds = new List<Genome>();
            //// place one random city in each round.
            //IRandomGenerator random = StaticRandomGenerator.Get();
            //for (int round_idx = 0; round_idx < 2; round_idx++)
            //{
            //    // select a random city to place.
            //    int city_idx = random.Generate(cities.Count);
            //    int city = cities[city_idx];
            //    cities.RemoveAt(city_idx);

            //    // create new genome.
            //    Genome genome = new Genome();
            //    genome.Add(city);
            //    new_rounds.Add(genome);
            //}

            //// best-place the rest.            
            //new_rounds = BestPlacementHelper.DoFast(
            //    solver.Problem,
            //    (solver.FitnessCalculator as FitnessCalculator),
            //    new_rounds,
            //    cities);

            //// replace old genomes
            //copy.Genomes[round1_idx] = new_rounds[0];
            //copy.Genomes[round2_idx] = new_rounds[1];

            //copy.Validate(solver.Problem);

            //// return the copied.
            //return copy;

            throw new NotImplementedException();
        }
    }
}
