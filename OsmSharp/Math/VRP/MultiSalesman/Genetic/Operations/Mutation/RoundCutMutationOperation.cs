// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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
using OsmSharp.Math.AI.Genetic;
using OsmSharp.Math.AI.Genetic.Operations;
using OsmSharp.Math.AI.Genetic.Solvers;
using OsmSharp.Math.Random;
using OsmSharp.Math.VRP.MultiSalesman.Genetic;
using OsmSharp.Math.VRP.MultiSalesman.Genetic.Helpers;

namespace OsmSharp.Math.VRP.MultiSalesman.Solver.Operations.Mutation
{
    internal class RoundCutMutationOperation : IMutationOperation<List<Genome>, Problem, Fitness>
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
            //Genome big = IndividualHelper.SelectRandom(copy, false);

            //// make sure there are at least two.
            //if (big.Count < 3)
            //{
            //    return copy;
            //}

            //// cut in two
            //int random_idx = StaticRandomGenerator.Get().Generate(
            //    big.Count - 2) + 1;

            //// remove the old round.
            //copy.Genomes.Remove(big);


            //// create new rounds.
            //Genome new_round1 = new Genome();
            //Genome new_round2 = new Genome();
            //new_round1.AddRange(
            //    big.GetRange(0, random_idx));
            //new_round2.AddRange(
            //    big.GetRange(random_idx, big.Count - random_idx));

            //// do best placement
            //random_idx = StaticRandomGenerator.Get().Generate(
            //    new_round1.Count);
            //Genome new_round1_1 = new Genome();
            //new_round1_1.Add(new_round1[random_idx]);
            //new_round1.RemoveAt(random_idx);
            //new_round1_1 = BestPlacementHelper.DoFast(
            //    solver.Problem,
            //    solver.FitnessCalculator as FitnessCalculator,
            //    new_round1_1,
            //    new_round1);
            //random_idx = StaticRandomGenerator.Get().Generate(
            //    new_round2.Count);  
            //Genome new_round2_1 = new Genome();
            //new_round2_1.Add(new_round2[random_idx]);
            //new_round2.RemoveAt(random_idx);
            //new_round2_1 = BestPlacementHelper.DoFast(
            //    solver.Problem,
            //    solver.FitnessCalculator as FitnessCalculator,
            //    new_round2_1,
            //    new_round2);

            //copy.Genomes.Add(new_round1_1);
            //copy.Genomes.Add(new_round2_1);

            //copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);

            //return copy;
            throw new NotImplementedException("Not re-implemented after refactoring GA");
        }
    }
}
