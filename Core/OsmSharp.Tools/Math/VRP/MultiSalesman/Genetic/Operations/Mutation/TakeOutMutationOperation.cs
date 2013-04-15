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
    internal class TakeOutMutationOperation : IMutationOperation<List<Genome>, Problem, Fitness>
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
            //// select a random round giving preference to the big ones.
            //Genome big_one = IndividualHelper.SelectRandom(copy, false);
            //if(big_one.Count <= 1)
            //{ // big_one has to have at least 2 customers to be able to remove one.
            //    // do nothing if this is the case
            //    return copy;
            //}

            //if (copy.Genomes.Count == 0)
            //{
            //    return copy;
            //}

            //// select a random customer from the big one and remove it.
            //int customer_idx = StaticRandomGenerator.Get().Generate(big_one.Count);
            //int customer = big_one[customer_idx];
            //big_one.RemoveAt(customer_idx);

            //// select all genomes except the big one.
            //List<Genome> genomes = IndividualHelper.Except(copy.Genomes, big_one);

            //// do best placement in all the other genomes.
            //genomes = BestPlacementHelper.DoFast(
            //    solver.Problem,
            //    (solver.FitnessCalculator as FitnessCalculator),
            //    genomes,
            //    customer);
            //copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            //return copy;

            throw new NotImplementedException("Not re-implemented after refactoring GA");
        }
    }
}
