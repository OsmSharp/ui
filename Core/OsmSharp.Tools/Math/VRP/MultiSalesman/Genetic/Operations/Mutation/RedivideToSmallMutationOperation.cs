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
using OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic.Helpers;
using OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic;

namespace OsmSharp.Tools.Math.VRP.MultiSalesman.Solver.Operations.Mutation
{
    internal class RedivideToSmallMutationOperation : IMutationOperation<List<Genome>, Problem, Fitness>
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
            //    Genome smallest = IndividualHelper.GetSmallest(copy);
            //    int smallest_idx = copy.Genomes.IndexOf(smallest);

            //    // build a list of the rest of the cities.
            //    List<int> cities = new List<int>();
            //    for (int city_to_place = 0; city_to_place < solver.Problem.Cities; city_to_place++)
            //    {
            //        if (!smallest.Contains(city_to_place))
            //        {
            //            cities.Add(city_to_place);
            //        }
            //    }

            //    // remove customers until the target time is reached.
            //    double weight = copy.Fitness.Times[smallest_idx];
            //    while (weight < solver.Problem.TargetTime.Value)
            //    {
            //        // best place one of the largest cities into the smallest.
            //        OsmSharp.Tools.Math.VRP.MultiSalesman.Genetic.Helpers.BestPlacementHelper.BestPlacementResult result =
            //            BestPlacementHelper.CalculateBestPlacementInGenome(
            //            solver.Problem, (solver.FitnessCalculator as FitnessCalculator), smallest, cities);

            //        // remove from largest/place in smallest after copying.
            //        bool empty = false;
            //        foreach (Genome genome in copy.Genomes)
            //        {
            //            if (genome.Remove(result.City))
            //            {
            //                if (genome.Count == 0)
            //                {
            //                    genome.Add(result.City);
            //                    empty = true;
            //                }
            //                break;
            //            }
            //        }
            //        if (!empty)
            //        {
            //            IndividualHelper.PlaceInGenome(smallest, result.CityIdx, result.City);

            //            // recalculate fitness
            //            copy.CalculateFitness(solver.Problem, solver.FitnessCalculator);

            //            // reset weight.
            //            weight = copy.Fitness.Times[smallest_idx];
            //        }


            //        cities.Remove(result.City);
            //    }
            //}
            //return copy;

            throw new NotImplementedException("Not re-implemented after refactoring GA");
        }
    }
}
