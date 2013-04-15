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
    internal class RoundRedivideMutationOperation : IMutationOperation<List<Genome>, Problem, Fitness>
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

            //if (copy.Genomes.Count <= 1)
            //{
            //    return copy;
            //}

            //if (!copy.FitnessCalculated)
            //{
            //    copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            //}
            //// randomly select a small genome.
            //Genome smallest1 = IndividualHelper.SelectRandom(copy, true);
            //copy.Genomes.Remove(smallest1);

            //// use best placement.
            //copy = BestPlacementHelper.DoFast(
            //    solver.Problem,
            //    solver.FitnessCalculator as FitnessCalculator,
            //    copy as Individual,
            //    smallest1);

            //copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);

            //return copy;
        }
    }
}
