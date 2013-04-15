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
using OsmSharp.Tools.Math.VRP.Core.BestPlacement;
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.Routing.VRP.NoDepot.MaxTime.Genetic.Mutation
{
    internal class ThreeOptMutation :
        IMutationOperation<MaxTimeSolution, MaxTimeProblem, Fitness>
    {
        public string Name
        {
            get
            {
                return "3Opt";
            }
        }

        /// <summary>
        /// "Mutates" a given individual by executing a local search 3-Opt operation.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="mutating"></param>
        /// <returns></returns>
        public Individual<MaxTimeSolution, MaxTimeProblem, Fitness> Mutate(
            Solver<MaxTimeSolution, MaxTimeProblem, Fitness> solver,
            Individual<MaxTimeSolution, MaxTimeProblem, Fitness> mutating)
        {
            // select a random route.
            MaxTimeSolution mutated = (mutating.Genomes.Clone() as MaxTimeSolution);
            IRoute route = mutated.Route(
                OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(mutated.Count));

            // apply the improvement operator.
            double difference;
            OsmSharp.Tools.Math.TSP.LocalSearch.HillClimbing3Opt.HillClimbing3OptSolver hillclimbing_3opt =
                new OsmSharp.Tools.Math.TSP.LocalSearch.HillClimbing3Opt.HillClimbing3OptSolver(true, true);
            hillclimbing_3opt.Improve(solver.Problem, route, out difference);

            return new Individual<MaxTimeSolution, MaxTimeProblem, Fitness>(
                mutated);
        }
    }
}
