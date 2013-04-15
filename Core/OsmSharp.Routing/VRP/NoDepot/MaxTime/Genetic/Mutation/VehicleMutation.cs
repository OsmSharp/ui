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
using OsmSharp.Tools.Math.VRP.Core.Routes;

namespace OsmSharp.Routing.VRP.NoDepot.MaxTime.Genetic.Mutation
{
    /// <summary>
    /// Mutation operation exchanging a part of some route to a part of another route.
    /// </summary>
    internal class VehicleMutation :
        IMutationOperation<MaxTimeSolution, MaxTimeProblem, Fitness>
    {
        /// <summary>
        /// Returns the name of this mutation.
        /// </summary>
        public string Name
        {
            get
            {
                return "VM";
            }
        }

        /// <summary>
        /// Does the mutation.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="mutating"></param>
        /// <returns></returns>
        public Individual<MaxTimeSolution, MaxTimeProblem, Fitness> Mutate(
            Solver<MaxTimeSolution, MaxTimeProblem, Fitness> solver,
            Individual<MaxTimeSolution, MaxTimeProblem, Fitness> mutating)
        {
            MaxTimeSolution solution = (mutating.Genomes.Clone() as MaxTimeSolution);
            //MaxTimeSolution copy = (mutating.Genomes.Clone() as MaxTimeSolution);

            if (solution.Count > 1)
            {
                // calculate average.
                int avg = solution.Size / solution.Count;

                // 
                int source_route_idx = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(solution.Count);

                // get the source count and descide what to do.
                List<int> sizes = new List<int>(solution.Sizes);
                int source_size = solution.Sizes[source_route_idx];
                if (source_size > avg)
                {
                    int size = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(source_size / 2);
                    //int size = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(source_size);
                    //int size = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(source_size / 3);

                    if (size > 0)
                    {
                        sizes[source_route_idx] = sizes[source_route_idx] - size;
                    }
                    sizes.Insert(source_route_idx, size);
                }
                else
                { // route is small; add it to a neighbour route.
                    int target_route_idx = -1;
                    do
                    {
                        target_route_idx = OsmSharp.Tools.Math.Random.StaticRandomGenerator.Get().Generate(solution.Sizes.Count);
                    } while (source_route_idx == target_route_idx);

                    if (target_route_idx >= 0 && target_route_idx < sizes.Count)
                    {
                        // merge the two into the target;
                        int size = sizes[source_route_idx];
                        sizes[target_route_idx] = sizes[target_route_idx] + size;
                        sizes.RemoveAt(source_route_idx);
                    }
                }

                // remove all zero's.
                while (sizes.Remove(0))
                {

                }
                solution = this.ChangeSizes(solver.Problem, solution, sizes);

                // apply 3Opt to all routes.
                for (int route_idx = 0; route_idx < solution.Count; route_idx++)
                {
                    double difference;
                    OsmSharp.Tools.Math.TSP.LocalSearch.HillClimbing3Opt.HillClimbing3OptSolver hillclimbing_3opt =
                        new OsmSharp.Tools.Math.TSP.LocalSearch.HillClimbing3Opt.HillClimbing3OptSolver(true, true);
                    hillclimbing_3opt.Improve(solver.Problem, solution.Route(route_idx), out difference);
                }

                if (!solution.IsValid())
                {
                    throw new Exception();
                }
            }

            return new Individual<MaxTimeSolution,MaxTimeProblem,Fitness>(solution);
        }


        public MaxTimeSolution ChangeSizes(MaxTimeProblem problem, MaxTimeSolution solution, IEnumerable<int> sizes)
        {
            MaxTimeSolution changed_solution = new MaxTimeSolution(
                solution.Size, true);
            //int[] next_array = new int[solution.Size];
            bool[] used = new bool[solution.Size];
            //List<int> first = new List<int>();
            foreach (int size in sizes)
            {
                int current = 0;
                while (used[current])
                {
                    current++;
                }
                IRoute route = changed_solution.Add(current);
                used[current] = true;
                int current_size = size - 1;
                int previous = current;
                while (current_size > 0)
                {
                    // choose the next customer.
                    current = solution.Next(current);
                    if (used[current])
                    {
                        double neighbour_weight = double.MaxValue;
                        foreach (int nn in problem.Get10NearestNeighbours(previous))
                        {
                            if (!used[nn])
                            {
                                double potential_weight =
                                    problem.WeightMatrix[previous][current];
                                if (potential_weight < neighbour_weight)
                                {
                                    current = nn;
                                    neighbour_weight = potential_weight;
                                }
                                //break;
                            }
                        }
                        int used_count = 0;
                        while (used[current])
                        {
                            current++;
                            used_count++;

                            if (current == solution.Size)
                            {
                                current = 0;
                            }

                            //if (used_count == problem.Size)
                            //{
                            //    break;
                            //}
                        }
                    }

                    // set the next array.
                    //route.InsertAfterAndRemove(previous, current, route.First);
                    route.InsertAfter(previous, current);
                    used[current] = true;
                    previous = current;
                    current_size--;
                }

                if (!changed_solution.IsValid())
                {
                    throw new Exception();
                }
            }
            return changed_solution;
        }

    }
}
