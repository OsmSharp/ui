using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools.Math.AI.Genetic.Operations.Mutations;
using Tools.Math.AI.Genetic;
using Tools.Math.AI.Genetic.Solvers;
using Tools.Math.VRP.Core.Routes;

namespace Osm.Routing.Core.VRP.NoDepot.MaxTime.Genetic.Mutation
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
                int source_route_idx = Tools.Math.Random.StaticRandomGenerator.Get().Generate(solution.Count);

                // get the source count and descide what to do.
                List<int> sizes = new List<int>(solution.Sizes);
                int source_size = solution.Sizes[source_route_idx];
                if (source_size > avg)
                {
                    int size = Tools.Math.Random.StaticRandomGenerator.Get().Generate(source_size / 2);

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
                        target_route_idx = Tools.Math.Random.StaticRandomGenerator.Get().Generate(solution.Sizes.Count);
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
                        foreach (int nn in problem.Get10NearestNeighbours(previous))
                        {
                            if (!used[nn])
                            {
                                current = nn;
                                break;
                            }
                        }
                        while (used[current])
                        {
                            current++;

                            if (current == solution.Size)
                            {
                                current = 0;
                            }
                        }
                    }

                    // set the next array.
                    route.Insert(previous, current, route.First);
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
