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
using OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic;
using OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic.Helpers;

namespace OsmSharp.Tools.Math.VRP.MultiSalesman.Solver.Operations.Mutation
{
    internal class DefaultMutationOperation : IMutationOperation<List<Genome>, Problem, Fitness>
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
            //// get from the largest round; place in the smallest round.
            //Genome smallest = IndividualHelper.GetSmallest(mutating);
            //int smallest_idx = mutating.Genomes.IndexOf(smallest);
            //Genome largest = IndividualHelper.GetLargest(mutating);
            //int largest_idx = mutating.Genomes.IndexOf(largest);

            //// best place one of the largest cities into the smallest.
            //if (!mutating.FitnessCalculated)
            //{
            //    mutating.CalculateFitness(
            //        solver.Problem,
            //        solver.FitnessCalculator);
            //}
            //Tools.Math.VRP.MultiSalesman.Genetic.Helpers.BestPlacementHelper.BestPlacementResult result = 
            //    BestPlacementHelper.CalculateBestPlacementInGenome(
            //    solver.Problem, (solver.FitnessCalculator as FitnessCalculator), smallest, largest);

            //// remove from largest/place in smallest after copying.
            //Individual<List<Genome>, Problem, Fitness> copy = mutating.Copy();
            //largest = copy.Genomes[largest_idx];
            //largest.Remove(result.City);
            //smallest = copy.Genomes[smallest_idx];
            //IndividualHelper.PlaceInGenome(smallest,result.CityIdx,result.City);

            //// recalculate fitness.
            ////copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);

            //return copy;

            throw new NotImplementedException();
        }
    }
}
