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
    internal class RedivideFromLargeMutationOperation : IMutationOperation<List<Genome>, Problem, Fitness>
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

            //if (mutating.Genomes.Count > 2)
            //{
            //    // get from the largest round; place in the smallest round.
            //    if (!copy.FitnessCalculated)
            //    {
            //        copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);
            //    }
            //    Genome biggest = IndividualHelper.GetLargest(copy);
            //    int biggest_idx = copy.Genomes.IndexOf(biggest);

            //    // build a list of the other genomes.
            //    List<Genome> other_genomes = new List<Genome>();
            //    foreach (Genome other in copy.Genomes)
            //    {
            //        if (other != biggest)
            //        {
            //            other_genomes.Add(other);
            //        }
            //    }

            //    // remove customers until the target time is reached.
            //    double weight = copy.Fitness.Times[biggest_idx];
            //    while (weight > solver.Problem.TargetTime.Value)
            //    {
            //        // remove the city from the biggest.
            //        if (biggest.Count > 1)
            //        {
            //            // best place one of the largest cities into the smallest.
            //            OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic.Helpers.BestPlacementHelper.BestPlacementResult result =
            //                BestPlacementHelper.CalculateBestPlacementInGenomes(
            //                solver.Problem, (solver.FitnessCalculator as FitnessCalculator), other_genomes, biggest[biggest.Count - 1]);

            //            biggest.RemoveAt(biggest.Count - 1);

            //            // place the result.
            //            IndividualHelper.PlaceInGenome(other_genomes[result.RoundIdx],
            //                result.CityIdx,
            //                result.City);

            //            // recalculate fitness
            //            copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);

            //            // reset weight.
            //            weight = copy.Fitness.Times[biggest_idx];
            //        }
            //        else
            //        {
            //            break;
            //        }
            //    }

            //}
            //return copy;
            throw new NotImplementedException("Not re-implemented after refactoring GA");
        }
    }
}
