using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.VRP.Core.BestPlacement;
using Tools.Math.VRP.Core.Routes;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime.Genetic.Mutation
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
                Tools.Math.Random.StaticRandomGenerator.Get().Generate(mutated.Count));

            // apply the improvement operator.
            float difference;
            Tools.Math.TSP.LocalSearch.HillClimbing3Opt.HillClimbing3OptSolver hillclimbing_3opt =
                new Tools.Math.TSP.LocalSearch.HillClimbing3Opt.HillClimbing3OptSolver(true, true);
            hillclimbing_3opt.Improve(solver.Problem, route, out difference);

            return new Individual<MaxTimeSolution, MaxTimeProblem, Fitness>(
                mutated);
        }
    }
}
